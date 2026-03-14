using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Daemon.Dto.Responses;
using BibXmr.Client.Network;
using BibXmr.Client.Wallet.Dto.Requests;
using BibXmr.Client.Wallet.Dto.Responses;

namespace BibXmr.Client.Network.Serialization
{
    /// <summary>
    /// Declares wallet RPC root DTO types for JSON source generation.
    /// </summary>
    internal sealed class WalletJsonTypeRoots
    {
        /// <summary>
        /// Gets or sets the balance response.
        /// </summary>
        public BalanceResponse? BalanceResponse { get; set; }

        /// <summary>
        /// Gets or sets the address response.
        /// </summary>
        public AddressResponse? AddressResponse { get; set; }

        /// <summary>
        /// Gets or sets the address index response.
        /// </summary>
        public AddressIndexResponse? AddressIndexResponse { get; set; }

        /// <summary>
        /// Gets or sets the address creation response.
        /// </summary>
        public AddressCreationResponse? AddressCreationResponse { get; set; }

        /// <summary>
        /// Gets or sets the address label response.
        /// </summary>
        public AddressLabelResponse? AddressLabelResponse { get; set; }

        /// <summary>
        /// Gets or sets the account response.
        /// </summary>
        public AccountResponse? AccountResponse { get; set; }

        /// <summary>
        /// Gets or sets the create account response.
        /// </summary>
        public CreateAccountResponse? CreateAccountResponse { get; set; }

        /// <summary>
        /// Gets or sets the account label response.
        /// </summary>
        public AccountLabelResponse? AccountLabelResponse { get; set; }

        /// <summary>
        /// Gets or sets the account tags response.
        /// </summary>
        public AccountTagsResponse? AccountTagsResponse { get; set; }

        /// <summary>
        /// Gets or sets the tag accounts response.
        /// </summary>
        public TagAccountsResponse? TagAccountsResponse { get; set; }

        /// <summary>
        /// Gets or sets the untag accounts response.
        /// </summary>
        public UntagAccountsResponse? UntagAccountsResponse { get; set; }

        /// <summary>
        /// Gets or sets the account tag and description response.
        /// </summary>
        public AccountTagAndDescriptionResponse? AccountTagAndDescriptionResponse { get; set; }

        /// <summary>
        /// Gets or sets the blockchain height response.
        /// </summary>
        public BlockchainHeightResponse? BlockchainHeightResponse { get; set; }

        /// <summary>
        /// Gets or sets the fund transfer response.
        /// </summary>
        public FundTransferResponse? FundTransferResponse { get; set; }

        /// <summary>
        /// Gets or sets the split fund transfer response.
        /// </summary>
        public SplitFundTransferResponse? SplitFundTransferResponse { get; set; }

        /// <summary>
        /// Gets or sets the sign transfer response.
        /// </summary>
        public SignTransferResponse? SignTransferResponse { get; set; }

        /// <summary>
        /// Gets or sets the submit transfer response.
        /// </summary>
        public SubmitTransferResponse? SubmitTransferResponse { get; set; }

        /// <summary>
        /// Gets or sets the sweep dust response.
        /// </summary>
        public SweepDustResponse? SweepDustResponse { get; set; }

        /// <summary>
        /// Gets or sets the sweep all response.
        /// </summary>
        public SweepAllResponse? SweepAllResponse { get; set; }

        /// <summary>
        /// Gets or sets the save wallet response.
        /// </summary>
        public SaveWalletResponse? SaveWalletResponse { get; set; }

        /// <summary>
        /// Gets or sets the stop wallet response.
        /// </summary>
        public StopWalletResponse? StopWalletResponse { get; set; }

        /// <summary>
        /// Gets or sets the incoming transfers response.
        /// </summary>
        public IncomingTransfersResponse? IncomingTransfersResponse { get; set; }

        /// <summary>
        /// Gets or sets the query key response.
        /// </summary>
        public QueryKeyResponse? QueryKeyResponse { get; set; }

        /// <summary>
        /// Gets or sets the set transaction notes response.
        /// </summary>
        public SetTransactionNotesResponse? SetTransactionNotesResponse { get; set; }

        /// <summary>
        /// Gets or sets the get transaction notes response.
        /// </summary>
        public GetTransactionNotesResponse? GetTransactionNotesResponse { get; set; }

