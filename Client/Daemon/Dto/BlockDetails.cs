using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for block details.
    /// </summary>
    public class BlockDetails
    {
        /// <summary>
        /// Gets or sets the major version.
        /// </summary>
        [JsonPropertyName("major_version")]
        public byte MajorVersion { get; set; }

        /// <summary>
        /// Gets or sets the minor version.
        /// </summary>
        [JsonPropertyName("minor_version")]
        public byte MinorVersion { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp for timestamp.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the prev id.
        /// </summary>
        [JsonPropertyName("prev_id")]
        public string PrevID { get; set; }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        [JsonPropertyName("nonce")]
        public uint Nonce { get; set; }

        /// <summary>
        /// Gets or sets the miner tx.
        /// </summary>
        [JsonPropertyName("miner_tx")]
        public MinerTransaction MinerTx { get; set; }

        /// <summary>
        /// Gets or sets the collection of tx hashes.
        /// </summary>
        [JsonPropertyName("tx_hashes")]
        public List<string> TxHashes { get; set; } = [];

        /// <summary>
        /// Gets the timestamp converted to local date and time.
        /// </summary>
        [JsonIgnore]
        public DateTime DateTime
        {
            get
            {
                return MoneroDateTime.UnixEpoch.AddSeconds(Timestamp);
            }
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder($"({DateTime.ToString(DateFormat.DateTimeFormat)}) {MajorVersion} / {MinorVersion} {PrevID}");
            sb.AppendLine($"{MinerTx}");
            return sb.ToString();
        }
    }
}


