using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>FeeEstimate</c>.
    /// </summary>
    internal class FeeEstimateResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public FeeEstimate Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for fee estimate.
    /// </summary>
    public class FeeEstimate
    {
        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        [JsonPropertyName("credits")]
        public ulong Credits { get; set; }

        /// <summary>
        /// Gets or sets the fee.
        /// </summary>
        [JsonPropertyName("fee")]
        public ulong Fee { get; set; }

        /// <summary>
        /// Gets or sets the quantization mask.
        /// </summary>
        [JsonPropertyName("quantization_mask")]
        public ulong QuantizationMask { get; set; }

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
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{MoneroAmount.PiconeroToXmr(Fee).ToString(MoneroAmount.PrecisionFormat)}";
        }
    }
}

