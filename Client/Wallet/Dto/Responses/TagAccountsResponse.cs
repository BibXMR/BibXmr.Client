using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>TagAccounts</c>.
    /// </summary>
    internal class TagAccountsResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public TagAccounts Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for tag accounts.
    /// </summary>
    public class TagAccounts
    {
        // ...
    }
}

