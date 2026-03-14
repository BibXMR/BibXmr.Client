using System.Text.Json.Serialization;
using BibXmr.Client.Network;

namespace BibXmr.Client.Wallet.Dto.Responses
{
    /// <summary>
    /// Represents an internal RPC response envelope for <c>RefreshWallet</c>.
    /// </summary>
    internal class RefreshWalletResponse : RpcResponse
    {
        /// <summary>
        /// Gets or sets the RPC result payload.
        /// </summary>
        [JsonPropertyName("result")]
        public RefreshWallet Result { get; set; }
    }

    /// <summary>
    /// Represents a wallet RPC response payload for refresh wallet.
    /// </summary>
    public class RefreshWallet
    {
        /// <summary>
        /// Gets or sets the blocks fetched.
        /// </summary>
        [JsonPropertyName("blocks_fetched")]
        public ulong BlocksFetched { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether received money.
        /// </summary>
        [JsonPropertyName("received_money")]
        public bool ReceivedMoney { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{(ReceivedMoney ? "Money Received" : "No Money Received")} (Blocks Fetched: {BlocksFetched})";
        }
    }
}

