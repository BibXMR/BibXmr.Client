using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>Account</c>.
    /// </summary>
    internal class AccountResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public Account Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for account.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the collection of subaddress accounts.
        /// </summary>
        [JsonPropertyName("subaddress_accounts")]
        public List<SubaddressDetails> SubaddressAccounts { get; set; } = [];

        /// <summary>
        /// Gets or sets the total balance.
        /// </summary>
        [JsonPropertyName("total_balance")]
        public ulong TotalBalance { get; set; }

        /// <summary>
        /// Gets or sets the total unlocked balance.
        /// </summary>
        [JsonPropertyName("total_unlocked_balance")]
        public ulong TotalUnlockedBalance { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"Unlocked {MoneroAmount.PiconeroToXmr(TotalUnlockedBalance).ToString(MoneroAmount.PrecisionFormat)} / Balance {MoneroAmount.PiconeroToXmr(TotalBalance).ToString(MoneroAmount.PrecisionFormat)}";
        }
    }
}

