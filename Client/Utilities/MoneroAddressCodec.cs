using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using BibXmr.Client.Network;

namespace BibXmr.Client.Utilities
{
    /// <summary>
    /// Local Monero address parsing and validation helpers.
    /// </summary>
    public static class MoneroAddressCodec
    {
        private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private const int FullBlockSize = 8;
        private const int FullEncodedBlockSize = 11;
        private const int StandardAddressLength = 95;
        private const int IntegratedAddressLength = 106;

        /// <summary>
        /// Compiled regex that validates a string contains only Monero base58 alphabet characters.
        /// </summary>
        private static readonly Regex Base58Regex = new("^[" + Alphabet + "]+$", RegexOptions.Compiled);

        private static readonly Dictionary<int, int> EncodedBlockSize = new()
        {
            { 0, 0 },
            { 2, 1 },
            { 3, 2 },
            { 5, 3 },
            { 6, 4 },
            { 7, 5 },
            { 9, 6 },
            { 10, 7 },
            { 11, 8 },
        };

        /// <summary>
        /// Decodes a Monero address and returns detected network and address kind.
        /// </summary>
        /// <param name="address">Address to decode.</param>
        /// <returns>Decoded address metadata.</returns>
        public static DecodedMoneroAddress Decode(string address)
        {
            if (!TryDecode(address, out DecodedMoneroAddress? decoded) || decoded == null)
            {
                throw new InvalidOperationException("Invalid Monero address.");
            }

            return decoded;
        }

        /// <summary>
        /// Checks whether the provided address is a valid Monero address.
        /// </summary>
        /// <param name="address">Address to validate.</param>
        /// <returns><c>true</c> when valid; otherwise <c>false</c>.</returns>
        public static bool IsValid(string address)
        {
            return TryDecode(address, out _);
        }

        /// <summary>
        /// Attempts to decode a Monero address.
        /// </summary>
        /// <param name="address">Address to decode.</param>
        /// <param name="decoded">Decoded metadata when successful.</param>
        /// <returns><c>true</c> if decoding succeeds; otherwise <c>false</c>.</returns>
        public static bool TryDecode(string address, out DecodedMoneroAddress? decoded)
        {
            decoded = null;

            if (string.IsNullOrWhiteSpace(address))
            {
                return false;
            }

            if (!Base58Regex.IsMatch(address))
            {
                return false;
            }

            if (address.Length != StandardAddressLength && address.Length != IntegratedAddressLength)
            {
                return false;
            }

            byte[] decodedBytes;
            try
            {
                decodedBytes = DecodeAddressToBytes(address);
            }
            catch
            {
                return false;
            }

            if (!HasExpectedLength(decodedBytes, address.Length))
            {
                return false;
            }

            if (!HasValidChecksum(decodedBytes))
            {
                return false;
            }

            byte networkByte = decodedBytes[0];
            if (!TryMapPrefix(networkByte, out MoneroNetwork network, out MoneroAddressKind kind))
            {
                return false;
            }

            if (address.Length == IntegratedAddressLength && kind != MoneroAddressKind.Integrated)
            {
                return false;
            }

            if (address.Length == StandardAddressLength && kind == MoneroAddressKind.Integrated)
            {
                return false;
            }

            decoded = new DecodedMoneroAddress
            {
                Address = address,
                AddressKind = kind,
                Network = network,
            };

            return true;
        }

        /// <summary>
        /// Checks that the decoded byte array length matches the expected length for the given encoded address format.
        /// </summary>
        /// <param name="decodedAddress">The decoded raw address bytes.</param>
        /// <param name="encodedLength">The length of the original base58-encoded address string.</param>
        /// <returns><see langword="true"/> if the length is valid; otherwise <see langword="false"/>.</returns>
        private static bool HasExpectedLength(byte[] decodedAddress, int encodedLength)
        {
            if (encodedLength == StandardAddressLength)
            {
                return decodedAddress.Length == 69;
            }

            if (encodedLength == IntegratedAddressLength)
            {
                return decodedAddress.Length == 77;
            }

            return false;
        }

