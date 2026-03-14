using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BibXmr.Client.Test.Unit")]

namespace BibXmr.Client.Utilities
{
    /// <summary>
    /// Conversion utilities for Monero atomic units (piconero) and display units (XMR).
    /// </summary>
    public static class MoneroAmount
    {
        /// <summary>
        /// The number of piconero in one XMR (10^12).
        /// </summary>
        public const ulong PiconeroPerXmr = 1_000_000_000_000;

        /// <summary>
        /// Standard numeric format string for displaying Monero amounts with full piconero precision.
        /// </summary>
        public const string PrecisionFormat = "N12";

        private const int LowestBase = 12;

        /// <summary>
        /// Converts a piconero amount to its XMR decimal representation.
        /// </summary>
        /// <param name="amount">The amount in piconero.</param>
        /// <returns>The equivalent amount in XMR.</returns>
        public static decimal PiconeroToXmr(ulong amount)
        {
            decimal piconero = amount;
            return piconero / PiconeroPerXmr;
        }

        /// <summary>
        /// Converts an XMR decimal amount to piconero.
        /// </summary>
        /// <param name="amount">The amount in XMR.</param>
        /// <returns>The equivalent amount in piconero.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the amount is negative or has more than 12 decimal places.</exception>
        /// <exception cref="OverflowException">Thrown when the result exceeds <see cref="ulong.MaxValue"/>.</exception>
        public static ulong XmrToPiconero(decimal amount)
        {
            if (amount < decimal.Zero)
            {
                throw new InvalidOperationException("Cannot have a negative amount of Monero");
            }

            int decimalPlaces = GetDecimalPlaces(amount);
            if (decimalPlaces > LowestBase)
            {
                throw new InvalidOperationException($"{amount} has more than {LowestBase} decimal places. " +
                    $"{amount} can only have 12 decimal places at most.");
            }

            amount = amount * (decimal)Math.Pow(10, LowestBase);
            return checked((ulong)amount);
        }

        /// <summary>
        /// Formats a piconero amount as a human-readable XMR string with full precision.
        /// </summary>
        /// <param name="piconero">The amount in piconero.</param>
        /// <returns>A formatted XMR string with 12 decimal places.</returns>
        public static string FormatXmr(ulong piconero)
        {
            return PiconeroToXmr(piconero).ToString(PrecisionFormat);
        }

        /// <summary>
        /// Counts the number of decimal places in the given number by repeatedly multiplying by 10.
        /// </summary>
        /// <param name="number">The decimal number to inspect.</param>
        /// <returns>The number of decimal places.</returns>
        private static int GetDecimalPlaces(decimal number)
        {
            int decimalPlaces = 0;
            number = Math.Abs(number);
            number -= (int)number; // Remove the integer part of the number
            while (number > 0)
            {
                decimalPlaces++;
                number *= 10;
                number -= (int)number;
            }

            return decimalPlaces;
        }
    }
}
