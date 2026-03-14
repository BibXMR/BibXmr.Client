using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>Sign</c>.
    /// </summary>
    internal class SignResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SignatureResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>Signature</c>.
    /// </summary>
    internal class SignatureResult
    {
        /// <summary>
        /// Gets or sets the sig.
        /// </summary>
        [JsonPropertyName("signature")]
        public string Sig { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return Sig;
        }
    }
}

