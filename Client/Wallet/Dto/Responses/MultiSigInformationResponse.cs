using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>MultiSigInformation</c>.
    /// </summary>
    internal class MultiSigInformationResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public MultiSigInformation Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for multi sig information.
    /// </summary>
    public class MultiSigInformation
    {
        /// <summary>
        /// Gets or sets a value indicating whether multi sig.
        /// </summary>
        [JsonPropertyName("multisig")]
        public bool IsMultiSig { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ready.
        /// </summary>
        [JsonPropertyName("ready")]
        public bool IsReady { get; set; }

        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        [JsonPropertyName("threshold")]
        public uint Threshold { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        [JsonPropertyName("total")]
        public uint Total { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"MultiSig? {IsMultiSig}, Ready? {IsReady}, Threshold: {Threshold}, Total: {Total}";
        }
    }
}

