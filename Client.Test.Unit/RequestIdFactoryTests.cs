using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;
using BibXmr.Client.Network;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>RequestIdFactory</c> behavior.
    /// </summary>
    public class RequestIdFactoryTests
    {
        [Fact]
        public async Task CustomRequestIdFactory_IsUsed_PerRequest()
        {
            int counter = 0;
            var observedIds = new List<string>();

            var options = new MoneroRpcClientOptions
            {
                RequestIdFactory = () => $"req-{++counter}",
            };

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string? id = doc.RootElement.GetProperty("id").GetString();
                Assert.False(string.IsNullOrWhiteSpace(id));
                observedIds.Add(id!);

                string json = $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":{{\"count\":1,\"status\":\"OK\",\"untrusted\":false}}}}";
                return HttpTestHelpers.Json(HttpStatusCode.OK, json);
            });

            using var httpClient = new HttpClient(handler);
            using MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"), options);

            _ = await client.GetBlockCountAsync();
            _ = await client.GetBlockCountAsync();

            Assert.Equal(new[] { "req-1", "req-2" }, observedIds);
        }
    }
}


