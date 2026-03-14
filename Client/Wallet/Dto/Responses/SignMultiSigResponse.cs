using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SignMultiSigTransaction</c>.
    /// </summary>
    internal class SignMultiSigTransactionResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SignMultiSigTransaction Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for sign multi sig transaction.
    /// </summary>
    public class SignMultiSigTransaction
    {
        /// <summary>
        /// Gets or sets the transaction data hex.
        /// </summary>
        [JsonPropertyName("tx_data_hex")]
        public string TransactionDataHex { get; set; }

        /// <summary>
        /// Gets or sets the collection of transaction hashes.
        /// </summary>
        [JsonPropertyName("tx_hash_list")]
        public List<string> TransactionHashes { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"DataHex: {TransactionDataHex} - TxHashes: {string.Join(" ", TransactionHashes)}";
        }
    }
}

