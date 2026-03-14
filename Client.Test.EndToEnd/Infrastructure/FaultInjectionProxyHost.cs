using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace BibXmr.Client.Test.EndToEnd.Infrastructure;

/// <summary>
/// Well-known proxy mode names used by the local fault-injection server.
/// </summary>
internal static class ProxyModes
{
    /// <summary>
    /// Forwards the request to the live daemon without modification.
    /// </summary>
    public const string Pass = "pass";

    /// <summary>
    /// Rewrites the JSON-RPC method name to an invalid method.
    /// </summary>
    public const string RewriteInvalidMethod = "rewrite-invalid-method";

    /// <summary>
    /// Forwards the request to a daemon path that does not exist.
    /// </summary>
    public const string Upstream404 = "upstream-404";

    /// <summary>
    /// Returns HTTP 403 without calling the daemon.
    /// </summary>
    public const string Status403 = "status-403";

    /// <summary>
    /// Returns HTTP 500 without calling the daemon.
    /// </summary>
    public const string Status500 = "status-500";

    /// <summary>
    /// Returns HTTP 200 with malformed JSON payload.
    /// </summary>
    public const string MalformedJson = "malformed-json";

    /// <summary>
    /// Delays the response long enough to trigger client timeout.
    /// </summary>
    public const string Slow = "slow";

    /// <summary>
    /// Returns HTTP 503 once, then forwards subsequent calls.
    /// </summary>
    public const string Transient503Once = "transient-503-once";
}

/// <summary>
/// Hosts a local HTTP reverse proxy that forwards to a live daemon and can inject deterministic faults.
/// </summary>
internal sealed class FaultInjectionProxyHost : IAsyncDisposable
{
    private readonly MoneroE2EOptions _options;
    private readonly ILogger<FaultInjectionProxyHost> _logger;
    private readonly ConcurrentDictionary<string, int> _transientCounters = new(StringComparer.Ordinal);

    private WebApplication? _app;
    private HttpClientHandler? _upstreamHandler;
    private HttpClient? _upstreamClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="FaultInjectionProxyHost"/> class.
    /// </summary>
    /// <param name="options">The E2E runtime options.</param>
    /// <param name="logger">A logger instance.</param>
    public FaultInjectionProxyHost(MoneroE2EOptions options, ILogger<FaultInjectionProxyHost> logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the proxy base address after the host is started.
    /// </summary>
    public Uri BaseAddress { get; private set; } = new("http://127.0.0.1/", UriKind.Absolute);

    /// <summary>
    /// Starts the proxy host.
    /// </summary>
    /// <param name="token">An optional cancellation token.</param>
    public async Task StartAsync(CancellationToken token = default)
    {
        if (_app != null)
            return;

        _upstreamHandler = new HttpClientHandler();

        if (_options.AllowInsecureTls)
            _upstreamHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        _upstreamClient = new HttpClient(_upstreamHandler)
        {
            Timeout = TimeSpan.FromSeconds(Math.Max(30, _options.DefaultTimeoutSeconds * 2)),
        };

        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Production,
        });

        builder.WebHost.UseUrls("http://127.0.0.1:0");

        WebApplication app = builder.Build();

        app.Map("/{mode}/{**rpcPath}", HandleProxyAsync);
        app.MapGet("/healthz", () => Results.Ok("ok"));

        _app = app;

        await app.StartAsync(token).ConfigureAwait(false);

