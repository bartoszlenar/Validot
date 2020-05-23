namespace Validot.Tests.Unit.Validation.Scopes
{
    using System;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Validation;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class ModelCommandScopeTests
    {
        public class TestClass
        {
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new ModelCommandScope<TestClass>();
        }

        [Fact]
        public void Should_Initialize_WithDefaultValues()
        {
            var commandScope = new ModelCommandScope<TestClass>();

            commandScope.ShouldHaveDefaultValues();
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunDiscovery(bool? shouldExecuteInfo, int? errorId, ErrorMode errorMode, string path)
        {
            var commandScope = new ModelCommandScope<TestClass>();

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestClass>)null
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
                context.Received().EnterScope<TestClass>(Arg.Is(123));
            });
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunValidation_OnReferenceType(bool? shouldExecuteInfo, int? errorId, ErrorMode errorMode, string path)
        {
            var commandScope = new ModelCommandScope<TestClass>();

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

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    context.Received().EnterScope(Arg.Is(123), Arg.Is(model));
                });

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunValidation_OnValueType(bool? shouldExecuteInfo, int? errorId, ErrorMode errorMode, string path)
        {
            var commandScope = new ModelCommandScope<decimal>();

            decimal model = 668;

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

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    context.Received().EnterScope(Arg.Is(123), Arg.Is(model));
                });

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }
    }
}
