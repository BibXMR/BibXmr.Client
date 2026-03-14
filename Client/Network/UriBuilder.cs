using System;

namespace BibXmr.Client.Network
{
    internal interface IUriBuilder
    {
        Uri Build();
    }

    /// <summary>
    /// Initializes a new instance of the UriBuilder class.
    /// </summary>
    /// <param name="url">The RPC endpoint URL.</param>
    /// <param name="port">The RPC endpoint port number.</param>
    /// <param name="endpoint">The endpoint.</param>
    internal class UriBuilder(string url, uint port, string endpoint) : IUriBuilder
    {
        private readonly string _url = url;
        private readonly string _endpoint = endpoint;
        private readonly uint _port = port;

        /// <summary>
        /// Executes the build operation.
        /// </summary>
        /// <returns>The operation result.</returns>
        public Uri Build()
        {
            return new Uri($"http://{_url}:{_port}/{_endpoint}");
        }
    }
}
