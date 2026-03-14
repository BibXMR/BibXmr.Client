using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;
using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>DaemonClientRequest</c> behavior.
    /// </summary>
    public class DaemonClientRequestTests
    {
        [Fact]
        public async Task GetTransactionPoolAsync_UsesCustomEndpoint_AndParsesResponse()
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                Assert.Equal("get_transaction_pool", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));

                // Custom endpoint response is not JSON-RPC; it's a plain JSON object.
                string json = @"
{
  ""credits"": 0,
  ""status"": ""OK"",
  ""top_hash"": ""0000000000000000000000000000000000000000000000000000000000000000"",
  ""spent_key_images"": [],
  ""transactions"": [],
  ""untrusted"": false
}";
                return Task.FromResult(HttpTestHelpers.Json(HttpStatusCode.OK, json));
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            TransactionPool pool = await client.GetTransactionPoolAsync(CancellationToken.None);
            Assert.Equal("OK", pool.Status);
            Assert.Empty(pool.Transactions);
            Assert.Empty(pool.SpentKeyImages);
        }

        [Fact]
        public async Task GetTransactionsAsync_UsesCustomEndpoint_AndSendsTxHashes()
        {
            string[] requested = new[] { "a", "b" };

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal("get_transactions", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));

                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string?[] txsHashes = doc.RootElement.GetProperty("txs_hashes").EnumerateArray().Select(e => e.GetString()).ToArray();
                Assert.Equal(requested, txsHashes);

                string json = @"
{
  ""credits"": 0,
  ""top_hash"": """",
  ""status"": ""OK"",
  ""txs"": [
    {
      ""as_hex"": """",
      ""as_json"": ""{}"",
      ""prunable_as_hex"": """",
      ""prunable_hash"": """",
      ""pruned_as_hex"": """",
      ""double_spend_seen"": false,
      ""in_pool"": false,
      ""tx_hash"": ""a"",
      ""block_height"": 0,
      ""block_timestamp"": 0
    }
  ],
  ""txs_as_hex"": [],
  ""untrusted"": false
}";
                return HttpTestHelpers.Json(HttpStatusCode.OK, json);
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            List<Transaction> txs = await client.GetTransactionsAsync(requested, CancellationToken.None);
            Assert.Single(txs);
            Assert.Equal("a", txs[0].TxHash);
        }

        [Fact]
        public async Task GetBlockTemplateAsync_ReserveSizeOver255_Throws()
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                // Should never be called since the client validates reserveSize.
                throw new InvalidOperationException("Request should not have been sent.");
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetBlockTemplateAsync(256, "44..."));
        }
    }
}



