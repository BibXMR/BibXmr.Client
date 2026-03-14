using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>CreateAccount</c>.
    /// </summary>
    internal class CreateAccountResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public CreateAccount Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for create account.
    /// </summary>
    public class CreateAccount
    {
        /// <summary>
        /// Gets or sets the account index.
        /// </summary>
        [JsonPropertyName("account_index")]
        public uint AccountIndex { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"[{AccountIndex}] {Address}";
        }
    }
}

