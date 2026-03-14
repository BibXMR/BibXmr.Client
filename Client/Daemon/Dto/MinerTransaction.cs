using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for miner transaction.
    /// </summary>
    public class MinerTransaction
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [JsonPropertyName("version")]
        public uint Version { get; set; }

        /// <summary>
        /// Gets or sets the unlock time.
        /// </summary>
        [JsonPropertyName("unlock_time")]
        public ulong UnlockTime { get; set; }

        /// <summary>
        /// Gets or sets the collection of vin.
        /// </summary>
        [JsonPropertyName("vin")]
        public List<TransactionInput> Vin { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of vout.
        /// </summary>
        [JsonPropertyName("vout")]
        public List<TransactionOutput> Vout { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of extra.
        /// </summary>
        [JsonPropertyName("extra")]
        public List<uint> Extra { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of signatures.
        /// </summary>
        [JsonPropertyName("signatures")]
        public List<string> Signatures { get; set; } = [];

        /// <summary>
        /// Gets or sets the estimated time till unlock.
        /// </summary>
        [JsonIgnore]
        public TimeSpan EstimatedTimeTillUnlock
        {
            get
            {
                return BlockchainNetworkDefaults.AverageBlockTime.Multiply(BlockchainNetworkDefaults.BaseBlockUnlockThreshold + UnlockTime);
            }
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Version: {Version} UnlockTime: {UnlockTime}");
            sb.AppendLine($"Vin: {string.Join(Environment.NewLine, Vin)}");
            sb.AppendLine($"Vout: {string.Join(Environment.NewLine, Vout)}");
            sb.AppendLine($"Extra: {string.Join(", ", Extra)}");
            sb.AppendLine($"Signatures: {string.Join(", ", Signatures)}");
            return sb.ToString();
        }
    }
}