        /// <summary>
        /// Validates the Keccak-256 checksum (last 4 bytes) of a decoded Monero address.
        /// </summary>
        /// <param name="decodedAddress">The decoded raw address bytes including the checksum.</param>
        /// <returns><see langword="true"/> if the checksum is valid; otherwise <see langword="false"/>.</returns>
        private static bool HasValidChecksum(byte[] decodedAddress)
        {
            if (decodedAddress.Length < 5)
            {
                return false;
            }

            int payloadLength = decodedAddress.Length - 4;
            byte[] payload = new byte[payloadLength];
            Array.Copy(decodedAddress, 0, payload, 0, payloadLength);

            byte[] checksum = Keccak256.Hash(payload);
            for (int i = 0; i < 4; i++)
            {
                if (decodedAddress[payloadLength + i] != checksum[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Maps a network byte prefix to its corresponding <see cref="MoneroNetwork"/> and <see cref="MoneroAddressKind"/>.
        /// </summary>
        /// <param name="prefix">The first byte of the decoded address.</param>
        /// <param name="network">The detected Monero network.</param>
        /// <param name="kind">The detected address kind.</param>
        /// <returns><see langword="true"/> if the prefix is recognized; otherwise <see langword="false"/>.</returns>
        private static bool TryMapPrefix(byte prefix, out MoneroNetwork network, out MoneroAddressKind kind)
        {
            network = MoneroNetwork.Mainnet;
            kind = MoneroAddressKind.Primary;

            switch (prefix)
            {
                case 18:
                    network = MoneroNetwork.Mainnet;
                    kind = MoneroAddressKind.Primary;
                    return true;
                case 19:
                    network = MoneroNetwork.Mainnet;
                    kind = MoneroAddressKind.Integrated;
                    return true;
                case 42:
                    network = MoneroNetwork.Mainnet;
                    kind = MoneroAddressKind.Subaddress;
                    return true;
                case 53:
                    network = MoneroNetwork.Testnet;
                    kind = MoneroAddressKind.Primary;
                    return true;
                case 54:
                    network = MoneroNetwork.Testnet;
                    kind = MoneroAddressKind.Integrated;
                    return true;
                case 63:
                    network = MoneroNetwork.Testnet;
                    kind = MoneroAddressKind.Subaddress;
                    return true;
                case 24:
                    network = MoneroNetwork.Stagenet;
                    kind = MoneroAddressKind.Primary;
                    return true;
                case 25:
                    network = MoneroNetwork.Stagenet;
                    kind = MoneroAddressKind.Integrated;
                    return true;
                case 36:
                    network = MoneroNetwork.Stagenet;
                    kind = MoneroAddressKind.Subaddress;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Decodes a full base58-encoded Monero address string into its raw byte representation.
        /// </summary>
        /// <param name="address">The base58-encoded address to decode.</param>
        /// <returns>The decoded raw bytes.</returns>
        private static byte[] DecodeAddressToBytes(string address)
        {
            int[] encoded = new int[address.Length];
            for (int i = 0; i < address.Length; i++)
            {
                encoded[i] = address[i];
            }

            int fullBlockCount = encoded.Length / FullEncodedBlockSize;
            int lastBlockSize = encoded.Length % FullEncodedBlockSize;
            int lastBlockDecodedSize = EncodedBlockSize[lastBlockSize];

            int dataSize = (fullBlockCount * FullBlockSize) + lastBlockDecodedSize;
            int[] data = new int[dataSize];

            for (int i = 0; i < fullBlockCount; i++)
            {
                int start = i * FullEncodedBlockSize;
                int[] block = SubArray(encoded, start, FullEncodedBlockSize);
                DecodeBlock(block, data, i * FullBlockSize);
            }

            if (lastBlockSize > 0)
            {
                int start = fullBlockCount * FullEncodedBlockSize;
                int[] block = SubArray(encoded, start, lastBlockSize);
                DecodeBlock(block, data, fullBlockCount * FullBlockSize);
            }

            byte[] bytes = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                bytes[i] = (byte)data[i];
            }

            return bytes;
        }

        /// <summary>
        /// Decodes a single base58 block into its raw byte representation and writes it into the output buffer.
        /// </summary>
        /// <param name="encodedBlock">The encoded base58 character values.</param>
        /// <param name="decodedBuffer">The output buffer to write decoded bytes into.</param>
        /// <param name="decodedOffset">The starting offset in the output buffer.</param>
        private static void DecodeBlock(int[] encodedBlock, int[] decodedBuffer, int decodedOffset)
        {
            if (!EncodedBlockSize.TryGetValue(encodedBlock.Length, out int decodedSize) || decodedSize <= 0)
            {
                throw new InvalidOperationException("Invalid encoded block size.");
            }

            BigInteger value = 0;
            BigInteger order = 1;
            for (int i = encodedBlock.Length - 1; i >= 0; i--)
            {
                int digit = Alphabet.IndexOf((char)encodedBlock[i]);
                if (digit < 0)
                {
                    throw new InvalidOperationException("Invalid base58 character.");
                }

                value += order * digit;
                order *= Alphabet.Length;
            }

            int[] decoded = UIntToBigEndianBytes(value, decodedSize);
            for (int i = 0; i < decoded.Length; i++)
            {
                decodedBuffer[decodedOffset + i] = decoded[i];
            }
        }

        /// <summary>
        /// Converts a <see cref="BigInteger"/> value to a big-endian byte array of the specified size.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="size">The desired output array length.</param>
        /// <returns>A big-endian byte array.</returns>
        private static int[] UIntToBigEndianBytes(BigInteger value, int size)
        {
            int[] result = new int[size];
            BigInteger divisor = BigInteger.Pow(2, 8);
            for (int i = size - 1; i >= 0; i--)
            {
                result[i] = (int)(value % divisor);
                value /= divisor;
            }

            return result;
        }

        /// <summary>
        /// Extracts a sub-array from the source starting at the given index with the specified length.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="index">The starting index.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A new array containing the extracted elements.</returns>
        private static int[] SubArray(int[] source, int index, int length)
        {
            int[] result = new int[length];
            Array.Copy(source, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Implements Keccak-256 hashing used by Monero address checksum logic.
        /// </summary>
        private static class Keccak256
        {
            private const int RateInBytes = 136; // 1088 bits
            private static readonly int[] RotationOffsets = new[]
            {
                0, 1, 62, 28, 27,
                36, 44, 6, 55, 20,
                3, 10, 43, 25, 39,
                41, 45, 15, 21, 8,
                18, 2, 61, 56, 14,
            };

            private static readonly ulong[] RoundConstants = new[]
            {
                0x0000000000000001UL, 0x0000000000008082UL,
                0x800000000000808AUL, 0x8000000080008000UL,
                0x000000000000808BUL, 0x0000000080000001UL,
                0x8000000080008081UL, 0x8000000000008009UL,
                0x000000000000008AUL, 0x0000000000000088UL,
                0x0000000080008009UL, 0x000000008000000AUL,
                0x000000008000808BUL, 0x800000000000008BUL,
                0x8000000000008089UL, 0x8000000000008003UL,
                0x8000000000008002UL, 0x8000000000000080UL,
                0x000000000000800AUL, 0x800000008000000AUL,
                0x8000000080008081UL, 0x8000000000008080UL,
                0x0000000080000001UL, 0x8000000080008008UL,
            };

            /// <summary>
            /// Executes the hash operation.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <returns>The operation result.</returns>
            public static byte[] Hash(byte[] input)
            {
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input));
                }

                ulong[] state = new ulong[25];
                int offset = 0;

                while (input.Length - offset >= RateInBytes)
                {
                    AbsorbBlock(state, input, offset, RateInBytes);
                    KeccakF(state);
                    offset += RateInBytes;
                }

                byte[] finalBlock = new byte[RateInBytes];
                int remaining = input.Length - offset;
                if (remaining > 0)
                {
                    Array.Copy(input, offset, finalBlock, 0, remaining);
                }

                // Keccak padding (not SHA3 domain separation).
                finalBlock[remaining] = 0x01;
                finalBlock[RateInBytes - 1] |= 0x80;

                AbsorbBlock(state, finalBlock, 0, RateInBytes);
                KeccakF(state);

                byte[] output = new byte[32];
                int outOffset = 0;
                int lane = 0;
                while (outOffset < output.Length)
                {
                    byte[] laneBytes = UInt64ToLittleEndian(state[lane]);
                    int toCopy = Math.Min(8, output.Length - outOffset);
                    Array.Copy(laneBytes, 0, output, outOffset, toCopy);
                    outOffset += toCopy;
                    lane++;
                }

                return output;
            }

            /// <summary>
            /// XORs one block of input data into the Keccak sponge state.
            /// </summary>
            /// <param name="state">The 25-lane sponge state.</param>
            /// <param name="input">The input byte array.</param>
            /// <param name="inputOffset">The offset into the input array.</param>
            /// <param name="blockSize">The number of bytes to absorb.</param>
            private static void AbsorbBlock(ulong[] state, byte[] input, int inputOffset, int blockSize)
            {
                int lanes = blockSize / 8;
                for (int i = 0; i < lanes; i++)
                {
                    state[i] ^= LittleEndianToUInt64(input, inputOffset + (i * 8));
                }
            }

            /// <summary>
            /// Performs the Keccak-f[1600] permutation (24 rounds) on the sponge state.
            /// </summary>
            /// <param name="state">The 25-lane sponge state to permute in place.</param>
            private static void KeccakF(ulong[] state)
            {
                ulong[] c = new ulong[5];
                ulong[] d = new ulong[5];
                ulong[] b = new ulong[25];

                for (int round = 0; round < 24; round++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        c[x] = state[x] ^ state[x + 5] ^ state[x + 10] ^ state[x + 15] ^ state[x + 20];
                    }

                    for (int x = 0; x < 5; x++)
                    {
                        d[x] = c[(x + 4) % 5] ^ RotateLeft(c[(x + 1) % 5], 1);
                    }

                    for (int y = 0; y < 5; y++)
                    {
                        int row = y * 5;
                        for (int x = 0; x < 5; x++)
                        {
                            state[row + x] ^= d[x];
                        }
                    }

                    for (int y = 0; y < 5; y++)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            int idx = x + (5 * y);
                            int newX = y;
                            int newY = ((2 * x) + (3 * y)) % 5;
                            b[newX + (5 * newY)] = RotateLeft(state[idx], RotationOffsets[idx]);
                        }
                    }

                    for (int y = 0; y < 5; y++)
                    {
                        int row = y * 5;
                        for (int x = 0; x < 5; x++)
                        {
                            state[row + x] = b[row + x] ^ ((~b[row + ((x + 1) % 5)]) & b[row + ((x + 2) % 5)]);
                        }
                    }

                    state[0] ^= RoundConstants[round];
                }
            }

            /// <summary>
            /// Performs a 64-bit left rotation by the specified number of bits.
            /// </summary>
            /// <param name="value">The value to rotate.</param>
            /// <param name="offset">The number of bits to rotate left.</param>
            /// <returns>The rotated value.</returns>
            private static ulong RotateLeft(ulong value, int offset)
            {
                if (offset == 0)
                {
                    return value;
                }

                return (value << offset) | (value >> (64 - offset));
            }

            /// <summary>
            /// Reads 8 bytes from the buffer at the given offset in little-endian order and returns them as a <see cref="ulong"/>.
            /// </summary>
            /// <param name="data">The source byte array.</param>
            /// <param name="offset">The byte offset to start reading from.</param>
            /// <returns>The decoded 64-bit unsigned integer.</returns>
            private static ulong LittleEndianToUInt64(byte[] data, int offset)
            {
                return ((ulong)data[offset + 0]) |
                       ((ulong)data[offset + 1] << 8) |
                       ((ulong)data[offset + 2] << 16) |
                       ((ulong)data[offset + 3] << 24) |
                       ((ulong)data[offset + 4] << 32) |
                       ((ulong)data[offset + 5] << 40) |
                       ((ulong)data[offset + 6] << 48) |
                       ((ulong)data[offset + 7] << 56);
            }

            /// <summary>
            /// Converts a <see cref="ulong"/> value to its 8-byte little-endian representation.
            /// </summary>
            /// <param name="value">The value to convert.</param>
            /// <returns>An 8-byte array in little-endian order.</returns>
            private static byte[] UInt64ToLittleEndian(ulong value)
            {
                return new[]
                {
                    (byte)(value & 0xFF),
                    (byte)((value >> 8) & 0xFF),
                    (byte)((value >> 16) & 0xFF),
                    (byte)((value >> 24) & 0xFF),
                    (byte)((value >> 32) & 0xFF),
                    (byte)((value >> 40) & 0xFF),
                    (byte)((value >> 48) & 0xFF),
                    (byte)((value >> 56) & 0xFF),
                };
            }
        }
    }
}
