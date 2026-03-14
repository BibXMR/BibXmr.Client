using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for spent key image.
    /// </summary>
    public class SpentKeyImage
    {
        /// <summary>
        /// Gets or sets the key image.
        /// </summary>
        [JsonPropertyName("id_hash")]
        public string KeyImage { get; set; }

        /// <summary>
        /// Gets or sets the collection of tx hashes.
        /// </summary>
        [JsonPropertyName("txs_hashes")]
        public List<string> TxHashes { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return KeyImage;
        }
    }
}

