using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>OutputHistogram</c>.
    /// </summary>
    internal class OutputHistogramResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public OutputHistogramResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>OutputHistogram</c>.
    /// </summary>
    internal class OutputHistogramResult
    {
        /// <summary>
        /// Gets or sets the collection of distributions.
        /// </summary>
        [JsonPropertyName("distributions")]
        public List<Distribution> Distributions { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return string.Join(", ", Distributions);
        }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for distribution.
    /// </summary>
    public class Distribution
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [JsonPropertyName("data")]
        public OutputDistributionData Data { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the compressed data.
        /// </summary>
        [JsonPropertyName("compressed_data")]
        public string CompressedData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether binary.
        /// </summary>
        [JsonPropertyName("binary")]
        public bool IsBinary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether compressed.
        /// </summary>
        [JsonPropertyName("compress")]
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"Amount: {MoneroAmount.PiconeroToXmr(Amount).ToString(MoneroAmount.PrecisionFormat)} - IsCompressed: {IsCompressed} - CompressedData: {CompressedData} - IsBinary: {IsBinary}";
        }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for output distribution data.
    /// </summary>
    public class OutputDistributionData
    {
        /// <summary>
        /// Gets or sets the collection of distributions.
        /// </summary>
        [JsonPropertyName("distribution")]
        public List<ulong> Distributions { get; set; } = [];

        /// <summary>
        /// Gets or sets the start height.
        /// </summary>
        [JsonPropertyName("start_height")]
        public ulong StartHeight { get; set; }

        /// <summary>
        /// Gets or sets the base.
        /// </summary>
        [JsonPropertyName("base")]
        public ulong Base { get; set; }
    }
}

