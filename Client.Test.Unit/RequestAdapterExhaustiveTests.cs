using System.Text.Json;
using BibXmr.Client.Network;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>RequestAdapterExhaustive</c> behavior.
    /// </summary>
    public class RequestAdapterExhaustiveTests
    {
        [Fact]
        public void MoneroRequestAdapter_BaseAddressValidation()
        {
            Assert.Throws<ArgumentNullException>(() => new MoneroRequestAdapter((Uri)null!));
            Assert.Throws<ArgumentException>(() => new MoneroRequestAdapter(new Uri("/relative", UriKind.Relative)));
        }

        [Theory]
        [InlineData("http://localhost:18081/")]
        [InlineData("http://127.0.0.1:18081/")]
        [InlineData("http://[::1]:18081/")]
        [InlineData("http://daemon.localhost:18081/")]
        public void MoneroRequestAdapter_AllowsLoopbackHttpByDefault(string address)
        {
            var adapter = new MoneroRequestAdapter(new Uri(address, UriKind.Absolute));
            using HttpRequestMessage request = adapter.CreateJsonRpcRequestMessage("get_block_count", new GenericRequestParameters());
            Assert.Equal("http", request.RequestUri!.Scheme);
        }

        [Fact]
        public void MoneroRequestAdapter_BlocksNonLoopbackHttpByDefault()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(() => new MoneroRequestAdapter(new Uri("http://example.org:18081/", UriKind.Absolute)));
            Assert.Contains("Clear-text HTTP is blocked for non-loopback endpoints", ex.Message, StringComparison.Ordinal);
        }

        [Fact]
        public void MoneroRequestAdapter_AllowsNonLoopbackHttpsByDefault()
        {
            var adapter = new MoneroRequestAdapter(new Uri("https://example.org:18081/", UriKind.Absolute));
            using HttpRequestMessage request = adapter.CreateJsonRpcRequestMessage("get_block_count", new GenericRequestParameters());
            Assert.Equal("https", request.RequestUri!.Scheme);
        }

        [Fact]
        public void MoneroRequestAdapter_JsonRpcBuilder_RequiresMethod()
        {
            var adapter = new MoneroRequestAdapter(new Uri("http://localhost:18081/", UriKind.Absolute));

            Assert.Throws<ArgumentException>(() => adapter.CreateJsonRpcRequestMessage("", new GenericRequestParameters()));
            Assert.Throws<ArgumentException>(() => adapter.CreateJsonRpcRequestMessage("   ", new GenericRequestParameters()));
        }

        [Fact]
        public async Task MoneroRequestAdapter_UrlPortCtor_BuildsAbsoluteBaseAddress()
        {
            var adapter = new MoneroRequestAdapter("localhost", 18081);
            using HttpRequestMessage msg = adapter.CreateJsonRpcRequestMessage("get_block_count", new GenericRequestParameters());

            Assert.NotNull(msg.RequestUri);
            Assert.True(msg.RequestUri!.IsAbsoluteUri);
            Assert.Equal("http", msg.RequestUri.Scheme);

            using JsonDocument doc = await Infrastructure.HttpTestHelpers.ReadJsonAsync(msg, CancellationToken.None);
            Assert.Equal("get_block_count", doc.RootElement.GetProperty("method").GetString());
        }

        [Fact]
        public async Task MoneroRequestAdapter_JsonRpcRequestBuilder_UsesRequestIdFactory()
        {
            var adapter = new MoneroRequestAdapter(new Uri("http://localhost:18081/", UriKind.Absolute), () => "custom-id");
            using HttpRequestMessage msg = adapter.CreateJsonRpcRequestMessage("get_block_count", new GenericRequestParameters());

            Assert.Equal("json_rpc", msg.RequestUri!.PathAndQuery.TrimStart('/'));
            Assert.Equal("custom-id", msg.GetMoneroRpcRequestId());

            using JsonDocument doc = await Infrastructure.HttpTestHelpers.ReadJsonAsync(msg, CancellationToken.None);
            Assert.Equal("custom-id", doc.RootElement.GetProperty("id").GetString());
            Assert.Equal("get_block_count", doc.RootElement.GetProperty("method").GetString());
            Assert.True(doc.RootElement.TryGetProperty("params", out _));
        }

        [Fact]
        public async Task MoneroRequestAdapter_PathJsonAndBinaryBuilders_UseExpectedTransport()
        {
            var adapter = new MoneroRequestAdapter(new Uri("http://localhost:18081/", UriKind.Absolute));

            using HttpRequestMessage jsonMessage = adapter.CreatePathJsonRequestMessage("get_peer_list", new GenericRequestParameters { Count = 1 });
            Assert.Equal("get_peer_list", jsonMessage.RequestUri!.PathAndQuery.TrimStart('/'));
            Assert.Null(jsonMessage.GetMoneroRpcRequestId());
            using (JsonDocument jsonDoc = await Infrastructure.HttpTestHelpers.ReadJsonAsync(jsonMessage, CancellationToken.None))
                Assert.Equal((uint)1, jsonDoc.RootElement.GetProperty("count").GetUInt32());

            using HttpRequestMessage binaryMessage = adapter.CreatePathBinaryRequestMessage("get_blocks.bin", new byte[] { 1, 2, 3 });
            Assert.Equal("get_blocks.bin", binaryMessage.RequestUri!.PathAndQuery.TrimStart('/'));
            Assert.Equal("application/octet-stream", binaryMessage.Content!.Headers.ContentType!.MediaType);
            byte[] bytes = await binaryMessage.Content.ReadAsByteArrayAsync();
            Assert.Equal(new byte[] { 1, 2, 3 }, bytes);
        }
    }
}

