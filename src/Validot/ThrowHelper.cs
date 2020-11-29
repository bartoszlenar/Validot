namespace Validot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class ThrowHelper
    {
        public static void Fatal(string message)
        {
            throw new ValidotException($"Oooops! Sorry! That shouldn't have happened! {Environment.NewLine}{message}{Environment.NewLine} Please raise the issue on github with this exception stacktrace and details.");
        }

        public static void NullArgument<T>(T argument, string name)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void NullInCollection<T>(IReadOnlyList<T> collection, string name)
            where T : class
        {
            NullArgument(collection, name);

            for (var i = 0; i < collection.Count; ++i)
            {
                if (collection.ElementAt(i) == null)
                {
                    throw new ArgumentNullException($"Collection `{name}` contains null under index{i}");
                }
            }
        }

        public static void NullInCollection<T>(IEnumerable<T> collection, string name)
            where T : class
        {
            NullArgument(collection, name);

            foreach (var item in collection)
            {
                if (item == null)
                {
                    throw new ArgumentNullException($"Collection `{name}` contains null");
                }
            }
        }

        public static void BelowZero(int argument, string name)
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(name, argument, $"{name} cannot be less than zero");
            }
        }

        public static void InvalidRange(long minArgument, string minName, long maxArgument, string maxName)
        {
            if (minArgument > maxArgument)
            {
                throw new ArgumentException($"{minName} (value: {minArgument}) cannot be above {maxName} (value: {maxArgument})");
            }
        }

        public static void InvalidRange(ulong minArgument, string minName, ulong maxArgument, string maxName)
        {
            if (minArgument > maxArgument)
            {
                throw new ArgumentException($"{minName} (value: {minArgument}) cannot be above {maxName} (value: {maxArgument})");
            }
        }

        public static void InvalidRange(decimal minArgument, string minName, decimal maxArgument, string maxName)
        {
            if (minArgument > maxArgument)
            {
                throw new ArgumentException($"{minName} (value: {minArgument}) cannot be above {maxName} (value: {maxArgument})");
            }
        }

        public static void InvalidRange(double minArgument, string minName, double maxArgument, string maxName)
        {
            if (minArgument > maxArgument)
            {
                throw new ArgumentException($"{minName} (value: {minArgument}) cannot be above {maxName} (value: {maxArgument})");
            }
        }
    }
}
