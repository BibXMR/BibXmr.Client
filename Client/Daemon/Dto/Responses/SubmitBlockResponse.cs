using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SubmitBlock</c>.
    /// </summary>
    internal class SubmitBlockResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SubmitBlock Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for submit block.
    /// </summary>
    public class SubmitBlock
    {
        // ...
    }
}

