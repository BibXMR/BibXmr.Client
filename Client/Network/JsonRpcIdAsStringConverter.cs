using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Converts JSON-RPC id values to strings during serialization and deserialization.
    /// </summary>
    internal sealed class JsonRpcIdAsStringConverter : JsonConverter<string?>
    {
        /// <summary>
        /// Executes the read operation.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">Additional options for the operation.</param>
        /// <returns>The operation result.</returns>
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString();

                case JsonTokenType.Number:
                    using (JsonDocument document = JsonDocument.ParseValue(ref reader))
                    {
                        return document.RootElement.GetRawText();
                    }

                case JsonTokenType.Null:
                    return null;

                default:
                    throw new JsonException($"JSON-RPC id token '{reader.TokenType}' is not supported.");
            }
        }

        /// <summary>
        /// Executes the write operation.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        /// <param name="options">Additional options for the operation.</param>
        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value);
        }
    }
}
