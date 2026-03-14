using BibXmr.Client.Wallet.Dto.Responses;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>SplitFundTransfer</c> behavior.
    /// </summary>
    public class SplitFundTransferTests
    {
        [Fact]
        public void SplitFundTransfer_ToString_FormatsTransfers_WhenCountsMatch()
        {
            var r = new SplitFundTransfer
            {
                TransactionHashes = { "h" },
                Amounts = { 1ul },
                Fees = { 1ul },
            };

            string s = r.ToString();
            Assert.Contains("Sent", s);
            Assert.Contains("[h]", s);
        }
    }
}




