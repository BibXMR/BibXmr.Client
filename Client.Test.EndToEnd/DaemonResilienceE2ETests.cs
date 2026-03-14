using System.Net;
using BibXmr.Client.Network.Exceptions;
using Microsoft.Extensions.Logging;
using BibXmr.Client.Test.EndToEnd.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.EndToEnd;

/// <summary>
/// Live daemon resilience and transient-failure handling coverage.
/// </summary>
[Collection(E2ECollectionNames.Daemon)]
public sealed class DaemonResilienceE2ETests
{
    private readonly MoneroE2EFixture _fixture;
    private readonly ILogger<DaemonResilienceE2ETests> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DaemonResilienceE2ETests"/> class.
    /// </summary>
    /// <param name="fixture">The shared E2E fixture.</param>
    public DaemonResilienceE2ETests(MoneroE2EFixture fixture)
    {
        _fixture = fixture;
        _logger = fixture.CreateLogger<DaemonResilienceE2ETests>();
    }

    /// <summary>
    /// Verifies retry logic can recover from one transient HTTP 503 response.
    /// </summary>
    [Fact]
    public async Task TransientUnavailable_WithRetryEventuallySucceeds()
    {
        await using DaemonClientSession session = await _fixture
            .CreateProxySessionAsync(ProxyModes.Transient503Once)
            ;

        ulong blockCount = await RetryHelper.ExecuteAsync(
                token => session.Client.GetBlockCountAsync(token),
                _fixture.Options.RetryCount,
                TimeSpan.FromMilliseconds(_fixture.Options.RetryDelayMilliseconds),
                shouldRetry: ex =>
                    ex is MoneroRpcHttpException rpcEx && rpcEx.StatusCode == HttpStatusCode.ServiceUnavailable,
                _logger)
            ;

        Assert.True(blockCount > 0);
    }

    /// <summary>
    /// Verifies connection failures are surfaced when the daemon endpoint is unavailable.
    /// </summary>
    [Fact]
    public async Task DaemonUnavailable_ClosedPort_ThrowsHttpRequestOrRpcHttpException()
    {
        await using DaemonClientSession session = await _fixture
            .CreateDirectSessionAsync(
                baseAddress: new Uri("https://127.0.0.1:65534/", UriKind.Absolute),
                timeout: TimeSpan.FromSeconds(1))
            ;

        Exception? exception = await Record.ExceptionAsync(() => session.Client.GetBlockCountAsync());

        Assert.NotNull(exception);
        Assert.True(
            exception is HttpRequestException
            || exception is TaskCanceledException
            || exception is MoneroRpcHttpException,
            $"Unexpected exception type: {exception.GetType().FullName}");
    }
}


