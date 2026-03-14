using System.Collections.Generic;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Used for non json_rpc interface commands.
    /// </summary>
    internal class CustomRequestParameters : GenericRequestParameters
    {
        /// <summary>
        /// Gets or sets the collection of txs hashes.
        /// </summary>
        public IEnumerable<string>? Txs_hashes { get; set; }
    }
}
