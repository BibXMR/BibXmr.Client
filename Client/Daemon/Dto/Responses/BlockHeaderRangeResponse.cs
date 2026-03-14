using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>BlockHeaderRange</c>.
    /// </summary>
    internal class BlockHeaderRangeResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public BlockHeaderRangeResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>BlockHeaderRange</c>.
    /// </summary>
    internal class BlockHeaderRangeResult
    {
        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        [JsonPropertyName("credits")]
        public ulong Credits { get; set; }

        /// <summary>
        /// Gets or sets the collection of headers.
        /// </summary>
        [JsonPropertyName("headers")]
        public List<BlockHeader> Headers { get; set; } = [];

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
        /// Gets or sets the top hash.
        /// </summary>
        [JsonPropertyName("top_hash")]
        public string TopHash { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, Headers);
        }
    }
}

