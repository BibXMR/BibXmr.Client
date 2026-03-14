using System;
using System.Net;

namespace BibXmr.Client.Network.Exceptions
{
    /// <summary>
    /// Extension methods for classifying Monero RPC exceptions.
    /// </summary>
    public static class MoneroRpcExceptionExtensions
    {
        /// <summary>
        /// Determines whether the exception (or any inner exception) represents a JSON-RPC "method not found" error.
        /// </summary>
        /// <param name="ex">The exception to inspect.</param>
        /// <returns><see langword="true"/> if the error indicates the requested method does not exist.</returns>
        public static bool IsMethodNotFound(this Exception ex)
        {
            for (Exception? current = ex; current is not null; current = current.InnerException)
            {
                if (current is JsonRpcException jrpc && jrpc.JsonRpcErrorCode == JsonRpcErrorCode.MethodNotFound)
                {
                    return true;
                }

                if (current is MoneroRpcRemoteException remote && remote.Code == (int)JsonRpcErrorCode.MethodNotFound)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the exception (or any inner exception) indicates the endpoint is running in restricted mode.
        /// </summary>
        /// <param name="ex">The exception to inspect.</param>
        /// <returns><see langword="true"/> if the error indicates restricted access.</returns>
        public static bool IsRestricted(this Exception ex)
        {
            for (Exception? current = ex; current is not null; current = current.InnerException)
            {
                if (current is MoneroRpcHttpException httpEx && httpEx.StatusCode == HttpStatusCode.Forbidden)
                {
                    return true;
                }

                if (current.Message.Contains("restricted", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the exception (or any inner exception) indicates HTTP 401 authentication is required.
        /// </summary>
        /// <param name="ex">The exception to inspect.</param>
        /// <returns><see langword="true"/> if the error indicates authentication is required.</returns>
        public static bool IsAuthenticationRequired(this Exception ex)
        {
            for (Exception? current = ex; current is not null; current = current.InnerException)
            {
                if (current is MoneroRpcHttpException httpEx && httpEx.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Extracts the <see cref="JsonRpcErrorCode"/> from the exception chain, if present.
        /// </summary>
        /// <param name="ex">The exception to inspect.</param>
        /// <returns>The error code, or <see langword="null"/> if none was found.</returns>
        public static JsonRpcErrorCode? GetJsonRpcErrorCode(this Exception ex)
        {
            for (Exception? current = ex; current is not null; current = current.InnerException)
            {
                if (current is JsonRpcException jrpc)
                {
                    return jrpc.JsonRpcErrorCode;
                }

                if (current is MoneroRpcRemoteException remote && Enum.IsDefined(typeof(JsonRpcErrorCode), remote.Code))
                {
                    return (JsonRpcErrorCode)remote.Code;
                }
            }

            return null;
        }
    }
}
