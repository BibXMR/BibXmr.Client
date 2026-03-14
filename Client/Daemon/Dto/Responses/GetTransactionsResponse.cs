using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents a daemon RPC response payload for transaction set.
    /// </summary>
    public class TransactionSet
    {
        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        [JsonPropertyName("credits")]
        public ulong Credits { get; set; }

        /// <summary>
        /// Gets or sets the top hash.
        /// </summary>
        [JsonPropertyName("top_hash")]
        public string TopHash { get; set; }

        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the collection of transactions.
        /// </summary>
        [JsonPropertyName("txs")]
        public List<Transaction> Transactions { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of txs as hex.
        /// </summary>
        [JsonPropertyName("txs_as_hex")]
        public List<string> TxsAsHex { get; set; } = [];

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

