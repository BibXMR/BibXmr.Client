namespace BibXmr.Client.Test.EndToEnd.Infrastructure;

/// <summary>
/// Represents runtime configuration for daemon E2E tests.
/// </summary>
internal sealed class MoneroE2EOptions
{
    /// <summary>
    /// Gets or sets the daemon base address used by direct live calls.
    /// </summary>
    public Uri DaemonBaseAddress { get; set; } = new("https://127.0.0.1:3000/", UriKind.Absolute);

    /// <summary>
    /// Gets or sets a value indicating whether TLS certificate validation should be bypassed.
    /// </summary>
    public bool AllowInsecureTls { get; set; } = true;

    /// <summary>
    /// Gets or sets the default timeout in seconds for each RPC request.
    /// </summary>
    public int DefaultTimeoutSeconds { get; set; } = 10;

    /// <summary>
    /// Gets or sets the retry count used by transient-failure tests.
    /// </summary>
    public int RetryCount { get; set; } = 2;

    /// <summary>
    /// Gets or sets the retry delay in milliseconds used by transient-failure tests.
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 300;

    /// <summary>
    /// Validates required settings.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when required settings are missing or invalid.</exception>
    public void Validate()
    {
        if (!DaemonBaseAddress.IsAbsoluteUri)
            throw new InvalidOperationException("MoneroE2E:DaemonBaseAddress must be an absolute URI.");

        if (DefaultTimeoutSeconds <= 0)
            throw new InvalidOperationException("MoneroE2E:DefaultTimeoutSeconds must be greater than 0.");

        if (RetryCount < 0)
            throw new InvalidOperationException("MoneroE2E:RetryCount must be zero or greater.");

        if (RetryDelayMilliseconds < 0)
            throw new InvalidOperationException("MoneroE2E:RetryDelayMilliseconds must be zero or greater.");
    }
}

