using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
#if NET8_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
using BibXmr.Client.Network.Serialization;
#endif

namespace BibXmr.Client.Network
{
    internal enum MoneroJsonProfile
    {
        Combined,
        Daemon,
        Wallet,
    }

    /// <summary>
    /// Builds System.Text.Json options used by RPC transport and DTO mapping.
    /// </summary>
    internal static class MoneroJson
    {
        /// <summary>
        /// Creates serializer options.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="options">Additional options for the operation.</param>
        /// <returns>The operation result.</returns>
        public static JsonSerializerOptions CreateSerializerOptions(MoneroJsonProfile profile, MoneroRpcClientOptions? options = null)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

#if NET8_0_OR_GREATER
            serializerOptions.TypeInfoResolver = CreateTypeInfoResolver(profile, options?.AdditionalTypeInfoResolver);
#endif

            return serializerOptions;
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Builds a composite <see cref="IJsonTypeInfoResolver"/> that combines the common, profile-specific, and optional user-supplied resolvers.
        /// </summary>
        /// <param name="profile">The serialization profile (Daemon, Wallet, or Combined).</param>
        /// <param name="additionalTypeInfoResolver">An optional user-supplied resolver to append.</param>
        /// <returns>A combined type info resolver.</returns>
        private static IJsonTypeInfoResolver CreateTypeInfoResolver(MoneroJsonProfile profile, IJsonTypeInfoResolver? additionalTypeInfoResolver)
        {
            List<IJsonTypeInfoResolver> resolvers =
            [
                MoneroJsonCommonContext.Default
            ];

            switch (profile)
            {
                case MoneroJsonProfile.Daemon:
                    resolvers.Add(MoneroDaemonJsonContext.Default);
                    break;
                case MoneroJsonProfile.Wallet:
                    resolvers.Add(MoneroWalletJsonContext.Default);
                    break;
                default:
                    resolvers.Add(MoneroDaemonJsonContext.Default);
                    resolvers.Add(MoneroWalletJsonContext.Default);
                    break;
            }

            if (additionalTypeInfoResolver is not null)
            {
                resolvers.Add(additionalTypeInfoResolver);
            }

            return JsonTypeInfoResolver.Combine([.. resolvers]);
        }
#endif
    }
}
