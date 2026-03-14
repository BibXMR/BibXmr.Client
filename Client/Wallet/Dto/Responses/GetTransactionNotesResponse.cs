using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>GetTransactionNotes</c>.
    /// </summary>
    internal class GetTransactionNotesResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public GetTransactionNotesResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>GetTransactionNotes</c>.
    /// </summary>
    internal class GetTransactionNotesResult
    {
        /// <summary>
        /// Gets or sets the collection of notes.
        /// </summary>
        [JsonPropertyName("notes")]
        public List<string> Notes { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, Notes);
        }
    }
}

