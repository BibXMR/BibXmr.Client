using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BibXmr.Client.Network
{
    /// <summary>
    /// Represents a JSON-RPC error payload.
    /// </summary>
    internal class Error
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [JsonPropertyName("data")]
        public JsonElement? Data { get; set; }

        /// <summary>
        /// Gets the mapped <see cref="JsonRpcErrorCode"/> value for the numeric RPC error code.
        /// </summary>
        [JsonIgnore]
        public JsonRpcErrorCode JsonRpcErrorCode
        {
            get
            {
                if (Code <= -32000 && Code >= -32099)
                {
                    return JsonRpcErrorCode.ServerError;
                }
                else
                {
                    if (Enum.IsDefined(typeof(JsonRpcErrorCode), Code))
                    {
                        return (JsonRpcErrorCode)Code;
                    }
                    else
                    {
                        return JsonRpcErrorCode.UnknownError;
                    }
                }
            }
        }
    }
}
