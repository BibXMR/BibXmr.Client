using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Network;
using BibXmr.Client.Network.Exceptions;
using BibXmr.Client.Wallet.Dto;
using BibXmr.Client.Test.Unit.CoverageHarness;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>RpcCommunicatorEdge</c> behavior.
    /// </summary>
    public class RpcCommunicatorEdgeTests
    {
        [Fact]
        public async Task SendAsync_WhenTimeoutIsZero_UsesDirectTokenPath()
        {
            RecordingHttpMessageHandler handler = UniversalRpcServer.CreateHandler();
            using var httpClient = new HttpClient(handler);
            var options = new MoneroRpcClientOptions { Timeout = TimeSpan.Zero };

            var rpc = new RpcCommunicator(httpClient, new Uri("http://localhost:18081/"), options);
            _ = await rpc.GetBlockCountAsync(CancellationToken.None);
        }

        [Fact]
        public async Task ReadResponseBodySnippetAsync_WhenContentIsNull_UsesNullSnippet()
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                // Intentionally leave Content = null
                return Task.FromResult(resp);
            });

            using var httpClient = new HttpClient(handler);
            var rpc = new RpcCommunicator(httpClient, new Uri("http://localhost:18081/"));

            MoneroRpcHttpException ex = await Assert.ThrowsAsync<MoneroRpcHttpException>(() => rpc.GetBlockCountAsync(CancellationToken.None));
            Assert.Null(ex.ResponseBodySnippet);
        }

        [Fact]
        public async Task TagAccounts_EmptyAccounts_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.TagAccountsAsync("t", Array.Empty<uint>(), CancellationToken.None));
        }

        [Fact]
        public async Task UntagAccounts_EmptyAccounts_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.UntagAccountsAsync(Array.Empty<uint>(), CancellationToken.None));
        }

        [Fact]
        public async Task TransferAsync_RingSizeTooSmall_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            (string, ulong)[] txs = new[] { ("addr", 1ul) };
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.TransferAsync(txs, TransferPriority.Default, ring_size: 1u, token: CancellationToken.None));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.TransferAsync(txs, TransferPriority.Default, ring_size: 1u, account_index: 0u, token: CancellationToken.None));
        }

        [Fact]
        public async Task TransferSplitAsync_RingSizeTooSmall_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            (string, ulong)[] txs = new[] { ("addr", 1ul) };
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.TransferSplitAsync(txs, TransferPriority.Default, ring_size: 1u, account_index: 0u, token: CancellationToken.None));
        }

        [Fact]
        public async Task GetTransfers_InvalidSelector_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.GetTransfersAsync(@in: false, @out: false, pending: false, failed: false, pool: false, token: CancellationToken.None));
        }

        [Fact]
        public async Task GetTransfers_MaxHeightLessThanMinHeight_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.GetTransfersAsync(@in: true, @out: false, pending: false, failed: false, pool: false, min_height: 2, max_height: 1, token: CancellationToken.None));
        }

        [Fact]
        public async Task GetTransfers_InvalidSelector_WithHeightFilter_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.GetTransfersAsync(@in: false, @out: false, pending: false, failed: false, pool: false, min_height: 0, max_height: 0, token: CancellationToken.None));
        }

        [Fact]
        public async Task SetTransactionNotes_EmptyTxIdsOrNotes_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.SetTransactionNotesAsync(Array.Empty<string>(), new[] { "n" }, CancellationToken.None));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.SetTransactionNotesAsync(new[] { "t" }, Array.Empty<string>(), CancellationToken.None));
        }

        [Fact]
        public async Task GetTransactionNotes_EmptyTxIds_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.GetTransactionNotesAsync(Array.Empty<string>(), CancellationToken.None));
        }

        [Fact]
        public async Task GetBlockHeaderRange_InvalidHeights_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.GetBlockHeaderRangeAsync(startHeight: 2, endHeight: 1, CancellationToken.None));
        }

        [Fact]
        public async Task GetOutputHistogram_InvalidHeights_Throws()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.GetOutputHistogramAsync(new[] { 1ul }, from_height: 2, to_height: 1, cumulative: false, binary: false, compress: false, CancellationToken.None));
        }

        [Fact]
        public async Task GetPrivateKey_KeyTypeSwitch_CoversAllBranches()
        {
            var rpc = new RpcCommunicator(new HttpClient(UniversalRpcServer.CreateHandler()), new Uri("http://localhost:18081/"));

            _ = await rpc.GetPrivateKey(KeyType.Mnemonic, CancellationToken.None);
            _ = await rpc.GetPrivateKey(KeyType.ViewKey, CancellationToken.None);
            _ = await rpc.GetPrivateKey(KeyType.SpendKey, CancellationToken.None);

            await Assert.ThrowsAsync<InvalidOperationException>(() => rpc.GetPrivateKey((KeyType)999, CancellationToken.None));
        }

        [Fact]
        public async Task ResponseDisposingStream_AllOverrides_AreCallable()
        {
            Type outer = typeof(RpcCommunicator);
            Type? nested = outer.GetNestedType("ResponseDisposingStream", BindingFlags.NonPublic);
            Assert.NotNull(nested);

            using var inner = new MemoryStream(new byte[] { 1, 2, 3 }, writable: true);
            using var response = new HttpResponseMessage(HttpStatusCode.OK);

            var s = (Stream)Activator.CreateInstance(nested!, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, binder: null, args: new object[] { inner, response }, culture: null)!;

            _ = s.CanRead;
            _ = s.CanSeek;
            _ = s.CanWrite;
            _ = s.Length;
            _ = s.Position;
            s.Position = 0;
            s.Flush();
            _ = s.Read(new byte[1], 0, 1);
            _ = s.Seek(0, SeekOrigin.Begin);
            s.SetLength(1);
            s.Write(new byte[] { 1 }, 0, 1);
            _ = await s.ReadAsync(new byte[1], 0, 1, CancellationToken.None);
            await s.WriteAsync(new byte[] { 1 }, 0, 1, CancellationToken.None);

            s.Dispose();
        }

        [Fact]
        public void Dispose_WhenOwningHttpClient_Disposes()
        {
            // Construction via url/port sets ownsHttpClient = true.
            var rpc = new RpcCommunicator("localhost", 1u);
            rpc.Dispose();
        }

        [Fact]
        public void PrivateCtor_WhenOwningHttpClient_SetsInfiniteTimeout()
        {
            var httpClient = new HttpClient();
            var adapter = new MoneroRequestAdapter(new Uri("http://localhost:18081/", UriKind.Absolute));
            var options = new MoneroRpcClientOptions();

            ConstructorInfo? ctor = typeof(RpcCommunicator).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: new[] { typeof(HttpClient), typeof(bool), typeof(MoneroRequestAdapter), typeof(MoneroRpcClientOptions) },
                modifiers: null);

            Assert.NotNull(ctor);

            var rpc = (RpcCommunicator)ctor!.Invoke(new object[] { httpClient, true, adapter, options });
            Assert.Equal(Timeout.InfiniteTimeSpan, httpClient.Timeout);

            rpc.Dispose();
        }

        [Fact]
        public void NetworkCtor_CoversAllSwitchCombinations()
        {
            // These are pure constructor paths (no network I/O) but exercise the adapter selection switch.
            new RpcCommunicator(MoneroNetwork.Stagenet, ConnectionType.Daemon).Dispose();
            new RpcCommunicator(MoneroNetwork.Testnet, ConnectionType.Daemon).Dispose();
            new RpcCommunicator(MoneroNetwork.Stagenet, ConnectionType.Wallet).Dispose();
            new RpcCommunicator(MoneroNetwork.Testnet, ConnectionType.Wallet).Dispose();

            Assert.Throws<InvalidOperationException>(() => new RpcCommunicator(MoneroNetwork.Mainnet, (ConnectionType)999));
        }
    }
}



