using System;
using System.Text.Json;

namespace BibXmr.Client.Network.Exceptions
{
    /// <summary>
    /// Raised when the remote JSON-RPC endpoint returns an error object.
    /// </summary>
    /// <remarks>
    /// Create a new exception representing a remote JSON-RPC error response.
    /// </remarks>
    public class MoneroRpcRemoteException(int code, string? remoteMessage, JsonElement? data, string message, Exception? innerException = null, string? rpcMethod = null, string? requestId = null, Uri? requestUri = null) : MoneroRpcException(message, innerException, rpcMethod, requestId, requestUri)
    {
        /// <summary>
        /// The numeric error code returned by the remote JSON-RPC endpoint.
        /// </summary>
        public int Code { get; } = code;

        /// <summary>
        /// The error message returned by the remote endpoint (when available).
        /// </summary>
        public string? RemoteMessage { get; } = remoteMessage;

        /// <summary>
        /// The remote error data object (when available).
        /// </summary>
        public new JsonElement? Data { get; } = data;
    }
}
