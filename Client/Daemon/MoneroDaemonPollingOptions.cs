using System;

namespace BibXmr.Client.Daemon
{
    /// <summary>
    /// Polling settings for daemon subscriptions.
    /// </summary>
    public sealed class MoneroDaemonPollingOptions
    {
        /// <summary>
        /// Gets or sets the interval between daemon polling cycles.
        /// </summary>
        public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(10);
    }

    // TODO: Would a jitter option be useful here?
}
