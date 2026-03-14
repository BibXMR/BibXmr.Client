using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>GetAddressBook</c>.
    /// </summary>
    internal class GetAddressBookResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public AddressBook Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for address book.
    /// </summary>
    public class AddressBook
    {
        /// <summary>
        /// Gets or sets the collection of entries.
        /// </summary>
        [JsonPropertyName("entries")]
        public List<AddressBookEntry> Entries { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, Entries);
        }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for address book entry.
    /// </summary>
    public class AddressBookEntry
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        [JsonPropertyName("index")]
        public ulong Index { get; set; }

        /// <summary>
        /// Gets or sets the payment id.
        /// </summary>
        [JsonPropertyName("payment_id")]
        public string PaymentID { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"[{Index}] {Address} - {Description} - {PaymentID}";
        }
    }
}

