using BibXmr.Client.Daemon;
using BibXmr.Client.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace BibXmr.Client.Test.EndToEnd.Infrastructure;

/// <summary>
/// Shared fixture that configures services and provides daemon client sessions for E2E tests.
/// </summary>
public sealed class MoneroE2EFixture : IAsyncLifetime
{
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// Gets the effective runtime options.
    /// </summary>
    internal MoneroE2EOptions Options { get; private set; } = new();

    /// <summary>
    /// Gets the local fault-injection proxy host.
    /// </summary>
    internal FaultInjectionProxyHost ProxyHost { get; private set; } = null!;

    /// <summary>
    /// Gets the logger factory.
    /// </summary>
    internal ILoggerFactory LoggerFactory =>
        _serviceProvider?.GetRequiredService<ILoggerFactory>()
        ?? throw new InvalidOperationException("Fixture has not been initialized.");

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        MoneroE2EOptions options = new();
        configuration.GetSection("MoneroE2E").Bind(options);
        options.Validate();
        Options = options;

        ServiceCollection serviceCollection = new();
        serviceCollection.AddSingleton(options);
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSimpleConsole(console =>
            {
                console.SingleLine = true;
                console.TimestampFormat = "HH:mm:ss ";
            });
            logging.SetMinimumLevel(LogLevel.Information);
        });

        _serviceProvider = serviceCollection.BuildServiceProvider();

        ILogger<FaultInjectionProxyHost> proxyLogger = _serviceProvider.GetRequiredService<ILogger<FaultInjectionProxyHost>>();
        ProxyHost = new FaultInjectionProxyHost(options, proxyLogger);
        await ProxyHost.StartAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        if (ProxyHost != null)
            await ProxyHost.DisposeAsync().ConfigureAwait(false);

        if (_serviceProvider is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        else
            _serviceProvider?.Dispose();
    }

    /// <summary>
    /// Creates a daemon client session that targets the live daemon directly.
    /// </summary>
    /// <param name="timeout">An optional per-request timeout override.</param>
    /// <param name="baseAddress">An optional daemon base address override.</param>
    internal Task<DaemonClientSession> CreateDirectSessionAsync(
        TimeSpan? timeout = null,
        Uri? baseAddress = null)
    {
        return CreateSessionAsync(
            baseAddress ?? Options.DaemonBaseAddress,
            timeout);
    }

    /// <summary>
    /// Creates a daemon client session that routes calls through the local fault proxy.
    /// </summary>
    /// <param name="mode">The proxy fault mode.</param>
    /// <param name="timeout">An optional per-request timeout override.</param>
    internal Task<DaemonClientSession> CreateProxySessionAsync(
        string mode,
        TimeSpan? timeout = null)
    {
        if (string.IsNullOrWhiteSpace(mode))
            throw new ArgumentException("Proxy mode is required.", nameof(mode));

        Uri proxyAddress = new(ProxyHost.BaseAddress, $"{mode}/");
        return CreateSessionAsync(proxyAddress, timeout);
    }

    /// <summary>
    /// Creates a logger for a test type.
    /// </summary>
    /// <typeparam name="T">The logger category type.</typeparam>
    internal ILogger<T> CreateLogger<T>() where T : class
    {
        return LoggerFactory.CreateLogger<T>();
    }

    private async Task<DaemonClientSession> CreateSessionAsync(
        Uri baseAddress,
        TimeSpan? timeout)
    {
        HttpClientHandler httpClientHandler = new();

        if (Options.AllowInsecureTls)
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        HttpClient httpClient = new(httpClientHandler);

        MoneroRpcClientOptions rpcOptions = new()
        {
            Timeout = timeout ?? TimeSpan.FromSeconds(Options.DefaultTimeoutSeconds),
        };

        MoneroDaemonClient daemonClient = await MoneroDaemonClient
            .CreateAsync(httpClient, baseAddress, rpcOptions)
            .ConfigureAwait(false);

        IReadOnlyList<IDisposable> disposables = new IDisposable[]
        {
            httpClientHandler,
        };

        return new DaemonClientSession(daemonClient, httpClient, disposables);
    }
}


