using System;
using BibXmr.Client.Utilities;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>PriceUtilities</c> behavior.
    /// </summary>
    public class PriceUtilitiesTests
    {
        [Fact]
        public void XmrToPiconero_NegativeMonero_Throws()
        {
            decimal monero = -1;
            Assert.Throws<InvalidOperationException>(() => MoneroAmount.XmrToPiconero(monero));
        }

        [Fact]
        public void XmrToPiconero_MoreThan12DecimalPlaces_Throws()
        {
            decimal monero = 1.000000000000123M;
            Assert.Throws<InvalidOperationException>(() => MoneroAmount.XmrToPiconero(monero));
        }

        [Fact]
        public void XmrToPiconero_ZeroMonero()
        {
            decimal monero = 0;
            ulong piconero = MoneroAmount.XmrToPiconero(monero);
            Assert.Equal(0ul, piconero);
        }

        [Fact]
        public void PiconeroToXmr_ZeroPiconero()
        {
            ulong piconero = 0;
            decimal monero = MoneroAmount.PiconeroToXmr(piconero);
            Assert.Equal(0M, monero);
        }

        [Fact]
        public void PiconeroToXmr_TenPiconero()
        {
            ulong piconero = 10;
            decimal monero = MoneroAmount.PiconeroToXmr(piconero);
            Assert.Equal(0.000000000010M, monero);
        }

        [Fact]
        public void PiconeroToXmr_MaxPiconero_DoesNotThrow()
        {
            ulong piconero = ulong.MaxValue;
            _ = MoneroAmount.PiconeroToXmr(piconero);
            // Does not throw.
        }

        [Fact]
        public void PiconeroToXmr_OnePiconero()
        {
            ulong piconero = 1;
            decimal monero = MoneroAmount.PiconeroToXmr(piconero);
            Assert.Equal(0.000000000001M, monero);
        }

        [Fact]
        public void XmrToPiconero_OnePiconero()
        {
            decimal monero = 0.000000000001M;
            ulong piconero = MoneroAmount.XmrToPiconero(monero);
            Assert.Equal(1ul, piconero);
        }

        [Fact]
        public void XmrToPiconero_OneMonero()
        {
            decimal monero = 1M;
            ulong piconero = MoneroAmount.XmrToPiconero(monero);
            Assert.Equal(1000000000000ul, piconero);
        }

        [Fact]
        public void XmrToPiconero_HalfMonero()
        {
            decimal monero = 0.5M;
            ulong piconero = MoneroAmount.XmrToPiconero(monero);
            Assert.Equal(500000000000ul, piconero);
        }

        [Fact]
        public void XmrToPiconero_OneAndAHalfMonero()
        {
            decimal monero = 1.5M;
            ulong piconero = MoneroAmount.XmrToPiconero(monero);
            Assert.Equal(1500000000000ul, piconero);
        }

        [Fact]
        public void XmrToPiconero_OneThirteenthAMonero_Throws()
        {
            decimal monero = 0.0000000000001M;
            Assert.Throws<InvalidOperationException>(() => MoneroAmount.XmrToPiconero(monero));
        }

        [Fact]
        public void XmrToPiconero_MaxMonero_Throws()
        {
            decimal monero = decimal.MaxValue;
            Assert.Throws<OverflowException>(() => MoneroAmount.XmrToPiconero(monero));
        }

        [Fact]
        public void FormatXmr_ZeroPiconero_FormatsCorrectly()
        {
            string formatted = MoneroAmount.FormatXmr(0);
            Assert.Contains("0.000000000000", formatted);
        }

        [Fact]
        public void PiconeroPerXmr_IsCorrect()
        {
            Assert.Equal(1_000_000_000_000UL, MoneroAmount.PiconeroPerXmr);
        }

        [Fact]
        public void PrecisionFormat_IsN12()
        {
            Assert.Equal("N12", MoneroAmount.PrecisionFormat);
        }
    }
}

