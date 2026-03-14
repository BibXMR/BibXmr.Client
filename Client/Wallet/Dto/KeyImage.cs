using System.Text.Json.Serialization;

namespace BibXmr.Client.Wallet.Dto
{
    /// <summary>
    /// Represents wallet RPC data for key image.
    /// </summary>
    public class KeyImage
    {
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        [JsonPropertyName("key_image")]
        public string Image { get; set; }
    }
}

