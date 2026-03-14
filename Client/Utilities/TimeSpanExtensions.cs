using System;

namespace BibXmr.Client.Utilities
{
    /// <summary>
    /// Provides extension helpers for <c>TimeSpan</c>.
    /// </summary>
    internal static class TimeSpanExtensions
    {
        /// <summary>
        /// Executes the multiply operation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The operation result.</returns>
        public static TimeSpan Multiply(this TimeSpan value, ulong factor)
        {
            // Used for estimating unlock times. checked to avoid silent overflow.
            return TimeSpan.FromTicks(checked(value.Ticks * (long)factor));
        }

        /// <summary>
        /// Executes the multiply operation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The operation result.</returns>
        public static TimeSpan Multiply(this TimeSpan value, uint factor)
        {
            return TimeSpan.FromTicks(checked(value.Ticks * factor));
        }

        /// <summary>
        /// Executes the multiply operation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The operation result.</returns>
        public static TimeSpan Multiply(this TimeSpan value, long factor)
        {
            return TimeSpan.FromTicks(checked(value.Ticks * factor));
        }

        /// <summary>
        /// Executes the divide to u int 64 operation.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator.</param>
        /// <returns>The operation result.</returns>
        public static ulong DivideToUInt64(this TimeSpan numerator, TimeSpan denominator)
        {
            if (denominator.Ticks == 0)
            {
                throw new DivideByZeroException("TimeSpan denominator must be non-zero.");
            }

            return (ulong)(numerator.Ticks / denominator.Ticks);
        }
    }
}
