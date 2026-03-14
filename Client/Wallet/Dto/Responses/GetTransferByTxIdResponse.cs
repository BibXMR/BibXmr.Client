using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>GetTransferByTxid</c>.
    /// </summary>
    internal class GetTransferByTxidResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ShowTransferByTxidResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>ShowTransferByTxid</c>.
    /// </summary>
    internal class ShowTransferByTxidResult
    {
        /// <summary>
        /// Gets or sets the transfer.
        /// </summary>
        [JsonPropertyName("transfer")]
        public Transfer Transfer { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Transfer}";
        }
    }
}

