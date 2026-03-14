using Xunit;

namespace BibXmr.Client.Test.EndToEnd.Infrastructure;

/// <summary>
/// Shared collection names for E2E test fixtures.
/// </summary>
internal static class E2ECollectionNames
{
    /// <summary>
    /// The daemon E2E test collection name.
    /// </summary>
    public const string Daemon = "MoneroDaemonE2E";
}

/// <summary>
/// Defines the daemon E2E test collection and fixture binding.
/// </summary>
[CollectionDefinition(E2ECollectionNames.Daemon)]
public sealed class MoneroDaemonE2ECollection : ICollectionFixture<MoneroE2EFixture>
{
}

