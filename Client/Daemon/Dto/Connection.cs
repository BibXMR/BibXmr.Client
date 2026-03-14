using System;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for connection.
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the address type.
        /// </summary>
        [JsonPropertyName("address_type")]
        public byte AddressType { get; set; }

        /// <summary>
        /// Gets or sets the avg download.
        /// </summary>
        [JsonPropertyName("avg_download")]
        public ulong AvgDownload { get; set; }

        /// <summary>
        /// Gets or sets the avg upload.
        /// </summary>
        [JsonPropertyName("avg_upload")]
        public ulong AvgUpload { get; set; }

        /// <summary>
        /// Gets or sets the connection id.
        /// </summary>
        [JsonPropertyName("connection_id")]
        public string ConnectionID { get; set; }

        /// <summary>
        /// Gets or sets the current download.
        /// </summary>
        [JsonPropertyName("current_download")]
        public uint CurrentDownload { get; set; }

        /// <summary>
        /// Gets or sets the current upload.
        /// </summary>
        [JsonPropertyName("current_upload")]
        public uint CurrentUpload { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        [JsonPropertyName("host")]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the live time.
        /// </summary>
        [JsonPropertyName("live_time")]
        public ulong LiveTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether local id.
        /// </summary>
        [JsonPropertyName("local_ip")]
        public bool IsLocalID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether localhost.
        /// </summary>
        [JsonPropertyName("localhost")]
        public bool IsLocalhost { get; set; }

        /// <summary>
        /// Gets or sets the peer id.
        /// </summary>
        [JsonPropertyName("peer_id")]
        public string PeerID { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        [JsonPropertyName("port")]
        [JsonConverter(typeof(JsonStringOrNumberConverter))]
        public string Port { get; set; }

        /// <summary>
        /// Gets or sets the pruning seed.
        /// </summary>
        [JsonPropertyName("pruning_seed")]
        public uint PruningSeed { get; set; }

        /// <summary>
        /// Gets or sets the recv count.
        /// </summary>
        [JsonPropertyName("recv_count")]
        public ulong RecvCount { get; set; }

        /// <summary>
        /// Gets or sets the recv idle time.
        /// </summary>
        [JsonPropertyName("recv_idle_time")]
        public ulong RecvIdleTime { get; set; }

        /// <summary>
        /// Gets or sets the recv credits per hash.
        /// </summary>
        [JsonPropertyName("recv_credits_per_hash")]
        public uint RecvCreditsPerHash { get; set; }

        /// <summary>
        /// Gets or sets the rpc port.
        /// </summary>
        [JsonPropertyName("rpc_port")]
        public ushort RpcPort { get; set; }

        /// <summary>
        /// Gets or sets the send count.
        /// </summary>
        [JsonPropertyName("send_count")]
        public ulong SendCount { get; set; }

        /// <summary>
        /// Gets or sets the send idle time.
        /// </summary>
        [JsonPropertyName("send_idle_time")]
        public ulong SendIdleTime { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the support flags.
        /// </summary>
        [JsonPropertyName("support_flags")]
        public uint SupportFlags { get; set; }

        /// <summary>
        /// Gets or sets the connection time.
        /// </summary>
        [JsonIgnore]
        public TimeSpan ConnectionTime
        {
            get
            {
                return TimeSpan.FromSeconds(LiveTime);
            }
        }

        /// <summary>
        /// Gets the inferred connection direction based on the daemon-reported state text.
        /// </summary>
        [JsonIgnore]
        public ConnectionDirection Direction => string.IsNullOrEmpty(State)
            ? ConnectionDirection.Unknown
            : State.Contains("incoming", StringComparison.OrdinalIgnoreCase)
                ? ConnectionDirection.Incoming
                : ConnectionDirection.Outgoing;

        /// <summary>
        /// Gets or sets a value indicating whether incoming.
        /// </summary>
        [JsonIgnore]
        public bool IsIncoming => Direction == ConnectionDirection.Incoming;

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Address} ({State})";
        }
    }
}

