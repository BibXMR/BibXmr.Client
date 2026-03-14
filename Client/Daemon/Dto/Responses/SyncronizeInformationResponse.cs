using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SyncronizeInformation</c>.
    /// </summary>
    internal class SyncronizeInformationResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SyncronizationInformation Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for syncronization information.
    /// </summary>
    public class SyncronizationInformation
    {
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the target height.
        /// </summary>
        [JsonPropertyName("target_height")]
        public ulong TargetHeight { get; set; }

        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        [JsonPropertyName("credits")]
        public ulong Credits { get; set; }

        /// <summary>
        /// Gets or sets the overview.
        /// </summary>
        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        /// <summary>
        /// Gets or sets the collection of peers.
        /// </summary>
        [JsonPropertyName("peers")]
        public List<GeneralSyncronizationInformation> Peers { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of spans.
        /// </summary>
        [JsonPropertyName("spans")]
        public List<Span> Spans { get; set; } = [];

        /// <summary>
        /// Gets or sets a value indicating whether untrusted.
        /// </summary>
        [JsonPropertyName("untrusted")]
        public bool IsUntrusted { get; set; }

        /// <summary>
        /// Gets or sets the next needed pruning seed.
        /// </summary>
        [JsonPropertyName("next_needed_pruning_seed")]
        public uint NextNeededPruningSeed { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{Height}] - TargetHeight: {TargetHeight} - Overview: {Overview}");
            sb.AppendLine("Peers:");
            sb.AppendLine(string.Join(Environment.NewLine, Peers));
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for general syncronization information.
    /// </summary>
    public class GeneralSyncronizationInformation
    {
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        [JsonPropertyName("info")]
        public Connection Connection { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Connection}";
        }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for span.
    /// </summary>
    public class Span
    {
        /// <summary>
        /// Gets or sets the start block height.
        /// </summary>
        [JsonPropertyName("start_block_height")]
        public ulong StartBlockHeight { get; set; }

        /// <summary>
        /// Gets or sets the blocks.
        /// </summary>
        [JsonPropertyName("nblocks")]
        public ulong N_Blocks { get; set; }

        /// <summary>
        /// Gets or sets the connection id.
        /// </summary>
        [JsonPropertyName("connection_id")]
        public string ConnectionID { get; set; }

        /// <summary>
        /// Gets or sets the rate.
        /// </summary>
        [JsonPropertyName("rate")]
        public uint Rate { get; set; }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        [JsonPropertyName("speed")]
        public uint Speed { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        [JsonPropertyName("size")]
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or sets the remote address.
        /// </summary>
        [JsonPropertyName("remote_address")]
        public string RemoteAddress { get; set; }
    }
}

