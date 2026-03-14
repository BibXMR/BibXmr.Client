namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents the outcome of executing an arbitrary daemon JSON-RPC method.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="RawJsonRpcExecutionResult"/> class.
    /// </remarks>
    /// <param name="isSuccess">Indicates whether the execution succeeded.</param>
    /// <param name="method">The executed daemon method name.</param>
    /// <param name="paramsJson">The normalized params JSON used for the call.</param>
    /// <param name="requestJson">The full request payload sent to the daemon.</param>
    /// <param name="responseJson">The full response payload returned by the daemon.</param>
    /// <param name="errorCode">Optional JSON-RPC error code.</param>
    /// <param name="errorMessage">Optional error message.</param>
    public sealed class RawJsonRpcExecutionResult(
        bool isSuccess,
        string method,
        string paramsJson,
        string requestJson,
        string responseJson,
        int? errorCode,
        string? errorMessage)
    {
        /// <summary>
        /// Gets a value indicating whether the execution succeeded.
        /// </summary>
        public bool IsSuccess { get; } = isSuccess;

        /// <summary>
        /// Gets the executed daemon method name.
        /// </summary>
        public string Method { get; } = method;

        /// <summary>
        /// Gets the normalized params JSON used for the call.
        /// </summary>
        public string ParamsJson { get; } = paramsJson;

        /// <summary>
        /// Gets the full request payload sent to the daemon.
        /// </summary>
        public string RequestJson { get; } = requestJson;

        /// <summary>
        /// Gets the full response payload returned by the daemon.
        /// </summary>
        public string ResponseJson { get; } = responseJson;

        /// <summary>
        /// Gets the optional JSON-RPC error code.
        /// </summary>
        public int? ErrorCode { get; } = errorCode;

        /// <summary>
        /// Gets the optional error message.
        /// </summary>
        public string? ErrorMessage { get; } = errorMessage;
    }
}

