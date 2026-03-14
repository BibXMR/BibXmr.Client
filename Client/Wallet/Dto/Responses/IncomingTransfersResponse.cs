using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>IncomingTransfers</c>.
    /// </summary>
    internal class IncomingTransfersResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public IncomingTransfersResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>IncomingTransfers</c>.
    /// </summary>
    internal class IncomingTransfersResult
    {
        /// <summary>
        /// Gets or sets the collection of transfers.
        /// </summary>
        [JsonPropertyName("transfers")]
        public List<IncomingTransfer> Transfers { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, Transfers);
        }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for incoming transfer.
    /// </summary>
    public class IncomingTransfer
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the block height.
        /// </summary>
        [JsonPropertyName("block_height")]
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// Gets or sets the global index.
        /// </summary>
        [JsonPropertyName("global_index")]
        public ulong GlobalIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether frozen.
        /// </summary>
        [JsonPropertyName("frozen")]
        public bool IsFrozen { get; set; }

        /// <summary>
        /// Gets or sets the key image.
        /// </summary>
        [JsonPropertyName("key_image")]
        public string KeyImage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether spent.
        /// </summary>
        [JsonPropertyName("spent")]
        public bool IsSpent { get; set; }

        /// <summary>
        /// Gets or sets the subaddress index.
        /// </summary>
        [JsonPropertyName("subaddr_index")]
        public SubaddressIndex SubaddressIndex { get; set; }

        /// <summary>
        /// Gets or sets the transaction hash.
        /// </summary>
        [JsonPropertyName("tx_hash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether unlocked.
        /// </summary>
        [JsonPropertyName("unlocked")]
        public bool IsUnlocked { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"[{BlockHeight}] - {TransactionHash} - {MoneroAmount.PiconeroToXmr(Amount).ToString(MoneroAmount.PrecisionFormat)} - {(IsSpent ? "Spent" : "Unspent")} - {(IsUnlocked ? "Unlocked" : "Locked")}";
        }
    }
}

