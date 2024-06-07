namespace Validot.Tests.Unit.Rules.Collections
{
    using System;
    using System.Collections.Generic;

    public static class CollectionsTestData
    {
        public static IEnumerable<object[]> ExactCollectionSize_Should_CollectError_Data<T>(Func<int[], T> convert)
        {
            yield return new object[] { convert(Array.Empty<int>()), 0, true };
            yield return new object[] { convert(new[] { 1 }), 1, true };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), 10, true };

            yield return new object[] { convert(Array.Empty<int>()), 5, false };
            yield return new object[] { convert(new[] { 1 }), 0, false };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), 5, false };
        }

        public static IEnumerable<object[]> NotEmptyCollection_Should_CollectError_Data<T>(Func<int[], T> convert)
        {
            yield return new object[] { convert(Array.Empty<int>()), false };

            yield return new object[] { convert(new[] { 1 }), true };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), true };
        }

        public static IEnumerable<object[]> EmptyCollection_Should_CollectError_Data<T>(Func<int[], T> convert)
        {
            yield return new object[] { convert(Array.Empty<int>()), true };

            yield return new object[] { convert(new[] { 1 }), false };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), false };
        }

        public static IEnumerable<object[]> MaxCollectionSize_Should_CollectError_Data<T>(Func<int[], T> convert)
        {
            yield return new object[] { convert(Array.Empty<int>()), 0, true };
            yield return new object[] { convert(new[] { 1, 2, 3 }), 4, true };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), 10, true };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), int.MaxValue, true };

            yield return new object[] { convert(new[] { 1 }), 0, false };
            yield return new object[] { convert(new[] { 1, 2, 3, 4 }), 3, false };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), 5, false };
        }

        public static IEnumerable<object[]> MinCollectionSize_Should_CollectError_Data<T>(Func<int[], T> convert)
        {
            yield return new object[] { convert(Array.Empty<int>()), 0, true };
            yield return new object[] { convert(new[] { 1, 2, 3 }), 1, true };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), 5, true };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), 0, true };

            yield return new object[] { convert(Array.Empty<int>()), 1, false };
            yield return new object[] { convert(new[] { 1, 2, 3, 4 }), 5, false };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), int.MaxValue, false };
        }

        public static IEnumerable<object[]> CollectionSizeBetween_Should_CollectError_Data<T>(Func<int[], T> convert)
        {
            yield return new object[] { convert(Array.Empty<int>()), 0, 1, true };

            yield return new object[] { convert(new[] { 1, 2, 3 }), 0, 3, true };
            yield return new object[] { convert(new[] { 1, 2, 3 }), 1, 3, true };
            yield return new object[] { convert(new[] { 1, 2, 3 }), 2, 3, true };
            yield return new object[] { convert(new[] { 1, 2, 3 }), 3, 3, true };
            yield return new object[] { convert(new[] { 1, 2, 3 }), 3, 4, true };
            yield return new object[] { convert(new[] { 1, 2, 3 }), 3, int.MaxValue, true };

            yield return new object[] { convert(Array.Empty<int>()), 1, 2, false };

            yield return new object[] { convert(new[] { 1, 2, 3 }), 4, 10, false };
            yield return new object[] { convert(new[] { 1, 2, 3 }), 4, int.MaxValue, false };

            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), 1, 9, false };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), 5, 5, false };
            yield return new object[] { convert(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }), 11, int.MaxValue, false };
        }
    }
}
