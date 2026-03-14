using System;
using BibXmr.Client.Daemon.Dto.Responses;
using BibXmr.Client.Network;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>DaemonInformation</c> behavior.
    /// </summary>
    public class DaemonInformationTests
    {
        [Fact]
        public void NetworkType_WhenFlagsUnset_Throws()
        {
            var info = new DaemonInformation();
            Assert.Throws<InvalidOperationException>(() => _ = info.NetworkType);
        }

        [Fact]
        public void NetworkType_WhenMainnet_ReturnsMainnet_AndToStringDoesNotThrow()
        {
            var info = new DaemonInformation
            {
                IsMainnet = true,
            };

            Assert.Equal(MoneroNetwork.Mainnet, info.NetworkType);
            _ = info.ToString();
        }

        [Fact]
        public void NetworkType_WhenStagenet_ReturnsStagenet()
        {
            var info = new DaemonInformation
            {
                IsStagenet = true,
            };

            Assert.Equal(MoneroNetwork.Stagenet, info.NetworkType);
        }

        [Fact]
        public void NetworkType_WhenTestnet_ReturnsTestnet()
        {
            var info = new DaemonInformation
            {
                IsTestnet = true,
            };

            Assert.Equal(MoneroNetwork.Testnet, info.NetworkType);
        }
    }
}



