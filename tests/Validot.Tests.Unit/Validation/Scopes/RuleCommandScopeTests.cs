namespace Validot.Tests.Unit.Validation.Scopes
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Validation;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class RuleCommandScopeTests
    {
        public class TestClass
        {
        }

        public static IEnumerable<object[]> Should_Discover_Data()
        {
            var sets = CommandScopeTestHelper.CommandScopeParameters();

            foreach (var set in sets)
            {
                if (set[1] == null)
                {
                    continue;
                }

                yield return new[]
                {
                    set[0],
                    (int)set[1],
                    set[2],
                    set[3]
                };
            }
        }

        [Theory]
        [MemberData(nameof(Should_Discover_Data))]
        public void Should_Discover(bool? shouldExecuteInfo, int errorId, ErrorMode errorMode, string path)
        {
            var commandScope = new RuleCommandScope<TestClass>();

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestClass>)null
                : m =>
                {
                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = errorMode;

            commandScope.Path = path;

            var discoveryContext = Substitute.For<IDiscoveryContext>();

            commandScope.Discover(discoveryContext);

            Received.InOrder(() =>
            {
                discoveryContext.Received().EnterPath(path);
                discoveryContext.Received().AddError(errorId);
                discoveryContext.Received().LeavePath();
            });

            discoveryContext.DidNotReceiveWithAnyArgs().EnterScope<TestClass>(default);
            discoveryContext.DidNotReceiveWithAnyArgs().EnterCollectionItemPath();
        }

        public static IEnumerable<object[]> Should_Validate_Data()
        {
            var sets = CommandScopeTestHelper.CommandScopeParameters();

            foreach (var set in sets)
            {
                if (set[1] == null)
                {
                    continue;
                }

                yield return new[]
                {
                    set[0],
                    (int)set[1],
                    set[2],
                    set[3],
                    true
                };

                yield return new[]
                {
                    set[0],
                    (int)set[1],
                    set[2],
                    set[3],
                    false
                };
            }
        }

        [Theory]
        [MemberData(nameof(Should_Validate_Data))]
        public void Should_Validate_ReferenceType(bool? shouldExecuteInfo, int errorId, ErrorMode errorMode, string path, bool isValid)
        {
            var commandScope = new RuleCommandScope<TestClass>();

            var model = new TestClass();

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestClass>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = errorMode;

            commandScope.Path = path;

            var isValidCount = 0;

            commandScope.IsValid = m =>
            {
                m.Should().BeSameAs(model);

                isValidCount++;

                return isValid;
            };

            var validationContext = Substitute.For<IValidationContext>();

            commandScope.Validate(model, validationContext);

            var shouldExecute = !shouldExecuteInfo.HasValue || shouldExecuteInfo.Value;

            if (shouldExecute)
            {
                Received.InOrder(() =>
                {
                    validationContext.Received().EnterPath(path);

                    if (!isValid)
                    {
                        validationContext.Received().AddError(errorId);
                    }

                    validationContext.Received().LeavePath();

                    isValidCount.Should().Be(1);
                });
            }
            else
            {
                validationContext.DidNotReceiveWithAnyArgs().EnterPath(default);
                validationContext.DidNotReceiveWithAnyArgs().AddError(default);
                validationContext.DidNotReceiveWithAnyArgs().LeavePath();
                isValidCount.Should().Be(0);
            }

            validationContext.DidNotReceiveWithAnyArgs().EnterCollectionItemPath(default);
            validationContext.DidNotReceiveWithAnyArgs().EnableErrorDetectionMode(default, default);

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }

        [Theory]
        [MemberData(nameof(Should_Validate_Data))]
        public void Should_Validate_ValueType(bool? shouldExecuteInfo, int errorId, ErrorMode errorMode, string path, bool isValid)
        {
            var commandScope = new RuleCommandScope<decimal>();

            var model = 234M;

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<decimal>)null
                : m =>
                {
                    m.Should().Be(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = errorMode;

            commandScope.Path = path;

            var isValidCount = 0;

            commandScope.IsValid = m =>
            {
                m.Should().Be(model);

                isValidCount++;

                return isValid;
            };

            var validationContext = Substitute.For<IValidationContext>();

            commandScope.Validate(model, validationContext);

            var shouldExecute = !shouldExecuteInfo.HasValue || shouldExecuteInfo.Value;

            if (shouldExecute)
            {
                Received.InOrder(() =>
                {
                    validationContext.Received().EnterPath(path);

                    if (!isValid)
                    {
                        validationContext.Received().AddError(errorId);
                    }

                    validationContext.Received().LeavePath();

                    isValidCount.Should().Be(1);
                });
            }
            else
            {
                validationContext.DidNotReceiveWithAnyArgs().EnterPath(default);
                validationContext.DidNotReceiveWithAnyArgs().AddError(default);
                validationContext.DidNotReceiveWithAnyArgs().LeavePath();
                isValidCount.Should().Be(0);
            }

            validationContext.DidNotReceiveWithAnyArgs().EnterCollectionItemPath(default);
            validationContext.DidNotReceiveWithAnyArgs().EnableErrorDetectionMode(default, default);

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }

        [Fact]
        public void Should_ErrorId_ThrowException_When_AssigningNull()
        {
            var commandScope = new RuleCommandScope<decimal>();

            Action action = () =>
            {
                (commandScope as ICommandScope<decimal>).ErrorId = null;
            };

            action.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Should_ErrorId_AssignValueAfterCast()
        {
            var commandScope = new RuleCommandScope<decimal>();

            commandScope.ErrorId.Should().Be(-1);
            (commandScope as ICommandScope<decimal>).ErrorId.Should().Be(-1);

            (commandScope as ICommandScope<decimal>).ErrorId = 321;

            commandScope.ErrorId.Should().Be(321);
            (commandScope as ICommandScope<decimal>).ErrorId.Should().Be(321);
        }
    }
}
