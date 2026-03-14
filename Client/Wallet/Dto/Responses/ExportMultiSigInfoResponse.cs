using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>ExportMultiSigInfo</c>.
    /// </summary>
    internal class ExportMultiSigInfoResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ExportMultiSigInformation Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for export multi sig information.
    /// </summary>
    public class ExportMultiSigInformation
    {
        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        [JsonPropertyName("info")]
        public string Information { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return Information;
        }
    }
}

