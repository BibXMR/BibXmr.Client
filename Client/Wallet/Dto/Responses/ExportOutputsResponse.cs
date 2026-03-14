using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>ExportOutputs</c>.
    /// </summary>
    internal class ExportOutputsResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ExportOutputsResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>ExportOutputs</c>.
    /// </summary>
    internal class ExportOutputsResult
    {
        /// <summary>
        /// Gets or sets the outputs data hex.
        /// </summary>
        [JsonPropertyName("outputs_data_hex")]
        public string OutputsDataHex { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return OutputsDataHex;
        }
    }
}

