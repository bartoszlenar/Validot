namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Xunit;

    public class PathsHelperTests
    {
        [Fact]
        public void Should_Initialize()
        {
            PathsHelper.Divider.Should().Be('.');
            PathsHelper.UpperLevelPointer.Should().Be('<');
            PathsHelper.CollectionIndexPrefix.Should().Be('#');
        }

        public class ResolveNextLevelPath
        {
            [Fact]
            public void Should_ThrowException_When_NullBasePath()
            {
                Action action = () => PathsHelper.ResolveNextLevelPath(null, "new");

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullNextSegment()
            {
                Action action = () => PathsHelper.ResolveNextLevelPath("base", null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ReturnBasePath_When_NewSegmentIsEmpty()
            {
                var result = PathsHelper.ResolveNextLevelPath("base.path", string.Empty);

                result.Should().Be("base.path");
            }

            [Fact]
            public void Should_ReturnNewSegment_When_BasePathIsEmpty()
            {
                var result = PathsHelper.ResolveNextLevelPath(string.Empty, "new.segment");

                result.Should().Be("new.segment");
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath.SimpleSegment), MemberType = typeof(PathsTestData.ResolveNextLevelPath))]
            public void Should_Resolve_When_NewSegmentIsSimple(string basePath, string newSegment, string expectedPath)
            {
                var result = PathsHelper.ResolveNextLevelPath(basePath, newSegment);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath.NewSegmentContainsMoreLevelsDown), MemberType = typeof(PathsTestData.ResolveNextLevelPath))]
            public void Should_Resolve_When_NewSegmentContainsMoreLevelsDown(string basePath, string newSegment, string expectedPath)
            {
                var result = PathsHelper.ResolveNextLevelPath(basePath, newSegment);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath.UncommonCharacters), MemberType = typeof(PathsTestData.ResolveNextLevelPath))]
            public void Should_Resolve_When_UncommonCharacters(string basePath, string newSegment, string expectedPath)
            {
                var result = PathsHelper.ResolveNextLevelPath(basePath, newSegment);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath.NewSegmentGoesLevelUp), MemberType = typeof(PathsTestData.ResolveNextLevelPath))]
            public void Should_Resolve_When_NewSegmentGoesLevelUp(string basePath, string newSegment, string expectedPath)
            {
                var result = PathsHelper.ResolveNextLevelPath(basePath, newSegment);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath.NewSegmentIsEmpty_And_GoesLevelUp), MemberType = typeof(PathsTestData.ResolveNextLevelPath))]
            public void Should_Resolve_When_NewSegmentIsEmpty_And_GoesLevelUp(string basePath, string newSegment, string expectedPath)
            {
                var result = PathsHelper.ResolveNextLevelPath(basePath, newSegment);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath.NewSegmentGoesLevelUp_And_ExceedsMinimumLevel), MemberType = typeof(PathsTestData.ResolveNextLevelPath))]
            public void Should_ReturnNewSegment_When_NewSegmentGoesLevelUp_And_ExceedsMinimumLevel(string basePath, string newSegment, string expectedPath)
            {
                var result = PathsHelper.ResolveNextLevelPath(basePath, newSegment);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath.ToSamePath), MemberType = typeof(PathsTestData.ResolveNextLevelPath))]
            public void Should_ReturnNewSegment_When_ToSamePath(string basePath, string newSegment, string expectedPath)
            {
                var result = PathsHelper.ResolveNextLevelPath(basePath, newSegment);

                result.Should().Be(expectedPath);
            }
        }

        public class GetWithIndexes
        {
            [Fact]
            public void Should_ThrowException_When_NullIndexes()
            {
                Action action = () => PathsHelper.GetWithIndexes("path", null);

                action.Should().ThrowExactly<NullReferenceException>();
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetWithIndexes.LargeIndexes), MemberType = typeof(PathsTestData.GetWithIndexes))]
            [MemberData(nameof(PathsTestData.GetWithIndexes.CommonCases), MemberType = typeof(PathsTestData.GetWithIndexes))]
            [MemberData(nameof(PathsTestData.GetWithIndexes.TrickyCases), MemberType = typeof(PathsTestData.GetWithIndexes))]
            [MemberData(nameof(PathsTestData.GetWithIndexes.LargeIndexes), MemberType = typeof(PathsTestData.GetWithIndexes))]
            public void Should_ReturnSamePath_When_NoIndexes(string path, IReadOnlyCollection<string> indexesStack, string expectedPath)
            {
                _ = indexesStack;
                _ = expectedPath;

                var resolvedPath = PathsHelper.GetWithIndexes(path, Array.Empty<string>());

                resolvedPath.Should().Be(path);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetWithIndexes.CommonCases), MemberType = typeof(PathsTestData.GetWithIndexes))]
            public void Should_Resolve_CommonCases(string path, IReadOnlyCollection<string> indexesStack, string expectedPath)
            {
                var resolvedPath = PathsHelper.GetWithIndexes(path, indexesStack);

                resolvedPath.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetWithIndexes.TrickyCases), MemberType = typeof(PathsTestData.GetWithIndexes))]
            public void Should_Resolve_TrickyCases(string path, IReadOnlyCollection<string> indexesStack, string expectedPath)
            {
                var resolvedPath = PathsHelper.GetWithIndexes(path, indexesStack);

                resolvedPath.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetWithIndexes.LargeIndexes), MemberType = typeof(PathsTestData.GetWithIndexes))]
            public void Should_Resolve_LargeIndexes(string path, IReadOnlyCollection<string> indexesStack, string expectedPath)
            {
                var resolvedPath = PathsHelper.GetWithIndexes(path, indexesStack);

                resolvedPath.Should().Be(expectedPath);
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

            var pathWithoutIndexes = PathsHelper.GetWithoutIndexes(pathWithIndexes);

            pathWithoutIndexes.Should().Be(path);
        }

        public class ContainsIndexes
        {
            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                Action action = () => PathsHelper.ContainsIndexes(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.ResolvedIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_ResolvedIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = PathsHelper.ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.PlaceholdersIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_PlaceholdersIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = PathsHelper.ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.WeirdIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_WeirdIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = PathsHelper.ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.InvalidIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnFalse_When_InvalidIndexes(string path)
            {
                var containsIndexes = PathsHelper.ContainsIndexes(path);

                containsIndexes.Should().Be(false);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.NoIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnFalse_When_NoIndexes(string path)
            {
                var containsIndexes = PathsHelper.ContainsIndexes(path);

                containsIndexes.Should().Be(false);
            }
        }

        public class GetIndexesAmount
        {
            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                Action action = () => PathsHelper.GetIndexesAmount(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.ResolvedIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_GetAmount_When_ResolvedIndexes(string path, int expectedAmount)
            {
                var amount = PathsHelper.GetIndexesAmount(path);

                amount.Should().Be(expectedAmount);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.PlaceholdersIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_GetAmount_When_PlaceholdersIndexes(string path, int expectedAmount)
            {
                var amount = PathsHelper.GetIndexesAmount(path);

                amount.Should().Be(expectedAmount);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.WeirdIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_GetAmount_When_WeirdIndexes(string path, int expectedAmount)
            {
                var amount = PathsHelper.GetIndexesAmount(path);

                amount.Should().Be(expectedAmount);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.InvalidIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnZero_When_InvalidIndexes(string path)
            {
                var amount = PathsHelper.GetIndexesAmount(path);

                amount.Should().Be(0);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetIndexesAmount.NoIndexes), MemberType = typeof(PathsTestData.GetIndexesAmount))]
            public void Should_ReturnZero_When_NoIndexes(string path)
            {
                var amount = PathsHelper.GetIndexesAmount(path);

                amount.Should().Be(0);
            }
        }

        public class GetLastLevel
        {
            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                Action action = () => PathsHelper.GetLastLevel(null);

                action.Should().ThrowExactly<NullReferenceException>();
            }

            [Theory]
            [InlineData("some.path", "path")]
            [InlineData("path", "path")]
            [InlineData("", "")]
            [InlineData("#", "#")]
            [InlineData("some.path.#", "#")]
            [InlineData("some.path.#23", "#23")]
            [InlineData("some.#123.path.#23", "#23")]
            public void Should_GetLastLevel(string path, string expectedLastLevel)
            {
                var lastLevel = PathsHelper.GetLastLevel(path);

                lastLevel.Should().Be(expectedLastLevel);
            }
        }

        public class IsValidAsPathSegment
        {
            [Fact]
            public void Should_ReturnFalse_When_NullPath()
            {
                var isValidAsPathSegment = PathsHelper.IsValidAsName(null);

                isValidAsPathSegment.Should().Be(false);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ValidPaths), MemberType = typeof(PathsTestData))]
            public void Should_ReturnTrue_For_ValidPaths(string path)
            {
                var isValidAsPathSegment = PathsHelper.IsValidAsName(path);

                isValidAsPathSegment.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.InvalidPaths), MemberType = typeof(PathsTestData))]
            public void Should_ReturnFalse_For_InvalidPaths(string path)
            {
                var isValidAsPathSegment = PathsHelper.IsValidAsName(path);

                isValidAsPathSegment.Should().Be(false);
            }
        }
    }
}
