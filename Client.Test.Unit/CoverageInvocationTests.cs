using System;
using System.Net.Http;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;
using BibXmr.Client.Network;
using BibXmr.Client.Wallet;
using BibXmr.Client.Test.Unit.CoverageHarness;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>CoverageInvocation</c> behavior.
    /// </summary>
    public class CoverageInvocationTests
    {
        [Fact]
        public async Task RpcCommunicator_PublicApi_IsInvokable_WithUniversalResponses()
        {
            RecordingHttpMessageHandler handler = UniversalRpcServer.CreateHandler();
            using var httpClient = new HttpClient(handler);

            // RpcCommunicator is internal but visible to this test assembly.
            var rpc = new RpcCommunicator(httpClient, new Uri("http://localhost:18081/"));
            await ReflectionInvoke.InvokeAllAsync(rpc);

            rpc.Dispose();
            rpc.Dispose(); // cover double-dispose branch.
        }

        [Fact]
        public async Task DaemonClient_PublicApi_IsInvokable_WithUniversalResponses()
        {
            RecordingHttpMessageHandler handler = UniversalRpcServer.CreateHandler();
            using var httpClient = new HttpClient(handler);

            using MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));
            await ReflectionInvoke.InvokeAllAsync(client);

            client.Dispose();
            client.Dispose(); // cover double-dispose branch.
        }

        [Fact]
        public async Task WalletClient_PublicApi_IsInvokable_WithUniversalResponses()
        {
            RecordingHttpMessageHandler handler = UniversalRpcServer.CreateHandler();
            using var httpClient = new HttpClient(handler);

            using MoneroWalletClient client = await MoneroWalletClient.CreateAsync(httpClient, new Uri("http://localhost:18082/"), "wallet.bin", "pw");
            await ReflectionInvoke.InvokeAllAsync(client);

            client.Dispose();
            client.Dispose(); // cover double-dispose branch.
        }
    }
}



