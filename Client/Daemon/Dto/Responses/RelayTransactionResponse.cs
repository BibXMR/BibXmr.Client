using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>RelayTransaction</c>.
    /// </summary>
    internal class RelayTransactionResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public RelayTransactionResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>RelayTransaction</c>.
    /// </summary>
    internal class RelayTransactionResult
    {
        /// <summary>
        /// Gets or sets the tx hash.
        /// </summary>
        [JsonPropertyName("tx_hash")]
        public string TxHash { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return TxHash;
        }
    }
}

