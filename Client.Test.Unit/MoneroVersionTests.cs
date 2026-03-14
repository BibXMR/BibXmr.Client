using BibXmr.Client.Utilities;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>MoneroVersion</c> behavior.
    /// </summary>
    public class MoneroVersionTests
    {
        [Fact]
        public void Decode_EncodedVersion_ExtractsComponents()
        {
            // 0.18.3 encoded: (0 << 16) | (18 << 8) | 3 = 0x1203 = 4611
            var version = new MoneroVersion(4611);
            Assert.Equal(0u, version.Major);
            Assert.Equal(18u, version.Minor);
            Assert.Equal(3u, version.Patch);
        }

        [Fact]
        public void Explicit_Constructor_EncodesCorrectly()
        {
            var version = new MoneroVersion(0, 18, 3);
            Assert.Equal(4611u, version.Encoded);
        }

        [Fact]
        public void Roundtrip_EncodeDecodeFromExplicit()
        {
            var original = new MoneroVersion(1, 2, 3);
            var decoded = new MoneroVersion(original.Encoded);
            Assert.Equal(original.Major, decoded.Major);
            Assert.Equal(original.Minor, decoded.Minor);
            Assert.Equal(original.Patch, decoded.Patch);
            Assert.Equal(original.Encoded, decoded.Encoded);
        }

        [Fact]
        public void ToString_ReturnsDottedFormat()
        {
            var version = new MoneroVersion(0, 18, 3);
            Assert.Equal("0.18.3", version.ToString());
        }

        [Fact]
        public void Equality_SameEncoded_AreEqual()
        {
            var a = new MoneroVersion(4611);
            var b = new MoneroVersion(0, 18, 3);
            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
        }

        [Fact]
        public void Equality_DifferentEncoded_AreNotEqual()
        {
            var a = new MoneroVersion(0, 18, 3);
            var b = new MoneroVersion(0, 18, 4);
            Assert.NotEqual(a, b);
            Assert.True(a != b);
        }

        [Fact]
        public void CompareTo_Ordering_IsCorrect()
        {
            var older = new MoneroVersion(0, 17, 3);
            var newer = new MoneroVersion(0, 18, 3);
            Assert.True(older < newer);
            Assert.True(newer > older);
            Assert.True(older <= newer);
            Assert.True(newer >= older);
        }

        [Fact]
        public void GetHashCode_SameEncoded_SameHash()
        {
            var a = new MoneroVersion(4611);
            var b = new MoneroVersion(4611);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void Equals_Object_ReturnsFalseForOtherType()
        {
            var v = new MoneroVersion(4611);
            Assert.False(v.Equals("not a version"));
        }

        [Fact]
        public void Zero_Version_HandlesGracefully()
        {
            var version = new MoneroVersion(0);
            Assert.Equal(0u, version.Major);
            Assert.Equal(0u, version.Minor);
            Assert.Equal(0u, version.Patch);
            Assert.Equal("0.0.0", version.ToString());
        }
    }
}

