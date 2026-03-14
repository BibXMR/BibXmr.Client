using System;
using System.Net.Http;
using BibXmr.Client.Network;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>MoneroRpcAuth</c> behavior.
    /// </summary>
    public class MoneroRpcAuthTests
    {
        [Fact]
        public void ApplyBasicAuth_SetsAuthorizationHeader()
        {
            using var client = new HttpClient();
            MoneroRpcAuth.ApplyBasicAuth(client, "user", "pass");

            Assert.NotNull(client.DefaultRequestHeaders.Authorization);
            Assert.Equal("Basic", client.DefaultRequestHeaders.Authorization!.Scheme);

            string decoded = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(client.DefaultRequestHeaders.Authorization.Parameter!));
            Assert.Equal("user:pass", decoded);
        }

        [Fact]
        public void ApplyBasicAuth_NullPassword_UsesEmptyPassword()
        {
            using var client = new HttpClient();
            MoneroRpcAuth.ApplyBasicAuth(client, "admin");

            string decoded = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(client.DefaultRequestHeaders.Authorization!.Parameter!));
            Assert.Equal("admin:", decoded);
        }

        [Fact]
        public void ApplyBasicAuth_NullClient_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => MoneroRpcAuth.ApplyBasicAuth(null!, "user"));
        }

        [Fact]
        public void ApplyBasicAuth_EmptyUsername_Throws()
        {
            using var client = new HttpClient();
            Assert.Throws<ArgumentException>(() => MoneroRpcAuth.ApplyBasicAuth(client, ""));
        }
    }
}

