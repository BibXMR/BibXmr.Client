using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>OpenWallet</c>.
    /// </summary>
    internal class OpenWalletResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public OpenWallet Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for open wallet.
    /// </summary>
    public class OpenWallet
    {
        // ...
    }
}

