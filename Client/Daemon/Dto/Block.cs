using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for block.
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Gets or sets the blob.
        /// </summary>
        [JsonPropertyName("blob")]
        public string Blob { get; set; }

        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        [JsonPropertyName("credits")]
        public ulong Credits { get; set; }

        /// <summary>
        /// Gets or sets the block header.
        /// </summary>
        [JsonPropertyName("block_header")]
        public BlockHeader BlockHeader { get; set; }

        /// <summary>
        /// Gets or sets the json.
        /// </summary>
        [JsonPropertyName("json")]
        public string Json { get; set; }

        /// <summary>
        /// Gets or sets the miner tx hash.
        /// </summary>
        [JsonPropertyName("miner_tx_hash")]
        public string MinerTxHash { get; set; }

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
            return string.Join(Environment.NewLine, TopHash, BlockHeader);
        }
    }
}

