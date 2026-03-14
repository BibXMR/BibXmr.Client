using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>DescribeTransfer</c>.
    /// </summary>
    internal class DescribeTransferResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public DescribeTransferResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>DescribeTransfer</c>.
    /// </summary>
    internal class DescribeTransferResult
    {
        /// <summary>
        /// Gets or sets the collection of transfer descriptions.
        /// </summary>
        [JsonPropertyName("desc")]
        public List<TransferDescription> TransferDescriptions { get; set; } = [];
    }

    /// <summary>
    /// Represents a wallet RPC response payload for transfer description.
    /// </summary>
    public class TransferDescription
    {
        /// <summary>
        /// Gets or sets the amount in.
        /// </summary>
        [JsonPropertyName("amount_in")]
        public ulong AmountIn { get; set; }

        /// <summary>
        /// Gets or sets the amount out.
        /// </summary>
        [JsonPropertyName("amount_out")]
        public ulong AmountOut { get; set; }

        /// <summary>
        /// Gets or sets the ring size.
        /// </summary>
        [JsonPropertyName("ring_size")]
        public uint RingSize { get; set; }

        /// <summary>
        /// Gets or sets the unlock time.
        /// </summary>
        [JsonPropertyName("unlock_time")]
        public ulong UnlockTime { get; set; }

        /// <summary>
        /// Gets or sets the collection of recipients.
        /// </summary>
        [JsonPropertyName("recipients")]
        public List<Recipient> Recipients { get; set; } = [];

        /// <summary>
        /// Gets or sets the payment id.
        /// </summary>
        [JsonPropertyName("payment_id")]
        public string PaymentID { get; set; }

        /// <summary>
        /// Gets or sets the change amount.
        /// </summary>
        [JsonPropertyName("change_amount")]
        public ulong ChangeAmount { get; set; }

        /// <summary>
        /// Gets or sets the change address.
        /// </summary>
        [JsonPropertyName("change_address")]
        public string ChangeAddress { get; set; }

        /// <summary>
        /// Gets or sets the fee.
        /// </summary>
        [JsonPropertyName("fee")]
        public ulong Fee { get; set; }

        /// <summary>
        /// Gets or sets the dummy outputs.
        /// </summary>
        [JsonPropertyName("dummy_outputs")]
        public uint DummyOutputs { get; set; }

        /// <summary>
        /// Gets or sets the extra.
        /// </summary>
        [JsonPropertyName("extra")]
        public string Extra { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            Type typeInfo = typeof(TransferDescription);
            IEnumerable<string> nonNullPropertyList = typeInfo.GetProperties()
                                              .Where(p => p.GetValue(this) != default)
                                              .Select(p => $"{p.Name}: {p.GetValue(this)} ");
            return string.Join(Environment.NewLine, nonNullPropertyList);
        }
    }
}

