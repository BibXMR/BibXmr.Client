using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>StopWallet</c>.
    /// </summary>
    internal class StopWalletResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public StopWalletResult Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for stop wallet result.
    /// </summary>
    public class StopWalletResult
    {
        // ...
    }
}

