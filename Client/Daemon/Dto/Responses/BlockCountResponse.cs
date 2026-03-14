using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>BlockCount</c>.
    /// </summary>
    internal class BlockCountResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public BlockCountResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>BlockCount</c>.
    /// </summary>
    internal class BlockCountResult
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        [JsonPropertyName("count")]
        public ulong Count { get; set; }

        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether untrusted.
        /// </summary>
        [JsonPropertyName("untrusted")]
        public bool IsUntrusted { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Count}";
        }
    }
}

