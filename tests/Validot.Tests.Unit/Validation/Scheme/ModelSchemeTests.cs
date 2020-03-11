namespace Validot.Tests.Unit.Validation.Scheme
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Errors;
    using Validot.Settings.Capacities;
    using Validot.Validation.Scheme;
    using Validot.Validation.Scopes;

    using Xunit;

    public class ModelSchemeTests
    {
        public class TestClass
        {
        }

        internal static ModelScheme<TestClass> GetDefault()
        {
            var rootSpecificationScope = new SpecificationScope<TestClass>();

            var rootSpecificationScopeId = 10;
            var specificationScopes = new Dictionary<int, object>();
            specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);

            var errorsRegistry = new Dictionary<int, IError>();
            errorsRegistry.Add(44, new Error());
            var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
            errorsMap.Add("path", new[] { 44 });
            var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

            pathsMap.Add("", new Dictionary<string, string>()
            {
                ["path"] = "path"
            });

            var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, Substitute.For<ICapacityInfo>(), false);

            return modelScheme;
        }

        public class Initializing
        {
            public static IEnumerable<object[]> Should_Initialize_Data()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();

                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);

                var errorsRegistry = new Dictionary<int, IError>();
                errorsRegistry.Add(44, new Error());

                var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
                errorsMap.Add("path", new[] { 44 });

                var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathsMap.Add("", new Dictionary<string, string>()
                {
                    ["path"] = "path"
                });

                yield return new object[]
                {
                    rootSpecificationScope,
                    specificationScopes,
                    rootSpecificationScopeId,
                    errorsRegistry,
                    errorsMap,
                    pathsMap,
                    Substitute.For<ICapacityInfo>(),
                    false
                };
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_Initialize(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, capacityInfo, infiniteReferencesLoopsPossible);

                modelScheme.RootSpecificationScope.Should().BeSameAs(rootSpecificationScope);

                modelScheme.ErrorsRegistry.Should().BeSameAs(errorsRegistry);
                modelScheme.ErrorsMap.Should().BeSameAs(errorsMap);
                modelScheme.CapacityInfo.Should().NotBeNull();
                modelScheme.InfiniteReferencesLoopsDetected.Should().Be(infiniteReferencesLoopsPossible);
                modelScheme.RootSpecificationScopeId.Should().Be(rootSpecificationScopeId);
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullSpecificationScopes(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;
                _ = specificationScopes;

                Action action = () => new ModelScheme<TestClass>(null, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, capacityInfo, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_RootSpecificationScopeId_NotPresentInSpecificationScopes(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;
                _ = rootSpecificationScopeId;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, -1, errorsRegistry, errorsMap, pathsMap, capacityInfo, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_RootSpecificationScopeId_OfSpecificationScopeOfInvalidType(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;

                var invalidRootSpecificationScope = new SpecificationScope<object>();
                specificationScopes[rootSpecificationScopeId] = invalidRootSpecificationScope;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, capacityInfo, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullErrorsRegistry(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;
                _ = errorsRegistry;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, null, errorsMap, pathsMap, capacityInfo, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullInErrorsRegistry(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;

                errorsRegistry.Add(45, null);

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, capacityInfo, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullErrorsMap(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;
                _ = errorsMap;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, null, pathsMap, capacityInfo, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullInErrorsMap(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;

                errorsMap.Add("some_path", null);

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, capacityInfo, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullPathsMap(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;
                _ = pathsMap;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, null, capacityInfo, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullInPathsMap(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;

                pathsMap.Add("some_path", null);

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, capacityInfo, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullInPathsMapInnerDictionary(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;

                pathsMap.Add("some_path", new Dictionary<string, string>()
                {
                    ["some_entry"] = null
                });

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, capacityInfo, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullCapacityInfo(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorsRegistry, Dictionary<string, IReadOnlyList<int>> errorsMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool infiniteReferencesLoopsPossible)
            {
                _ = rootSpecificationScope;
                _ = capacityInfo;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, null, infiniteReferencesLoopsPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class GetSpecificationScope
        {
            [Fact]
            public void Should_GetSpecificationScope()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();

                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);

                var errorsRegistry = new Dictionary<int, IError>();
                errorsRegistry.Add(44, new Error());
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
                errorsMap.Add("path", new[] { 44 });
                var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathsMap.Add("", new Dictionary<string, string>()
                {
                    ["path"] = "path"
                });

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, Substitute.For<ICapacityInfo>(), false);

                var scope = modelScheme.GetSpecificationScope<TestClass>(10);

                scope.Should().BeSameAs(rootSpecificationScope);
            }

            [Fact]
            public void Should_GetSpecificationScope_MultipleTimes()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();

                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);

                var errorsRegistry = new Dictionary<int, IError>();
                errorsRegistry.Add(44, new Error());
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
                errorsMap.Add("path", new[] { 44 });
                var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathsMap.Add("", new Dictionary<string, string>()
                {
                    ["path"] = "path"
                });

                var specificationScope1 = new SpecificationScope<TestClass>();
                specificationScopes.Add(30, specificationScope1);

                var specificationScope2 = new SpecificationScope<TestClass>();
                specificationScopes.Add(31, specificationScope2);

                var specificationScope3 = new SpecificationScope<TestClass>();
                specificationScopes.Add(32, specificationScope3);

                var specificationScope4 = new SpecificationScope<int>();
                specificationScopes.Add(33, specificationScope4);

                var specificationScope5 = new SpecificationScope<DateTimeOffset?>();
                specificationScopes.Add(34, specificationScope5);

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, Substitute.For<ICapacityInfo>(), false);

                modelScheme.GetSpecificationScope<TestClass>(30).Should().BeSameAs(specificationScope1);
                modelScheme.GetSpecificationScope<TestClass>(31).Should().BeSameAs(specificationScope2);
                modelScheme.GetSpecificationScope<TestClass>(32).Should().BeSameAs(specificationScope3);
                modelScheme.GetSpecificationScope<int>(33).Should().BeSameAs(specificationScope4);
                modelScheme.GetSpecificationScope<DateTimeOffset?>(34).Should().BeSameAs(specificationScope5);
            }

            [Fact]
            public void Should_ThrowException_When_InvalidId()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();

                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);

                var errorsRegistry = new Dictionary<int, IError>();
                errorsRegistry.Add(44, new Error());
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
                errorsMap.Add("path", new[] { 44 });
                var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathsMap.Add("", new Dictionary<string, string>()
                {
                    ["path"] = "path"
                });

                var specificationScope1 = new SpecificationScope<TestClass>();
                specificationScopes.Add(30, specificationScope1);

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, Substitute.For<ICapacityInfo>(), false);

                Action action = () => modelScheme.GetSpecificationScope<TestClass>(123321);
                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_InvalidType()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();

                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);

                var errorsRegistry = new Dictionary<int, IError>();
                errorsRegistry.Add(44, new Error());
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
                errorsMap.Add("path", new[] { 44 });
                var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathsMap.Add("", new Dictionary<string, string>()
                {
                    ["path"] = "path"
                });

                var specificationScope1 = new SpecificationScope<decimal>();
                specificationScopes.Add(30, specificationScope1);

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, Substitute.For<ICapacityInfo>(), false);

                Action action = () => modelScheme.GetSpecificationScope<TestClass>(30);
                action.Should().ThrowExactly<InvalidCastException>();
            }
        }

        public class GetPathWithIndexes
        {
            [Fact]
            public void Should_ThrowException_When_NullIndexes()
            {
                var modelScheme = GetDefault();

                Action action = () => modelScheme.GetPathWithIndexes("path", null);

                action.Should().ThrowExactly<NullReferenceException>();
            }

            [Theory]
            [InlineData("path")]
            [InlineData("path.#.segment")]
            public void Should_ReturnSamePath_When_NoIndexes(string path)
            {
                var modelScheme = GetDefault();
                var resolvedPath = modelScheme.GetPathWithIndexes(path, Array.Empty<string>());

                resolvedPath.Should().Be(path);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.GetWithIndexes_AllCases), MemberType = typeof(PathsTestData))]
            public void Should_Resolve_CommonCases(string path, IReadOnlyCollection<string> indexesStack, string expectedPath)
            {
                var modelScheme = GetDefault();
                var resolvedPath = modelScheme.GetPathWithIndexes(path, indexesStack);

                resolvedPath.Should().Be(expectedPath);
            }
        }

        public class GetPathForScope
        {
            [Fact]
            public void Should_ThrowException_When_NullNextLevel()
            {
                var modelScheme = GetDefault();

                Action action = () => modelScheme.GetPathForScope("some_path", null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                var modelScheme = GetDefault();

                Action action = () => modelScheme.GetPathForScope(null, "some_level");

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ReturnPathFromMap_IfPresent()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);
                var errorsRegistry = new Dictionary<int, IError>();
                errorsRegistry.Add(44, new Error());
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
                errorsMap.Add("path", new[] { 44 });

                var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathsMap.Add("first", new Dictionary<string, string>()
                {
                    ["second"] = "third"
                });

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, Substitute.For<ICapacityInfo>(), false);

                var path = modelScheme.GetPathForScope("first", "second");

                path.Should().Be("third");
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath_AllCases), MemberType = typeof(PathsTestData))]
            public void Should_ReturnPathFromHelper_IfNotPresent(string basePath, string newSegment, string expectedPath)
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);
                var errorsRegistry = new Dictionary<int, IError>();
                errorsRegistry.Add(44, new Error());
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
                errorsMap.Add("path", new[] { 44 });

                var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, Substitute.For<ICapacityInfo>(), false);

                var path = modelScheme.GetPathForScope(basePath, newSegment);

                path.Should().Be(expectedPath);
            }

            [Fact]
            public void Should_ReturnPathFromFromPathsHelper_IfOnlyBasePathNotPresent()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);
                var errorsRegistry = new Dictionary<int, IError>();
                errorsRegistry.Add(44, new Error());
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
                errorsMap.Add("path", new[] { 44 });

                var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathsMap.Add("first_not_present", new Dictionary<string, string>()
                {
                    ["second"] = "third"
                });

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, Substitute.For<ICapacityInfo>(), false);

                var path = modelScheme.GetPathForScope("first", "second");

                path.Should().Be("first.second");
            }

            [Fact]
            public void Should_ReturnPathFromFromPathsHelper_IfOnlyNextLevelNotPresent()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);
                var errorsRegistry = new Dictionary<int, IError>();
                errorsRegistry.Add(44, new Error());
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
                errorsMap.Add("path", new[] { 44 });

                var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathsMap.Add("first", new Dictionary<string, string>()
                {
                    ["next_level_not_present"] = "third"
                });

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, Substitute.For<ICapacityInfo>(), false);

                var path = modelScheme.GetPathForScope("first", "second");

                path.Should().Be("first.second");
            }

            [Fact]
            public void Should_ReturnPath_FroMapIfAllPresent_FromHelperIfNot()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);
                var errorsRegistry = new Dictionary<int, IError>();
                errorsRegistry.Add(44, new Error());
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>();
                errorsMap.Add("path", new[] { 44 });

                var pathsMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathsMap.Add("A", new Dictionary<string, string>()
                {
                    ["B"] = "C",
                    ["b"] = "c",
                    ["d"] = "d",
                    ["b2"] = "c2",
                    ["<X"] = "XPATH",
                });

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorsRegistry, errorsMap, pathsMap, Substitute.For<ICapacityInfo>(), false);

                modelScheme.GetPathForScope("first", "second").Should().Be("first.second");
                modelScheme.GetPathForScope("A", "B").Should().Be("C");
                modelScheme.GetPathForScope("a", "B").Should().Be("a.B");
                modelScheme.GetPathForScope("A", "B1").Should().Be("A.B1");
                modelScheme.GetPathForScope("A", "b").Should().Be("c");
                modelScheme.GetPathForScope("A", "d").Should().Be("d");

                modelScheme.GetPathForScope("A", "<X").Should().Be("XPATH");
                modelScheme.GetPathForScope("A", "<x").Should().Be("x");
            }
        }
    }
}
