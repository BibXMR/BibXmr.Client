using BibXmr.Client.Network;

namespace BibXmr.Client.Utilities
{
    /// <summary>
    /// Decoded Monero address details.
    /// </summary>
    public sealed class DecodedMoneroAddress
    {
        /// <summary>
        /// Gets or sets the original encoded address.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the detected address kind.
        /// </summary>
        public MoneroAddressKind AddressKind { get; set; }

        /// <summary>
        /// Gets or sets the detected Monero network.
        /// </summary>
        public MoneroNetwork Network { get; set; }
    }
}
