using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;
using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Network.Exceptions;
using BibXmr.Client.Wallet;
using BibXmr.Client.Wallet.Dto;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>AdvancedRpcMapping</c> behavior.
    /// </summary>
    public class AdvancedRpcMappingTests
    {
        [Fact]
        public async Task WalletAdvancedMethods_MapToExpectedJsonRpcMethods()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal("json_rpc", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));

                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                string? method = doc.RootElement.GetProperty("method").GetString();
                JsonElement p = doc.RootElement.GetProperty("params");

                switch (method)
                {
                    case "open_wallet":
                        Assert.Equal("wallet.bin", p.GetProperty("filename").GetString());
                        Assert.Equal("pw", p.GetProperty("password").GetString());
                        return JsonRpcOk(id);
                    case "validate_address":
                        Assert.Equal("44addr", p.GetProperty("address").GetString());
                        Assert.True(p.GetProperty("any_net_type").GetBoolean());
                        Assert.False(p.GetProperty("allow_openalias").GetBoolean());
                        return JsonRpc(id, "{\"valid\":true,\"integrated\":false,\"subaddress\":false,\"nettype\":\"mainnet\"}");
                    case "make_integrated_address":
                        Assert.Equal("44addr", p.GetProperty("standard_address").GetString());
                        Assert.Equal("0123456789abcdef", p.GetProperty("payment_id").GetString());
                        return JsonRpc(id, "{\"integrated_address\":\"4int\",\"standard_address\":\"44addr\",\"payment_id\":\"0123456789abcdef\"}");
                    case "split_integrated_address":
                        Assert.Equal("4int", p.GetProperty("integrated_address").GetString());
                        return JsonRpc(id, "{\"integrated_address\":\"4int\",\"standard_address\":\"44addr\",\"payment_id\":\"0123456789abcdef\"}");
                    case "auto_refresh":
                        Assert.True(p.GetProperty("enable").GetBoolean());
                        Assert.Equal((uint)30, p.GetProperty("period").GetUInt32());
                        return JsonRpcOk(id);
                    case "scan_tx":
                        Assert.Equal("tx1", p.GetProperty("txids").EnumerateArray().Single().GetString());
                        return JsonRpcOk(id);
                    case "rescan_blockchain":
                        return JsonRpcOk(id);
                    case "relay_tx":
                        Assert.Equal("metadata-1", p.GetProperty("tx_metadata").GetString());
                        return JsonRpc(id, "{\"tx_hash\":\"abc123\"}");
                    case "freeze":
                    case "thaw":
                    case "frozen":
                        Assert.Equal("key-image-1", p.GetProperty("key_image").GetString());
                        if (method == "frozen")
                            return JsonRpc(id, "{\"frozen\":true}");
                        return JsonRpcOk(id);
                    case "get_default_fee_priority":
                        return JsonRpc(id, "{\"priority\":2}");
                    case "get_tx_proof":
                        Assert.Equal("txid-1", p.GetProperty("txid").GetString());
                        Assert.Equal("44dest", p.GetProperty("address").GetString());
                        Assert.Equal("memo", p.GetProperty("message").GetString());
                        return JsonRpc(id, "{\"signature\":\"proof-1\"}");
                    case "check_tx_proof":
                        Assert.Equal("sig-1", p.GetProperty("signature").GetString());
                        return JsonRpc(id, "{\"good\":true,\"in_pool\":false,\"received\":10,\"confirmations\":2}");
                    case "get_spend_proof":
                        return JsonRpc(id, "{\"signature\":\"proof-2\"}");
                    case "check_spend_proof":
                        return JsonRpc(id, "{\"good\":true}");
                    case "get_reserve_proof":
                        Assert.Equal((uint)0, p.GetProperty("account_index").GetUInt32());
                        Assert.Equal((ulong)11, p.GetProperty("amount").GetUInt64());
                        return JsonRpc(id, "{\"signature\":\"proof-3\"}");
                    case "check_reserve_proof":
                        return JsonRpc(id, "{\"good\":true,\"total\":123,\"spent\":45}");
                    case "set_daemon":
                        Assert.Equal("127.0.0.1:18081", p.GetProperty("address").GetString());
                        Assert.Equal("u", p.GetProperty("username").GetString());
                        Assert.Equal("p", p.GetProperty("password").GetString());
                        Assert.True(p.GetProperty("trusted").GetBoolean());
                        return JsonRpcOk(id);
                    case "store":
                    case "close_wallet":
                        return JsonRpcOk(id);
                    default:
                        throw new InvalidOperationException($"Unexpected wallet RPC method '{method}'.");
                }
            });

            using var httpClient = new HttpClient(handler);
            using MoneroWalletClient client = await MoneroWalletClient.CreateAsync(httpClient, new Uri("http://localhost:18082/"), "wallet.bin", "pw");

            ValidateAddress validated = await client.ValidateAddressAsync("44addr", anyNetType: true, allowOpenAlias: false);
            Assert.True(validated.Valid);

            IntegratedAddress integrated = await client.MakeIntegratedAddressAsync("44addr", "0123456789abcdef");
            Assert.Equal("4int", integrated.Address);

            _ = await client.SplitIntegratedAddressAsync("4int");
            await client.SetAutoRefreshAsync(true, 30);
            await client.ScanTransactionsAsync(new[] { "tx1" });
            await client.RescanBlockchainAsync();
            Assert.Equal("abc123", await client.RelayTransactionMetadataAsync("metadata-1"));

            await client.FreezeOutputAsync("key-image-1");
            await client.ThawOutputAsync("key-image-1");
            Assert.True(await client.IsOutputFrozenAsync("key-image-1"));
            Assert.Equal(TransferPriority.Normal, await client.GetDefaultFeePriorityAsync());

            Assert.Equal("proof-1", await client.GetTransactionProofAsync("txid-1", "44dest", "memo"));
            Assert.True((await client.CheckTransactionProofAsync("txid-1", "44dest", "memo", "sig-1")).Good);
            Assert.Equal("proof-2", await client.GetSpendProofAsync("txid-1", "memo"));
            Assert.True(await client.CheckSpendProofAsync("txid-1", "memo", "sig-2"));
            Assert.Equal("proof-3", await client.GetReserveProofAsync("memo", 0, 11));
            Assert.True((await client.CheckReserveProofAsync("44dest", "memo", "sig-3")).Good);

            await client.SetDaemonConnectionAsync(new MoneroDaemonConnection
            {
                Address = "127.0.0.1:18081",
                Username = "u",
                Password = "p",
                Trusted = true,
            });
        }

        [Fact]
        public async Task DaemonAdvancedMethods_MapToExpectedJsonAndPathEndpoints()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                string path = HttpTestHelpers.GetPathAndQuery(req.RequestUri!);
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);

                if (string.Equals(path, "json_rpc", StringComparison.OrdinalIgnoreCase))
                {
                    string id = doc.RootElement.GetProperty("id").GetString()!;
                    string? method = doc.RootElement.GetProperty("method").GetString();
                    if (method == "on_get_block_hash")
                    {
                        Assert.Equal((ulong)123, doc.RootElement.GetProperty("params").EnumerateArray().Single().GetUInt64());
                        return JsonRpc(id, "\"hash-123\"");
                    }
                    else if (method == "get_info")
                    {
                        Assert.Equal(42, doc.RootElement.GetProperty("params").GetProperty("foo").GetInt32());
                        return JsonRpc(id, "{\"status\":\"OK\",\"height\":321}");
                    }
                    else if (method == "get_output_distribution")
                    {
                        return JsonRpc(id, "{\"status\":\"OK\",\"distributions\":[]}");
                    }
                    else if (method == "flush_cache")
                    {
                        return JsonRpc(id, "{\"status\":\"OK\"}");
                    }

                    throw new InvalidOperationException($"Unexpected daemon JSON-RPC method '{method}'.");
                }

                switch (path)
                {
                    case "get_alt_blocks_hashes":
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"blks_hashes\":[\"h1\",\"h2\"]}");
                    case "get_peer_list":
                        return HttpTestHelpers.Json(
                            HttpStatusCode.OK,
                            "{\"status\":\"OK\",\"white_list\":[{\"address\":\"2.2.2.2:18080\",\"port\":18080}],\"gray_list\":[]}");
                    case "get_transaction_pool_stats":
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"pool_stats\":{\"bytes_total\":1,\"bytes_min\":1,\"bytes_max\":1,\"txs_total\":1}}");
                    case "is_key_image_spent":
                        Assert.Equal(2, doc.RootElement.GetProperty("key_images").GetArrayLength());
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"spent_status\":[0,1,2]}");
                    case "get_outs":
                        Assert.True(doc.RootElement.GetProperty("get_txid").GetBoolean());
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"outs\":[{\"amount\":7,\"index\":9,\"mask\":\"m\",\"txid\":\"t\"}]}");
                    case "get_limit":
                    case "set_limit":
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"limit_down\":10,\"limit_up\":20}");
                    case "out_peers":
                        if (doc.RootElement.TryGetProperty("out_peers", out JsonElement outPeers))
                        {
                            Assert.Equal(8, outPeers.GetInt32());
                            return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\"}");
                        }

                        Assert.False(doc.RootElement.GetProperty("set").GetBoolean());
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"out_peers\":128}");
                    case "in_peers":
                        if (doc.RootElement.TryGetProperty("in_peers", out JsonElement inPeers))
                        {
                            Assert.Equal(9, inPeers.GetInt32());
                            return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\"}");
                        }

                        Assert.False(doc.RootElement.GetProperty("set").GetBoolean());
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"in_peers\":64}");
                    case "start_mining":
                        Assert.Equal("44miner", doc.RootElement.GetProperty("miner_address").GetString());
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\"}");
                    case "stop_mining":
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\"}");
                    case "mining_status":
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"active\":true,\"threads_count\":2,\"address\":\"44miner\",\"speed\":77}");
                    case "send_raw_transaction":
                        Assert.Equal("aabb", doc.RootElement.GetProperty("tx_as_hex").GetString());
                        Assert.True(doc.RootElement.GetProperty("do_not_relay").GetBoolean());
                        Assert.False(doc.RootElement.GetProperty("do_sanity_checks").GetBoolean());
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"double_spend\":false}");
                    case "update":
                        string? command = doc.RootElement.GetProperty("command").GetString();
                        if (command == "check")
                            return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"update\":true,\"version\":\"1.0\",\"hash\":\"hh\"}");
                        if (command == "download")
                            return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"path\":\"/tmp/u\"}");
                        throw new InvalidOperationException("Unexpected update command.");
                    case "stop_daemon":
                    case "save_bc":
                    case "set_log_hash_rate":
                    case "set_log_level":
                    case "set_log_categories":
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\"}");
                    case "get_net_stats":
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"total_bytes_in\":11,\"total_bytes_out\":22}");
                    case "get_public_nodes":
                        Assert.True(doc.RootElement.GetProperty("gray").GetBoolean());
                        Assert.False(doc.RootElement.GetProperty("white").GetBoolean());
                        Assert.True(doc.RootElement.GetProperty("include_blocked").GetBoolean());
                        return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"OK\",\"white\":[\"w1\"],\"gray\":[\"g1\"]}");
                    default:
                        throw new InvalidOperationException($"Unexpected daemon path endpoint '{path}'.");
                }
            });

            using var httpClient = new HttpClient(handler);
            using MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            Assert.Equal("hash-123", await client.GetBlockHashAsync(123));
            Assert.Equal(2, (await client.GetAlternateBlockHashesAsync()).Count);
            RawJsonRpcExecutionResult raw = await client.ExecuteRawJsonRpcAsync("get_info", "{\"foo\":42}");
            Assert.True(raw.IsSuccess);
            Assert.Equal("get_info", raw.Method);
            Assert.Contains("\"height\": 321", raw.ResponseJson, StringComparison.Ordinal);

            PeerList peerList = await client.GetPeerListAsync();
            Assert.NotNull(peerList);
            Assert.Single(peerList.WhiteList);
            Assert.Equal("18080", peerList.WhiteList[0].Port);
            Assert.Equal((ulong)1, (await client.GetTransactionPoolStatsAsync()).TransactionsTotal);

            List<KeyImageSpentStatus> spent = await client.GetKeyImageSpentStatusesAsync(new[] { "k1", "k2" });
            Assert.Equal(KeyImageSpentStatus.Unspent, spent[0]);
            Assert.Equal(KeyImageSpentStatus.SpentInBlockchain, spent[1]);

            List<DaemonOutput> outs = await client.GetOutputsAsync(new[] { new DaemonOutputRequest { Amount = 7, Index = 9 } }, includeTransactionId: true);
            Assert.Single(outs);

            _ = await client.GetOutputDistributionAsync(new[] { 1ul, 2ul }, cumulative: true, fromHeight: 1, toHeight: 3);

            BandwidthLimit limit = await client.GetBandwidthLimitAsync();
            Assert.Equal(10, limit.LimitDown);
            Assert.Equal(20, (await client.SetBandwidthLimitAsync(10, 20)).LimitUp);
            Assert.Equal(128, await client.GetOutgoingPeerLimitAsync());
            Assert.Equal(64, await client.GetIncomingPeerLimitAsync());

            await client.SetOutgoingPeerLimitAsync(8);
            await client.SetIncomingPeerLimitAsync(9);
            await client.StartMiningAsync("44miner", 2, backgroundMining: true, ignoreBattery: false);
            await client.StopMiningAsync();
            Assert.True((await client.GetMiningStatusAsync()).Active);

            Assert.False((await client.SubmitRawTransactionAsync("aabb", doNotRelay: true, doSanityChecks: false)).DoubleSpend);
            Assert.True((await client.CheckForUpdateAsync()).UpdateAvailable);
            Assert.Equal("/tmp/u", (await client.DownloadUpdateAsync("/tmp")).Path);
            await client.StopDaemonAsync();

            NetworkStats netStats = await client.GetNetworkStatsAsync();
            Assert.Equal((ulong)11, netStats.TotalBytesIn);

            PublicNodes nodes = await client.GetPublicNodesAsync(gray: true, white: false, includeBlocked: true);
            Assert.Single(nodes.GrayNodes);
            Assert.Single(nodes.WhiteNodes);

            await client.SaveBlockchainAsync();
            await client.SetLogHashRateAsync(true);
            await client.SetLogLevelAsync(1);
            await client.SetLogCategoriesAsync("net");
            await client.FlushCacheAsync(true, true);
        }

        [Fact]
        public async Task SubmitRawTransactionAsync_NonOkStatus_ThrowsMoneroRpcProtocolException()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                Assert.Equal("send_raw_transaction", HttpTestHelpers.GetPathAndQuery(req.RequestUri!));

                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                Assert.Equal("beef", doc.RootElement.GetProperty("tx_as_hex").GetString());
                Assert.False(doc.RootElement.GetProperty("do_not_relay").GetBoolean());
                Assert.True(doc.RootElement.GetProperty("do_sanity_checks").GetBoolean());

                return HttpTestHelpers.Json(HttpStatusCode.OK, "{\"status\":\"BUSY\",\"double_spend\":false}");
            });

            using var httpClient = new HttpClient(handler);
            using MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            MoneroRpcProtocolException ex = await Assert.ThrowsAsync<MoneroRpcProtocolException>(
                () => client.SubmitRawTransactionAsync("beef", doNotRelay: false, doSanityChecks: true));

            Assert.Contains("send_raw_transaction", ex.Message, StringComparison.Ordinal);
            Assert.Contains("BUSY", ex.Message, StringComparison.Ordinal);
        }

        private static HttpResponseMessage JsonRpcOk(string id) => JsonRpc(id, "{}");

        private static HttpResponseMessage JsonRpc(string id, string resultJson)
        {
            string json = $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":{resultJson}}}";
            return HttpTestHelpers.Json(HttpStatusCode.OK, json);
        }
    }
}



