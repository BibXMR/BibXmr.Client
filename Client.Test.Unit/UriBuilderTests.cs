using System;
using BibXmr.Client.Network;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>UriBuilder</c> behavior.
    /// </summary>
    public class UriBuilderTests
    {
        [Fact]
        public void UriBuilder_BuildsExpectedUri()
        {
            var builder = new BibXmr.Client.Network.UriBuilder("example.com", 123u, "json_rpc");
            Uri uri = builder.Build();
            Assert.Equal(new Uri("http://example.com:123/json_rpc"), uri);
        }

        [Fact]
        public void UriBuilderDirector_DelegatesBuild()
        {
            IUriBuilder inner = new BibXmr.Client.Network.UriBuilder("localhost", 1u, "x");
            var director = new UriBuilderDirector(inner);
            Assert.Equal(new Uri("http://localhost:1/x"), director.Build());
        }
    }
}


