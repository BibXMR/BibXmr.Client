using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>DaemonClientLifecycle</c> behavior.
    /// </summary>
    public class DaemonClientLifecycleTests
    {
        [Fact]
        public async Task DisposeAsync_DisposesWithoutError()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                return HttpTestHelpers.Json(
                    HttpStatusCode.OK,
                    $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            await client.DisposeAsync();
        }

        [Fact]
        public async Task DisposeAsync_CalledTwice_DoesNotThrow()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                return HttpTestHelpers.Json(
                    HttpStatusCode.OK,
                    $"{{\"jsonrpc\":\"2.0\",\"id\":\"{doc.RootElement.GetProperty("id").GetString()}\",\"result\":{{}}}}");
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            await client.DisposeAsync();
            await client.DisposeAsync();
        }

        [Fact]
        public async Task CreateAsync_NonLoopbackHttpWithoutOverride_ThrowsArgumentException_FailFast()
        {
            int requestCount = 0;
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                requestCount++;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

            using var httpClient = new HttpClient(handler);
            ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://example.org:18081/")));

            Assert.Contains("Clear-text HTTP is blocked for non-loopback endpoints", ex.Message, StringComparison.Ordinal);
            Assert.Equal(0, requestCount);
        }

        [Fact]
        public async Task CreateAsync_NonLoopbackHttpWithOverride_AllowsRequests()
        {
            var options = new BibXmr.Client.Network.MoneroRpcClientOptions
            {
                UnsafeAllowClearTextHttpOnNonLoopback = true,
            };

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal("json_rpc", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));

                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                Assert.Equal("get_block_count", doc.RootElement.GetProperty("method").GetString());
                string id = doc.RootElement.GetProperty("id").GetString()!;

                return HttpTestHelpers.Json(
                    HttpStatusCode.OK,
                    $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":{{\"count\":1,\"status\":\"OK\",\"untrusted\":false}}}}");
            });

            using var httpClient = new HttpClient(handler);
            using MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(
                httpClient,
                new Uri("http://example.org:18081/"),
                options);

            ulong count = await client.GetBlockCountAsync();
            Assert.Equal(1ul, count);
        }
    }
}

