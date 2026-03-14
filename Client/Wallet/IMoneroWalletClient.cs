using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Utilities;
using BibXmr.Client.Wallet.Dto;
using BibXmr.Client.Wallet.Dto.Responses;

namespace BibXmr.Client.Wallet
{
    /// <summary>
    /// High-level client for interacting with <c>monero-wallet-rpc</c> via its JSON-RPC interface.
    /// </summary>
    public interface IMoneroWalletClient : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Returns the wallet's balance for the given account, address indices, and filtering options.
        /// </summary>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="addressIndices">The subaddress indices to query.</param>
        /// <param name="allAccounts">The <paramref name="allAccounts"/> value.</param>
        /// <param name="strict">The <paramref name="strict"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<Balance> GetBalanceAsync(uint accountIndex, IEnumerable<uint> addressIndices, bool allAccounts = false, bool strict = false, CancellationToken token = default);

        /// <summary>
        /// Returns the wallet's balance for a given account index.
        /// </summary>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<Balance> GetBalanceAsync(uint accountIndex, CancellationToken token = default);

        /// <summary>
        /// Return the wallet's addresses for an account.
        /// </summary>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<Addresses> GetAddressAsync(uint accountIndex, CancellationToken token = default);

        /// <summary>
        /// Return the wallet's addresses for an account, filtered by specific subaddress indices.
        /// </summary>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="addressIndices">The subaddress indices to query.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<Addresses> GetAddressAsync(uint accountIndex, IEnumerable<uint> addressIndices, CancellationToken token = default);

        /// <summary>
        /// Get account and address indexes from a specific (sub)address.
        /// </summary>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<AddressIndex> GetAddressIndexAsync(string address, CancellationToken token = default);

        /// <summary>
        /// Create a new address for an account.
        /// </summary>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<AddressCreation> CreateAddressAsync(uint accountIndex, CancellationToken token = default);

        /// <summary>
        /// Create a new address for an account with a label.
        /// </summary>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="label">The <paramref name="label"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<AddressCreation> CreateAddressAsync(uint accountIndex, string label, CancellationToken token = default);

        /// <summary>
        /// Label an address.
        /// </summary>
        /// <param name="majorIndex">The <paramref name="majorIndex"/> value.</param>
        /// <param name="minorIndex">The <paramref name="minorIndex"/> value.</param>
        /// <param name="label">The <paramref name="label"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<AddressLabel> LabelAddressAsync(uint majorIndex, uint minorIndex, string label, CancellationToken token = default);

        /// <summary>
        /// Get all accounts for a wallet.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<Account> GetAccountsAsync(CancellationToken token = default);

        /// <summary>
        /// Get all accounts for a wallet, filtered by tag.
        /// </summary>
        /// <param name="tag">The <paramref name="tag"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<Account> GetAccountsAsync(string tag, CancellationToken token = default);

        /// <summary>
        /// Create a new account.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<CreateAccount> CreateAccountAsync(CancellationToken token = default);

        /// <summary>
        /// Create a new account with an optional label.
        /// </summary>
        /// <param name="label">The <paramref name="label"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<CreateAccount> CreateAccountAsync(string label, CancellationToken token = default);

        /// <summary>
        /// Label an account.
        /// </summary>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="label">The <paramref name="label"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<AccountLabel> LabelAccountAsync(uint accountIndex, string label, CancellationToken token = default);

        /// <summary>
        /// Get a list of user-defined account tags.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<AccountTags> GetAccountTagsAsync(CancellationToken token = default);

        /// <summary>
        /// Apply a filtering tag to a list of accounts.
        /// </summary>
        /// <param name="tag">The <paramref name="tag"/> value.</param>
        /// <param name="accounts">The <paramref name="accounts"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<TagAccounts> TagAccountsAsync(string tag, IEnumerable<uint> accounts, CancellationToken token = default);

        /// <summary>
        /// Remove filtering tag from a list of accounts.
        /// </summary>
        /// <param name="accounts">The <paramref name="accounts"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<UntagAccounts> UntagAccountsAsync(IEnumerable<uint> accounts, CancellationToken token = default);

        /// <summary>
        /// Set description for an account tag.
        /// </summary>
        /// <param name="tag">The <paramref name="tag"/> value.</param>
        /// <param name="description">The <paramref name="description"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<AccountTagAndDescription> SetAccountTagDescriptionAsync(string tag, string description, CancellationToken token = default);

        /// <summary>
        /// Returns the wallet's current block height.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<ulong> GetHeightAsync(CancellationToken token = default);

