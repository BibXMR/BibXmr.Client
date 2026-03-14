using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Daemon.Dto.Requests;
using BibXmr.Client.Daemon.Dto.Responses;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

#if NET9_0_OR_GREATER
using SyncLock = System.Threading.Lock;
#else
using SyncLock = System.Object;
#endif

namespace BibXmr.Client.Daemon;

/// <summary>
/// High-level client for interacting with <c>monerod</c> via its RPC interface.
/// </summary>
public partial class MoneroDaemonClient : IUnrestrictedMoneroDaemonClient
{
    private readonly RpcCommunicator _moneroRpcCommunicator;

    /// <summary>
    /// Synchronization lock guarding the disposal flag.
    /// </summary>
    private readonly object _disposalLock = new();

    /// <summary>
    /// Synchronization lock guarding the subscriptions list.
    /// </summary>
    private readonly object _subscriptionLock = new();
    private readonly List<IAsyncDisposable> _subscriptions = [];
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MoneroDaemonClient"/> class using an existing RPC communicator.
    /// </summary>
    /// <param name="rpcCommunicator">The RPC communicator used for daemon communication.</param>
    private MoneroDaemonClient(RpcCommunicator rpcCommunicator)
    {
        _moneroRpcCommunicator = rpcCommunicator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoneroDaemonClient"/> class targeting the given host and port.
    /// </summary>
    /// <param name="url">The daemon host name or IP address.</param>
    /// <param name="port">The daemon RPC port number.</param>
    private MoneroDaemonClient(string url, uint port)
    {
        _moneroRpcCommunicator = new RpcCommunicator(url, port, ConnectionType.Daemon);
    }

    /// <summary>
    /// Initialize a Monero Daemon Client using default network settings (&lt;localhost&gt;:&lt;defaultport&gt;).
    /// </summary>
    private MoneroDaemonClient(MoneroNetwork networkType)
    {
        _moneroRpcCommunicator = new RpcCommunicator(networkType, ConnectionType.Daemon);
    }

    /// <summary>
    /// Create a daemon client targeting the given host and port.
    /// </summary>
    public static Task<MoneroDaemonClient> CreateAsync(string url, uint port, CancellationToken cancellationToken = default)
    {
        var moneroDaemonClient = new MoneroDaemonClient(url, port);
        return moneroDaemonClient.InitializeAsync();
    }

    /// <summary>
    /// Create a daemon client using an externally-managed <see cref="HttpClient"/>.
    /// </summary>
    public static Task<MoneroDaemonClient> CreateAsync(HttpClient httpClient, Uri baseAddress, MoneroRpcClientOptions? options = null, CancellationToken ct = default)
    {
        var rpcCommunicator = new RpcCommunicator(httpClient, baseAddress, ConnectionType.Daemon, options);
        var moneroDaemonClient = new MoneroDaemonClient(rpcCommunicator);
        return moneroDaemonClient.InitializeAsync();
    }

    /// <summary>
    /// Create a daemon client using default settings for the specified Monero network.
    /// </summary>
    public static Task<MoneroDaemonClient> CreateAsync(MoneroNetwork moneroNetwork, CancellationToken cancellationToken = default)
    {
        var moneroDaemonClient = new MoneroDaemonClient(moneroNetwork);
        return moneroDaemonClient.InitializeAsync();
    }

    /// <summary>
    /// Create a daemon client constrained to the restricted RPC surface.
    /// </summary>
    /// <param name="url">Daemon host name or IP address.</param>
    /// <param name="port">Daemon RPC port.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A restricted daemon client interface.</returns>
    public static async Task<IRestrictedMoneroDaemonClient> CreateRestrictedAsync(string url, uint port, CancellationToken cancellationToken = default)
    {
        return await CreateAsync(url, port, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a daemon client constrained to the restricted RPC surface.
    /// </summary>
    /// <param name="httpClient">Externally managed HTTP client.</param>
    /// <param name="baseAddress">Daemon base address.</param>
    /// <param name="options">Optional RPC client options.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A restricted daemon client interface.</returns>
    public static async Task<IRestrictedMoneroDaemonClient> CreateRestrictedAsync(HttpClient httpClient, Uri baseAddress, MoneroRpcClientOptions? options = null, CancellationToken ct = default)
    {
        return await CreateAsync(httpClient, baseAddress, options, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a daemon client constrained to the restricted RPC surface.
    /// </summary>
    /// <param name="moneroNetwork">Network preset to connect to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A restricted daemon client interface.</returns>
    public static async Task<IRestrictedMoneroDaemonClient> CreateRestrictedAsync(MoneroNetwork moneroNetwork, CancellationToken cancellationToken = default)
    {
        return await CreateAsync(moneroNetwork, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a daemon client constrained to the unrestricted RPC surface.
    /// </summary>
    /// <param name="url">Daemon host name or IP address.</param>
    /// <param name="port">Daemon RPC port.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An unrestricted daemon client interface.</returns>
    public static async Task<IUnrestrictedMoneroDaemonClient> CreateUnrestrictedAsync(string url, uint port, CancellationToken cancellationToken = default)
    {
        return await CreateAsync(url, port, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a daemon client constrained to the unrestricted RPC surface.
    /// </summary>
    /// <param name="httpClient">Externally managed HTTP client.</param>
    /// <param name="baseAddress">Daemon base address.</param>
    /// <param name="options">Optional RPC client options.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An unrestricted daemon client interface.</returns>
    public static async Task<IUnrestrictedMoneroDaemonClient> CreateUnrestrictedAsync(HttpClient httpClient, Uri baseAddress, MoneroRpcClientOptions? options = null, CancellationToken ct = default)
    {
        return await CreateAsync(httpClient, baseAddress, options, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Create a daemon client constrained to the unrestricted RPC surface.
    /// </summary>
    /// <param name="moneroNetwork">Network preset to connect to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An unrestricted daemon client interface.</returns>
    public static async Task<IUnrestrictedMoneroDaemonClient> CreateUnrestrictedAsync(MoneroNetwork moneroNetwork, CancellationToken cancellationToken = default)
    {
        return await CreateAsync(moneroNetwork, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Detects whether the target daemon endpoint is restricted or unrestricted.
    /// </summary>
    /// <param name="url">Daemon host name or IP address.</param>
    /// <param name="port">Daemon RPC port.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The endpoint RPC access mode.</returns>
    public static async Task<MoneroDaemonRpcAccess> DetectRpcAccessAsync(string url, uint port, CancellationToken cancellationToken = default)
    {
        using MoneroDaemonClient client = await CreateAsync(url, port, cancellationToken).ConfigureAwait(false);
        return await DetectRpcAccessAsync(client, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Detects whether the target daemon endpoint is restricted or unrestricted.
    /// </summary>
    /// <param name="httpClient">Externally managed HTTP client.</param>
    /// <param name="baseAddress">Daemon base address.</param>
    /// <param name="options">Optional RPC client options.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The endpoint RPC access mode.</returns>
    public static async Task<MoneroDaemonRpcAccess> DetectRpcAccessAsync(HttpClient httpClient, Uri baseAddress, MoneroRpcClientOptions? options = null, CancellationToken ct = default)
    {
        using MoneroDaemonClient client = await CreateAsync(httpClient, baseAddress, options, ct).ConfigureAwait(false);
        return await DetectRpcAccessAsync(client, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Detects whether the network preset daemon endpoint is restricted or unrestricted.
    /// </summary>
    /// <param name="moneroNetwork">Network preset to connect to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The endpoint RPC access mode.</returns>
    public static async Task<MoneroDaemonRpcAccess> DetectRpcAccessAsync(MoneroNetwork moneroNetwork, CancellationToken cancellationToken = default)
    {
        using MoneroDaemonClient client = await CreateAsync(moneroNetwork, cancellationToken).ConfigureAwait(false);
        return await DetectRpcAccessAsync(client, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously disposes the client and its underlying transport resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        lock (_disposalLock)
        {
            if (_disposed)
            {
                return;
            }
            else
            {
                _disposed = true;
            }
        }

        List<IAsyncDisposable> subscriptionsToDispose;
        lock (_subscriptionLock)
        {
            subscriptionsToDispose = [.. _subscriptions];
            _subscriptions.Clear();
        }

        foreach (IAsyncDisposable subscription in subscriptionsToDispose)
        {
            await subscription.DisposeAsync().ConfigureAwait(false);
        }

        _moneroRpcCommunicator.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose the client and its underlying transport resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Look up how many blocks are in the longest chain known to the node.
    /// </summary>
    public async Task<ulong> GetBlockCountAsync(CancellationToken token = default)
    {
        BlockCountResponse result = await _moneroRpcCommunicator.GetBlockCountAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBlockCountAsync));
        return result.Result!.Count;
    }

    /// <summary>
    /// Block header information for the most recent block is easily retrieved with this method.
    /// </summary>
    public async Task<BlockHeader> GetLastBlockHeaderAsync(CancellationToken token = default)
    {
        BlockHeaderResponse result = await _moneroRpcCommunicator.GetLastBlockHeaderAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetLastBlockHeaderAsync));
        return result.Result!.BlockHeader;
    }

    /// <summary>
    ///  Retrieve basic information about the block by hash.
    /// </summary>
    public async Task<BlockHeader> GetBlockHeaderByHashAsync(string hash, CancellationToken token = default)
    {
        BlockHeaderResponse result = await _moneroRpcCommunicator.GetBlockHeaderByHashAsync(hash, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBlockHeaderByHashAsync));
        return result.Result!.BlockHeader;
    }

    /// <summary>
    ///  Retrieve basic information about the block by height.
    /// </summary>
    public async Task<BlockHeader> GetBlockHeaderByHeightAsync(ulong height, CancellationToken token = default)
    {
        BlockHeaderResponse result = await _moneroRpcCommunicator.GetBlockHeaderByHeightAsync(height, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBlockHeaderByHeightAsync));
        return result.Result!.BlockHeader;
    }

    /// <summary>
    /// Similar to get_block_header_by_height above, but for a range of blocks.
    /// This method includes a starting block height and an ending block height as parameters to retrieve basic information about the range of blocks.
    /// </summary>
    /// <param name="startHeight">The starting block's height.</param>
    /// <param name="endHeight">The ending block's height.</param>
    /// <param name="token">The cancellation token.</param>
    public async Task<List<BlockHeader>> GetBlockHeaderRangeAsync(uint startHeight, uint endHeight, CancellationToken token = default)
    {
        BlockHeaderRangeResponse result = await _moneroRpcCommunicator.GetBlockHeaderRangeAsync(startHeight, endHeight, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBlockHeaderRangeAsync));
        return result.Result!.Headers;
    }

    /// <summary>
    /// Retrieve information about incoming and outgoing connections to your node.
    /// </summary>
    public async Task<List<Connection>> GetConnectionsAsync(CancellationToken token = default)
    {
        ConnectionResponse result = await _moneroRpcCommunicator.GetConnectionsAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetConnectionsAsync));
        return result.Result!.Connections;
    }

    /// <summary>
    /// Retrieve general information about the state of your node and the network.
    /// </summary>
    public async Task<DaemonInformation> GetDaemonInformationAsync(CancellationToken token = default)
    {
        DaemonInformationResponse result = await _moneroRpcCommunicator.GetDaemonInformationAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetDaemonInformationAsync));
        return result.Result!;
    }

    /// <summary>
    /// Look up information regarding hard fork voting and readiness.
    /// </summary>
    public async Task<HardforkInformation> GetHardforkInformationAsync(CancellationToken token = default)
    {
        HardforkInformationResponse result = await _moneroRpcCommunicator.GetHardforkInformationAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetHardforkInformationAsync));
        return result.Result!;
    }

    /// <summary>
    /// Get list of banned IPs.
    /// </summary>
    public async Task<List<Ban>> GetBanInformationAsync(CancellationToken token = default)
    {
        GetBansResponse result = await _moneroRpcCommunicator.GetBansAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBanInformationAsync));
        return result.Result!.Bans;
    }

    /// <summary>
    /// Flush tx ids from transaction pool
    /// </summary>
    /// <param name="txids">List of transactions IDs to flush from pool (all tx ids flushed if empty).</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A string representing transaction pool status.</returns>
    public async Task<string> FlushTransactionPoolAsync(IEnumerable<string> txids, CancellationToken token = default)
    {
        FlushTransactionPoolResponse result = await _moneroRpcCommunicator.FlushTransactionPoolAsync(txids, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.FlushTransactionPoolAsync));
        return result.Result!.Status;
    }

    /// <summary>
    /// Get a histogram of output amounts. For all amounts (possibly filtered by parameters), gives the number of outputs on the chain for that amount. RingCT outputs counts as 0 amount.
    /// </summary>
    public async Task<List<Distribution>> GetOutputHistogramAsync(IEnumerable<ulong> amounts, ulong fromHeight, ulong toHeight, bool cumulative = false, bool binary = true, bool compress = false, CancellationToken token = default)
    {
        OutputHistogramResponse result = await _moneroRpcCommunicator.GetOutputHistogramAsync(amounts, fromHeight, toHeight, cumulative, binary, compress, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetOutputHistogramAsync));
        return result.Result!.Distributions;
    }

    /// <summary>
    /// Get the coinbase amount and the fees amount for n last blocks starting at particular height.
    /// </summary>
    public async Task<CoinbaseTransactionSum> GetCoinbaseTransactionSumAsync(ulong height, uint count, CancellationToken token = default)
    {
        CoinbaseTransactionSumResponse result = await _moneroRpcCommunicator.GetCoinbaseTransactionSumAsync(height, count, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetCoinbaseTransactionSumAsync));
        return result.Result!;
    }

    /// <summary>
    /// Give the node current version.
    /// </summary>
    public async Task<uint> GetVersionAsync(CancellationToken token = default)
    {
        VersionResponse result = await _moneroRpcCommunicator.GetDaemonVersionAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetVersionAsync));
        return result.Result!.Version;
    }

    /// <summary>
    /// Gets the daemon version as a structured <see cref="MoneroVersion"/> with major, minor, and patch components.
    /// </summary>
    public async Task<MoneroVersion> GetStructuredVersionAsync(CancellationToken token = default)
    {
        uint encoded = await GetVersionAsync(token).ConfigureAwait(false);
        return new MoneroVersion(encoded);
    }

    /// <summary>
    /// Executes an arbitrary JSON-RPC daemon method and returns raw request and response payloads.
    /// </summary>
    /// <param name="method">The daemon JSON-RPC method name.</param>
    /// <param name="paramsJson">Optional JSON params payload. Must be an object or array when provided.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The raw JSON-RPC execution result.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="paramsJson"/> is malformed JSON or does not represent an object or array.</exception>
    public Task<RawJsonRpcExecutionResult> ExecuteRawJsonRpcAsync(string method, string? paramsJson = null, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.ExecuteRawJsonRpcAsync(method, paramsJson, token);
    }

    /// <summary>
    /// Gives an estimation on fees (piconero) per byte.
    /// </summary>
    public async Task<ulong> GetFeeEstimateAsync(uint graceBlocks, CancellationToken token = default)
    {
        FeeEstimateResponse result = await _moneroRpcCommunicator.GetFeeEstimateAsync(graceBlocks, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetFeeEstimateAsync));
        return result.Result!.Fee;
    }

    /// <summary>
    /// Gets all fee estimate parameters, including fees (piconero) per byte.
    /// </summary>
    public async Task<FeeEstimate> GetFeeEstimateParametersAsync(uint graceBlocks, CancellationToken token = default)
    {
        FeeEstimateResponse result = await _moneroRpcCommunicator.GetFeeEstimateAsync(graceBlocks, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetFeeEstimateAsync));
        return result.Result!;
    }

    /// <summary>
    /// Display alternative chains seen by the node.
    /// </summary>
    public async Task<List<Chain>> GetAlternateChainsAsync(CancellationToken token = default)
    {
        AlternateChainResponse result = await _moneroRpcCommunicator.GetAlternateChainsAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetAlternateChainsAsync));
        return result.Result!.Chains;
    }

    /// <summary>
    /// Relay a transaction.
    /// </summary>
    /// <returns>The transaction hash of the relayed transaction.</returns>
    public async Task<string> RelayTransactionAsync(string hex, CancellationToken token = default)
    {
        RelayTransactionResponse result = await _moneroRpcCommunicator.RelayTransactionAsync(hex, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.RelayTransactionAsync));
        return result.Result!.TxHash;
    }

    /// <summary>
    /// Get synchronization information.
    /// </summary>
    public async Task<SyncronizationInformation> SyncInformationAsync(CancellationToken token = default)
    {
        SyncronizeInformationResponse result = await _moneroRpcCommunicator.SyncInformationAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SyncInformationAsync));
        return result.Result!;
    }

    /// <summary>
    /// Block header information can be retrieved using either a block's hash or height.
    /// This method includes a block's height as an input parameter to retrieve basic information about the block.
    /// </summary>
    public async Task<Block> GetBlockAsync(uint height, CancellationToken token = default)
    {
        BlockResponse result = await _moneroRpcCommunicator.GetBlockAsync(height, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBlockAsync));
        return result.Result!;
    }

    /// <summary>
    /// Block header information can be retrieved using either a block's hash or height.
    /// This method includes a block's hash as an input parameter to retrieve basic information about the block.
    /// </summary>
    public async Task<Block> GetBlockAsync(string hash, CancellationToken token = default)
    {
        BlockResponse result = await _moneroRpcCommunicator.GetBlockAsync(hash, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBlockAsync));
        return result.Result!;
    }

    /// <summary>
    /// Ban a node by IP using typed <see cref="NodeBan"/> objects.
    /// </summary>
    /// <returns>A string representing the banned node's status.</returns>
    public async Task<string> SetBansAsync(IEnumerable<NodeBan> bans, CancellationToken token = default)
    {
        SetBansResponse result = await _moneroRpcCommunicator.SetBansAsync(bans, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SetBansAsync));
        return result.Result!.Status;
    }

    /// <summary>
    /// Submit a mined block to the network.
    /// </summary>
    /// <returns>Success of block submission.</returns>
    public async Task<bool> SubmitBlocksAsync(IEnumerable<string> blockBlobs, CancellationToken token = default)
    {
        SubmitBlockResponse result = await _moneroRpcCommunicator.SubmitBlocksAsync(blockBlobs, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SubmitBlocksAsync));
        return result.Result! != null;
    }

    /// <summary>
    /// Get a block template on which mining a new block.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="reserveSize"/> is greater than <c>255</c>.</exception>
    public async Task<BlockTemplate> GetBlockTemplateAsync(ulong reserveSize, string walletAddress, string? prevBlock = null, string? extraNonce = null, CancellationToken token = default)
    {
        if (reserveSize > 255ul)
        {
            throw new InvalidOperationException($"Maximum {nameof(reserveSize)} cannot be greater than 255.");
        }

        GetBlockTemplateResponse result = await _moneroRpcCommunicator.GetBlockTemplateAsync(reserveSize, walletAddress, prevBlock, extraNonce, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBlockTemplateAsync));
        return result.Result!;
    }

    /// <summary>
    /// Get the status of an address that may or may not be banned.
    /// </summary>
    /// <param name="address">IP address of the node whose ban status is to be checked (e.g. 95.216.217.238)</param>
    /// <param name="token">The cancellation token.</param>
    public async Task<BanStatus> GetBanStatusAsync(string address, CancellationToken token = default)
    {
        GetBanStatusResponse result = await _moneroRpcCommunicator.GetBanStatusAsync(address, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBanStatusAsync));
        return result.Result!;
    }

    /// <summary>
    /// Get the transaction pool's backlog.
    /// </summary>
    public async Task<TransactionPoolBacklog> GetTransactionPoolBacklogAsync(CancellationToken token = default)
    {
        TransactionPoolBacklogResponse result = await _moneroRpcCommunicator.GetTransactionPoolBacklogAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetTransactionPoolBacklogAsync));
        return result.Result!;
    }

    /// <summary>
    /// Retrieve the current transaction pool.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    public async Task<TransactionPool> GetTransactionPoolAsync(CancellationToken token = default)
    {
        TransactionPool result = await _moneroRpcCommunicator.GetTransactionPoolAsync(token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result == null, nameof(this.GetTransactionPoolAsync));
        return result!;
    }

    /// <summary>
    /// Retrieve transactions by hash.
    /// </summary>
    /// <param name="txHashes">Transaction hashes to fetch.</param>
    /// <param name="token">The cancellation token.</param>
    public async Task<List<Transaction>> GetTransactionsAsync(IEnumerable<string> txHashes, CancellationToken token = default)
    {
        TransactionSet result = await _moneroRpcCommunicator.GetTransactionsAsync(txHashes, token).ConfigureAwait(false);
        ErrorGuard.ThrowIfResultIsNull(result?.Transactions == null, nameof(this.GetTransactionsAsync));
        return result!.Transactions;
    }

    /// <summary>
    /// Get the block hash for a specific chain height.
    /// </summary>
    /// <param name="height">The block height to query.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The block hash at the specified height.</returns>
    public Task<string> GetBlockHashAsync(ulong height, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetBlockHashAsync(height, token);
    }

    /// <summary>
    /// Get hashes for all known alternate chain tips.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Alternate block hashes known to the daemon.</returns>
    public Task<List<string>> GetAlternateBlockHashesAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetAlternateBlockHashesAsync(token);
    }

    /// <summary>
    /// Get white and gray peer lists from the daemon.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Current peer list information.</returns>
    public Task<PeerList> GetPeerListAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetPeerListAsync(token);
    }

    /// <summary>
    /// Get transaction pool statistics from the daemon.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Transaction pool statistics.</returns>
    public Task<TransactionPoolStats> GetTransactionPoolStatsAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetTransactionPoolStatsAsync(token);
    }

    /// <summary>
    /// Query whether key images are unspent, spent in blockchain, or spent in mempool.
    /// </summary>
    /// <param name="keyImages">The key images to test.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Spent status for each key image.</returns>
    public Task<List<KeyImageSpentStatus>> GetKeyImageSpentStatusesAsync(IEnumerable<string> keyImages, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetKeyImageSpentStatusesAsync(keyImages, token);
    }

    /// <summary>
    /// Get daemon output information for specific output references.
    /// </summary>
    /// <param name="outputs">The outputs to fetch.</param>
    /// <param name="includeTransactionId">Whether transaction ids should be included in response entries.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Resolved output records.</returns>
    public Task<List<DaemonOutput>> GetOutputsAsync(IEnumerable<DaemonOutputRequest> outputs, bool includeTransactionId = true, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetOutputsAsync(outputs, includeTransactionId, token);
    }

    /// <summary>
    /// Get output distribution data for one or more output amounts.
    /// </summary>
    /// <param name="amounts">The output amounts to query.</param>
    /// <param name="cumulative">Whether to return cumulative counts.</param>
    /// <param name="fromHeight">Optional start height.</param>
    /// <param name="toHeight">Optional end height.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Distribution statistics for the requested amounts.</returns>
    public Task<OutputDistribution> GetOutputDistributionAsync(IEnumerable<ulong> amounts, bool cumulative = false, ulong? fromHeight = null, ulong? toHeight = null, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetOutputDistributionAsync(amounts, cumulative, fromHeight, toHeight, token);
    }

    /// <summary>
    /// Get current daemon bandwidth limits.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Bandwidth limit values.</returns>
    public Task<BandwidthLimit> GetBandwidthLimitAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetBandwidthLimitAsync(token);
    }

    /// <summary>
    /// Set daemon bandwidth limits.
    /// </summary>
    /// <param name="limitDown">Optional download limit in kB/s.</param>
    /// <param name="limitUp">Optional upload limit in kB/s.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The resulting bandwidth limits.</returns>
    public Task<BandwidthLimit> SetBandwidthLimitAsync(int? limitDown = null, int? limitUp = null, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.SetBandwidthLimitAsync(limitDown, limitUp, token);
    }

    /// <summary>
    /// Get the maximum number of outgoing peers.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The current outgoing peer limit.</returns>
    public Task<int> GetOutgoingPeerLimitAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetOutgoingPeerLimitAsync(token);
    }

    /// <summary>
    /// Get the maximum number of incoming peers.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The current incoming peer limit.</returns>
    public Task<int> GetIncomingPeerLimitAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetIncomingPeerLimitAsync(token);
    }

    /// <summary>
    /// Set the maximum number of outgoing peers.
    /// </summary>
    /// <param name="count">Outgoing peer limit.</param>
    /// <param name="token">The cancellation token.</param>
    public Task SetOutgoingPeerLimitAsync(int count, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.SetOutgoingPeerLimitAsync(count, token);
    }

    /// <summary>
    /// Set the maximum number of incoming peers.
    /// </summary>
    /// <param name="count">Incoming peer limit.</param>
    /// <param name="token">The cancellation token.</param>
    public Task SetIncomingPeerLimitAsync(int count, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.SetIncomingPeerLimitAsync(count, token);
    }

    /// <summary>
    /// Start daemon mining.
    /// </summary>
    /// <param name="minerAddress">The miner reward address.</param>
    /// <param name="threadCount">Number of mining threads.</param>
    /// <param name="backgroundMining">Whether background mining is enabled.</param>
    /// <param name="ignoreBattery">Whether battery state is ignored.</param>
    /// <param name="token">The cancellation token.</param>
    public Task StartMiningAsync(string minerAddress, ulong threadCount, bool backgroundMining = false, bool ignoreBattery = true, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.StartDaemonMiningAsync(minerAddress, threadCount, backgroundMining, ignoreBattery, token);
    }

    /// <summary>
    /// Stop daemon mining.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    public Task StopMiningAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.StopDaemonMiningAsync(token);
    }

    /// <summary>
    /// Get current daemon mining status.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The current mining status.</returns>
    public Task<MiningStatus> GetMiningStatusAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetMiningStatusAsync(token);
    }

    /// <summary>
    /// Submit a raw transaction blob to the daemon.
    /// </summary>
    /// <param name="txAsHex">The raw transaction hex.</param>
    /// <param name="doNotRelay">Whether to avoid relaying the transaction.</param>
    /// <param name="doSanityChecks">Whether sanity checks are performed.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Submission result details.</returns>
    /// <exception cref="BibXmr.Client.Network.Exceptions.MoneroRpcProtocolException">Thrown when the daemon returns a non-OK status.</exception>
    public Task<SubmitRawTransaction> SubmitRawTransactionAsync(string txAsHex, bool doNotRelay = false, bool doSanityChecks = true, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.SubmitRawTransactionAsync(txAsHex, doNotRelay, doSanityChecks, token);
    }

    /// <summary>
    /// Check whether a daemon update is available.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Update availability information.</returns>
    public Task<DaemonUpdateCheck> CheckForUpdateAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.CheckForUpdateAsync(token);
    }

    /// <summary>
    /// Download a daemon update package.
    /// </summary>
    /// <param name="path">Optional destination directory.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Downloaded update information.</returns>
    public Task<DaemonUpdateDownload> DownloadUpdateAsync(string? path = null, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.DownloadUpdateAsync(path, token);
    }

    /// <summary>
    /// Stop the daemon process.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    public Task StopDaemonAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.StopDaemonAsync(token);
    }

    /// <summary>
    /// Get daemon-level network statistics.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Network statistics.</returns>
    public Task<NetworkStats> GetNetworkStatsAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetNetworkStatsAsync(token);
    }

    /// <summary>
    /// Get known public Monero nodes.
    /// </summary>
    /// <param name="gray">Whether to include graylisted nodes.</param>
    /// <param name="white">Whether to include whitelisted nodes.</param>
    /// <param name="includeBlocked">Whether to include blocked nodes.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Public node information.</returns>
    public Task<PublicNodes> GetPublicNodesAsync(bool gray = false, bool white = true, bool includeBlocked = false, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetPublicNodesAsync(gray, white, includeBlocked, token);
    }

    /// <summary>
    /// Save blockchain state to disk.
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    public Task SaveBlockchainAsync(CancellationToken token = default)
    {
        return _moneroRpcCommunicator.SaveBlockchainAsync(token);
    }

    /// <summary>
    /// Enable or disable hash-rate logging.
    /// </summary>
    /// <param name="visible">Whether hash-rate logging is visible.</param>
    /// <param name="token">The cancellation token.</param>
    public Task SetLogHashRateAsync(bool visible, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.SetLogHashRateAsync(visible, token);
    }

    /// <summary>
    /// Set daemon log level.
    /// </summary>
    /// <param name="level">The numeric log level.</param>
    /// <param name="token">The cancellation token.</param>
    public Task SetLogLevelAsync(uint level, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.SetLogLevelAsync(level, token);
    }

    /// <summary>
    /// Set daemon log categories.
    /// </summary>
    /// <param name="categories">Category expression string.</param>
    /// <param name="token">The cancellation token.</param>
    public Task SetLogCategoriesAsync(string categories, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.SetLogCategoriesAsync(categories, token);
    }

    /// <summary>
    /// Flush daemon caches.
    /// </summary>
    /// <param name="badTxs">Whether to flush bad transaction cache.</param>
    /// <param name="badBlocks">Whether to flush bad block cache.</param>
    /// <param name="token">The cancellation token.</param>
    public Task FlushCacheAsync(bool badTxs = false, bool badBlocks = false, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.FlushCacheAsync(badTxs, badBlocks, token);
    }

    /// <summary>
    /// Call the raw binary <c>/getblocks.bin</c> endpoint.
    /// </summary>
    /// <param name="requestBody">Serialized binary request payload.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Raw binary response payload.</returns>
    public Task<BinaryRpcPayload> GetBlocksBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetBlocksBinaryAsync(requestBody, token);
    }

    /// <summary>
    /// Call the raw binary <c>/get_blocks_by_height.bin</c> endpoint.
    /// </summary>
    /// <param name="requestBody">Serialized binary request payload.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Raw binary response payload.</returns>
    public Task<BinaryRpcPayload> GetBlocksByHeightBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetBlocksByHeightBinaryAsync(requestBody, token);
    }

    /// <summary>
    /// Call the raw binary <c>/gethashes.bin</c> endpoint.
    /// </summary>
    /// <param name="requestBody">Serialized binary request payload.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Raw binary response payload.</returns>
    public Task<BinaryRpcPayload> GetHashesBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetHashesBinaryAsync(requestBody, token);
    }

    /// <summary>
    /// Call the raw binary <c>/get_o_indexes.bin</c> endpoint.
    /// </summary>
    /// <param name="requestBody">Serialized binary request payload.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Raw binary response payload.</returns>
    public Task<BinaryRpcPayload> GetOutputIndexesBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetOutputIndexesBinaryAsync(requestBody, token);
    }

    /// <summary>
    /// Call the raw binary <c>/get_outs.bin</c> endpoint.
    /// </summary>
    /// <param name="requestBody">Serialized binary request payload.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Raw binary response payload.</returns>
    public Task<BinaryRpcPayload> GetOutputsBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetOutputsBinaryAsync(requestBody, token);
    }

    /// <summary>
    /// Call the raw binary <c>/get_transaction_pool_hashes.bin</c> endpoint.
    /// </summary>
    /// <param name="requestBody">Serialized binary request payload.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Raw binary response payload.</returns>
    public Task<BinaryRpcPayload> GetTransactionPoolHashesBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetTransactionPoolHashesBinaryAsync(requestBody, token);
    }

    /// <summary>
    /// Call the raw binary <c>/get_output_distribution.bin</c> endpoint.
    /// </summary>
    /// <param name="requestBody">Serialized binary request payload.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>Raw binary response payload.</returns>
    public Task<BinaryRpcPayload> GetOutputDistributionBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
    {
        return _moneroRpcCommunicator.GetOutputDistributionBinaryAsync(requestBody, token);
    }

    /// <summary>
    /// Subscribe to daemon polling notifications.
    /// </summary>
    /// <param name="listener">Callback container for polling events.</param>
    /// <param name="options">Optional polling configuration.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>An async-disposable subscription handle.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="listener"/> is <see langword="null"/>.</exception>
    public Task<IAsyncDisposable> SubscribeAsync(MoneroDaemonListener listener, MoneroDaemonPollingOptions? options = null, CancellationToken token = default)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        MoneroDaemonPollingOptions pollingOptions = options ?? new MoneroDaemonPollingOptions();
        var subscription = new DaemonPollingSubscription(this, listener, pollingOptions);
        lock (_subscriptionLock)
        {
            _subscriptions.Add(subscription);
        }

        return Task.FromResult<IAsyncDisposable>(subscription);
    }

    /// <summary>
    /// Dispose pattern hook.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        lock (_disposalLock)
        {
            if (_disposed)
            {
                return;
            }
            else
            {
                _disposed = true;
            }
        }

        if (disposing)
        {
            // Free managed objects.
            List<IAsyncDisposable> subscriptionsToDispose;
            lock (_subscriptionLock)
            {
                subscriptionsToDispose = [.. _subscriptions];
                _subscriptions.Clear();
            }

            foreach (IAsyncDisposable subscription in subscriptionsToDispose)
            {
                try
                {
                    subscription.DisposeAsync().AsTask().GetAwaiter().GetResult();
                }
                catch
                {
                    // Best-effort: sync Dispose must not throw from async cleanup.
                }
            }

            _moneroRpcCommunicator.Dispose();
        }

        // Free unmanaged objects.
    }

    /// <summary>
    /// Async initialization hook called by all <see cref="CreateAsync(string, uint, CancellationToken)"/> factory overloads.
    /// Currently a no-op; add future startup work here without changing the public API.
    /// </summary>
    private Task<MoneroDaemonClient> InitializeAsync()
    {
        // Nothing to do yet.
        return Task.FromResult(this);
    }

    /// <summary>
    /// Removes a polling subscription from the tracked subscription list.
    /// </summary>
    /// <param name="subscription">The subscription to remove.</param>
    private void RemoveSubscription(IAsyncDisposable subscription)
    {
        lock (_subscriptionLock)
        {
            _subscriptions.Remove(subscription);
        }
    }

    /// <summary>
    /// Represents an active daemon polling subscription.
    /// </summary>
    private sealed class DaemonPollingSubscription : IAsyncDisposable
    {
        private readonly MoneroDaemonClient _owner;
        private readonly MoneroDaemonListener _listener;
        private readonly MoneroDaemonPollingOptions _options;

        /// <summary>
        /// Cancellation source used to signal the polling loop to stop.
        /// </summary>
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _runTask;
        private bool _disposed;
        private string? _lastHash;

        /// <summary>
        /// Initializes a new instance of the DaemonPollingSubscription class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="options">Additional options for the operation.</param>
        public DaemonPollingSubscription(MoneroDaemonClient owner, MoneroDaemonListener listener, MoneroDaemonPollingOptions options)
        {
            _owner = owner;
            _listener = listener;
            _options = options;
            _runTask = Task.Run(RunAsync);
        }

        /// <summary>
        /// Asynchronously releases resources used by this instance.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

#if NET8_0_OR_GREATER
            await _cts.CancelAsync().ConfigureAwait(false);
#else
            _cts.Cancel();
#endif
            try
            {
                await _runTask.ConfigureAwait(false);
            }
            finally
            {
                _cts.Dispose();
                _owner.RemoveSubscription(this);
            }
        }

        /// <summary>
        /// Main polling loop that periodically checks for new block headers and fires listener callbacks.
        /// </summary>
        private async Task RunAsync()
        {
            CancellationToken token = _cts.Token;
            try
            {
                BlockHeader header = await _owner.GetLastBlockHeaderAsync(token).ConfigureAwait(false);
                _lastHash = header.Hash;

                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(_options.PollInterval, token).ConfigureAwait(false);
                    await PollOnceAsync(token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                // Normal shutdown.
            }
            catch (Exception ex)
            {
                await FirePollingErrorAsync(ex, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Executes a single poll iteration, comparing the latest block header hash to detect changes.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        private async Task PollOnceAsync(CancellationToken token)
        {
            try
            {
                BlockHeader header = await _owner.GetLastBlockHeaderAsync(token).ConfigureAwait(false);
                if (_lastHash == header.Hash)
                {
                    return;
                }

                _lastHash = header.Hash;
                if (_listener.OnBlockHeaderAsync != null)
                {
                    await _listener.OnBlockHeaderAsync(header, token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                // Normal shutdown.
            }
            catch (Exception ex)
            {
                await FirePollingErrorAsync(ex, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Invokes the listener's error callback, swallowing any exceptions thrown by the callback itself.
        /// </summary>
        /// <param name="exception">The polling exception to report.</param>
        /// <param name="token">The cancellation token.</param>
        private async Task FirePollingErrorAsync(Exception exception, CancellationToken token)
        {
            if (_listener.OnPollingErrorAsync == null)
            {
                return;
            }

            try
            {
                await _listener.OnPollingErrorAsync(exception, token).ConfigureAwait(false);
            }
            catch
            {
                // Callback exceptions are intentionally ignored.
            }
        }
    }
}
