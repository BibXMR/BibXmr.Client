using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>RescanSpent</c>.
    /// </summary>
    internal class RescanSpentResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public RescanSpent Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for rescan spent.
    /// </summary>
    public class RescanSpent
    {
        // ...
    }
}

