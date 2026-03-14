using System.Text.Json.Serialization;

namespace BibXmr.Client.Network.Serialization
{
    /// <summary>
    /// Provides source-generated JSON metadata for daemon RPC payload types.
    /// </summary>
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata, PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    [JsonSerializable(typeof(DaemonJsonTypeRoots))]
    internal partial class MoneroDaemonJsonContext : JsonSerializerContext
    {
    }
}
