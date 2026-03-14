using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>Connection</c>.
    /// </summary>
    internal class ConnectionResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ConnectionResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>Connection</c>.
    /// </summary>
    internal class ConnectionResult
    {
        /// <summary>
        /// Gets or sets the collection of connections.
        /// </summary>
        [JsonPropertyName("connections")]
        public List<Connection> Connections { get; set; } = [];

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
            return string.Join(Environment.NewLine, Connections);
        }
    }
}

