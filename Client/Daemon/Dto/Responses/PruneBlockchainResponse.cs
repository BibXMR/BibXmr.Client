using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>PruneBlockchain</c>.
    /// </summary>
    internal class PruneBlockchainResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public PruneBlockchain Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for prune blockchain.
    /// </summary>
    public class PruneBlockchain
    {
        /// <summary>
        /// Gets or sets a value indicating whether pruned.
        /// </summary>
        [JsonPropertyName("pruned")]
        public bool IsPruned { get; set; }

        /// <summary>
        /// Gets or sets the pruning seed.
        /// </summary>
        [JsonPropertyName("pruning_seed")]
        public uint PruningSeed { get; set; }
    }
}

