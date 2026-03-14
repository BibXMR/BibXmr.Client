using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>HardforkInformation</c>.
    /// </summary>
    internal class HardforkInformationResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public HardforkInformation Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for hardfork information.
    /// </summary>
    public class HardforkInformation
    {
        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        [JsonPropertyName("credits")]
        public ulong Credits { get; set; }

        /// <summary>
        /// Gets or sets the earliest height.
        /// </summary>
        [JsonPropertyName("earliest_height")]
        public ulong EarliestHeight { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [JsonPropertyName("state")]
        public uint State { get; set; }

        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        [JsonPropertyName("threshold")]
        public byte Threshold { get; set; }

        /// <summary>
        /// Gets or sets the top hash.
        /// </summary>
        [JsonPropertyName("top_hash")]
        public string TopHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether untrusted.
        /// </summary>
        [JsonPropertyName("untrusted")]
        public bool IsUntrusted { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [JsonPropertyName("version")]
        public byte Version { get; set; }

        /// <summary>
        /// Gets or sets the votes.
        /// </summary>
        [JsonPropertyName("votes")]
        public uint Votes { get; set; }

        /// <summary>
        /// Gets or sets the voting.
        /// </summary>
        [JsonPropertyName("voting")]
        public byte Voting { get; set; }

        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        [JsonPropertyName("window")]
        public uint Window { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            Type typeInfo = typeof(HardforkInformation);
            IEnumerable<string> nonNullPropertyList = typeInfo.GetProperties()
                                              .Where(p => p.GetValue(this) != default)
                                              .Select(p => $"{p.Name}: {p.GetValue(this)} ");
            return string.Join(Environment.NewLine, nonNullPropertyList);
        }
    }
}

