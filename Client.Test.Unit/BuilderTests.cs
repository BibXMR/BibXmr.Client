using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BibXmr.Client.Wallet;
using BibXmr.Client.Wallet.Dto;
using BibXmr.Client.Wallet.Dto.Responses;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>Builder</c> behavior.
    /// </summary>
    public class BuilderTests
    {
        [Fact]
        public void WalletAndTxBuilders_ApplyFluentValues()
        {
            MoneroWalletConfig walletConfig = new MoneroWalletConfigBuilder()
                .WithFilename("w.bin")
                .WithPassword("pw")
                .WithLanguage("English")
                .WithSeed("seed")
                .WithSeedOffset("offset")
                .WithPrivateViewKey("view")
                .WithPrivateSpendKey("spend")
                .WithRestoreHeight(10)
                .WithEnableMultisigExperimental(true)
                .Build();

            Assert.Equal("w.bin", walletConfig.Filename);
            Assert.Equal("pw", walletConfig.Password);
            Assert.Equal((uint)10, walletConfig.RestoreHeight);
            Assert.True(walletConfig.EnableMultisigExperimental);

            MoneroTxConfig txConfig = new MoneroTxConfigBuilder()
                .AddDestination("44dest", 123)
                .WithPaymentId("pid")
                .WithPriority(TransferPriority.Elevated)
                .WithAccountIndex(2)
                .WithSubaddressIndices([3, 4])
                .WithRelay(false)
                .WithUnlockTime(55)
                .Build();

            Assert.Single(txConfig.Destinations);
            Assert.Equal("44dest", txConfig.Destinations[0].Address);
            Assert.Equal((ulong)123, txConfig.Destinations[0].Amount);
            Assert.Equal(TransferPriority.Elevated, txConfig.Priority);
            Assert.False(txConfig.Relay);
            Assert.Equal((ulong)55, txConfig.UnlockTime);
        }

        [Fact]
        public async Task BuilderBasedWalletAndTxApis_MapToExpectedRequestPayloads()
        {
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                using JsonDocument doc = await HttpTestHelpers.ReadJsonAsync(req, ct);
                string id = doc.RootElement.GetProperty("id").GetString()!;
                string? method = doc.RootElement.GetProperty("method").GetString();
                JsonElement p = doc.RootElement.GetProperty("params");

                switch (method)
                {
                    case "open_wallet":
                        return JsonRpcOk(id);
                    case "create_wallet":
                        Assert.Equal("new-wallet", p.GetProperty("filename").GetString());
                        Assert.Equal("English", p.GetProperty("language").GetString()); // default language from config overload
                        Assert.Equal("pw", p.GetProperty("password").GetString());
                        return JsonRpcOk(id);
                    case "transfer":
                    case "transfer_split":
                        Assert.Equal("44dest", p.GetProperty("destinations")[0].GetProperty("address").GetString());
                        Assert.Equal((ulong)123, p.GetProperty("destinations")[0].GetProperty("amount").GetUInt64());
                        Assert.Equal("pid", p.GetProperty("payment_id").GetString());
                        Assert.Equal((uint)3, p.GetProperty("priority").GetUInt32());
                        Assert.Equal((uint)2, p.GetProperty("account_index").GetUInt32());
                        Assert.Equal((uint)3, p.GetProperty("subaddr_indices")[0].GetUInt32());
                        Assert.True(p.GetProperty("do_not_relay").GetBoolean()); // inferred from Relay = false
                        Assert.Equal((ulong)55, p.GetProperty("unlock_time").GetUInt64());

                        if (method == "transfer")
                        {
                            return JsonRpc(
                                id,
                                "{\"amount\":1,\"fee\":1,\"multisig_txset\":\"\",\"tx_blob\":\"\",\"tx_hash\":\"h\",\"tx_key\":\"k\",\"tx_metadata\":\"m\",\"unsigned_txset\":\"\",\"weight\":1}");
                        }

                        return JsonRpc(
                            id,
                            "{\"tx_hash_list\":[\"h\"],\"tx_key_list\":[\"k\"],\"amount_list\":[1],\"fee_list\":[1],\"tx_metadata_list\":[\"m\"],\"multisig_txset\":\"\",\"unsigned_txset\":\"\",\"weight_list\":[1]}");
                    case "store":
                    case "close_wallet":
                        return JsonRpcOk(id);
                    default:
                        throw new InvalidOperationException($"Unexpected method '{method}'.");
                }
            });

            using var httpClient = new HttpClient(handler);
            using MoneroWalletClient client = await MoneroWalletClient.CreateAsync(httpClient, new Uri("http://localhost:18082/"), "wallet.bin", "pw");

            MoneroWalletConfig walletConfig = new MoneroWalletConfigBuilder()
                .WithFilename("new-wallet")
                .WithPassword("pw")
                .Build();

            _ = await client.CreateWalletAsync(walletConfig);
            _ = await client.OpenWalletAsync(walletConfig);

            MoneroTxConfig txConfig = new MoneroTxConfigBuilder()
                .AddDestination("44dest", 123)
                .WithPaymentId("pid")
                .WithPriority(TransferPriority.Elevated)
                .WithAccountIndex(2)
                .WithSubaddressIndices([3, 4])
                .WithRelay(false)
                .WithUnlockTime(55)
                .Build();

            FundTransfer tx = await client.TransferAsync(txConfig);
            Assert.Equal("h", tx.TransactionHash);

            SplitFundTransfer split = await client.TransferSplitAsync(txConfig);
            Assert.Single(split.TransactionHashes);
        }

        private static HttpResponseMessage JsonRpcOk(string id) => JsonRpc(id, "{}");

        private static HttpResponseMessage JsonRpc(string id, string resultJson)
        {
            string json = $"{{\"jsonrpc\":\"2.0\",\"id\":\"{id}\",\"result\":{resultJson}}}";
            return HttpTestHelpers.Json(HttpStatusCode.OK, json);
        }
    }
}



