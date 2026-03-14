using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;
using BibXmr.Client.Wallet;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>Listener</c> behavior.
    /// </summary>
    public class ListenerTests
    {
        [Fact]
        public async Task DaemonListener_FiresOnBlockChange_AndStopsAfterDispose()
        {
            int requestCount = 0;
            int callbackCount = 0;
            var callbackSeen = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                string hash = requestCount switch
                {
                    0 => "h1",
                    1 => "h1",
                    2 => "h2",
                    _ => "h2",
                };
                requestCount++;
                return JsonRpcBlockHeader(id, hash, (ulong)requestCount);
            });

            using var httpClient = new HttpClient(handler);
            using MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            await using IAsyncDisposable subscription = await client.SubscribeAsync(
                new MoneroDaemonListener
                {
                    OnBlockHeaderAsync = (header, ct) =>
                    {
                        if (Interlocked.Increment(ref callbackCount) == 1)
                            callbackSeen.TrySetResult(true);
                        return ValueTask.CompletedTask;
                    },
                },
                new MoneroDaemonPollingOptions { PollInterval = TimeSpan.FromMilliseconds(10) });

            await WaitOrThrowAsync(callbackSeen.Task, TimeSpan.FromSeconds(2));
            await Task.Delay(60);
            Assert.Equal(1, callbackCount);

            await subscription.DisposeAsync();
            int beforeDelay = callbackCount;
            await Task.Delay(60);
            Assert.Equal(beforeDelay, callbackCount);
        }

        [Fact]
        public async Task DaemonListener_CallbackException_Isolated()
        {
            int requestCount = 0;
            int blockCallbacks = 0;
            int errorCallbacks = 0;
            var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                string hash = requestCount switch
                {
                    0 => "h1",
                    1 => "h2",
                    2 => "h3",
                    _ => "h4",
                };
                requestCount++;
                return JsonRpcBlockHeader(id, hash, (ulong)requestCount);
            });

            using var httpClient = new HttpClient(handler);
            using MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            await using IAsyncDisposable subscription = await client.SubscribeAsync(
                new MoneroDaemonListener
                {
                    OnBlockHeaderAsync = (header, ct) =>
                    {
                        Interlocked.Increment(ref blockCallbacks);
                        throw new InvalidOperationException("listener failure");
                    },
                    OnPollingErrorAsync = (ex, ct) =>
                    {
                        if (Interlocked.Increment(ref errorCallbacks) >= 2 &&
                            Volatile.Read(ref blockCallbacks) >= 2)
                        {
                            done.TrySetResult(true);
                        }

                        return ValueTask.CompletedTask;
                    },
                },
                new MoneroDaemonPollingOptions { PollInterval = TimeSpan.FromMilliseconds(10) });

            await WaitOrThrowAsync(done.Task, TimeSpan.FromSeconds(2));
            Assert.True(blockCallbacks >= 2);
            Assert.True(errorCallbacks >= 2);
        }

        [Fact]
        public async Task WalletListener_FiresOnBlockChange_AndStopsAfterDispose()
        {
            int heightCalls = 0;
            int callbackCount = 0;
            var callbackSeen = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                string? method = doc.RootElement.GetProperty("method").GetString();

                switch (method)
                {
                    case "open_wallet":
                    case "store":
                    case "close_wallet":
                        return JsonRpcOk(id);
                    case "get_height":
                        ulong height = heightCalls switch
                        {
                            0 => 100,
                            1 => 100,
                            2 => 101,
                            _ => 101,
                        };
                        heightCalls++;
                        return JsonRpc(id, $"{{\"height\":{height}}}");
                    case "get_balance":
                        return JsonRpc(id, "{\"balance\":1000,\"blocks_to_unlock\":0,\"multisig_import_needed\":false,\"per_subaddress\":[],\"time_to_unlock\":0,\"unlocked_balance\":1000}");
                    case "incoming_transfers":
                        return JsonRpc(id, "{\"transfers\":[]}");
                    default:
                        throw new InvalidOperationException($"Unexpected wallet method '{method}'.");
                }
            });

            using var httpClient = new HttpClient(handler);
            using MoneroWalletClient client = await MoneroWalletClient.CreateAsync(httpClient, new Uri("http://localhost:18082/"), "wallet.bin", "pw");

            await using IAsyncDisposable subscription = await client.SubscribeAsync(
                new MoneroWalletListener
                {
                    OnNewBlockAsync = (height, ct) =>
                    {
                        if (Interlocked.Increment(ref callbackCount) == 1)
                            callbackSeen.TrySetResult(true);
                        return ValueTask.CompletedTask;
                    },
                },
                new MoneroWalletPollingOptions { PollInterval = TimeSpan.FromMilliseconds(10) });

            await WaitOrThrowAsync(callbackSeen.Task, TimeSpan.FromSeconds(2));
            await Task.Delay(60);
            Assert.Equal(1, callbackCount);

            await subscription.DisposeAsync();
            int beforeDelay = callbackCount;
            await Task.Delay(60);
            Assert.Equal(beforeDelay, callbackCount);
        }

        [Fact]
        public async Task WalletListener_CallbackException_Isolated()
        {
            int blockCallbacks = 0;
            int errorCallbacks = 0;
            int heightCalls = 0;
            var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                string? method = doc.RootElement.GetProperty("method").GetString();

                switch (method)
                {
                    case "open_wallet":
                    case "store":
                    case "close_wallet":
                        return JsonRpcOk(id);
                    case "get_height":
                        ulong height = heightCalls == 0 ? 100ul : 101ul;
                        heightCalls++;
                        return JsonRpc(id, $"{{\"height\":{height}}}");
                    case "get_balance":
                        return JsonRpc(id, "{\"balance\":1000,\"blocks_to_unlock\":0,\"multisig_import_needed\":false,\"per_subaddress\":[],\"time_to_unlock\":0,\"unlocked_balance\":1000}");
                    case "incoming_transfers":
                        return JsonRpc(id, "{\"transfers\":[]}");
                    default:
                        throw new InvalidOperationException($"Unexpected wallet method '{method}'.");
                }
            });

            using var httpClient = new HttpClient(handler);
            using MoneroWalletClient client = await MoneroWalletClient.CreateAsync(httpClient, new Uri("http://localhost:18082/"), "wallet.bin", "pw");

            await using IAsyncDisposable subscription = await client.SubscribeAsync(
                new MoneroWalletListener
                {
                    OnNewBlockAsync = (height, ct) =>
                    {
                        Interlocked.Increment(ref blockCallbacks);
                        throw new InvalidOperationException("listener failure");
                    },
                    OnPollingErrorAsync = (ex, ct) =>
                    {
                        if (Interlocked.Increment(ref errorCallbacks) >= 2 &&
                            Volatile.Read(ref blockCallbacks) >= 2)
                        {
                            done.TrySetResult(true);
                        }

                        return ValueTask.CompletedTask;
                    },
                },
                new MoneroWalletPollingOptions { PollInterval = TimeSpan.FromMilliseconds(10) });

            await WaitOrThrowAsync(done.Task, TimeSpan.FromSeconds(2));
            Assert.True(blockCallbacks >= 2);
            Assert.True(errorCallbacks >= 2);
        }

        private static async Task WaitOrThrowAsync(Task task, TimeSpan timeout)
        {
            Task completed = await Task.WhenAny(task, Task.Delay(timeout));
            if (!ReferenceEquals(completed, task))
                throw new TimeoutException("Timed out waiting for listener callback.");

            await task;
        }

        private static HttpResponseMessage JsonRpcOk(string id) => JsonRpc(id, "{}");

        private static HttpResponseMessage JsonRpc(string id, string resultJson)
        {
            string json = $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":{resultJson}}}";
            return HttpTestHelpers.Json(HttpStatusCode.OK, json);
        }

        private static HttpResponseMessage JsonRpcBlockHeader(string id, string hash, ulong height)
        {
            string resultJson =
                "{" +
                "\"block_header\":{" +
                "\"block_size\":0,\"block_weight\":0,\"cumulative_difficulty\":0,\"cumulative_difficulty_top64\":0," +
                "\"depth\":0,\"difficulty\":0,\"difficulty_top64\":0,\"hash\":\"" + hash + "\"," +
                "\"height\":" + height + ",\"long_term_weight\":0,\"major_version\":1,\"miner_tx_hash\":\"mtx\"," +
                "\"minor_version\":1,\"nonce\":0,\"num_txes\":0,\"orphan_status\":false,\"pow_hash\":\"pow\"," +
                "\"prev_hash\":\"prev\",\"reward\":0,\"timestamp\":0,\"wide_cumulative_difficulty\":\"0\",\"wide_difficulty\":\"0\"" +
                "}," +
                "\"status\":\"OK\",\"untrusted\":false,\"top_hash\":\"" + hash + "\",\"credits\":0" +
                "}";

            return JsonRpc(id, resultJson);
        }
    }
}


