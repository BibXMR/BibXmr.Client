using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for transaction pool transaction.
    /// </summary>
    public class TransactionPoolTransaction
    {
        /// <summary>
        /// Gets or sets the blob size.
        /// </summary>
        [JsonPropertyName("blob_size")]
        public ulong BlobSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether double spend seen.
        /// </summary>
        [JsonPropertyName("double_spend_seen")]
        public bool IsDoubleSpendSeen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether do not relay.
        /// </summary>
        [JsonPropertyName("do_not_relay")]
        public bool DoNotRelay { get; set; }

        /// <summary>
        /// Gets or sets the fee.
        /// </summary>
        [JsonPropertyName("fee")]
        public ulong Fee { get; set; }

        /// <summary>
        /// Gets or sets the tx hash.
        /// </summary>
        [JsonPropertyName("id_hash")]
        public string TxHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether kept by block.
        /// </summary>
        [JsonPropertyName("kept_by_block")]
        public bool KeptByBlock { get; set; }

        /// <summary>
        /// Gets or sets the last failed height.
        /// </summary>
        [JsonPropertyName("last_failed_height")]
        public ulong LastFailedHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether previously failed.
        /// </summary>
        [JsonIgnore]
        public bool PreviouslyFailed
        {
            get
            {
                const string DefaultIdHash = "0000000000000000000000000000000000000000000000000000000000000000";
                const ulong DefaultHeight = 0;
                return LastFailedHeight != DefaultHeight &&
                    string.Compare(DefaultIdHash, LastFailedTxHash) != 0;
            }
        }

        /// <summary>
        /// Gets or sets the last failed tx hash.
        /// </summary>
        [JsonPropertyName("last_failed_id_hash")]
        public string LastFailedTxHash { get; set; }

        /// <summary>
        /// Gets or sets the last relayed time.
        /// </summary>
        [JsonPropertyName("last_relayed_time")]
        public ulong LastRelayedTime { get; set; }

        /// <summary>
        /// Gets or sets the last relay date time.
        /// </summary>
        [JsonIgnore]
        public DateTime LastRelayDateTime
        {
            get
            {
                return DateTime.SpecifyKind(MoneroDateTime.UnixEpoch.AddSeconds(LastRelayedTime), DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Gets the last relay timestamp as a <see cref="DateTimeOffset"/> in UTC.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset LastRelayTimestampUtc => new(LastRelayDateTime);

        /// <summary>
        /// Gets or sets the max used block height.
        /// </summary>
        [JsonPropertyName("max_used_block_height")]
        public ulong MaxUsedBlockHeight { get; set; }

        /// <summary>
        /// Gets or sets the max used block tx hash.
        /// </summary>
        [JsonPropertyName("max_used_block_id_hash")]
        public string MaxUsedBlockTxHash { get; set; }

        /// <summary>
        /// Gets or sets the receive time.
        /// </summary>
        [JsonPropertyName("receive_time")]
        public ulong ReceiveTime { get; set; }

        /// <summary>
        /// Gets or sets the receive date time.
        /// </summary>
        [JsonIgnore]
        public DateTime ReceiveDateTime
        {
            get
            {
                return DateTime.SpecifyKind(MoneroDateTime.UnixEpoch.AddSeconds(ReceiveTime), DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Gets the receive timestamp as a <see cref="DateTimeOffset"/> in UTC.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset ReceiveTimestampUtc => new(ReceiveDateTime);

        /// <summary>
        /// Gets or sets a value indicating whether relayed.
        /// </summary>
        [JsonPropertyName("relayed")]
        public bool Relayed { get; set; }

        /// <summary>
        /// Gets or sets the tx blob.
        /// </summary>
        [JsonPropertyName("tx_blob")]
        public string TxBlob { get; set; }

        /// <summary>
        /// Gets or sets the tx json.
        /// </summary>
        [JsonPropertyName("tx_json")]
        public string TxJson { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        [JsonPropertyName("weight")]
        public ulong Weight { get; set; }

        /// <summary>
        /// Gets or sets the effective size.
        /// </summary>
        [JsonIgnore]
        public ulong EffectiveSize => Weight != 0 ? Weight : BlobSize;

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

