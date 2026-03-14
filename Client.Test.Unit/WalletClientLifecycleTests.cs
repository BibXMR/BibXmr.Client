using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Network;
using BibXmr.Client.Wallet;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>WalletClientLifecycle</c> behavior.
    /// </summary>
    public class WalletClientLifecycleTests
    {
        [Fact]
        public async Task CreateAsync_SendsOpenWallet_AndDispose_SendsStoreAndClose()
        {
            int openWalletCalls = 0;
            int storeCalls = 0;
            int closeCalls = 0;

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal("json_rpc", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));

                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string? method = doc.RootElement.GetProperty("method").GetString();

                // Keep the branching explicit; when this grows, it is easy to audit.
                switch (method)
                {
                    case "open_wallet":
                        openWalletCalls++;
                        Assert.Equal("wallet.bin", doc.RootElement.GetProperty("params").GetProperty("filename").GetString());
                        Assert.Equal("pw", doc.RootElement.GetProperty("params").GetProperty("password").GetString());
                        return HttpTestHelpers.Json(
                            HttpStatusCode.OK,
                            $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");

                    case "store":
                        storeCalls++;
                        return HttpTestHelpers.Json(
                            HttpStatusCode.OK,
                            $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");

                    case "close_wallet":
                        closeCalls++;
                        return HttpTestHelpers.Json(
                            HttpStatusCode.OK,
                            $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");

                    default:
                        throw new InvalidOperationException($"Unexpected wallet RPC method '{method}'.");
                }
            });

            using var httpClient = new HttpClient(handler);
            MoneroWalletClient client = await MoneroWalletClient.CreateAsync(httpClient, new Uri("http://localhost:18082/"), "wallet.bin", "pw");

            Assert.Equal(1, openWalletCalls);

            client.Dispose();

            Assert.Equal(1, storeCalls);
            Assert.Equal(1, closeCalls);
        }

        [Fact]
        public async Task DisposeAsync_SendsStoreAndClose()
        {
            int openWalletCalls = 0;
            int storeCalls = 0;
            int closeCalls = 0;

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string? method = doc.RootElement.GetProperty("method").GetString();

                switch (method)
                {
                    case "open_wallet":
                        openWalletCalls++;
                        return HttpTestHelpers.Json(
                            HttpStatusCode.OK,
                            $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");

                    case "store":
                        storeCalls++;
                        return HttpTestHelpers.Json(
                            HttpStatusCode.OK,
                            $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");

                    case "close_wallet":
                        closeCalls++;
                        return HttpTestHelpers.Json(
                            HttpStatusCode.OK,
                            $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");

                    default:
                        throw new InvalidOperationException($"Unexpected wallet RPC method '{method}'.");
                }
            });

            using var httpClient = new HttpClient(handler);
            MoneroWalletClient client = await MoneroWalletClient.CreateAsync(httpClient, new Uri("http://localhost:18082/"), "wallet.bin", "pw");

            Assert.Equal(1, openWalletCalls);

            await client.DisposeAsync();

            Assert.Equal(1, storeCalls);
            Assert.Equal(1, closeCalls);
        }

        [Fact]
        public async Task DisposeAsync_CalledTwice_OnlySendsStoreAndCloseOnce()
        {
            int storeCalls = 0;
            int closeCalls = 0;

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string? method = doc.RootElement.GetProperty("method").GetString();

                switch (method)
                {
                    case "open_wallet":
                        return HttpTestHelpers.Json(
                            HttpStatusCode.OK,
                            $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");
                    case "store":
                        storeCalls++;
                        return HttpTestHelpers.Json(
                            HttpStatusCode.OK,
                            $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");
                    case "close_wallet":
                        closeCalls++;
                        return HttpTestHelpers.Json(
                            HttpStatusCode.OK,
                            $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");
                    default:
                        throw new InvalidOperationException($"Unexpected wallet RPC method '{method}'.");
                }
            });

            using var httpClient = new HttpClient(handler);
            MoneroWalletClient client = await MoneroWalletClient.CreateAsync(httpClient, new Uri("http://localhost:18082/"), "wallet.bin", "pw");

            await client.DisposeAsync();
            await client.DisposeAsync();

            Assert.Equal(1, storeCalls);
            Assert.Equal(1, closeCalls);
        }

        [Fact]
        public async Task CreateAsync_WithOptions_UsesOptionsForTimeout()
        {
            var options = new MoneroRpcClientOptions
            {
                Timeout = TimeSpan.FromMilliseconds(50),
            };

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string? method = doc.RootElement.GetProperty("method").GetString();

                if (method == "open_wallet")
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), ct);
                    return HttpTestHelpers.Json(
                        HttpStatusCode.OK,
                        $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");
                }

                throw new InvalidOperationException("Unexpected method.");
            });

            using var httpClient = new HttpClient(handler);

            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                MoneroWalletClient.CreateAsync(httpClient, new Uri("http://localhost:18082/"), "wallet.bin", "pw", options));
        }
    }
}


