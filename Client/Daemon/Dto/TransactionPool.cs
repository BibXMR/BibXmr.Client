using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for transaction pool.
    /// </summary>
    public class TransactionPool
    {
        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        [JsonPropertyName("credits")]
        public ulong Credits { get; set; }

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
        /// Gets or sets the collection of spent key images.
        /// </summary>
        [JsonPropertyName("spent_key_images")]
        public List<SpentKeyImage> SpentKeyImages { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of transactions.
        /// </summary>
        [JsonPropertyName("transactions")]
        public List<TransactionPoolTransaction> Transactions { get; set; } = [];

        /// <summary>
        /// Gets or sets a value indicating whether untrusted.
        /// </summary>
        [JsonPropertyName("untrusted")]
        public bool Untrusted { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"Tx Count: {Transactions.Count}";
        }
    }
}

