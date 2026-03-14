using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>AddressIndex</c>.
    /// </summary>
    internal class AddressIndexResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public AddressIndex Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for address index.
    /// </summary>
    public class AddressIndex
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

