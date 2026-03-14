using System;

namespace BibXmr.Client.Utilities
{
    /// <summary>
    /// Represents a decoded Monero software version.
    /// </summary>
    public readonly struct MoneroVersion : IEquatable<MoneroVersion>, IComparable<MoneroVersion>
    {
        /// <summary>
        /// Decodes a Monero version from its packed integer representation.
        /// </summary>
        /// <param name="encoded">The encoded version value (major in upper 16 bits, minor in bits 8-15, patch in bits 0-7).</param>
        public MoneroVersion(uint encoded)
        {
            Encoded = encoded;
            Major = encoded >> 16;
            Minor = (encoded >> 8) & 0xFF;
            Patch = encoded & 0xFF;
        }

        /// <summary>
        /// Creates a version from explicit major, minor, and patch components.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version (0-255).</param>
        /// <param name="patch">The patch version (0-255).</param>
        public MoneroVersion(uint major, uint minor, uint patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Encoded = (major << 16) | ((minor & 0xFF) << 8) | (patch & 0xFF);
        }

        /// <summary>
        /// The major version component.
        /// </summary>
        public uint Major { get; }

        /// <summary>
        /// The minor version component.
        /// </summary>
        public uint Minor { get; }

        /// <summary>
        /// The patch version component.
        /// </summary>
        public uint Patch { get; }

        /// <summary>
        /// The original encoded version value.
        /// </summary>
        public uint Encoded { get; }

        /// <summary>Equality operator.</summary>
        public static bool operator ==(MoneroVersion left, MoneroVersion right) => left.Equals(right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(MoneroVersion left, MoneroVersion right) => !left.Equals(right);

        /// <summary>Less-than operator.</summary>
        public static bool operator <(MoneroVersion left, MoneroVersion right) => left.CompareTo(right) < 0;

        /// <summary>Greater-than operator.</summary>
        public static bool operator >(MoneroVersion left, MoneroVersion right) => left.CompareTo(right) > 0;

        /// <summary>Less-than-or-equal operator.</summary>
        public static bool operator <=(MoneroVersion left, MoneroVersion right) => left.CompareTo(right) <= 0;

        /// <summary>Greater-than-or-equal operator.</summary>
        public static bool operator >=(MoneroVersion left, MoneroVersion right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Returns the version as a dotted string (e.g. "0.18.3").
        /// </summary>
        public override string ToString() => $"{Major}.{Minor}.{Patch}";

        /// <inheritdoc/>
        public bool Equals(MoneroVersion other) => Encoded == other.Encoded;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is MoneroVersion other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => Encoded.GetHashCode();

        /// <inheritdoc/>
        public int CompareTo(MoneroVersion other) => Encoded.CompareTo(other.Encoded);
    }
}
