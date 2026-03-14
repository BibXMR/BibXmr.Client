using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>FundTransfer</c>.
    /// </summary>
    internal class FundTransferResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public FundTransfer Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for fund transfer.
    /// </summary>
    public class FundTransfer
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the fee.
        /// </summary>
        [JsonPropertyName("fee")]
        public ulong Fee { get; set; }

        /// <summary>
        /// Gets or sets the multi sig tx set.
        /// </summary>
        [JsonPropertyName("multisig_txset")]
        public string MultiSigTxSet { get; set; }

        /// <summary>
        /// Gets or sets the transaction blob.
        /// </summary>
        [JsonPropertyName("tx_blob")]
        public string TransactionBlob { get; set; }

        /// <summary>
        /// Gets or sets the transaction hash.
        /// </summary>
        [JsonPropertyName("tx_hash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// Gets or sets the transaction key.
        /// </summary>
        [JsonPropertyName("tx_key")]
        public string TransactionKey { get; set; }

        /// <summary>
        /// Gets or sets the transaction metadata.
        /// </summary>
        [JsonPropertyName("tx_metadata")]
        public string TransactionMetadata { get; set; }

        /// <summary>
        /// Gets or sets the unsigned tx set.
        /// </summary>
        [JsonPropertyName("unsigned_txset")]
        public string UnsignedTxSet { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        [JsonPropertyName("weight")]
        public ulong Weight { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"Sent {MoneroAmount.PiconeroToXmr(Amount).ToString(MoneroAmount.PrecisionFormat)} with a fee of {MoneroAmount.PiconeroToXmr(Fee).ToString(MoneroAmount.PrecisionFormat)} [{TransactionHash}]";
        }
    }
}

