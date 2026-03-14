using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>AddressLabel</c>.
    /// </summary>
    internal class AddressLabelResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public AddressLabel Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for address label.
    /// </summary>
    public class AddressLabel
    {
        // ...
    }
}

