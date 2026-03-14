using System;

namespace BibXmr.Client.Wallet
{
    /// <summary>
    /// Polling settings for wallet subscriptions.
    /// </summary>
    public sealed class MoneroWalletPollingOptions
    {
        /// <summary>
        /// Interval between polling iterations.
        /// </summary>
        public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(20);
    }
}
