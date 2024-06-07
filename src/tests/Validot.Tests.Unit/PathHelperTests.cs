namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Xunit;

    public class PathHelperTests
    {
        [Fact]
        public void Should_Initialize()
        {
            PathHelper.Divider.Should().Be('.');
            PathHelper.UpperLevelPointer.Should().Be('<');
            PathHelper.CollectionIndexPrefix.Should().Be('#');
        }

        public class ResolvePath
        {
            [Fact]
            public void Should_ThrowException_When_NullBasePath()
            {
                Action action = () => PathHelper.ResolvePath(null, "new");

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullRelativePath()
            {
                Action action = () => PathHelper.ResolvePath("base", null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ReturnBasePath_When_RelativePathIsEmpty()
            {
                var result = PathHelper.ResolvePath("base.path", string.Empty);

                result.Should().Be("base.path");
            }

            [Fact]
            public void Should_ReturnNewSegment_When_BasePathIsEmpty()
            {
                var result = PathHelper.ResolvePath(string.Empty, "new.segment");

                result.Should().Be("new.segment");
            }

            [Theory]
            [MemberData(nameof(PathTestData.ResolvePath.SimpleRelativePath), MemberType = typeof(PathTestData.ResolvePath))]
            public void Should_Resolve_When_RelativePathIsSimple(string basePath, string relativePath, string expectedPath)
            {
                var result = PathHelper.ResolvePath(basePath, relativePath);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathTestData.ResolvePath.RelativePathContainsMoreLevelsDown), MemberType = typeof(PathTestData.ResolvePath))]
            public void Should_Resolve_When_RelativePathContainsMoreLevelsDown(string basePath, string relativePath, string expectedPath)
            {
                var result = PathHelper.ResolvePath(basePath, relativePath);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathTestData.ResolvePath.UncommonCharacters), MemberType = typeof(PathTestData.ResolvePath))]
            public void Should_Resolve_When_UncommonCharacters(string basePath, string relativePath, string expectedPath)
            {
                var result = PathHelper.ResolvePath(basePath, relativePath);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathTestData.ResolvePath.RelativePathGoesLevelUp), MemberType = typeof(PathTestData.ResolvePath))]
            public void Should_Resolve_When_RelativePathGoesLevelUp(string basePath, string relativePath, string expectedPath)
            {
                var result = PathHelper.ResolvePath(basePath, relativePath);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathTestData.ResolvePath.RelativePathIsEmpty_And_GoesLevelUp), MemberType = typeof(PathTestData.ResolvePath))]
            public void Should_Resolve_When_RelativePathIsEmpty_And_GoesLevelUp(string basePath, string relativePath, string expectedPath)
            {
                var result = PathHelper.ResolvePath(basePath, relativePath);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathTestData.ResolvePath.RelativePathGoesLevelUp_And_ExceedsMinimumLevel), MemberType = typeof(PathTestData.ResolvePath))]
            public void Should_ReturnNewSegment_When_RelativePathGoesLevelUp_And_ExceedsMinimumLevel(string basePath, string relativePath, string expectedPath)
            {
                var result = PathHelper.ResolvePath(basePath, relativePath);

                result.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathTestData.ResolvePath.ToSamePath), MemberType = typeof(PathTestData.ResolvePath))]
            public void Should_ReturnNewSegment_When_ToSamePath(string basePath, string relativePath, string expectedPath)
            {
                var result = PathHelper.ResolvePath(basePath, relativePath);

                result.Should().Be(expectedPath);
            }
        }

        public class GetWithIndexes
        {
            [Fact]
            public void Should_ThrowException_When_NullIndexes()
            {
                Action action = () => PathHelper.GetWithIndexes("path", null);

                action.Should().ThrowExactly<NullReferenceException>();
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetWithIndexes.LargeIndexes), MemberType = typeof(PathTestData.GetWithIndexes))]
            [MemberData(nameof(PathTestData.GetWithIndexes.CommonCases), MemberType = typeof(PathTestData.GetWithIndexes))]
            [MemberData(nameof(PathTestData.GetWithIndexes.TrickyCases), MemberType = typeof(PathTestData.GetWithIndexes))]
            [MemberData(nameof(PathTestData.GetWithIndexes.LargeIndexes), MemberType = typeof(PathTestData.GetWithIndexes))]
            public void Should_ReturnSamePath_When_NoIndexes(string path, IReadOnlyCollection<string> indexesStack, string expectedPath)
            {
                _ = indexesStack;
                _ = expectedPath;

                var resolvedPath = PathHelper.GetWithIndexes(path, Array.Empty<string>());

                resolvedPath.Should().Be(path);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetWithIndexes.CommonCases), MemberType = typeof(PathTestData.GetWithIndexes))]
            public void Should_Resolve_CommonCases(string path, IReadOnlyCollection<string> indexesStack, string expectedPath)
            {
                var resolvedPath = PathHelper.GetWithIndexes(path, indexesStack);

                resolvedPath.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetWithIndexes.TrickyCases), MemberType = typeof(PathTestData.GetWithIndexes))]
            public void Should_Resolve_TrickyCases(string path, IReadOnlyCollection<string> indexesStack, string expectedPath)
            {
                var resolvedPath = PathHelper.GetWithIndexes(path, indexesStack);

                resolvedPath.Should().Be(expectedPath);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetWithIndexes.LargeIndexes), MemberType = typeof(PathTestData.GetWithIndexes))]
            public void Should_Resolve_LargeIndexes(string path, IReadOnlyCollection<string> indexesStack, string expectedPath)
            {
                var resolvedPath = PathHelper.GetWithIndexes(path, indexesStack);

                resolvedPath.Should().Be(expectedPath);
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

            var pathWithoutIndexes = PathHelper.GetWithoutIndexes(pathWithIndexes);

            pathWithoutIndexes.Should().Be(path);
        }

        public class ContainsIndexes
        {
            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                Action action = () => PathHelper.ContainsIndexes(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.ResolvedIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_ResolvedIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = PathHelper.ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.PlaceholdersIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_PlaceholdersIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = PathHelper.ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.WeirdIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnTrue_When_WeirdIndexes(string path, int amount)
            {
                _ = amount;

                var containsIndexes = PathHelper.ContainsIndexes(path);

                containsIndexes.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.InvalidIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnFalse_When_InvalidIndexes(string path)
            {
                var containsIndexes = PathHelper.ContainsIndexes(path);

                containsIndexes.Should().Be(false);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.NoIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnFalse_When_NoIndexes(string path)
            {
                var containsIndexes = PathHelper.ContainsIndexes(path);

                containsIndexes.Should().Be(false);
            }
        }

        public class GetIndexesAmount
        {
            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                Action action = () => PathHelper.GetIndexesAmount(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.ResolvedIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_GetAmount_When_ResolvedIndexes(string path, int expectedAmount)
            {
                var amount = PathHelper.GetIndexesAmount(path);

                amount.Should().Be(expectedAmount);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.PlaceholdersIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_GetAmount_When_PlaceholdersIndexes(string path, int expectedAmount)
            {
                var amount = PathHelper.GetIndexesAmount(path);

                amount.Should().Be(expectedAmount);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.WeirdIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_GetAmount_When_WeirdIndexes(string path, int expectedAmount)
            {
                var amount = PathHelper.GetIndexesAmount(path);

                amount.Should().Be(expectedAmount);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.InvalidIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnZero_When_InvalidIndexes(string path)
            {
                var amount = PathHelper.GetIndexesAmount(path);

                amount.Should().Be(0);
            }

            [Theory]
            [MemberData(nameof(PathTestData.GetIndexesAmount.NoIndexes), MemberType = typeof(PathTestData.GetIndexesAmount))]
            public void Should_ReturnZero_When_NoIndexes(string path)
            {
                var amount = PathHelper.GetIndexesAmount(path);

                amount.Should().Be(0);
            }
        }

        public class GetLastLevel
        {
            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                Action action = () => PathHelper.GetLastLevel(null);

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
                var lastLevel = PathHelper.GetLastLevel(path);

                lastLevel.Should().Be(expectedLastLevel);
            }
        }

        public class IsValidAsPath
        {
            [Fact]
            public void Should_ReturnFalse_When_NullPath()
            {
                var isValidAsPath = PathHelper.IsValidAsPath(null);

                isValidAsPath.Should().Be(false);
            }

            [Theory]
            [MemberData(nameof(PathTestData.ValidPaths), MemberType = typeof(PathTestData))]
            public void Should_ReturnTrue_For_ValidPaths(string path)
            {
                var isValidAsPath = PathHelper.IsValidAsPath(path);

                isValidAsPath.Should().Be(true);
            }

            [Theory]
            [MemberData(nameof(PathTestData.InvalidPaths), MemberType = typeof(PathTestData))]
            public void Should_ReturnFalse_For_InvalidPaths(string path)
            {
                var isValidAsPath = PathHelper.IsValidAsPath(path);

                isValidAsPath.Should().Be(false);
            }
        }

        public class NormalizePath
        {
            [Fact]
            public void Should_ReturnSingleSpace_When_NullPath()
            {
                var normalized = PathHelper.NormalizePath(null);

                normalized.Should().Be(" ");

                PathHelper.IsValidAsPath(normalized).Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(PathTestData.NormalizePath.TrimmingInitialAngleBracts), MemberType = typeof(PathTestData.NormalizePath))]
            [MemberData(nameof(PathTestData.NormalizePath.DotsTrimmingAndSquashing), MemberType = typeof(PathTestData.NormalizePath))]
            public void Should_NormalizeInvalidPaths(string path, string expectedNormalized)
            {
                var normalized = PathHelper.NormalizePath(path);

                normalized.Should().Be(expectedNormalized);

                PathHelper.IsValidAsPath(normalized).Should().BeTrue();
            }

            [Theory]
            [InlineData("path1.path2")]
            [InlineData("path1.path2.path3")]
            [InlineData("path 1 . path2 . path3")]
            [MemberData(nameof(PathTestData.ValidPaths), MemberType = typeof(PathTestData))]
            public void Should_LeaveAsIs_If_PathIsValid(string path)
            {
                var isValidAsPath = PathHelper.IsValidAsPath(path);

                isValidAsPath.Should().Be(true);
            }
        }
    }
}