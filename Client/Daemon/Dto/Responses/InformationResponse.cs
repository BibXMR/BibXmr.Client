using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>DaemonInformation</c>.
    /// </summary>
    internal class DaemonInformationResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public DaemonInformation Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for daemon information.
    /// </summary>
    public class DaemonInformation
    {
        /// <summary>
        /// Gets or sets the adjusted time.
        /// </summary>
        [JsonPropertyName("adjusted_time")]
        public ulong AdjustedTime { get; set; }

        /// <summary>
        /// Gets or sets the alt blocks count.
        /// </summary>
        [JsonPropertyName("alt_blocks_count")]
        public ulong AltBlocksCount { get; set; }

        /// <summary>
        /// Gets or sets the block size limit.
        /// </summary>
        [JsonPropertyName("block_size_limit")]
        public ulong BlockSizeLimit { get; set; }

        /// <summary>
        /// Gets or sets the block size median.
        /// </summary>
        [JsonPropertyName("block_size_median")]
        public ulong BlockSizeMedian { get; set; }

        /// <summary>
        /// Gets or sets the block weight limit.
        /// </summary>
        [JsonPropertyName("block_weight_limit")]
        public ulong BlockWeightLimit { get; set; }

        /// <summary>
        /// Gets or sets the block weight median.
        /// </summary>
        [JsonPropertyName("block_weight_median")]
        public ulong BlockWeightMedian { get; set; }

        /// <summary>
        /// Gets or sets the bootstrap daemon address.
        /// </summary>
        [JsonPropertyName("bootstrap_daemon_address")]
        public string BootstrapDaemonAddress { get; set; }

        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        [JsonPropertyName("credits")]
        public ulong Credits { get; set; }

        /// <summary>
        /// Gets or sets the cumulative difficulty.
        /// </summary>
        [JsonPropertyName("cumulative_difficulty")]
        public ulong CumulativeDifficulty { get; set; }

        /// <summary>
        /// Gets or sets the cumulative difficulty top 64.
        /// </summary>
        [JsonPropertyName("cumulative_difficulty_top64")]
        public ulong CumulativeDifficultyTop64 { get; set; }

        /// <summary>
        /// Gets or sets the database size.
        /// </summary>
        [JsonPropertyName("database_size")]
        public ulong DatabaseSize { get; set; }

        /// <summary>
        /// Gets or sets the difficulty.
        /// </summary>
        [JsonPropertyName("difficulty")]
        public ulong Difficulty { get; set; }

        /// <summary>
        /// Gets or sets the difficulty top 64.
        /// </summary>
        [JsonPropertyName("difficulty_top64")]
        public ulong DifficultyTop64 { get; set; }

        /// <summary>
        /// Gets or sets the free space.
        /// </summary>
        [JsonPropertyName("free_space")]
        public ulong FreeSpace { get; set; }

        /// <summary>
        /// Gets or sets the grey peerlist size.
        /// </summary>
        [JsonPropertyName("grey_peerlist_size")]
        public ulong GreyPeerlistSize { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Gets or sets the height without bootstrap.
        /// </summary>
        [JsonPropertyName("height_without_bootstrap")]
        public ulong HeightWithoutBootstrap { get; set; }

        /// <summary>
        /// Gets or sets the incoming connections count.
        /// </summary>
        [JsonPropertyName("incoming_connections_count")]
        public ulong IncomingConnectionsCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mainnet.
        /// </summary>
        [JsonPropertyName("mainnet")]
        public bool IsMainnet { get; set; }

        /// <summary>
        /// Gets or sets the net type.
        /// </summary>
        [JsonPropertyName("nettype")]
        public string NetType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether offline.
        /// </summary>
        [JsonPropertyName("offline")]
        public bool IsOffline { get; set; }

        /// <summary>
        /// Gets or sets the outgoing connections count.
        /// </summary>
        [JsonPropertyName("outgoing_connections_count")]
        public ulong OutgoingConnectionsCount { get; set; }

        /// <summary>
        /// Gets or sets the rpc connections count.
        /// </summary>
        [JsonPropertyName("rpc_connections_count")]
        public ulong RpcConnectionsCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the connected RPC endpoint is restricted.
        /// </summary>
        [JsonPropertyName("restricted")]
        public bool IsRestrictedRpc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether stagenet.
        /// </summary>
        [JsonPropertyName("stagenet")]
        public bool IsStagenet { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        [JsonPropertyName("start_time")]
        public ulong StartTime { get; set; }

        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        [JsonPropertyName("target")]
        public ulong Target { get; set; }

        /// <summary>
        /// Gets or sets the target height.
        /// </summary>
        [JsonPropertyName("target_height")]
        public ulong TargetHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether testnet.
        /// </summary>
        [JsonPropertyName("testnet")]
        public bool IsTestnet { get; set; }

        /// <summary>
        /// Gets or sets the top block hash.
        /// </summary>
        [JsonPropertyName("top_block_hash")]
        public string TopBlockHash { get; set; }

        /// <summary>
        /// Gets or sets the top hash.
        /// </summary>
        [JsonPropertyName("top_hash")]
        public string TopHash { get; set; }

        /// <summary>
        /// Gets or sets the tx count.
        /// </summary>
        [JsonPropertyName("tx_count")]
        public ulong TxCount { get; set; }

        /// <summary>
        /// Gets or sets the tx pool size.
        /// </summary>
        [JsonPropertyName("tx_pool_size")]
        public ulong TxPoolSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether untrusted.
        /// </summary>
        [JsonPropertyName("untrusted")]
        public bool IsUntrusted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether update available.
        /// </summary>
        [JsonPropertyName("update_available")]
        public bool IsUpdateAvailable { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bootstrap ever used.
        /// </summary>
        [JsonPropertyName("was_bootstrap_ever_used")]
        public bool WasBootstrapEverUsed { get; set; }

        /// <summary>
        /// Gets or sets the white peerlist size.
        /// </summary>
        [JsonPropertyName("white_peerlist_size")]
        public ulong WhitePeerlistSize { get; set; }

        /// <summary>
        /// Gets or sets the wide cumulative difficulty.
        /// </summary>
        [JsonPropertyName("wide_cumulative_difficulty")]
        public string WideCumulativeDifficulty { get; set; }

        /// <summary>
        /// Gets or sets the wide difficulty.
        /// </summary>
        [JsonPropertyName("wide_difficulty")]
        public string WideDifficulty { get; set; }

        /// <summary>
        /// Gets the effective <see cref="MoneroNetwork"/> derived from daemon network flags.
        /// </summary>
        [JsonIgnore]
        public MoneroNetwork NetworkType
        {
            get
            {
                if (IsMainnet)
                {
                    return MoneroNetwork.Mainnet;
                }
                else if (IsStagenet)
                {
                    return MoneroNetwork.Stagenet;
                }
                else if (IsTestnet)
                {
                    return MoneroNetwork.Testnet;
                }
                else
                {
                    throw new InvalidOperationException("Unknown network type");
                }
            }
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            Type typeInfo = typeof(DaemonInformation);
            IEnumerable<string> nonNullPropertyList = typeInfo.GetProperties()
                                              .Where(p => p.GetValue(this) != default)
                                              .Select(p => $"{p.Name}: {p.GetValue(this)} ");
            return string.Join(Environment.NewLine, nonNullPropertyList);
        }
    }
}

