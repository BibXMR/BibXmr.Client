using System;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;
using BibXmr.Client.Network;
using BibXmr.Client.Wallet;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>ClientOverloadCoverage</c> behavior.
    /// </summary>
    public class ClientOverloadCoverageTests
    {
        [Fact]
        public async Task DaemonClient_CreateAsync_UrlPort_AndNetwork_DoNotThrowOnInit()
        {
            using MoneroDaemonClient c1 = await MoneroDaemonClient.CreateAsync("localhost", 18081);
            using MoneroDaemonClient c2 = await MoneroDaemonClient.CreateAsync(MoneroNetwork.Mainnet);

            c1.Dispose();
            c2.Dispose();
        }

        [Fact]
        public async Task WalletClient_CreateAsync_UrlPort_AndNetwork_CanBeCancelledImmediately()
        {
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                MoneroWalletClient.CreateAsync("localhost", 18082, "wallet.bin", "pw", cts.Token));

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                MoneroWalletClient.CreateAsync(MoneroNetwork.Mainnet, "wallet.bin", "pw", cts.Token));
        }
    }
}


