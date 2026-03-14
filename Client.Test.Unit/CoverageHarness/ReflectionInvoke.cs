using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BibXmr.Client.Test.Unit.CoverageHarness
{
    /// <summary>
    /// Provides reflection-based invocation helpers for coverage tests.
    /// </summary>
    internal static class ReflectionInvoke
    {
        public static IEnumerable<MethodInfo> GetInvokablePublicInstanceMethods(Type t)
        {
            return t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => !m.IsSpecialName)
                .Where(m => m.DeclaringType != typeof(object));
        }

        public static async Task InvokeAllAsync(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            MethodInfo[] methods = GetInvokablePublicInstanceMethods(target.GetType())
                .OrderBy(m => m.Name, StringComparer.Ordinal)
                .ToArray();

            foreach (MethodInfo? method in methods)
            {
                // Keep Dispose under explicit control to avoid ordering surprises.
                if (string.Equals(method.Name, "Dispose", StringComparison.Ordinal))
                    continue;

                object?[] args = CreateArguments(method);
                try
                {
                    await InvokeAsync(target, method, args).ConfigureAwait(false);
                }
                catch (NotImplementedException)
                {
                    // Some RPC surface area is explicitly not implemented yet (e.g., Verify).
                }
            }
        }

        private static object?[] CreateArguments(MethodInfo method)
        {
            ParameterInfo[] ps = method.GetParameters();
            object?[] args = new object?[ps.Length];

            for (int i = 0; i < ps.Length; i++)
                args[i] = CreateArgumentValue(ps[i]);

            // A few RPC helper methods require at least one selector flag to be true.
            if (string.Equals(method.Name, "GetTransfersAsync", StringComparison.Ordinal))
            {
                for (int i = 0; i < ps.Length; i++)
                {
                    if (ps[i].ParameterType == typeof(bool))
                    {
                        args[i] = true;
                        break;
                    }
                }
            }

            return args;
        }

        private static object? CreateArgumentValue(ParameterInfo p)
        {
            Type t = p.ParameterType;
            string name = p.Name ?? string.Empty;

            if (t == typeof(CancellationToken))
                return CancellationToken.None;

            if (t == typeof(string))
            {
                if (string.Equals(name, "paramsJson", StringComparison.Ordinal))
                    return "{}";
                return "x";
            }

            if (t == typeof(bool))
                return false;

            if (t == typeof(int))
                return 1;

            if (t == typeof(uint))
            {
                if (name.Contains("ring", StringComparison.OrdinalIgnoreCase))
                    return 2u;
                return 1u;
            }

            if (t == typeof(ulong))
                return 1ul;

            if (t == typeof(long))
                return 1L;

            if (t == typeof(Uri))
                return new Uri("http://localhost/", UriKind.Absolute);

            if (t.IsEnum)
                return Enum.GetValues(t).GetValue(0);

            if (t.IsArray)
            {
                Type elem = t.GetElementType() ?? throw new InvalidOperationException("Array element type was null.");
                var a = Array.CreateInstance(elem, 1);
                a.SetValue(CreateArgumentValue(elem, name), 0);
                return a;
            }

            // Handle IEnumerable<T>/ICollection<T>/IReadOnlyCollection<T>/IReadOnlyList<T> etc.
            Type? ienum = GetGenericIEnumerableElementType(t);
            if (ienum != null)
            {
                Type listType = typeof(List<>).MakeGenericType(ienum);
                var list = (IList)Activator.CreateInstance(listType)!;
                list.Add(CreateArgumentValue(ienum, name));
                return list;
            }

            if (IsValueTuple(t))
            {
                // Most call sites only need a non-default tuple instance.
                object?[] args = t.GetGenericArguments().Select(a => CreateArgumentValue(a, name)).ToArray();
                return Activator.CreateInstance(t, args);
            }

            // Nullable<T>
            if (Nullable.GetUnderlyingType(t) is Type u)
                return CreateArgumentValue(u, name);

            // Last resort: try to create an instance (public or non-public parameterless).
            try
            {
                return Activator.CreateInstance(t, nonPublic: true);
            }
            catch
            {
                // Some request DTOs don't have parameterless constructors; use null for reference types.
                return t.IsValueType ? Activator.CreateInstance(t) : null;
            }
        }

        private static object? CreateArgumentValue(Type t, string nameHint)
        {
            // Used for element values in arrays/lists/tuples where we don't have ParameterInfo.
            if (t == typeof(string))
                return "x";
            if (t == typeof(bool))
                return false;
            if (t == typeof(uint))
            {
                if (nameHint.Contains("ring", StringComparison.OrdinalIgnoreCase))
                    return 2u;
                return 1u;
            }
            if (t == typeof(ulong))
                return 1ul;
            if (t.IsEnum)
                return Enum.GetValues(t).GetValue(0);
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            try
            {
                return Activator.CreateInstance(t, nonPublic: true);
            }
            catch
            {
                return null;
            }
        }

        private static Type? GetGenericIEnumerableElementType(Type t)
        {
            if (t == typeof(string))
                return null;

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return t.GetGenericArguments()[0];

            Type? iface = t.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return iface?.GetGenericArguments()[0];
        }

        private static bool IsValueTuple(Type t)
        {
            if (!t.IsValueType || !t.IsGenericType)
                return false;

            Type def = t.GetGenericTypeDefinition();
            return def == typeof(ValueTuple<>) ||
                   def == typeof(ValueTuple<,>) ||
                   def == typeof(ValueTuple<,,>) ||
                   def == typeof(ValueTuple<,,,>) ||
                   def == typeof(ValueTuple<,,,,>) ||
                   def == typeof(ValueTuple<,,,,,>) ||
                   def == typeof(ValueTuple<,,,,,,>) ||
                   def == typeof(ValueTuple<,,,,,,,>);
        }

        private static async Task InvokeAsync(object target, MethodInfo method, object?[] args)
        {
            object? result;
            try
            {
                result = method.Invoke(target, args);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }

            if (result is Task t)
            {
                await t.ConfigureAwait(false);
                return;
            }
        }
    }
}

