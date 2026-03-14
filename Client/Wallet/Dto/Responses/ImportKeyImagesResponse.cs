using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>ImportKeyImages</c>.
    /// </summary>
    internal class ImportKeyImagesResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ImportKeyImages Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for import key images.
    /// </summary>
    public class ImportKeyImages
    {
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Gets or sets the spent.
        /// </summary>
        [JsonPropertyName("spent")]
        public ulong Spent { get; set; }

        /// <summary>
        /// Gets or sets the unspent.
        /// </summary>
        [JsonPropertyName("unspent")]
        public ulong Unspent { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"[{Height}] Unspent {MoneroAmount.PiconeroToXmr(Unspent).ToString(MoneroAmount.PrecisionFormat)} / Spend {MoneroAmount.PiconeroToXmr(Spent).ToString(MoneroAmount.PrecisionFormat)}";
        }
    }
}

