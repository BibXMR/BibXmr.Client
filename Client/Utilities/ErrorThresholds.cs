using System;
using BibXmr.Client.Network;

namespace BibXmr.Client.Utilities
{
    /// <summary>
    /// Defines threshold constants used by client-side guard checks.
    /// </summary>
    internal class ErrorThresholds
    {
        /// <summary>
        /// Maximum unlock horizon accepted by client-side guards, expressed in blocks.
        /// </summary>
        public static readonly ulong MaximumUnlockTime = TimeSpan.FromDays(365).DivideToUInt64(BlockchainNetworkDefaults.AverageBlockTime);
    }
}
