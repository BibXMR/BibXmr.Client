using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>PaymentDetail</c>.
    /// </summary>
    internal class PaymentDetailResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public PaymentDetailResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>PaymentDetail</c>.
    /// </summary>
    internal class PaymentDetailResult
    {
        /// <summary>
        /// Gets or sets the collection of payments.
        /// </summary>
        [JsonPropertyName("payments")]
        public List<PaymentDetail> Payments { get; set; } = [];
    }
}

