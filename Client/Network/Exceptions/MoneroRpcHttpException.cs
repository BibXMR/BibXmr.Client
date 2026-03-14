using System;
using System.Net;

namespace BibXmr.Client.Network.Exceptions
{
    /// <summary>
    /// Raised when the HTTP response status is non-success.
    /// </summary>
    /// <remarks>
    /// Create a new exception representing a non-success HTTP response.
    /// </remarks>
    public sealed class MoneroRpcHttpException(HttpStatusCode statusCode, string? responseBodySnippet, string message, Exception? innerException = null, string? rpcMethod = null, string? requestId = null, Uri? requestUri = null) : MoneroRpcException(message, innerException, rpcMethod, requestId, requestUri)
    {
        /// <summary>
        /// The HTTP status code returned by the remote endpoint.
        /// </summary>
        public HttpStatusCode StatusCode { get; } = statusCode;

        /// <summary>
        /// A short snippet of the response body (when available), captured for diagnostics.
        /// </summary>
        public string? ResponseBodySnippet { get; } = responseBodySnippet;
    }
}
