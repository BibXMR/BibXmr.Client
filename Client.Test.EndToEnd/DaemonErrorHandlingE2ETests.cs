using System.Net;
using BibXmr.Client.Network;
using BibXmr.Client.Network.Exceptions;
using BibXmr.Client.Test.EndToEnd.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.EndToEnd;

/// <summary>
/// Live daemon error handling and protocol resilience coverage.
/// </summary>
[Collection(E2ECollectionNames.Daemon)]
public sealed class DaemonErrorHandlingE2ETests
{
    private readonly MoneroE2EFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="DaemonErrorHandlingE2ETests"/> class.
    /// </summary>
    /// <param name="fixture">The shared E2E fixture.</param>
    public DaemonErrorHandlingE2ETests(MoneroE2EFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies invalid JSON-RPC method requests are surfaced as remote JSON-RPC errors.
    /// </summary>
    [Fact]
    public async Task InvalidRequest_RewrittenMethod_ThrowsJsonRpcExceptionMethodNotFound()
    {
        await using DaemonClientSession session = await _fixture
            .CreateProxySessionAsync(ProxyModes.RewriteInvalidMethod)
            ;

        JsonRpcException exception = await Assert.ThrowsAsync<JsonRpcException>(() => session.Client.GetBlockCountAsync());

        Assert.Equal(-32601, exception.Code);
    }

    /// <summary>
    /// Verifies request timeout handling when the daemon is deliberately slow.
    /// </summary>
    [Fact]
    public async Task Timeout_SlowProxyAndShortClientTimeout_ThrowsTaskCanceledException()
    {
        await using DaemonClientSession session = await _fixture
            .CreateProxySessionAsync(
                ProxyModes.Slow,
                timeout: TimeSpan.FromMilliseconds(200))
            ;

        await Assert.ThrowsAsync<TaskCanceledException>(() => session.Client.GetBlockCountAsync());
    }

    /// <summary>
    /// Verifies malformed JSON payloads are surfaced as protocol exceptions.
    /// </summary>
    [Fact]
    public async Task MalformedRpcResponse_ThrowsMoneroRpcProtocolException()
    {
        await using DaemonClientSession session = await _fixture
            .CreateProxySessionAsync(ProxyModes.MalformedJson)
            ;

        await Assert.ThrowsAsync<MoneroRpcProtocolException>(() => session.Client.GetBlockCountAsync());
    }

    /// <summary>
    /// Verifies HTTP 403 responses are surfaced as HTTP exceptions.
    /// </summary>
    [Fact]
    public async Task HttpError_403_ThrowsMoneroRpcHttpException()
    {
        await using DaemonClientSession session = await _fixture
            .CreateProxySessionAsync(ProxyModes.Status403)
            ;

        MoneroRpcHttpException exception = await Assert.ThrowsAsync<MoneroRpcHttpException>(() => session.Client.GetBlockCountAsync());

        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
    }

    /// <summary>
    /// Verifies HTTP 500 responses are surfaced as HTTP exceptions.
    /// </summary>
    [Fact]
    public async Task HttpError_500_ThrowsMoneroRpcHttpException()
    {
        await using DaemonClientSession session = await _fixture
            .CreateProxySessionAsync(ProxyModes.Status500)
            ;

        MoneroRpcHttpException exception = await Assert.ThrowsAsync<MoneroRpcHttpException>(() => session.Client.GetBlockCountAsync());

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    /// <summary>
    /// Verifies upstream daemon 404 responses are surfaced as HTTP exceptions.
    /// </summary>
    [Fact]
    public async Task HttpError_404FromUpstream_ThrowsMoneroRpcHttpException()
    {
        await using DaemonClientSession session = await _fixture
            .CreateProxySessionAsync(ProxyModes.Upstream404)
            ;

        MoneroRpcHttpException exception = await Assert.ThrowsAsync<MoneroRpcHttpException>(() => session.Client.GetBlockCountAsync());

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }
}


