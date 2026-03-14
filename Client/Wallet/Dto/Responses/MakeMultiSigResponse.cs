using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>MakeMultiSig</c>.
    /// </summary>
    internal class MakeMultiSigResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public MakeMultiSig Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for make multi sig.
    /// </summary>
    public class MakeMultiSig
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

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
            return $"{Address} - {MultiSigInformation}";
        }
    }
}

