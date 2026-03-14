using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for transaction.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Gets or sets the as hex.
        /// </summary>
        [JsonPropertyName("as_hex")]
        public string AsHex { get; set; }

        /// <summary>
        /// Gets or sets the as json.
        /// </summary>
        [JsonPropertyName("as_json")]
        public string AsJson { get; set; }

        /// <summary>
        /// Gets or sets the prunable as hex.
        /// </summary>
        [JsonPropertyName("prunable_as_hex")]
        public string PrunableAsHex { get; set; }

        /// <summary>
        /// Gets or sets the prunable hash.
        /// </summary>
        [JsonPropertyName("prunable_hash")]
        public string PrunableHash { get; set; }

        /// <summary>
        /// Gets or sets the pruned as hex.
        /// </summary>
        [JsonPropertyName("pruned_as_hex")]
        public string PrunedAsHex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether double spend seen.
        /// </summary>
        [JsonPropertyName("double_spend_seen")]
        public bool IsDoubleSpendSeen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether in pool.
        /// </summary>
        [JsonPropertyName("in_pool")]
        public bool InPool { get; set; }

        /// <summary>
        /// Gets or sets the tx hash.
        /// </summary>
        [JsonPropertyName("tx_hash")]
        public string TxHash { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("block_height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp for timestamp.
        /// </summary>
        [JsonPropertyName("block_timestamp")]
        public ulong Timestamp { get; set; }

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
            return TxHash;
        }
    }
}

