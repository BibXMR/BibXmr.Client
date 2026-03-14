using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>AddAddressBook</c>.
    /// </summary>
    internal class AddAddressBookResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public AddAddressBook Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for add address book.
    /// </summary>
    public class AddAddressBook
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        [JsonPropertyName("index")]
        public uint Index { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"Index {Index}";
        }
    }
}

