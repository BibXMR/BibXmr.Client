using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>Verify</c>.
    /// </summary>
    internal class VerifyResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public VerifyResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>Verify</c>.
    /// </summary>
    internal class VerifyResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether good.
        /// </summary>
        [JsonPropertyName("good")]
        public bool IsGood { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{(IsGood ? "Good" : "Bad")}";
        }
    }
}

