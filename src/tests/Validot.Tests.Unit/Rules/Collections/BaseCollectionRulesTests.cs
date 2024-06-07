namespace Validot.Tests.Unit.Rules.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class BaseCollectionRulesTests
    {
        private static readonly Func<int[], CustomCollection> Convert = array => new CustomCollection(array);

        public static IEnumerable<object[]> ExactCollectionSize_Should_CollectError_Data()
        {
            return CollectionsTestData.ExactCollectionSize_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(ExactCollectionSize_Should_CollectError_Data))]
        public void ExactCollectionSize_Should_CollectError(CustomCollection member, int size, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                member,
                m => m.ExactCollectionSize<CustomCollection, int>(size),
                expectedIsValid,
                MessageKey.Collections.ExactCollectionSize,
                Arg.Number("size", size));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ExactCollectionSize_Should_ThrowException_When_NegativeCollectionSize(int size)
        {
            Tester.TestExceptionOnInit<CustomCollection>(
                s => s.ExactCollectionSize<CustomCollection, int>(size),
                typeof(ArgumentOutOfRangeException));
        }

        public static IEnumerable<object[]> NotEmptyCollection_Should_CollectError_Data()
        {
            return CollectionsTestData.NotEmptyCollection_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(NotEmptyCollection_Should_CollectError_Data))]
        public void NotEmptyCollection_Should_CollectError(CustomCollection member, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                member,
                m => m.NotEmptyCollection<CustomCollection, int>(),
                expectedIsValid,
                MessageKey.Collections.NotEmptyCollection);
        }

        public static IEnumerable<object[]> EmptyCollection_Should_CollectError_Data()
        {
            return CollectionsTestData.EmptyCollection_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(EmptyCollection_Should_CollectError_Data))]
        public void EmptyCollection_Should_CollectError(CustomCollection member, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                member,
                m => m.EmptyCollection<CustomCollection, int>(),
                expectedIsValid,
                MessageKey.Collections.EmptyCollection);
        }

        public static IEnumerable<object[]> MaxCollectionSize_Should_CollectError_Data()
        {
            return CollectionsTestData.MaxCollectionSize_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(MaxCollectionSize_Should_CollectError_Data))]
        public void MaxCollectionSize_Should_CollectError(CustomCollection member, int max, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                member,
                m => m.MaxCollectionSize<CustomCollection, int>(max),
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
        public void MinCollectionSize_Should_CollectError(CustomCollection member, int min, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                member,
                m => m.MinCollectionSize<CustomCollection, int>(min),
                expectedIsValid,
                MessageKey.Collections.MinCollectionSize,
                Arg.Number("min", min));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void MinCollectionSize_Should_ThrowException_When_NegativeCollectionSize(int min)
        {
            Tester.TestExceptionOnInit<CustomCollection>(
                s => s.MinCollectionSize<CustomCollection, int>(min),
                typeof(ArgumentOutOfRangeException));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void MaxCollectionSize_Should_ThrowException_When_NegativeCollectionSize(int max)
        {
            Tester.TestExceptionOnInit<CustomCollection>(
                s => s.MaxCollectionSize<CustomCollection, int>(max),
                typeof(ArgumentOutOfRangeException));
        }

        public static IEnumerable<object[]> CollectionSizeBetween_Should_CollectError_Data()
        {
            return CollectionsTestData.CollectionSizeBetween_Should_CollectError_Data(Convert);
        }

        [Theory]
        [MemberData(nameof(CollectionSizeBetween_Should_CollectError_Data))]
        public void CollectionSizeBetween_Should_CollectError(CustomCollection member, int min, int max, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                member,
                m => m.CollectionSizeBetween<CustomCollection, int>(min, max),
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
            Tester.TestExceptionOnInit<CustomCollection>(
                s => s.CollectionSizeBetween<CustomCollection, int>(0, max),
                typeof(ArgumentOutOfRangeException));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void CollectionSizeBetween_Should_ThrowException_When_MinCollectionSizeIsNegative(int min)
        {
            Tester.TestExceptionOnInit<CustomCollection>(
                s => s.CollectionSizeBetween<CustomCollection, int>(min, 10),
                typeof(ArgumentOutOfRangeException));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(20, 0)]
        [InlineData(int.MaxValue, 1)]
        public void CollectionSizeBetween_Should_ThrowException_When_MinLargerThanMax(int min, int max)
        {
            Tester.TestExceptionOnInit<CustomCollection>(
                s => s.CollectionSizeBetween<CustomCollection, int>(min, max),
                typeof(ArgumentException));
        }

        public class CustomCollection : IEnumerable<int>
        {
            private readonly List<int> _source;

            public CustomCollection(IEnumerable<int> source)
            {
                _source = source.ToList();
            }

            public IEnumerator<int> GetEnumerator()
            {
                return _source.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
