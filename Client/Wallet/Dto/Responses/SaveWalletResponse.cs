using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SaveWallet</c>.
    /// </summary>
    internal class SaveWalletResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SaveWallet Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for save wallet.
    /// </summary>
    public class SaveWallet
    {
        // ...
    }
}