        /// <summary>
        /// Sends funds to one or more destinations.
        /// </summary>
        /// <param name="transactions">Destination addresses and amounts to transfer.</param>
        /// <param name="transferPriority">The transfer priority to request.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to transfer result details.</returns>
        Task<FundTransfer> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, CancellationToken token = default);

        /// <summary>
        /// Sends funds and optionally returns transaction key and transaction hex data.
        /// </summary>
        /// <param name="transactions">Destination addresses and amounts to transfer.</param>
        /// <param name="transferPriority">The transfer priority to request.</param>
        /// <param name="getTxKey">Whether transaction key data is included in the result.</param>
        /// <param name="getTxHex">Whether transaction hex data is included in the result.</param>
        /// <param name="unlockTime">The unlock time value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to transfer result details.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        Task<FundTransfer> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, bool getTxKey, bool getTxHex, ulong unlockTime = 0, CancellationToken token = default);

        /// <summary>
        /// Sends funds with an explicit ring size and optional transaction key and hex output.
        /// </summary>
        /// <param name="transactions">Destination addresses and amounts to transfer.</param>
        /// <param name="transferPriority">The transfer priority to request.</param>
        /// <param name="ringSize">The ring size to request.</param>
        /// <param name="unlockTime">The unlock time value.</param>
        /// <param name="getTxKey">Whether transaction key data is included in the result.</param>
        /// <param name="getTxHex">Whether transaction hex data is included in the result.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to transfer result details.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        Task<FundTransfer> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, uint ringSize, ulong unlockTime = 0, bool getTxKey = true, bool getTxHex = true, CancellationToken token = default);

        /// <summary>
        /// Sends funds from a specific account with explicit ring size and optional transaction key and hex output.
        /// </summary>
        /// <param name="transactions">Destination addresses and amounts to transfer.</param>
        /// <param name="transferPriority">The transfer priority to request.</param>
        /// <param name="ringSize">The ring size to request.</param>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="unlockTime">The unlock time value.</param>
        /// <param name="getTxKey">Whether transaction key data is included in the result.</param>
        /// <param name="getTxHex">Whether transaction hex data is included in the result.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to transfer result details.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        Task<FundTransfer> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, uint ringSize, uint accountIndex, ulong unlockTime = 0, bool getTxKey = true, bool getTxHex = true, CancellationToken token = default);

        /// <summary>
        /// Same as transfer, but can split into more than one tx if necessary.
        /// </summary>
        /// <param name="transactions">Destination addresses and amounts to transfer.</param>
        /// <param name="transferPriority">The transfer priority to request.</param>
        /// <param name="newAlgorithm">The <paramref name="newAlgorithm"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<SplitFundTransfer> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, bool newAlgorithm = true, CancellationToken token = default);

        /// <summary>
        /// Sends a split transfer and optionally returns transaction key and transaction hex data.
        /// </summary>
        /// <param name="transactions">Destination addresses and amounts to transfer.</param>
        /// <param name="transferPriority">The transfer priority to request.</param>
        /// <param name="getTxKey">The <paramref name="getTxKey"/> value.</param>
        /// <param name="getTxHex">The <paramref name="getTxHex"/> value.</param>
        /// <param name="newAlgorithm">The <paramref name="newAlgorithm"/> value.</param>
        /// <param name="unlockTime">The unlock time value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to split transfer result details.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        Task<SplitFundTransfer> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, bool getTxKey, bool getTxHex, bool newAlgorithm = true, ulong unlockTime = 0, CancellationToken token = default);

        /// <summary>
        /// Sends a split transfer with explicit ring size and optional transaction key and hex output.
        /// </summary>
        /// <param name="transactions">Destination addresses and amounts to transfer.</param>
        /// <param name="transferPriority">The transfer priority to request.</param>
        /// <param name="ringSize">The ring size to request.</param>
        /// <param name="newAlgorithm">The <paramref name="newAlgorithm"/> value.</param>
        /// <param name="unlockTime">The unlock time value.</param>
        /// <param name="getTxKey">The <paramref name="getTxKey"/> value.</param>
        /// <param name="getTxHex">The <paramref name="getTxHex"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to split transfer result details.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        Task<SplitFundTransfer> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, uint ringSize, bool newAlgorithm = true, ulong unlockTime = 0, bool getTxKey = true, bool getTxHex = true, CancellationToken token = default);

        /// <summary>
        /// Sends a split transfer from a specific account with explicit ring size and optional transaction key and hex output.
        /// </summary>
        /// <param name="transactions">Destination addresses and amounts to transfer.</param>
        /// <param name="transferPriority">The transfer priority to request.</param>
        /// <param name="ringSize">The ring size to request.</param>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="newAlgorithm">The <paramref name="newAlgorithm"/> value.</param>
        /// <param name="unlockTime">The unlock time value.</param>
        /// <param name="getTxKey">The <paramref name="getTxKey"/> value.</param>
        /// <param name="getTxHex">The <paramref name="getTxHex"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to split transfer result details.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        Task<SplitFundTransfer> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, uint ringSize, uint accountIndex, bool newAlgorithm = true, ulong unlockTime = 0, bool getTxKey = true, bool getTxHex = true, CancellationToken token = default);

        /// <summary>
        /// Sign a transaction created on a read-only wallet (in cold-signing process).
        /// </summary>
        /// <param name="unsignedTxSet">The <paramref name="unsignedTxSet"/> value.</param>
        /// <param name="exportRaw">The <paramref name="exportRaw"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<SignTransfer> SignTransferAsync(string unsignedTxSet, bool exportRaw = false, CancellationToken token = default);

        /// <summary>
        /// Submit a previously signed transaction on a read-only wallet (in cold-signing process).
        /// </summary>
        /// <param name="txDataHex">The <paramref name="txDataHex"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<string>> SubmitTransferAsync(string txDataHex, CancellationToken token = default);

        /// <summary>
        /// Parse an unsigned transfer to retrieve its transfer descriptions.
        /// </summary>
        /// <param name="unsignedTxSet">The <paramref name="unsignedTxSet"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<TransferDescription>> DescribeUnsignedTransferAsync(string unsignedTxSet, CancellationToken token = default);

        /// <summary>
        /// Parse a multisig transfer to retrieve its transfer descriptions.
        /// </summary>
        /// <param name="multiSigTxSet">The <paramref name="multiSigTxSet"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<TransferDescription>> DescribeMultiSigTransferAsync(string multiSigTxSet, CancellationToken token = default);

        /// <summary>
        /// Send all dust outputs back to the wallet to make them easier to spend and mix.
        /// </summary>
        /// <param name="getTxKey">The <paramref name="getTxKey"/> value.</param>
        /// <param name="getTxHex">The <paramref name="getTxHex"/> value.</param>
        /// <param name="getTxMetadata">The <paramref name="getTxMetadata"/> value.</param>
        /// <param name="doNotRelay">The <paramref name="doNotRelay"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<SweepDust> SweepDustAsync(bool getTxKey, bool getTxHex, bool getTxMetadata, bool doNotRelay = false, CancellationToken token = default);

        /// <summary>
        /// Send all unlocked balance to an address.
        /// </summary>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="transactionPriority">The transfer priority to request.</param>
        /// <param name="ringSize">The ring size to request.</param>
        /// <param name="unlockTime">The unlock time value.</param>
        /// <param name="belowAmount">The <paramref name="belowAmount"/> value.</param>
        /// <param name="getTxKeys">The <paramref name="getTxKeys"/> value.</param>
        /// <param name="getTxHex">The <paramref name="getTxHex"/> value.</param>
        /// <param name="getTxMetadata">The <paramref name="getTxMetadata"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<SplitFundTransfer> SweepAllAsync(string address, uint accountIndex, TransferPriority transactionPriority, uint ringSize, ulong unlockTime = 0, ulong belowAmount = ulong.MaxValue, bool getTxKeys = true, bool getTxHex = true, bool getTxMetadata = true, CancellationToken token = default);

        /// <summary>
        /// Send a single swept output to an address.
        /// </summary>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="transactionPriority">The transfer priority to request.</param>
        /// <param name="ringSize">The ring size to request.</param>
        /// <param name="unlockTime">The unlock time value.</param>
        /// <param name="getTxKey">The <paramref name="getTxKey"/> value.</param>
        /// <param name="getTxHex">The <paramref name="getTxHex"/> value.</param>
        /// <param name="getTxMetadata">The <paramref name="getTxMetadata"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<SweepSingle> SweepSingleAsync(string address, uint accountIndex, TransferPriority transactionPriority, uint ringSize, ulong unlockTime = 0, bool getTxKey = true, bool getTxHex = true, bool getTxMetadata = true, CancellationToken token = default);

        /// <summary>
        /// Save the wallet.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<SaveWallet> SaveWalletAsync(CancellationToken token = default);

        /// <summary>
        /// Return a list of incoming transfers to the wallet.
        /// </summary>
        /// <param name="transferType">The <paramref name="transferType"/> value.</param>
        /// <param name="returnKeyImage">The <paramref name="returnKeyImage"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<IncomingTransfer>> GetIncomingTransfersAsync(TransferType transferType, bool returnKeyImage = false, CancellationToken token = default);

        /// <summary>
        /// Return a list of incoming transfers to the wallet for a specific account.
        /// </summary>
        /// <param name="transferType">The <paramref name="transferType"/> value.</param>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="returnKeyImage">The <paramref name="returnKeyImage"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<IncomingTransfer>> GetIncomingTransfersAsync(TransferType transferType, uint accountIndex, bool returnKeyImage = false, CancellationToken token = default);

        /// <summary>
        /// Return a list of incoming transfers to the wallet for a specific account and subaddress indices.
        /// </summary>
        /// <param name="transferType">The <paramref name="transferType"/> value.</param>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="subaddrIndices">The subaddress indices to query.</param>
        /// <param name="returnKeyImage">The <paramref name="returnKeyImage"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<IncomingTransfer>> GetIncomingTransfersAsync(TransferType transferType, uint accountIndex, IEnumerable<uint> subaddrIndices, bool returnKeyImage = false, CancellationToken token = default);

        /// <summary>
        /// Return the spend or view private key.
        /// </summary>
        /// <param name="keyType">The <paramref name="keyType"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> GetPrivateKey(KeyType keyType, CancellationToken token = default);

        /// <summary>
        /// Stops the wallet, storing the current state.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<StopWalletResult> StopWalletAsync(CancellationToken token = default);

        /// <summary>
        /// Set arbitrary string notes for transactions.
        /// </summary>
        /// <param name="txids">The <paramref name="txids"/> value.</param>
        /// <param name="notes">The <paramref name="notes"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<SetTransactionNotes> SetTransactionNotesAsync(IEnumerable<string> txids, IEnumerable<string> notes, CancellationToken token = default);

        /// <summary>
        /// Get string notes for transactions of interest.
        /// </summary>
        /// <param name="txids">The <paramref name="txids"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<string>> GetTransactionNotesAsync(IEnumerable<string> txids, CancellationToken token = default);

        /// <summary>
        /// Get transaction secret key from transaction id.
        /// </summary>
        /// <param name="txid">The <paramref name="txid"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> GetTransactionKeyAsync(string txid, CancellationToken token = default);

        /// <summary>
        /// Check a transaction in the blockchain with its secret key.
        /// </summary>
        /// <param name="txid">The <paramref name="txid"/> value.</param>
        /// <param name="txKey">The <paramref name="txKey"/> value.</param>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<CheckTransactionKey> CheckTransactionKeyAsync(string txid, string txKey, string address, CancellationToken token = default);

        /// <summary>
        /// Returns a list of transfers from one or more of the desired categories.
        /// </summary>
        /// <param name="in">The <paramref name="in"/> value.</param>
        /// <param name="out">The <paramref name="out"/> value.</param>
        /// <param name="pending">The <paramref name="pending"/> value.</param>
        /// <param name="failed">The <paramref name="failed"/> value.</param>
        /// <param name="pool">The <paramref name="pool"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<ShowTransfers> GetTransfersAsync(bool @in, bool @out, bool pending, bool failed, bool pool, CancellationToken token = default);

        /// <summary>
        /// Returns a list of transfers filtered by block height range.
        /// </summary>
        /// <param name="in">The <paramref name="in"/> value.</param>
        /// <param name="out">The <paramref name="out"/> value.</param>
        /// <param name="pending">The <paramref name="pending"/> value.</param>
        /// <param name="failed">The <paramref name="failed"/> value.</param>
        /// <param name="pool">The <paramref name="pool"/> value.</param>
        /// <param name="minHeight">The <paramref name="minHeight"/> value.</param>
        /// <param name="maxHeight">The <paramref name="maxHeight"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<ShowTransfers> GetTransfersAsync(bool @in, bool @out, bool pending, bool failed, bool pool, ulong minHeight, ulong maxHeight, CancellationToken token = default);

        /// <summary>
        /// Show information about a transfer to/from this address by transaction id.
        /// </summary>
        /// <param name="txid">The <paramref name="txid"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<Transfer> GetTransferByTxidAsync(string txid, CancellationToken token = default);

        /// <summary>
        /// Show information about a transfer to/from this address by transaction id and account index.
        /// </summary>
        /// <param name="txid">The <paramref name="txid"/> value.</param>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<Transfer> GetTransferByTxidAsync(string txid, uint accountIndex, CancellationToken token = default);

        /// <summary>
        /// Sign a string.
        /// </summary>
        /// <param name="data">The <paramref name="data"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> SignAsync(string data, CancellationToken token = default);

        /// <summary>
        /// Verify a signature on a string.
        /// </summary>
        /// <param name="data">The <paramref name="data"/> value.</param>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="signature">The <paramref name="signature"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<bool> VerifyAsync(string data, string address, string signature, CancellationToken token = default);

        /// <summary>
        /// Export outputs in hex format.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> ExportOutputsAsync(CancellationToken token = default);

        /// <summary>
        /// Import outputs in hex format.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<ulong> ImportOutputsAsync(CancellationToken token = default);

        /// <summary>
        /// Export a signed set of key images.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<SignedKeyImage>> ExportKeyImagesAsync(CancellationToken token = default);

        /// <summary>
        /// Import signed key images list and verify their spent status.
        /// </summary>
        /// <param name="signedKeyImages">The <paramref name="signedKeyImages"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<ImportKeyImages> ImportKeyImagesAsync(IEnumerable<(string KeyImage, string Signature)> signedKeyImages, CancellationToken token = default);

        /// <summary>
        /// Create a payment URI using the official URI spec.
        /// </summary>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="amount">The <paramref name="amount"/> value.</param>
        /// <param name="recipientName">The <paramref name="recipientName"/> value.</param>
        /// <param name="txDescription">The <paramref name="txDescription"/> value.</param>
        /// <param name="paymentId">The <paramref name="paymentId"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> MakeUriAsync(string address, ulong amount, string recipientName, string? txDescription = null, string? paymentId = null, CancellationToken token = default);

        /// <summary>
        /// Parse a payment URI to get payment information.
        /// </summary>
        /// <param name="uri">The <paramref name="uri"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<MoneroUri> ParseUriAsync(string uri, CancellationToken token = default);

        /// <summary>
        /// Retrieves one or more entries from the wallet address book.
        /// </summary>
        /// <param name="entires">The address-book entry indices to retrieve.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the requested address-book entries.</returns>
        Task<AddressBook> GetAddressBookAsync(IEnumerable<uint> entires, CancellationToken token = default);

        /// <summary>
        /// Add an entry to the address book.
        /// </summary>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="description">The <paramref name="description"/> value.</param>
        /// <param name="paymentId">The <paramref name="paymentId"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<AddAddressBook> AddAddressBookAsync(string address, string? description = null, string? paymentId = null, CancellationToken token = default);

        /// <summary>
        /// Delete an entry from the address book.
        /// </summary>
        /// <param name="index">The <paramref name="index"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<DeleteAddressBook> DeleteAddressBookAsync(uint index, CancellationToken token = default);

        /// <summary>
        /// Refresh a wallet after opening.
        /// </summary>
        /// <param name="startHeight">The <paramref name="startHeight"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<RefreshWallet> RefreshWalletAsync(uint startHeight, CancellationToken token = default);

        /// <summary>
        /// Rescan the blockchain for spent outputs.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<RescanSpent> RescanSpentAsync(CancellationToken token = default);

        /// <summary>
        /// Create a new wallet. Requires <c>--wallet-dir</c> when launching <c>monero-wallet-rpc</c>.
        /// </summary>
        /// <param name="filename">The <paramref name="filename"/> value.</param>
        /// <param name="language">The <paramref name="language"/> value.</param>
        /// <param name="password">The <paramref name="password"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<CreateWallet> CreateWalletAsync(string filename, string language, string? password = null, CancellationToken token = default);

        /// <summary>
        /// Open a wallet. Requires <c>--wallet-dir</c> when launching <c>monero-wallet-rpc</c>.
        /// </summary>
        /// <param name="filename">The <paramref name="filename"/> value.</param>
        /// <param name="password">The <paramref name="password"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<OpenWallet> OpenWalletAsync(string filename, string? password = null, CancellationToken token = default);

        /// <summary>
        /// Close the currently opened wallet, after trying to save it.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<CloseWallet> CloseWalletAsync(CancellationToken token = default);

        /// <summary>
        /// Change a wallet password.
        /// </summary>
        /// <param name="oldPassword">The <paramref name="oldPassword"/> value.</param>
        /// <param name="newPassword">The <paramref name="newPassword"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<ChangeWalletPassword> ChangeWalletPasswordAsync(string? oldPassword = null, string? newPassword = null, CancellationToken token = default);

        /// <summary>
        /// Get RPC version Major and Minor integer-format, where Major is the first 16 bits and Minor the last 16 bits.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<uint> GetVersionAsync(CancellationToken token = default);

        /// <summary>
        /// Gets the wallet RPC version as a structured <see cref="MoneroVersion"/> with major, minor, and patch components.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<MoneroVersion> GetStructuredVersionAsync(CancellationToken token = default);

        /// <summary>
        /// Check if a wallet is a multisig one.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<MultiSigInformation> GetMultiSigInformationAsync(CancellationToken token = default);

        /// <summary>
        /// Prepare a wallet for multisig by generating a multisig string to share with peers.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> PrepareMultiSigAsync(CancellationToken token = default);

        /// <summary>
        /// Make a wallet multisig by importing peers multisig string.
        /// </summary>
        /// <param name="multiSigInfo">The <paramref name="multiSigInfo"/> value.</param>
        /// <param name="threshold">The <paramref name="threshold"/> value.</param>
        /// <param name="password">The <paramref name="password"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<MakeMultiSig> MakeMultiSigAsync(IEnumerable<string> multiSigInfo, uint threshold, string password, CancellationToken token = default);

        /// <summary>
        /// Exchange multisig key material with cosigners.
        /// </summary>
        /// <param name="multisigInfo">The <paramref name="multisigInfo"/> value.</param>
        /// <param name="password">The <paramref name="password"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<ExchangeMultiSigKeys> ExchangeMultiSigKeysAsync(IEnumerable<string> multisigInfo, string password, CancellationToken token = default);

        /// <summary>
        /// Export multisig info for other participants.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> ExportMultiSigInfoAsync(CancellationToken token = default);

        /// <summary>
        /// Import multisig info from other participants.
        /// </summary>
        /// <param name="info">The <paramref name="info"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<ImportMultiSigInformation> ImportMultiSigInfoAsync(IEnumerable<string> info, CancellationToken token = default);

        /// <summary>
        /// Turn this wallet into a multisig wallet, extra step for N-1/N wallets.
        /// </summary>
        /// <param name="multisigInfo">The <paramref name="multisigInfo"/> value.</param>
        /// <param name="password">The <paramref name="password"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> FinalizeMultiSigAsync(IEnumerable<string> multisigInfo, string password, CancellationToken token = default);

        /// <summary>
        /// Sign a transaction in multisig.
        /// </summary>
        /// <param name="txDataHex">The <paramref name="txDataHex"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<SignMultiSigTransaction> SignMultiSigAsync(string txDataHex, CancellationToken token = default);

        /// <summary>
        /// Submit a signed multisig transaction.
        /// </summary>
        /// <param name="txDataHex">The <paramref name="txDataHex"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<string>> SubmitMultiSigAsync(string txDataHex, CancellationToken token = default);

        /// <summary>
        /// Retrieve the payment details associated with a given payment id.
        /// </summary>
        /// <param name="paymentId">The <paramref name="paymentId"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<PaymentDetail>> GetPaymentDetailAsync(string paymentId, CancellationToken token = default);

        /// <summary>
        /// Retrieve payments for multiple payment ids.
        /// </summary>
        /// <param name="paymentIds">The <paramref name="paymentIds"/> value.</param>
        /// <param name="minBlockHeight">The <paramref name="minBlockHeight"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<List<PaymentDetail>> GetBulkPaymentsAsync(IEnumerable<string> paymentIds, ulong? minBlockHeight = null, CancellationToken token = default);

        /// <summary>
        /// Validate a Monero address using wallet RPC rules.
        /// </summary>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="anyNetType">The <paramref name="anyNetType"/> value.</param>
        /// <param name="allowOpenAlias">The <paramref name="allowOpenAlias"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<ValidateAddress> ValidateAddressAsync(string address, bool anyNetType = false, bool allowOpenAlias = true, CancellationToken token = default);

        /// <summary>
        /// Create an integrated address from a standard address and payment id.
        /// </summary>
        /// <param name="standardAddress">The <paramref name="standardAddress"/> value.</param>
        /// <param name="paymentId">The <paramref name="paymentId"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<IntegratedAddress> MakeIntegratedAddressAsync(string? standardAddress = null, string? paymentId = null, CancellationToken token = default);

        /// <summary>
        /// Split an integrated address into standard address and payment id.
        /// </summary>
        /// <param name="integratedAddress">The <paramref name="integratedAddress"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<IntegratedAddress> SplitIntegratedAddressAsync(string integratedAddress, CancellationToken token = default);

        /// <summary>
        /// Enable or disable automatic wallet refresh.
        /// </summary>
        /// <param name="enabled">The <paramref name="enabled"/> value.</param>
        /// <param name="periodSeconds">The <paramref name="periodSeconds"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetAutoRefreshAsync(bool enabled, uint? periodSeconds = null, CancellationToken token = default);

        /// <summary>
        /// Scan wallet transactions by transaction id.
        /// </summary>
        /// <param name="txIds">The <paramref name="txIds"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ScanTransactionsAsync(IEnumerable<string> txIds, CancellationToken token = default);

        /// <summary>
        /// Rescan blockchain data for the current wallet.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RescanBlockchainAsync(CancellationToken token = default);

        /// <summary>
        /// Relay transaction metadata produced by transfer operations.
        /// </summary>
        /// <param name="txMetadata">The <paramref name="txMetadata"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> RelayTransactionMetadataAsync(string txMetadata, CancellationToken token = default);

        /// <summary>
        /// Start wallet-managed mining.
        /// </summary>
        /// <param name="threadsCount">The <paramref name="threadsCount"/> value.</param>
        /// <param name="doBackgroundMining">The <paramref name="doBackgroundMining"/> value.</param>
        /// <param name="ignoreBattery">The <paramref name="ignoreBattery"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartMiningAsync(ulong threadsCount, bool doBackgroundMining, bool ignoreBattery, CancellationToken token = default);

        /// <summary>
        /// Stop wallet-managed mining.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StopMiningAsync(CancellationToken token = default);

        /// <summary>
        /// Freeze a wallet output by key image.
        /// </summary>
        /// <param name="keyImage">The <paramref name="keyImage"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task FreezeOutputAsync(string keyImage, CancellationToken token = default);

        /// <summary>
        /// Unfreeze a previously frozen wallet output.
        /// </summary>
        /// <param name="keyImage">The <paramref name="keyImage"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ThawOutputAsync(string keyImage, CancellationToken token = default);

        /// <summary>
        /// Check whether an output is currently frozen.
        /// </summary>
        /// <param name="keyImage">The <paramref name="keyImage"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<bool> IsOutputFrozenAsync(string keyImage, CancellationToken token = default);

        /// <summary>
        /// Get the wallet's default transfer priority.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<TransferPriority> GetDefaultFeePriorityAsync(CancellationToken token = default);

        /// <summary>
        /// Generate a transaction proof signature.
        /// </summary>
        /// <param name="txid">The <paramref name="txid"/> value.</param>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="message">The <paramref name="message"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> GetTransactionProofAsync(string txid, string address, string? message = null, CancellationToken token = default);

        /// <summary>
        /// Verify a transaction proof signature.
        /// </summary>
        /// <param name="txid">The <paramref name="txid"/> value.</param>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="message">The <paramref name="message"/> value.</param>
        /// <param name="signature">The <paramref name="signature"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<CheckTransactionProof> CheckTransactionProofAsync(string txid, string address, string message, string signature, CancellationToken token = default);

        /// <summary>
        /// Generate a spend proof signature.
        /// </summary>
        /// <param name="txid">The <paramref name="txid"/> value.</param>
        /// <param name="message">The <paramref name="message"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> GetSpendProofAsync(string txid, string? message = null, CancellationToken token = default);

        /// <summary>
        /// Verify a spend proof signature.
        /// </summary>
        /// <param name="txid">The <paramref name="txid"/> value.</param>
        /// <param name="message">The <paramref name="message"/> value.</param>
        /// <param name="signature">The <paramref name="signature"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<bool> CheckSpendProofAsync(string txid, string message, string signature, CancellationToken token = default);

        /// <summary>
        /// Generate a reserve proof signature.
        /// </summary>
        /// <param name="message">The <paramref name="message"/> value.</param>
        /// <param name="accountIndex">The wallet account index.</param>
        /// <param name="amount">The <paramref name="amount"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> GetReserveProofAsync(string message, uint? accountIndex = null, ulong? amount = null, CancellationToken token = default);

        /// <summary>
        /// Verify a reserve proof signature.
        /// </summary>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="message">The <paramref name="message"/> value.</param>
        /// <param name="signature">The <paramref name="signature"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<CheckReserveProof> CheckReserveProofAsync(string address, string message, string signature, CancellationToken token = default);

        /// <summary>
        /// Set wallet daemon connection parameters.
        /// </summary>
        /// <param name="connection">The <paramref name="connection"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetDaemonConnectionAsync(MoneroDaemonConnection connection, CancellationToken token = default);

        /// <summary>
        /// Estimate transaction size and weight for a transfer configuration.
        /// </summary>
        /// <param name="config">The <paramref name="config"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<EstimateTxSizeAndWeight> EstimateTransactionSizeAndWeightAsync(MoneroTxConfig config, CancellationToken token = default);

        /// <summary>
        /// Initialize wallet background sync.
        /// </summary>
        /// <param name="walletPassword">The <paramref name="walletPassword"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetupBackgroundSyncAsync(string? walletPassword = null, CancellationToken token = default);

        /// <summary>
        /// Start background synchronization.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartBackgroundSyncAsync(CancellationToken token = default);

        /// <summary>
        /// Stop background synchronization.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StopBackgroundSyncAsync(CancellationToken token = default);

        /// <summary>
        /// Generate a wallet from explicit key material.
        /// </summary>
        /// <param name="options">The <paramref name="options"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<GenerateFromKeysWallet> GenerateFromKeysWalletAsync(GenerateFromKeysOptions options, CancellationToken token = default);

        /// <summary>
        /// Restore a deterministic wallet from seed information.
        /// </summary>
        /// <param name="options">The <paramref name="options"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<RestoreDeterministicWallet> RestoreDeterministicWalletAsync(RestoreDeterministicWalletOptions options, CancellationToken token = default);

        /// <summary>
        /// Edit an existing address-book entry.
        /// </summary>
        /// <param name="index">The <paramref name="index"/> value.</param>
        /// <param name="setAddress">The <paramref name="setAddress"/> value.</param>
        /// <param name="address">The <paramref name="address"/> value.</param>
        /// <param name="setDescription">The <paramref name="setDescription"/> value.</param>
        /// <param name="description">The <paramref name="description"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task EditAddressBookAsync(uint index, bool setAddress, string? address, bool setDescription, string? description, CancellationToken token = default);

        /// <summary>
        /// Creates a wallet using fluent wallet configuration.
        /// </summary>
        /// <param name="config">Wallet creation configuration.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the created wallet details.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is <see langword="null"/>.</exception>
        Task<CreateWallet> CreateWalletAsync(MoneroWalletConfig config, CancellationToken token = default);

        /// <summary>
        /// Opens a wallet using fluent wallet configuration.
        /// </summary>
        /// <param name="config">Wallet open configuration.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the open-wallet result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is <see langword="null"/>.</exception>
        Task<OpenWallet> OpenWalletAsync(MoneroWalletConfig config, CancellationToken token = default);

        /// <summary>
        /// Transfers funds using fluent transaction configuration.
        /// </summary>
        /// <param name="config">Transfer configuration.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to transfer result details.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is <see langword="null"/>.</exception>
        Task<FundTransfer> TransferAsync(MoneroTxConfig config, CancellationToken token = default);

        /// <summary>
        /// Transfers funds in split mode using fluent transaction configuration.
        /// </summary>
        /// <param name="config">Split-transfer configuration.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to split transfer result details.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is <see langword="null"/>.</exception>
        Task<SplitFundTransfer> TransferSplitAsync(MoneroTxConfig config, CancellationToken token = default);

        /// <summary>
        /// Subscribes to wallet polling notifications.
        /// </summary>
        /// <param name="listener">Callback container for polling events.</param>
        /// <param name="options">Optional polling configuration.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to an async-disposable subscription handle.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="listener"/> is <see langword="null"/>.</exception>
        Task<IAsyncDisposable> SubscribeAsync(MoneroWalletListener listener, MoneroWalletPollingOptions? options = null, CancellationToken token = default);

        /// <summary>
        /// Set an attribute (key/value pair) to store additional info in the wallet.
        /// </summary>
        /// <param name="key">The <paramref name="key"/> value.</param>
        /// <param name="value">The <paramref name="value"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetAttributeAsync(string key, string value, CancellationToken token = default);

        /// <summary>
        /// Get an attribute value from the wallet, given its key.
        /// </summary>
        /// <param name="key">The <paramref name="key"/> value.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that resolves to the operation result.</returns>
        Task<string> GetAttributeAsync(string key, CancellationToken token = default);
    }
}
