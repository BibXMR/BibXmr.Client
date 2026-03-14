using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>CoinbaseTransactionSum</c>.
    /// </summary>
    internal class CoinbaseTransactionSumResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public CoinbaseTransactionSum Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for coinbase transaction sum.
    /// </summary>
    public class CoinbaseTransactionSum
    {
        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        [JsonPropertyName("credits")]
        public ulong Credits { get; set; }

        /// <summary>
        /// Gets or sets the emission amount.
        /// </summary>
        [JsonPropertyName("emission_amount")]
        public ulong EmissionAmount { get; set; }

        /// <summary>
        /// Gets or sets the emission amount top 64.
        /// </summary>
        [JsonPropertyName("emission_amount_top64")]
        public ulong EmissionAmountTop64 { get; set; }

        /// <summary>
        /// Gets or sets the fee amount.
        /// </summary>
        [JsonPropertyName("fee_amount")]
        public ulong FeeAmount { get; set; }

        /// <summary>
        /// Gets or sets the fee amount top 64.
        /// </summary>
        [JsonPropertyName("fee_amount_top64")]
        public ulong FeeAmountTop64 { get; set; }

        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the top hash.
        /// </summary>
        [JsonPropertyName("top_hash")]
        public string TopHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether untrusted.
        /// </summary>
        [JsonPropertyName("untrusted")]
        public bool IsUntrusted { get; set; }

        /// <summary>
        /// Gets or sets the wide emission amount.
        /// </summary>
        [JsonPropertyName("wide_emision_amount")]
        public string WideEmissionAmount { get; set; }

        /// <summary>
        /// Gets or sets the wide fee amount.
        /// </summary>
        [JsonPropertyName("wide_fee_amount")]
        public string WideFeeAmount { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{TopHash} - Emission: {MoneroAmount.PiconeroToXmr(EmissionAmount).ToString(MoneroAmount.PrecisionFormat)} - Fee: {MoneroAmount.PiconeroToXmr(FeeAmount).ToString(MoneroAmount.PrecisionFormat)}";
        }
    }
}

