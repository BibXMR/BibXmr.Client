using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>AccountTags</c>.
    /// </summary>
    internal class AccountTagsResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public AccountTags Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for account tags.
    /// </summary>
    public class AccountTags
    {
        /// <summary>
        /// Gets or sets the collection of acccount tags.
        /// </summary>
        [JsonPropertyName("account_tags")]
        public List<AccountTag> AcccountTags { get; set; } = [];
    }

    /// <summary>
    /// Represents a wallet RPC response payload for account tag.
    /// </summary>
    public class AccountTag
    {
        /// <summary>
        /// Gets or sets the collection of accounts.
        /// </summary>
        [JsonPropertyName("accounts")]
        public List<uint> Accounts { get; set; } = [];

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"({Tag}) {Label}";
        }
    }
}

