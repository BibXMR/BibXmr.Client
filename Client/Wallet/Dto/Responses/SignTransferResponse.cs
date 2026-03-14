using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SignTransfer</c>.
    /// </summary>
    internal class SignTransferResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SignTransfer Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for sign transfer.
    /// </summary>
    public class SignTransfer
    {
        /// <summary>
        /// Gets or sets the signed transaction set.
        /// </summary>
        [JsonPropertyName("signed_txset")]
        public string SignedTransactionSet { get; set; }

        /// <summary>
        /// Gets or sets the collection of transaction hashes.
        /// </summary>
        [JsonPropertyName("tx_hash_list")]
        public List<string> TransactionHashes { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of raw transactions.
        /// </summary>
        [JsonPropertyName("tx_raw_list")]
        public List<string> RawTransactions { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, TransactionHashes);
        }
    }
}

