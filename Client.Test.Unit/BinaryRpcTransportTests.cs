using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BibXmr.Client.Daemon;
using BibXmr.Client.Network;
using BibXmr.Client.Network.Exceptions;
using BibXmr.Client.Test.Unit.Infrastructure;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>BinaryRpcTransport</c> behavior.
    /// </summary>
    public class BinaryRpcTransportTests
    {
        [Fact]
        public async Task BinaryEndpoints_UseExpectedPath_AndPassThroughRawPayload()
        {
            var calls = new (string path, Func<MoneroDaemonClient, ReadOnlyMemory<byte>, Task<BibXmr.Client.Network.BinaryRpcPayload>> invoke)[]
            {
                ("get_blocks.bin", (c, b) => c.GetBlocksBinaryAsync(b)),
                ("get_blocks_by_height.bin", (c, b) => c.GetBlocksByHeightBinaryAsync(b)),
                ("get_hashes.bin", (c, b) => c.GetHashesBinaryAsync(b)),
                ("get_o_indexes.bin", (c, b) => c.GetOutputIndexesBinaryAsync(b)),
                ("get_outs.bin", (c, b) => c.GetOutputsBinaryAsync(b)),
                ("get_transaction_pool_hashes.bin", (c, b) => c.GetTransactionPoolHashesBinaryAsync(b)),
                ("get_output_distribution.bin", (c, b) => c.GetOutputDistributionBinaryAsync(b)),
            };

            int callIndex = 0;
            var handler = new RecordingHttpMessageHandler(async (req, ct) =>
            {
                (string path, Func<MoneroDaemonClient, ReadOnlyMemory<byte>, Task<BinaryRpcPayload>> invoke) expected = calls[callIndex];
                Assert.Equal(expected.path, HttpTestHelpers.GetPathAndQuery(req.RequestUri!));

                Assert.NotNull(req.Content);
                Assert.Equal("application/octet-stream", req.Content!.Headers.ContentType!.MediaType);

                byte[] requestBytes = await req.Content.ReadAsByteArrayAsync(ct);
                Assert.Equal(new byte[] { 1, 2, (byte)callIndex }, requestBytes);

                byte[] responseBytes = new byte[] { 9, 8, (byte)callIndex };
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(responseBytes),
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Headers.Add("X-Test", $"v{callIndex}");
                callIndex++;
                return response;
            });

            using var httpClient = new HttpClient(handler);
            using MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            for (int i = 0; i < calls.Length; i++)
            {
                BinaryRpcPayload payload = await calls[i].invoke(client, new byte[] { 1, 2, (byte)i });
                Assert.Equal(HttpStatusCode.OK, payload.StatusCode);
                Assert.Equal("application/octet-stream", payload.ContentType);
                Assert.Equal(new byte[] { 9, 8, (byte)i }, payload.Body);
                Assert.True(payload.Headers.TryGetValue("X-Test", out IReadOnlyList<string>? values));
                Assert.Equal($"v{i}", values.Single());
            }
        }

        [Fact]
        public async Task BinaryEndpoint_NonSuccessHttp_ThrowsMoneroRpcHttpException()
        {
            var handler = new RecordingHttpMessageHandler((req, ct) =>
            {
                return Task.FromResult(HttpTestHelpers.Text(HttpStatusCode.BadGateway, "bad gateway"));
            });

            using var httpClient = new HttpClient(handler);
            using MoneroDaemonClient client = await MoneroDaemonClient.CreateAsync(httpClient, new Uri("http://localhost:18081/"));

            MoneroRpcHttpException ex = await Assert.ThrowsAsync<MoneroRpcHttpException>(() => client.GetBlocksBinaryAsync(new byte[] { 1, 2, 3 }));
            Assert.Equal(HttpStatusCode.BadGateway, ex.StatusCode);
        }
    }
}


