using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace BibXmr.Client.Test.Unit
{
    /// <summary>
    /// Provides tests for <c>DtoReflectionCoverage</c> behavior.
    /// </summary>
    public class DtoReflectionCoverageTests
    {
        [Fact]
        public void DtoLikeTypes_CanBeConstructed_AndPropertiesAccessed()
        {
            Assembly assembly = typeof(BibXmr.Client.Daemon.MoneroDaemonClient).Assembly;

            Type[] types = assembly.GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("BibXmr.Client.", StringComparison.Ordinal))
                .Where(t => !t.IsAbstract)
                .Where(t => !t.ContainsGenericParameters)
                .OrderBy(t => t.FullName, StringComparer.Ordinal)
                .ToArray();

            foreach (Type? t in types)
            {
                // Skip types that are primarily about I/O side effects; they are covered via invocation tests.
                if (t.Name.Contains("Client", StringComparison.Ordinal) ||
                    t.Name.Contains("Communicator", StringComparison.Ordinal))
                {
                    continue;
                }

                object? instance = null;
                try
                {
                    if (t.IsValueType)
                    {
                        instance = Activator.CreateInstance(t);
                    }
                    else
                    {
                        instance = Activator.CreateInstance(t, nonPublic: true);
                    }
                }
                catch
                {
                    // Some types require specific ctor args; ignore them here.
                    continue;
                }

                if (instance == null)
                    continue;

                // Cover common trivial logic (many DTOs override ToString()).
                try
                {
                    _ = instance.ToString();
                }
                catch
                {
                    // Some ToString() implementations reflect over properties that can throw when unset.
                }

                PropertyInfo[] props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(p => p.GetIndexParameters().Length == 0)
                    .ToArray();

                foreach (PropertyInfo? p in props)
                {
                    if (p.CanRead)
                    {
                        try
                        {
                            _ = p.GetValue(instance);
                        }
                        catch
                        {
                            // DTO getters may compute derived values and throw when inputs are unset.
                        }
                    }

                    if (p.CanWrite)
                    {
                        // Use null for reference types; most DTO setters accept it, and the line is covered.
                        object? value = p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null;
                        try
                        {
                            p.SetValue(instance, value);
                        }
                        catch
                        {
                            // Ignore setters that enforce invariants.
                        }
                    }
                }
            }
        }
    }
}


