using System;

namespace BibXmr.Client.Network.Exceptions
{
    /// <summary>
    /// Base class for exceptions raised by Monero RPC communication and parsing.
    /// </summary>
    public class MoneroRpcException : Exception
    {
        /// <summary>
        /// Create a new exception instance.
        /// </summary>
        public MoneroRpcException()
        {
        }

        /// <summary>
        /// Create a new exception instance with a message.
        /// </summary>
        public MoneroRpcException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception instance with a message and an inner exception.
        /// </summary>
        public MoneroRpcException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Create a new exception instance with additional request context.
        /// </summary>
        protected MoneroRpcException(string message, Exception? innerException, string? rpcMethod, string? requestId, Uri? requestUri)
            : base(message, innerException)
        {
            RpcMethod = rpcMethod;
            RequestId = requestId;
            RequestUri = requestUri;
        }

        /// <summary>
        /// The RPC method name associated with the failing request (when available).
        /// </summary>
        public string? RpcMethod { get; }

        /// <summary>
        /// The JSON-RPC request id associated with the failing request (when available).
        /// </summary>
        public string? RequestId { get; }

        /// <summary>
        /// The request URI associated with the failing request (when available).
        /// </summary>
        public Uri? RequestUri { get; }
    }
}
