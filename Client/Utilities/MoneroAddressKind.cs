namespace BibXmr.Client.Utilities
{
    /// <summary>
    /// Type of Monero address.
    /// </summary>
    public enum MoneroAddressKind
    {
        /// <summary>
        /// Standard primary address.
        /// </summary>
        Primary,

        /// <summary>
        /// Integrated address with embedded payment id.
        /// </summary>
        Integrated,

        /// <summary>
        /// Subaddress.
        /// </summary>
        Subaddress,
    }
}
