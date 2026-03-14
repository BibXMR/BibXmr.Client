using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;
using BibXmr.Client.Network.Exceptions;
using BibXmr.Client.Wallet;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>Regression</c> behavior.
    /// </summary>
    public class RegressionTests
    {
        [Fact]
        public async Task RelayTransaction_UsesRelayTxMethod()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal("json_rpc", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                Assert.Equal("relay_tx", doc.RootElement.GetProperty("method").GetString());
                string? id = doc.RootElement.GetProperty("id").GetString();
                return HttpTestHelpers.Json(HttpStatusCode.OK, $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":{{\"tx_hash\":\"abc\"}}}}");
            });

            using var httpClient = new HttpClient(handler);
            using MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            string txHash = await client.RelayTransactionAsync("deadbeef");
            Assert.Equal("abc", txHash);
        }

        [Fact]
        public async Task SweepAll_HttpFailure_UsesStandardHttpExceptionPath()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                string? method = doc.RootElement.GetProperty("method").GetString();

                if (method == "open_wallet" || method == "store" || method == "close_wallet")
                    return HttpTestHelpers.Json(HttpStatusCode.OK, $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":{{}}}}");

                if (method == "sweep_all")
                    return HttpTestHelpers.Text(HttpStatusCode.InternalServerError, "server-error", "text/plain");

                throw new InvalidOperationException($"Unexpected method '{method}'.");
            });

            using var httpClient = new HttpClient(handler);
            using MoneroWalletClient client = await MoneroWalletClient.CreateAsync(httpClient, new Uri("http://localhost:18082/"), "wallet.bin", "pw");

            MoneroRpcHttpException ex = await Assert.ThrowsAsync<MoneroRpcHttpException>(() =>
                client.SweepAllAsync("44dest", 0, BibXmr.Client.Wallet.Dto.TransferPriority.Default, 2));

            Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
            Assert.Equal("sweep_all", ex.RpcMethod);
        }
    }
}



