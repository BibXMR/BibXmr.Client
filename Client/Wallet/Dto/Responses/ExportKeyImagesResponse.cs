using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>ExportKeyImages</c>.
    /// </summary>
    internal class ExportKeyImagesResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ExportKeyImages Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for export key images.
    /// </summary>
    public class ExportKeyImages
    {
        /// <summary>
        /// Gets or sets the collection of signed key images.
        /// </summary>
        [JsonPropertyName("signed_key_images")]
        public List<SignedKeyImage> SignedKeyImages { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, SignedKeyImages);
        }
    }
}

