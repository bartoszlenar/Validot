namespace Validot.Tests.Unit.Settings.Capacities
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Settings.Capacities;

    using Xunit;

    public class CapacityInfoHelpersTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new CapacityInfoHelpers();
        }

        public class ContainsIndexes
        {
            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                Action action = () => new CapacityInfoHelpers().ContainsIndexes(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.ResolvedIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_ResolvedIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = new CapacityInfoHelpers().ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.PlaceholdersIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_PlaceholdersIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = new CapacityInfoHelpers().ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.WeirdIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_WeirdIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = new CapacityInfoHelpers().ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.InvalidIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnFalse_When_InvalidIndexes(string path)
            {
                var containsIndexes = new CapacityInfoHelpers().ContainsIndexes(path);

                containsIndexes.Should().Be(false);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.NoIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnFalse_When_NoIndexes(string path)
            {
                var containsIndexes = new CapacityInfoHelpers().ContainsIndexes(path);

                containsIndexes.Should().Be(false);
            }
        }

        [Theory]
        [MemberData(nameof(PathsTestData.GetWithIndexes.LargeIndexes), MemberType = typeof(PathsTestData.GetWithIndexes))]
        [MemberData(nameof(PathsTestData.GetWithIndexes.CommonCases), MemberType = typeof(PathsTestData.GetWithIndexes))]
        [MemberData(nameof(PathsTestData.GetWithIndexes.TrickyCases), MemberType = typeof(PathsTestData.GetWithIndexes))]
        [MemberData(nameof(PathsTestData.GetWithIndexes.LargeIndexes), MemberType = typeof(PathsTestData.GetWithIndexes))]
        public void GetWithoutIndexes(string path, IReadOnlyCollection<string> indexesStack, string pathWithIndexes)
        {
            _ = indexesStack;

            var pathWithoutIndexes = new CapacityInfoHelpers().GetWithoutIndexes(pathWithIndexes);

            pathWithoutIndexes.Should().Be(path);
        }
    }
}
