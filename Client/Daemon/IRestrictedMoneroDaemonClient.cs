using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Daemon.Dto.Responses;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Daemon
{
    /// <summary>
    /// Represents the daemon RPC surface that is valid for restricted RPC endpoints.
    /// </summary>
    public interface IRestrictedMoneroDaemonClient : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Gets the current canonical block count.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The blockchain height reported by the daemon.</returns>
        Task<ulong> GetBlockCountAsync(CancellationToken token = default);

        /// <summary>
        /// Gets the most recent block header.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The latest block header.</returns>
        Task<BlockHeader> GetLastBlockHeaderAsync(CancellationToken token = default);

        /// <summary>
        /// Gets a block header by its hash.
        /// </summary>
        /// <param name="hash">The block hash to query.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The requested block header.</returns>
        Task<BlockHeader> GetBlockHeaderByHashAsync(string hash, CancellationToken token = default);

        /// <summary>
        /// Gets a block header by its height.
        /// </summary>
        /// <param name="height">The block height to query.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The requested block header.</returns>
        Task<BlockHeader> GetBlockHeaderByHeightAsync(ulong height, CancellationToken token = default);

        /// <summary>
        /// Gets block headers in the specified inclusive height range.
        /// </summary>
        /// <param name="startHeight">The start height (inclusive).</param>
        /// <param name="endHeight">The end height (inclusive).</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Headers in the requested range.</returns>
        Task<List<BlockHeader>> GetBlockHeaderRangeAsync(uint startHeight, uint endHeight, CancellationToken token = default);

        /// <summary>
        /// Gets general daemon status and network information.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Daemon information.</returns>
        Task<DaemonInformation> GetDaemonInformationAsync(CancellationToken token = default);

        /// <summary>
        /// Gets hard fork voting and readiness information.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Hard fork information.</returns>
        Task<HardforkInformation> GetHardforkInformationAsync(CancellationToken token = default);

        /// <summary>
        /// Gets an output amount histogram.
        /// </summary>
        /// <param name="amounts">Amounts to query.</param>
        /// <param name="fromHeight">Lower height bound.</param>
        /// <param name="toHeight">Upper height bound.</param>
        /// <param name="cumulative">Whether cumulative values are requested.</param>
        /// <param name="binary">Whether binary histogram encoding is requested.</param>
        /// <param name="compress">Whether compressed binary output is requested.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Histogram distributions for requested amounts.</returns>
        Task<List<Distribution>> GetOutputHistogramAsync(IEnumerable<ulong> amounts, ulong fromHeight, ulong toHeight, bool cumulative = false, bool binary = true, bool compress = false, CancellationToken token = default);

        /// <summary>
        /// Gets daemon version information.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The encoded daemon version.</returns>
        Task<uint> GetVersionAsync(CancellationToken token = default);

        /// <summary>
        /// Gets daemon version information as a structured <see cref="MoneroVersion"/>.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A decoded version containing major, minor, and patch components.</returns>
        Task<MoneroVersion> GetStructuredVersionAsync(CancellationToken token = default);

        /// <summary>
        /// Executes an arbitrary JSON-RPC daemon method and returns raw request and response payloads.
        /// </summary>
        /// <param name="method">The daemon JSON-RPC method name.</param>
        /// <param name="paramsJson">Optional JSON params payload. Must be an object or array when provided.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The raw JSON-RPC execution result.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="paramsJson"/> is malformed JSON or does not represent an object or array.</exception>
        Task<RawJsonRpcExecutionResult> ExecuteRawJsonRpcAsync(string method, string? paramsJson = null, CancellationToken token = default);

        /// <summary>
        /// Gets the current fee estimate (piconero per byte).
        /// </summary>
        /// <param name="graceBlocks">Grace blocks parameter forwarded to RPC.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The estimated fee.</returns>
        Task<ulong> GetFeeEstimateAsync(uint graceBlocks, CancellationToken token = default);

        /// <summary>
        /// Gets detailed fee estimate parameters.
        /// </summary>
        /// <param name="graceBlocks">Grace blocks parameter forwarded to RPC.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Detailed fee estimate data.</returns>
        Task<FeeEstimate> GetFeeEstimateParametersAsync(uint graceBlocks, CancellationToken token = default);

        /// <summary>
        /// Gets full block information by height.
        /// </summary>
        /// <param name="height">The block height.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The requested block.</returns>
        Task<Block> GetBlockAsync(uint height, CancellationToken token = default);

        /// <summary>
        /// Gets full block information by hash.
        /// </summary>
        /// <param name="hash">The block hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The requested block.</returns>
        Task<Block> GetBlockAsync(string hash, CancellationToken token = default);

        /// <summary>
        /// Submits one or more block blobs for validation and relay.
        /// </summary>
        /// <param name="blockBlobs">Block blobs encoded as hex strings.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see langword="true"/> if submission succeeded.</returns>
        Task<bool> SubmitBlocksAsync(IEnumerable<string> blockBlobs, CancellationToken token = default);

        /// <summary>
        /// Requests a block template for mining.
        /// </summary>
        /// <param name="reserveSize">Reserved bytes for miner extra data.</param>
        /// <param name="walletAddress">The miner reward address.</param>
        /// <param name="prevBlock">Optional previous block hash override.</param>
        /// <param name="extraNonce">Optional extra nonce value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Template data for block construction.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="reserveSize"/> is greater than <c>255</c>.</exception>
        Task<BlockTemplate> GetBlockTemplateAsync(ulong reserveSize, string walletAddress, string? prevBlock = null, string? extraNonce = null, CancellationToken token = default);

        /// <summary>
        /// Gets transaction pool backlog information.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Backlog information.</returns>
        Task<TransactionPoolBacklog> GetTransactionPoolBacklogAsync(CancellationToken token = default);

        /// <summary>
        /// Gets full transaction pool contents.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Transaction pool data.</returns>
        Task<TransactionPool> GetTransactionPoolAsync(CancellationToken token = default);

        /// <summary>
        /// Gets transactions for the provided hashes.
        /// </summary>
        /// <param name="txHashes">Transaction hashes to query.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Transaction details.</returns>
        Task<List<Transaction>> GetTransactionsAsync(IEnumerable<string> txHashes, CancellationToken token = default);

        /// <summary>
        /// Gets a block hash by height.
        /// </summary>
        /// <param name="height">The block height.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The block hash.</returns>
        Task<string> GetBlockHashAsync(ulong height, CancellationToken token = default);

        /// <summary>
        /// Gets alternate chain block hashes.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Alternate block hashes.</returns>
        Task<List<string>> GetAlternateBlockHashesAsync(CancellationToken token = default);

        /// <summary>
        /// Gets transaction pool statistics.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Transaction pool statistics.</returns>
        Task<TransactionPoolStats> GetTransactionPoolStatsAsync(CancellationToken token = default);

        /// <summary>
        /// Checks whether key images are spent.
        /// </summary>
        /// <param name="keyImages">Key images to query.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Spend status values in request order.</returns>
        Task<List<KeyImageSpentStatus>> GetKeyImageSpentStatusesAsync(IEnumerable<string> keyImages, CancellationToken token = default);

        /// <summary>
        /// Gets output details for the requested output references.
        /// </summary>
        /// <param name="outputs">Output references.</param>
        /// <param name="includeTransactionId">Whether transaction ids are requested in response items.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Output details.</returns>
        Task<List<DaemonOutput>> GetOutputsAsync(IEnumerable<DaemonOutputRequest> outputs, bool includeTransactionId = true, CancellationToken token = default);

        /// <summary>
        /// Gets output distribution data for the requested amounts.
        /// </summary>
        /// <param name="amounts">Output amounts to query.</param>
        /// <param name="cumulative">Whether cumulative counts are requested.</param>
        /// <param name="fromHeight">Optional lower height bound.</param>
        /// <param name="toHeight">Optional upper height bound.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Output distribution data.</returns>
        Task<OutputDistribution> GetOutputDistributionAsync(IEnumerable<ulong> amounts, bool cumulative = false, ulong? fromHeight = null, ulong? toHeight = null, CancellationToken token = default);

        /// <summary>
        /// Gets configured bandwidth limits.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Current bandwidth limits.</returns>
        Task<BandwidthLimit> GetBandwidthLimitAsync(CancellationToken token = default);

        /// <summary>
        /// Submits a raw transaction blob.
        /// </summary>
        /// <param name="txAsHex">Transaction blob in hex format.</param>
        /// <param name="doNotRelay">Whether relay should be skipped.</param>
        /// <param name="doSanityChecks">Whether daemon sanity checks should run.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Submission status details.</returns>
        /// <exception cref="BibXmr.Client.Network.Exceptions.MoneroRpcProtocolException">Thrown when the daemon returns a non-OK status.</exception>
        Task<SubmitRawTransaction> SubmitRawTransactionAsync(string txAsHex, bool doNotRelay = false, bool doSanityChecks = true, CancellationToken token = default);

        /// <summary>
        /// Gets known public nodes.
        /// </summary>
        /// <param name="gray">Whether graylisted nodes are included.</param>
        /// <param name="white">Whether whitelisted nodes are included.</param>
        /// <param name="includeBlocked">Whether blocked nodes are included.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Public node lists.</returns>
        Task<PublicNodes> GetPublicNodesAsync(bool gray = false, bool white = true, bool includeBlocked = false, CancellationToken token = default);

        /// <summary>
        /// Calls the raw binary <c>/get_blocks.bin</c> endpoint.
        /// </summary>
        /// <param name="requestBody">Serialized binary request payload.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Raw binary response payload.</returns>
        Task<BinaryRpcPayload> GetBlocksBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default);

        /// <summary>
        /// Calls the raw binary <c>/get_blocks_by_height.bin</c> endpoint.
        /// </summary>
        /// <param name="requestBody">Serialized binary request payload.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Raw binary response payload.</returns>
        Task<BinaryRpcPayload> GetBlocksByHeightBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default);

        /// <summary>
        /// Calls the raw binary <c>/get_hashes.bin</c> endpoint.
        /// </summary>
        /// <param name="requestBody">Serialized binary request payload.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Raw binary response payload.</returns>
        Task<BinaryRpcPayload> GetHashesBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default);

        /// <summary>
        /// Calls the raw binary <c>/get_o_indexes.bin</c> endpoint.
        /// </summary>
        /// <param name="requestBody">Serialized binary request payload.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Raw binary response payload.</returns>
        Task<BinaryRpcPayload> GetOutputIndexesBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default);

        /// <summary>
        /// Calls the raw binary <c>/get_outs.bin</c> endpoint.
        /// </summary>
        /// <param name="requestBody">Serialized binary request payload.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Raw binary response payload.</returns>
        Task<BinaryRpcPayload> GetOutputsBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default);

        /// <summary>
        /// Calls the raw binary <c>/get_transaction_pool_hashes.bin</c> endpoint.
        /// </summary>
        /// <param name="requestBody">Serialized binary request payload.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Raw binary response payload.</returns>
        Task<BinaryRpcPayload> GetTransactionPoolHashesBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default);

        /// <summary>
        /// Calls the raw binary <c>/get_output_distribution.bin</c> endpoint.
        /// </summary>
        /// <param name="requestBody">Serialized binary request payload.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Raw binary response payload.</returns>
        Task<BinaryRpcPayload> GetOutputDistributionBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default);

        /// <summary>
        /// Subscribes to polling callbacks for daemon tip changes.
        /// </summary>
        /// <param name="listener">Callback container for polling events.</param>
        /// <param name="options">Optional polling settings.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An async-disposable subscription handle.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="listener"/> is <see langword="null"/>.</exception>
        Task<IAsyncDisposable> SubscribeAsync(MoneroDaemonListener listener, MoneroDaemonPollingOptions? options = null, CancellationToken token = default);
    }
}
