using System;

namespace BibXmr.Client.Utilities
{
    /// <summary>
    /// Utility helpers for converting Monero Unix-epoch timestamps to .NET date/time types.
    /// </summary>
    public static class MoneroDateTime
    {
        /// <summary>
        /// The Unix epoch (1970-01-01T00:00:00Z).
        /// </summary>
        public static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a Unix timestamp in seconds to a UTC <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="seconds">Seconds since the Unix epoch.</param>
        /// <returns>A <see cref="DateTimeOffset"/> representing the specified point in time.</returns>
        public static DateTimeOffset FromUnixSeconds(ulong seconds)
        {
            return new DateTimeOffset(UnixEpoch.AddSeconds(seconds), TimeSpan.Zero);
        }
    }
}
