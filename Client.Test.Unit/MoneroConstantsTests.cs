using System;
using BibXmr.Client.Network;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>MoneroConstants</c> behavior.
    /// </summary>
    public class MoneroConstantsTests
    {
        [Fact]
        public void GetDaemonPort_Mainnet_Returns18081()
        {
            Assert.Equal(18081u, MoneroConstants.GetDaemonPort(MoneroNetwork.Mainnet));
        }

        [Fact]
        public void GetDaemonPort_Testnet_Returns28081()
        {
            Assert.Equal(28081u, MoneroConstants.GetDaemonPort(MoneroNetwork.Testnet));
        }

        [Fact]
        public void GetDaemonPort_Stagenet_Returns38081()
        {
            Assert.Equal(38081u, MoneroConstants.GetDaemonPort(MoneroNetwork.Stagenet));
        }

        [Fact]
        public void GetWalletPort_Mainnet_Returns18082()
        {
            Assert.Equal(18082u, MoneroConstants.GetWalletPort(MoneroNetwork.Mainnet));
        }

        [Fact]
        public void GetWalletPort_Testnet_Returns28082()
        {
            Assert.Equal(28082u, MoneroConstants.GetWalletPort(MoneroNetwork.Testnet));
        }

        [Fact]
        public void GetWalletPort_Stagenet_Returns38082()
        {
            Assert.Equal(38082u, MoneroConstants.GetWalletPort(MoneroNetwork.Stagenet));
        }

        [Fact]
        public void GetDaemonPort_InvalidNetwork_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => MoneroConstants.GetDaemonPort((MoneroNetwork)99));
        }

        [Fact]
        public void GetWalletPort_InvalidNetwork_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => MoneroConstants.GetWalletPort((MoneroNetwork)99));
        }

        [Fact]
        public void AverageBlockTime_IsTwoMinutes()
        {
            Assert.Equal(TimeSpan.FromMinutes(2), MoneroConstants.AverageBlockTime);
        }

        [Fact]
        public void CoinbaseUnlockBlocks_Is60()
        {
            Assert.Equal(60, MoneroConstants.CoinbaseUnlockBlocks);
        }

        [Fact]
        public void DefaultHost_IsLocalhost()
        {
            Assert.Equal("127.0.0.1", MoneroConstants.DefaultHost);
        }
    }
}

