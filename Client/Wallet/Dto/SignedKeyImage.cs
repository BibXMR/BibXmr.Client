using System.Text.Json.Serialization;

namespace BibXmr.Client.Wallet.Dto
{
    /// <summary>
    /// Represents wallet RPC data for signed key image.
    /// </summary>
    public class SignedKeyImage : KeyImage
    {
        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return Signature;
        }
    }
}

