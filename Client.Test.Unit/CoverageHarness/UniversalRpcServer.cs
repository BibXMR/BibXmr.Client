using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Test.Unit.Infrastructure;

namespace BibXmr.Client.Test.Unit.CoverageHarness
{
    /// <summary>
    /// Hosts an in-memory universal RPC endpoint for coverage tests.
    /// </summary>
    internal static class UniversalRpcServer
    {
        public static RecordingHttpMessageHandler CreateHandler()
        {
            return new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Uri uri = req.RequestUri ?? throw new InvalidOperationException("RequestUri was null.");
                string path = HttpTestHelpers.GetPathAndQuery(uri);

                // Most code paths only require a syntactically-valid payload; they do not depend on concrete
                // monerod/monero-wallet-rpc semantics.
                if (string.Equals(path, "json_rpc", StringComparison.OrdinalIgnoreCase))
                {
                    // Ensure the request body can be read (covers request content plumbing too).
                    using JsonDocument requestJson = await HttpTestHelpers.ReadJsonAsync(req, ct).ConfigureAwait(false);
                    string id = requestJson.RootElement.GetProperty("id").GetString() ?? string.Empty;
                    string? method = requestJson.RootElement.GetProperty("method").GetString();

                    if (string.Equals(method, "on_get_block_hash", StringComparison.Ordinal))
                    {
                        string stringResult = $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":\"deadbeef\"}}";
                        return HttpTestHelpers.Json(HttpStatusCode.OK, stringResult);
                    }

                    string json = $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":{{}}}}";
                    return HttpTestHelpers.Json(HttpStatusCode.OK, json);
                }

                // Custom (non-JSON-RPC) endpoints.
                if (string.Equals(path, "get_transaction_pool", StringComparison.OrdinalIgnoreCase))
                    return HttpTestHelpers.Json(HttpStatusCode.OK, "{}");

                if (string.Equals(path, "get_transactions", StringComparison.OrdinalIgnoreCase))
                    return HttpTestHelpers.Json(HttpStatusCode.OK, "{}");

                if (string.Equals(path, "out_peers", StringComparison.OrdinalIgnoreCase))
                    return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"out_peers\":8}");

                if (string.Equals(path, "in_peers", StringComparison.OrdinalIgnoreCase))
                    return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"in_peers\":16}");

                return HttpTestHelpers.Json(HttpStatusCode.OK, "{}");
            });
        }
    }
}

