using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Probes a candidate daemon URI and falls back to HTTPS when the initial scheme fails.
    /// </summary>
    public static class MoneroUriResolver
    {
        /// <summary>
        /// Attempts to reach a daemon at <paramref name="candidateUri"/>; if that fails and the scheme is HTTP,
        /// retries with HTTPS on the same port (or port 443 for the default HTTP port).
        /// </summary>
        /// <param name="httpClient">An externally managed HTTP client.</param>
        /// <param name="candidateUri">The initial URI to probe.</param>
        /// <param name="options">Optional RPC client options forwarded to the probe client.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A tuple containing the resolved URI and whether HTTPS fallback was used.</returns>
        public static async Task<(Uri ResolvedUri, bool UsedHttpsFallback)> ResolveAsync(
            HttpClient httpClient,
            Uri candidateUri,
            MoneroRpcClientOptions? options = null,
            CancellationToken token = default)
        {
            if (await CanQueryInfoAsync(httpClient, candidateUri, options, token).ConfigureAwait(false))
            {
                return (candidateUri, false);
            }

            if (!string.Equals(candidateUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
            {
                return (candidateUri, false);
            }

            var builder = new System.UriBuilder(candidateUri);
            builder.Scheme = Uri.UriSchemeHttps;
            builder.Port = candidateUri.IsDefaultPort ? 443 : candidateUri.Port;

            Uri httpsUri = builder.Uri;
            if (await CanQueryInfoAsync(httpClient, httpsUri, options, token).ConfigureAwait(false))
            {
                return (httpsUri, true);
            }

            return (candidateUri, false);
        }

        /// <summary>
        /// Probes a daemon URI by attempting to create a restricted client and call <c>get_info</c>.
        /// </summary>
        /// <param name="httpClient">An externally managed HTTP client.</param>
        /// <param name="daemonUri">The URI to probe.</param>
        /// <param name="rpcOptions">Optional RPC client options.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns><see langword="true"/> if the probe succeeded; otherwise <see langword="false"/>.</returns>
        private static async Task<bool> CanQueryInfoAsync(
            HttpClient httpClient,
            Uri daemonUri,
            MoneroRpcClientOptions? rpcOptions,
            CancellationToken ct)
        {
            try
            {
                await using IRestrictedMoneroDaemonClient probe = await MoneroDaemonClient
                    .CreateRestrictedAsync(httpClient, daemonUri, rpcOptions, ct)
                    .ConfigureAwait(false);

                _ = await probe.GetDaemonInformationAsync(ct).ConfigureAwait(false);
                return true;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                return false;
            }
        }
    }
}
