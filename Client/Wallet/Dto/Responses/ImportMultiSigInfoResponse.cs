using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>ImportMultiSigInfo</c>.
    /// </summary>
    internal class ImportMultiSigInfoResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ImportMultiSigInformation Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for import multi sig information.
    /// </summary>
    public class ImportMultiSigInformation
    {
        /// <summary>
        /// Gets or sets the outputs.
        /// </summary>
        [JsonPropertyName("n_outputs")]
        public uint N_Outputs { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{N_Outputs}";
        }
    }
}

