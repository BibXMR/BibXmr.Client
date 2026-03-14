using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;
using BibXmr.Client.Wallet.Dto;
using BibXmr.Client.Wallet.Dto.Responses;

#if NET9_0_OR_GREATER
using SyncLock = System.Threading.Lock;
#else
using SyncLock = System.Object;
#endif

namespace BibXmr.Client.Wallet
{
    /// <summary>
    /// High-level client for interacting with <c>monero-wallet-rpc</c> via its JSON-RPC interface.
    /// </summary>
    public class MoneroWalletClient : IMoneroWalletClient
    {
        private readonly RpcCommunicator _moneroRpcCommunicator;

        /// <summary>
        /// Synchronization lock guarding the disposal flag.
        /// </summary>
        private readonly object _disposingLock = new();

        /// <summary>
        /// Synchronization lock guarding the subscriptions list.
        /// </summary>
        private readonly object _subscriptionLock = new();
        private readonly List<IAsyncDisposable> _subscriptions = [];
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoneroWalletClient"/> class using an existing RPC communicator.
        /// </summary>
        /// <param name="rpcCommunicator">The RPC communicator used for wallet communication.</param>
        private MoneroWalletClient(RpcCommunicator rpcCommunicator)
        {
            _moneroRpcCommunicator = rpcCommunicator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoneroWalletClient"/> class targeting the given host and port.
        /// </summary>
        /// <param name="url">The wallet RPC host name or IP address.</param>
        /// <param name="port">The wallet RPC port number.</param>
        private MoneroWalletClient(string url, uint port)
        {
            _moneroRpcCommunicator = new RpcCommunicator(url, port, ConnectionType.Wallet);
        }

        /// <summary>
        /// Initialize a Monero Wallet Client using default network settings (&lt;localhost&gt;:&lt;defaultport&gt;).
        /// </summary>
        private MoneroWalletClient(MoneroNetwork networkType)
        {
            _moneroRpcCommunicator = new RpcCommunicator(networkType, ConnectionType.Wallet);
        }

        /// <summary>
        /// Initialize a Monero Wallet Client using default network settings (&lt;localhost&gt;:&lt;defaultport&gt;), opening the wallet while doing so.
        /// </summary>
        public static Task<MoneroWalletClient> CreateAsync(string url, uint port, string filename, string password, CancellationToken cancellationToken = default)
        {
            var moneroWalletClient = new MoneroWalletClient(url, port);
            return moneroWalletClient.InitializeAsync(filename, password, cancellationToken);
        }

        /// <summary>
        /// Initialize a Monero Wallet Client using an externally-managed <see cref="HttpClient"/>, opening the wallet while doing so.
        /// </summary>
        public static Task<MoneroWalletClient> CreateAsync(HttpClient httpClient, Uri baseAddress, string filename, string password, MoneroRpcClientOptions? options = null, CancellationToken ct = default)
        {
            var rpcCommunicator = new RpcCommunicator(httpClient, baseAddress, ConnectionType.Wallet, options);
            var moneroWalletClient = new MoneroWalletClient(rpcCommunicator);
            return moneroWalletClient.InitializeAsync(filename, password, ct);
        }

        /// <summary>
        /// Initialize a Monero Wallet Client using default settings for the specified Monero network, opening the wallet while doing so.
        /// </summary>
        public static Task<MoneroWalletClient> CreateAsync(MoneroNetwork networkType, string filename, string password, CancellationToken cancellationToken = default)
        {
            var moneroWalletClient = new MoneroWalletClient(networkType);
            return moneroWalletClient.InitializeAsync(filename, password, cancellationToken);
        }

        /// <summary>
        /// Asynchronously disposes the client, saving and closing the wallet.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            lock (_disposingLock)
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

            await SaveWalletAsync().ConfigureAwait(false);
            await CloseWalletAsync().ConfigureAwait(false);

            _moneroRpcCommunicator.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object (also calls <see cref="CloseWalletAsync(CancellationToken)"/>).
        /// Prefer <see cref="DisposeAsync"/> to avoid sync-over-async deadlock risk.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns wallet's balance.
        /// </summary>
        public async Task<Balance> GetBalanceAsync(uint accountIndex, IEnumerable<uint> addressIndices, bool allAccounts = false, bool strict = false, CancellationToken token = default)
        {
            BalanceResponse result = await _moneroRpcCommunicator.GetBalanceAsync(accountIndex, addressIndices, allAccounts, strict, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBalanceAsync));
            return result.Result!;
        }

        /// <summary>
        /// Returns wallet's balance for a given account index.
        /// </summary>
        public async Task<Balance> GetBalanceAsync(uint accountIndex, CancellationToken token = default)
        {
            BalanceResponse result = await _moneroRpcCommunicator.GetBalanceAsync(accountIndex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetBalanceAsync));
            return result.Result!;
        }

