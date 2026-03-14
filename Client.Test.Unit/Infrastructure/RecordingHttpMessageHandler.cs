using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BibXmr.Client.Test.Unit.Infrastructure
{
    /// <summary>
    /// A test helper <see cref="HttpMessageHandler"/> that records requests and delegates response generation.
    /// </summary>
    internal sealed class RecordingHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsync;
        private readonly ConcurrentQueue<HttpRequestMessage> _requests = new ConcurrentQueue<HttpRequestMessage>();

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="sendAsync">Delegate used to produce responses.</param>
        public RecordingHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
        {
            _sendAsync = sendAsync ?? throw new ArgumentNullException(nameof(sendAsync));
        }

        /// <summary>
        /// Gets a thread-safe snapshot of the requests observed so far.
        /// </summary>
        public HttpRequestMessage[] Requests => _requests.ToArray();

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _requests.Enqueue(request);

            HttpResponseMessage response = await _sendAsync(request, cancellationToken).ConfigureAwait(false);
            response.RequestMessage ??= request;
            return response;
        }
    }
}

