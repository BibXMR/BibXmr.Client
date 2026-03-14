using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>AccountLabel</c>.
    /// </summary>
    internal class AccountLabelResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public AccountLabel Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for account label.
    /// </summary>
    public class AccountLabel
    {
        // ...
    }
}

