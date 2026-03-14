using System;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Well-known Monero network constants.
    /// </summary>
    public static class MoneroConstants
    {
        /// <summary>Default host address for local Monero services.</summary>
        public const string DefaultHost = "127.0.0.1";

        /// <summary>Default daemon RPC port for Mainnet.</summary>
        public const uint DaemonMainnetPort = 18081;

        /// <summary>Default wallet RPC port for Mainnet.</summary>
        public const uint WalletMainnetPort = 18082;

        /// <summary>Default daemon RPC port for Testnet.</summary>
        public const uint DaemonTestnetPort = 28081;

        /// <summary>Default wallet RPC port for Testnet.</summary>
        public const uint WalletTestnetPort = 28082;

        /// <summary>Default daemon RPC port for Stagenet.</summary>
        public const uint DaemonStagenetPort = 38081;

        /// <summary>Default wallet RPC port for Stagenet.</summary>
        public const uint WalletStagenetPort = 38082;

        /// <summary>Number of blocks before coinbase outputs can be spent (60 blocks).</summary>
        public const int CoinbaseUnlockBlocks = 60;

        /// <summary>Base unlock threshold for non-coinbase transactions (10 blocks).</summary>
        public const int BaseBlockUnlockThreshold = 10;

        /// <summary>Average time between Monero blocks (~2 minutes).</summary>
        public static readonly TimeSpan AverageBlockTime = TimeSpan.FromMinutes(2);

        /// <summary>
        /// Gets the default daemon RPC port for the specified network.
        /// </summary>
        /// <param name="network">The Monero network type.</param>
        /// <returns>The default daemon port.</returns>
        public static uint GetDaemonPort(MoneroNetwork network) => network switch
        {
            MoneroNetwork.Mainnet => DaemonMainnetPort,
            MoneroNetwork.Testnet => DaemonTestnetPort,
            MoneroNetwork.Stagenet => DaemonStagenetPort,
            _ => throw new ArgumentOutOfRangeException(nameof(network), network, "Unknown Monero network."),
        };

        /// <summary>
        /// Gets the default wallet RPC port for the specified network.
        /// </summary>
        /// <param name="network">The Monero network type.</param>
        /// <returns>The default wallet port.</returns>
        public static uint GetWalletPort(MoneroNetwork network) => network switch
        {
            MoneroNetwork.Mainnet => WalletMainnetPort,
            MoneroNetwork.Testnet => WalletTestnetPort,
            MoneroNetwork.Stagenet => WalletStagenetPort,
            _ => throw new ArgumentOutOfRangeException(nameof(network), network, "Unknown Monero network."),
        };
    }
}
