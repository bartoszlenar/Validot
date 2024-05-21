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

    public class DictionaryCommandScopeTests
    {
        public class TestKey
        {
            public TestKey(int value)
            {
                Value = value;
            }

            public int Value { get; }

            public override bool Equals(object obj)
            {
                return obj is TestKey key &&
                       Value == key.Value;
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }

        public class TestValue
        {
        }

        public class TestDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
        {
            private readonly Dictionary<TKey, TValue> _dictionary;

            public TestDictionary(Dictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count => _dictionary.Count;

            public bool ContainsKey(TKey key)
            {
                return _dictionary.ContainsKey(key);
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return _dictionary.TryGetValue(key, out value);
            }

            public TValue this[TKey key]
            {
                get => _dictionary[key];
                set => _dictionary[key] = value;
            }

            public IEnumerable<TKey> Keys => _dictionary.Keys;

            public IEnumerable<TValue> Values => _dictionary.Values;
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new DictionaryCommandScope<TestDictionary<TestKey, TestValue>, TestKey, TestValue>();
        }

        [Fact]
        public void Should_Initialize_WithDefaultValues()
        {
            var commandScope = new DictionaryCommandScope<TestDictionary<TestKey, TestValue>, TestKey, TestValue>();

            commandScope.ShouldHaveDefaultValues();
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunDiscovery(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path)
        {
            var commandScope = new DictionaryCommandScope<TestDictionary<TestKey, TestValue>, TestKey, TestValue>();

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestDictionary<TestKey, TestValue>>)null
                : m =>
                {
                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var discoveryContext = Substitute.For<IDiscoveryContext>();

            commandScope.ShouldDiscover(discoveryContext, context =>
            {
                context.Received().EnterCollectionItemPath();
                context.Received().EnterScope<TestValue>(Arg.Is(123));
                context.Received().LeavePath();
            });
        }

        public static IEnumerable<object[]> Should_RunValidation_Data()
        {
            var itemsCount = new[]
            {
                0, 1, 5
            };

            var sets = CommandScopeTestHelper.CommandScopeParameters();

            foreach (var count in itemsCount)
            {
                foreach (var set in sets)
                {
                    yield return new[]
                    {
                        set[0], set[1], set[2], set[3], count
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Should_RunValidation_Data))]
        public void Should_RunValidation_OnReferenceClassKey(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path, int itemsCount)
        {
            var commandScope = new DictionaryCommandScope<TestDictionary<TestKey, TestValue>, TestKey, TestValue>()
            {
                KeyStringifier = key => $"REF_{key.Value}"
            };

            var items = Enumerable.Range(0, itemsCount).ToDictionary(i => new TestKey(i), _ => new TestValue());

            var model = new TestDictionary<TestKey, TestValue>(items);

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestDictionary<TestKey, TestValue>>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var itemScope = Substitute.For<IValidatable<TestValue>>();

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    for (var i = 0; i < itemsCount; ++i)
                    {
                        context.Received().EnterPath(Arg.Is("REF_" + i));
                        context.Received().EnterScope(Arg.Is(123), Arg.Is(items[new TestKey(i)]));
                        context.Received().LeavePath();
                    }
                });

            if (itemsCount == 0)
            {
                validationContext.DidNotReceiveWithAnyArgs().EnterScope(default, Arg.Any<IReadOnlyDictionary<TestKey, TestValue>>());
                itemScope.DidNotReceiveWithAnyArgs().Validate(default, default);
            }

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }

        [Theory]
        [MemberData(nameof(Should_RunValidation_Data))]
        public void Should_RunValidation_OnValueTypeKey(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path, int itemsCount)
        {
            var commandScope = new DictionaryCommandScope<IReadOnlyDictionary<int, TestValue>, int, TestValue>()
            {
                KeyStringifier = i => $"VAL_{i}"
            };

            var items = Enumerable.Range(0, itemsCount).ToDictionary(i => i, _ => new TestValue());

            var model = new Dictionary<int, TestValue>(items);

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<IReadOnlyDictionary<int, TestValue>>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var valueScope = Substitute.For<IValidatable<TestValue>>();

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    for (var i = 0; i < itemsCount; ++i)
                    {
                        var key = $"VAL_{i}";
                        context.Received().EnterPath(Arg.Is(key));
                        context.Received().EnterScope(Arg.Is(123), Arg.Is(items[i]));
                        context.Received().LeavePath();
                    }
                });

            if (itemsCount == 0)
            {
                validationContext.DidNotReceive().EnterPath(Arg.Is<string>(p => p != path));
                valueScope.DidNotReceiveWithAnyArgs().Validate(default, default);
            }

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }

        [Theory]
        [MemberData(nameof(Should_RunValidation_Data))]
        public void Should_RunValidation_OnStringKey(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path, int itemsCount)
        {
            var commandScope = new DictionaryCommandScope<IReadOnlyDictionary<string, TestValue>, string, TestValue>();

            var items = Enumerable.Range(0, itemsCount).ToDictionary(i => $"STR_{i}", _ => new TestValue());

            var model = new Dictionary<string, TestValue>(items);

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<IReadOnlyDictionary<string, TestValue>>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var valueScope = Substitute.For<IValidatable<TestValue>>();

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    for (var i = 0; i < itemsCount; ++i)
                    {
                        var key = $"STR_{i}";
                        context.Received().EnterPath(Arg.Is(key));
                        context.Received().EnterScope(Arg.Is(123), Arg.Is(items[key]));
                        context.Received().LeavePath();
                    }
                });

            if (itemsCount == 0)
            {
                validationContext.DidNotReceive().EnterPath(Arg.Is<string>(p => p != path));
                valueScope.DidNotReceiveWithAnyArgs().Validate(default, default);
            }

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }

        public static IEnumerable<object[]> Should_RunValidation_And_FallBack_Data()
        {
            var itemsCount = new[]
            {
                0, 1, 5
            };

            var errorIdValues = new int?[]
            {
                null, 1
            };

            var errorModeValues = new[]
            {
                ErrorMode.Append, ErrorMode.Override,
            };

            var nameValues = new[]
            {
                null, "someName"
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
                                errorId, errorMode, name, count
                            };
                        }
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(Should_RunValidation_And_FallBack_Data))]
        public void Should_RunValidation_And_FallBack(int? errorId, object errorModeBoxed, string path, int fallBackIndex)
        {
            var commandScope = new DictionaryCommandScope<IReadOnlyDictionary<TestKey, TestValue>, TestKey, TestValue>()
            {
                KeyStringifier = i => $"KEY_{i.Value}"
            };

            commandScope.ExecutionCondition = m => true;

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var shouldFallBack = false;

            validationContext.ShouldFallBack.Returns(c => shouldFallBack);

            var validateCount = 0;

            var items = Enumerable.Range(0, 5).ToDictionary(i => new TestKey(i), _ => new TestValue());

            var model = new TestDictionary<TestKey, TestValue>(items);

            validationContext.When(v => v.EnterScope(Arg.Any<int>(), Arg.Any<TestValue>())).Do(callInfo =>
            {
                shouldFallBack = ++validateCount > fallBackIndex;
            });

            commandScope.ShouldValidate(
                model,
                validationContext,
                true,
                context =>
                {
                    var limit = Math.Min(fallBackIndex + 1, items.Count);

                    for (var i = 0; i < limit; ++i)
                    {
                        context.Received().EnterPath(Arg.Is($"KEY_{i}"));
                        context.Received().EnterScope(Arg.Is(123), Arg.Is(items[new TestKey(i)]));
                        context.Received().LeavePath();
                    }
                });
        }

        [Theory]
        [MemberData(nameof(Should_RunValidation_Data))]
        public void Should_RunValidation_UsingKeyStringifier_When_KeyIsNotString(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path, int itemsCount)
        {
            var stringifiedKeys = new Dictionary<int, string>();

            var commandScope = new DictionaryCommandScope<IReadOnlyDictionary<int, TestValue>, int, TestValue>()
            {
                KeyStringifier = i =>
                {
                    stringifiedKeys.Add(i, $"VAL_{i}");

                    return stringifiedKeys[i];
                }
            };

            var items = Enumerable.Range(0, itemsCount).ToDictionary(i => i, _ => new TestValue());

            var model = new Dictionary<int, TestValue>(items);

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<IReadOnlyDictionary<int, TestValue>>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var valueScope = Substitute.For<IValidatable<TestValue>>();

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    for (var i = 0; i < itemsCount; ++i)
                    {
                        var key = $"VAL_{i}";
                        context.Received().EnterPath(Arg.Is(key));
                        context.Received().EnterScope(Arg.Is(123), Arg.Is(model[i]));
                        context.Received().LeavePath();
                    }
                });

            if (itemsCount == 0)
            {
                validationContext.DidNotReceive().EnterPath(Arg.Is<string>(p => p != path));
                valueScope.DidNotReceiveWithAnyArgs().Validate(default, default);
            }

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);

            if (shouldExecuteInfo == true)
            {
                stringifiedKeys.Count.Should().Be(itemsCount);

                for (var i = 0; i < itemsCount; ++i)
                {
                    stringifiedKeys[i].Should().Be($"VAL_{i}");
                }
            }
        }

        [Theory]
        [MemberData(nameof(Should_RunValidation_Data))]
        public void Should_RunValidation_UsingKeyStringifier_When_KeyIsString(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path, int itemsCount)
        {
            var stringifiedKeys = new Dictionary<string, string>();

            var commandScope = new DictionaryCommandScope<IReadOnlyDictionary<string, TestValue>, string, TestValue>()
            {
                KeyStringifier = i =>
                {
                    stringifiedKeys.Add($"{i}", $"STR_{i}");

                    return stringifiedKeys[$"{i}"];
                }
            };

            var items = Enumerable.Range(0, itemsCount).ToDictionary(i => $"{i}", _ => new TestValue());

            var model = new Dictionary<string, TestValue>(items);

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<IReadOnlyDictionary<string, TestValue>>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var valueScope = Substitute.For<IValidatable<TestValue>>();

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    for (var i = 0; i < itemsCount; ++i)
                    {
                        var key = $"STR_{i}";
                        context.Received().EnterPath(Arg.Is(key));
                        context.Received().EnterScope(Arg.Is(123), Arg.Is(model[$"{i}"]));
                        context.Received().LeavePath();
                    }
                });

            if (itemsCount == 0)
            {
                validationContext.DidNotReceive().EnterPath(Arg.Is<string>(p => p != path));
                valueScope.DidNotReceiveWithAnyArgs().Validate(default, default);
            }

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);

            if (shouldExecuteInfo == true)
            {
                stringifiedKeys.Count.Should().Be(itemsCount);

                for (var i = 0; i < itemsCount; ++i)
                {
                    stringifiedKeys[$"{i}"].Should().Be($"STR_{i}");
                }
            }
        }

        [Theory]
        [MemberData(nameof(Should_RunValidation_Data))]
        public void Should_RunValidation_NotUsingKeyStringifier_When_KeyStringifierIsNull(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path, int itemsCount)
        {
            var commandScope = new DictionaryCommandScope<IReadOnlyDictionary<string, TestValue>, string, TestValue>();

            var items = Enumerable.Range(0, itemsCount).ToDictionary(i => $"{i}", _ => new TestValue());

            var model = new Dictionary<string, TestValue>(items);

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<IReadOnlyDictionary<string, TestValue>>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var valueScope = Substitute.For<IValidatable<TestValue>>();

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    for (var i = 0; i < itemsCount; ++i)
                    {
                        var key = $"{i}";
                        context.Received().EnterPath(Arg.Is(key));
                        context.Received().EnterScope(Arg.Is(123), Arg.Is(items[key]));
                        context.Received().LeavePath();
                    }
                });

            if (itemsCount == 0)
            {
                validationContext.DidNotReceive().EnterPath(Arg.Is<string>(p => p != path));
                valueScope.DidNotReceiveWithAnyArgs().Validate(default, default);
            }

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }
    }
}