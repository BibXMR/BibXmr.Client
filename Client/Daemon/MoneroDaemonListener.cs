using System;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon.Dto;

namespace BibXmr.Client.Daemon
{
    /// <summary>
    /// Callback container for daemon polling subscriptions.
    /// </summary>
    public sealed class MoneroDaemonListener
    {
        /// <summary>
        /// Callback invoked when the daemon tip block header changes.
        /// </summary>
        public Func<BlockHeader, CancellationToken, ValueTask>? OnBlockHeaderAsync { get; set; }

        /// <summary>
        /// Callback invoked when polling encounters an exception.
        /// </summary>
        public Func<Exception, CancellationToken, ValueTask>? OnPollingErrorAsync { get; set; }
    }
}
