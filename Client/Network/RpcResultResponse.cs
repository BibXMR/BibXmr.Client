using System.Text.Json.Serialization;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Represents a typed JSON-RPC response envelope.
    /// </summary>
    internal sealed class RpcResultResponse<T> : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public T? Result { get; set; }
    }
}
