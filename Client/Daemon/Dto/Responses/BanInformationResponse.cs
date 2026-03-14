using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>GetBans</c>.
    /// </summary>
    internal class GetBansResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public BanInformation Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for ban information.
    /// </summary>
    public class BanInformation
    {
        /// <summary>
        /// Gets or sets the collection of bans.
        /// </summary>
        [JsonPropertyName("bans")]
        public List<Ban> Bans { get; set; } = [];

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
            return string.Join(", ", Bans);
        }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for ban.
    /// </summary>
    public class Ban
    {
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        [JsonPropertyName("host")]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the ip.
        /// </summary>
        [JsonPropertyName("ip")]
        public ulong IP { get; set; }

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
            return $"{Host} ({Seconds})";
        }
    }
}