        /// <summary>
        /// Gets or sets the get transaction key response.
        /// </summary>
        public GetTransactionKeyResponse? GetTransactionKeyResponse { get; set; }

        /// <summary>
        /// Gets or sets the check transaction key response.
        /// </summary>
        public CheckTransactionKeyResponse? CheckTransactionKeyResponse { get; set; }

        /// <summary>
        /// Gets or sets the show transfers response.
        /// </summary>
        public ShowTransfersResponse? ShowTransfersResponse { get; set; }

        /// <summary>
        /// Gets or sets the get transfer by txid response.
        /// </summary>
        public GetTransferByTxidResponse? GetTransferByTxidResponse { get; set; }

        /// <summary>
        /// Gets or sets the sign response.
        /// </summary>
        public SignResponse? SignResponse { get; set; }

        /// <summary>
        /// Gets or sets the verify response.
        /// </summary>
        public VerifyResponse? VerifyResponse { get; set; }

        /// <summary>
        /// Gets or sets the export outputs response.
        /// </summary>
        public ExportOutputsResponse? ExportOutputsResponse { get; set; }

        /// <summary>
        /// Gets or sets the import outputs response.
        /// </summary>
        public ImportOutputsResponse? ImportOutputsResponse { get; set; }

        /// <summary>
        /// Gets or sets the export key images response.
        /// </summary>
        public ExportKeyImagesResponse? ExportKeyImagesResponse { get; set; }

        /// <summary>
        /// Gets or sets the import key images response.
        /// </summary>
        public ImportKeyImagesResponse? ImportKeyImagesResponse { get; set; }

        /// <summary>
        /// Gets or sets the make uri response.
        /// </summary>
        public MakeUriResponse? MakeUriResponse { get; set; }

        /// <summary>
        /// Gets or sets the parse uri response.
        /// </summary>
        public ParseUriResponse? ParseUriResponse { get; set; }

        /// <summary>
        /// Gets or sets the get address book response.
        /// </summary>
        public GetAddressBookResponse? GetAddressBookResponse { get; set; }

        /// <summary>
        /// Gets or sets the add address book response.
        /// </summary>
        public AddAddressBookResponse? AddAddressBookResponse { get; set; }

        /// <summary>
        /// Gets or sets the delete address book response.
        /// </summary>
        public DeleteAddressBookResponse? DeleteAddressBookResponse { get; set; }

        /// <summary>
        /// Gets or sets the refresh wallet response.
        /// </summary>
        public RefreshWalletResponse? RefreshWalletResponse { get; set; }

        /// <summary>
        /// Gets or sets the rescan spent response.
        /// </summary>
        public RescanSpentResponse? RescanSpentResponse { get; set; }

        /// <summary>
        /// Gets or sets the languages response.
        /// </summary>
        public LanguagesResponse? LanguagesResponse { get; set; }

        /// <summary>
        /// Gets or sets the create wallet response.
        /// </summary>
        public CreateWalletResponse? CreateWalletResponse { get; set; }

        /// <summary>
        /// Gets or sets the open wallet response.
        /// </summary>
        public OpenWalletResponse? OpenWalletResponse { get; set; }

        /// <summary>
        /// Gets or sets the close wallet response.
        /// </summary>
        public CloseWalletResponse? CloseWalletResponse { get; set; }

        /// <summary>
        /// Gets or sets the change wallet password response.
        /// </summary>
        public ChangeWalletPasswordResponse? ChangeWalletPasswordResponse { get; set; }

        /// <summary>
        /// Gets or sets the get rpc version response.
        /// </summary>
        public GetRpcVersionResponse? GetRpcVersionResponse { get; set; }

        /// <summary>
        /// Gets or sets the multi sig information response.
        /// </summary>
        public MultiSigInformationResponse? MultiSigInformationResponse { get; set; }

        /// <summary>
        /// Gets or sets the prepare multi sig response.
        /// </summary>
        public PrepareMultiSigResponse? PrepareMultiSigResponse { get; set; }

        /// <summary>
        /// Gets or sets the make multi sig response.
        /// </summary>
        public MakeMultiSigResponse? MakeMultiSigResponse { get; set; }

        /// <summary>
        /// Gets or sets the export multi sig info response.
        /// </summary>
        public ExportMultiSigInfoResponse? ExportMultiSigInfoResponse { get; set; }

