using System;
#if NET8_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Configuration for Monero RPC HTTP/JSON-RPC behavior (timeouts, diagnostics).
    /// </summary>
    public sealed class MoneroRpcClientOptions
    {
        /// <summary>
        /// Factory used to generate a JSON-RPC request id for each request.
        /// </summary>
        public Func<string> RequestIdFactory { get; set; } = static () => Guid.NewGuid().ToString("N");

        /// <summary>
        /// Maximum time allowed per RPC request. Default is 100 seconds.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);

        /// <summary>
        /// Maximum number of characters captured from a non-success HTTP response body.
        /// </summary>
        public int MaxResponseBodySnippetChars { get; set; } = 4096;

        /// <summary>
        /// Allows clear-text HTTP for non-loopback hosts.
        /// Default is <see langword="false"/>, which blocks non-loopback <c>http://</c> endpoints.
        /// Enable only when you fully trust the network path.
        /// </summary>
        public bool UnsafeAllowClearTextHttpOnNonLoopback { get; set; } = false;

#if NET8_0_OR_GREATER
        /// <summary>
        /// Optional resolver appended after Client's built-in JSON metadata resolvers.
        /// Use this to register application-specific DTO metadata for AOT-safe serialization.
        /// </summary>
        public IJsonTypeInfoResolver? AdditionalTypeInfoResolver { get; set; }
#endif
    }
}
