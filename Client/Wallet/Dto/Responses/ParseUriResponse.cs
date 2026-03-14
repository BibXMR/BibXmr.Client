using System.Text.Json.Serialization;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>ParseUri</c>.
    /// </summary>
    internal class ParseUriResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public ParseUriResult Result { get; set; }
    }

    /// <summary>
    /// Represents an internal RPC result payload for <c>ParseUri</c>.
    /// </summary>
    internal class ParseUriResult
    {
        /// <summary>
        /// Gets or sets the uri.
        /// </summary>
        [JsonPropertyName("uri")]
        public MoneroUri Uri { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Uri}";
        }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for monero uri.
    /// </summary>
    public class MoneroUri
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the payment id.
        /// </summary>
        [JsonPropertyName("payment_id")]
        public string PaymentID { get; set; }

        /// <summary>
        /// Gets or sets the recipient name.
        /// </summary>
        [JsonPropertyName("recipient_name")]
        public string RecipientName { get; set; }

        /// <summary>
        /// Gets or sets the transaction description.
        /// </summary>
        [JsonPropertyName("tx_description")]
        public string TransactionDescription { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"Address: {Address} Amount: {MoneroAmount.PiconeroToXmr(Amount).ToString(MoneroAmount.PrecisionFormat)} PaymentID: {PaymentID} RecipientName: {RecipientName} Description: {TransactionDescription}";
        }
    }
}

