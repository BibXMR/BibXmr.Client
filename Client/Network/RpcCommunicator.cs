using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Daemon.Dto.Requests;
using BibXmr.Client.Daemon.Dto.Responses;
using BibXmr.Client.Network.Exceptions;
using BibXmr.Client.Utilities;
using BibXmr.Client.Wallet.Dto;
using BibXmr.Client.Wallet.Dto.Requests;
using BibXmr.Client.Wallet.Dto.Responses;

namespace BibXmr.Client.Network
{
    internal enum ConnectionType
    {
        Wallet,
        Daemon,
    }

    /// <summary>
    /// Coordinates HTTP transport, serialization, and error mapping for RPC calls.
    /// </summary>
    internal partial class RpcCommunicator
    {
        private readonly HttpClient _httpClient;
        private readonly MoneroRequestAdapter _requestAdapter;
        private readonly MoneroRpcClientOptions _options;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly bool _ownsHttpClient;

        /// <summary>
        /// Initializes a new communicator for the specified host and port using default options.
        /// </summary>
        /// <param name="url">RPC host name or IP address (for example, <c>127.0.0.1</c>).</param>
        /// <param name="port">RPC port number.</param>
        public RpcCommunicator(string url, uint port)
            : this(url, port, null)
        {
        }

        /// <summary>
        /// Initializes a new communicator for the specified host, port, and connection type.
        /// </summary>
        /// <param name="url">The RPC endpoint URL.</param>
        /// <param name="port">The RPC endpoint port number.</param>
        /// <param name="connectionType">Connection role used to select request serialization behavior.</param>
        public RpcCommunicator(string url, uint port, ConnectionType? connectionType)
        {
            var options = new MoneroRpcClientOptions();
            JsonSerializerOptions serializerOptions = CreateSerializerOptions(connectionType, options);
            var adapter = new MoneroRequestAdapter(
                url,
                port,
                options.RequestIdFactory,
                serializerOptions,
                options.UnsafeAllowClearTextHttpOnNonLoopback);
            var httpClient = new HttpClient();
            _httpClient = httpClient;
            _ownsHttpClient = true;
            _requestAdapter = adapter;
            _options = options;
            _serializerOptions = serializerOptions;

            // Apply our desired semantics to our owned client.
            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        /// <summary>
        /// Initializes a new communicator using built-in network defaults for the given connection type.
        /// </summary>
        /// <param name="networkType">The target Monero network.</param>
        /// <param name="connectionType">Connection role used to select daemon or wallet defaults.</param>
        public RpcCommunicator(MoneroNetwork networkType, ConnectionType connectionType)
        {
            var options = new MoneroRpcClientOptions();
            JsonSerializerOptions serializerOptions = CreateSerializerOptions(connectionType, options);
            MoneroRequestAdapter adapter = (connectionType, networkType) switch
            {
                (ConnectionType.Daemon, MoneroNetwork.Mainnet) => new MoneroRequestAdapter(
                    MoneroNetworkDefaults.DaemonMainnetUrl,
                    MoneroNetworkDefaults.DaemonMainnetPort,
                    options.RequestIdFactory,
                    serializerOptions,
                    options.UnsafeAllowClearTextHttpOnNonLoopback),
                (ConnectionType.Daemon, MoneroNetwork.Stagenet) => new MoneroRequestAdapter(
                    MoneroNetworkDefaults.DaemonStagenetUrl,
                    MoneroNetworkDefaults.DaemonStagenetPort,
                    options.RequestIdFactory,
                    serializerOptions,
                    options.UnsafeAllowClearTextHttpOnNonLoopback),
                (ConnectionType.Daemon, MoneroNetwork.Testnet) => new MoneroRequestAdapter(
                    MoneroNetworkDefaults.DaemonTestnetUrl,
                    MoneroNetworkDefaults.DaemonTestnetPort,
                    options.RequestIdFactory,
                    serializerOptions,
                    options.UnsafeAllowClearTextHttpOnNonLoopback),
                (ConnectionType.Wallet, MoneroNetwork.Mainnet) => new MoneroRequestAdapter(
                    MoneroNetworkDefaults.WalletMainnetUrl,
                    MoneroNetworkDefaults.WalletMainnetPort,
                    options.RequestIdFactory,
                    serializerOptions,
                    options.UnsafeAllowClearTextHttpOnNonLoopback),
                (ConnectionType.Wallet, MoneroNetwork.Stagenet) => new MoneroRequestAdapter(
                    MoneroNetworkDefaults.WalletStagenetUrl,
                    MoneroNetworkDefaults.WalletStagenetPort,
                    options.RequestIdFactory,
                    serializerOptions,
                    options.UnsafeAllowClearTextHttpOnNonLoopback),
                (ConnectionType.Wallet, MoneroNetwork.Testnet) => new MoneroRequestAdapter(
                    MoneroNetworkDefaults.WalletTestnetUrl,
                    MoneroNetworkDefaults.WalletTestnetPort,
                    options.RequestIdFactory,
                    serializerOptions,
                    options.UnsafeAllowClearTextHttpOnNonLoopback),
                (_, _) => throw new InvalidOperationException($"Unknown MoneroNetwork ({networkType}) and ConnectionType ({connectionType}) combination"),
            };

            var httpClient = new HttpClient();

            _httpClient = httpClient;
            _ownsHttpClient = true;
            _requestAdapter = adapter;
            _options = options;
            _serializerOptions = serializerOptions;

            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        /// <summary>
        /// Initializes a new instance of the RpcCommunicator class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send RPC requests.</param>
        /// <param name="baseAddress">The base address of the RPC endpoint.</param>
        /// <param name="options">Additional options for the operation.</param>
        public RpcCommunicator(HttpClient httpClient, Uri baseAddress, MoneroRpcClientOptions? options = null)
            : this(httpClient, baseAddress, null, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RpcCommunicator class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send RPC requests.</param>
        /// <param name="baseAddress">The base address of the RPC endpoint.</param>
        /// <param name="connectionType">The connection type.</param>
        /// <param name="options">Additional options for the operation.</param>
        public RpcCommunicator(HttpClient httpClient, Uri baseAddress, ConnectionType? connectionType, MoneroRpcClientOptions? options = null)
        {
            MoneroRpcClientOptions effectiveOptions = options ?? new MoneroRpcClientOptions();
            JsonSerializerOptions serializerOptions = CreateSerializerOptions(connectionType, effectiveOptions);
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ownsHttpClient = false;
            _requestAdapter = new MoneroRequestAdapter(
                baseAddress,
                effectiveOptions.RequestIdFactory,
                serializerOptions,
                effectiveOptions.UnsafeAllowClearTextHttpOnNonLoopback);
            _options = effectiveOptions;
            _serializerOptions = serializerOptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RpcCommunicator"/> class, delegating to the full constructor with default serializer options.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send RPC requests.</param>
        /// <param name="ownsHttpClient">Whether this instance owns and should dispose the HTTP client.</param>
        /// <param name="requestAdapter">The adapter that builds HTTP request messages.</param>
        /// <param name="options">RPC client options controlling timeout, transport policy, and serialization.</param>
        private RpcCommunicator(HttpClient httpClient, bool ownsHttpClient, MoneroRequestAdapter requestAdapter, MoneroRpcClientOptions options)
            : this(httpClient, ownsHttpClient, requestAdapter, options, MoneroJson.CreateSerializerOptions(MoneroJsonProfile.Combined, options))
        {
        }

        /// <summary>
        /// Core private constructor that stores all injected dependencies.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send RPC requests.</param>
        /// <param name="ownsHttpClient">Whether this instance owns and should dispose the HTTP client.</param>
        /// <param name="requestAdapter">The adapter that builds HTTP request messages.</param>
        /// <param name="options">RPC client options controlling timeout, transport policy, and serialization.</param>
        /// <param name="serializerOptions">Pre-configured JSON serializer options.</param>
        private RpcCommunicator(
            HttpClient httpClient,
            bool ownsHttpClient,
            MoneroRequestAdapter requestAdapter,
            MoneroRpcClientOptions options,
            JsonSerializerOptions serializerOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ownsHttpClient = ownsHttpClient;
            _requestAdapter = requestAdapter ?? throw new ArgumentNullException(nameof(requestAdapter));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));

            if (_ownsHttpClient)
            {
                // Use our own cancellation/timeout semantics (linked CTS) rather than HttpClient.Timeout.
                _httpClient.Timeout = Timeout.InfiniteTimeSpan;
            }
        }

        /// <summary>
        /// Executes the execute raw json rpc async operation.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="paramsJson">The params json.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<RawJsonRpcExecutionResult> ExecuteRawJsonRpcAsync(string method, string? paramsJson = null, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentException("RPC method is required.", nameof(method));
            }

            string normalizedMethod = method.Trim();
            JsonElement normalizedParamsElement = NormalizeRawParams(paramsJson);
            string normalizedParamsJson = PrettyPrintIfJson(normalizedParamsElement.GetRawText());

            using HttpRequestMessage request = _requestAdapter.CreateJsonRpcRequestMessage(normalizedMethod, normalizedParamsElement);
            HttpContent requestContent = request.Content
                ?? throw new MoneroRpcProtocolException("JSON-RPC request did not contain a content payload.");
            string requestJson = PrettyPrintIfJson(await ReadContentAsStringAsync(requestContent, token).ConfigureAwait(false));

            using HttpResponseMessage response = await SendAsync(request, token).ConfigureAwait(false);
            string rawResponseJson = response.Content is null
                ? string.Empty
                : await ReadContentAsStringAsync(response.Content, token).ConfigureAwait(false);
            string normalizedResponseJson = PrettyPrintIfJson(rawResponseJson);

            bool isSuccess = response.IsSuccessStatusCode;
            int? errorCode = null;
            string? errorMessage = null;
            if (response.IsSuccessStatusCode)
            {
                TryExtractJsonRpcError(normalizedResponseJson, ref isSuccess, out errorCode, out errorMessage);
            }

            if (!response.IsSuccessStatusCode)
            {
                errorMessage ??= $"HTTP {(int)response.StatusCode} ({response.StatusCode})";
            }

            return new RawJsonRpcExecutionResult(
                isSuccess,
                normalizedMethod,
                normalizedParamsJson,
                requestJson,
                normalizedResponseJson,
                errorCode,
                errorMessage);
        }

