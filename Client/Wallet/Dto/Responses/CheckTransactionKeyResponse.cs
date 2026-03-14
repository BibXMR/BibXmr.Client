using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>CheckTransactionKey</c>.
    /// </summary>
    internal class CheckTransactionKeyResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public CheckTransactionKey Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for check transaction key.
    /// </summary>
    public class CheckTransactionKey
    {
        /// <summary>
        /// Gets or sets the confirmations.
        /// </summary>
        [JsonPropertyName("confirmations")]
        public ulong Confirmations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether in pool.
        /// </summary>
        [JsonPropertyName("in_pool")]
        public bool IsInPool { get; set; }

        /// <summary>
        /// Gets or sets the received.
        /// </summary>
        [JsonPropertyName("received")]
        public ulong Received { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        [JsonIgnore]
        public bool IsInBlockchain
        {
            get
            {
                return Confirmations > 0ul;
            }
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"Confirmations: {Confirmations}";
        }
    }
}

