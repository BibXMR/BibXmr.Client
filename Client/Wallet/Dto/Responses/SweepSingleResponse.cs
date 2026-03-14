using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SweepSingle</c>.
    /// </summary>
    internal class SweepSingleResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SweepSingle Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for sweep single.
    /// </summary>
    public class SweepSingle
    {
        /// <summary>
        /// Gets or sets the tx hash.
        /// </summary>
        [JsonPropertyName("tx_hash")]
        public string TxHash { get; set; }

        /// <summary>
        /// Gets or sets the tx key.
        /// </summary>
        [JsonPropertyName("tx_key")]
        public string TxKey { get; set; }

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
        /// Gets or sets the weight.
        /// </summary>
        [JsonPropertyName("weight")]
        public ulong Weight { get; set; }

        /// <summary>
        /// Gets or sets the tx blob.
        /// </summary>
        [JsonPropertyName("tx_blob")]
        public string TxBlob { get; set; }

        /// <summary>
        /// Gets or sets the tx meta data.
        /// </summary>
        [JsonPropertyName("tx_metadata")]
        public string TxMetaData { get; set; }

        /// <summary>
        /// Gets or sets the multi sig tx set.
        /// </summary>
        [JsonPropertyName("multisig_txset")]
        public string MultiSigTxSet { get; set; }

        /// <summary>
        /// Gets or sets the unsigned tx set.
        /// </summary>
        [JsonPropertyName("unsigned_txset")]
        public string UnsignedTxSet { get; set; }
    }
}

