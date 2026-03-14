using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>ImportOutputs</c>.
    /// </summary>
    internal class ImportOutputsResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ImportOutputsResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>ImportOutputs</c>.
    /// </summary>
    internal class ImportOutputsResult
    {
        /// <summary>
        /// Gets or sets the num imported.
        /// </summary>
        [JsonPropertyName("num_imported")]
        public ulong NumImported { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{NumImported} imported";
        }
    }
}