        string? boundAddress = app.Urls.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(boundAddress))
            throw new InvalidOperationException("Proxy host did not expose a bound address.");

        BaseAddress = EnsureTrailingSlash(new Uri(boundAddress, UriKind.Absolute));
        _logger.LogInformation("Fault-injection proxy started at {BaseAddress}", BaseAddress);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_app != null)
        {
            try
            {
                await _app.StopAsync().ConfigureAwait(false);
            }
            finally
            {
                await _app.DisposeAsync().ConfigureAwait(false);
                _app = null;
            }
        }

        _upstreamClient?.Dispose();
        _upstreamClient = null;

        _upstreamHandler?.Dispose();
        _upstreamHandler = null;
    }

    private async Task HandleProxyAsync(HttpContext context)
    {
        string mode = context.Request.RouteValues["mode"]?.ToString() ?? ProxyModes.Pass;
        string rpcPath = context.Request.RouteValues["rpcPath"]?.ToString() ?? "json_rpc";

        _logger.LogInformation("Proxy mode {Mode} handling {Method} {Path}", mode, context.Request.Method, rpcPath);

        switch (mode)
        {
            case ProxyModes.Status403:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;

            case ProxyModes.Status500:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return;

            case ProxyModes.MalformedJson:
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"broken\":", Encoding.UTF8, context.RequestAborted).ConfigureAwait(false);
                return;

            case ProxyModes.Slow:
                await Task.Delay(TimeSpan.FromSeconds(3), context.RequestAborted).ConfigureAwait(false);
                break;

            case ProxyModes.Transient503Once:
            {
                int hit = _transientCounters.AddOrUpdate(mode, 1, (_, count) => count + 1);
                if (hit == 1)
                {
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    return;
                }
                break;
            }

            case ProxyModes.Pass:
            case ProxyModes.RewriteInvalidMethod:
            case ProxyModes.Upstream404:
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync($"Unknown proxy mode '{mode}'.", context.RequestAborted).ConfigureAwait(false);
                return;
        }

        await ForwardToDaemonAsync(context, mode, rpcPath).ConfigureAwait(false);
    }

    private async Task ForwardToDaemonAsync(HttpContext context, string mode, string rpcPath)
    {
        if (_upstreamClient == null)
            throw new InvalidOperationException("Proxy is not initialized.");

        string effectivePath = mode == ProxyModes.Upstream404 ? "not_a_real_path" : rpcPath;

        byte[] requestBody = await ReadRequestBodyAsync(context.Request, context.RequestAborted).ConfigureAwait(false);

        if (mode == ProxyModes.RewriteInvalidMethod)
            requestBody = RewriteMethodToInvalid(requestBody);

        Uri upstreamUri = new(_options.DaemonBaseAddress, effectivePath + context.Request.QueryString.Value);

        using HttpRequestMessage upstreamRequest = new(new HttpMethod(context.Request.Method), upstreamUri)
        {
            Content = new ByteArrayContent(requestBody),
        };

        if (!string.IsNullOrWhiteSpace(context.Request.ContentType))
            upstreamRequest.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(context.Request.ContentType);

        if (context.Request.Headers.TryGetValue("Accept", out StringValues acceptValues))
            upstreamRequest.Headers.TryAddWithoutValidation("Accept", (IEnumerable<string>)acceptValues);

        using HttpResponseMessage upstreamResponse = await _upstreamClient
            .SendAsync(upstreamRequest, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted)
            .ConfigureAwait(false);

        context.Response.StatusCode = (int)upstreamResponse.StatusCode;

        foreach (KeyValuePair<string, IEnumerable<string>> header in upstreamResponse.Headers)
            context.Response.Headers[header.Key] = header.Value.ToArray();

        foreach (KeyValuePair<string, IEnumerable<string>> header in upstreamResponse.Content.Headers)
            context.Response.Headers[header.Key] = header.Value.ToArray();

        context.Response.Headers.Remove("transfer-encoding");

        await upstreamResponse.Content.CopyToAsync(context.Response.Body, context.RequestAborted).ConfigureAwait(false);
    }

    private static async Task<byte[]> ReadRequestBodyAsync(HttpRequest request, CancellationToken token)
    {
        if (request.Body == null)
            return Array.Empty<byte>();

        using MemoryStream ms = new();
        await request.Body.CopyToAsync(ms, token).ConfigureAwait(false);
        return ms.ToArray();
    }

    private static byte[] RewriteMethodToInvalid(byte[] requestBody)
    {
        if (requestBody.Length == 0)
            return requestBody;

        try
        {
            using JsonDocument document = JsonDocument.Parse(requestBody);
            JsonElement root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
                return requestBody;

            using MemoryStream stream = new();
            using (Utf8JsonWriter writer = new(stream))
            {
                writer.WriteStartObject();

                foreach (JsonProperty property in root.EnumerateObject())
                {
                    if (string.Equals(property.Name, "method", StringComparison.Ordinal))
                        writer.WriteString("method", "definitely_not_a_method");
                    else
                        property.WriteTo(writer);
                }

                writer.WriteEndObject();
            }

            return stream.ToArray();
        }
        catch (JsonException)
        {
            return requestBody;
        }
    }

    private static Uri EnsureTrailingSlash(Uri uri)
    {
        if (uri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal))
            return uri;

        return new Uri(uri.AbsoluteUri + "/", UriKind.Absolute);
    }
}

