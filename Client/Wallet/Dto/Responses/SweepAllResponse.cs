using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SweepAll</c>.
    /// </summary>
    internal class SweepAllResponse : RpcResponse
    {
        // Result is formatted the same as FundTransferResponse.

        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SplitFundTransfer Result { get; set; }
    }
}

