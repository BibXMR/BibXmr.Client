using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Daemon.Dto.Responses;
using BibXmr.Client.Network;
using BibXmr.Client.Wallet.Dto.Requests;
using BibXmr.Client.Wallet.Dto.Responses;

namespace BibXmr.Client.Network.Serialization
{
    /// <summary>
    /// Declares daemon RPC root DTO types for JSON source generation.
    /// </summary>
    internal sealed class DaemonJsonTypeRoots
    {
        /// <summary>
        /// Gets or sets the block count response.
        /// </summary>
        public BlockCountResponse? BlockCountResponse { get; set; }

        /// <summary>
        /// Gets or sets the block header response.
        /// </summary>
        public BlockHeaderResponse? BlockHeaderResponse { get; set; }

        /// <summary>
        /// Gets or sets the block header range response.
        /// </summary>
        public BlockHeaderRangeResponse? BlockHeaderRangeResponse { get; set; }

        /// <summary>
        /// Gets or sets the connection response.
        /// </summary>
        public ConnectionResponse? ConnectionResponse { get; set; }

        /// <summary>
        /// Gets or sets the daemon information response.
        /// </summary>
        public DaemonInformationResponse? DaemonInformationResponse { get; set; }

        /// <summary>
        /// Gets or sets the hardfork information response.
        /// </summary>
        public HardforkInformationResponse? HardforkInformationResponse { get; set; }

        /// <summary>
        /// Gets or sets the get bans response.
        /// </summary>
        public GetBansResponse? GetBansResponse { get; set; }

        /// <summary>
        /// Gets or sets the flush transaction pool response.
        /// </summary>
        public FlushTransactionPoolResponse? FlushTransactionPoolResponse { get; set; }

        /// <summary>
        /// Gets or sets the output histogram response.
        /// </summary>
        public OutputHistogramResponse? OutputHistogramResponse { get; set; }

        /// <summary>
        /// Gets or sets the coinbase transaction sum response.
        /// </summary>
        public CoinbaseTransactionSumResponse? CoinbaseTransactionSumResponse { get; set; }

        /// <summary>
        /// Gets or sets the version response.
        /// </summary>
        public VersionResponse? VersionResponse { get; set; }

        /// <summary>
        /// Gets or sets the fee estimate response.
        /// </summary>
        public FeeEstimateResponse? FeeEstimateResponse { get; set; }

        /// <summary>
        /// Gets or sets the alternate chain response.
        /// </summary>
        public AlternateChainResponse? AlternateChainResponse { get; set; }

        /// <summary>
        /// Gets or sets the relay transaction response.
        /// </summary>
        public RelayTransactionResponse? RelayTransactionResponse { get; set; }

        /// <summary>
        /// Gets or sets the syncronize information response.
        /// </summary>
        public SyncronizeInformationResponse? SyncronizeInformationResponse { get; set; }

        /// <summary>
        /// Gets or sets the block response.
        /// </summary>
        public BlockResponse? BlockResponse { get; set; }

        /// <summary>
        /// Gets or sets the set bans response.
        /// </summary>
        public SetBansResponse? SetBansResponse { get; set; }

        /// <summary>
        /// Gets or sets the submit block response.
        /// </summary>
        public SubmitBlockResponse? SubmitBlockResponse { get; set; }

        /// <summary>
        /// Gets or sets the get block template response.
        /// </summary>
        public GetBlockTemplateResponse? GetBlockTemplateResponse { get; set; }

        /// <summary>
        /// Gets or sets the get ban status response.
        /// </summary>
        public GetBanStatusResponse? GetBanStatusResponse { get; set; }

        /// <summary>
        /// Gets or sets the prune blockchain response.
        /// </summary>
        public PruneBlockchainResponse? PruneBlockchainResponse { get; set; }

        /// <summary>
        /// Gets or sets the transaction pool backlog response.
        /// </summary>
        public TransactionPoolBacklogResponse? TransactionPoolBacklogResponse { get; set; }

        /// <summary>
        /// Gets or sets the transaction set.
        /// </summary>
        public TransactionSet? TransactionSet { get; set; }

        /// <summary>
        /// Gets or sets the transaction pool.
        /// </summary>
        public TransactionPool? TransactionPool { get; set; }

        /// <summary>
        /// Gets or sets the status only response.
        /// </summary>
        public StatusOnlyResponse? StatusOnlyResponse { get; set; }

        /// <summary>
        /// Gets or sets the block hash response.
        /// </summary>
        public BlockHashResponse? BlockHashResponse { get; set; }

        /// <summary>
        /// Gets or sets the alternate block hashes response.
        /// </summary>
        public AlternateBlockHashesResponse? AlternateBlockHashesResponse { get; set; }

        /// <summary>
        /// Gets or sets the peer list response.
        /// </summary>
        public PeerListResponse? PeerListResponse { get; set; }

        /// <summary>
        /// Gets or sets the transaction pool stats response.
        /// </summary>
        public TransactionPoolStatsResponse? TransactionPoolStatsResponse { get; set; }

        /// <summary>
        /// Gets or sets the key image spent statuses response.
        /// </summary>
        public KeyImageSpentStatusesResponse? KeyImageSpentStatusesResponse { get; set; }

        /// <summary>
        /// Gets or sets the outputs response.
        /// </summary>
        public OutputsResponse? OutputsResponse { get; set; }

        /// <summary>
        /// Gets or sets the output distribution response.
        /// </summary>
        public OutputDistributionResponse? OutputDistributionResponse { get; set; }

        /// <summary>
        /// Gets or sets the flush cache response.
        /// </summary>
        public FlushCacheResponse? FlushCacheResponse { get; set; }

        /// <summary>
        /// Gets or sets the bandwidth limit response.
        /// </summary>
        public BandwidthLimitResponse? BandwidthLimitResponse { get; set; }

        /// <summary>
        /// Gets or sets the peer limit response.
        /// </summary>
        public PeerLimitResponse? PeerLimitResponse { get; set; }

        /// <summary>
        /// Gets or sets the mining status response.
        /// </summary>
        public MiningStatusResponse? MiningStatusResponse { get; set; }

        /// <summary>
        /// Gets or sets the submit raw transaction response.
        /// </summary>
        public SubmitRawTransactionResponse? SubmitRawTransactionResponse { get; set; }

        /// <summary>
        /// Gets or sets the daemon update response.
        /// </summary>
        public DaemonUpdateResponse? DaemonUpdateResponse { get; set; }

        /// <summary>
        /// Gets or sets the network stats response.
        /// </summary>
        public NetworkStatsResponse? NetworkStatsResponse { get; set; }

        /// <summary>
        /// Gets or sets the public nodes response.
        /// </summary>
        public PublicNodesResponse? PublicNodesResponse { get; set; }
    }
}
