using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>StatusOnly</c>.
    /// </summary>
    internal sealed class StatusOnlyResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>BlockHash</c>.
    /// </summary>
    internal sealed class BlockHashResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public string? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>AlternateBlockHashes</c>.
    /// </summary>
    internal sealed class AlternateBlockHashesResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the collection of block hashes.
        /// </summary>
        [JsonPropertyName("blks_hashes")]
        public List<string> BlockHashes { get; set; } = [];
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>PeerList</c>.
    /// </summary>
    internal sealed class PeerListResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the collection of white list.
        /// </summary>
        [JsonPropertyName("white_list")]
        public List<Connection> WhiteList { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of gray list.
        /// </summary>
        [JsonPropertyName("gray_list")]
        public List<Connection> GrayList { get; set; } = [];
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>TransactionPoolStats</c>.
    /// </summary>
    internal sealed class TransactionPoolStatsResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the pool stats.
        /// </summary>
        [JsonPropertyName("pool_stats")]
        public TransactionPoolStats? PoolStats { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>KeyImageSpentStatuses</c>.
    /// </summary>
    internal sealed class KeyImageSpentStatusesResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the collection of spent status.
        /// </summary>
        [JsonPropertyName("spent_status")]
        public List<int> SpentStatus { get; set; } = [];
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>Outputs</c>.
    /// </summary>
    internal sealed class OutputsResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the collection of outputs.
        /// </summary>
        [JsonPropertyName("outs")]
        public List<DaemonOutput> Outputs { get; set; } = [];
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>OutputDistribution</c>.
    /// </summary>
    internal sealed class OutputDistributionResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public OutputDistributionResult? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>OutputDistribution</c>.
    /// </summary>
    internal sealed class OutputDistributionResult
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the collection of distributions.
        /// </summary>
        [JsonPropertyName("distributions")]
        public List<Distribution> Distributions { get; set; } = [];
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>FlushCache</c>.
    /// </summary>
    internal sealed class FlushCacheResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public FlushCacheResult? Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>FlushCache</c>.
    /// </summary>
    internal sealed class FlushCacheResult
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>BandwidthLimit</c>.
    /// </summary>
    internal sealed class BandwidthLimitResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the limit down.
        /// </summary>
        [JsonPropertyName("limit_down")]
        public int LimitDown { get; set; }

        /// <summary>
        /// Gets or sets the limit up.
        /// </summary>
        [JsonPropertyName("limit_up")]
        public int LimitUp { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>PeerLimit</c>.
    /// </summary>
    internal sealed class PeerLimitResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the in peers.
        /// </summary>
        [JsonPropertyName("in_peers")]
        public int? InPeers { get; set; }

        /// <summary>
        /// Gets or sets the out peers.
        /// </summary>
        [JsonPropertyName("out_peers")]
        public int? OutPeers { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>MiningStatus</c>.
    /// </summary>
    internal sealed class MiningStatusResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the threads count.
        /// </summary>
        [JsonPropertyName("threads_count")]
        public ulong ThreadsCount { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        [JsonPropertyName("speed")]
        public ulong Speed { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>SubmitRawTransaction</c>.
    /// </summary>
    internal sealed class SubmitRawTransactionResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether double spend.
        /// </summary>
        [JsonPropertyName("double_spend")]
        public bool DoubleSpend { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>DaemonUpdate</c>.
    /// </summary>
    internal sealed class DaemonUpdateResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether update available.
        /// </summary>
        [JsonPropertyName("update")]
        public bool UpdateAvailable { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [JsonPropertyName("version")]
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        [JsonPropertyName("path")]
        public string? Path { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>NetworkStats</c>.
    /// </summary>
    internal sealed class NetworkStatsResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the total bytes in.
        /// </summary>
        [JsonPropertyName("total_bytes_in")]
        public ulong TotalBytesIn { get; set; }

        /// <summary>
        /// Gets or sets the total bytes out.
        /// </summary>
        [JsonPropertyName("total_bytes_out")]
        public ulong TotalBytesOut { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC response envelope for <c>PublicNodes</c>.
    /// </summary>
    internal sealed class PublicNodesResponse
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the collection of white.
        /// </summary>
        [JsonPropertyName("white")]
        public List<string> White { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of gray.
        /// </summary>
        [JsonPropertyName("gray")]
        public List<string> Gray { get; set; } = [];
    }
}


