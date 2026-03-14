using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto
{
    /// <summary>
    /// Represents wallet RPC data for payment detail.
    /// </summary>
    public class PaymentDetail
    {
        /// <summary>
        /// Gets or sets the payment id.
        /// </summary>
        [JsonPropertyName("payment_id")]
        public string PaymentID { get; set; }

        /// <summary>
        /// Gets or sets the tx hash.
        /// </summary>
        [JsonPropertyName("tx_hash")]
        public string TxHash { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("block_height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Gets or sets the unlock time.
        /// </summary>
        [JsonPropertyName("unlock_time")]
        public ulong UnlockTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether locked.
        /// </summary>
        [JsonPropertyName("locked")]
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets the subaddress index.
        /// </summary>
        [JsonPropertyName("subaddr_index")]
        public SubaddressIndex SubaddressIndex { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// The amount of time it would take, from the moment the payment was made, until the payment would be unlocked.
        /// </summary>
        [JsonIgnore]
        public TimeSpan EstimatedTimeTillUnlock
        {
            get
            {
                return BlockchainNetworkDefaults.AverageBlockTime.Multiply(BlockchainNetworkDefaults.BaseBlockUnlockThreshold + UnlockTime);
            }
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            Type typeInfo = typeof(PaymentDetail);
            IEnumerable<string> nonNullPropertyList = typeInfo.GetProperties()
                                              .Where(p => p.GetValue(this) != default)
                                              .Select(p => $"{p.Name}: {p.GetValue(this)} ");
            return string.Join(Environment.NewLine, nonNullPropertyList);
        }
    }
}


