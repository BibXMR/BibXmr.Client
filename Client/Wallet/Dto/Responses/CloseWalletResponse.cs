using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>CloseWallet</c>.
    /// </summary>
    internal class CloseWalletResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public CloseWallet Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for close wallet.
    /// </summary>
    public class CloseWallet
    {
        // ...
    }
}

