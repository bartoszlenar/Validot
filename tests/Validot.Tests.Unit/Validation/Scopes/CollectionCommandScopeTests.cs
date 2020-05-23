namespace Validot.Tests.Unit.Validation.Scopes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Validation;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class CollectionCommandScopeTests
    {
        public class TestItem
        {
        }

        public class TestCollection<T> : IEnumerable<T>
        {
            private List<T> _items;

            public TestCollection(IEnumerable<T> items)
            {
                _items = items?.ToList() ?? throw new ArgumentNullException(nameof(items));
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new CollectionCommandScope<TestCollection<TestItem>, TestItem>();
        }

        [Fact]
        public void Should_Initialize_WithDefaultValues()
        {
            var commandScope = new CollectionCommandScope<TestCollection<TestItem>, TestItem>();

            commandScope.ShouldHaveDefaultValues();
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunDiscovery(bool? shouldExecuteInfo, int? errorId, ErrorMode errorMode, string path)
        {
            var commandScope = new CollectionCommandScope<TestCollection<TestItem>, TestItem>();

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestCollection<TestItem>>)null
                : m =>
                {
                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = errorMode;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var discoveryContext = Substitute.For<IDiscoveryContext>();

            commandScope.ShouldDiscover(discoveryContext, context =>
            {
                context.Received().EnterCollectionItemPath();
                context.Received().EnterScope<TestItem>(Arg.Is(123));
                context.Received().LeavePath();
            });
        }

        public static IEnumerable<object[]> Should_RunValidation_Data()
        {
            var itemsCount = new[]
            {
                0,
                1,
                5
            };

            var sets = CommandScopeTestHelper.CommandScopeParameters();

            foreach (var count in itemsCount)
            {
                foreach (var set in sets)
                {
                    yield return new[]
                    {
                        set[0],
                        set[1],
                        set[2],
                        set[3],
                        count
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Should_RunValidation_Data))]
        public void Should_RunValidation_OnReferenceTypeItem(bool? shouldExecuteInfo, int? errorId, ErrorMode errorMode, string path, int itemsCount)
        {
            var commandScope = new CollectionCommandScope<TestCollection<TestItem>, TestItem>();

            var items = Enumerable.Range(0, itemsCount).Select(i => new TestItem()).ToList();

            var model = new TestCollection<TestItem>(items);

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestCollection<TestItem>>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = errorMode;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var itemScope = Substitute.For<IValidatable<TestItem>>();

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    for (var i = 0; i < itemsCount; ++i)
                    {
                        context.Received().EnterCollectionItemPath(Arg.Is(i));
                        context.Received().EnterScope(Arg.Is(123), Arg.Is(items[i]));
                        context.Received().LeavePath();
                    }
                });

            if (itemsCount == 0)
            {
                validationContext.DidNotReceiveWithAnyArgs().EnterCollectionItemPath(default);
                itemScope.DidNotReceiveWithAnyArgs().Validate(default, default);
            }

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }

        [Theory]
        [MemberData(nameof(Should_RunValidation_Data))]
        public void Should_RunValidation_OnValueTypeItem(bool? shouldExecuteInfo, int? errorId, ErrorMode errorMode, string path, int itemsCount)
        {
            var commandScope = new CollectionCommandScope<TestCollection<decimal>, decimal>();

            var items = Enumerable.Range(0, itemsCount).Select(i => (decimal)i).ToList();

            var model = new TestCollection<decimal>(items);

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestCollection<decimal>>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = errorMode;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var itemScope = Substitute.For<IValidatable<decimal>>();

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    for (var i = 0; i < itemsCount; ++i)
                    {
                        context.Received().EnterCollectionItemPath(Arg.Is(i));
                        context.Received().EnterScope(Arg.Is(123), Arg.Is(items[i]));
                        context.Received().LeavePath();
                    }
                });

            if (itemsCount == 0)
            {
                validationContext.DidNotReceiveWithAnyArgs().EnterCollectionItemPath(default);
                itemScope.DidNotReceiveWithAnyArgs().Validate(default, default);
            }

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }

        public static IEnumerable<object[]> Should_RunValidation_And_FallBack_Data()
        {
            var itemsCount = new[]
            {
                0,
                1,
                5
            };

            var errorIdValues = new int?[]
            {
                null,
                1
            };

            var errorModeValues = new[]
            {
                ErrorMode.Append,
                ErrorMode.Override,
            };

            var nameValues = new[]
            {
                null,
                "someName"
            };

            foreach (var count in itemsCount)
            {
                foreach (var errorId in errorIdValues)
                {
                    foreach (var errorMode in errorModeValues)
                    {
                        foreach (var name in nameValues)
                        {
                            yield return new object[]
                            {
                                errorId,
                                errorMode,
                                name,
                                count
                            };
                        }
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(Should_RunValidation_And_FallBack_Data))]
        public void Should_RunValidation_And_FallBack(int? errorId, ErrorMode errorMode, string path, int fallBackIndex)
        {
            var commandScope = new CollectionCommandScope<TestCollection<int>, int>();

            commandScope.ExecutionCondition = m => true;

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = errorMode;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var shouldFallBack = false;

            validationContext.ShouldFallBack.Returns(c => shouldFallBack);

            var validateCount = 0;

            var items = Enumerable.Range(0, 5).Select(i => i).ToList();

            validationContext.When(v => v.EnterScope(Arg.Any<int>(), Arg.Any<int>())).Do(callInfo =>
            {
                shouldFallBack = ++validateCount > fallBackIndex;
            });

            var model = new TestCollection<int>(items);

            commandScope.ShouldValidate(
                model,
                validationContext,
                true,
                context =>
                {
                    var limit = Math.Min(fallBackIndex + 1, items.Count);

                    for (var i = 0; i < limit; ++i)
                    {
                        context.Received().EnterCollectionItemPath(Arg.Is(i));
                        context.Received().EnterScope(Arg.Is(123), Arg.Is(items[i]));
                        context.Received().LeavePath();
                    }
                });
        }
    }
}
