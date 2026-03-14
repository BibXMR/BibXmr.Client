namespace BibXmr.Client.Wallet.Dto.Requests
{
    /// <summary>
    /// Represents an account/subaddress index payload sent to wallet RPC methods.
    /// </summary>
    internal class AddressIndexParameter
    {
        /// <summary>
        /// Gets or sets the major.
        /// </summary>
        public uint Major { get; set; }

        /// <summary>
        /// Gets or sets the minor.
        /// </summary>
        public uint Minor { get; set; }
    }
}

