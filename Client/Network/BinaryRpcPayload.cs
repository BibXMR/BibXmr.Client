using System;
using System.Collections.Generic;
using System.Net;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Represents a raw binary response returned by a daemon binary RPC endpoint.
    /// </summary>
    public sealed class BinaryRpcPayload
    {
        /// <summary>
        /// Gets the HTTP status code returned by the daemon.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets the response content type if available.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Gets response headers keyed by header name.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers { get; set; } =
            new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the raw response body bytes.
        /// </summary>
        public byte[] Body { get; set; } = Array.Empty<byte>();
    }
}
