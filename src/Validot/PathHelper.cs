namespace Validot
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class PathHelper
    {
        private const char UpperLevelPointerChar = '<';

        private const char DividerChar = '.';

        private const char CollectionIndexPrefixChar = '#';

        private static readonly Regex CollectionItemSegmentRegex = new Regex(@"(?<=(^|\.))#\d{0,}(?=(\.|$))", RegexOptions.Compiled);

        private static readonly string _indexInTheMiddle = $"{Divider}{CollectionIndexPrefix}{Divider}";

        private static readonly string _indexAtStart = $"{CollectionIndexPrefix}{Divider}";

        private static readonly string _indexAtEnd = $"{Divider}{CollectionIndexPrefix}";

        public static char UpperLevelPointer => UpperLevelPointerChar;

        public static char Divider => DividerChar;

        public static char CollectionIndexPrefix => CollectionIndexPrefixChar;

        public static string CollectionIndexPrefixString { get; } = char.ToString(CollectionIndexPrefix);

        public static string ResolvePath(string basePath, string relativePath)
        {
            ThrowHelper.NullArgument(basePath, nameof(basePath));
            ThrowHelper.NullArgument(relativePath, nameof(relativePath));

            if (relativePath.Length == 0)
            {
                return basePath;
            }

            if (basePath.Length == 0)
            {
                if (relativePath[0] != UpperLevelPointer)
                {
                    return relativePath;
                }
            }

            if (relativePath[0] == UpperLevelPointer)
            {
                var up = 0;

                while (++up < relativePath.Length)
                {
                    if (relativePath[up] != UpperLevelPointer)
                    {
                        break;
                    }
                }

                var newSegmentCore = relativePath.TrimStart(UpperLevelPointer);

                for (var i = basePath.Length - 2; i >= 0; --i)
                {
                    if (basePath[i] == Divider && --up == 0)
                    {
                        if (newSegmentCore.Length == 0)
                        {
                            return basePath.Substring(0, i);
                        }

                        return basePath.Substring(0, i + 1) + newSegmentCore;
                    }
                }

                return newSegmentCore;
            }

            return $"{basePath}.{relativePath}";
        }

        public static string GetWithoutIndexes(string path)
        {
            return CollectionItemSegmentRegex.Replace(path, CollectionIndexPrefixString);
        }

        public static string GetWithIndexes(string path, IReadOnlyCollection<string> indexesStack)
        {
            if (indexesStack.Count == 0)
            {
                return path;
            }

            var isSingleIndex = indexesStack.Count == 1 && path.Length == 1 && path[0] == CollectionIndexPrefix;

            var builder = !isSingleIndex ? new StringBuilder(path) : null;

            var i = 0;

            foreach (var index in indexesStack)
            {
                if (isSingleIndex)
                {
                    return FormatCollectionIndex(index);
                }

                if (i == 0 && builder.ToString().EndsWith(_indexAtEnd, StringComparison.Ordinal))
                {
                    builder.Replace(_indexAtEnd, $"{Divider}{FormatCollectionIndex(index)}", builder.Length - 2, 2);

                    continue;
                }

                var pointer = builder.ToString().LastIndexOf(_indexInTheMiddle, StringComparison.InvariantCulture);

                if (pointer != -1)
                {
                    builder.Replace(_indexInTheMiddle, $"{Divider}{FormatCollectionIndex(index)}{Divider}", pointer, 3);
                }
                else if (builder.ToString().StartsWith(_indexAtStart, StringComparison.Ordinal))
                {
                    builder.Replace(_indexAtStart, $"{FormatCollectionIndex(index)}{Divider}", 0, 2);

                    break;
                }

                ++i;
            }

            return builder.ToString();
        }

        public static bool ContainsIndexes(string path)
        {
            return CollectionItemSegmentRegex.IsMatch(path);
        }

        public static int GetIndexesAmount(string path)
        {
            return CollectionItemSegmentRegex.Matches(path).Count;
        }

        public static string GetLastLevel(string path)
        {
            var lastDividerIndex = path.LastIndexOf(Divider);

            if (lastDividerIndex == -1)
            {
                return path;
            }

            return path.Substring(lastDividerIndex + 1);
        }

        public static bool IsValidAsPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            path = path.TrimStart('<');

            if (path.StartsWith(".", StringComparison.Ordinal) ||
                path.EndsWith(".", StringComparison.Ordinal))
            {
                return false;
            }

            if (path.IndexOf("..", StringComparison.Ordinal) != -1)
            {
                return false;
            }

            return true;
        }

        private static string FormatCollectionIndex(string index)
        {
            return $"{CollectionIndexPrefix}{index}";
        }
    }
}
