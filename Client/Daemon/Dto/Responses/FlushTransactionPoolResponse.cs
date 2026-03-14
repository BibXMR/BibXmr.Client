using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>FlushTransactionPool</c>.
    /// </summary>
    internal class FlushTransactionPoolResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public FlushTransactionPoolResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>FlushTransactionPool</c>.
    /// </summary>
    internal class FlushTransactionPoolResult
    {
        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return Status;
        }
    }
}

