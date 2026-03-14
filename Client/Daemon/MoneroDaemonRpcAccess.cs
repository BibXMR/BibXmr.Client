namespace BibXmr.Client.Daemon
{
    /// <summary>
    /// Declares whether a daemon RPC endpoint is operating in restricted or unrestricted mode.
    /// </summary>
    public enum MoneroDaemonRpcAccess
    {
        /// <summary>
        /// The endpoint is restricted and does not allow administrative or privacy-sensitive calls.
        /// </summary>
        Restricted,

        /// <summary>
        /// The endpoint is unrestricted and exposes the full daemon RPC surface.
        /// </summary>
        Unrestricted,
    }
}
