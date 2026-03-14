using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BibXmr.Client.Test.Unit.Infrastructure
{
    /// <summary>
    /// Helper utilities for HTTP-based unit tests.
    /// </summary>
    internal static class HttpTestHelpers
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public static HttpResponseMessage Json(HttpStatusCode statusCode, string json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, Utf8, "application/json"),
            };
        }

        public static HttpResponseMessage Text(HttpStatusCode statusCode, string text, string mediaType = "text/plain")
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(text, Utf8, mediaType),
            };
        }

        public static async Task<JsonDocument> ReadJsonAsync(HttpRequestMessage request, CancellationToken token)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.Content == null)
                throw new InvalidOperationException("Request.Content was null.");

            string payload = await request.Content.ReadAsStringAsync(token).ConfigureAwait(false);
            return JsonDocument.Parse(payload);
        }

        public static string GetPathAndQuery(Uri uri) => uri.PathAndQuery.TrimStart('/');
    }
}

