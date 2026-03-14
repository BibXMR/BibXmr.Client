using System;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Initializes a new instance of the UriBuilderDirector class.
    /// </summary>
    /// <param name="uriBuilder">The uri builder.</param>
    internal class UriBuilderDirector(IUriBuilder uriBuilder) : IUriBuilder
    {
        private readonly IUriBuilder _uriBuilder = uriBuilder;

        /// <summary>
        /// Executes the build operation.
        /// </summary>
        /// <returns>The operation result.</returns>
        public Uri Build()
        {
            return _uriBuilder.Build();
        }
    }
}
