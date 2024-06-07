namespace Validot.Tests.Unit.Validation.Scopes
{
    using System;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Validation;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class ConvertedCommandScopeTests
    {
        private class SourceClass
        {
        }

        private class TargetClass
        {
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new ConvertedCommandScope<SourceClass, TargetClass>();
        }

        [Fact]
        public void Should_Initialize_WithDefaultValues()
        {
            var commandScope = new ConvertedCommandScope<SourceClass, TargetClass>();

            commandScope.ShouldHaveDefaultValues();
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunDiscovery(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path)
        {
            var commandScope = new ConvertedCommandScope<SourceClass, TargetClass>();

            var convertedValue = new TargetClass();

            commandScope.Converter = s => convertedValue;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<SourceClass>)null
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
                context.Received().EnterScope<TargetClass>(Arg.Is(123));
            });
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunValidation_OnConvertedValue(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path)
        {
            var commandScope = new ConvertedCommandScope<SourceClass, TargetClass>();

            var source = new SourceClass();
            var target = new TargetClass();

            var shouldExecuteCount = 0;
            var convertCount = 0;

            commandScope.Converter = sourceToConvert =>
            {
                sourceToConvert.Should().BeSameAs(source);
                convertCount++;

                return target;
            };

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<SourceClass>)null
                : m =>
                {
                    m.Should().BeSameAs(source);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            commandScope.ShouldValidate(
                source,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    context.Received().EnterScope(Arg.Is(123), Arg.Is(target));
                });

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);

            convertCount.Should().Be(shouldExecuteInfo != false ? 1 : 0);
        }
    }
}