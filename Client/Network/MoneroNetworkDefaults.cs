using System;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Defines default daemon and wallet endpoint values for supported networks.
    /// </summary>
    internal static class MoneroNetworkDefaults
    {
        /// <summary>
        /// Gets the daemon mainnet url.
        /// </summary>
        public const string DaemonMainnetUrl = MoneroConstants.DefaultHost;

        /// <summary>
        /// Gets the daemon mainnet port.
        /// </summary>
        public const uint DaemonMainnetPort = MoneroConstants.DaemonMainnetPort;

        /// <summary>
        /// Gets the daemon stagenet url.
        /// </summary>
        public const string DaemonStagenetUrl = MoneroConstants.DefaultHost;

        /// <summary>
        /// Gets the daemon stagenet port.
        /// </summary>
        public const uint DaemonStagenetPort = MoneroConstants.DaemonStagenetPort;

        /// <summary>
        /// Gets the daemon testnet url.
        /// </summary>
        public const string DaemonTestnetUrl = MoneroConstants.DefaultHost;

        /// <summary>
        /// Gets the daemon testnet port.
        /// </summary>
        public const uint DaemonTestnetPort = MoneroConstants.DaemonTestnetPort;

        /// <summary>
        /// Gets the wallet mainnet url.
        /// </summary>
        public const string WalletMainnetUrl = MoneroConstants.DefaultHost;

        /// <summary>
        /// Gets the wallet mainnet port.
        /// </summary>
        public const uint WalletMainnetPort = MoneroConstants.WalletMainnetPort;

        /// <summary>
        /// Gets the wallet stagenet url.
        /// </summary>
        public const string WalletStagenetUrl = MoneroConstants.DefaultHost;

        /// <summary>
        /// Gets the wallet stagenet port.
        /// </summary>
        public const uint WalletStagenetPort = MoneroConstants.WalletStagenetPort;

        /// <summary>
        /// Gets the wallet testnet url.
        /// </summary>
        public const string WalletTestnetUrl = MoneroConstants.DefaultHost;

        /// <summary>
        /// Gets the wallet testnet port.
        /// </summary>
        public const uint WalletTestnetPort = MoneroConstants.WalletTestnetPort;
    }
}
