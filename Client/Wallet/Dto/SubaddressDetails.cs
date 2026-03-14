using System.Text.Json.Serialization;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto
{
    /// <summary>
    /// Represents wallet RPC data for subaddress details.
    /// </summary>
    public class SubaddressDetails
    {
        /// <summary>
        /// Gets or sets the account index.
        /// </summary>
        [JsonPropertyName("account_index")]
        public uint AccountIndex { get; set; }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        [JsonPropertyName("balance")]
        public ulong Balance { get; set; }

        /// <summary>
        /// Gets or sets the base address.
        /// </summary>
        [JsonPropertyName("base_address")]
        public string BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the unlocked balance.
        /// </summary>
        [JsonPropertyName("unlocked_balance")]
        public ulong UnlockedBalance { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"[{AccountIndex}] ({Tag}) {BaseAddress} - Unlocked {MoneroAmount.PiconeroToXmr(UnlockedBalance).ToString(MoneroAmount.PrecisionFormat)} / Total {MoneroAmount.PiconeroToXmr(Balance).ToString(MoneroAmount.PrecisionFormat)}";
        }
    }
}

