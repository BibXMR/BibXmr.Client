using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using BibXmr.Client.Network;
using BibXmr.Client.Utilities;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>AddressCodec</c> behavior.
    /// </summary>
    public class AddressCodecTests
    {
        [Theory]
        // Mainnet
        [InlineData("42U9v3qs5CjZEePHBZHwuSckQXebuZu299NSmVEmQ41YJZQhKcPyujyMSzpDH4VMMVSBo3U3b54JaNvQLwAjqDhKS3rvM3L", MoneroNetwork.Mainnet, MoneroAddressKind.Primary)]
        [InlineData("891TQPrWshJVpnBR4ZMhHiHpLx1PUnMqa3ccV5TJFBbqcJa3DWhjBh2QByCv3Su7WDPTGMHmCKkiVFN2fyGJKwbM1t6G7Ea", MoneroNetwork.Mainnet, MoneroAddressKind.Subaddress)]
        // Testnet
        [InlineData("9tUBnNCkC3UKGygHCwYvAB1FscpjUuq5e9MYJd2rXuiiTjjfVeSVjnbSG5VTnJgBgy9Y7GTLfxpZNMUwNZjGfdFr1z79eV1", MoneroNetwork.Testnet, MoneroAddressKind.Primary)]
        [InlineData("BgnKzHPJQDcg7xiP7bMN9MfPv9Z8ciT71iEMYnCdgBRBFETWgu9nKTr8fnzyGfU9h9gyNA8SFzYYzHfTS9KhqytSU943Nu1", MoneroNetwork.Testnet, MoneroAddressKind.Subaddress)]
        // Stagenet
        [InlineData("5B8s3obCY2ETeQB3GNAGPK2zRGen5UeW1WzegSizVsmf6z5NvM2GLoN6zzk1vHyzGAAfA8pGhuYAeCFZjHAp59jRVQkunGS", MoneroNetwork.Stagenet, MoneroAddressKind.Primary)]
        [InlineData("778B5D2JmMh5TJVWFbygJR15dvio5Z5B24hfSrWDzeroM8j8Lqc9sMoFE6324xg2ReaAZqHJkgfGFRugRmYHugHZ4f17Gxo", MoneroNetwork.Stagenet, MoneroAddressKind.Subaddress)]
        public void TryDecode_KnownVectors_AreClassified(string address, MoneroNetwork expectedNetwork, MoneroAddressKind expectedKind)
        {
            bool ok = MoneroAddressCodec.TryDecode(address, out DecodedMoneroAddress? decoded);
            Assert.True(ok);
            Assert.NotNull(decoded);
            Assert.Equal(expectedNetwork, decoded!.Network);
            Assert.Equal(expectedKind, decoded.AddressKind);
        }

        [Fact]
        public void TryDecode_GeneratedIntegratedAddresses_AcrossNetworks()
        {
            string mainnet = BuildIntegratedAddress(prefix: 19);
            string testnet = BuildIntegratedAddress(prefix: 54);
            string stagenet = BuildIntegratedAddress(prefix: 25);

            Assert.True(MoneroAddressCodec.TryDecode(mainnet, out DecodedMoneroAddress? mainDecoded));
            Assert.Equal(MoneroNetwork.Mainnet, mainDecoded!.Network);
            Assert.Equal(MoneroAddressKind.Integrated, mainDecoded.AddressKind);

            Assert.True(MoneroAddressCodec.TryDecode(testnet, out DecodedMoneroAddress? testDecoded));
            Assert.Equal(MoneroNetwork.Testnet, testDecoded!.Network);
            Assert.Equal(MoneroAddressKind.Integrated, testDecoded.AddressKind);

            Assert.True(MoneroAddressCodec.TryDecode(stagenet, out DecodedMoneroAddress? stageDecoded));
            Assert.Equal(MoneroNetwork.Stagenet, stageDecoded!.Network);
            Assert.Equal(MoneroAddressKind.Integrated, stageDecoded.AddressKind);
        }

        [Fact]
        public void TryDecode_InvalidChecksum_ReturnsFalse()
        {
            const string valid = "42U9v3qs5CjZEePHBZHwuSckQXebuZu299NSmVEmQ41YJZQhKcPyujyMSzpDH4VMMVSBo3U3b54JaNvQLwAjqDhKS3rvM3L";
            string mutated = valid.Substring(0, valid.Length - 1) + (valid.EndsWith("1") ? "2" : "1");

            Assert.False(MoneroAddressCodec.TryDecode(mutated, out _));
            Assert.False(MoneroAddressCodec.IsValid(mutated));
        }

        [Fact]
        public void TryDecode_InvalidCharacters_AndLength_ReturnFalse()
        {
            Assert.False(MoneroAddressCodec.TryDecode("not-a-monero-address", out _));
            Assert.False(MoneroAddressCodec.TryDecode("42U9v3", out _));
            Assert.False(MoneroAddressCodec.TryDecode("I0O", out _)); // contains disallowed base58 chars
        }

        [Fact]
        public void Decode_InvalidAddress_Throws()
        {
            Assert.Throws<System.InvalidOperationException>(() => MoneroAddressCodec.Decode("invalid"));
        }

        private static string BuildIntegratedAddress(byte prefix)
        {
            byte[] spend = Enumerable.Range(1, 32).Select(i => (byte)i).ToArray();
            byte[] view = Enumerable.Range(65, 32).Select(i => (byte)i).ToArray();
            byte[] paymentId = Enumerable.Range(200, 8).Select(i => (byte)i).ToArray();

            byte[] payload = new byte[1 + spend.Length + view.Length + paymentId.Length];
            payload[0] = prefix;
            Array.Copy(spend, 0, payload, 1, spend.Length);
            Array.Copy(view, 0, payload, 1 + spend.Length, view.Length);
            Array.Copy(paymentId, 0, payload, 1 + spend.Length + view.Length, paymentId.Length);

            byte[] checksum = ComputeKeccak(payload).Take(4).ToArray();
            byte[] full = new byte[payload.Length + checksum.Length];
            Array.Copy(payload, 0, full, 0, payload.Length);
            Array.Copy(checksum, 0, full, payload.Length, checksum.Length);

            return EncodeMoneroBase58(full);
        }

        private static byte[] ComputeKeccak(byte[] payload)
        {
            Type? nestedType = typeof(MoneroAddressCodec).GetNestedType("Keccak256", BindingFlags.NonPublic);
            Assert.NotNull(nestedType);
            MethodInfo? hashMethod = nestedType!.GetMethod("Hash", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(hashMethod);
            return (byte[])hashMethod!.Invoke(null, new object[] { payload })!;
        }

        private static string EncodeMoneroBase58(byte[] data)
        {
            const string alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            int[] encodedBlockSize = { 0, 2, 3, 5, 6, 7, 9, 10, 11 };
            const int fullBlockSize = 8;
            const int fullEncodedBlockSize = 11;

            var sb = new StringBuilder();
            int fullBlocks = data.Length / fullBlockSize;
            int lastBlockSize = data.Length % fullBlockSize;

            for (int i = 0; i < fullBlocks; i++)
                EncodeBlock(data, i * fullBlockSize, fullBlockSize, fullEncodedBlockSize, alphabet, sb);

            if (lastBlockSize > 0)
                EncodeBlock(data, fullBlocks * fullBlockSize, lastBlockSize, encodedBlockSize[lastBlockSize], alphabet, sb);

            return sb.ToString();
        }

        private static void EncodeBlock(byte[] data, int offset, int blockSize, int encodedSize, string alphabet, StringBuilder sb)
        {
            BigInteger value = BigInteger.Zero;
            for (int i = 0; i < blockSize; i++)
                value = (value << 8) + data[offset + i];

            char[] chars = Enumerable.Repeat('1', encodedSize).ToArray();
            for (int i = encodedSize - 1; i >= 0 && value > 0; i--)
            {
                value = BigInteger.DivRem(value, 58, out BigInteger remainder);
                chars[i] = alphabet[(int)remainder];
            }

            sb.Append(chars);
        }
    }
}


