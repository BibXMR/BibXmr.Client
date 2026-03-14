using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SetAttribute</c>.
    /// </summary>
    internal class SetAttributeResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public GetAttributeResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>SetAttribute</c>.
    /// </summary>
    internal class SetAttributeResult
    {
        // Empty
    }
}

