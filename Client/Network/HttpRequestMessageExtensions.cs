using System;
using System.Net.Http;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Provides HttpRequestMessage extensions for Monero RPC metadata.
    /// </summary>
    internal static class HttpRequestMessageExtensions
    {
        private const string RpcMethodKey = "MoneroRpcMethod";
        private const string RequestIdKey = "MoneroRpcRequestId";

        /// <summary>
        /// Sets monero rpc method.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="method">The method.</param>
        public static void SetMoneroRpcMethod(this HttpRequestMessage request, string? method)
        {
            if (method == null)
            {
                return;
            }

            request.Options.Set(new HttpRequestOptionsKey<string>(RpcMethodKey), method);
        }

        /// <summary>
        /// Retrieves monero rpc method.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The operation result.</returns>
        public static string? GetMoneroRpcMethod(this HttpRequestMessage request)
        {
            return request.Options.TryGetValue(new HttpRequestOptionsKey<string>(RpcMethodKey), out string? v) ? v : null;
        }

        /// <summary>
        /// Sets monero rpc request id.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="id">The id.</param>
        public static void SetMoneroRpcRequestId(this HttpRequestMessage request, string? id)
        {
            if (id == null)
            {
                return;
            }

            request.Options.Set(new HttpRequestOptionsKey<string>(RequestIdKey), id);
        }

        /// <summary>
        /// Retrieves monero rpc request id.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The operation result.</returns>
        public static string? GetMoneroRpcRequestId(this HttpRequestMessage request)
        {
            return request.Options.TryGetValue(new HttpRequestOptionsKey<string>(RequestIdKey), out string? v) ? v : null;
        }
    }
}
