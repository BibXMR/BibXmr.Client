using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SplitFundTransfer</c>.
    /// </summary>
    internal class SplitFundTransferResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SplitFundTransfer Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for split fund transfer.
    /// </summary>
    public class SplitFundTransfer
    {
        /// <summary>
        /// Gets or sets the collection of transaction hashes.
        /// </summary>
        [JsonPropertyName("tx_hash_list")]
        public List<string> TransactionHashes { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of transaction keys.
        /// </summary>
        [JsonPropertyName("tx_key_list")]
        public List<string> TransactionKeys { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of amounts.
        /// </summary>
        [JsonPropertyName("amount_list")]
        public List<ulong> Amounts { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of fees.
        /// </summary>
        [JsonPropertyName("fee_list")]
        public List<ulong> Fees { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of transaction metadata.
        /// </summary>
        [JsonPropertyName("tx_metadata_list")]
        public List<string> TransactionMetadata { get; set; } = [];

        /// <summary>
        /// Gets or sets the multi sig transaction set.
        /// </summary>
        [JsonPropertyName("multisig_txset")]
        public string MultiSigTransactionSet { get; set; }

        /// <summary>
        /// Gets or sets the unsigned transaction set.
        /// </summary>
        [JsonPropertyName("unsigned_txset")]
        public string UnsignedTransactionSet { get; set; }

        /// <summary>
        /// Gets or sets the collection of weights.
        /// </summary>
        [JsonPropertyName("weight_list")]
        public List<ulong> Weights { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            bool equalAmounts = TransactionHashes.Count == Amounts.Count && Amounts.Count == Fees.Count;
            var sb = new StringBuilder();
            if (equalAmounts)
            {
                for (int transferNumber = 0; transferNumber < TransactionHashes.Count; ++transferNumber)
                {
                    sb.AppendLine($"Sent {MoneroAmount.PiconeroToXmr(Amounts[transferNumber]).ToString(MoneroAmount.PrecisionFormat)} with a fee of {MoneroAmount.PiconeroToXmr(Fees[transferNumber]).ToString(MoneroAmount.PrecisionFormat)} [{TransactionHashes[transferNumber]}]");
                }
            }
            return sb.ToString();
        }
    }
}

