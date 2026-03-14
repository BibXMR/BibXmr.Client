using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>AlternateChain</c>.
    /// </summary>
    internal class AlternateChainResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public AlternateChainResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>AlternateChain</c>.
    /// </summary>
    internal class AlternateChainResult
    {
        /// <summary>
        /// Gets or sets the collection of chains.
        /// </summary>
        [JsonPropertyName("chains")]
        public List<Chain> Chains { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, Chains);
        }
    }
}

