using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>Languages</c>.
    /// </summary>
    internal class LanguagesResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public Languages Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for languages.
    /// </summary>
    public class Languages
    {
        /// <summary>
        /// Gets or sets the collection of all languages.
        /// </summary>
        [JsonPropertyName("languages")]
        public List<string> AllLanguages { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of local languages.
        /// </summary>
        [JsonPropertyName("languages_local")]
        public List<string> LocalLanguages { get; set; } = [];
    }
}

