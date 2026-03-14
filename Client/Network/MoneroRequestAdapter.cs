using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Builds HTTP request messages for Monero JSON-RPC, path JSON, and binary RPC calls.
    /// </summary>
    internal class MoneroRequestAdapter
    {
        private readonly Uri _baseAddress;
        private readonly Func<string> _requestIdFactory;
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Initializes a new instance of the MoneroRequestAdapter class.
        /// </summary>
        /// <param name="url">The RPC endpoint URL.</param>
        /// <param name="port">The RPC endpoint port number.</param>
        /// <param name="requestIdFactory">The request id factory.</param>
        /// <param name="serializerOptions">The serializer options.</param>
        /// <param name="unsafeAllowClearTextHttpOnNonLoopback">
        /// Allows plain HTTP for non-loopback hosts when set to <see langword="true"/>.
        /// </param>
        public MoneroRequestAdapter(
            string url,
            uint port,
            Func<string>? requestIdFactory = null,
            JsonSerializerOptions? serializerOptions = null,
            bool unsafeAllowClearTextHttpOnNonLoopback = false)
        {
            if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                // Caller supplied an explicit scheme — strip any trailing slash, append port.
                string trimmed = url.TrimEnd('/');
                _baseAddress = new Uri($"{trimmed}:{port}/", UriKind.Absolute);
            }
            else
            {
                string hostPart = IPAddress.TryParse(url, out IPAddress? parsed) && parsed.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                    ? $"[{url}]"
                    : url;
                _baseAddress = new Uri($"http://{hostPart}:{port}/", UriKind.Absolute);
            }

            ValidateTransportPolicy(_baseAddress, unsafeAllowClearTextHttpOnNonLoopback, nameof(url));
            _requestIdFactory = requestIdFactory ?? DefaultRequestIdFactory;
            _serializerOptions = serializerOptions ?? MoneroJson.CreateSerializerOptions(MoneroJsonProfile.Combined);
        }

        /// <summary>
        /// Initializes a new instance of the MoneroRequestAdapter class.
        /// </summary>
        /// <param name="baseAddress">The base address of the RPC endpoint.</param>
        /// <param name="requestIdFactory">The request id factory.</param>
        /// <param name="serializerOptions">The serializer options.</param>
        /// <param name="unsafeAllowClearTextHttpOnNonLoopback">
        /// Allows plain HTTP for non-loopback hosts when set to <see langword="true"/>.
        /// </param>
        public MoneroRequestAdapter(
            Uri baseAddress,
            Func<string>? requestIdFactory = null,
            JsonSerializerOptions? serializerOptions = null,
            bool unsafeAllowClearTextHttpOnNonLoopback = false)
        {
            if (baseAddress == null)
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            if (!baseAddress.IsAbsoluteUri)
            {
                throw new ArgumentException("Base address must be an absolute URI.", nameof(baseAddress));
            }

            _baseAddress = baseAddress.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)
                ? baseAddress
                : new Uri($"{baseAddress.AbsoluteUri}/", UriKind.Absolute);
            ValidateTransportPolicy(_baseAddress, unsafeAllowClearTextHttpOnNonLoopback, nameof(baseAddress));
            _requestIdFactory = requestIdFactory ?? DefaultRequestIdFactory;
            _serializerOptions = serializerOptions ?? MoneroJson.CreateSerializerOptions(MoneroJsonProfile.Combined);
        }

        /// <summary>
        /// Creates json rpc request message.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="requestParams">The request params.</param>
        /// <returns>The operation result.</returns>
        public HttpRequestMessage CreateJsonRpcRequestMessage(string method, object? requestParams)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentException("RPC method is required.", nameof(method));
            }

            string normalizedMethod = method.Trim();
            string requestId = _requestIdFactory();
            var uri = new Uri(_baseAddress, "json_rpc");

            var payload = new AnonymousRequest
            {
                Jsonrpc = FieldAndHeaderDefaults.JsonRpc,
                Id = requestId,
                Method = normalizedMethod,
                Params = requestParams,
            };

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.SetMoneroRpcRequestId(requestId);
            message.SetMoneroRpcMethod(normalizedMethod);
            message.Content = new JsonHttpContent(payload, payload.GetType(), _serializerOptions);
            return message;
        }

        /// <summary>
        /// Creates path json request message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="requestBody">The request body.</param>
        /// <returns>The operation result.</returns>
        public HttpRequestMessage CreatePathJsonRequestMessage(string endpoint, object? requestBody)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException("Endpoint is required.", nameof(endpoint));
            }

            string trimmedEndpoint = endpoint.TrimStart('/');
            var uri = new Uri(_baseAddress, trimmedEndpoint);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.SetMoneroRpcMethod(trimmedEndpoint);
            object body = requestBody ?? new GenericRequestParameters();
            message.Content = new JsonHttpContent(body, body.GetType(), _serializerOptions);
            return message;
        }

        /// <summary>
        /// Creates path binary request message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="requestBody">The request body.</param>
        /// <returns>The operation result.</returns>
        public HttpRequestMessage CreatePathBinaryRequestMessage(string endpoint, ReadOnlyMemory<byte> requestBody)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException("Endpoint is required.", nameof(endpoint));
            }

            string trimmedEndpoint = endpoint.TrimStart('/');
            var uri = new Uri(_baseAddress, trimmedEndpoint);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.SetMoneroRpcMethod(trimmedEndpoint);
            var content = new ByteArrayContent(requestBody.ToArray());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            message.Content = content;
            return message;
        }

        /// <summary>
        /// Generates a unique request identifier using a GUID without hyphens.
        /// </summary>
        /// <returns>A 32-character hexadecimal request id string.</returns>
        private static string DefaultRequestIdFactory() => Guid.NewGuid().ToString("N");

        /// <summary>
        /// Validates that the transport security policy is satisfied; throws if plain HTTP is used on a non-loopback host without explicit opt-in.
        /// </summary>
        /// <param name="baseAddress">The resolved base address to validate.</param>
        /// <param name="unsafeAllowClearTextHttpOnNonLoopback">Whether the caller explicitly allows clear-text HTTP.</param>
        /// <param name="parameterName">The name of the originating parameter for exception messages.</param>
        /// <exception cref="ArgumentException">Thrown when clear-text HTTP is blocked for a non-loopback host.</exception>
        private static void ValidateTransportPolicy(Uri baseAddress, bool unsafeAllowClearTextHttpOnNonLoopback, string parameterName)
        {
            if (!string.Equals(baseAddress.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (unsafeAllowClearTextHttpOnNonLoopback)
            {
                return;
            }

            if (IsLoopbackHost(baseAddress.Host))
            {
                return;
            }

            throw new ArgumentException(
                "Clear-text HTTP is blocked for non-loopback endpoints. Use HTTPS or set MoneroRpcClientOptions.UnsafeAllowClearTextHttpOnNonLoopback=true.",
                parameterName);
        }

        /// <summary>
        /// Determines whether the given host name or IP address resolves to a loopback address.
        /// </summary>
        /// <param name="host">The host name or IP address string to check.</param>
        /// <returns><see langword="true"/> if the host is a loopback address; otherwise <see langword="false"/>.</returns>
        private static bool IsLoopbackHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return false;
            }

            if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (host.EndsWith(".localhost", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (IPAddress.TryParse(host, out IPAddress? ipAddress))
            {
                return IPAddress.IsLoopback(ipAddress);
            }

            return false;
        }
    }
}
