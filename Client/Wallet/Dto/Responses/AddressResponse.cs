using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>Address</c>.
    /// </summary>
    internal class AddressResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public Addresses Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for addresses.
    /// </summary>
    public class Addresses
    {
        /// <summary>
        /// Gets or sets the primary address.
        /// </summary>
        [JsonPropertyName("address")]
        public string PrimaryAddress { get; set; }

        /// <summary>
        /// Gets or sets the collection of all addresses.
        /// </summary>
        [JsonPropertyName("addresses")]
        public List<AddressInformation> AllAddresses { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(PrimaryAddress);
            sb.AppendLine("All Addresses:");
            sb.AppendLine(string.Join(Environment.NewLine, AllAddresses));
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for address information.
    /// </summary>
    public class AddressInformation
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
        /// Gets or sets the label.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether used.
        /// </summary>
        [JsonPropertyName("used")]
        public bool IsUsed { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"[{AddressIndex}] ({Label}) - {(IsUsed ? "Used" : "Unused")} - {Address}";
        }
    }
}

