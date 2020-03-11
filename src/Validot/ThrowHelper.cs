namespace Validot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class ThrowHelper
    {
        public static T NullArgument<T>(T argument, string name)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name);
            }

            return argument;
        }

        public static IReadOnlyCollection<T> NullInCollection<T>(IReadOnlyList<T> collection, string name)
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

            return collection;
        }

        public static IEnumerable<T> NullInCollection<T>(IEnumerable<T> collection, string name)
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

            return collection;
        }

        public static int BelowZero(int argument, string name)
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(name, argument, $"{name} cannot be less than zero");
            }

            return argument;
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
