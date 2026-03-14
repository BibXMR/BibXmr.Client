using System;
using BibXmr.Client.Network;
using BibXmr.Client.Network.Exceptions;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>ExceptionCoverage</c> behavior.
    /// </summary>
    public class ExceptionCoverageTests
    {
        /// <summary>
        /// Provides a concrete MoneroRpcException type for constructor coverage tests.
        /// </summary>
        private sealed class TestMoneroRpcException : MoneroRpcException
        {
            public TestMoneroRpcException(string message, Exception? inner, string? rpcMethod, string? requestId, Uri? requestUri)
                : base(message, inner, rpcMethod, requestId, requestUri)
            {
            }
        }

        [Fact]
        public void JsonRpcException_Constructors_AndToString_AreCovered()
        {
            _ = new JsonRpcException().ToString();
            _ = new JsonRpcException("m").ToString();
            _ = new JsonRpcException("m", new InvalidOperationException("x")).ToString();
            Assert.Equal(JsonRpcErrorCode.MethodNotFound, new JsonRpcException("m", JsonRpcErrorCode.MethodNotFound).JsonRpcErrorCode);
        }

        [Fact]
        public void MoneroRpcException_MessageAndInner_Constructors()
        {
            var inner = new InvalidOperationException("x");
            var ex1 = new MoneroRpcException("m");
            var ex2 = new MoneroRpcException("m", inner);
            Assert.Equal("m", ex1.Message);
            Assert.Same(inner, ex2.InnerException);
        }

        [Fact]
        public void MoneroRpcException_ProtectedCtor_SetsContext()
        {
            var uri = new Uri("http://localhost/");
            var ex = new TestMoneroRpcException("m", null, "rpc", "0", uri);
            Assert.Equal("rpc", ex.RpcMethod);
            Assert.Equal("0", ex.RequestId);
            Assert.Equal(uri, ex.RequestUri);
        }

        [Fact]
        public void Error_JsonRpcErrorCode_ServerErrorRange_IsDetected()
        {
            var e = new Error { Code = -32001 };
            Assert.Equal(JsonRpcErrorCode.ServerError, e.JsonRpcErrorCode);
        }
    }
}



