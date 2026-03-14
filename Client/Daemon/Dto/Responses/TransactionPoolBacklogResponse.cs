using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Daemon.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>TransactionPoolBacklog</c>.
    /// </summary>
    internal class TransactionPoolBacklogResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public TransactionPoolBacklog Result { get; set; }
    }

    /// <summary>
    /// Represents a daemon RPC response payload for transaction pool backlog.
    /// </summary>
    public class TransactionPoolBacklog
    {
        /// <summary>
        /// Gets or sets the backlog.
        /// </summary>
        [JsonPropertyName("backlog")]
        public string Backlog { get; set; }

        /// <summary>
        /// Gets or sets the credits.
        /// </summary>
        [JsonPropertyName("credits")]
        public ulong Credits { get; set; }

        /// <summary>
        /// Gets or sets the RPC status string.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the top hash.
        /// </summary>
        [JsonPropertyName("top_hash")]
        public string TopHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether untrusted.
        /// </summary>
        [JsonPropertyName("untrusted")]
        public bool IsUntrusted { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{TopHash} - {Status} - {(IsUntrusted ? "Untrusted" : "Trusted")}";
        }
    }
}

