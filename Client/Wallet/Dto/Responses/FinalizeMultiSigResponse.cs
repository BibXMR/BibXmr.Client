using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>FinalizeMultiSig</c>.
    /// </summary>
    internal class FinalizeMultiSigResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public FinalizeMultiSigResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>FinalizeMultiSig</c>.
    /// </summary>
    internal class FinalizeMultiSigResult
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return Address;
        }
    }
}

