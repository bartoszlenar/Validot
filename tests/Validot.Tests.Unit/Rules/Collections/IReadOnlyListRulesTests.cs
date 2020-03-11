namespace Validot.Tests.Unit.Rules.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class IReadOnlyListRulesTests
    {
        private static readonly Func<int[], IReadOnlyList<int>> Convert = array => array.ToList();

        public static IEnumerable<object[]> ExactCollectionSize_Should_CollectError_Data()
        {
            return CollectionsTestData.ExactCollectionSize_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(ExactCollectionSize_Should_CollectError_Data))]
        public void ExactCollectionSize_Should_CollectError(IReadOnlyList<int> model, int size, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.ExactCollectionSize(size),
                expectedIsValid,
                MessageKey.Collections.ExactCollectionSize,
                Arg.Number("size", size));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ExactCollectionSize_Should_ThrowException_When_NegativeCollectionSize(int size)
        {
            Tester.TestExceptionOnInit<IReadOnlyList<int>>(
                s => s.ExactCollectionSize(size),
                typeof(ArgumentOutOfRangeException));
        }

        public static IEnumerable<object[]> NotEmptyCollection_Should_CollectError_Data()
        {
            return CollectionsTestData.NotEmptyCollection_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(NotEmptyCollection_Should_CollectError_Data))]
        public void NotEmptyCollection_Should_CollectError(IReadOnlyList<int> model, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEmptyCollection(),
                expectedIsValid,
                MessageKey.Collections.NotEmptyCollection);
        }

        public static IEnumerable<object[]> EmptyCollection_Should_CollectError_Data()
        {
            return CollectionsTestData.EmptyCollection_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(EmptyCollection_Should_CollectError_Data))]
        public void EmptyCollection_Should_CollectError(IReadOnlyList<int> model, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.EmptyCollection(),
                expectedIsValid,
                MessageKey.Collections.EmptyCollection);
        }

        public static IEnumerable<object[]> MaxCollectionSize_Should_CollectError_Data()
        {
            return CollectionsTestData.MaxCollectionSize_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(MaxCollectionSize_Should_CollectError_Data))]
        public void MaxCollectionSize_Should_CollectError(IReadOnlyList<int> model, int max, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.MaxCollectionSize(max),
                expectedIsValid,
                MessageKey.Collections.MaxCollectionSize,
                Arg.Number("max", max));
        }

        public static IEnumerable<object[]> MinCollectionSize_Should_CollectError_Data()
        {
            return CollectionsTestData.MinCollectionSize_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(MinCollectionSize_Should_CollectError_Data))]
        public void MinCollectionSize_Should_CollectError(IReadOnlyList<int> model, int min, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.MinCollectionSize(min),
                expectedIsValid,
                MessageKey.Collections.MinCollectionSize,
                Arg.Number("min", min));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void MinCollectionSize_Should_ThrowException_When_NegativeCollectionSize(int min)
        {
            Tester.TestExceptionOnInit<IReadOnlyList<int>>(
                s => s.MinCollectionSize(min),
                typeof(ArgumentOutOfRangeException));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void MaxCollectionSize_Should_ThrowException_When_NegativeCollectionSize(int max)
        {
            Tester.TestExceptionOnInit<IReadOnlyList<int>>(
                s => s.MaxCollectionSize(max),
                typeof(ArgumentOutOfRangeException));
        }

        public static IEnumerable<object[]> CollectionSizeBetween_Should_CollectError_Data()
        {
            return CollectionsTestData.CollectionSizeBetween_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(CollectionSizeBetween_Should_CollectError_Data))]
        public void CollectionSizeBetween_Should_CollectError(IReadOnlyList<int> model, int min, int max, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.CollectionSizeBetween(min, max),
                expectedIsValid,
                MessageKey.Collections.CollectionSizeBetween,
                Arg.Number("min", min),
                Arg.Number("max", max));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void CollectionSizeBetween_Should_ThrowException_When_MaxCollectionSizeIsNegative(int max)
        {
            Tester.TestExceptionOnInit<IReadOnlyList<int>>(
                s => s.CollectionSizeBetween(0, max),
                typeof(ArgumentOutOfRangeException));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void CollectionSizeBetween_Should_ThrowException_When_MinCollectionSizeIsNegative(int min)
        {
            Tester.TestExceptionOnInit<IReadOnlyList<int>>(
                s => s.CollectionSizeBetween(min, 10),
                typeof(ArgumentOutOfRangeException));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(20, 0)]
        [InlineData(int.MaxValue, 1)]
        public void CollectionSizeBetween_Should_ThrowException_When_MinLargerThanMax(int min, int max)
        {
            Tester.TestExceptionOnInit<IReadOnlyList<int>>(
                s => s.CollectionSizeBetween(min, max),
                typeof(ArgumentException));
        }
    }
}
