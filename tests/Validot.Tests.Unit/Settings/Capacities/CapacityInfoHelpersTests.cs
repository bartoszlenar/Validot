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
            [MemberData(nameof(PathTestData.GetIndexesAmount.ResolvedIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_ResolvedIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = new CapacityInfoHelpers().ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.PlaceholdersIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_PlaceholdersIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = new CapacityInfoHelpers().ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.WeirdIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_WeirdIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = new CapacityInfoHelpers().ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.InvalidIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnFalse_When_InvalidIndexes(string path)
            {
                var containsIndexes = new CapacityInfoHelpers().ContainsIndexes(path);

                containsIndexes.Should().Be(false);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.NoIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnFalse_When_NoIndexes(string path)
            {
                var containsIndexes = new CapacityInfoHelpers().ContainsIndexes(path);

                containsIndexes.Should().Be(false);
            }
        }

        [Theory]
        [MemberData(nameof(PathTestData.GetWithIndexes.LargeIndexes), MemberType = typeof(PathTestData.GetWithIndexes))]
        [MemberData(nameof(PathTestData.GetWithIndexes.CommonCases), MemberType = typeof(PathTestData.GetWithIndexes))]
        [MemberData(nameof(PathTestData.GetWithIndexes.TrickyCases), MemberType = typeof(PathTestData.GetWithIndexes))]
        [MemberData(nameof(PathTestData.GetWithIndexes.LargeIndexes), MemberType = typeof(PathTestData.GetWithIndexes))]
        public void GetWithoutIndexes(string path, IReadOnlyCollection<string> indexesStack, string pathWithIndexes)
        {
            _ = indexesStack;

            var pathWithoutIndexes = new CapacityInfoHelpers().GetWithoutIndexes(pathWithIndexes);

            pathWithoutIndexes.Should().Be(path);
        }
    }
}
