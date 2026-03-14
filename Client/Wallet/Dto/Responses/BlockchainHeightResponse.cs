using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>BlockchainHeight</c>.
    /// </summary>
    internal class BlockchainHeightResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public BlockchainHeightResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>BlockchainHeight</c>.
    /// </summary>
    internal class BlockchainHeightResult
    {
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Height}";
        }
    }
}

