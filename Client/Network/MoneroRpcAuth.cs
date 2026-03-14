using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Helper for applying HTTP Basic authentication to Monero RPC clients.
    /// </summary>
    public static class MoneroRpcAuth
    {
        /// <summary>
        /// Sets the HTTP Basic <c>Authorization</c> header on the supplied <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="httpClient">The HTTP client to configure.</param>
        /// <param name="username">The RPC username.</param>
        /// <param name="password">The optional RPC password.</param>
        public static void ApplyBasicAuth(HttpClient httpClient, string username, string? password = null)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username must not be null or empty.", nameof(username));
            }

            string value = $"{username}:{password ?? string.Empty}";
            string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        }
    }
}
