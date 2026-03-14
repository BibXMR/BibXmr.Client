using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BibXmr.Client.Utilities;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for transaction output.
    /// </summary>
    public class TransactionOutput
    {
        /// <summary>
        /// Gets or sets the collection of vout.
        /// </summary>
        [JsonPropertyName("vout")]
        public List<Output> Vout { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, Vout);
        }
    }

    /// <summary>
    /// Represents daemon data for output.
    /// </summary>
    public class Output
    {
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        [JsonPropertyName("target")]
        public Target Target { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{MoneroAmount.PiconeroToXmr(Amount).ToString(MoneroAmount.PrecisionFormat)} - {Target}";
        }
    }

    /// <summary>
    /// Represents daemon data for target.
    /// </summary>
    public class Target
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return Key;
        }
    }
}

