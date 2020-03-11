namespace Validot
{
    using System;
    using System.Linq;

    internal static class TypeStringifier
    {
        // https://stackoverflow.com/a/47477303
        public static string GetFriendlyName(this Type type, bool includeNamespace = false)
        {
            var typeName = type.Name;

            if (type.IsGenericType)
            {
                var name = type.Name.Substring(0, type.Name.IndexOf('`'));
                var types = string.Join(",", type.GetGenericArguments().Select(t => t.GetFriendlyName(includeNamespace)));

                typeName = $"{name}<{types}>";
            }

            return includeNamespace
                ? $"{type.Namespace}.{typeName}"
                : typeName;
        }
    }
}
