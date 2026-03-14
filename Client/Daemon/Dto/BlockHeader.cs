using System;
using System.Text.Json.Serialization;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for block header.
    /// </summary>
    public class BlockHeader
    {
        /// <summary>
        /// Gets or sets the block size.
        /// </summary>
        [JsonPropertyName("block_size")]
        public ulong BlockSize { get; set; }

        /// <summary>
        /// Gets or sets the block weight.
        /// </summary>
        [JsonPropertyName("block_weight")]
        public ulong BlockWeight { get; set; }

        /// <summary>
        /// Gets or sets the cumulative difficulty.
        /// </summary>
        [JsonPropertyName("cumulative_difficulty")]
        public ulong CumulativeDifficulty { get; set; }

        /// <summary>
        /// Gets or sets the cumulative difficulty top 64.
        /// </summary>
        [JsonPropertyName("cumulative_difficulty_top64")]
        public ulong CumulativeDifficultyTop64 { get; set; }

        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        [JsonPropertyName("depth")]
        public ulong Depth { get; set; }

        /// <summary>
        /// Gets or sets the difficulty.
        /// </summary>
        [JsonPropertyName("difficulty")]
        public ulong Difficulty { get; set; }

        /// <summary>
        /// Gets or sets the difficulty top 64.
        /// </summary>
        [JsonPropertyName("difficulty_top64")]
        public ulong DifficultyTop64 { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Gets or sets the long term weight.
        /// </summary>
        [JsonPropertyName("long_term_weight")]
        public ulong LongTermWeight { get; set; }

        /// <summary>
        /// Gets or sets the major version.
        /// </summary>
        [JsonPropertyName("major_version")]
        public byte MajorVersion { get; set; }

        /// <summary>
        /// Gets or sets the miner tx hash.
        /// </summary>
        [JsonPropertyName("miner_tx_hash")]
        public string MinerTxHash { get; set; }

        /// <summary>
        /// Gets or sets the minor version.
        /// </summary>
        [JsonPropertyName("minor_version")]
        public byte MinorVersion { get; set; }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        [JsonPropertyName("nonce")]
        public uint Nonce { get; set; }

        /// <summary>
        /// Gets or sets the num txes.
        /// </summary>
        [JsonPropertyName("num_txes")]
        public ulong NumTxes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether orphan.
        /// </summary>
        [JsonPropertyName("orphan_status")]
        public bool IsOrphan { get; set; }

        /// <summary>
        /// Gets or sets the pow hash.
        /// </summary>
        [JsonPropertyName("pow_hash")]
        public string PowHash { get; set; }

        /// <summary>
        /// Gets or sets the prev hash.
        /// </summary>
        [JsonPropertyName("prev_hash")]
        public string PrevHash { get; set; }

        /// <summary>
        /// Gets or sets the reward.
        /// </summary>
        [JsonPropertyName("reward")]
        public ulong Reward { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp for timestamp.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the wide cumulative difficulty.
        /// </summary>
        [JsonPropertyName("wide_cumulative_difficulty")]
        public string WideCumulativeDifficulty { get; set; }

        /// <summary>
        /// Gets or sets the wide difficulty.
        /// </summary>
        [JsonPropertyName("wide_difficulty")]
        public string WideDifficulty { get; set; }

        /// <summary>
        /// Gets the timestamp converted to local date and time.
        /// </summary>
        [JsonIgnore]
        public DateTime DateTime
        {
            get
            {
                return DateTime.SpecifyKind(MoneroDateTime.UnixEpoch.AddSeconds(Timestamp), DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Gets the UTC timestamp as a <see cref="DateTimeOffset"/>.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset TimestampUtc => new(DateTime);

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"[{Height}] ({DateTime.ToString(DateFormat.DateTimeFormat)}) {Hash} - Size: {BlockSize}, Weight: {BlockWeight}, TxCount: {NumTxes}, Reward: {MoneroAmount.PiconeroToXmr(Reward).ToString(MoneroAmount.PrecisionFormat)}";
        }
    }
}

