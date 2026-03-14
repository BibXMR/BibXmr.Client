using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>CreateWallet</c>.
    /// </summary>
    internal class CreateWalletResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public CreateWallet Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for create wallet.
    /// </summary>
    public class CreateWallet
    {
        // ...
    }
}

