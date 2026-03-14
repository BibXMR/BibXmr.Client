using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BibXmr.Client.Network.Exceptions;
using BibXmr.Client.Wallet.Dto;
using BibXmr.Client.Wallet.Dto.Requests;

namespace BibXmr.Client.Network
{
    internal partial class RpcCommunicator
    {
        /// <summary>
        /// Maps a <see cref="ConnectionType"/> to the appropriate <see cref="MoneroJsonProfile"/> and creates serializer options.
        /// </summary>
        /// <param name="connectionType">The connection type (Daemon, Wallet, or <see langword="null"/> for combined).</param>
        /// <param name="options">RPC client options providing additional type info resolver configuration.</param>
        /// <returns>Configured JSON serializer options.</returns>
        private static JsonSerializerOptions CreateSerializerOptions(ConnectionType? connectionType, MoneroRpcClientOptions options)
        {
            MoneroJsonProfile profile = connectionType switch
            {
                ConnectionType.Daemon => MoneroJsonProfile.Daemon,
                ConnectionType.Wallet => MoneroJsonProfile.Wallet,
                _ => MoneroJsonProfile.Combined,
            };

            return MoneroJson.CreateSerializerOptions(profile, options);
        }

        /// <summary>
        /// Throws a <see cref="MoneroRpcProtocolException"/> when the RPC response status is neither null, empty, nor "OK".
        /// </summary>
        /// <param name="status">The status string from the RPC response.</param>
        /// <param name="endpointOrMethod">The RPC method or endpoint name for the error message.</param>
        private static void ThrowIfNonOkStatus(string? status, string endpointOrMethod)
        {
            if (string.IsNullOrWhiteSpace(status) || string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            throw new MoneroRpcProtocolException($"Unexpected status '{status}' from Monero RPC call '{endpointOrMethod}'.");
        }

        /// <summary>
        /// Parses and validates a raw JSON params string, defaulting to an empty object when <see langword="null"/> or whitespace.
        /// Ensures the result is a JSON object or array.
        /// </summary>
        /// <param name="paramsJson">The raw JSON params string, or <see langword="null"/>.</param>
        /// <returns>A cloned <see cref="JsonElement"/> representing the normalized params.</returns>
        private static JsonElement NormalizeRawParams(string? paramsJson)
        {
            string input = string.IsNullOrWhiteSpace(paramsJson)
                ? "{}"
                : paramsJson!.Trim();

            try
            {
                using JsonDocument document = JsonDocument.Parse(input);
                if (document.RootElement.ValueKind is not (JsonValueKind.Object or JsonValueKind.Array))
                {
                    throw new ArgumentException("RPC params must be a JSON object or array.", nameof(paramsJson));
                }

                return document.RootElement.Clone();
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("RPC params must be valid JSON.", nameof(paramsJson), ex);
            }
        }

        /// <summary>
        /// Reads HTTP content as a string.
        /// </summary>
        /// <param name="content">The HTTP content to read.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The content body as a string.</returns>
        private static async Task<string> ReadContentAsStringAsync(HttpContent content, CancellationToken token)
        {
            return await content.ReadAsStringAsync(token).ConfigureAwait(false);
        }

        /// <summary>
        /// Attempts to parse the value as JSON and re-serialize it with indentation. Returns the original string if parsing fails.
        /// </summary>
        /// <param name="value">The string to pretty-print.</param>
        /// <returns>The indented JSON, or the original string if it is not valid JSON.</returns>
        private static string PrettyPrintIfJson(string value)
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(value);
                using MemoryStream buffer = new();
                using (Utf8JsonWriter writer = new(buffer, new JsonWriterOptions { Indented = true }))
                {
                    document.RootElement.WriteTo(writer);
                }

                return System.Text.Encoding.UTF8.GetString(buffer.ToArray());
            }
            catch (JsonException)
            {
                return value;
            }
        }

