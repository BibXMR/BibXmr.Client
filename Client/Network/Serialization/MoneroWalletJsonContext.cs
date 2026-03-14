using System.Text.Json.Serialization;

namespace BibXmr.Client.Network.Serialization
{
    /// <summary>
    /// Provides source-generated JSON metadata for wallet RPC payload types.
    /// </summary>
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata, PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    [JsonSerializable(typeof(WalletJsonTypeRoots))]
    internal partial class MoneroWalletJsonContext : JsonSerializerContext
    {
    }
}
