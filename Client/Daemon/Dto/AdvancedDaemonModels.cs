using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Daemon.Dto.Responses;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Specifies values for key image spent status.
    /// </summary>
    public enum KeyImageSpentStatus
    {
        /// <summary>
        /// Represents unspent.
        /// </summary>
        Unspent = 0,

        /// <summary>
        /// Represents spent in blockchain.
        /// </summary>
        SpentInBlockchain = 1,

        /// <summary>
        /// Represents spent in pool.
        /// </summary>
        SpentInPool = 2,
    }

    /// <summary>
    /// Represents peer list data.
    /// </summary>
    public sealed class PeerList
    {
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
    /// Represents transaction pool stats data.
    /// </summary>
    public sealed class TransactionPoolStats
    {
        /// <summary>
        /// Gets or sets the bytes total.
        /// </summary>
        [JsonPropertyName("bytes_total")]
        public ulong BytesTotal { get; set; }

        /// <summary>
        /// Gets or sets the bytes min.
        /// </summary>
        [JsonPropertyName("bytes_min")]
        public ulong BytesMin { get; set; }

        /// <summary>
        /// Gets or sets the bytes max.
        /// </summary>
        [JsonPropertyName("bytes_max")]
        public ulong BytesMax { get; set; }

        /// <summary>
        /// Gets or sets the transactions total.
        /// </summary>
        [JsonPropertyName("txs_total")]
        public ulong TransactionsTotal { get; set; }
    }

    /// <summary>
    /// Represents daemon output request data.
    /// </summary>
    public sealed class DaemonOutputRequest
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        [JsonPropertyName("index")]
        public ulong Index { get; set; }
    }

    /// <summary>
    /// Represents daemon output data.
    /// </summary>
    public sealed class DaemonOutput
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        [JsonPropertyName("index")]
        public ulong Index { get; set; }

        /// <summary>
        /// Gets or sets the mask.
        /// </summary>
        [JsonPropertyName("mask")]
        public string? Mask { get; set; }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        [JsonPropertyName("txid")]
        public string? TransactionId { get; set; }
    }

    /// <summary>
    /// Represents output distribution data.
    /// </summary>
    public sealed class OutputDistribution
    {
        /// <summary>
        /// Gets or sets the collection of distributions.
        /// </summary>
        [JsonPropertyName("distributions")]
        public List<Distribution> Distributions { get; set; } = [];
    }

    /// <summary>
    /// Represents bandwidth limit data.
    /// </summary>
    public sealed class BandwidthLimit
    {
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
    /// Represents mining status data.
    /// </summary>
    public sealed class MiningStatus
    {
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
    /// Represents submit raw transaction data.
    /// </summary>
    public sealed class SubmitRawTransaction
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
    /// Represents daemon update check data.
    /// </summary>
    public sealed class DaemonUpdateCheck
    {
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
    }

    /// <summary>
    /// Represents daemon update download data.
    /// </summary>
    public sealed class DaemonUpdateDownload
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        [JsonPropertyName("path")]
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    /// <summary>
    /// Represents network stats data.
    /// </summary>
    public sealed class NetworkStats
    {
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
    /// Represents public nodes data.
    /// </summary>
    public sealed class PublicNodes
    {
        /// <summary>
        /// Gets or sets the collection of white nodes.
        /// </summary>
        [JsonPropertyName("white")]
        public List<string> WhiteNodes { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of gray nodes.
        /// </summary>
        [JsonPropertyName("gray")]
        public List<string> GrayNodes { get; set; } = [];
    }
}


