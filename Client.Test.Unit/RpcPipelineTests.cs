using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;
using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Network;
using BibXmr.Client.Network.Exceptions;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>RpcPipeline</c> behavior.
    /// </summary>
    public class RpcPipelineTests
    {
        [Fact]
        public async Task JsonRpcRemoteError_ThrowsJsonRpcException_WithCodeAndMetadata()
        {
            string? requestId = null;
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal("json_rpc", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));

                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                Assert.Equal("get_block_count", doc.RootElement.GetProperty("method").GetString());
                Assert.Equal("2.0", doc.RootElement.GetProperty("jsonrpc").GetString());
                requestId = doc.RootElement.GetProperty("id").GetString();
                Assert.False(string.IsNullOrWhiteSpace(requestId));

                string json = $"{{\"jsonrpc\":\"2.0\",\"id\":\"{requestId}\",\"error\":{{\"code\":-32601,\"message\":\"Method not found\"}}}}";
                return HttpTestHelpers.Json(HttpStatusCode.OK, json);
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            JsonRpcException ex = await Assert.ThrowsAsync<JsonRpcException>(() => client.GetBlockCountAsync());
            Assert.Equal(-32601, ex.Code);
            Assert.Equal("Method not found", ex.RemoteMessage);
            Assert.Equal("get_block_count", ex.RpcMethod);
            Assert.Equal(requestId, ex.RequestId);
            Assert.NotNull(ex.RequestUri);
        }

        [Fact]
        public async Task HttpNonSuccess_ThrowsMoneroRpcHttpException_WithSnippetAndMetadata()
        {
            var options = new MoneroRpcClientOptions
            {
                MaxResponseBodySnippetChars = 3,
            };

            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                return Task.FromResult(HttpTestHelpers.Text(HttpStatusCode.InternalServerError, "fail..."));
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"), options);

            MoneroRpcHttpException ex = await Assert.ThrowsAsync<MoneroRpcHttpException>(() => client.GetBlockCountAsync());
            Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
            Assert.Equal("fai", ex.ResponseBodySnippet);
            Assert.Equal("get_block_count", ex.RpcMethod);
            Assert.False(string.IsNullOrWhiteSpace(ex.RequestId));
            Assert.NotNull(ex.RequestUri);
        }

        [Fact]
        public async Task Cancellation_IsPropagated()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, ct);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
            await Assert.ThrowsAsync<TaskCanceledException>(() => client.GetBlockCountAsync(cts.Token));
        }

        [Fact]
        public async Task Timeout_UsesOptionsTimeout_AndCancelsRequest()
        {
            var options = new MoneroRpcClientOptions
            {
                Timeout = TimeSpan.FromMilliseconds(50),
            };

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10), ct);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"), options);

            await Assert.ThrowsAsync<TaskCanceledException>(() => client.GetBlockCountAsync());
        }

        [Fact]
        public async Task InvalidJson_ThrowsMoneroRpcProtocolException_WithRequestContext()
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                return Task.FromResult(HttpTestHelpers.Text(HttpStatusCode.OK, "not-json", "application/json"));
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            MoneroRpcProtocolException ex = await Assert.ThrowsAsync<MoneroRpcProtocolException>(() => client.GetBlockCountAsync());
            Assert.Equal("get_block_count", ex.RpcMethod);
            Assert.False(string.IsNullOrWhiteSpace(ex.RequestId));
            Assert.NotNull(ex.RequestUri);
        }

        [Fact]
        public async Task NullJson_ThrowsMoneroRpcProtocolException()
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                return Task.FromResult(HttpTestHelpers.Json(HttpStatusCode.OK, "null"));
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            await Assert.ThrowsAsync<MoneroRpcProtocolException>(() => client.GetBlockCountAsync());
        }

        [Fact]
        public async Task MissingJsonRpcFields_ThrowsMoneroRpcProtocolException()
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                // Missing "id"
                string json = "{\"jsonrpc\":\"2.0\",\"result\":{\"count\":1,\"status\":\"OK\",\"untrusted\":false}}";
                return Task.FromResult(HttpTestHelpers.Json(HttpStatusCode.OK, json));
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            await Assert.ThrowsAsync<MoneroRpcProtocolException>(() => client.GetBlockCountAsync());
        }

        [Fact]
        public async Task WrongJsonRpcVersion_ThrowsMoneroRpcProtocolException()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct).ConfigureAwait(false);
                string? id = doc.RootElement.GetProperty("id").GetString();
                string json = $"{{\"jsonrpc\":\"1.0\",\"id\":\"{id}\",\"result\":{{\"count\":1,\"status\":\"OK\",\"untrusted\":false}}}}";
                return HttpTestHelpers.Json(HttpStatusCode.OK, json);
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            await Assert.ThrowsAsync<MoneroRpcProtocolException>(() => client.GetBlockCountAsync());
        }

        [Fact]
        public async Task NumericJsonRpcId_IsAccepted_WhenRequestIdIsNumericString()
        {
            var options = new MoneroRpcClientOptions
            {
                RequestIdFactory = static () => "1",
            };

            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                string json = "{\"jsonrpc\":\"2.0\",\"id\":1,\"result\":{\"count\":1,\"status\":\"OK\",\"untrusted\":false}}";
                return Task.FromResult(HttpTestHelpers.Json(HttpStatusCode.OK, json));
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"), options);

            ulong count = await client.GetBlockCountAsync();
            Assert.Equal(1ul, count);
        }

        [Fact]
        public async Task MismatchedJsonRpcId_ThrowsMoneroRpcProtocolException()
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                string json = "{\"jsonrpc\":\"2.0\",\"id\":\"1\",\"result\":{\"count\":1,\"status\":\"OK\",\"untrusted\":false}}";
                return Task.FromResult(HttpTestHelpers.Json(HttpStatusCode.OK, json));
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            await Assert.ThrowsAsync<MoneroRpcProtocolException>(() => client.GetBlockCountAsync());
        }

        [Fact]
        public async Task ExecuteRawJsonRpcAsync_ReturnsJsonRpcErrorWithoutThrowing()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal("json_rpc", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct).ConfigureAwait(false);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                Assert.Equal("get_info", doc.RootElement.GetProperty("method").GetString());

                string json = $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"error\":{{\"code\":-32601,\"message\":\"Method not found\"}}}}";
                return HttpTestHelpers.Json(HttpStatusCode.OK, json);
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            RawJsonRpcExecutionResult result = await client.ExecuteRawJsonRpcAsync("get_info", "{}");
            Assert.False(result.IsSuccess);
            Assert.Equal(-32601, result.ErrorCode);
            Assert.Equal("Method not found", result.ErrorMessage);
            Assert.Contains("\"method\": \"get_info\"", result.RequestJson, StringComparison.Ordinal);
            Assert.Contains("\"error\": {", result.ResponseJson, StringComparison.Ordinal);
        }

        [Fact]
        public async Task ExecuteRawJsonRpcAsync_PrettyPrintsValidJsonParamsAndResponse()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal("json_rpc", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct).ConfigureAwait(false);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                Assert.Equal("get_info", doc.RootElement.GetProperty("method").GetString());

                string json = $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":{{\"status\":\"OK\",\"height\":321}}}}";
                return HttpTestHelpers.Json(HttpStatusCode.OK, json);
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            RawJsonRpcExecutionResult result = await client.ExecuteRawJsonRpcAsync(
                "get_info",
                "{\"foo\":42,\"nested\":{\"bar\":true}}");

            Assert.True(result.IsSuccess);
            Assert.Contains("\"foo\": 42", result.ParamsJson, StringComparison.Ordinal);
            Assert.Contains("\"nested\": {", result.ParamsJson, StringComparison.Ordinal);
            Assert.Contains("\"result\": {", result.ResponseJson, StringComparison.Ordinal);
        }

        [Fact]
        public async Task ExecuteRawJsonRpcAsync_HttpError_ReturnsFailureResult()
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                return Task.FromResult(HttpTestHelpers.Text(HttpStatusCode.BadGateway, "gateway down"));
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            RawJsonRpcExecutionResult result = await client.ExecuteRawJsonRpcAsync("get_info");
            Assert.False(result.IsSuccess);
            Assert.Equal("HTTP 502 (BadGateway)", result.ErrorMessage);
            Assert.Equal("gateway down", result.ResponseJson);
        }

        [Theory]
        [InlineData("x")]
        [InlineData("{]")]
        public async Task ExecuteRawJsonRpcAsync_MalformedParams_ThrowsArgumentException(string malformedParams)
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                return Task.FromResult(HttpTestHelpers.Json(HttpStatusCode.OK, "{}"));
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(() => client.ExecuteRawJsonRpcAsync("get_info", malformedParams));
            Assert.Equal("paramsJson", ex.ParamName);
            Assert.IsAssignableFrom<JsonException>(ex.InnerException);
        }

        [Fact]
        public async Task ExecuteRawJsonRpcAsync_InvalidTopLevelParamsType_ThrowsArgumentException()
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                return Task.FromResult(HttpTestHelpers.Json(HttpStatusCode.OK, "{}"));
            });

            using var httpClient = new HttpClient(handler);
            MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(() => client.ExecuteRawJsonRpcAsync("get_info", "42"));
            Assert.Equal("paramsJson", ex.ParamName);
        }
    }
}



