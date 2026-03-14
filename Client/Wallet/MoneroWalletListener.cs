using System;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Wallet.Dto;
using BibXmr.Client.Wallet.Dto.Responses;

namespace BibXmr.Client.Wallet
{
    /// <summary>
    /// Callback container for wallet polling subscriptions.
    /// </summary>
    public sealed class MoneroWalletListener
    {
        /// <summary>
        /// Invoked when wallet height changes.
        /// </summary>
        public Func<ulong, CancellationToken, ValueTask>? OnNewBlockAsync { get; set; }

        /// <summary>
        /// Invoked when total or unlocked balance changes.
        /// </summary>
        public Func<ulong, ulong, CancellationToken, ValueTask>? OnBalancesChangedAsync { get; set; }

        /// <summary>
        /// Invoked when a new incoming output is observed.
        /// </summary>
        public Func<IncomingTransfer, CancellationToken, ValueTask>? OnOutputReceivedAsync { get; set; }

        /// <summary>
        /// Invoked when a tracked output becomes spent.
        /// </summary>
        public Func<IncomingTransfer, CancellationToken, ValueTask>? OnOutputSpentAsync { get; set; }

        /// <summary>
        /// Invoked when polling encounters an exception.
        /// </summary>
        public Func<Exception, CancellationToken, ValueTask>? OnPollingErrorAsync { get; set; }
    }
}
