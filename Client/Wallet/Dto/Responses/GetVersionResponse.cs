using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>GetRpcVersion</c>.
    /// </summary>
    internal class GetRpcVersionResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public GetVersionResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>GetVersion</c>.
    /// </summary>
    internal class GetVersionResult
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [JsonPropertyName("version")]
        public uint Version { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Version}";
        }
    }
}