        /// <summary>
        /// Return the wallet's addresses for an account.
        /// </summary>
        public async Task<Addresses> GetAddressAsync(uint accountIndex, CancellationToken token = default)
        {
            AddressResponse result = await _moneroRpcCommunicator.GetAddressAsync(accountIndex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetAddressAsync));
            return result.Result!;
        }

        /// <summary>
        /// <seealso cref="GetAddressAsync(uint, CancellationToken)"/> Also filter for specific set of subaddresses.
        /// </summary>
        public async Task<Addresses> GetAddressAsync(uint accountIndex, IEnumerable<uint> addressIndices, CancellationToken token = default)
        {
            AddressResponse result = await _moneroRpcCommunicator.GetAddressAsync(accountIndex, addressIndices, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetAddressAsync));
            return result.Result!;
        }

        /// <summary>
        /// Get account and address indexes from a specific (sub)address
        /// </summary>
        public async Task<AddressIndex> GetAddressIndexAsync(string address, CancellationToken token = default)
        {
            AddressIndexResponse result = await _moneroRpcCommunicator.GetAddressIndexAsync(address, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetAddressIndexAsync));
            return result.Result!;
        }

        /// <summary>
        /// Create a new address for an account.
        /// </summary>
        public async Task<AddressCreation> CreateAddressAsync(uint accountIndex, CancellationToken token = default)
        {
            AddressCreationResponse result = await _moneroRpcCommunicator.CreateAddressAsync(accountIndex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.CreateAddressAsync));
            return result.Result!;
        }

        /// <summary>
        /// Create a new address for an account and apply a label to the new subaddress.
        /// </summary>
        /// <seealso cref="CreateAddressAsync(uint, CancellationToken)"/>
        public async Task<AddressCreation> CreateAddressAsync(uint accountIndex, string label, CancellationToken token = default)
        {
            AddressCreationResponse result = await _moneroRpcCommunicator.CreateAddressAsync(accountIndex, label, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.CreateAddressAsync));
            return result.Result!;
        }

        /// <summary>
        /// Label an address.
        /// </summary>
        public async Task<AddressLabel> LabelAddressAsync(uint majorIndex, uint minorIndex, string label, CancellationToken token = default)
        {
            AddressLabelResponse result = await _moneroRpcCommunicator.LabelAddressAsync(majorIndex, minorIndex, label, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.LabelAddressAsync));
            return result.Result!;
        }

        /// <summary>
        /// Get all accounts for a wallet.
        /// </summary>
        public async Task<Account> GetAccountsAsync(CancellationToken token = default)
        {
            AccountResponse result = await _moneroRpcCommunicator.GetAccountsAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetAccountsAsync));
            return result.Result!;
        }

        /// <summary>
        /// <seealso cref="GetAccountsAsync(CancellationToken)"/> Also filter accounts by tag.
        /// </summary>
        public async Task<Account> GetAccountsAsync(string tag, CancellationToken token = default)
        {
            AccountResponse result = await _moneroRpcCommunicator.GetAccountsAsync(tag, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetAccountsAsync));
            return result.Result!;
        }

        /// <summary>
        /// Create a new account.
        /// </summary>
        public async Task<CreateAccount> CreateAccountAsync(CancellationToken token = default)
        {
            CreateAccountResponse result = await _moneroRpcCommunicator.CreateAccountAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.CreateAccountAsync));
            return result.Result!;
        }

        /// <summary>
        /// <seealso cref="CreateAccountAsync(CancellationToken)"/> Doing so with an optional label.
        /// </summary>
        public async Task<CreateAccount> CreateAccountAsync(string label, CancellationToken token = default)
        {
            CreateAccountResponse result = await _moneroRpcCommunicator.CreateAccountAsync(label, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.CreateAccountAsync));
            return result.Result!;
        }

        /// <summary>
        /// Label an account.
        /// </summary>
        public async Task<AccountLabel> LabelAccountAsync(uint accountIndex, string label, CancellationToken token = default)
        {
            AccountLabelResponse result = await _moneroRpcCommunicator.LabelAccountAsync(accountIndex, label, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.LabelAccountAsync));
            return result.Result!;
        }

        /// <summary>
        /// Get a list of user-defined account tags.
        /// </summary>
        public async Task<AccountTags> GetAccountTagsAsync(CancellationToken token = default)
        {
            AccountTagsResponse result = await _moneroRpcCommunicator.GetAccountTagsAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetAccountTagsAsync));
            return result.Result!;
        }

        /// <summary>
        /// Apply a filtering tag to a list of accounts.
        /// </summary>
        public async Task<TagAccounts> TagAccountsAsync(string tag, IEnumerable<uint> accounts, CancellationToken token = default)
        {
            TagAccountsResponse result = await _moneroRpcCommunicator.TagAccountsAsync(tag, accounts, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.TagAccountsAsync));
            return result.Result!;
        }

        /// <summary>
        /// Remove filtering tag from a list of accounts.
        /// </summary>
        public async Task<UntagAccounts> UntagAccountsAsync(IEnumerable<uint> accounts, CancellationToken token = default)
        {
            UntagAccountsResponse result = await _moneroRpcCommunicator.UntagAccountsAsync(accounts, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.UntagAccountsAsync));
            return result.Result!;
        }

        /// <summary>
        /// Set description for an account tag.
        /// </summary>
        public async Task<AccountTagAndDescription> SetAccountTagDescriptionAsync(string tag, string description, CancellationToken token = default)
        {
            AccountTagAndDescriptionResponse result = await _moneroRpcCommunicator.SetAccountTagDescriptionAsync(tag, description, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SetAccountTagDescriptionAsync));
            return result.Result!;
        }

        /// <summary>
        /// Returns the wallet's current block height.
        /// </summary>
        public async Task<ulong> GetHeightAsync(CancellationToken token = default)
        {
            BlockchainHeightResponse result = await _moneroRpcCommunicator.GetHeightAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetHeightAsync));
            return result.Result!.Height;
        }

        /// <summary>
        /// Send monero to a number of recipients.
        /// </summary>
        public async Task<FundTransfer> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, CancellationToken token = default)
        {
            FundTransferResponse result = await _moneroRpcCommunicator.TransferAsync(transactions, transferPriority, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.TransferAsync));
            return result.Result!;
        }

        /// <summary>
        /// Send funds while optionally returning transaction key and transaction hex data.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        public async Task<FundTransfer> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, bool getTxKey, bool getTxHex, ulong unlockTime = 0, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfUnlockTimeIsToolarge(unlockTime, nameof(unlockTime));
            FundTransferResponse result = await _moneroRpcCommunicator.TransferAsync(transactions, transferPriority, getTxKey, getTxHex, unlockTime, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.TransferAsync));
            return result.Result!;
        }

        /// <summary>
        /// Send funds with explicit ring size control and optional transaction key and transaction hex output.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        public async Task<FundTransfer> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, uint ringSize, ulong unlockTime = 0, bool getTxKey = true, bool getTxHex = true, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfUnlockTimeIsToolarge(unlockTime, nameof(unlockTime));
            FundTransferResponse result = await _moneroRpcCommunicator.TransferAsync(transactions, transferPriority, ringSize, unlockTime, getTxKey, getTxHex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.TransferAsync));
            return result.Result!;
        }

        /// <summary>
        /// Send funds from a specific account with explicit ring size control and optional transaction key and transaction hex output.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        public async Task<FundTransfer> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, uint ringSize, uint accountIndex, ulong unlockTime = 0, bool getTxKey = true, bool getTxHex = true, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfUnlockTimeIsToolarge(unlockTime, nameof(unlockTime));
            FundTransferResponse result = await _moneroRpcCommunicator.TransferAsync(transactions, transferPriority, ringSize, accountIndex, unlockTime, getTxKey, getTxHex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.TransferAsync));
            return result.Result!;
        }

        /// <summary>
        /// Same as transfer, but can split into more than one tx if necessary.
        /// </summary>
        public async Task<SplitFundTransfer> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, bool newAlgorithm = true, CancellationToken token = default)
        {
            SplitFundTransferResponse result = await _moneroRpcCommunicator.TransferSplitAsync(transactions, transferPriority, newAlgorithm, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.TransferSplitAsync));
            return result.Result!;
        }

        /// <summary>
        /// Send a split transfer while optionally returning transaction key and transaction hex data.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        public async Task<SplitFundTransfer> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, bool getTxKey, bool getTxHex, bool newAlgorithm = true, ulong unlockTime = 0, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfUnlockTimeIsToolarge(unlockTime, nameof(unlockTime));
            SplitFundTransferResponse result = await _moneroRpcCommunicator.TransferSplitAsync(transactions, transferPriority, getTxKey, getTxHex, newAlgorithm, unlockTime, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.TransferSplitAsync));
            return result.Result!;
        }

        /// <summary>
        /// Send a split transfer with explicit ring size control and optional transaction key and transaction hex output.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        public async Task<SplitFundTransfer> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, uint ringSize, bool newAlgorithm = true, ulong unlockTime = 0, bool getTxKey = true, bool getTxHex = true, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfUnlockTimeIsToolarge(unlockTime, nameof(unlockTime));
            SplitFundTransferResponse result = await _moneroRpcCommunicator.TransferSplitAsync(transactions, transferPriority, ringSize, newAlgorithm, unlockTime, getTxKey, getTxHex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.TransferSplitAsync));
            return result.Result!;
        }

        /// <summary>
        /// Send a split transfer from a specific account with explicit ring size control and optional transaction key and transaction hex output.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds the supported safety threshold.</exception>
        public async Task<SplitFundTransfer> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transferPriority, uint ringSize, uint accountIndex, bool newAlgorithm = true, ulong unlockTime = 0, bool getTxKey = true, bool getTxHex = true, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfUnlockTimeIsToolarge(unlockTime, nameof(unlockTime));
            SplitFundTransferResponse result = await _moneroRpcCommunicator.TransferSplitAsync(transactions, transferPriority, ringSize, accountIndex, newAlgorithm, unlockTime, getTxKey, getTxHex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.TransferSplitAsync));
            return result.Result!;
        }

        /// <summary>
        /// Sign a transaction created on a read-only wallet (in cold-signing process).
        /// </summary>
        public async Task<SignTransfer> SignTransferAsync(string unsignedTxSet, bool exportRaw = false, CancellationToken token = default)
        {
            SignTransferResponse result = await _moneroRpcCommunicator.SignTransferAsync(unsignedTxSet, exportRaw, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SignTransferAsync));
            return result.Result!;
        }

        /// <summary>
        /// Submit a previously signed transaction on a read-only wallet (in cold-signing process).
        /// </summary>
        /// <returns>A list of transaction hashes.</returns>
        public async Task<List<string>> SubmitTransferAsync(string txDataHex, CancellationToken token = default)
        {
            SubmitTransferResponse result = await _moneroRpcCommunicator.SubmitTransferAsync(txDataHex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SubmitTransferAsync));
            return result.Result!.TransactionHashes;
        }

        /// <summary>
        /// Send all dust outputs back to the wallet's, to make them easier to spend (and mix).
        /// </summary>
        public async Task<SweepDust> SweepDustAsync(bool getTxKey, bool getTxHex, bool getTxMetadata, bool doNotRelay = false, CancellationToken token = default)
        {
            SweepDustResponse result = await _moneroRpcCommunicator.SweepDustAsync(getTxKey, getTxHex, getTxMetadata, doNotRelay, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SweepDustAsync));
            return result.Result!;
        }

        /// <summary>
        /// Send all unlocked balance to an address. Be careful...
        /// </summary>
        public async Task<SplitFundTransfer> SweepAllAsync(string address, uint accountIndex, TransferPriority transactionPriority, uint ringSize, ulong unlockTime = 0, ulong belowAmount = ulong.MaxValue, bool getTxKeys = true, bool getTxHex = true, bool getTxMetadata = true, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfUnlockTimeIsToolarge(unlockTime, nameof(unlockTime));
            SweepAllResponse result = await _moneroRpcCommunicator.SweepAllAsync(address, accountIndex, transactionPriority, ringSize, unlockTime, belowAmount, getTxKeys, getTxHex, getTxMetadata, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SweepAllAsync));
            return result.Result!;
        }

        /// <summary>
        /// Save the wallet.
        /// </summary>
        public async Task<SaveWallet> SaveWalletAsync(CancellationToken token = default)
        {
            SaveWalletResponse result = await _moneroRpcCommunicator.SaveWalletAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SaveWalletAsync));
            return result.Result!;
        }

        /// <summary>
        /// Return a list of incoming transfers to the wallet.
        /// </summary>
        public async Task<List<IncomingTransfer>> GetIncomingTransfersAsync(TransferType transferType, bool returnKeyImage = false, CancellationToken token = default)
        {
            IncomingTransfersResponse result = await _moneroRpcCommunicator.GetIncomingTransfersAsync(transferType, returnKeyImage, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetIncomingTransfersAsync));
            return result.Result!.Transfers;
        }

        /// <summary>
        /// <seealso cref="GetIncomingTransfersAsync(TransferType, bool, CancellationToken)"/>
        /// </summary>
        public async Task<List<IncomingTransfer>> GetIncomingTransfersAsync(TransferType transferType, uint accountIndex, bool returnKeyImage = false, CancellationToken token = default)
        {
            IncomingTransfersResponse result = await _moneroRpcCommunicator.GetIncomingTransfersAsync(transferType, accountIndex, returnKeyImage, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetIncomingTransfersAsync));
            return result.Result!.Transfers;
        }

        /// <summary>
        /// <seealso cref="GetIncomingTransfersAsync(TransferType, bool, CancellationToken)"/>
        /// </summary>
        public async Task<List<IncomingTransfer>> GetIncomingTransfersAsync(TransferType transferType, uint accountIndex, IEnumerable<uint> subaddrIndices, bool returnKeyImage = false, CancellationToken token = default)
        {
            IncomingTransfersResponse result = await _moneroRpcCommunicator.GetIncomingTransfersAsync(transferType, accountIndex, subaddrIndices, returnKeyImage, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetIncomingTransfersAsync));
            return result.Result!.Transfers;
        }

        /// <summary>
        /// Return the spend or view private key.
        /// </summary>
        public async Task<string> GetPrivateKey(KeyType keyType, CancellationToken token = default)
        {
            QueryKeyResponse result = await _moneroRpcCommunicator.GetPrivateKey(keyType, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetPrivateKey));
            return result.Result!.Key;
        }

        /// <summary>
        /// Stops the wallet, storing the current state.
        /// </summary>
        public async Task<StopWalletResult> StopWalletAsync(CancellationToken token = default)
        {
            StopWalletResponse result = await _moneroRpcCommunicator.StopWalletAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.StopWalletAsync));
            return result.Result!;
        }

        /// <summary>
        /// Set arbitrary string notes for transaction.
        /// </summary>
        public async Task<SetTransactionNotes> SetTransactionNotesAsync(IEnumerable<string> txids, IEnumerable<string> notes, CancellationToken token = default)
        {
            SetTransactionNotesResponse result = await _moneroRpcCommunicator.SetTransactionNotesAsync(txids, notes, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SetTransactionNotesAsync));
            return result.Result!;
        }

        /// <summary>
        /// Get string notes for transactions of interest.
        /// </summary>
        public async Task<List<string>> GetTransactionNotesAsync(IEnumerable<string> txids, CancellationToken token = default)
        {
            GetTransactionNotesResponse result = await _moneroRpcCommunicator.GetTransactionNotesAsync(txids, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetTransactionNotesAsync));
            return result.Result!.Notes;
        }

        /// <summary>
        /// Get transaction secret key from transaction id.
        /// </summary>
        public async Task<string> GetTransactionKeyAsync(string txid, CancellationToken token = default)
        {
            GetTransactionKeyResponse result = await _moneroRpcCommunicator.GetTransactionKeyAsync(txid, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetTransactionKeyAsync));
            return result.Result!.TransactionKey;
        }

        /// <summary>
        /// Check a transaction in the blockchain with its secret key.
        /// </summary>
        public async Task<CheckTransactionKey> CheckTransactionKeyAsync(string txid, string txKey, string address, CancellationToken token = default)
        {
            CheckTransactionKeyResponse result = await _moneroRpcCommunicator.CheckTransactionKeyAsync(txid, txKey, address, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.CheckTransactionKeyAsync));
            return result.Result!;
        }

        /// <summary>
        /// Returns a list of transfers from one or more of the desired categories.
        /// </summary>
        public async Task<ShowTransfers> GetTransfersAsync(bool @in, bool @out, bool pending, bool failed, bool pool, CancellationToken token = default)
        {
            ShowTransfersResponse result = await _moneroRpcCommunicator.GetTransfersAsync(@in, @out, pending, failed, pool, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetTransfersAsync));
            return result.Result!;
        }

        /// <summary>
        /// <seealso cref="GetTransfersAsync(bool, bool, bool, bool, bool, CancellationToken)"/>
        /// </summary>
        public async Task<ShowTransfers> GetTransfersAsync(bool @in, bool @out, bool pending, bool failed, bool pool, ulong minHeight, ulong maxHeight, CancellationToken token = default)
        {
            ShowTransfersResponse result = await _moneroRpcCommunicator.GetTransfersAsync(@in, @out, pending, failed, pool, minHeight, maxHeight, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetTransfersAsync));
            return result.Result!;
        }

        /// <summary>
        /// <seealso cref="GetTransferByTxidAsync(string, uint, CancellationToken)"/>
        /// </summary>
        public async Task<Transfer> GetTransferByTxidAsync(string txid, CancellationToken token = default)
        {
            GetTransferByTxidResponse result = await _moneroRpcCommunicator.GetTransferByTxidAsync(txid, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetTransferByTxidAsync));
            return result.Result!.Transfer;
        }

        /// <summary>
        /// Show information about a transfer to/from this address.
        /// </summary>
        public async Task<Transfer> GetTransferByTxidAsync(string txid, uint accountIndex, CancellationToken token = default)
        {
            GetTransferByTxidResponse result = await _moneroRpcCommunicator.GetTransferByTxidAsync(txid, accountIndex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetTransferByTxidAsync));
            return result.Result!.Transfer;
        }

        /// <summary>
        /// Sign a string.
        /// </summary>
        public async Task<string> SignAsync(string data, CancellationToken token = default)
        {
            SignResponse result = await _moneroRpcCommunicator.SignAsync(data, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SignAsync));
            return result.Result!.Sig;
        }

        /// <summary>
        /// Verify a signature on a string.
        /// </summary>
        public Task<bool> VerifyAsync(string data, string address, string signature, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.VerifyBoolAsync(data, address, signature, token);
        }

        /// <summary>
        /// Export outputs in hex format.
        /// </summary>
        /// <returns>Output data hex.</returns>
        public async Task<string> ExportOutputsAsync(CancellationToken token = default)
        {
            ExportOutputsResponse result = await _moneroRpcCommunicator.ExportOutputsAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.ExportOutputsAsync));
            return result.Result!.OutputsDataHex;
        }

        /// <summary>
        /// Import outputs in hex format.
        /// </summary>
        /// <returns>The number of outputs imported.</returns>
        public async Task<ulong> ImportOutputsAsync(CancellationToken token = default)
        {
            ImportOutputsResponse result = await _moneroRpcCommunicator.ImportOutputsAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.ImportOutputsAsync));
            return result.Result!.NumImported;
        }

        /// <summary>
        /// Export a signed set of key images.
        /// </summary>
        public async Task<List<SignedKeyImage>> ExportKeyImagesAsync(CancellationToken token = default)
        {
            ExportKeyImagesResponse result = await _moneroRpcCommunicator.ExportKeyImagesAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.ExportKeyImagesAsync));
            return result.Result!.SignedKeyImages;
        }

        /// <summary>
        /// Import signed key images list and verify their spent status.
        /// </summary>
        public async Task<ImportKeyImages> ImportKeyImagesAsync(IEnumerable<(string KeyImage, string Signature)> signedKeyImages, CancellationToken token = default)
        {
            ImportKeyImagesResponse result = await _moneroRpcCommunicator.ImportKeyImagesAsync(signedKeyImages, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.ImportKeyImagesAsync));
            return result.Result!;
        }

        /// <summary>
        /// Create a payment URI using the official URI spec.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        /// <param name="amount">Amount to receive in atomicunits.</param>
        /// <param name="recipientName">Name of the payment recipient.</param>
        /// <param name="txDescription">Description of the reason for the tx.</param>
        /// <param name="paymentId">16 or 64 character hexadecimal payment id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Payment URI.</returns>
        public async Task<string> MakeUriAsync(string address, ulong amount, string recipientName, string? txDescription = null, string? paymentId = null, CancellationToken token = default)
        {
            MakeUriResponse result = await _moneroRpcCommunicator.MakeUriAsync(address, amount, recipientName, txDescription, paymentId, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.MakeUriAsync));
            return result.Result!.Uri;
        }

        /// <summary>
        /// Parse a payment URI to get payment information.
        /// </summary>
        public async Task<MoneroUri> ParseUriAsync(string uri, CancellationToken token = default)
        {
            ParseUriResponse result = await _moneroRpcCommunicator.ParseUriAsync(uri, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.ParseUriAsync));
            return result.Result!.Uri;
        }

        /// <summary>
        /// Retrieves entries from the address book.
        /// </summary>
        /// <param name="entries">Indices of the requested address book entries.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task<AddressBook> GetAddressBookAsync(IEnumerable<uint> entries, CancellationToken token = default)
        {
            GetAddressBookResponse result = await _moneroRpcCommunicator.GetAddressBookAsync(entries, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetAddressBookAsync));
            return result.Result!;
        }

        /// <summary>
        /// Add an entry to the address book.
        /// </summary>
        public async Task<AddAddressBook> AddAddressBookAsync(string address, string? description = null, string? paymentId = null, CancellationToken token = default)
        {
            AddAddressBookResponse result = await _moneroRpcCommunicator.AddAddressBookAsync(address, description, paymentId, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.AddAddressBookAsync));
            return result.Result!;
        }

        /// <summary>
        /// Delete an entry from the address book.
        /// </summary>
        public async Task<DeleteAddressBook> DeleteAddressBookAsync(uint index, CancellationToken token = default)
        {
            DeleteAddressBookResponse result = await _moneroRpcCommunicator.DeleteAddressBookAsync(index, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.DeleteAddressBookAsync));
            return result.Result!;
        }

        /// <summary>
        /// Refresh a wallet after opening.
        /// </summary>
        public async Task<RefreshWallet> RefreshWalletAsync(uint startHeight, CancellationToken token = default)
        {
            RefreshWalletResponse result = await _moneroRpcCommunicator.RefreshWalletAsync(startHeight, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.RefreshWalletAsync));
            return result.Result!;
        }

        /// <summary>
        /// Rescan the blockchain for spent outputs.
        /// </summary>
        public async Task<RescanSpent> RescanSpentAsync(CancellationToken token = default)
        {
            RescanSpentResponse result = await _moneroRpcCommunicator.RescanSpentAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.RescanSpentAsync));
            return result.Result!;
        }

        /// <summary>
        /// Create a new wallet. You need to have set the argument "–wallet-dir" when launching monero-wallet-rpc to make this work.
        /// </summary>
        /// <param name="filename">Wallet file name.</param>
        /// <param name="language">Language for your wallet's seed.</param>
        /// <param name="password">Password to protect the wallet.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task<CreateWallet> CreateWalletAsync(string filename, string language, string? password = null, CancellationToken token = default)
        {
            CreateWalletResponse result = await _moneroRpcCommunicator.CreateWalletAsync(filename, language, password, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.CreateWalletAsync));
            return result.Result!;
        }

        /// <summary>
        /// Open a wallet. You need to have set the argument "–wallet-dir" when launching monero-wallet-rpc to make this work.
        /// </summary>
        public async Task<OpenWallet> OpenWalletAsync(string filename, string? password = null, CancellationToken token = default)
        {
            OpenWalletResponse result = await _moneroRpcCommunicator.OpenWalletAsync(filename, password, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.OpenWalletAsync));
            return result.Result!;
        }

        /// <summary>
        /// Close the currently opened wallet, after trying to save it.
        /// Note: After you close a wallet, every subsequent query will fail.
        /// </summary>
        public async Task<CloseWallet> CloseWalletAsync(CancellationToken token = default)
        {
            CloseWalletResponse result = await _moneroRpcCommunicator.CloseWalletAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.CloseWalletAsync));
            return result.Result!;
        }

        /// <summary>
        /// Change a wallet password.
        /// </summary>
        public async Task<ChangeWalletPassword> ChangeWalletPasswordAsync(string? oldPassword = null, string? newPassword = null, CancellationToken token = default)
        {
            ChangeWalletPasswordResponse result = await _moneroRpcCommunicator.ChangeWalletPasswordAsync(oldPassword, newPassword, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.ChangeWalletPasswordAsync));
            return result.Result!;
        }

        /// <summary>
        /// Get RPC version Major &amp; Minor integer-format, where Major is the first 16 bits and Minor the last 16 bits.
        /// </summary>
        public async Task<uint> GetVersionAsync(CancellationToken token = default)
        {
            GetRpcVersionResponse result = await _moneroRpcCommunicator.GetRpcVersionAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetVersionAsync));
            return result.Result!.Version;
        }

        /// <summary>
        /// Gets the wallet RPC version as a structured <see cref="MoneroVersion"/> with major, minor, and patch components.
        /// </summary>
        public async Task<MoneroVersion> GetStructuredVersionAsync(CancellationToken token = default)
        {
            uint encoded = await GetVersionAsync(token).ConfigureAwait(false);
            return new MoneroVersion(encoded);
        }

        /// <summary>
        /// Check if a wallet is a multi-signature (multisig) one.
        /// </summary>
        public async Task<MultiSigInformation> GetMultiSigInformationAsync(CancellationToken token = default)
        {
            MultiSigInformationResponse result = await _moneroRpcCommunicator.IsMultiSigAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetMultiSigInformationAsync));
            return result.Result!;
        }

        /// <summary>
        /// Prepare a wallet for multisig by generating a multisig string to share with peers.
        /// </summary>
        /// <returns>A string representing multisig information.</returns>
        public async Task<string> PrepareMultiSigAsync(CancellationToken token = default)
        {
            PrepareMultiSigResponse result = await _moneroRpcCommunicator.PrepareMultiSigAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.PrepareMultiSigAsync));
            return result.Result!.MultiSigInformation;
        }

        /// <summary>
        /// Make a wallet multisig by importing peers multisig string.
        /// </summary>
        /// <param name="multiSigInfo">List of multisig string from peers.</param>
        /// <param name="threshold">Amount of signatures needed to sign a transfer. Must be less or equal than the amount of signature in multisig_info.</param>
        /// <param name="password">Wallet password.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task<MakeMultiSig> MakeMultiSigAsync(IEnumerable<string> multiSigInfo, uint threshold, string password, CancellationToken token = default)
        {
            MakeMultiSigResponse result = await _moneroRpcCommunicator.MakeMultiSigAsync(multiSigInfo, threshold, password, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.MakeMultiSigAsync));
            return result.Result!;
        }

        /// <summary>
        /// Export multisig info for other participants.
        /// </summary>
        /// <returns>A string representing multisig information.</returns>
        public async Task<string> ExportMultiSigInfoAsync(CancellationToken token = default)
        {
            ExportMultiSigInfoResponse result = await _moneroRpcCommunicator.ExportMultiSigInfoAsync(token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.ExportMultiSigInfoAsync));
            return result.Result!.Information;
        }

        /// <summary>
        /// Import multisig info from other participants.
        /// </summary>
        /// <param name="info"> List of multisig info in hex format from other participants.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task<ImportMultiSigInformation> ImportMultiSigInfoAsync(IEnumerable<string> info, CancellationToken token = default)
        {
            ImportMultiSigInfoResponse result = await _moneroRpcCommunicator.ImportMultiSigInfoAsync(info, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.ImportMultiSigInfoAsync));
            return result.Result!;
        }

        /// <summary>
        /// Turn this wallet into a multisig wallet, extra step for N-1/N wallets.
        /// </summary>
        /// <param name="multiSigInfo">List of multisig string from peers.</param>
        /// <param name="password">Wallet password.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The multisig wallet address.</returns>
        public async Task<string> FinalizeMultiSigAsync(IEnumerable<string> multiSigInfo, string password, CancellationToken token = default)
        {
            FinalizeMultiSigResponse result = await _moneroRpcCommunicator.FinalizeMultiSigAsync(multiSigInfo, password, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.FinalizeMultiSigAsync));
            return result.Result!.Address;
        }

        /// <summary>
        /// Sign a transaction in multisig.
        /// </summary>
        /// <param name="txDataHex">Multisig transaction in hex format, as returned by transfer under multisig_txset.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task<SignMultiSigTransaction> SignMultiSigAsync(string txDataHex, CancellationToken token = default)
        {
            SignMultiSigTransactionResponse result = await _moneroRpcCommunicator.SignMultiSigAsync(txDataHex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SignMultiSigAsync));
            return result.Result!;
        }

        /// <summary>
        /// Submit a signed multisig transaction.
        /// </summary>
        /// <param name="txDataHex">Multisig transaction in hex format, as returned by sign_multisig under tx_data_hex.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>List of transaction hashes.</returns>
        public async Task<List<string>> SubmitMultiSigAsync(string txDataHex, CancellationToken token = default)
        {
            SubmitMultiSigTransactionResponse result = await _moneroRpcCommunicator.SubmitMultiSigAsync(txDataHex, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SubmitMultiSigAsync));
            return result.Result!.TransactionHashes;
        }

        /// <summary>
        /// Parse a transfer that is either multi-signature, or unsigned from a cold-wallet.
        /// </summary>
        public async Task<List<TransferDescription>> DescribeUnsignedTransferAsync(string unsignedTxSet, CancellationToken token = default)
        {
            DescribeTransferResponse result = await _moneroRpcCommunicator.DescribeTransferAsync(unsignedTxSet, false, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.DescribeUnsignedTransferAsync));
            return result.Result!.TransferDescriptions;
        }

        /// <summary>
        /// Parse a transfer that is either multi-signature, or unsigned from a cold-wallet.
        /// </summary>
        public async Task<List<TransferDescription>> DescribeMultiSigTransferAsync(string multiSigTxSet, CancellationToken token = default)
        {
            DescribeTransferResponse result = await _moneroRpcCommunicator.DescribeTransferAsync(multiSigTxSet, true, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.DescribeMultiSigTransferAsync));
            return result.Result!.TransferDescriptions;
        }

        /// <summary>
        /// Send all dust outputs back to the wallet's, to make them easier to spend (and mix).
        /// </summary>
        public async Task<SweepSingle> SweepSingleAsync(string address, uint accountIndex, TransferPriority transactionPriority, uint ringSize, ulong unlockTime = 0, bool getTxKey = true, bool getTxHex = true, bool getTxMetadata = true, CancellationToken token = default)
        {
            SweepSingleResponse result = await _moneroRpcCommunicator.SweepSingleAsync(address, accountIndex, transactionPriority, ringSize, unlockTime, getTxKey, getTxHex, getTxMetadata, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SweepSingleAsync));
            return result.Result!;
        }

        /// <summary>
        /// Retrieve the payment details associated with a given payment id.
        /// </summary>
        public async Task<List<PaymentDetail>> GetPaymentDetailAsync(string payment_id, CancellationToken token = default)
        {
            PaymentDetailResponse result = await _moneroRpcCommunicator.GetPaymentDetailAsync(payment_id, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetPaymentDetailAsync));
            return result.Result!.Payments;
        }

        /// <summary>
        /// Retrieve payments for multiple payment ids.
        /// </summary>
        /// <param name="paymentIds">Payment ids to query.</param>
        /// <param name="minBlockHeight">Optional minimum block height filter.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Matching payment details.</returns>
        public Task<List<PaymentDetail>> GetBulkPaymentsAsync(IEnumerable<string> paymentIds, ulong? minBlockHeight = null, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.GetBulkPaymentsAsync(paymentIds, minBlockHeight, token);
        }

        /// <summary>
        /// Validate a Monero address using wallet RPC rules.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <param name="anyNetType">Whether any network type is accepted.</param>
        /// <param name="allowOpenAlias">Whether OpenAlias resolution is allowed.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Address validation result.</returns>
        public Task<ValidateAddress> ValidateAddressAsync(string address, bool anyNetType = false, bool allowOpenAlias = true, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.ValidateAddressAsync(address, anyNetType, allowOpenAlias, token);
        }

        /// <summary>
        /// Create an integrated address from a standard address and payment id.
        /// </summary>
        /// <param name="standardAddress">Optional standard address; wallet primary address is used when omitted.</param>
        /// <param name="paymentId">Optional payment id; one is generated when omitted.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The generated integrated address and payment id.</returns>
        public Task<IntegratedAddress> MakeIntegratedAddressAsync(string? standardAddress = null, string? paymentId = null, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.MakeIntegratedAddressAsync(standardAddress, paymentId, token);
        }

        /// <summary>
        /// Split an integrated address into standard address and payment id.
        /// </summary>
        /// <param name="integratedAddress">Integrated address to split.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The split address data.</returns>
        public Task<IntegratedAddress> SplitIntegratedAddressAsync(string integratedAddress, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.SplitIntegratedAddressAsync(integratedAddress, token);
        }

        /// <summary>
        /// Enable or disable automatic wallet refresh.
        /// </summary>
        /// <param name="enabled">Whether auto-refresh is enabled.</param>
        /// <param name="periodSeconds">Optional refresh period in seconds.</param>
        /// <param name="token">The cancellation token.</param>
        public Task SetAutoRefreshAsync(bool enabled, uint? periodSeconds = null, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.SetAutoRefreshAsync(enabled, periodSeconds, token);
        }

        /// <summary>
        /// Scan wallet transactions by transaction id.
        /// </summary>
        /// <param name="txIds">Transaction ids to scan.</param>
        /// <param name="token">The cancellation token.</param>
        public Task ScanTransactionsAsync(IEnumerable<string> txIds, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.ScanTransactionsAsync(txIds, token);
        }

        /// <summary>
        /// Rescan blockchain data for the current wallet.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        public Task RescanBlockchainAsync(CancellationToken token = default)
        {
            return _moneroRpcCommunicator.RescanBlockchainAsync(token);
        }

        /// <summary>
        /// Relay transaction metadata produced by transfer operations.
        /// </summary>
        /// <param name="txMetadata">Transaction metadata to relay.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The relayed transaction id.</returns>
        public Task<string> RelayTransactionMetadataAsync(string txMetadata, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.RelayTransactionMetadataAsync(txMetadata, token);
        }

        /// <summary>
        /// Start wallet-managed mining.
        /// </summary>
        /// <param name="threadsCount">Number of mining threads.</param>
        /// <param name="doBackgroundMining">Whether background mining is enabled.</param>
        /// <param name="ignoreBattery">Whether battery state is ignored.</param>
        /// <param name="token">The cancellation token.</param>
        public Task StartMiningAsync(ulong threadsCount, bool doBackgroundMining, bool ignoreBattery, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.StartWalletMiningAsync(threadsCount, doBackgroundMining, ignoreBattery, token);
        }

        /// <summary>
        /// Stop wallet-managed mining.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        public Task StopMiningAsync(CancellationToken token = default)
        {
            return _moneroRpcCommunicator.StopWalletMiningAsync(token);
        }

        /// <summary>
        /// Freeze a wallet output by key image.
        /// </summary>
        /// <param name="keyImage">Output key image.</param>
        /// <param name="token">The cancellation token.</param>
        public Task FreezeOutputAsync(string keyImage, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.FreezeOutputAsync(keyImage, token);
        }

        /// <summary>
        /// Unfreeze a previously frozen wallet output.
        /// </summary>
        /// <param name="keyImage">Output key image.</param>
        /// <param name="token">The cancellation token.</param>
        public Task ThawOutputAsync(string keyImage, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.ThawOutputAsync(keyImage, token);
        }

        /// <summary>
        /// Check whether an output is currently frozen.
        /// </summary>
        /// <param name="keyImage">Output key image.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see langword="true"/> when frozen; otherwise <see langword="false"/>.</returns>
        public Task<bool> IsOutputFrozenAsync(string keyImage, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.IsOutputFrozenAsync(keyImage, token);
        }

        /// <summary>
        /// Get the wallet's default transfer priority.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Default fee priority.</returns>
        public Task<TransferPriority> GetDefaultFeePriorityAsync(CancellationToken token = default)
        {
            return _moneroRpcCommunicator.GetDefaultFeePriorityAsync(token);
        }

        /// <summary>
        /// Generate a transaction proof signature.
        /// </summary>
        /// <param name="txid">Transaction id.</param>
        /// <param name="address">Destination address.</param>
        /// <param name="message">Optional signed message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Proof signature string.</returns>
        public Task<string> GetTransactionProofAsync(string txid, string address, string? message = null, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.GetTransactionProofAsync(txid, address, message, token);
        }

        /// <summary>
        /// Verify a transaction proof signature.
        /// </summary>
        /// <param name="txid">Transaction id.</param>
        /// <param name="address">Address used for proof generation.</param>
        /// <param name="message">Signed message.</param>
        /// <param name="signature">Proof signature.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Transaction proof verification result.</returns>
        public Task<CheckTransactionProof> CheckTransactionProofAsync(string txid, string address, string message, string signature, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.CheckTransactionProofAsync(txid, address, message, signature, token);
        }

        /// <summary>
        /// Generate a spend proof signature.
        /// </summary>
        /// <param name="txid">Transaction id.</param>
        /// <param name="message">Optional signed message.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Spend proof signature string.</returns>
        public Task<string> GetSpendProofAsync(string txid, string? message = null, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.GetSpendProofAsync(txid, message, token);
        }

        /// <summary>
        /// Verify a spend proof signature.
        /// </summary>
        /// <param name="txid">Transaction id.</param>
        /// <param name="message">Signed message.</param>
        /// <param name="signature">Spend proof signature.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see langword="true"/> when the signature is valid; otherwise <see langword="false"/>.</returns>
        public Task<bool> CheckSpendProofAsync(string txid, string message, string signature, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.CheckSpendProofAsync(txid, message, signature, token);
        }

        /// <summary>
        /// Generate a reserve proof signature.
        /// </summary>
        /// <param name="message">Signed message.</param>
        /// <param name="accountIndex">Optional account index to prove.</param>
        /// <param name="amount">Optional amount to prove.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Reserve proof signature string.</returns>
        public Task<string> GetReserveProofAsync(string message, uint? accountIndex = null, ulong? amount = null, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.GetReserveProofAsync(message, accountIndex, amount, token);
        }

        /// <summary>
        /// Verify a reserve proof signature.
        /// </summary>
        /// <param name="address">Address used for proof generation.</param>
        /// <param name="message">Signed message.</param>
        /// <param name="signature">Reserve proof signature.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Reserve proof verification result.</returns>
        public Task<CheckReserveProof> CheckReserveProofAsync(string address, string message, string signature, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.CheckReserveProofAsync(address, message, signature, token);
        }

        /// <summary>
        /// Set wallet daemon connection parameters.
        /// </summary>
        /// <param name="connection">Daemon connection settings.</param>
        /// <param name="token">The cancellation token.</param>
        public Task SetDaemonConnectionAsync(MoneroDaemonConnection connection, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.SetDaemonConnectionAsync(connection, token);
        }

        /// <summary>
        /// Exchange multisig key material with cosigners.
        /// </summary>
        /// <param name="multisigInfo">Multisig info blobs from peers.</param>
        /// <param name="password">Wallet password.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Multisig exchange result.</returns>
        public Task<ExchangeMultiSigKeys> ExchangeMultiSigKeysAsync(IEnumerable<string> multisigInfo, string password, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.ExchangeMultiSigKeysAsync(multisigInfo, password, token);
        }

        /// <summary>
        /// Estimate transaction size and weight for a transfer configuration.
        /// </summary>
        /// <param name="config">Transfer configuration to estimate.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Estimated size and weight details.</returns>
        public Task<EstimateTxSizeAndWeight> EstimateTransactionSizeAndWeightAsync(MoneroTxConfig config, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.EstimateTxSizeAndWeightAsync(config, token);
        }

        /// <summary>
        /// Initialize wallet background sync.
        /// </summary>
        /// <param name="walletPassword">Optional wallet password.</param>
        /// <param name="token">The cancellation token.</param>
        public Task SetupBackgroundSyncAsync(string? walletPassword = null, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.SetupBackgroundSyncAsync(walletPassword, token);
        }

        /// <summary>
        /// Start background synchronization.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        public Task StartBackgroundSyncAsync(CancellationToken token = default)
        {
            return _moneroRpcCommunicator.StartBackgroundSyncAsync(token);
        }

        /// <summary>
        /// Stop background synchronization.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        public Task StopBackgroundSyncAsync(CancellationToken token = default)
        {
            return _moneroRpcCommunicator.StopBackgroundSyncAsync(token);
        }

        /// <summary>
        /// Generate a wallet from explicit key material.
        /// </summary>
        /// <param name="options">Generate-from-keys options.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Created wallet details.</returns>
        public Task<GenerateFromKeysWallet> GenerateFromKeysWalletAsync(GenerateFromKeysOptions options, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.GenerateFromKeysWalletAsync(options, token);
        }

        /// <summary>
        /// Restore a deterministic wallet from seed information.
        /// </summary>
        /// <param name="options">Restore options.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Restored wallet details.</returns>
        public Task<RestoreDeterministicWallet> RestoreDeterministicWalletAsync(RestoreDeterministicWalletOptions options, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.RestoreDeterministicWalletAsync(options, token);
        }

        /// <summary>
        /// Edit an existing address-book entry.
        /// </summary>
        /// <param name="index">Address-book entry index.</param>
        /// <param name="setAddress">Whether to update the address field.</param>
        /// <param name="address">New address value when <paramref name="setAddress"/> is true.</param>
        /// <param name="setDescription">Whether to update the description field.</param>
        /// <param name="description">New description value when <paramref name="setDescription"/> is true.</param>
        /// <param name="token">The cancellation token.</param>
        public Task EditAddressBookAsync(uint index, bool setAddress, string? address, bool setDescription, string? description, CancellationToken token = default)
        {
            return _moneroRpcCommunicator.EditAddressBookAsync(index, setAddress, address, setDescription, description, token);
        }

        /// <summary>
        /// Create a wallet using fluent wallet configuration.
        /// </summary>
        /// <param name="config">Wallet creation configuration.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Created wallet details.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is <see langword="null"/>.</exception>
        public Task<CreateWallet> CreateWalletAsync(MoneroWalletConfig config, CancellationToken token = default)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return CreateWalletAsync(config.Filename, config.Language ?? "English", config.Password, token);
        }

        /// <summary>
        /// Open a wallet using fluent wallet configuration.
        /// </summary>
        /// <param name="config">Wallet open configuration.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Open wallet result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is <see langword="null"/>.</exception>
        public Task<OpenWallet> OpenWalletAsync(MoneroWalletConfig config, CancellationToken token = default)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return OpenWalletAsync(config.Filename, config.Password, token);
        }

        /// <summary>
        /// Transfer funds using fluent transaction configuration.
        /// </summary>
        /// <param name="config">Transfer configuration.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Transfer result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is <see langword="null"/>.</exception>
        public Task<FundTransfer> TransferAsync(MoneroTxConfig config, CancellationToken token = default)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return _moneroRpcCommunicator.TransferWithConfigAsync(config, token);
        }

        /// <summary>
        /// Transfer funds using split mode and fluent transaction configuration.
        /// </summary>
        /// <param name="config">Transfer split configuration.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Split transfer result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="config"/> is <see langword="null"/>.</exception>
        public Task<SplitFundTransfer> TransferSplitAsync(MoneroTxConfig config, CancellationToken token = default)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return _moneroRpcCommunicator.TransferSplitWithConfigAsync(config, token);
        }

        /// <summary>
        /// Subscribe to wallet polling notifications.
        /// </summary>
        /// <param name="listener">Callback container for polling events.</param>
        /// <param name="options">Optional polling configuration.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An async-disposable subscription handle.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="listener"/> is <see langword="null"/>.</exception>
        public Task<IAsyncDisposable> SubscribeAsync(MoneroWalletListener listener, MoneroWalletPollingOptions? options = null, CancellationToken token = default)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            MoneroWalletPollingOptions pollingOptions = options ?? new MoneroWalletPollingOptions();
            var subscription = new WalletPollingSubscription(this, listener, pollingOptions);
            lock (_subscriptionLock)
            {
                _subscriptions.Add(subscription);
            }

            return Task.FromResult<IAsyncDisposable>(subscription);
        }

        /// <summary>
        /// Set an attribute (key/value pair) to store any additional info in the wallet
        /// </summary>
        public async Task SetAttributeAsync(string key, string value, CancellationToken token = default)
        {
            SetAttributeResponse result = await _moneroRpcCommunicator.SetAttributeAsync(key, value, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.SetAttributeAsync));
        }

        /// <summary>
        /// Get an attribute value from the wallet, given its key; throws an error if not present
        /// </summary>
        public async Task<string> GetAttributeAsync(string key, CancellationToken token = default)
        {
            GetAttributeResponse result = await _moneroRpcCommunicator.GetAttributeAsync(key, token).ConfigureAwait(false);
            ErrorGuard.ThrowIfResultIsNull(result, nameof(this.GetAttributeAsync));
            return result.Result!.Value;
        }

        /// <summary>
        /// Dispose pattern hook.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            lock (_disposingLock)
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

                try
                {
                    SaveWalletAsync().GetAwaiter().GetResult();
                    CloseWalletAsync().GetAwaiter().GetResult();
                }
                catch
                {
                    // Best-effort: sync Dispose must not throw from async network calls.
                }

                _moneroRpcCommunicator.Dispose();
            }

            // Free unmanaged objects.
        }

        /// <summary>
        /// Opens the specified wallet file, completing the async factory initialization.
        /// </summary>
        /// <param name="filename">The wallet file name to open.</param>
        /// <param name="password">The wallet password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>This instance, after the wallet has been opened.</returns>
        private async Task<MoneroWalletClient> InitializeAsync(string filename, string password, CancellationToken cancellationToken)
        {
            await OpenWalletAsync(filename, password, cancellationToken).ConfigureAwait(false);
            return this;
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
        /// Represents an active wallet polling subscription.
        /// </summary>
        private sealed class WalletPollingSubscription : IAsyncDisposable
        {
            private readonly MoneroWalletClient _owner;
            private readonly MoneroWalletListener _listener;
            private readonly MoneroWalletPollingOptions _options;

            /// <summary>
            /// Cancellation source used to signal the polling loop to stop.
            /// </summary>
            private readonly CancellationTokenSource _cts = new();
            private readonly Task _runTask;

            /// <summary>
            /// Tracks the last-known spent status of each transfer key image for change detection.
            /// </summary>
            private readonly Dictionary<string, bool> _transferSpentByKeyImage = new(StringComparer.Ordinal);
            private bool _disposed;
            private ulong _lastHeight;
            private ulong _lastBalance;
            private ulong _lastUnlockedBalance;

            /// <summary>
            /// Initializes a new instance of the WalletPollingSubscription class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            /// <param name="listener">The listener.</param>
            /// <param name="options">Additional options for the operation.</param>
            public WalletPollingSubscription(MoneroWalletClient owner, MoneroWalletListener listener, MoneroWalletPollingOptions options)
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
            /// Main polling loop that takes an initial snapshot and then periodically polls for wallet state changes.
            /// </summary>
            private async Task RunAsync()
            {
                CancellationToken token = _cts.Token;
                try
                {
                    await SnapshotAsync(token).ConfigureAwait(false);
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
            /// Captures the initial wallet state (height, balance, transfers) to serve as a baseline for change detection.
            /// </summary>
            /// <param name="token">The cancellation token.</param>
            private async Task SnapshotAsync(CancellationToken token)
            {
                _lastHeight = await _owner.GetHeightAsync(token).ConfigureAwait(false);
                Balance balance = await _owner.GetBalanceAsync(0u, Array.Empty<uint>(), allAccounts: true, strict: false, token: token).ConfigureAwait(false);
                _lastBalance = balance.TotalBalance;
                _lastUnlockedBalance = balance.UnlockedBalance;

                List<IncomingTransfer> transfers = await _owner.GetIncomingTransfersAsync(TransferType.All, returnKeyImage: true, token).ConfigureAwait(false);
                _transferSpentByKeyImage.Clear();
                foreach (IncomingTransfer transfer in transfers)
                {
                    if (!string.IsNullOrWhiteSpace(transfer.KeyImage))
                    {
                        _transferSpentByKeyImage[transfer.KeyImage] = transfer.IsSpent;
                    }
                }
            }

            /// <summary>
            /// Executes a single poll iteration, detecting height changes, balance changes, new outputs, and spent outputs.
            /// </summary>
            /// <param name="token">The cancellation token.</param>
            private async Task PollOnceAsync(CancellationToken token)
            {
                try
                {
                    ulong height = await _owner.GetHeightAsync(token).ConfigureAwait(false);
                    if (height != _lastHeight && _listener.OnNewBlockAsync != null)
                    {
                        await _listener.OnNewBlockAsync(height, token).ConfigureAwait(false);
                        _lastHeight = height;
                    }

                    Balance balance = await _owner.GetBalanceAsync(0u, Array.Empty<uint>(), allAccounts: true, strict: false, token: token).ConfigureAwait(false);
                    if ((balance.TotalBalance != _lastBalance || balance.UnlockedBalance != _lastUnlockedBalance) && _listener.OnBalancesChangedAsync != null)
                    {
                        await _listener.OnBalancesChangedAsync(balance.TotalBalance, balance.UnlockedBalance, token).ConfigureAwait(false);
                        _lastBalance = balance.TotalBalance;
                        _lastUnlockedBalance = balance.UnlockedBalance;
                    }

                    List<IncomingTransfer> transfers = await _owner.GetIncomingTransfersAsync(TransferType.All, returnKeyImage: true, token).ConfigureAwait(false);
                    foreach (IncomingTransfer transfer in transfers)
                    {
                        if (string.IsNullOrWhiteSpace(transfer.KeyImage))
                        {
                            continue;
                        }

                        if (!_transferSpentByKeyImage.TryGetValue(transfer.KeyImage, out bool previousSpent))
                        {
                            _transferSpentByKeyImage[transfer.KeyImage] = transfer.IsSpent;
                            if (_listener.OnOutputReceivedAsync != null)
                            {
                                await _listener.OnOutputReceivedAsync(transfer, token).ConfigureAwait(false);
                            }

                            continue;
                        }

                        if (!previousSpent && transfer.IsSpent)
                        {
                            _transferSpentByKeyImage[transfer.KeyImage] = true;
                            if (_listener.OnOutputSpentAsync != null)
                            {
                                await _listener.OnOutputSpentAsync(transfer, token).ConfigureAwait(false);
                            }
                        }
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
}
