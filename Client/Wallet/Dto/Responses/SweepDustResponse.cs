using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SweepDust</c>.
    /// </summary>
    internal class SweepDustResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SweepDust Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for sweep dust.
    /// </summary>
    public class SweepDust
    {
        // ...
    }
}

