using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>ChangeWalletPassword</c>.
    /// </summary>
    internal class ChangeWalletPasswordResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ChangeWalletPassword Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for change wallet password.
    /// </summary>
    public class ChangeWalletPassword
    {
        // ...
    }
}