        /// <summary>
        /// Gets or sets the import multi sig info response.
        /// </summary>
        public ImportMultiSigInfoResponse? ImportMultiSigInfoResponse { get; set; }

        /// <summary>
        /// Gets or sets the finalize multi sig response.
        /// </summary>
        public FinalizeMultiSigResponse? FinalizeMultiSigResponse { get; set; }

        /// <summary>
        /// Gets or sets the sign multi sig transaction response.
        /// </summary>
        public SignMultiSigTransactionResponse? SignMultiSigTransactionResponse { get; set; }

        /// <summary>
        /// Gets or sets the submit multi sig transaction response.
        /// </summary>
        public SubmitMultiSigTransactionResponse? SubmitMultiSigTransactionResponse { get; set; }

        /// <summary>
        /// Gets or sets the describe transfer response.
        /// </summary>
        public DescribeTransferResponse? DescribeTransferResponse { get; set; }

        /// <summary>
        /// Gets or sets the sweep single response.
        /// </summary>
        public SweepSingleResponse? SweepSingleResponse { get; set; }

        /// <summary>
        /// Gets or sets the payment detail response.
        /// </summary>
        public PaymentDetailResponse? PaymentDetailResponse { get; set; }

        /// <summary>
        /// Gets or sets the set attribute response.
        /// </summary>
        public SetAttributeResponse? SetAttributeResponse { get; set; }

        /// <summary>
        /// Gets or sets the get attribute response.
        /// </summary>
        public GetAttributeResponse? GetAttributeResponse { get; set; }

        /// <summary>
        /// Gets or sets the validate address response.
        /// </summary>
        public ValidateAddressResponse? ValidateAddressResponse { get; set; }

        /// <summary>
        /// Gets or sets the integrated address response.
        /// </summary>
        public IntegratedAddressResponse? IntegratedAddressResponse { get; set; }

        /// <summary>
        /// Gets or sets the bulk payments response.
        /// </summary>
        public BulkPaymentsResponse? BulkPaymentsResponse { get; set; }

        /// <summary>
        /// Gets or sets the frozen output response.
        /// </summary>
        public FrozenOutputResponse? FrozenOutputResponse { get; set; }

        /// <summary>
        /// Gets or sets the default fee priority response.
        /// </summary>
        public DefaultFeePriorityResponse? DefaultFeePriorityResponse { get; set; }

        /// <summary>
        /// Gets or sets the transaction proof response.
        /// </summary>
        public TransactionProofResponse? TransactionProofResponse { get; set; }

        /// <summary>
        /// Gets or sets the check transaction proof response.
        /// </summary>
        public CheckTransactionProofResponse? CheckTransactionProofResponse { get; set; }

        /// <summary>
        /// Gets or sets the spend proof response.
        /// </summary>
        public SpendProofResponse? SpendProofResponse { get; set; }

        /// <summary>
        /// Gets or sets the check spend proof response.
        /// </summary>
        public CheckSpendProofResponse? CheckSpendProofResponse { get; set; }

        /// <summary>
        /// Gets or sets the reserve proof response.
        /// </summary>
        public ReserveProofResponse? ReserveProofResponse { get; set; }

        /// <summary>
        /// Gets or sets the check reserve proof response.
        /// </summary>
        public CheckReserveProofResponse? CheckReserveProofResponse { get; set; }

        /// <summary>
        /// Gets or sets the exchange multi sig keys response.
        /// </summary>
        public ExchangeMultiSigKeysResponse? ExchangeMultiSigKeysResponse { get; set; }

        /// <summary>
        /// Gets or sets the estimate tx size and weight response.
        /// </summary>
        public EstimateTxSizeAndWeightResponse? EstimateTxSizeAndWeightResponse { get; set; }

        /// <summary>
        /// Gets or sets the generate from keys wallet response.
        /// </summary>
        public GenerateFromKeysWalletResponse? GenerateFromKeysWalletResponse { get; set; }

        /// <summary>
        /// Gets or sets the restore deterministic wallet response.
        /// </summary>
        public RestoreDeterministicWalletResponse? RestoreDeterministicWalletResponse { get; set; }

        /// <summary>
        /// Gets or sets the relay transaction response.
        /// </summary>
        public RelayTransactionResponse? RelayTransactionResponse { get; set; }
    }
}
