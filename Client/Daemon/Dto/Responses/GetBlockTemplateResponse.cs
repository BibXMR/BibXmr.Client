using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>GetBlockTemplate</c>.
    /// </summary>
    internal class GetBlockTemplateResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public BlockTemplate Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for block template.
    /// </summary>
    public class BlockTemplate
    {
        /// <summary>
        /// Gets or sets the difficulty.
        /// </summary>
        [JsonPropertyName("difficulty")]
        public ulong Difficulty { get; set; }

        /// <summary>
        /// Gets or sets the wide difficulty.
        /// </summary>
        [JsonPropertyName("wide_difficulty")]
        public string WideDifficulty { get; set; }

        /// <summary>
        /// Gets or sets the difficulty top 64.
        /// </summary>
        [JsonPropertyName("difficulty_top64")]
        public ulong DifficultyTop64 { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Gets or sets the reserved offset.
        /// </summary>
        [JsonPropertyName("reserved_offset")]
        public ulong ReservedOffset { get; set; }

        /// <summary>
        /// Gets or sets the expected reward.
        /// </summary>
        [JsonPropertyName("expected_reward")]
        public ulong ExpectedReward { get; set; }

        /// <summary>
        /// Gets or sets the previous hash.
        /// </summary>
        [JsonPropertyName("prev_hash")]
        public string PreviousHash { get; set; }

        /// <summary>
        /// Gets or sets the seed height.
        /// </summary>
        [JsonPropertyName("seed_height")]
        public ulong SeedHeight { get; set; }

        /// <summary>
        /// Gets or sets the seed hash.
        /// </summary>
        [JsonPropertyName("seed_hash")]
        public string SeedHash { get; set; }

        /// <summary>
        /// Gets or sets the next seed hash.
        /// </summary>
        [JsonPropertyName("next_seed_hash")]
        public string NextSeedHash { get; set; }

        /// <summary>
        /// Gets or sets the block template blob.
        /// </summary>
        [JsonPropertyName("blocktemplate_blob")]
        public string BlockTemplateBlob { get; set; }

        /// <summary>
        /// Gets or sets the block hashing blob.
        /// </summary>
        [JsonPropertyName("blockhashing_blob")]
        public string BlockHashingBlob { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            Type typeInfo = typeof(BlockTemplate);
            IEnumerable<string> nonNullPropertyList = typeInfo.GetProperties()
                                              .Where(p => p.GetValue(this) != default)
                                              .Select(p => $"{p.Name}: {p.GetValue(this)} ");
            return string.Join(Environment.NewLine, nonNullPropertyList);
        }
    }
}

