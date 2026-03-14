using System;
using System.Text.Json;
using BibXmr.Client.Network;
using BibXmr.Client.Network.Exceptions;
using BibXmr.Client.Utilities;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>ErrorGuard</c> behavior.
    /// </summary>
    public class ErrorGuardTests
    {
        [Fact]
        public void ThrowIfWalletNotOpen_WhenNotOpen_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => ErrorGuard.ThrowIfWalletNotOpen(isWalletOpen: false, "f"));
        }

        [Fact]
        public void ThrowIfWalletNotOpen_WhenOpen_DoesNotThrow()
        {
            ErrorGuard.ThrowIfWalletNotOpen(isWalletOpen: true, "f");
        }

        [Fact]
        public void ThrowIfWalletOpen_WhenOpen_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => ErrorGuard.ThrowIfWalletOpen(isWalletOpen: true, "f"));
        }

        [Fact]
        public void ThrowIfWalletOpen_WhenNotOpen_DoesNotThrow()
        {
            ErrorGuard.ThrowIfWalletOpen(isWalletOpen: false, "f");
        }

        [Fact]
        public void ThrowIfResultIsNull_BoolVariant_ThrowsAndDoesNotThrow()
        {
            Assert.Throws<MoneroRpcProtocolException>(() => ErrorGuard.ThrowIfResultIsNull(resultIsNull: true, "f"));
            ErrorGuard.ThrowIfResultIsNull(resultIsNull: false, "f");
        }

        [Fact]
        public void ThrowIfResultIsNull_RpcResponseVariant_ThrowsOnNullOrMalformed()
        {
            Assert.Throws<MoneroRpcProtocolException>(() => ErrorGuard.ThrowIfResultIsNull((RpcResponse)null!, "f"));

            var malformed = new RpcResponse { Id = null, JsonRpc = null };
            Assert.Throws<MoneroRpcProtocolException>(() => ErrorGuard.ThrowIfResultIsNull(malformed, "f"));
        }

        [Fact]
        public void ThrowIfResultIsNull_RpcResponseVariant_ThrowsOnRemoteError()
        {
            using var doc = JsonDocument.Parse("{\"x\":1}");
            var resp = new RpcResponse
            {
                Id = "0",
                JsonRpc = "2.0",
                Error = new Error
                {
                    Code = -32601,
                    Message = "Method not found",
                    Data = doc.RootElement,
                }
            };

            Assert.Throws<JsonRpcException>(() => ErrorGuard.ThrowIfResultIsNull(resp, "f"));
        }

        [Fact]
        public void ThrowIfResultIsNull_RpcResponseVariant_DoesNotThrowOnOkResponse()
        {
            var resp = new RpcResponse { Id = "0", JsonRpc = "2.0", Error = null };
            ErrorGuard.ThrowIfResultIsNull(resp, "f");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ThrowIfNullOrWhiteSpace_Throws(string? s)
        {
            Assert.Throws<InvalidOperationException>(() => ErrorGuard.ThrowIfNullOrWhiteSpace(s, "p"));
        }

        [Fact]
        public void ThrowIfNullOrWhiteSpace_NonEmpty_DoesNotThrow()
        {
            ErrorGuard.ThrowIfNullOrWhiteSpace("x", "p");
        }

        [Fact]
        public void ThrowIfUnlockTimeIsTooLarge_ThrowsAndDoesNotThrow()
        {
            ErrorGuard.ThrowIfUnlockTimeIsToolarge(ErrorThresholds.MaximumUnlockTime, "u");
            Assert.Throws<InvalidOperationException>(() => ErrorGuard.ThrowIfUnlockTimeIsToolarge(ErrorThresholds.MaximumUnlockTime + 1, "u"));
        }
    }
}



