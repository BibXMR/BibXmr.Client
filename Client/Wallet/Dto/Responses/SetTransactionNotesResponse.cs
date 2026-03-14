using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>SetTransactionNotes</c>.
    /// </summary>
    internal class SetTransactionNotesResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public SetTransactionNotes Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for set transaction notes.
    /// </summary>
    public class SetTransactionNotes
    {
        // ...
    }
}

