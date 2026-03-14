using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>GetTransactionKey</c>.
    /// </summary>
    internal class GetTransactionKeyResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public GetTransactionKeyResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>GetTransactionKey</c>.
    /// </summary>
    internal class GetTransactionKeyResult
    {
        /// <summary>
        /// Gets or sets the transaction key.
        /// </summary>
        [JsonPropertyName("tx_key")]
        public string TransactionKey { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return TransactionKey;
        }
    }
}

