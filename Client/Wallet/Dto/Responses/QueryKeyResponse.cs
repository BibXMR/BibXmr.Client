using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>QueryKey</c>.
    /// </summary>
    internal class QueryKeyResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public QueryKeyResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>QueryKey</c>.
    /// </summary>
    internal class QueryKeyResult
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}

