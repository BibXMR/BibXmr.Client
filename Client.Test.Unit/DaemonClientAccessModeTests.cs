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
    /// Provides tests for <c>DaemonClientAccessMode</c> behavior.
    /// </summary>
    public class DaemonClientAccessModeTests
    {
        [Fact]
        public async Task CreateRestrictedAsync_CanRunRestrictedSafeCall()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                string? method = doc.RootElement.GetProperty("method").GetString();

                Assert.Equal("get_block_count", method);
                return JsonRpc(id, "{\"count\":42,\"status\":\"OK\",\"untrusted\":false}");
            });

            using var httpClient = new HttpClient(handler);
            using IRestrictedMoneroDaemonClient client = await MoneroDaemonClient.CreateRestrictedAsync(httpClient, new Uri("http://localhost:18081/"));

            ulong blockCount = await client.GetBlockCountAsync();
            Assert.Equal((ulong)42, blockCount);
        }

        [Fact]
        public async Task CreateUnrestrictedAsync_CanRunUnrestrictedCall()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal("set_log_level", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));

                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                uint level = doc.RootElement.GetProperty("level").GetUInt32();
                Assert.Equal((uint)2, level);

                return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\"}");
            });

            using var httpClient = new HttpClient(handler);
            using IUnrestrictedMoneroDaemonClient client = await MoneroDaemonClient.CreateUnrestrictedAsync(httpClient, new Uri("http://localhost:18081/"));

            await client.SetLogLevelAsync(2);
        }

        [Fact]
        public async Task DetectRpcAccessAsync_WhenRestrictedFlagTrue_ReturnsRestricted()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                string? method = doc.RootElement.GetProperty("method").GetString();
                Assert.Equal("get_info", method);
                return JsonRpc(id, "{\"restricted\":true}");
            });

            using var httpClient = new HttpClient(handler);

            MoneroDaemonRpcAccess access = await MoneroDaemonClient.DetectRpcAccessAsync(httpClient, new Uri("http://localhost:18081/"));
            Assert.Equal(MoneroDaemonRpcAccess.Restricted, access);
        }

        [Fact]
        public async Task DetectRpcAccessAsync_WhenRestrictedFlagFalse_ReturnsUnrestricted()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                string? method = doc.RootElement.GetProperty("method").GetString();
                Assert.Equal("get_info", method);
                return JsonRpc(id, "{\"restricted\":false}");
            });

            using var httpClient = new HttpClient(handler);

            MoneroDaemonRpcAccess access = await MoneroDaemonClient.DetectRpcAccessAsync(httpClient, new Uri("http://localhost:18081/"));
            Assert.Equal(MoneroDaemonRpcAccess.Unrestricted, access);
        }

        private static HttpResponseMessage JsonRpc(string id, string resultJson)
        {
            string json = $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":{resultJson}}}";
            return HttpTestHelpers.Json(HttpStatusCode.OK, json);
        }
    }
}

