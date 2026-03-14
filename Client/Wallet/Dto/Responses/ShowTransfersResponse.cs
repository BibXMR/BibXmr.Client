using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>ShowTransfers</c>.
    /// </summary>
    internal class ShowTransfersResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ShowTransfers Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for show transfers.
    /// </summary>
    public class ShowTransfers
    {
        /// <summary>
        /// Gets or sets the collection of incoming transfers.
        /// </summary>
        [JsonPropertyName("in")]
        public List<Transfer> IncomingTransfers { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of outgoing transfers.
        /// </summary>
        [JsonPropertyName("out")]
        public List<Transfer> OutgoingTransfers { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of pending transfers.
        /// </summary>
        [JsonPropertyName("pending")]
        public List<Transfer> PendingTransfers { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of failed transfers.
        /// </summary>
        [JsonPropertyName("failed")]
        public List<Transfer> FailedTransfers { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of pooled transfers.
        /// </summary>
        [JsonPropertyName("pool")]
        public List<Transfer> PooledTransfers { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            bool hasIncomingTransfers = IncomingTransfers.Count > 0, hasOutgoingTransfers = OutgoingTransfers.Count > 0, hasPendingTransfers = PendingTransfers.Count > 0,
                hasFailedTransfers = FailedTransfers.Count > 0, hasPooledTransfers = PooledTransfers.Count > 0;
            if (hasIncomingTransfers)
            {
                sb.AppendLine("Incoming Transfers:");
                sb.AppendLine(string.Join(Environment.NewLine, IncomingTransfers));
                sb.AppendLine();
            }
            if (hasOutgoingTransfers)
            {
                sb.AppendLine("Outgoing Transfers:");
                sb.AppendLine(string.Join(Environment.NewLine, OutgoingTransfers));
                sb.AppendLine();
            }
            if (hasPendingTransfers)
            {
                sb.AppendLine("Pending Transfers:");
                sb.AppendLine(string.Join(Environment.NewLine, PendingTransfers));
                sb.AppendLine();
            }
            if (hasFailedTransfers)
            {
                sb.AppendLine("Failed Transfers:");
                sb.AppendLine(string.Join(Environment.NewLine, FailedTransfers));
                sb.AppendLine();
            }
            if (hasPooledTransfers)
            {
                sb.AppendLine("Pooled Transfers:");
                sb.AppendLine(string.Join(Environment.NewLine, PooledTransfers));
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for transfer destination.
    /// </summary>
    public class TransferDestination
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
    }

    /// <summary>
    /// Represents a wallet RPC response payload for transfer.
    /// </summary>
    public class Transfer
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
        /// Gets or sets the collection of amounts.
        /// </summary>
        [JsonPropertyName("amounts")]
        public List<ulong> Amounts { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of destinations.
        /// </summary>
        [JsonPropertyName("destinations")]
        public List<TransferDestination> Destinations { get; set; } = [];

        /// <summary>
        /// Gets or sets the confirmations.
        /// </summary>
        [JsonPropertyName("confirmations")]
        public ulong Confirmations { get; set; }

        /// <summary>
        /// Gets or sets the fee.
        /// </summary>
        [JsonPropertyName("fee")]
        public ulong Fee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether double spend seen.
        /// </summary>
        [JsonPropertyName("double_spend_seen")]
        public bool IsDoubleSpendSeen { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether locked.
        /// </summary>
        [JsonPropertyName("locked")]
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        [JsonPropertyName("note")]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the payment id.
        /// </summary>
        [JsonPropertyName("payment_id")]
        public string PaymentID { get; set; }

        /// <summary>
        /// Gets or sets the subaddress index.
        /// </summary>
        [JsonPropertyName("subaddr_index")]
        public SubaddressIndex SubaddressIndex { get; set; }

        /// <summary>
        /// Gets or sets the collection of subaddress indices.
        /// </summary>
        [JsonPropertyName("subaddr_indices")]
        public List<SubaddressIndex> SubaddressIndices { get; set; } = [];

        /// <summary>
        /// Gets or sets the suggested confirmations threshold.
        /// </summary>
        [JsonPropertyName("suggested_confirmations_threshold")]
        public ulong SuggestedConfirmationsThreshold { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp for timestamp.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public ulong Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        [JsonPropertyName("txid")]
        public string TransactionID { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the unlock time.
        /// </summary>
        [JsonPropertyName("unlock_time")]
        public ulong UnlockTime { get; set; }

        /// <summary>
        /// Gets or sets the estimated time till unlock.
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
        /// Gets the timestamp converted to local date and time.
        /// </summary>
        [JsonIgnore]
        public DateTime DateTime
        {
            get
            {
                return MoneroDateTime.UnixEpoch.AddSeconds(Timestamp);
            }
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"[{Height}] [{TransactionID}] ({DateTime.ToString(DateFormat.DateTimeFormat)}) - {Address} - {MoneroAmount.PiconeroToXmr(Amount).ToString(MoneroAmount.PrecisionFormat)} - {MoneroAmount.PiconeroToXmr(Fee)} - Confirmations: {Confirmations}";
        }
    }
}


