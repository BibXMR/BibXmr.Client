using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Deserializes a JSON value that can be either a string or number into a string.
    /// </summary>
    internal sealed class JsonStringOrNumberConverter : JsonConverter<string?>
    {
        /// <inheritdoc />
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
                    throw new JsonException($"Token '{reader.TokenType}' cannot be converted to string.");
            }
        }

        /// <inheritdoc />
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