        /// <summary>
        /// Retrieves balance async.
        /// </summary>
        /// <param name="account_index">The account index.</param>
        /// <param name="address_indices">The address indices.</param>
        /// <param name="all_accounts">The all accounts.</param>
        /// <param name="strict">The strict.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<BalanceResponse> GetBalanceAsync(uint account_index, IEnumerable<uint> address_indices, bool all_accounts, bool strict, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Account_index = account_index,
                Address_indices = address_indices,
                All_accounts = all_accounts,
                Strict = strict,
            };
            return GetBalanceAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Retrieves balance async.
        /// </summary>
        /// <param name="account_index">The account index.</param>
        /// <param name="address_indices">The address indices.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<BalanceResponse> GetBalanceAsync(uint account_index, IEnumerable<uint> address_indices, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Account_index = account_index,
                Address_indices = address_indices,
            };
            return GetBalanceAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Retrieves balance async.
        /// </summary>
        /// <param name="account_index">The account index.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<BalanceResponse> GetBalanceAsync(uint account_index, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Account_index = account_index,
            };
            return GetBalanceAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Retrieves address async.
        /// </summary>
        /// <param name="account_index">The account index.</param>
        /// <param name="address_indices">The address indices.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AddressResponse> GetAddressAsync(uint account_index, IEnumerable<uint> address_indices, CancellationToken token = default)
        {
            var walletParameters = new GenericRequestParameters()
            {
                Account_index = account_index,
                Address_indices = address_indices,
            };
            AddressResponse responseObject = await SendJsonRpcCommandAsync<AddressResponse>("get_address", walletParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves address async.
        /// </summary>
        /// <param name="account_index">The account index.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AddressResponse> GetAddressAsync(uint account_index, CancellationToken token = default)
        {
            AddressResponse responseObject = await SendJsonRpcCommandAsync<AddressResponse>("get_address", new GenericRequestParameters() { Account_index = account_index, }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves address index async.
        /// </summary>
        /// <param name="address">The target Monero address.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AddressIndexResponse> GetAddressIndexAsync(string address, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(address, nameof(address));
            AddressIndexResponse responseObject = await SendJsonRpcCommandAsync<AddressIndexResponse>("get_address_index", new GenericRequestParameters() { Address = address, }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Creates address async.
        /// </summary>
        /// <param name="account_index">The account index.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AddressCreationResponse> CreateAddressAsync(uint account_index, CancellationToken token = default)
        {
            AddressCreationResponse responseObject = await SendJsonRpcCommandAsync<AddressCreationResponse>("create_address", new GenericRequestParameters() { Account_index = account_index, }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Creates address async.
        /// </summary>
        /// <param name="account_index">The account index.</param>
        /// <param name="label">The label.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AddressCreationResponse> CreateAddressAsync(uint account_index, string label, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(label, nameof(label));
            var genericRequestParameters = new GenericRequestParameters()
            {
                Account_index = account_index,
                Label = label,
            };
            AddressCreationResponse responseObject = await SendJsonRpcCommandAsync<AddressCreationResponse>("create_address", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the label address async operation.
        /// </summary>
        /// <param name="major_index">The major index.</param>
        /// <param name="minor_index">The minor index.</param>
        /// <param name="label">The label.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AddressLabelResponse> LabelAddressAsync(uint major_index, uint minor_index, string label, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(label, nameof(label));
            var genericRequestParameters = new GenericRequestParameters()
            {
                Index = new AddressIndexParameter()
                {
                    Major = major_index,
                    Minor = minor_index,
                },
            };
            AddressLabelResponse responseObject = await SendJsonRpcCommandAsync<AddressLabelResponse>("label_address", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves accounts async.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AccountResponse> GetAccountsAsync(string tag, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(tag, nameof(tag));
            var genericRequestParameters = new GenericRequestParameters()
            {
                Label = tag,
            };
            AccountResponse responseObject = await SendJsonRpcCommandAsync<AccountResponse>("get_accounts", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves accounts async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AccountResponse> GetAccountsAsync(CancellationToken token = default)
        {
            AccountResponse responseObject = await SendJsonRpcCommandAsync<AccountResponse>("get_accounts", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Creates account async.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<CreateAccountResponse> CreateAccountAsync(string label, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(label, nameof(label));
            CreateAccountResponse responseObject = await SendJsonRpcCommandAsync<CreateAccountResponse>("create_account", new GenericRequestParameters() { Label = label, }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Creates account async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<CreateAccountResponse> CreateAccountAsync(CancellationToken token = default)
        {
            CreateAccountResponse responseObject = await SendJsonRpcCommandAsync<CreateAccountResponse>("create_account", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the label account async operation.
        /// </summary>
        /// <param name="account_index">The account index.</param>
        /// <param name="label">The label.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AccountLabelResponse> LabelAccountAsync(uint account_index, string label, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(label, nameof(label));
            var genericRequestParameters = new GenericRequestParameters()
            {
                Label = label,
                Account_index = account_index,
            };
            AccountLabelResponse responseObject = await SendJsonRpcCommandAsync<AccountLabelResponse>("label_account", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves account tags async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AccountTagsResponse> GetAccountTagsAsync(CancellationToken token = default)
        {
            AccountTagsResponse responseObject = await SendJsonRpcCommandAsync<AccountTagsResponse>("get_account_tags", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the tag accounts async operation.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="accounts">The accounts.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<TagAccountsResponse> TagAccountsAsync(string tag, IEnumerable<uint> accounts, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(tag, nameof(tag));
            if (accounts == null || !accounts.Any())
            {
                throw new InvalidOperationException("Accounts is either null or empty");
            }

            var genericRequestParameters = new GenericRequestParameters()
            {
                Tag = tag,
                Accounts = accounts,
            };
            TagAccountsResponse responseObject = await SendJsonRpcCommandAsync<TagAccountsResponse>("tag_accounts", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the untag accounts async operation.
        /// </summary>
        /// <param name="accounts">The accounts.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<UntagAccountsResponse> UntagAccountsAsync(IEnumerable<uint> accounts, CancellationToken token = default)
        {
            if (accounts == null || !accounts.Any())
            {
                throw new InvalidOperationException("Accounts is either null or empty");
            }

            var genericRequestParameters = new GenericRequestParameters()
            {
                Accounts = accounts,
            };
            UntagAccountsResponse responseObject = await SendJsonRpcCommandAsync<UntagAccountsResponse>("untag_accounts", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Sets account tag description async.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="description">The description.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AccountTagAndDescriptionResponse> SetAccountTagDescriptionAsync(string tag, string description, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(tag, nameof(tag));
            ErrorGuard.ThrowIfNullOrWhiteSpace(description, nameof(description));
            var genericRequestParameters = new GenericRequestParameters()
            {
                Tag = tag,
                Description = description,
            };
            AccountTagAndDescriptionResponse responseObject = await SendJsonRpcCommandAsync<AccountTagAndDescriptionResponse>("set_account_tag_description", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves height async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<BlockchainHeightResponse> GetHeightAsync(CancellationToken token = default)
        {
            BlockchainHeightResponse responseObject = await SendJsonRpcCommandAsync<BlockchainHeightResponse>("get_height", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the transfer async operation.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="transfer_priority">The transfer priority.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<FundTransferResponse> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transfer_priority, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Destinations = TransactionToFundTransferParameter(transactions),
                Priority = (uint)transfer_priority,
            };
            return TransferFundsAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Executes the transfer async operation.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="transfer_priority">The transfer priority.</param>
        /// <param name="get_tx_key">The get tx key.</param>
        /// <param name="get_tx_hex">The get tx hex.</param>
        /// <param name="unlock_time">The unlock time.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<FundTransferResponse> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transfer_priority, bool get_tx_key, bool get_tx_hex, ulong unlock_time = 0, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Destinations = TransactionToFundTransferParameter(transactions),
                Priority = (uint)transfer_priority,
                Unlock_time = unlock_time,
                Get_tx_key = get_tx_key,
                Get_tx_hex = get_tx_hex,
            };
            return TransferFundsAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Executes the transfer async operation.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="transfer_priority">The transfer priority.</param>
        /// <param name="ring_size">The ring size.</param>
        /// <param name="unlock_time">The unlock time.</param>
        /// <param name="get_tx_key">The get tx key.</param>
        /// <param name="get_tx_hex">The get tx hex.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<FundTransferResponse> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transfer_priority, uint ring_size, ulong unlock_time = 0, bool get_tx_key = true, bool get_tx_hex = true, CancellationToken token = default)
        {
            if (ring_size <= 1)
            {
                throw new InvalidOperationException($"ring_size must be at least 2");
            }

            var genericRequestParameters = new GenericRequestParameters()
            {
                Destinations = TransactionToFundTransferParameter(transactions),
                Priority = (uint)transfer_priority,
                Unlock_time = unlock_time,
                Get_tx_key = get_tx_key,
                Get_tx_hex = get_tx_hex,
                Ring_size = ring_size,
                Mixin = ring_size - 1,
            };
            return TransferFundsAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Executes the transfer async operation.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="transfer_priority">The transfer priority.</param>
        /// <param name="ring_size">The ring size.</param>
        /// <param name="account_index">The account index.</param>
        /// <param name="unlock_time">The unlock time.</param>
        /// <param name="get_tx_key">The get tx key.</param>
        /// <param name="get_tx_hex">The get tx hex.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<FundTransferResponse> TransferAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transfer_priority, uint ring_size, uint account_index, ulong unlock_time = 0, bool get_tx_key = true, bool get_tx_hex = true, CancellationToken token = default)
        {
            if (ring_size <= 1)
            {
                throw new InvalidOperationException($"ring_size must be at least 2");
            }

            var genericRequestParameters = new GenericRequestParameters()
            {
                Destinations = TransactionToFundTransferParameter(transactions),
                Priority = (uint)transfer_priority,
                Unlock_time = unlock_time,
                Get_tx_key = get_tx_key,
                Get_tx_hex = get_tx_hex,
                Ring_size = ring_size,
                Mixin = ring_size - 1,
                Account_index = account_index,
            };
            return TransferFundsAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Executes the transfer split async operation.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="transfer_priority">The transfer priority.</param>
        /// <param name="new_algorithm">The new algorithm.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<SplitFundTransferResponse> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transfer_priority, bool new_algorithm = true, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Destinations = TransactionToFundTransferParameter(transactions),
                Priority = (uint)transfer_priority,
            };
            return TransferSplitFundsAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Executes the transfer split async operation.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="transfer_priority">The transfer priority.</param>
        /// <param name="get_tx_key">The get tx key.</param>
        /// <param name="get_tx_hex">The get tx hex.</param>
        /// <param name="new_algorithm">The new algorithm.</param>
        /// <param name="unlock_time">The unlock time.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<SplitFundTransferResponse> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transfer_priority, bool get_tx_key, bool get_tx_hex, bool new_algorithm = true, ulong unlock_time = 0, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Destinations = TransactionToFundTransferParameter(transactions),
                Priority = (uint)transfer_priority,
                Get_tx_key = get_tx_key,
                Get_tx_hex = get_tx_hex,
                New_algorithm = new_algorithm,
                Unlock_time = unlock_time,
            };
            return TransferSplitFundsAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Executes the transfer split async operation.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="transfer_priority">The transfer priority.</param>
        /// <param name="ring_size">The ring size.</param>
        /// <param name="new_algorithm">The new algorithm.</param>
        /// <param name="unlock_time">The unlock time.</param>
        /// <param name="get_tx_key">The get tx key.</param>
        /// <param name="get_tx_hex">The get tx hex.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<SplitFundTransferResponse> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transfer_priority, uint ring_size, bool new_algorithm = true, ulong unlock_time = 0, bool get_tx_key = true, bool get_tx_hex = true, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Destinations = TransactionToFundTransferParameter(transactions),
                Priority = (uint)transfer_priority,
                Ring_size = ring_size,
                Get_tx_key = get_tx_key,
                Get_tx_hex = get_tx_hex,
                New_algorithm = new_algorithm,
                Unlock_time = unlock_time,
            };
            return TransferSplitFundsAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Executes the transfer split async operation.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="transfer_priority">The transfer priority.</param>
        /// <param name="ring_size">The ring size.</param>
        /// <param name="account_index">The account index.</param>
        /// <param name="new_algorithm">The new algorithm.</param>
        /// <param name="unlock_time">The unlock time.</param>
        /// <param name="get_tx_key">The get tx key.</param>
        /// <param name="get_tx_hex">The get tx hex.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<SplitFundTransferResponse> TransferSplitAsync(IEnumerable<(string Address, ulong Amount)> transactions, TransferPriority transfer_priority, uint ring_size, uint account_index, bool new_algorithm = true, ulong unlock_time = 0, bool get_tx_key = true, bool get_tx_hex = true, CancellationToken token = default)
        {
            if (ring_size <= 1)
            {
                throw new InvalidOperationException($"ring_size must be at least 2");
            }

            var genericRequestParameters = new GenericRequestParameters()
            {
                Destinations = TransactionToFundTransferParameter(transactions),
                Priority = (uint)transfer_priority,
                Ring_size = ring_size,
                Mixin = ring_size - 1,
                Account_index = account_index,
                Get_tx_key = get_tx_key,
                Get_tx_hex = get_tx_hex,
                New_algorithm = new_algorithm,
                Unlock_time = unlock_time,
            };
            return TransferSplitFundsAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Executes the sign transfer async operation.
        /// </summary>
        /// <param name="unsigned_txset">The unsigned txset.</param>
        /// <param name="export_raw">The export raw.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SignTransferResponse> SignTransferAsync(string unsigned_txset, bool export_raw = false, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Unsigned_txset = unsigned_txset,
                Export_raw = export_raw,
            };
            SignTransferResponse responseObject = await SendJsonRpcCommandAsync<SignTransferResponse>("sign_transfer", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the submit transfer async operation.
        /// </summary>
        /// <param name="tx_data_hex">The tx data hex.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SubmitTransferResponse> SubmitTransferAsync(string tx_data_hex, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Tx_data_hex = tx_data_hex,
            };
            SubmitTransferResponse responseObject = await SendJsonRpcCommandAsync<SubmitTransferResponse>("submit_transfer", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the sweep dust async operation.
        /// </summary>
        /// <param name="get_tx_keys">The get tx keys.</param>
        /// <param name="get_tx_hex">The get tx hex.</param>
        /// <param name="get_tx_metadata">The get tx metadata.</param>
        /// <param name="do_not_relay">The do not relay.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SweepDustResponse> SweepDustAsync(bool get_tx_keys, bool get_tx_hex, bool get_tx_metadata, bool do_not_relay = false, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Get_tx_keys = get_tx_keys,
                Get_tx_hex = get_tx_hex,
                Get_tx_metadata = get_tx_metadata,
                Do_not_relay = do_not_relay,
            };
            SweepDustResponse responseObject = await SendJsonRpcCommandAsync<SweepDustResponse>("sweep_dust", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the sweep all async operation.
        /// </summary>
        /// <param name="address">The target Monero address.</param>
        /// <param name="account_index">The account index.</param>
        /// <param name="transaction_priority">The transaction priority.</param>
        /// <param name="ring_size">The ring size.</param>
        /// <param name="unlock_time">The unlock time.</param>
        /// <param name="below_amount">The below amount.</param>
        /// <param name="get_tx_keys">The get tx keys.</param>
        /// <param name="get_tx_hex">The get tx hex.</param>
        /// <param name="get_tx_metadata">The get tx metadata.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SweepAllResponse> SweepAllAsync(string address, uint account_index, TransferPriority transaction_priority, uint ring_size, ulong unlock_time = 0, ulong below_amount = ulong.MaxValue, bool get_tx_keys = true, bool get_tx_hex = true, bool get_tx_metadata = true, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Address = address,
                Account_index = account_index,
                Priority = (uint)transaction_priority,
                Ring_size = ring_size,
                Mixin = ring_size - 1,
                Unlock_time = unlock_time,
                Get_tx_keys = get_tx_keys,
                Get_tx_hex = get_tx_hex,
                Get_tx_metadata = get_tx_metadata,
                Below_amount = below_amount,
            };
            SweepAllResponse responseObject = await SendJsonRpcCommandAsync<SweepAllResponse>("sweep_all", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the save wallet async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SaveWalletResponse> SaveWalletAsync(CancellationToken token = default)
        {
            SaveWalletResponse responseObject = await SendJsonRpcCommandAsync<SaveWalletResponse>("store", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves incoming transfers async.
        /// </summary>
        /// <param name="transfer_type">The transfer type.</param>
        /// <param name="return_key_image">The return key image.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<IncomingTransfersResponse> GetIncomingTransfersAsync(TransferType transfer_type, bool return_key_image = false, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Transfer_type = transfer_type.ToString().ToLowerInvariant(),
                Verbose = return_key_image,
            };
            return GetIncomingTransfersAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Retrieves incoming transfers async.
        /// </summary>
        /// <param name="transfer_type">The transfer type.</param>
        /// <param name="account_index">The account index.</param>
        /// <param name="return_key_image">The return key image.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<IncomingTransfersResponse> GetIncomingTransfersAsync(TransferType transfer_type, uint account_index, bool return_key_image = false, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Transfer_type = transfer_type.ToString().ToLowerInvariant(),
                Verbose = return_key_image,
                Account_index = account_index,
            };
            return GetIncomingTransfersAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Retrieves incoming transfers async.
        /// </summary>
        /// <param name="transfer_type">The transfer type.</param>
        /// <param name="account_index">The account index.</param>
        /// <param name="subaddr_indices">The subaddr indices.</param>
        /// <param name="return_key_image">The return key image.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<IncomingTransfersResponse> GetIncomingTransfersAsync(TransferType transfer_type, uint account_index, IEnumerable<uint> subaddr_indices, bool return_key_image = false, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Transfer_type = transfer_type.ToString().ToLowerInvariant(),
                Verbose = return_key_image,
                Account_index = account_index,
                Subaddr_indices = subaddr_indices,
            };
            return GetIncomingTransfersAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Retrieves private key.
        /// </summary>
        /// <param name="key_type">The key type.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<QueryKeyResponse> GetPrivateKey(KeyType key_type, CancellationToken token = default)
        {
            static string KeyTypeToString(KeyType keyType)
            {
                return keyType switch
                {
                    KeyType.Mnemonic => "mnemonic",
                    KeyType.ViewKey => "view_key",
                    KeyType.SpendKey => "spend_key",
                    _ => throw new InvalidOperationException($"Unknown KeyType ({keyType})"),
                };
            }

            var genericRequestParameters = new GenericRequestParameters()
            {
                Key_type = KeyTypeToString(key_type),
            };
            QueryKeyResponse responseObject = await SendJsonRpcCommandAsync<QueryKeyResponse>("query_key", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the stop wallet async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<StopWalletResponse> StopWalletAsync(CancellationToken token = default)
        {
            StopWalletResponse responseObject = await SendJsonRpcCommandAsync<StopWalletResponse>("stop_wallet", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Sets transaction notes async.
        /// </summary>
        /// <param name="txids">The txids.</param>
        /// <param name="notes">The notes.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SetTransactionNotesResponse> SetTransactionNotesAsync(IEnumerable<string> txids, IEnumerable<string> notes, CancellationToken token = default)
        {
            if (txids == null || !txids.Any())
            {
                throw new InvalidOperationException("txids is either null or empty");
            }

            if (notes == null || !notes.Any())
            {
                throw new InvalidOperationException("notes is either null or empty");
            }

            var genericRequestParameters = new GenericRequestParameters()
            {
                Txids = txids,
                Notes = notes,
            };
            SetTransactionNotesResponse responseObject = await SendJsonRpcCommandAsync<SetTransactionNotesResponse>("set_tx_notes", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves transaction notes async.
        /// </summary>
        /// <param name="txids">The txids.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<GetTransactionNotesResponse> GetTransactionNotesAsync(IEnumerable<string> txids, CancellationToken token = default)
        {
            if (txids == null || !txids.Any())
            {
                throw new InvalidOperationException("txids is either null or empty");
            }

            var genericRequestParameters = new GenericRequestParameters()
            {
                Txids = txids,
            };
            GetTransactionNotesResponse responseObject = await SendJsonRpcCommandAsync<GetTransactionNotesResponse>("get_tx_notes", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves transaction key async.
        /// </summary>
        /// <param name="txid">The txid.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<GetTransactionKeyResponse> GetTransactionKeyAsync(string txid, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(txid, nameof(txid));
            var genericRequestParameters = new GenericRequestParameters()
            {
                Txid = txid,
            };
            GetTransactionKeyResponse responseObject = await SendJsonRpcCommandAsync<GetTransactionKeyResponse>("get_tx_key", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the check transaction key async operation.
        /// </summary>
        /// <param name="txid">The txid.</param>
        /// <param name="tx_key">The tx key.</param>
        /// <param name="address">The target Monero address.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<CheckTransactionKeyResponse> CheckTransactionKeyAsync(string txid, string tx_key, string address, CancellationToken token = default)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(txid, nameof(txid));
            ErrorGuard.ThrowIfNullOrWhiteSpace(tx_key, nameof(tx_key));
            ErrorGuard.ThrowIfNullOrWhiteSpace(address, nameof(address));
            var genericRequestParameters = new GenericRequestParameters
            {
                Txid = txid,
                Transaction_key = tx_key,
                Address = address,
            };
            CheckTransactionKeyResponse responseObject = await SendJsonRpcCommandAsync<CheckTransactionKeyResponse>("check_tx_key", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves transfers async.
        /// </summary>
        /// <param name="in">The in.</param>
        /// <param name="out">The out.</param>
        /// <param name="pending">The pending.</param>
        /// <param name="failed">The failed.</param>
        /// <param name="pool">The pool.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<ShowTransfersResponse> GetTransfersAsync(bool @in, bool @out, bool pending, bool failed, bool pool, CancellationToken token = default)
        {
            bool isValidRequest = false;
            isValidRequest = @in | @out | pending | failed | pool;
            if (!isValidRequest)
            {
                throw new InvalidOperationException("Not requesting to view any form of transfer");
            }

            var genericRequestParameters = new GenericRequestParameters()
            {
                In = @in,
                Out = @out,
                Pending = pending,
                Failed = failed,
                Pool = pool,
            };
            return GetTransfersAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Retrieves transfers async.
        /// </summary>
        /// <param name="in">The in.</param>
        /// <param name="out">The out.</param>
        /// <param name="pending">The pending.</param>
        /// <param name="failed">The failed.</param>
        /// <param name="pool">The pool.</param>
        /// <param name="min_height">The min height.</param>
        /// <param name="max_height">The max height.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<ShowTransfersResponse> GetTransfersAsync(bool @in, bool @out, bool pending, bool failed, bool pool, ulong min_height, ulong max_height, CancellationToken token = default)
        {
            bool isValidRequest = false;
            isValidRequest = @in | @out | pending | failed | pool;
            if (!isValidRequest)
            {
                throw new InvalidOperationException("Not requesting to view any form of transfer");
            }

            if (max_height < min_height)
            {
                throw new InvalidOperationException($"max_height ({max_height}) cannot be less than min_height({min_height})");
            }

            var genericRequestParameters = new GenericRequestParameters()
            {
                In = @in,
                Out = @out,
                Pending = pending,
                Failed = failed,
                Pool = pool,
                Min_height = min_height,
                Max_height = max_height,
                Filter_by_height = true,
            };
            return GetTransfersAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Retrieves transfer by txid async.
        /// </summary>
        /// <param name="txid">The txid.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<GetTransferByTxidResponse> GetTransferByTxidAsync(string txid, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Txid = txid,
            };
            return GetTransferByTxidAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Retrieves transfer by txid async.
        /// </summary>
        /// <param name="txid">The txid.</param>
        /// <param name="account_index">The account index.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<GetTransferByTxidResponse> GetTransferByTxidAsync(string txid, uint account_index, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Txid = txid,
                Account_index = account_index,
            };
            return GetTransferByTxidAsync(genericRequestParameters, token);
        }

        /// <summary>
        /// Executes the sign async operation.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SignResponse> SignAsync(string data, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Data = data,
            };
            SignResponse responseObject = await SendJsonRpcCommandAsync<SignResponse>("sign", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the verify async operation.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="address">The target Monero address.</param>
        /// <param name="signature">The signature.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<VerifyResponse> VerifyAsync(string data, string address, string signature, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Data = data,
                Address = address,
                Signature = signature,
            };
            VerifyResponse responseObject = await SendJsonRpcCommandAsync<VerifyResponse>("verify", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the export outputs async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ExportOutputsResponse> ExportOutputsAsync(CancellationToken token = default)
        {
            ExportOutputsResponse responseObject = await SendJsonRpcCommandAsync<ExportOutputsResponse>("export_outputs", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the import outputs async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ImportOutputsResponse> ImportOutputsAsync(CancellationToken token = default)
        {
            ImportOutputsResponse responseObject = await SendJsonRpcCommandAsync<ImportOutputsResponse>("import_outputs", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the export key images async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ExportKeyImagesResponse> ExportKeyImagesAsync(CancellationToken token = default)
        {
            ExportKeyImagesResponse responseObject = await SendJsonRpcCommandAsync<ExportKeyImagesResponse>("export_key_images", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the import key images async operation.
        /// </summary>
        /// <param name="signed_key_images">The signed key images.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ImportKeyImagesResponse> ImportKeyImagesAsync(IEnumerable<(string KeyImage, string Signature)> signed_key_images, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Signed_key_images = KeyImageAndSignatureToSignedKeyImages(signed_key_images),
            };
            ImportKeyImagesResponse responseObject = await SendJsonRpcCommandAsync<ImportKeyImagesResponse>("import_key_images", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the make uri async operation.
        /// </summary>
        /// <param name="address">The target Monero address.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="recipient_name">The recipient name.</param>
        /// <param name="tx_description">The tx description.</param>
        /// <param name="payment_id">The payment id.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<MakeUriResponse> MakeUriAsync(string address, ulong amount, string recipient_name, string? tx_description = null, string? payment_id = null, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Address = address,
                Amount = amount,
                Recipient_name = recipient_name,
                Tx_description = tx_description,
                Payment_id = payment_id,
            };
            MakeUriResponse responseObject = await SendJsonRpcCommandAsync<MakeUriResponse>("make_uri", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the parse uri async operation.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ParseUriResponse> ParseUriAsync(string uri, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Uri = uri,
            };
            ParseUriResponse responseObject = await SendJsonRpcCommandAsync<ParseUriResponse>("parse_uri", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves address book async.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<GetAddressBookResponse> GetAddressBookAsync(IEnumerable<uint> entries, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Entries = entries,
            };
            GetAddressBookResponse responseObject = await SendJsonRpcCommandAsync<GetAddressBookResponse>("get_address_book", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Adds address book async.
        /// </summary>
        /// <param name="address">The target Monero address.</param>
        /// <param name="description">The description.</param>
        /// <param name="payment_id">The payment id.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AddAddressBookResponse> AddAddressBookAsync(string address, string? description = null, string? payment_id = null, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Address = address,
                Description = description,
                Payment_id = payment_id,
            };
            AddAddressBookResponse responseObject = await SendJsonRpcCommandAsync<AddAddressBookResponse>("add_address_book", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the delete address book async operation.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<DeleteAddressBookResponse> DeleteAddressBookAsync(uint index, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters
            {
                Index = index,
            };
            DeleteAddressBookResponse responseObject = await SendJsonRpcCommandAsync<DeleteAddressBookResponse>("delete_address_book", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the refresh wallet async operation.
        /// </summary>
        /// <param name="start_height">The start height.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<RefreshWalletResponse> RefreshWalletAsync(uint start_height, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Start_height = start_height,
            };
            RefreshWalletResponse responseObject = await SendJsonRpcCommandAsync<RefreshWalletResponse>("refresh", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the rescan spent async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<RescanSpentResponse> RescanSpentAsync(CancellationToken token = default)
        {
            RescanSpentResponse responseObject = await SendJsonRpcCommandAsync<RescanSpentResponse>("rescan_spent", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Creates wallet async.
        /// </summary>
        /// <param name="filename">The wallet file name.</param>
        /// <param name="language">The language.</param>
        /// <param name="password">The wallet password.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<CreateWalletResponse> CreateWalletAsync(string filename, string language, string? password = null, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Filename = filename,
                Language = language,
                Password = password,
            };
            CreateWalletResponse responseObject = await SendJsonRpcCommandAsync<CreateWalletResponse>("create_wallet", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Opens wallet async.
        /// </summary>
        /// <param name="filename">The wallet file name.</param>
        /// <param name="password">The wallet password.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<OpenWalletResponse> OpenWalletAsync(string filename, string? password = null, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Filename = filename,
                Password = password,
            };
            OpenWalletResponse responseObject = await SendJsonRpcCommandAsync<OpenWalletResponse>("open_wallet", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Closes wallet async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<CloseWalletResponse> CloseWalletAsync(CancellationToken token = default)
        {
            CloseWalletResponse responseObject = await SendJsonRpcCommandAsync<CloseWalletResponse>("close_wallet", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the change wallet password async operation.
        /// </summary>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ChangeWalletPasswordResponse> ChangeWalletPasswordAsync(string? oldPassword = null, string? newPassword = null, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Old_password = oldPassword,
                New_password = newPassword,
            };
            ChangeWalletPasswordResponse responseObject = await SendJsonRpcCommandAsync<ChangeWalletPasswordResponse>("change_wallet_password", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves rpc version async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<GetRpcVersionResponse> GetRpcVersionAsync(CancellationToken token = default)
        {
            GetRpcVersionResponse responseObject = await SendJsonRpcCommandAsync<GetRpcVersionResponse>("get_version", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the is multi sig async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<MultiSigInformationResponse> IsMultiSigAsync(CancellationToken token = default)
        {
            MultiSigInformationResponse responseObject = await SendJsonRpcCommandAsync<MultiSigInformationResponse>("is_multisig", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the prepare multi sig async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<PrepareMultiSigResponse> PrepareMultiSigAsync(CancellationToken token = default)
        {
            PrepareMultiSigResponse responseObject = await SendJsonRpcCommandAsync<PrepareMultiSigResponse>("prepare_multisig", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the make multi sig async operation.
        /// </summary>
        /// <param name="multisig_info">The multisig info.</param>
        /// <param name="threshold">The threshold.</param>
        /// <param name="password">The wallet password.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<MakeMultiSigResponse> MakeMultiSigAsync(IEnumerable<string> multisig_info, uint threshold, string password, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Multisig_info = multisig_info,
                Threshold = threshold,
                Password = password,
            };
            MakeMultiSigResponse responseObject = await SendJsonRpcCommandAsync<MakeMultiSigResponse>("make_multisig", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the export multi sig info async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ExportMultiSigInfoResponse> ExportMultiSigInfoAsync(CancellationToken token = default)
        {
            ExportMultiSigInfoResponse responseObject = await SendJsonRpcCommandAsync<ExportMultiSigInfoResponse>("export_multisig_info", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the import multi sig info async operation.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ImportMultiSigInfoResponse> ImportMultiSigInfoAsync(IEnumerable<string> info, CancellationToken token = default)
        {
            ImportMultiSigInfoResponse responseObject = await SendJsonRpcCommandAsync<ImportMultiSigInfoResponse>("import_multisig_info", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the finalize multi sig async operation.
        /// </summary>
        /// <param name="multisigInfo">The multisig info.</param>
        /// <param name="password">The wallet password.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<FinalizeMultiSigResponse> FinalizeMultiSigAsync(IEnumerable<string> multisigInfo, string password, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Multisig_info = multisigInfo,
                Password = password,
            };
            FinalizeMultiSigResponse responseObject = await SendJsonRpcCommandAsync<FinalizeMultiSigResponse>("finalize_multisig", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the sign multi sig async operation.
        /// </summary>
        /// <param name="tx_data_hex">The tx data hex.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SignMultiSigTransactionResponse> SignMultiSigAsync(string tx_data_hex, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Tx_data_hex = tx_data_hex,
            };
            SignMultiSigTransactionResponse responseObject = await SendJsonRpcCommandAsync<SignMultiSigTransactionResponse>("sign_multisig", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the submit multi sig async operation.
        /// </summary>
        /// <param name="txDataHex">The tx data hex.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SubmitMultiSigTransactionResponse> SubmitMultiSigAsync(string txDataHex, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Tx_data_hex = txDataHex,
            };
            SubmitMultiSigTransactionResponse responseObject = await SendJsonRpcCommandAsync<SubmitMultiSigTransactionResponse>("submit_multisig", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves block count async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<BlockCountResponse> GetBlockCountAsync(CancellationToken token)
        {
            BlockCountResponse responseObject = await SendJsonRpcCommandAsync<BlockCountResponse>("get_block_count", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves block header by hash async.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<BlockHeaderResponse> GetBlockHeaderByHashAsync(string hash, CancellationToken token)
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace(hash, nameof(hash));
            BlockHeaderResponse responseObject = await SendJsonRpcCommandAsync<BlockHeaderResponse>("get_block_header_by_hash", new GenericRequestParameters() { Hash = hash, }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves block header by height async.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<BlockHeaderResponse> GetBlockHeaderByHeightAsync(ulong height, CancellationToken token)
        {
            BlockHeaderResponse responseObject = await SendJsonRpcCommandAsync<BlockHeaderResponse>("get_block_header_by_height", new GenericRequestParameters() { Height = height, }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves block header range async.
        /// </summary>
        /// <param name="startHeight">The start height.</param>
        /// <param name="endHeight">The end height.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<BlockHeaderRangeResponse> GetBlockHeaderRangeAsync(uint startHeight, uint endHeight, CancellationToken token)
        {
            if (endHeight < startHeight)
            {
                throw new InvalidOperationException($"startHeight ({startHeight}) cannot be greater than endHeight ({endHeight})");
            }

            BlockHeaderRangeResponse responseObject = await SendJsonRpcCommandAsync<BlockHeaderRangeResponse>("get_block_headers_range", new GenericRequestParameters() { Start_height = startHeight, End_height = endHeight, }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves connections async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ConnectionResponse> GetConnectionsAsync(CancellationToken token)
        {
            ConnectionResponse responseObject = await SendJsonRpcCommandAsync<ConnectionResponse>("get_connections", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves daemon information async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<DaemonInformationResponse> GetDaemonInformationAsync(CancellationToken token)
        {
            DaemonInformationResponse responseObject = await SendJsonRpcCommandAsync<DaemonInformationResponse>("get_info", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves hardfork information async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<HardforkInformationResponse> GetHardforkInformationAsync(CancellationToken token)
        {
            HardforkInformationResponse responseObject = await SendJsonRpcCommandAsync<HardforkInformationResponse>("hard_fork_info", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves bans async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<GetBansResponse> GetBansAsync(CancellationToken token)
        {
            GetBansResponse responseObject = await SendJsonRpcCommandAsync<GetBansResponse>("get_bans", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves last block header async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<BlockHeaderResponse> GetLastBlockHeaderAsync(CancellationToken token)
        {
            BlockHeaderResponse responseObject = await SendJsonRpcCommandAsync<BlockHeaderResponse>("get_last_block_header", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the flush transaction pool async operation.
        /// </summary>
        /// <param name="txids">The txids.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<FlushTransactionPoolResponse> FlushTransactionPoolAsync(IEnumerable<string> txids, CancellationToken token)
        {
            FlushTransactionPoolResponse responseObject = await SendJsonRpcCommandAsync<FlushTransactionPoolResponse>("flush_txpool", new GenericRequestParameters() { Txids = txids, }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves output histogram async.
        /// </summary>
        /// <param name="amounts">The amounts.</param>
        /// <param name="from_height">The from height.</param>
        /// <param name="to_height">The to height.</param>
        /// <param name="cumulative">The cumulative.</param>
        /// <param name="binary">The binary.</param>
        /// <param name="compress">The compress.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<OutputHistogramResponse> GetOutputHistogramAsync(IEnumerable<ulong> amounts, ulong from_height, ulong to_height, bool cumulative, bool binary, bool compress, CancellationToken token)
        {
            if (from_height > to_height)
            {
                throw new InvalidOperationException($"from_height ({from_height}) cannot be greater than to_height ({to_height})");
            }

            var requestParameters = new GenericRequestParameters()
            {
                Amounts = amounts,
                From_height = from_height,
                To_height = to_height,
                Cumulative = cumulative,
                Binary = binary,
                Compress = compress,
            };

            OutputHistogramResponse responseObject = await SendJsonRpcCommandAsync<OutputHistogramResponse>("get_output_histogram", requestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves coinbase transaction sum async.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="count">The count.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<CoinbaseTransactionSumResponse> GetCoinbaseTransactionSumAsync(ulong height, uint count, CancellationToken token)
        {
            CoinbaseTransactionSumResponse responseObject = await SendJsonRpcCommandAsync<CoinbaseTransactionSumResponse>("get_coinbase_tx_sum", new GenericRequestParameters() { Height = height, Count = count, }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves daemon version async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<VersionResponse> GetDaemonVersionAsync(CancellationToken token)
        {
            VersionResponse responseObject = await SendJsonRpcCommandAsync<VersionResponse>("get_version", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves fee estimate async.
        /// </summary>
        /// <param name="grace_blocks">The grace blocks.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<FeeEstimateResponse> GetFeeEstimateAsync(uint grace_blocks, CancellationToken token)
        {
            FeeEstimateResponse responseObject = await SendJsonRpcCommandAsync<FeeEstimateResponse>("get_fee_estimate", new GenericRequestParameters() { Grace_blocks = grace_blocks }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves alternate chains async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<AlternateChainResponse> GetAlternateChainsAsync(CancellationToken token = default)
        {
            AlternateChainResponse responseObject = await SendJsonRpcCommandAsync<AlternateChainResponse>("get_alternate_chains", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the relay transaction async operation.
        /// </summary>
        /// <param name="hex">The hex.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<RelayTransactionResponse> RelayTransactionAsync(string hex, CancellationToken token = default)
        {
            var genericRequestParameters = new GenericRequestParameters()
            {
                Hex = hex,
            };
            RelayTransactionResponse responseObject = await SendJsonRpcCommandAsync<RelayTransactionResponse>("relay_tx", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the sync information async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SyncronizeInformationResponse> SyncInformationAsync(CancellationToken token = default)
        {
            SyncronizeInformationResponse responseObject = await SendJsonRpcCommandAsync<SyncronizeInformationResponse>("sync_info", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves block async.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<BlockResponse> GetBlockAsync(uint height, CancellationToken token = default)
        {
            BlockResponse responseObject = await SendJsonRpcCommandAsync<BlockResponse>("get_block", new GenericRequestParameters() { Height = height }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves block async.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<BlockResponse> GetBlockAsync(string hash, CancellationToken token = default)
        {
            BlockResponse responseObject = await SendJsonRpcCommandAsync<BlockResponse>("get_block", new GenericRequestParameters() { Hash = hash }, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Sets bans async.
        /// </summary>
        /// <param name="bans">The bans.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SetBansResponse> SetBansAsync(IEnumerable<NodeBan> bans, CancellationToken token = default)
        {
            var daemonRequestParameters = new GenericRequestParameters()
            {
                Bans = [.. bans],
            };
            SetBansResponse responseObject = await SendJsonRpcCommandAsync<SetBansResponse>("set_bans", daemonRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the sweep single async operation.
        /// </summary>
        /// <param name="address">The target Monero address.</param>
        /// <param name="account_index">The account index.</param>
        /// <param name="transaction_priority">The transaction priority.</param>
        /// <param name="ring_size">The ring size.</param>
        /// <param name="unlock_time">The unlock time.</param>
        /// <param name="get_tx_key">The get tx key.</param>
        /// <param name="get_tx_hex">The get tx hex.</param>
        /// <param name="get_tx_metadata">The get tx metadata.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SweepSingleResponse> SweepSingleAsync(string address, uint account_index, TransferPriority transaction_priority, uint ring_size, ulong unlock_time, bool get_tx_key, bool get_tx_hex, bool get_tx_metadata, CancellationToken token = default)
        {
            var walletRequestParameters = new GenericRequestParameters()
            {
                Address = address,
                Account_index = account_index,
                Priority = (uint)transaction_priority,
                Ring_size = ring_size,
                Unlock_time = unlock_time,
                Get_tx_key = get_tx_key,
                Get_tx_hex = get_tx_hex,
                Get_tx_metadata = get_tx_metadata,
            };

            SweepSingleResponse responseObject = await SendJsonRpcCommandAsync<SweepSingleResponse>("sweep_single", walletRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the describe transfer async operation.
        /// </summary>
        /// <param name="txSet">The tx set.</param>
        /// <param name="isMultiSig">The is multi sig.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<DescribeTransferResponse> DescribeTransferAsync(string txSet, bool isMultiSig, CancellationToken token = default)
        {
            var walletRequestParameters = new GenericRequestParameters();
            if (isMultiSig)
            {
                walletRequestParameters.Multisig_txset = txSet;
            }
            else
            {
                walletRequestParameters.Unsigned_txset = txSet;
            }

            DescribeTransferResponse responseObject = await SendJsonRpcCommandAsync<DescribeTransferResponse>("describe_transfer", walletRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves payment detail async.
        /// </summary>
        /// <param name="payment_id">The payment id.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<PaymentDetailResponse> GetPaymentDetailAsync(string payment_id, CancellationToken token = default)
        {
            var walletRequestParameters = new GenericRequestParameters()
            {
                Payment_id = payment_id,
            };
            PaymentDetailResponse responseObject = await SendJsonRpcCommandAsync<PaymentDetailResponse>("get_payments", walletRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the submit blocks async operation.
        /// </summary>
        /// <param name="blockBlobs">The block blobs.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SubmitBlockResponse> SubmitBlocksAsync(IEnumerable<string> blockBlobs, CancellationToken token = default)
        {
            var daemonRequestParameters = new GenericRequestParameters
            {
                Request = blockBlobs,
            };
            SubmitBlockResponse responseObject = await SendJsonRpcCommandAsync<SubmitBlockResponse>("submit_block", daemonRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves block template async.
        /// </summary>
        /// <param name="reserve_size">The reserve size.</param>
        /// <param name="wallet_address">The wallet address.</param>
        /// <param name="prev_block">The prev block.</param>
        /// <param name="extra_nonce">The extra nonce.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<GetBlockTemplateResponse> GetBlockTemplateAsync(ulong reserve_size, string wallet_address, string? prev_block = null, string? extra_nonce = null, CancellationToken token = default)
        {
            var daemonRequestParameters = new GenericRequestParameters()
            {
                Reserve_size = reserve_size,
                Wallet_address = wallet_address,
                Prev_block = prev_block,
                Extra_nonce = extra_nonce,
            };
            GetBlockTemplateResponse responseObject = await SendJsonRpcCommandAsync<GetBlockTemplateResponse>("get_block_template", daemonRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves ban status async.
        /// </summary>
        /// <param name="address">The target Monero address.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<GetBanStatusResponse> GetBanStatusAsync(string address, CancellationToken token = default)
        {
            var daemonRequestParameters = new GenericRequestParameters()
            {
                Address = address,
            };
            GetBanStatusResponse responseObject = await SendJsonRpcCommandAsync<GetBanStatusResponse>("banned", daemonRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the prune blockchain async operation.
        /// </summary>
        /// <param name="check">The check.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<PruneBlockchainResponse> PruneBlockchainAsync(bool check, CancellationToken token = default)
        {
            var daemonRequestParameters = new GenericRequestParameters()
            {
                Check = check,
            };
            PruneBlockchainResponse responseObject = await SendJsonRpcCommandAsync<PruneBlockchainResponse>("prune_blockchain", daemonRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves transaction pool backlog async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<TransactionPoolBacklogResponse> GetTransactionPoolBacklogAsync(CancellationToken token = default)
        {
            TransactionPoolBacklogResponse responseObject = await SendJsonRpcCommandAsync<TransactionPoolBacklogResponse>("get_txpool_backlog", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Sets attribute async.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SetAttributeResponse> SetAttributeAsync(string key, string value, CancellationToken token = default)
        {
            var daemonRequestParameters = new GenericRequestParameters()
            {
                Key = key,
                Value = value,
            };
            SetAttributeResponse responseObject = await SendJsonRpcCommandAsync<SetAttributeResponse>("set_attribute", daemonRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves attribute async.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<GetAttributeResponse> GetAttributeAsync(string key, CancellationToken token = default)
        {
            var daemonRequestParameters = new GenericRequestParameters()
            {
                Key = key,
            };
            GetAttributeResponse responseObject = await SendJsonRpcCommandAsync<GetAttributeResponse>("get_attribute", daemonRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves transaction pool async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<TransactionPool> GetTransactionPoolAsync(CancellationToken token = default)
        {
            TransactionPool responseObject = await SendPathJsonCommandAsync<TransactionPool>("get_transaction_pool", null, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Retrieves transactions async.
        /// </summary>
        /// <param name="txHashes">The tx hashes.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<TransactionSet> GetTransactionsAsync(IEnumerable<string> txHashes, CancellationToken token = default)
        {
            var daemonRequestParameters = new CustomRequestParameters()
            {
                Txs_hashes = txHashes,
            };
            TransactionSet responseObject = await SendPathJsonCommandAsync<TransactionSet>("get_transactions", daemonRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Executes the verify bool async operation.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="address">The target Monero address.</param>
        /// <param name="signature">The signature.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<bool> VerifyBoolAsync(string data, string address, string signature, CancellationToken token = default)
        {
            VerifyResponse result = await VerifyAsync(data, address, signature, token).ConfigureAwait(false);
            return result.Result?.IsGood ?? false;
        }

        /// <summary>
        /// Executes the validate address async operation.
        /// </summary>
        /// <param name="address">The target Monero address.</param>
        /// <param name="anyNetType">The any net type.</param>
        /// <param name="allowOpenAlias">The allow open alias.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ValidateAddress> ValidateAddressAsync(string address, bool anyNetType = false, bool allowOpenAlias = true, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Address = address,
                Any_net_type = anyNetType,
                Allow_openalias = allowOpenAlias,
            };

            ValidateAddressResponse response = await SendJsonRpcCommandAsync<ValidateAddressResponse>("validate_address", parameters, token).ConfigureAwait(false);
            return response.Result ?? new ValidateAddress();
        }

        /// <summary>
        /// Executes the make integrated address async operation.
        /// </summary>
        /// <param name="standardAddress">The standard address.</param>
        /// <param name="paymentId">The payment id.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<IntegratedAddress> MakeIntegratedAddressAsync(string? standardAddress = null, string? paymentId = null, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Standard_address = standardAddress,
                Payment_id = paymentId,
            };

            IntegratedAddressResponse response = await SendJsonRpcCommandAsync<IntegratedAddressResponse>("make_integrated_address", parameters, token).ConfigureAwait(false);
            return response.Result ?? new IntegratedAddress();
        }

        /// <summary>
        /// Executes the split integrated address async operation.
        /// </summary>
        /// <param name="integratedAddress">The integrated address.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<IntegratedAddress> SplitIntegratedAddressAsync(string integratedAddress, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters { Integrated_address = integratedAddress };
            IntegratedAddressResponse response = await SendJsonRpcCommandAsync<IntegratedAddressResponse>("split_integrated_address", parameters, token).ConfigureAwait(false);
            return response.Result ?? new IntegratedAddress();
        }

        /// <summary>
        /// Retrieves bulk payments async.
        /// </summary>
        /// <param name="paymentIds">The payment ids.</param>
        /// <param name="minBlockHeight">The min block height.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<List<PaymentDetail>> GetBulkPaymentsAsync(IEnumerable<string> paymentIds, ulong? minBlockHeight = null, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Payment_ids = paymentIds,
                Min_block_height = minBlockHeight,
            };
            BulkPaymentsResponse response = await SendJsonRpcCommandAsync<BulkPaymentsResponse>("get_bulk_payments", parameters, token).ConfigureAwait(false);
            return response.Result?.Payments ?? [];
        }

        /// <summary>
        /// Sets auto refresh async.
        /// </summary>
        /// <param name="enabled">The enabled.</param>
        /// <param name="periodSeconds">The period seconds.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetAutoRefreshAsync(bool enabled, uint? periodSeconds = null, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Enable = enabled,
                Period = periodSeconds,
            };

            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("auto_refresh", parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the scan transactions async operation.
        /// </summary>
        /// <param name="txIds">The tx ids.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ScanTransactionsAsync(IEnumerable<string> txIds, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters { Txids = txIds };
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("scan_tx", parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the rescan blockchain async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RescanBlockchainAsync(CancellationToken token = default)
        {
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("rescan_blockchain", new GenericRequestParameters(), token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the relay transaction metadata async operation.
        /// </summary>
        /// <param name="txMetadata">The tx metadata.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<string> RelayTransactionMetadataAsync(string txMetadata, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters { Transaction_metadata = txMetadata };
            RelayTransactionResponse response = await SendJsonRpcCommandAsync<RelayTransactionResponse>("relay_tx", parameters, token).ConfigureAwait(false);
            return response.Result?.TxHash ?? string.Empty;
        }

        /// <summary>
        /// Executes the start wallet mining async operation.
        /// </summary>
        /// <param name="threadsCount">The threads count.</param>
        /// <param name="doBackgroundMining">The do background mining.</param>
        /// <param name="ignoreBattery">The ignore battery.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StartWalletMiningAsync(ulong threadsCount, bool doBackgroundMining, bool ignoreBattery, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Threads_count = threadsCount,
                Do_background_mining = doBackgroundMining,
                Ignore_battery = ignoreBattery,
            };
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("start_mining", parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the stop wallet mining async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StopWalletMiningAsync(CancellationToken token = default)
        {
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("stop_mining", new GenericRequestParameters(), token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the freeze output async operation.
        /// </summary>
        /// <param name="keyImage">The key image.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task FreezeOutputAsync(string keyImage, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters { Key_image = keyImage };
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("freeze", parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the thaw output async operation.
        /// </summary>
        /// <param name="keyImage">The key image.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ThawOutputAsync(string keyImage, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters { Key_image = keyImage };
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("thaw", parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the is output frozen async operation.
        /// </summary>
        /// <param name="keyImage">The key image.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<bool> IsOutputFrozenAsync(string keyImage, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters { Key_image = keyImage };
            FrozenOutputResponse response = await SendJsonRpcCommandAsync<FrozenOutputResponse>("frozen", parameters, token).ConfigureAwait(false);
            return response.Result?.Frozen ?? false;
        }

        /// <summary>
        /// Retrieves default fee priority async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<TransferPriority> GetDefaultFeePriorityAsync(CancellationToken token = default)
        {
            DefaultFeePriorityResponse response = await SendJsonRpcCommandAsync<DefaultFeePriorityResponse>("get_default_fee_priority", new GenericRequestParameters(), token).ConfigureAwait(false);
            uint priority = response.Result?.Priority ?? 0u;
            if (!Enum.IsDefined(typeof(TransferPriority), priority))
            {
                return TransferPriority.Default;
            }

            return (TransferPriority)priority;
        }

        /// <summary>
        /// Retrieves transaction proof async.
        /// </summary>
        /// <param name="txid">The txid.</param>
        /// <param name="address">The target Monero address.</param>
        /// <param name="message">The message.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<string> GetTransactionProofAsync(string txid, string address, string? message = null, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Txid = txid,
                Address = address,
                Message = message,
            };
            TransactionProofResponse response = await SendJsonRpcCommandAsync<TransactionProofResponse>("get_tx_proof", parameters, token).ConfigureAwait(false);
            return response.Result?.Signature ?? string.Empty;
        }

        /// <summary>
        /// Executes the check transaction proof async operation.
        /// </summary>
        /// <param name="txid">The txid.</param>
        /// <param name="address">The target Monero address.</param>
        /// <param name="message">The message.</param>
        /// <param name="signature">The signature.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<CheckTransactionProof> CheckTransactionProofAsync(string txid, string address, string message, string signature, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Txid = txid,
                Address = address,
                Message = message,
                Signature = signature,
            };
            CheckTransactionProofResponse response = await SendJsonRpcCommandAsync<CheckTransactionProofResponse>("check_tx_proof", parameters, token).ConfigureAwait(false);
            return response.Result ?? new CheckTransactionProof();
        }

        /// <summary>
        /// Retrieves spend proof async.
        /// </summary>
        /// <param name="txid">The txid.</param>
        /// <param name="message">The message.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<string> GetSpendProofAsync(string txid, string? message = null, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Txid = txid,
                Message = message,
            };
            SpendProofResponse response = await SendJsonRpcCommandAsync<SpendProofResponse>("get_spend_proof", parameters, token).ConfigureAwait(false);
            return response.Result?.Signature ?? string.Empty;
        }

        /// <summary>
        /// Executes the check spend proof async operation.
        /// </summary>
        /// <param name="txid">The txid.</param>
        /// <param name="message">The message.</param>
        /// <param name="signature">The signature.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<bool> CheckSpendProofAsync(string txid, string message, string signature, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Txid = txid,
                Message = message,
                Signature = signature,
            };
            CheckSpendProofResponse response = await SendJsonRpcCommandAsync<CheckSpendProofResponse>("check_spend_proof", parameters, token).ConfigureAwait(false);
            return response.Result?.Good ?? false;
        }

        /// <summary>
        /// Retrieves reserve proof async.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="accountIndex">The account index.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<string> GetReserveProofAsync(string message, uint? accountIndex = null, ulong? amount = null, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Message = message,
                Account_index = accountIndex,
                Amount = amount,
            };
            ReserveProofResponse response = await SendJsonRpcCommandAsync<ReserveProofResponse>("get_reserve_proof", parameters, token).ConfigureAwait(false);
            return response.Result?.Signature ?? string.Empty;
        }

        /// <summary>
        /// Executes the check reserve proof async operation.
        /// </summary>
        /// <param name="address">The target Monero address.</param>
        /// <param name="message">The message.</param>
        /// <param name="signature">The signature.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<CheckReserveProof> CheckReserveProofAsync(string address, string message, string signature, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Address = address,
                Message = message,
                Signature = signature,
            };
            CheckReserveProofResponse response = await SendJsonRpcCommandAsync<CheckReserveProofResponse>("check_reserve_proof", parameters, token).ConfigureAwait(false);
            return response.Result ?? new CheckReserveProof();
        }

        /// <summary>
        /// Sets daemon connection async.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetDaemonConnectionAsync(MoneroDaemonConnection connection, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Address = connection.Address,
                Username = connection.Username,
                Password = connection.Password,
                Trusted = connection.Trusted,
            };
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("set_daemon", parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the exchange multi sig keys async operation.
        /// </summary>
        /// <param name="multisigInfo">The multisig info.</param>
        /// <param name="password">The wallet password.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<ExchangeMultiSigKeys> ExchangeMultiSigKeysAsync(IEnumerable<string> multisigInfo, string password, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Multisig_info = multisigInfo,
                Password = password,
            };
            ExchangeMultiSigKeysResponse response = await SendJsonRpcCommandAsync<ExchangeMultiSigKeysResponse>("exchange_multisig_keys", parameters, token).ConfigureAwait(false);
            return response.Result ?? new ExchangeMultiSigKeys();
        }

        /// <summary>
        /// Executes the estimate tx size and weight async operation.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<EstimateTxSizeAndWeight> EstimateTxSizeAndWeightAsync(MoneroTxConfig config, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Destinations = ConvertDestinations(config.Destinations),
                Payment_id = config.PaymentId,
                Priority = config.Priority.HasValue ? (uint?)config.Priority.Value : null,
                Account_index = config.AccountIndex,
                Subaddr_indices = config.SubaddressIndices,
                Relay = config.Relay,
                Do_not_relay = config.DoNotRelay,
                Unlock_time = config.UnlockTime,
            };
            EstimateTxSizeAndWeightResponse response = await SendJsonRpcCommandAsync<EstimateTxSizeAndWeightResponse>("estimate_tx_size_and_weight", parameters, token).ConfigureAwait(false);
            return response.Result ?? new EstimateTxSizeAndWeight();
        }

        /// <summary>
        /// Sets up background sync async.
        /// </summary>
        /// <param name="walletPassword">The wallet password.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetupBackgroundSyncAsync(string? walletPassword = null, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters { Password = walletPassword };
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("setup_background_sync", parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the start background sync async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StartBackgroundSyncAsync(CancellationToken token = default)
        {
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("start_background_sync", new GenericRequestParameters(), token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the stop background sync async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StopBackgroundSyncAsync(CancellationToken token = default)
        {
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("stop_background_sync", new GenericRequestParameters(), token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the generate from keys wallet async operation.
        /// </summary>
        /// <param name="options">Additional options for the operation.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<GenerateFromKeysWallet> GenerateFromKeysWalletAsync(GenerateFromKeysOptions options, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Restore_height = options.RestoreHeight,
                Filename = options.Filename,
                Address = options.Address,
                Spendkey = options.SpendKey,
                Viewkey = options.ViewKey,
                Password = options.Password,
                Autosave_current = options.SaveCurrent,
            };
            GenerateFromKeysWalletResponse response = await SendJsonRpcCommandAsync<GenerateFromKeysWalletResponse>("generate_from_keys", parameters, token).ConfigureAwait(false);
            return response.Result ?? new GenerateFromKeysWallet();
        }

        /// <summary>
        /// Executes the restore deterministic wallet async operation.
        /// </summary>
        /// <param name="options">Additional options for the operation.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<RestoreDeterministicWallet> RestoreDeterministicWalletAsync(RestoreDeterministicWalletOptions options, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Filename = options.Filename,
                Password = options.Password,
                Seed = options.Seed,
                Seed_offset = options.SeedOffset,
                Restore_height = options.RestoreHeight,
                Language = options.Language,
                Autosave_current = options.SaveCurrent,
                Enable_multisig_experimental = options.EnableMultisigExperimental,
            };
            RestoreDeterministicWalletResponse response = await SendJsonRpcCommandAsync<RestoreDeterministicWalletResponse>("restore_deterministic_wallet", parameters, token).ConfigureAwait(false);
            return response.Result ?? new RestoreDeterministicWallet();
        }

        /// <summary>
        /// Executes the edit address book async operation.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="setAddress">The set address.</param>
        /// <param name="address">The target Monero address.</param>
        /// <param name="setDescription">The set description.</param>
        /// <param name="description">The description.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task EditAddressBookAsync(uint index, bool setAddress, string? address, bool setDescription, string? description, CancellationToken token = default)
        {
            var parameters = new GenericRequestParameters
            {
                Index = index,
                Should_set_address = setAddress,
                Address = address,
                Should_set_description = setDescription,
                Description = description,
            };
            _ = await SendJsonRpcCommandAsync<RpcResultResponse<object>>("edit_address_book", parameters, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the transfer with config async operation.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<FundTransfer> TransferWithConfigAsync(MoneroTxConfig config, CancellationToken token = default)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            bool? doNotRelay = config.DoNotRelay;
            if (!doNotRelay.HasValue && config.Relay.HasValue)
            {
                doNotRelay = !config.Relay.Value;
            }

            var parameters = new GenericRequestParameters
            {
                Destinations = ConvertDestinations(config.Destinations),
                Payment_id = config.PaymentId,
                Priority = config.Priority.HasValue ? (uint?)config.Priority.Value : null,
                Account_index = config.AccountIndex,
                Subaddr_indices = config.SubaddressIndices,
                Do_not_relay = doNotRelay,
                Unlock_time = config.UnlockTime,
            };

            FundTransferResponse response = await SendJsonRpcCommandAsync<FundTransferResponse>("transfer", parameters, token).ConfigureAwait(false);
            return response.Result ?? new FundTransfer();
        }

        /// <summary>
        /// Executes the transfer split with config async operation.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SplitFundTransfer> TransferSplitWithConfigAsync(MoneroTxConfig config, CancellationToken token = default)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            bool? doNotRelay = config.DoNotRelay;
            if (!doNotRelay.HasValue && config.Relay.HasValue)
            {
                doNotRelay = !config.Relay.Value;
            }

            var parameters = new GenericRequestParameters
            {
                Destinations = ConvertDestinations(config.Destinations),
                Payment_id = config.PaymentId,
                Priority = config.Priority.HasValue ? (uint?)config.Priority.Value : null,
                Account_index = config.AccountIndex,
                Subaddr_indices = config.SubaddressIndices,
                Do_not_relay = doNotRelay,
                Unlock_time = config.UnlockTime,
            };

            SplitFundTransferResponse response = await SendJsonRpcCommandAsync<SplitFundTransferResponse>("transfer_split", parameters, token).ConfigureAwait(false);
            return response.Result ?? new SplitFundTransfer();
        }

        /// <summary>
        /// Retrieves block hash async.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<string> GetBlockHashAsync(ulong height, CancellationToken token = default)
        {
            BlockHashResponse response = await SendJsonRpcCommandAsync<BlockHashResponse>("on_get_block_hash", new[] { height }, token).ConfigureAwait(false);
            return response.Result ?? string.Empty;
        }

        /// <summary>
        /// Retrieves alternate block hashes async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<List<string>> GetAlternateBlockHashesAsync(CancellationToken token = default)
        {
            AlternateBlockHashesResponse response = await SendPathJsonCommandAsync<AlternateBlockHashesResponse>("get_alt_blocks_hashes", new GenericRequestParameters(), token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "get_alt_blocks_hashes");
            return response.BlockHashes;
        }

        /// <summary>
        /// Retrieves peer list async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<PeerList> GetPeerListAsync(CancellationToken token = default)
        {
            PeerListResponse response = await SendPathJsonCommandAsync<PeerListResponse>("get_peer_list", new GenericRequestParameters(), token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "get_peer_list");
            return new PeerList
            {
                WhiteList = response.WhiteList,
                GrayList = response.GrayList,
            };
        }

        /// <summary>
        /// Retrieves transaction pool stats async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<TransactionPoolStats> GetTransactionPoolStatsAsync(CancellationToken token = default)
        {
            TransactionPoolStatsResponse response = await SendPathJsonCommandAsync<TransactionPoolStatsResponse>("get_transaction_pool_stats", new GenericRequestParameters(), token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "get_transaction_pool_stats");
            return response.PoolStats ?? new TransactionPoolStats();
        }

        /// <summary>
        /// Retrieves key image spent statuses async.
        /// </summary>
        /// <param name="keyImages">The key images.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<List<KeyImageSpentStatus>> GetKeyImageSpentStatusesAsync(IEnumerable<string> keyImages, CancellationToken token = default)
        {
            KeyImageSpentStatusesResponse response = await SendPathJsonCommandAsync<KeyImageSpentStatusesResponse>(
                "is_key_image_spent",
                new GenericRequestParameters { Key_images = keyImages },
                token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "is_key_image_spent");
            return [.. response.SpentStatus.Select(s => Enum.IsDefined(typeof(KeyImageSpentStatus), s) ? (KeyImageSpentStatus)s : KeyImageSpentStatus.Unspent)];
        }

        /// <summary>
        /// Retrieves outputs async.
        /// </summary>
        /// <param name="outputs">The outputs.</param>
        /// <param name="includeTransactionId">The include transaction id.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<List<DaemonOutput>> GetOutputsAsync(IEnumerable<DaemonOutputRequest> outputs, bool includeTransactionId = true, CancellationToken token = default)
        {
            OutputsResponse response = await SendPathJsonCommandAsync<OutputsResponse>(
                "get_outs",
                new GenericRequestParameters { Outputs = outputs, Include_txid = includeTransactionId },
                token).ConfigureAwait(false);

            ThrowIfNonOkStatus(response.Status, "get_outs");
            return response.Outputs;
        }

        /// <summary>
        /// Retrieves output distribution async.
        /// </summary>
        /// <param name="amounts">The amounts.</param>
        /// <param name="cumulative">The cumulative.</param>
        /// <param name="fromHeight">The from height.</param>
        /// <param name="toHeight">The to height.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<OutputDistribution> GetOutputDistributionAsync(IEnumerable<ulong> amounts, bool cumulative = false, ulong? fromHeight = null, ulong? toHeight = null, CancellationToken token = default)
        {
            OutputDistributionResponse response = await SendJsonRpcCommandAsync<OutputDistributionResponse>(
                "get_output_distribution",
                new GenericRequestParameters { Amounts = amounts, Cumulative = cumulative, From_height = fromHeight, To_height = toHeight },
                token).ConfigureAwait(false);

            return new OutputDistribution { Distributions = response.Result?.Distributions ?? [] };
        }

        /// <summary>
        /// Retrieves bandwidth limit async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<BandwidthLimit> GetBandwidthLimitAsync(CancellationToken token = default)
        {
            BandwidthLimitResponse response = await SendPathJsonCommandAsync<BandwidthLimitResponse>("get_limit", new GenericRequestParameters(), token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "get_limit");
            return new BandwidthLimit { LimitDown = response.LimitDown, LimitUp = response.LimitUp };
        }

        /// <summary>
        /// Sets bandwidth limit async.
        /// </summary>
        /// <param name="limitDown">The limit down.</param>
        /// <param name="limitUp">The limit up.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<BandwidthLimit> SetBandwidthLimitAsync(int? limitDown = null, int? limitUp = null, CancellationToken token = default)
        {
            BandwidthLimitResponse response = await SendPathJsonCommandAsync<BandwidthLimitResponse>(
                "set_limit",
                new GenericRequestParameters { Limit_down = limitDown ?? 0, Limit_up = limitUp ?? 0 },
                token).ConfigureAwait(false);

            ThrowIfNonOkStatus(response.Status, "set_limit");
            return new BandwidthLimit { LimitDown = response.LimitDown, LimitUp = response.LimitUp };
        }

        /// <summary>
        /// Retrieves outgoing peer limit async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<int> GetOutgoingPeerLimitAsync(CancellationToken token = default)
        {
            PeerLimitResponse response = await SendPathJsonCommandAsync<PeerLimitResponse>("out_peers", new GenericRequestParameters { Set = false }, token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "out_peers");
            if (!response.OutPeers.HasValue)
            {
                throw new MoneroRpcProtocolException("Missing 'out_peers' value from Monero RPC call 'out_peers'.");
            }

            return response.OutPeers.Value;
        }

        /// <summary>
        /// Retrieves incoming peer limit async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<int> GetIncomingPeerLimitAsync(CancellationToken token = default)
        {
            PeerLimitResponse response = await SendPathJsonCommandAsync<PeerLimitResponse>("in_peers", new GenericRequestParameters { Set = false }, token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "in_peers");
            if (!response.InPeers.HasValue)
            {
                throw new MoneroRpcProtocolException("Missing 'in_peers' value from Monero RPC call 'in_peers'.");
            }

            return response.InPeers.Value;
        }

        /// <summary>
        /// Sets outgoing peer limit async.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetOutgoingPeerLimitAsync(int count, CancellationToken token = default)
        {
            StatusOnlyResponse response = await SendPathJsonCommandAsync<StatusOnlyResponse>("out_peers", new GenericRequestParameters { Out_peers = count }, token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "out_peers");
        }

        /// <summary>
        /// Sets incoming peer limit async.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetIncomingPeerLimitAsync(int count, CancellationToken token = default)
        {
            StatusOnlyResponse response = await SendPathJsonCommandAsync<StatusOnlyResponse>("in_peers", new GenericRequestParameters { In_peers = count }, token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "in_peers");
        }

        /// <summary>
        /// Executes the start daemon mining async operation.
        /// </summary>
        /// <param name="minerAddress">The miner address.</param>
        /// <param name="threadCount">The thread count.</param>
        /// <param name="backgroundMining">The background mining.</param>
        /// <param name="ignoreBattery">The ignore battery.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StartDaemonMiningAsync(string minerAddress, ulong threadCount, bool backgroundMining = false, bool ignoreBattery = true, CancellationToken token = default)
        {
            StatusOnlyResponse response = await SendPathJsonCommandAsync<StatusOnlyResponse>(
                "start_mining",
                new GenericRequestParameters
                {
                    Miner_address = minerAddress,
                    Threads_count = threadCount,
                    Do_background_mining = backgroundMining,
                    Ignore_battery = ignoreBattery,
                },
                token).ConfigureAwait(false);

            ThrowIfNonOkStatus(response.Status, "start_mining");
        }

        /// <summary>
        /// Executes the stop daemon mining async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StopDaemonMiningAsync(CancellationToken token = default)
        {
            StatusOnlyResponse response = await SendPathJsonCommandAsync<StatusOnlyResponse>("stop_mining", new GenericRequestParameters(), token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "stop_mining");
        }

        /// <summary>
        /// Retrieves mining status async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<MiningStatus> GetMiningStatusAsync(CancellationToken token = default)
        {
            MiningStatusResponse response = await SendPathJsonCommandAsync<MiningStatusResponse>("mining_status", new GenericRequestParameters(), token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "mining_status");
            return new MiningStatus
            {
                Active = response.Active,
                Address = response.Address,
                ThreadsCount = response.ThreadsCount,
                Speed = response.Speed,
            };
        }

        /// <summary>
        /// Executes the submit raw transaction async operation.
        /// </summary>
        /// <param name="txAsHex">The tx as hex.</param>
        /// <param name="doNotRelay">The do not relay.</param>
        /// <param name="doSanityChecks">The do sanity checks.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<SubmitRawTransaction> SubmitRawTransactionAsync(string txAsHex, bool doNotRelay = false, bool doSanityChecks = true, CancellationToken token = default)
        {
            SubmitRawTransactionResponse response = await SendPathJsonCommandAsync<SubmitRawTransactionResponse>(
                "send_raw_transaction",
                new GenericRequestParameters
                {
                    Tx_as_hex = txAsHex,
                    Do_not_relay = doNotRelay,
                    Do_sanity_checks = doSanityChecks,
                },
                token).ConfigureAwait(false);

            ThrowIfNonOkStatus(response.Status, "send_raw_transaction");

            return new SubmitRawTransaction
            {
                Status = response.Status,
                DoubleSpend = response.DoubleSpend,
            };
        }

        /// <summary>
        /// Executes the check for update async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<DaemonUpdateCheck> CheckForUpdateAsync(CancellationToken token = default)
        {
            DaemonUpdateResponse response = await SendPathJsonCommandAsync<DaemonUpdateResponse>("update", new GenericRequestParameters { Command = "check" }, token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "update");
            return new DaemonUpdateCheck
            {
                UpdateAvailable = response.UpdateAvailable,
                Version = response.Version,
                Hash = response.Hash,
            };
        }

        /// <summary>
        /// Executes the download update async operation.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<DaemonUpdateDownload> DownloadUpdateAsync(string? path = null, CancellationToken token = default)
        {
            DaemonUpdateResponse response = await SendPathJsonCommandAsync<DaemonUpdateResponse>(
                "update",
                new GenericRequestParameters { Command = "download", Path = path },
                token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "update");
            return new DaemonUpdateDownload
            {
                Path = response.Path,
                Status = response.Status,
            };
        }

        /// <summary>
        /// Executes the stop daemon async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StopDaemonAsync(CancellationToken token = default)
        {
            StatusOnlyResponse response = await SendPathJsonCommandAsync<StatusOnlyResponse>("stop_daemon", new GenericRequestParameters(), token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "stop_daemon");
        }

        /// <summary>
        /// Retrieves network stats async.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<NetworkStats> GetNetworkStatsAsync(CancellationToken token = default)
        {
            NetworkStatsResponse response = await SendPathJsonCommandAsync<NetworkStatsResponse>("get_net_stats", new GenericRequestParameters(), token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "get_net_stats");
            return new NetworkStats
            {
                TotalBytesIn = response.TotalBytesIn,
                TotalBytesOut = response.TotalBytesOut,
            };
        }

        /// <summary>
        /// Retrieves public nodes async.
        /// </summary>
        /// <param name="gray">The gray.</param>
        /// <param name="white">The white.</param>
        /// <param name="includeBlocked">The include blocked.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public async Task<PublicNodes> GetPublicNodesAsync(bool gray = false, bool white = true, bool includeBlocked = false, CancellationToken token = default)
        {
            PublicNodesResponse response = await SendPathJsonCommandAsync<PublicNodesResponse>(
                "get_public_nodes",
                new GenericRequestParameters { Gray = gray, White = white, Include_blocked = includeBlocked },
                token).ConfigureAwait(false);

            ThrowIfNonOkStatus(response.Status, "get_public_nodes");
            return new PublicNodes
            {
                GrayNodes = response.Gray,
                WhiteNodes = response.White,
            };
        }

        /// <summary>
        /// Executes the save blockchain async operation.
        /// </summary>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SaveBlockchainAsync(CancellationToken token = default)
        {
            StatusOnlyResponse response = await SendPathJsonCommandAsync<StatusOnlyResponse>("save_bc", new GenericRequestParameters(), token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "save_bc");
        }

        /// <summary>
        /// Sets log hash rate async.
        /// </summary>
        /// <param name="visible">The visible.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetLogHashRateAsync(bool visible, CancellationToken token = default)
        {
            StatusOnlyResponse response = await SendPathJsonCommandAsync<StatusOnlyResponse>("set_log_hash_rate", new GenericRequestParameters { Visible = visible }, token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "set_log_hash_rate");
        }

        /// <summary>
        /// Sets log level async.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetLogLevelAsync(uint level, CancellationToken token = default)
        {
            StatusOnlyResponse response = await SendPathJsonCommandAsync<StatusOnlyResponse>("set_log_level", new GenericRequestParameters { Level = level }, token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "set_log_level");
        }

        /// <summary>
        /// Sets log categories async.
        /// </summary>
        /// <param name="categories">The categories.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetLogCategoriesAsync(string categories, CancellationToken token = default)
        {
            StatusOnlyResponse response = await SendPathJsonCommandAsync<StatusOnlyResponse>("set_log_categories", new GenericRequestParameters { Categories = categories }, token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Status, "set_log_categories");
        }

        /// <summary>
        /// Executes the flush cache async operation.
        /// </summary>
        /// <param name="badTxs">The bad txs.</param>
        /// <param name="badBlocks">The bad blocks.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task FlushCacheAsync(bool badTxs = false, bool badBlocks = false, CancellationToken token = default)
        {
            FlushCacheResponse response = await SendJsonRpcCommandAsync<FlushCacheResponse>(
                "flush_cache",
                new GenericRequestParameters { Bad_txs = badTxs, Bad_blocks = badBlocks },
                token).ConfigureAwait(false);
            ThrowIfNonOkStatus(response.Result?.Status, "flush_cache");
        }

        /// <summary>
        /// Retrieves blocks binary async.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<BinaryRpcPayload> GetBlocksBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
            => SendPathBinaryCommandAsync("get_blocks.bin", requestBody, token);

        /// <summary>
        /// Retrieves blocks by height binary async.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<BinaryRpcPayload> GetBlocksByHeightBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
            => SendPathBinaryCommandAsync("get_blocks_by_height.bin", requestBody, token);

        /// <summary>
        /// Retrieves hashes binary async.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<BinaryRpcPayload> GetHashesBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
            => SendPathBinaryCommandAsync("get_hashes.bin", requestBody, token);

        /// <summary>
        /// Retrieves output indexes binary async.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<BinaryRpcPayload> GetOutputIndexesBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
            => SendPathBinaryCommandAsync("get_o_indexes.bin", requestBody, token);

        /// <summary>
        /// Retrieves outputs binary async.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<BinaryRpcPayload> GetOutputsBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
            => SendPathBinaryCommandAsync("get_outs.bin", requestBody, token);

        /// <summary>
        /// Retrieves transaction pool hashes binary async.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<BinaryRpcPayload> GetTransactionPoolHashesBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
            => SendPathBinaryCommandAsync("get_transaction_pool_hashes.bin", requestBody, token);

        /// <summary>
        /// Retrieves output distribution binary async.
        /// </summary>
        /// <param name="requestBody">The request body.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
        public Task<BinaryRpcPayload> GetOutputDistributionBinaryAsync(ReadOnlyMemory<byte> requestBody, CancellationToken token = default)
            => SendPathBinaryCommandAsync("get_output_distribution.bin", requestBody, token);

        /// <summary>
        /// Releases resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            if (_ownsHttpClient)
            {
                _httpClient.Dispose();
            }
        }

        /// <summary>
        /// Sends an HTTP request with the configured timeout, returning the response message.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The HTTP response message.</returns>
        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            if (_options.Timeout <= TimeSpan.Zero)
            {
                return await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.CancelAfter(_options.Timeout);
            return await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token).ConfigureAwait(false);
        }

        /// <summary>
        /// Extracts the response body stream from a successful HTTP response, throwing <see cref="MoneroRpcHttpException"/> on non-success status codes.
        /// </summary>
        /// <param name="response">The HTTP response to read.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A stream wrapping the response body that disposes the response when closed.</returns>
        private async Task<Stream> GetResponseStreamAsync(HttpResponseMessage response, CancellationToken token)
        {
            if (!response.IsSuccessStatusCode)
            {
                string? snippet = await ReadResponseBodySnippetAsync(response, token).ConfigureAwait(false);

                HttpRequestMessage? req = response.RequestMessage;
                string? rpcMethod = req?.GetMoneroRpcMethod();
                string? requestId = req?.GetMoneroRpcRequestId();
                Uri? requestUri = req?.RequestUri;
                HttpStatusCode statusCode = response.StatusCode;

                response.Dispose();

                throw new MoneroRpcHttpException(
                    statusCode,
                    snippet,
                    message: $"HTTP {(int)statusCode} ({statusCode}) calling Monero RPC.",
                    rpcMethod: rpcMethod,
                    requestId: requestId,
                    requestUri: requestUri);
            }

            if (response.Content is null)
            {
                response.Dispose();
                throw new MoneroRpcProtocolException("Monero RPC response did not contain a content payload.");
            }

            Stream stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
            return new ResponseDisposingStream(stream, response);
        }

        /// <summary>
        /// Reads the first N characters from an HTTP response body for use in error messages.
        /// The snippet length is controlled by <see cref="MoneroRpcClientOptions.MaxResponseBodySnippetChars"/>.
        /// </summary>
        /// <param name="response">The HTTP response whose body to read.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A truncated string of the response body, or <see langword="null"/> if the body is empty.</returns>
        private async Task<string?> ReadResponseBodySnippetAsync(HttpResponseMessage response, CancellationToken token)
        {
            if (response.Content is null)
            {
                return null;
            }

            using Stream stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

            using var reader = new StreamReader(stream);
            char[] buffer = new char[Math.Max(1, _options.MaxResponseBodySnippetChars)];
            int read = await reader.ReadBlockAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            return read <= 0 ? null : new string(buffer, 0, read);
        }

        /// <summary>
        /// Deserializes a JSON response stream, validates JSON-RPC protocol constraints, and throws typed exceptions for protocol or remote errors.
        /// </summary>
        /// <typeparam name="T">The expected response type.</typeparam>
        /// <param name="responseStream">The response body stream.</param>
        /// <param name="request">The original HTTP request (used for error context).</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The deserialized response object.</returns>
        private async Task<T> DeserializeResponseAsync<T>(Stream responseStream, HttpRequestMessage request, CancellationToken token)
        {
            T? obj;
            try
            {
                JsonTypeInfo<T> typeInfo = (JsonTypeInfo<T>)_serializerOptions.GetTypeInfo(typeof(T));
                obj = await JsonSerializer.DeserializeAsync(responseStream, typeInfo, token).ConfigureAwait(false);
            }
            catch (JsonException ex)
            {
                throw new MoneroRpcProtocolException(
                    message: "Invalid JSON received from Monero RPC.",
                    innerException: ex,
                    rpcMethod: request.GetMoneroRpcMethod(),
                    requestId: request.GetMoneroRpcRequestId(),
                    requestUri: request.RequestUri);
            }

            if (obj == null)
            {
                throw new MoneroRpcProtocolException(
                    message: "Empty or unrecognized JSON payload received from Monero RPC.",
                    rpcMethod: request.GetMoneroRpcMethod(),
                    requestId: request.GetMoneroRpcRequestId(),
                    requestUri: request.RequestUri);
            }

            if (obj is RpcResponse rpcResponse)
            {
                if (rpcResponse.Id == null || rpcResponse.JsonRpc == null)
                {
                    throw new MoneroRpcProtocolException(
                        message: "Malformed JSON-RPC response received from Monero RPC.",
                        rpcMethod: request.GetMoneroRpcMethod(),
                        requestId: request.GetMoneroRpcRequestId(),
                        requestUri: request.RequestUri);
                }

                if (!string.Equals(rpcResponse.JsonRpc, FieldAndHeaderDefaults.JsonRpc, StringComparison.Ordinal))
                {
                    throw new MoneroRpcProtocolException(
                        message: $"Unexpected jsonrpc version '{rpcResponse.JsonRpc}'.",
                        rpcMethod: request.GetMoneroRpcMethod(),
                        requestId: request.GetMoneroRpcRequestId(),
                        requestUri: request.RequestUri);
                }

                string? expectedId = request.GetMoneroRpcRequestId();
                if (expectedId != null && !string.Equals(expectedId, rpcResponse.Id, StringComparison.Ordinal))
                {
                    throw new MoneroRpcProtocolException(
                        message: $"Mismatched JSON-RPC id. Expected '{expectedId}', got '{rpcResponse.Id}'.",
                        rpcMethod: request.GetMoneroRpcMethod(),
                        requestId: expectedId,
                        requestUri: request.RequestUri);
                }

                if (rpcResponse.ContainsError && rpcResponse.Error != null)
                {
                    string? msg = rpcResponse.Error.Message;
                    int code = rpcResponse.Error.Code;
                    JsonElement? data = rpcResponse.Error.Data;
                    JsonRpcErrorCode jsonRpcErrorCode = rpcResponse.Error.JsonRpcErrorCode;

                    throw new JsonRpcException(
                        message: $"Remote JSON-RPC error {code}: {msg}",
                        jsonRpcErrorCode: jsonRpcErrorCode,
                        code: code,
                        remoteMessage: msg,
                        data: data,
                        rpcMethod: request.GetMoneroRpcMethod(),
                        requestId: rpcResponse.Id,
                        requestUri: request.RequestUri);
                }
            }

            return obj;
        }

        /// <summary>
        /// Sends a JSON-RPC request to the <c>/json_rpc</c> endpoint, reads the response stream, and deserializes the result.
        /// </summary>
        /// <typeparam name="T">The expected response type.</typeparam>
        /// <param name="method">The JSON-RPC method name.</param>
        /// <param name="requestParameters">The method parameters, or <see langword="null"/>.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The deserialized response.</returns>
        private async Task<T> SendJsonRpcCommandAsync<T>(string method, object? requestParameters, CancellationToken token)
        {
            using HttpRequestMessage request = _requestAdapter.CreateJsonRpcRequestMessage(method, requestParameters);
            using HttpResponseMessage response = await SendAsync(request, token).ConfigureAwait(false);
            using Stream stream = await GetResponseStreamAsync(response, token).ConfigureAwait(false);
            return await DeserializeResponseAsync<T>(stream, request, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a JSON POST request to a path-based (non-JSON-RPC) endpoint and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The expected response type.</typeparam>
        /// <param name="endpoint">The relative endpoint path (e.g. <c>get_transactions</c>).</param>
        /// <param name="requestBody">The request body, or <see langword="null"/>.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The deserialized response.</returns>
        private async Task<T> SendPathJsonCommandAsync<T>(string endpoint, object? requestBody, CancellationToken token)
        {
            using HttpRequestMessage request = _requestAdapter.CreatePathJsonRequestMessage(endpoint, requestBody);
            using HttpResponseMessage response = await SendAsync(request, token).ConfigureAwait(false);
            using Stream stream = await GetResponseStreamAsync(response, token).ConfigureAwait(false);
            return await DeserializeResponseAsync<T>(stream, request, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a binary POST request to a <c>.bin</c> endpoint and returns the raw response payload.
        /// </summary>
        /// <param name="endpoint">The relative binary endpoint path (e.g. <c>getblocks.bin</c>).</param>
        /// <param name="requestBody">The serialized binary request payload.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The raw binary response including status code, headers, and body.</returns>
        private async Task<BinaryRpcPayload> SendPathBinaryCommandAsync(string endpoint, ReadOnlyMemory<byte> requestBody, CancellationToken token)
        {
            using HttpRequestMessage request = _requestAdapter.CreatePathBinaryRequestMessage(endpoint, requestBody);
            using HttpResponseMessage response = await SendAsync(request, token).ConfigureAwait(false);

            HttpStatusCode statusCode = response.StatusCode;
            string? contentType = response.Content?.Headers?.ContentType?.MediaType;
            var headers = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
            {
                headers[header.Key] = header.Value.ToArray();
            }

            if (response.Content != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in response.Content.Headers)
                {
                    headers[header.Key] = header.Value.ToArray();
                }
            }

            using Stream stream = await GetResponseStreamAsync(response, token).ConfigureAwait(false);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, 81920, token).ConfigureAwait(false);

            return new BinaryRpcPayload
            {
                StatusCode = statusCode,
                ContentType = contentType,
                Headers = headers,
                Body = memoryStream.ToArray(),
            };
        }

        /// <summary>
        /// Sends a <c>get_balance</c> JSON-RPC request with the given parameters.
        /// </summary>
        /// <param name="genericRequestParameters">The request parameters.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The balance response.</returns>
        private async Task<BalanceResponse> GetBalanceAsync(GenericRequestParameters genericRequestParameters, CancellationToken token)
        {
            BalanceResponse responseObject = await SendJsonRpcCommandAsync<BalanceResponse>("get_balance", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Sends a <c>transfer</c> JSON-RPC request with the given parameters.
        /// </summary>
        /// <param name="genericRequestParameters">The request parameters.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The fund transfer response.</returns>
        private async Task<FundTransferResponse> TransferFundsAsync(GenericRequestParameters genericRequestParameters, CancellationToken token)
        {
            FundTransferResponse responseObject = await SendJsonRpcCommandAsync<FundTransferResponse>("transfer", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Sends a <c>transfer_split</c> JSON-RPC request with the given parameters.
        /// </summary>
        /// <param name="genericRequestParameters">The request parameters.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The split fund transfer response.</returns>
        private async Task<SplitFundTransferResponse> TransferSplitFundsAsync(GenericRequestParameters genericRequestParameters, CancellationToken token)
        {
            SplitFundTransferResponse responseObject = await SendJsonRpcCommandAsync<SplitFundTransferResponse>("transfer_split", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Sends an <c>incoming_transfers</c> JSON-RPC request with the given parameters.
        /// </summary>
        /// <param name="genericRequestParameters">The request parameters.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The incoming transfers response.</returns>
        private async Task<IncomingTransfersResponse> GetIncomingTransfersAsync(GenericRequestParameters genericRequestParameters, CancellationToken token)
        {
            IncomingTransfersResponse responseObject = await SendJsonRpcCommandAsync<IncomingTransfersResponse>("incoming_transfers", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Sends a <c>get_transfers</c> JSON-RPC request with the given parameters.
        /// </summary>
        /// <param name="genericRequestParameters">The request parameters.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The show transfers response.</returns>
        private async Task<ShowTransfersResponse> GetTransfersAsync(GenericRequestParameters genericRequestParameters, CancellationToken token)
        {
            ShowTransfersResponse responseObject = await SendJsonRpcCommandAsync<ShowTransfersResponse>("get_transfers", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }

        /// <summary>
        /// Sends a <c>get_transfer_by_txid</c> JSON-RPC request with the given parameters.
        /// </summary>
        /// <param name="genericRequestParameters">The request parameters.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The transfer-by-txid response.</returns>
        private async Task<GetTransferByTxidResponse> GetTransferByTxidAsync(GenericRequestParameters genericRequestParameters, CancellationToken token)
        {
            GetTransferByTxidResponse responseObject = await SendJsonRpcCommandAsync<GetTransferByTxidResponse>("get_transfer_by_txid", genericRequestParameters, token).ConfigureAwait(false);
            return responseObject;
        }
    }
}