        /// <summary>
        /// Inspects a raw JSON-RPC response string for an "error" object and extracts the code and message if present.
        /// </summary>
        /// <param name="responseJson">The raw JSON-RPC response string.</param>
        /// <param name="isSuccess">Set to <see langword="false"/> if an error object is found.</param>
        /// <param name="errorCode">The extracted error code, or <see langword="null"/>.</param>
        /// <param name="errorMessage">The extracted error message, or <see langword="null"/>.</param>
        private static void TryExtractJsonRpcError(string responseJson, ref bool isSuccess, out int? errorCode, out string? errorMessage)
        {
            errorCode = null;
            errorMessage = null;

            try
            {
                using JsonDocument document = JsonDocument.Parse(responseJson);
                JsonElement root = document.RootElement;

                if (root.TryGetProperty("error", out JsonElement errorElement) && errorElement.ValueKind == JsonValueKind.Object)
                {
                    isSuccess = false;

                    if (errorElement.TryGetProperty("code", out JsonElement codeElement) && codeElement.TryGetInt32(out int parsedCode))
                    {
                        errorCode = parsedCode;
                    }

                    if (errorElement.TryGetProperty("message", out JsonElement messageElement))
                    {
                        errorMessage = messageElement.GetString();
                    }

                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        errorMessage = "Unknown JSON-RPC error";
                    }
                }
            }
            catch (JsonException)
            {
                isSuccess = false;
                errorMessage = "Invalid JSON response payload.";
            }
        }

        /// <summary>
        /// Converts a sequence of (address, amount) tuples into <see cref="FundTransferParameter"/> objects for the transfer RPC call.
        /// </summary>
        /// <param name="transactions">The address-amount pairs.</param>
        /// <returns>A list of fund transfer parameters.</returns>
        private static List<FundTransferParameter> TransactionToFundTransferParameter(IEnumerable<(string Address, ulong Amount)> transactions)
        {
            List<FundTransferParameter> fundTransferParameters = [];
            foreach ((string Address, ulong Amount) da in transactions)
            {
                fundTransferParameters.Add(new FundTransferParameter()
                {
                    Address = da.Address,
                    Amount = da.Amount,
                });
            }

            return fundTransferParameters;
        }

        /// <summary>
        /// Converts a sequence of (key_image, signature) tuples into <see cref="SignedKeyImage"/> objects for the <c>import_key_images</c> RPC call.
        /// </summary>
        /// <param name="signed_key_images">The key image and signature pairs.</param>
        /// <returns>A list of signed key image objects.</returns>
        private static List<SignedKeyImage> KeyImageAndSignatureToSignedKeyImages(IEnumerable<(string KeyImage, string Signature)> signed_key_images)
        {
            List<SignedKeyImage> signedKeyImages = [];
            foreach ((string KeyImage, string Signature) keyImagePair in signed_key_images)
            {
                signedKeyImages.Add(new SignedKeyImage()
                {
                    Image = keyImagePair.KeyImage,
                    Signature = keyImagePair.Signature,
                });
            }

            return signedKeyImages;
        }

        /// <summary>
        /// Converts a sequence of <see cref="MoneroTxDestination"/> objects into <see cref="FundTransferParameter"/> objects for transfer RPC calls.
        /// </summary>
        /// <param name="destinations">The transaction destinations.</param>
        /// <returns>An enumerable of fund transfer parameters.</returns>
        private static IEnumerable<FundTransferParameter> ConvertDestinations(IEnumerable<MoneroTxDestination> destinations)
        {
            return destinations.Select(static destination => new FundTransferParameter
            {
                Address = destination.Address,
                Amount = destination.Amount,
            });
        }

        /// <summary>
        /// Initializes a new instance of the ResponseDisposingStream class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        /// <param name="response">The response.</param>
        private sealed class ResponseDisposingStream(Stream inner, HttpResponseMessage response) : Stream
        {
            private readonly Stream _inner = inner;
            private readonly HttpResponseMessage _response = response;

            /// <summary>
            /// Gets or sets a value indicating whether read.
            /// </summary>
            public override bool CanRead => _inner.CanRead;

            /// <summary>
            /// Gets or sets a value indicating whether seek.
            /// </summary>
            public override bool CanSeek => _inner.CanSeek;

            /// <summary>
            /// Gets or sets a value indicating whether write.
            /// </summary>
            public override bool CanWrite => _inner.CanWrite;

            /// <summary>
            /// Gets or sets the length.
            /// </summary>
            public override long Length => _inner.Length;

            /// <summary>
            /// Gets or sets the position.
            /// </summary>
            public override long Position { get => _inner.Position; set => _inner.Position = value; }

            /// <summary>
            /// Executes the flush operation.
            /// </summary>
            public override void Flush() => _inner.Flush();

            /// <summary>
            /// Executes the read operation.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="count">The count.</param>
            /// <returns>The operation result.</returns>
            public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);

            /// <summary>
            /// Executes the seek operation.
            /// </summary>
            /// <param name="offset">The offset.</param>
            /// <param name="origin">The origin.</param>
            /// <returns>The operation result.</returns>
            public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);

            /// <summary>
            /// Sets length.
            /// </summary>
            /// <param name="value">The value.</param>
            public override void SetLength(long value) => _inner.SetLength(value);

            /// <summary>
            /// Executes the write operation.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="count">The count.</param>
            public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);

            /// <summary>
            /// Executes the read async operation.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="count">The count.</param>
            /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
            /// <returns>A task that represents the asynchronous operation and contains the result value.</returns>
            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => _inner.ReadAsync(buffer, offset, count, cancellationToken);

            /// <summary>
            /// Executes the write async operation.
            /// </summary>
            /// <param name="buffer">The buffer.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="count">The count.</param>
            /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
            /// <returns>A task that represents the asynchronous operation.</returns>
            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => _inner.WriteAsync(buffer, offset, count, cancellationToken);

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _inner.Dispose();
                    _response.Dispose();
                }

                base.Dispose(disposing);
            }
        }
    }
}
