using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto
{
    /// <summary>
    /// Represents wallet RPC data for recipient.
    /// </summary>
    public class Recipient
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Address} - {MoneroAmount.PiconeroToXmr(Amount).ToString(MoneroAmount.PrecisionFormat)}";
        }
    }
}

