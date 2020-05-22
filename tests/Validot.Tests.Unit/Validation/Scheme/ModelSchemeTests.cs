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

            var errorRegistry = new Dictionary<int, IError>();
            errorRegistry.Add(44, new Error());
            var errorMap = new Dictionary<string, IReadOnlyList<int>>();
            errorMap.Add("path", new[] { 44 });
            var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

            pathMap.Add("", new Dictionary<string, string>()
            {
                ["path"] = "path"
            });

            var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, Substitute.For<ICapacityInfo>(), false);

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

                var errorRegistry = new Dictionary<int, IError>();
                errorRegistry.Add(44, new Error());

                var errorMap = new Dictionary<string, IReadOnlyList<int>>();
                errorMap.Add("path", new[] { 44 });

                var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathMap.Add("", new Dictionary<string, string>()
                {
                    ["path"] = "path"
                });

                yield return new object[]
                {
                    rootSpecificationScope,
                    specificationScopes,
                    rootSpecificationScopeId,
                    errorRegistry,
                    errorMap,
                    pathMap,
                    Substitute.For<ICapacityInfo>(),
                    false
                };
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_Initialize(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, capacityInfo, isReferenceLoopPossible);

                modelScheme.RootSpecificationScope.Should().BeSameAs(rootSpecificationScope);

                modelScheme.ErrorRegistry.Should().BeSameAs(errorRegistry);
                modelScheme.ErrorMap.Should().BeSameAs(errorMap);
                modelScheme.CapacityInfo.Should().NotBeNull();
                modelScheme.IsReferenceLoopPossible.Should().Be(isReferenceLoopPossible);
                modelScheme.RootSpecificationScopeId.Should().Be(rootSpecificationScopeId);
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullSpecificationScopes(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;
                _ = specificationScopes;

                Action action = () => new ModelScheme<TestClass>(null, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, capacityInfo, isReferenceLoopPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_RootSpecificationScopeId_NotPresentInSpecificationScopes(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;
                _ = rootSpecificationScopeId;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, -1, errorRegistry, errorMap, pathMap, capacityInfo, isReferenceLoopPossible);

                action.Should().ThrowExactly<ArgumentException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_RootSpecificationScopeId_OfSpecificationScopeOfInvalidType(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;

                var invalidRootSpecificationScope = new SpecificationScope<object>();
                specificationScopes[rootSpecificationScopeId] = invalidRootSpecificationScope;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, capacityInfo, isReferenceLoopPossible);

                action.Should().ThrowExactly<ArgumentException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullErrorRegistry(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;
                _ = errorRegistry;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, null, errorMap, pathMap, capacityInfo, isReferenceLoopPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullInErrorRegistry(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;

                errorRegistry.Add(45, null);

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, capacityInfo, isReferenceLoopPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullErrorMap(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;
                _ = errorMap;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, null, pathMap, capacityInfo, isReferenceLoopPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullInErrorMap(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;

                errorMap.Add("some_path", null);

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, capacityInfo, isReferenceLoopPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullPathMap(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;
                _ = pathMap;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, null, capacityInfo, isReferenceLoopPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullInPathMap(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;

                pathMap.Add("some_path", null);

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, capacityInfo, isReferenceLoopPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullInPathMapInnerDictionary(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;

                pathMap.Add("some_path", new Dictionary<string, string>()
                {
                    ["some_entry"] = null
                });

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, capacityInfo, isReferenceLoopPossible);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Theory]
            [MemberData(nameof(Should_Initialize_Data))]
            public void Should_ThrowException_When_Initialize_With_NullCapacityInfo(object rootSpecificationScope, Dictionary<int, object> specificationScopes, int rootSpecificationScopeId, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<int>> errorMap, Dictionary<string, IReadOnlyDictionary<string, string>> pathMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
            {
                _ = rootSpecificationScope;
                _ = capacityInfo;

                Action action = () => new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, null, isReferenceLoopPossible);

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

                var errorRegistry = new Dictionary<int, IError>();
                errorRegistry.Add(44, new Error());
                var errorMap = new Dictionary<string, IReadOnlyList<int>>();
                errorMap.Add("path", new[] { 44 });
                var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathMap.Add("", new Dictionary<string, string>()
                {
                    ["path"] = "path"
                });

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, Substitute.For<ICapacityInfo>(), false);

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

                var errorRegistry = new Dictionary<int, IError>();
                errorRegistry.Add(44, new Error());
                var errorMap = new Dictionary<string, IReadOnlyList<int>>();
                errorMap.Add("path", new[] { 44 });
                var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathMap.Add("", new Dictionary<string, string>()
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

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, Substitute.For<ICapacityInfo>(), false);

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

                var errorRegistry = new Dictionary<int, IError>();
                errorRegistry.Add(44, new Error());
                var errorMap = new Dictionary<string, IReadOnlyList<int>>();
                errorMap.Add("path", new[] { 44 });
                var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathMap.Add("", new Dictionary<string, string>()
                {
                    ["path"] = "path"
                });

                var specificationScope1 = new SpecificationScope<TestClass>();
                specificationScopes.Add(30, specificationScope1);

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, Substitute.For<ICapacityInfo>(), false);

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

                var errorRegistry = new Dictionary<int, IError>();
                errorRegistry.Add(44, new Error());
                var errorMap = new Dictionary<string, IReadOnlyList<int>>();
                errorMap.Add("path", new[] { 44 });
                var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathMap.Add("", new Dictionary<string, string>()
                {
                    ["path"] = "path"
                });

                var specificationScope1 = new SpecificationScope<decimal>();
                specificationScopes.Add(30, specificationScope1);

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, Substitute.For<ICapacityInfo>(), false);

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
            [MemberData(nameof(PathTestData.GetWithIndexes_AllCases), MemberType = typeof(PathTestData))]
            public void Should_Resolve_CommonCases(string path, IReadOnlyCollection<string> indexesStack, string expectedPath)
            {
                var modelScheme = GetDefault();
                var resolvedPath = modelScheme.GetPathWithIndexes(path, indexesStack);

                resolvedPath.Should().Be(expectedPath);
            }
        }

        public class ResolvePath
        {
            [Fact]
            public void Should_ThrowException_When_NullRelativePath()
            {
                var modelScheme = GetDefault();

                Action action = () => modelScheme.ResolvePath("some_path", null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullBasePath()
            {
                var modelScheme = GetDefault();

                Action action = () => modelScheme.ResolvePath(null, "some_level");

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ReturnPathFromMap_IfPresent()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);
                var errorRegistry = new Dictionary<int, IError>();
                errorRegistry.Add(44, new Error());
                var errorMap = new Dictionary<string, IReadOnlyList<int>>();
                errorMap.Add("path", new[] { 44 });

                var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathMap.Add("first", new Dictionary<string, string>()
                {
                    ["second"] = "third"
                });

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, Substitute.For<ICapacityInfo>(), false);

                var path = modelScheme.ResolvePath("first", "second");

                path.Should().Be("third");
            }

            [Theory]
            [MemberData(nameof(PathTestData.ResolvePath_AllCases), MemberType = typeof(PathTestData))]
            public void Should_ReturnPathFromHelper_IfNotPresent(string basePath, string newSegment, string expectedPath)
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);
                var errorRegistry = new Dictionary<int, IError>();
                errorRegistry.Add(44, new Error());
                var errorMap = new Dictionary<string, IReadOnlyList<int>>();
                errorMap.Add("path", new[] { 44 });

                var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, Substitute.For<ICapacityInfo>(), false);

                var path = modelScheme.ResolvePath(basePath, newSegment);

                path.Should().Be(expectedPath);
            }

            [Fact]
            public void Should_ReturnPathFromFromPathsHelper_IfOnlyBasePathNotPresent()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);
                var errorRegistry = new Dictionary<int, IError>();
                errorRegistry.Add(44, new Error());
                var errorMap = new Dictionary<string, IReadOnlyList<int>>();
                errorMap.Add("path", new[] { 44 });

                var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathMap.Add("first_not_present", new Dictionary<string, string>()
                {
                    ["second"] = "third"
                });

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, Substitute.For<ICapacityInfo>(), false);

                var path = modelScheme.ResolvePath("first", "second");

                path.Should().Be("first.second");
            }

            [Fact]
            public void Should_ReturnPathFromFromPathsHelper_IfOnlyNextLevelNotPresent()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);
                var errorRegistry = new Dictionary<int, IError>();
                errorRegistry.Add(44, new Error());
                var errorMap = new Dictionary<string, IReadOnlyList<int>>();
                errorMap.Add("path", new[] { 44 });

                var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathMap.Add("first", new Dictionary<string, string>()
                {
                    ["next_level_not_present"] = "third"
                });

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, Substitute.For<ICapacityInfo>(), false);

                var path = modelScheme.ResolvePath("first", "second");

                path.Should().Be("first.second");
            }

            [Fact]
            public void Should_ReturnPath_FroMapIfAllPresent_FromHelperIfNot()
            {
                var rootSpecificationScope = new SpecificationScope<TestClass>();
                var rootSpecificationScopeId = 10;
                var specificationScopes = new Dictionary<int, object>();
                specificationScopes.Add(rootSpecificationScopeId, rootSpecificationScope);
                var errorRegistry = new Dictionary<int, IError>();
                errorRegistry.Add(44, new Error());
                var errorMap = new Dictionary<string, IReadOnlyList<int>>();
                errorMap.Add("path", new[] { 44 });

                var pathMap = new Dictionary<string, IReadOnlyDictionary<string, string>>();

                pathMap.Add("A", new Dictionary<string, string>()
                {
                    ["B"] = "C",
                    ["b"] = "c",
                    ["d"] = "d",
                    ["b2"] = "c2",
                    ["<X"] = "XPATH",
                });

                var modelScheme = new ModelScheme<TestClass>(specificationScopes, rootSpecificationScopeId, errorRegistry, errorMap, pathMap, Substitute.For<ICapacityInfo>(), false);

                modelScheme.ResolvePath("first", "second").Should().Be("first.second");
                modelScheme.ResolvePath("A", "B").Should().Be("C");
                modelScheme.ResolvePath("a", "B").Should().Be("a.B");
                modelScheme.ResolvePath("A", "B1").Should().Be("A.B1");
                modelScheme.ResolvePath("A", "b").Should().Be("c");
                modelScheme.ResolvePath("A", "d").Should().Be("d");

                modelScheme.ResolvePath("A", "<X").Should().Be("XPATH");
                modelScheme.ResolvePath("A", "<x").Should().Be("x");
            }
        }
    }
}
