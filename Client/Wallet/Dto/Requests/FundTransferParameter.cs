namespace BibXmr.Client.Wallet.Dto.Requests
{
    /// <summary>
    /// Represents a wallet transfer destination payload sent to wallet RPC methods.
    /// </summary>
    internal class FundTransferParameter
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string Address { get; set; }
    }
}

