using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BibXmr.Client.Test.EndToEnd.Infrastructure;

/// <summary>
/// Provides bounded retry execution for transient network failures.
/// </summary>
internal static class RetryHelper
{
    /// <summary>
    /// Executes an asynchronous operation with retry semantics.
    /// </summary>
    /// <typeparam name="T">The operation result type.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="maxRetryCount">The maximum number of retries after the first attempt.</param>
    /// <param name="retryDelay">The delay between retries.</param>
    /// <param name="shouldRetry">A predicate that determines whether an exception is retryable.</param>
    /// <param name="logger">A logger used to emit retry diagnostics.</param>
    /// <param name="token">An optional cancellation token.</param>
    /// <returns>The operation result when successful.</returns>
    public static async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        int maxRetryCount,
        TimeSpan retryDelay,
        Func<Exception, bool> shouldRetry,
        ILogger logger,
        CancellationToken token = default)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));
        if (shouldRetry == null)
            throw new ArgumentNullException(nameof(shouldRetry));
        if (logger == null)
            throw new ArgumentNullException(nameof(logger));

        int attempt = 0;

        while (true)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                return await operation(token).ConfigureAwait(false);
            }
            catch (Exception ex) when (attempt < maxRetryCount && shouldRetry(ex))
            {
                attempt++;
                logger.LogWarning(
                    ex,
                    "Transient failure on attempt {Attempt}/{MaxAttempts}. Retrying in {DelayMilliseconds} ms.",
                    attempt,
                    maxRetryCount + 1,
                    retryDelay.TotalMilliseconds);

                if (retryDelay > TimeSpan.Zero)
                    await Task.Delay(retryDelay, token).ConfigureAwait(false);
            }
        }
    }
}

