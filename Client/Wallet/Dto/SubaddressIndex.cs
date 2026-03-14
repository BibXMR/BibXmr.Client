using System.Text.Json.Serialization;

namespace BibXmr.Client.Wallet.Dto
{
    /// <summary>
    /// Represents wallet RPC data for subaddress index.
    /// </summary>
    public class SubaddressIndex
    {
        /// <summary>
        /// Gets or sets the major.
        /// </summary>
        [JsonPropertyName("major")]
        public uint Major { get; set; }

        /// <summary>
        /// Gets or sets the minor.
        /// </summary>
        [JsonPropertyName("minor")]
        public uint Minor { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Major} / {Minor}";
        }
    }
}

