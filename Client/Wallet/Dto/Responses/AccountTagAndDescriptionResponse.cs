using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>AccountTagAndDescription</c>.
    /// </summary>
    internal class AccountTagAndDescriptionResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public AccountTagAndDescription Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for account tag and description.
    /// </summary>
    public class AccountTagAndDescription
    {
        // ...
    }
}

