using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SubmitTransfer</c>.
    /// </summary>
    internal class SubmitTransferResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SubmitTransferResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>SubmitTransfer</c>.
    /// </summary>
    internal class SubmitTransferResult
    {
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
            return string.Join(Environment.NewLine, TransactionHashes);
        }
    }
}

