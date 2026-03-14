using System;
using System.Text.Json;
using BibXmr.Client.Network.Exceptions;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Backwards-compatible JSON-RPC exception for callers that already depend on <see cref="JsonRpcException"/>.
    /// </summary>
    public class JsonRpcException : MoneroRpcRemoteException
    {
        /// <summary>
        /// Create a new exception instance.
        /// </summary>
        public JsonRpcException()
            : base((int)JsonRpcErrorCode.UnknownError, null, null, message: nameof(JsonRpcException))
        {
        }

        /// <summary>
        /// Create a new exception instance with a message.
        /// </summary>
        public JsonRpcException(string message)
            : base((int)JsonRpcErrorCode.UnknownError, null, null, message: message)
        {
        }

        /// <summary>
        /// Create a new exception instance with a message and an inner exception.
        /// </summary>
        public JsonRpcException(string message, Exception innerException)
            : base((int)JsonRpcErrorCode.UnknownError, null, null, message: message, innerException: innerException)
        {
        }

        /// <summary>
        /// Create a new exception instance with a message and a known <see cref="Network.JsonRpcErrorCode"/>.
        /// </summary>
        public JsonRpcException(string message, JsonRpcErrorCode jsonRpcErrorCode)
            : base((int)jsonRpcErrorCode, null, null, message: message)
        {
            JsonRpcErrorCode = jsonRpcErrorCode;
        }

        /// <summary>
        /// Creates a fully-populated JSON-RPC exception used internally when the daemon or wallet returns an error response.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="jsonRpcErrorCode">The coarse JSON-RPC error classification.</param>
        /// <param name="code">The raw numeric error code from the JSON-RPC response.</param>
        /// <param name="remoteMessage">The error message reported by the remote RPC endpoint.</param>
        /// <param name="data">Optional additional JSON data from the error response.</param>
        /// <param name="rpcMethod">The RPC method that triggered the error.</param>
        /// <param name="requestId">The JSON-RPC request id.</param>
        /// <param name="requestUri">The URI of the RPC endpoint.</param>
        /// <param name="innerException">An optional inner exception.</param>
        internal JsonRpcException(string message, JsonRpcErrorCode jsonRpcErrorCode, int code, string? remoteMessage, JsonElement? data, string? rpcMethod, string? requestId, Uri? requestUri, Exception? innerException = null)
            : base(code, remoteMessage, data, message: message, innerException: innerException, rpcMethod: rpcMethod, requestId: requestId, requestUri: requestUri)
        {
            JsonRpcErrorCode = jsonRpcErrorCode;
        }

        /// <summary>
        /// A coarse error classification for the JSON-RPC error (when known).
        /// </summary>
        public JsonRpcErrorCode JsonRpcErrorCode { get; private set; } = JsonRpcErrorCode.UnknownError;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(JsonRpcException)} - {JsonRpcErrorCode}";
        }
    }
}
