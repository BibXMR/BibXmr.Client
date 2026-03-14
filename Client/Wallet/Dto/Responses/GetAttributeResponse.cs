using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>GetAttribute</c>.
    /// </summary>
    internal class GetAttributeResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public GetAttributeResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>GetAttribute</c>.
    /// </summary>
    internal class GetAttributeResult
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}

