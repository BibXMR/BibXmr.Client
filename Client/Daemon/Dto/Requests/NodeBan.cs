using System.Text.Json.Serialization;

namespace BibXmr.Client.Daemon.Dto.Requests
{
    /// <summary>
    /// Represents a daemon RPC request payload for node ban.
    /// </summary>
    public class NodeBan
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
        /// Gets or sets a value indicating whether banned.
        /// </summary>
        [JsonPropertyName("ban")]
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
            return $"{Host}:{IP} - {(IsBanned ? "Banned" : "Not Banned")} ({Seconds})";
        }
    }
}

