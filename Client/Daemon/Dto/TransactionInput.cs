using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BibXmr.Client.Daemon.Dto
{
    /// <summary>
    /// Represents daemon data for transaction input.
    /// </summary>
    public class TransactionInput
    {
        /// <summary>
        /// Gets or sets the collection of gens.
        /// </summary>
        [JsonPropertyName("gen")]
        public List<Gen> Gens { get; set; } = [];

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return string.Join(", ", Gens);
        }
    }

    /// <summary>
    /// Represents daemon data for gen.
    /// </summary>
    public class Gen
    {
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [JsonPropertyName("height")]
        public ulong Height { get; set; }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string representation of this instance.</returns>
        public override string ToString()
        {
            return $"{Height}";
        }
    }
}

