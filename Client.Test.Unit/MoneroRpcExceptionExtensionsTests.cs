using System;
using System.Net;
using BibXmr.Client.Network;
using BibXmr.Client.Network.Exceptions;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>MoneroRpcExceptionExtensions</c> behavior.
    /// </summary>
    public class MoneroRpcExceptionExtensionsTests
    {
        [Fact]
        public void IsMethodNotFound_WithRemoteException_ReturnsTrue()
        {
            var ex = new MoneroRpcRemoteException(
                code: -32601,
                remoteMessage: "Method not found",
                data: null,
                message: "Remote error");

            Assert.True(ex.IsMethodNotFound());
        }

        [Fact]
        public void IsMethodNotFound_WithDifferentCode_ReturnsFalse()
        {
            var ex = new MoneroRpcRemoteException(
                code: -32600,
                remoteMessage: "Invalid request",
                data: null,
                message: "Remote error");

            Assert.False(ex.IsMethodNotFound());
        }

        [Fact]
        public void IsMethodNotFound_WithWrappedException_WalksChain()
        {
            var inner = new MoneroRpcRemoteException(
                code: -32601,
                remoteMessage: "Method not found",
                data: null,
                message: "Inner");
            var outer = new Exception("Wrapper", inner);

            Assert.True(outer.IsMethodNotFound());
        }

        [Fact]
        public void IsRestricted_WithHttp403_ReturnsTrue()
        {
            var ex = new MoneroRpcHttpException(
                HttpStatusCode.Forbidden,
                responseBodySnippet: null,
                message: "403 Forbidden");

            Assert.True(ex.IsRestricted());
        }

        [Fact]
        public void IsRestricted_WithRestrictedMessage_ReturnsTrue()
        {
            var ex = new Exception("This endpoint is restricted");

            Assert.True(ex.IsRestricted());
        }

        [Fact]
        public void IsRestricted_WithUnrelatedError_ReturnsFalse()
        {
            var ex = new Exception("Connection refused");

            Assert.False(ex.IsRestricted());
        }

        [Fact]
        public void IsAuthenticationRequired_WithHttp401_ReturnsTrue()
        {
            var ex = new MoneroRpcHttpException(
                HttpStatusCode.Unauthorized,
                responseBodySnippet: null,
                message: "401 Unauthorized");

            Assert.True(ex.IsAuthenticationRequired());
        }

        [Fact]
        public void IsAuthenticationRequired_WithOtherCode_ReturnsFalse()
        {
            var ex = new MoneroRpcHttpException(
                HttpStatusCode.InternalServerError,
                responseBodySnippet: null,
                message: "500");

            Assert.False(ex.IsAuthenticationRequired());
        }

        [Fact]
        public void GetJsonRpcErrorCode_WithRemoteException_ReturnsCode()
        {
            var ex = new MoneroRpcRemoteException(
                code: -32601,
                remoteMessage: "Method not found",
                data: null,
                message: "Remote error");

            Assert.Equal(JsonRpcErrorCode.MethodNotFound, ex.GetJsonRpcErrorCode());
        }

        [Fact]
        public void GetJsonRpcErrorCode_WithNoRpcException_ReturnsNull()
        {
            var ex = new Exception("Some error");

            Assert.Null(ex.GetJsonRpcErrorCode());
        }
    }
}

