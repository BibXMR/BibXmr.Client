using System;
using BibXmr.Client.Wallet.Dto;
using BibXmr.Client.Wallet.Dto.Responses;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>ShowTransfers</c> behavior.
    /// </summary>
    public class ShowTransfersTests
    {
        [Fact]
        public void Transfer_DerivedProperties_AndToString_AreComputed()
        {
            var t = new Transfer
            {
                Address = "addr",
                Amount = 1,
                Fee = 1,
                Confirmations = 1,
                Height = 1,
                Timestamp = 1,
                TransactionID = "txid",
                Type = "in",
                UnlockTime = 1,
                Note = "",
                PaymentID = "",
                SubaddressIndex = new SubaddressIndex(),
            };

            Assert.True(t.EstimatedTimeTillUnlock > TimeSpan.Zero);
            Assert.Equal(BibXmr.Client.Utilities.MoneroDateTime.UnixEpoch.AddSeconds(1), t.DateTime);
            _ = t.ToString();
        }

        [Fact]
        public void ShowTransfers_ToString_IncludesAllNonEmptySections()
        {
            var transfer = new Transfer
            {
                Address = "addr",
                Amount = 1,
                Fee = 1,
                Confirmations = 1,
                Height = 1,
                Timestamp = 1,
                TransactionID = "txid",
                Type = "in",
                UnlockTime = 0,
                Note = "",
                PaymentID = "",
                SubaddressIndex = new SubaddressIndex(),
            };

            var show = new ShowTransfers();
            show.IncomingTransfers.Add(transfer);
            show.OutgoingTransfers.Add(transfer);
            show.PendingTransfers.Add(transfer);
            show.FailedTransfers.Add(transfer);
            show.PooledTransfers.Add(transfer);

            string s = show.ToString();
            Assert.Contains("Incoming Transfers:", s);
            Assert.Contains("Outgoing Transfers:", s);
            Assert.Contains("Pending Transfers:", s);
            Assert.Contains("Failed Transfers:", s);
            Assert.Contains("Pooled Transfers:", s);
        }
    }
}




