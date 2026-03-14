using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>PrepareMultiSig</c>.
    /// </summary>
    internal class PrepareMultiSigResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public PrepareMultiSigResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>PrepareMultiSig</c>.
    /// </summary>
    internal class PrepareMultiSigResult
    {
        /// <summary>
        /// Gets or sets the multi sig information.
        /// </summary>
        [JsonPropertyName("multisig_info")]
        public string MultiSigInformation { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return MultiSigInformation;
        }
    }
}

