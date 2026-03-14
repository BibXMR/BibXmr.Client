using System.Text.Json.Serialization;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Represents the common JSON-RPC response envelope.
    /// </summary>
    internal class RpcResponse
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonConverter(typeof(JsonRpcIdAsStringConverter))]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the json rpc.
        /// </summary>
        [JsonPropertyName("jsonrpc")]
        public string? JsonRpc { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        [JsonPropertyName("error")]
        public Error? Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether contains error.
        /// </summary>
        [JsonIgnore]
        public bool ContainsError
        {
            get
            {
                return Error != null;
            }
        }
    }
}
