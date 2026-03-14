using System;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Defines blockchain timing constants used by unlock-time calculations.
    /// </summary>
    internal static class BlockchainNetworkDefaults
    {
        /// <summary>
        /// Gets the base block unlock threshold.
        /// </summary>
        public const int BaseBlockUnlockThreshold = MoneroConstants.BaseBlockUnlockThreshold;

        /// <summary>
        /// Gets the average block time.
        /// </summary>
        public static readonly TimeSpan AverageBlockTime = MoneroConstants.AverageBlockTime;
    }
}
