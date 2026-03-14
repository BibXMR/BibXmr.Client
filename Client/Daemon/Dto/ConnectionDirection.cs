namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Indicates the direction of a peer connection.
    /// </summary>
    public enum ConnectionDirection
    {
        /// <summary>The connection direction could not be determined.</summary>
        Unknown = 0,

        /// <summary>The remote peer initiated the connection.</summary>
        Incoming = 1,

        /// <summary>The local node initiated the connection.</summary>
        Outgoing = 2,
    }
}

