using BibXmr.Client.Daemon.Dto;
using BibXmr.Client.Daemon.Dto.Responses;
using BibXmr.Client.Test.EndToEnd.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.EndToEnd;

/// <summary>
/// Live daemon connectivity and information retrieval coverage.
/// </summary>
[Collection(E2ECollectionNames.Daemon)]
public sealed class DaemonConnectivityE2ETests
{
    private readonly MoneroE2EFixture _fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="DaemonConnectivityE2ETests"/> class.
    /// </summary>
    /// <param name="fixture">The shared E2E fixture.</param>
    public DaemonConnectivityE2ETests(MoneroE2EFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that basic daemon connectivity succeeds.
    /// </summary>
    [Fact]
    public async Task BasicConnectivity_GetBlockCount_Succeeds()
    {
        await using DaemonClientSession session = await _fixture.CreateDirectSessionAsync();

        ulong blockCount = await session.Client.GetBlockCountAsync();

        Assert.True(blockCount > 0);
    }

    /// <summary>
    /// Verifies that daemon network info can be retrieved.
    /// </summary>
    [Fact]
    public async Task NetworkInfo_GetDaemonInformation_Succeeds()
    {
        await using DaemonClientSession session = await _fixture.CreateDirectSessionAsync();

        DaemonInformation info = await session.Client.GetDaemonInformationAsync();

        Assert.NotNull(info);
        Assert.Equal("OK", info.Status);
        Assert.True(info.Height > 0);
        Assert.False(string.IsNullOrWhiteSpace(info.Version));
    }

    /// <summary>
    /// Verifies blockchain height retrieval returns a positive value.
    /// </summary>
    [Fact]
    public async Task BlockchainHeight_GetBlockCount_IsPositive()
    {
        await using DaemonClientSession session = await _fixture.CreateDirectSessionAsync();

        ulong blockCount = await session.Client.GetBlockCountAsync();

        Assert.True(blockCount > 0);
    }

    /// <summary>
    /// Verifies fee estimation calls return a valid fee structure.
    /// </summary>
    [Fact]
    public async Task FeeEstimation_GetFeeEstimateParameters_Succeeds()
    {
        await using DaemonClientSession session = await _fixture.CreateDirectSessionAsync();

        FeeEstimate feeEstimate = await session.Client.GetFeeEstimateParametersAsync(graceBlocks: 10);

        Assert.NotNull(feeEstimate);
        Assert.Equal("OK", feeEstimate.Status);
        Assert.True(feeEstimate.Fee > 0);
        Assert.True(feeEstimate.QuantizationMask > 0);
    }

    /// <summary>
    /// Verifies peer list retrieval succeeds.
    /// </summary>
    [Fact]
    public async Task PeerList_GetConnections_Succeeds()
    {
        await using DaemonClientSession session = await _fixture.CreateDirectSessionAsync();

        List<Connection> connections = await session.Client.GetConnectionsAsync();

        Assert.NotNull(connections);
        Assert.All(connections.Take(5), connection => Assert.False(string.IsNullOrWhiteSpace(connection.Address)));
    }
}


