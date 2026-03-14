using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Daemon.Dto.Requests;
using BibXmr.Client.Daemon.Dto.Responses;

namespace BibXmr.Client.Daemon
{
    /// <summary>
    /// Represents the full daemon RPC surface that requires an unrestricted RPC endpoint.
    /// </summary>
    /// <remarks>
    /// Extends <see cref="IRestrictedMoneroDaemonClient"/> with privileged operations such as
    /// peer management, mining control, and daemon configuration that are unavailable on
    /// restricted RPC endpoints.
    /// </remarks>
    public interface IUnrestrictedMoneroDaemonClient : IRestrictedMoneroDaemonClient
    {
        /// <summary>
        /// Gets incoming and outgoing peer connections.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Connection details.</returns>
        Task<List<Connection>> GetConnectionsAsync(CancellationToken token = default);

        /// <summary>
        /// Gets currently banned hosts.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Ban entries.</returns>
        Task<List<Ban>> GetBanInformationAsync(CancellationToken token = default);

        /// <summary>
        /// Gets ban status for a specific address.
        /// </summary>
        /// <param name="address">The address to evaluate.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Ban status information.</returns>
        Task<BanStatus> GetBanStatusAsync(string address, CancellationToken token = default);

        /// <summary>
        /// Flushes transaction ids from the mempool.
        /// </summary>
        /// <param name="txids">Transaction ids to remove. Pass empty to flush all entries.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Daemon status string.</returns>
        Task<string> FlushTransactionPoolAsync(IEnumerable<string> txids, CancellationToken token = default);

        /// <summary>
        /// Gets coinbase and fee sums for a range of blocks.
        /// </summary>
        /// <param name="height">Range starting height.</param>
        /// <param name="count">Number of blocks to include.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Coinbase sum information.</returns>
        Task<CoinbaseTransactionSum> GetCoinbaseTransactionSumAsync(ulong height, uint count, CancellationToken token = default);

        /// <summary>
        /// Gets alternate chain details known by the daemon.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Alternate chain details.</returns>
        Task<List<Chain>> GetAlternateChainsAsync(CancellationToken token = default);

        /// <summary>
        /// Relays a transaction to peers.
        /// </summary>
        /// <param name="hex">The transaction blob in hex format.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The relayed transaction hash.</returns>
        Task<string> RelayTransactionAsync(string hex, CancellationToken token = default);

        /// <summary>
        /// Gets daemon synchronization diagnostics.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Synchronization information.</returns>
        Task<SyncronizationInformation> SyncInformationAsync(CancellationToken token = default);

        /// <summary>
        /// Applies ban rules for peers using typed <see cref="NodeBan"/> objects.
        /// </summary>
        /// <param name="bans">Ban rule entries.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Daemon status string.</returns>
        Task<string> SetBansAsync(IEnumerable<NodeBan> bans, CancellationToken token = default);

        /// <summary>
        /// Gets daemon peer list information.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Peer list data.</returns>
        Task<PeerList> GetPeerListAsync(CancellationToken token = default);

        /// <summary>
        /// Sets daemon bandwidth limits.
        /// </summary>
        /// <param name="limitDown">Optional download limit in kB/s.</param>
        /// <param name="limitUp">Optional upload limit in kB/s.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The resulting bandwidth limits.</returns>
        Task<BandwidthLimit> SetBandwidthLimitAsync(int? limitDown = null, int? limitUp = null, CancellationToken token = default);

        /// <summary>
        /// Gets the current maximum outgoing peer count.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The current outgoing peer limit.</returns>
        Task<int> GetOutgoingPeerLimitAsync(CancellationToken token = default);

        /// <summary>
        /// Gets the current maximum incoming peer count.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The current incoming peer limit.</returns>
        Task<int> GetIncomingPeerLimitAsync(CancellationToken token = default);

        /// <summary>
        /// Sets the maximum outgoing peer count.
        /// </summary>
        /// <param name="count">Outgoing peer limit.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetOutgoingPeerLimitAsync(int count, CancellationToken token = default);

        /// <summary>
        /// Sets the maximum incoming peer count.
        /// </summary>
        /// <param name="count">Incoming peer limit.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetIncomingPeerLimitAsync(int count, CancellationToken token = default);

        /// <summary>
        /// Starts daemon mining.
        /// </summary>
        /// <param name="minerAddress">Miner reward address.</param>
        /// <param name="threadCount">Number of mining threads.</param>
        /// <param name="backgroundMining">Whether background mining is enabled.</param>
        /// <param name="ignoreBattery">Whether battery restrictions are ignored.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartMiningAsync(string minerAddress, ulong threadCount, bool backgroundMining = false, bool ignoreBattery = true, CancellationToken token = default);

        /// <summary>
        /// Stops daemon mining.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StopMiningAsync(CancellationToken token = default);

        /// <summary>
        /// Gets current mining status.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Mining status details.</returns>
        Task<MiningStatus> GetMiningStatusAsync(CancellationToken token = default);

        /// <summary>
        /// Checks whether a daemon update is available.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Update availability metadata.</returns>
        Task<DaemonUpdateCheck> CheckForUpdateAsync(CancellationToken token = default);

        /// <summary>
        /// Downloads an available daemon update.
        /// </summary>
        /// <param name="path">Optional destination directory.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Download result metadata.</returns>
        Task<DaemonUpdateDownload> DownloadUpdateAsync(string? path = null, CancellationToken token = default);

        /// <summary>
        /// Requests daemon shutdown.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StopDaemonAsync(CancellationToken token = default);

        /// <summary>
        /// Gets daemon-level network statistics.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Network statistics.</returns>
        Task<NetworkStats> GetNetworkStatsAsync(CancellationToken token = default);

        /// <summary>
        /// Flushes in-memory daemon cache data.
        /// </summary>
        /// <param name="badTxs">Whether bad transaction cache entries are flushed.</param>
        /// <param name="badBlocks">Whether bad block cache entries are flushed.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task FlushCacheAsync(bool badTxs = false, bool badBlocks = false, CancellationToken token = default);

        /// <summary>
        /// Persists blockchain state to disk.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SaveBlockchainAsync(CancellationToken token = default);

        /// <summary>
        /// Enables or disables hash rate logging.
        /// </summary>
        /// <param name="visible">Whether hash rate logging should be visible.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetLogHashRateAsync(bool visible, CancellationToken token = default);

        /// <summary>
        /// Sets daemon log level.
        /// </summary>
        /// <param name="level">New log level.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetLogLevelAsync(uint level, CancellationToken token = default);

        /// <summary>
        /// Sets daemon log categories.
        /// </summary>
        /// <param name="categories">Category expression value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetLogCategoriesAsync(string categories, CancellationToken token = default);
    }
}
