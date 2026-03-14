using BibXmr.Client.Daemon;

namespace BibXmr.Client.Test.EndToEnd.Infrastructure;

/// <summary>
/// Owns the network resources and daemon client used by one test session.
/// </summary>
internal sealed class DaemonClientSession : IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IReadOnlyList<IDisposable> _ownedDisposables;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DaemonClientSession"/> class.
    /// </summary>
    /// <param name="client">The daemon client instance.</param>
    /// <param name="httpClient">The HTTP client used by the daemon client.</param>
    /// <param name="ownedDisposables">Additional owned resources that should be disposed with this session.</param>
    public DaemonClientSession(MoneroDaemonClient client, HttpClient httpClient, IReadOnlyList<IDisposable> ownedDisposables)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ownedDisposables = ownedDisposables ?? throw new ArgumentNullException(nameof(ownedDisposables));
    }

    /// <summary>
    /// Gets the daemon client.
    /// </summary>
    public MoneroDaemonClient Client { get; }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        if (_disposed)
            return ValueTask.CompletedTask;

        _disposed = true;

        Client.Dispose();
        _httpClient.Dispose();

        for (int index = _ownedDisposables.Count - 1; index >= 0; index--)
            _ownedDisposables[index].Dispose();

        return ValueTask.CompletedTask;
    }
}


