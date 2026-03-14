using System;
using BibXmr.Client.Utilities;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>TimeSpanExtensions</c> behavior.
    /// </summary>
    public class TimeSpanExtensionsTests
    {
        [Fact]
        public void Multiply_UInt64()
        {
            var ts = TimeSpan.FromTicks(2);
            Assert.Equal(TimeSpan.FromTicks(6), TimeSpanExtensions.Multiply(ts, 3ul));
        }

        [Fact]
        public void Multiply_UInt32()
        {
            var ts = TimeSpan.FromTicks(2);
            Assert.Equal(TimeSpan.FromTicks(6), TimeSpanExtensions.Multiply(ts, 3u));
        }

        [Fact]
        public void Multiply_Int64()
        {
            var ts = TimeSpan.FromTicks(2);
            Assert.Equal(TimeSpan.FromTicks(6), TimeSpanExtensions.Multiply(ts, 3L));
        }

        [Fact]
        public void DivideToUInt64_DivideByZero_Throws()
        {
            Assert.Throws<DivideByZeroException>(() => TimeSpan.FromSeconds(1).DivideToUInt64(TimeSpan.Zero));
        }

        [Fact]
        public void DivideToUInt64_UsesTicks()
        {
            Assert.Equal(2ul, TimeSpan.FromSeconds(2).DivideToUInt64(TimeSpan.FromSeconds(1)));
        }
    }
}


