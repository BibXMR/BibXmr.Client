using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>UntagAccounts</c>.
    /// </summary>
    internal class UntagAccountsResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public UntagAccounts Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for untag accounts.
    /// </summary>
    public class UntagAccounts
    {
        // ...
    }
}

