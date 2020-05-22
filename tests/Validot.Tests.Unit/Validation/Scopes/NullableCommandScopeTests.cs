namespace Validot.Tests.Unit.Validation.Scopes
{
    using System;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Validation;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class NullableCommandScopeTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new NullableCommandScope<decimal>();
        }

        [Fact]
        public void Should_Initialize_WithDefaultValues()
        {
            var commandScope = new NullableCommandScope<decimal>();

            commandScope.ShouldHaveDefaultValues();
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunDiscovery(bool? shouldExecuteInfo, int? errorId, ErrorMode errorMode, string path)
        {
            var commandScope = new NullableCommandScope<decimal>();

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<decimal?>)null
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
                context.Received().EnterScope<decimal>(Arg.Is(123));
            });
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunValidation(bool? shouldExecuteInfo, int? errorId, ErrorMode errorMode, string path)
        {
            var commandScope = new NullableCommandScope<decimal>();

            var model = (decimal?)667;

            var executionCounter = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<decimal?>)null
                : m =>
                {
                    m.Should().Be(model);
                    executionCounter++;

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

            executionCounter.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_NotRunValidation_When_NullableHasNoValue(bool? shouldExecuteInfo, int? errorId, ErrorMode errorMode, string path)
        {
            var commandScope = new NullableCommandScope<decimal>();

            var executionCounter = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<decimal?>)null
                : m =>
                {
                    m.Should().BeNull();
                    executionCounter++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = errorMode;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            commandScope.ShouldValidate(
                null,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                });

            validationContext.DidNotReceiveWithAnyArgs().EnterScope<decimal>(default, default);
            validationContext.DidNotReceiveWithAnyArgs().EnterScope<decimal?>(default, default);

            executionCounter.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }
    }
}
