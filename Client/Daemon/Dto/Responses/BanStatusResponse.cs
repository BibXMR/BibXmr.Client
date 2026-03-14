using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>GetBanStatus</c>.
    /// </summary>
    internal class GetBanStatusResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public BanStatus Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for ban status.
    /// </summary>
    public class BanStatus
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether banned.
        /// </summary>
        [JsonPropertyName("banned")]
        public bool IsBanned { get; set; }

        /// <summary>
        /// Gets or sets the seconds.
        /// </summary>
        [JsonPropertyName("seconds")]
        public uint Seconds { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{(IsBanned ? $"Banned for {Seconds} seconds" : "Not banned")}";
        }
    }
}

