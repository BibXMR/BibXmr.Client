using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>Balance</c>.
    /// </summary>
    internal class BalanceResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public Balance Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for balance.
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// Gets or sets the total balance.
        /// </summary>
        [JsonPropertyName("balance")]
        public ulong TotalBalance { get; set; }

        /// <summary>
        /// Gets or sets the blocks to unlock.
        /// </summary>
        [JsonPropertyName("blocks_to_unlock")]
        public ulong BlocksToUnlock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multi sig import needed.
        /// </summary>
        [JsonPropertyName("multisig_import_needed")]
        public bool IsMultiSigImportNeeded { get; set; }

        /// <summary>
        /// Gets or sets the collection of subaddress details.
        /// </summary>
        [JsonPropertyName("per_subaddress")]
        public List<AddressDetails> SubaddressDetails { get; set; } = [];

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

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"Unlocked {MoneroAmount.PiconeroToXmr(UnlockedBalance).ToString(MoneroAmount.PrecisionFormat)} / Total {MoneroAmount.PiconeroToXmr(TotalBalance).ToString(MoneroAmount.PrecisionFormat)}";
        }
    }
}


