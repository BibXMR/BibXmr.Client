using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for chain.
    /// </summary>
    public class Chain
    {
        /// <summary>
        /// Gets or sets the block hash.
        /// </summary>
        [JsonPropertyName("block_hash")]
        public string BlockHash { get; set; }

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
        /// Gets or sets the collection of block hashes.
        /// </summary>
        [JsonPropertyName("block_hashes")]
        public List<string> BlockHashes { get; set; } = [];

        /// <summary>
        /// Gets or sets the main chain parent block.
        /// </summary>
        [JsonPropertyName("main_chain_parent_block")]
        public string MainChainParentBlock { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        [JsonPropertyName("length")]
        public uint Length { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"[{Height}] {BlockHash} - Difficulty: {Difficulty} - Length: {Length} - MainChainParentBlock: {MainChainParentBlock}";
        }
    }
}

