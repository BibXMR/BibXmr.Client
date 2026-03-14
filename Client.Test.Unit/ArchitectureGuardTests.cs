using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>ArchitectureGuard</c> behavior.
    /// </summary>
    public class ArchitectureGuardTests
    {
        private static readonly Regex NamespaceRegex = new(
            @"^\s*namespace\s+([^\s;{]+)",
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        [Fact]
        public void ProductionCode_DoesNotUseLegacySubtypeRouting()
        {
            string root = FindRepositoryRoot();
            string clientProjectRoot = Path.Combine(root, "Client");

            List<string> sourceFiles = EnumerateSourceFiles(clientProjectRoot);

            string[] blockedPatterns =
            {
                "GetRequestMessage(",
                "MoneroResponseSubType",
            };

            foreach (string file in sourceFiles)
            {
                string content = File.ReadAllText(file);
                foreach (string pattern in blockedPatterns)
                {
                    Assert.DoesNotContain(pattern, content, StringComparison.Ordinal);
                }
            }
        }

        [Fact]
        public void SourceNamespaces_FollowCurrentConvention()
        {
            string root = FindRepositoryRoot();
            AssertNamespaceConventions(Path.Combine(root, "Client"), "BibXmr.Client", enforcePathAlignment: true);
            AssertNamespaceConventions(Path.Combine(root, "Client.Test.Unit"), "BibXmr.Client.Test.Unit", enforcePathAlignment: false);
            AssertNamespaceConventions(Path.Combine(root, "Client.Test.EndToEnd"), "BibXmr.Client.Test.EndToEnd", enforcePathAlignment: false);
        }

        private static void AssertNamespaceConventions(string projectRoot, string rootNamespace, bool enforcePathAlignment)
        {
            List<string> sourceFiles = EnumerateSourceFiles(projectRoot);
            foreach (string file in sourceFiles)
            {
                string content = File.ReadAllText(file);
                Match match = NamespaceRegex.Match(content);
                if (!match.Success)
                {
                    // Some source files only contain assembly-level attributes or top-level declarations.
                    continue;
                }

                string namespaceName = match.Groups[1].Value;
                Assert.StartsWith(rootNamespace, namespaceName, StringComparison.Ordinal);
                Assert.DoesNotContain(".DTO", namespaceName, StringComparison.Ordinal);
                Assert.DoesNotContain(".E2ETests", namespaceName, StringComparison.Ordinal);
                Assert.DoesNotContain(".Tests", namespaceName, StringComparison.Ordinal);

                if (enforcePathAlignment)
                {
                    string relativeDir = Path.GetDirectoryName(Path.GetRelativePath(projectRoot, file)) ?? string.Empty;
                    string expectedPrefix = string.IsNullOrWhiteSpace(relativeDir)
                        ? rootNamespace
                        : rootNamespace + "." + relativeDir.Replace(Path.DirectorySeparatorChar, '.');

                    bool matchesPath = namespaceName.Equals(expectedPrefix, StringComparison.Ordinal)
                        || namespaceName.StartsWith(expectedPrefix + ".", StringComparison.Ordinal);
                    Assert.True(matchesPath, $"Namespace '{namespaceName}' does not align with path '{file}'. Expected prefix '{expectedPrefix}'.");
                }
            }
        }

        private static List<string> EnumerateSourceFiles(string projectRoot) =>
            Directory
                .EnumerateFiles(projectRoot, "*.cs", SearchOption.AllDirectories)
                .Where(static path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                .Where(static path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                .ToList();

        private static string FindRepositoryRoot()
        {
            string current = AppContext.BaseDirectory;
            DirectoryInfo? dir = new DirectoryInfo(current);
            while (dir != null)
            {
                string solutionPath = Path.Combine(dir.FullName, "BibXmr.Client.slnx");
                if (File.Exists(solutionPath))
                    return dir.FullName;

                dir = dir.Parent;
            }

            throw new InvalidOperationException("Could not locate repository root containing BibXmr.Client.slnx.");
        }
    }
}

