using System;

namespace BibXmr.Client.Network.Exceptions
{
    /// <summary>
    /// Raised when an HTTP response payload is malformed, invalid JSON, or does not match the expected protocol shape.
    /// </summary>
    /// <remarks>
    /// Create a new exception instance.
    /// </remarks>
    public sealed class MoneroRpcProtocolException(string message, Exception? innerException = null, string? rpcMethod = null, string? requestId = null, Uri? requestUri = null) : MoneroRpcException(message, innerException, rpcMethod, requestId, requestUri)
    {
    }
}
