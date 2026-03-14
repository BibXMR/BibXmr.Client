using System;
using System.Text;
using BibXmr.Client.Network;
using BibXmr.Client.Network.Exceptions;

namespace BibXmr.Client.Utilities
{
    /// <summary>
    /// Provides guard helpers for wallet and RPC precondition checks.
    /// </summary>
    internal class ErrorGuard
    {
        /// <summary>
        /// Throws when an operation requires an open wallet but the wallet is closed.
        /// </summary>
        /// <param name="isWalletOpen">Whether a wallet is currently open.</param>
        /// <param name="functionName">Name of the calling operation.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="isWalletOpen"/> is <see langword="false"/>.</exception>
        public static void ThrowIfWalletNotOpen(bool isWalletOpen, string functionName)
        {
            if (!isWalletOpen)
            {
                throw new InvalidOperationException($"Wallet is not open. Cannot call {functionName}");
            }

            return;
        }

        /// <summary>
        /// Throws when an operation requires no open wallet but a wallet is already open.
        /// </summary>
        /// <param name="isWalletOpen">Whether a wallet is currently open.</param>
        /// <param name="functionName">Name of the calling operation.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="isWalletOpen"/> is <see langword="true"/>.</exception>
        public static void ThrowIfWalletOpen(bool isWalletOpen, string functionName)
        {
            if (isWalletOpen)
            {
                throw new InvalidOperationException($"Wallet is open. Cannot call {functionName}");
            }

            return;
        }

        /// <summary>
        /// Throws a protocol exception when an RPC result payload is missing.
        /// </summary>
        /// <param name="resultIsNull">Whether the expected RPC result payload is missing.</param>
        /// <param name="functionName">Name of the calling operation.</param>
        /// <exception cref="MoneroRpcProtocolException">Thrown when <paramref name="resultIsNull"/> is <see langword="true"/>.</exception>
        public static void ThrowIfResultIsNull(bool resultIsNull, string functionName)
        {
            string errorMessage = $"Error experienced when making RPC call in {functionName}.";
            if (resultIsNull)
            {
                throw new MoneroRpcProtocolException(errorMessage);
            }
        }

        /// <summary>
        /// Validates a raw RPC response envelope and throws protocol or JSON-RPC exceptions when invalid.
        /// </summary>
        /// <param name="rpcResponse">The RPC response envelope to validate.</param>
        /// <param name="functionName">Name of the calling operation.</param>
        /// <exception cref="MoneroRpcProtocolException">Thrown when required envelope fields are missing.</exception>
        /// <exception cref="JsonRpcException">Thrown when the response carries a JSON-RPC error payload.</exception>
        public static void ThrowIfResultIsNull(RpcResponse? rpcResponse, string functionName)
        {
            StringBuilder errorMessageBuilder = new($"Error experienced when making RPC call in {functionName}.");
            if (rpcResponse == null || rpcResponse.Id == null || rpcResponse.JsonRpc == null)
            {
                throw new MoneroRpcProtocolException(errorMessageBuilder.ToString());
            }
            else if (rpcResponse.ContainsError)
            {
                errorMessageBuilder.Append($" JsonRpcError: {rpcResponse.Error?.Message}");
                throw new JsonRpcException(errorMessageBuilder.ToString(), rpcResponse.Error?.JsonRpcErrorCode ?? JsonRpcErrorCode.UnknownError);
            }
        }

        /// <summary>
        /// Throws when a required string argument is null, empty, or whitespace.
        /// </summary>
        /// <param name="objectCheked">The argument value to validate.</param>
        /// <param name="parameterName">Name of the argument being validated.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="objectCheked"/> is null, empty, or whitespace.</exception>
        public static void ThrowIfNullOrWhiteSpace(string? objectCheked, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(objectCheked))
            {
                throw new InvalidOperationException($"Cannot be null or whitespace (Parameter: {parameterName})");
            }

            return;
        }

        /// <summary>
        /// Throws when unlock time exceeds the configured safety threshold.
        /// </summary>
        /// <param name="unlockTime">The requested unlock time.</param>
        /// <param name="parameterName">Name of the unlock-time argument being validated.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="unlockTime"/> exceeds <see cref="ErrorThresholds.MaximumUnlockTime"/>.</exception>
        public static void ThrowIfUnlockTimeIsToolarge(ulong unlockTime, string parameterName)
        {
            if (unlockTime > ErrorThresholds.MaximumUnlockTime)
            {
                throw new InvalidOperationException($"You are attempting to perform an action with a massive unlock time. " +
                    $"{parameterName} is too large. {parameterName} is limited to {ErrorThresholds.MaximumUnlockTime} for your safety.");
            }
        }
    }
}
