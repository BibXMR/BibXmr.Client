using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>AddressCreation</c>.
    /// </summary>
    internal class AddressCreationResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public AddressCreation Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for address creation.
    /// </summary>
    public class AddressCreation
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the address index.
        /// </summary>
        [JsonPropertyName("address_index")]
        public uint AddressIndex { get; set; }

        /// <summary>
        /// Gets or sets the collection of address indices.
        /// </summary>
        [JsonPropertyName("address_indices")]
        public List<uint> AddressIndices { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of addresses.
        /// </summary>
        [JsonPropertyName("addresses")]
        public List<string> Addresses { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"[{AddressIndex}] {Address}";
        }
    }
}

