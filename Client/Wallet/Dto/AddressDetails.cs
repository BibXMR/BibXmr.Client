using System;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto
{
    /// <summary>
    /// Represents wallet RPC data for address details.
    /// </summary>
    public class AddressDetails
    {
        /// <summary>
        /// Gets or sets the account index.
        /// </summary>
        [JsonPropertyName("account_index")]
        public uint AccountIndex { get; set; }

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
        /// Gets or sets the balance.
        /// </summary>
        [JsonPropertyName("balance")]
        public ulong Balance { get; set; }

        /// <summary>
        /// Gets or sets the blocks to unlock.
        /// </summary>
        [JsonPropertyName("blocks_to_unlock")]
        public uint BlocksToUnlock { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the num unspent outputs.
        /// </summary>
        [JsonPropertyName("num_unspent_outputs")]
        public ulong NumUnspentOutputs { get; set; }

        /// <summary>
        /// Gets or sets the time to unlock.
        /// </summary>
        [JsonPropertyName("time_to_unlock")]
        public ulong TimeToUnlock { get; set; }

        /// <summary>
        /// Gets or sets the unlocked balance.
        /// </summary>
        [JsonPropertyName("unlocked_balance")]
        public ulong UnlockedBalance { get; set; }

        /// <summary>
        /// Gets or sets the estimated time till unlock.
        /// </summary>
        [JsonIgnore]
        public TimeSpan EstimatedTimeTillUnlock
        {
            get
            {
                return BlockchainNetworkDefaults.AverageBlockTime.Multiply(BlocksToUnlock);
            }
        }
    }
}


