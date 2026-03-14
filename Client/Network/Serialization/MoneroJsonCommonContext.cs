using System.Text.Json.Serialization;

namespace BibXmr.Client.Network.Serialization
{
    /// <summary>
    /// Provides source-generated JSON metadata for shared RPC payload types.
    /// </summary>
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata, PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    [JsonSerializable(typeof(CommonJsonTypeRoots))]
    internal partial class MoneroJsonCommonContext : JsonSerializerContext
    {
    }
}
