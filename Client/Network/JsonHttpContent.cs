using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Serializes request bodies into JSON HTTP content with configured options.
    /// </summary>
    internal sealed class JsonHttpContent : HttpContent
    {
        private readonly byte[] _payload;

        /// <summary>
        /// Initializes a new instance of the JsonHttpContent class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <param name="options">Additional options for the operation.</param>
        public JsonHttpContent(object? value, Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo typeInfo = options.GetTypeInfo(type);
            _payload = JsonSerializer.SerializeToUtf8Bytes(value, typeInfo);
            Headers.ContentType = new MediaTypeHeaderValue(FieldAndHeaderDefaults.ApplicationJson);
            Headers.ContentLength = _payload.Length;
        }

        /// <inheritdoc />
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
            => SerializeToStreamWithTypeInfoAsync(stream);

        /// <inheritdoc />
        protected override bool TryComputeLength(out long length)
        {
            length = _payload.Length;
            return true;
        }

        /// <summary>
        /// Writes the pre-serialized JSON payload bytes to the given stream.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <returns>A task representing the asynchronous write operation.</returns>
        private Task SerializeToStreamWithTypeInfoAsync(Stream stream)
        {
            return stream.WriteAsync(_payload, 0, _payload.Length);
        }
    }
}
