namespace Validot.Tests.Unit.Specification.Commands
{
    using System;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Specification.Commands;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class RuleCommandTests
    {
        [Fact]
        public void Should_Get_RuleBlockBuilder()
        {
            Predicate<object> validCondition = x => true;

            var command = new RuleCommand<object>(validCondition);

            var blockBuilder = command.GetScopeBuilder();

            blockBuilder.Should().NotBeNull();
            blockBuilder.Should().BeAssignableTo<RuleCommandScopeBuilder<object>>();
        }

        [Fact]
        public void Should_GetRuleBlock_With_Predicate()
        {
            Predicate<object> validCondition = x => true;

            var command = new RuleCommand<object>(validCondition);

            var blockBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            var block = blockBuilder.Build(buildingContext);

            block.Should().BeOfType<RuleCommandScope<object>>();

            var ruleBlock = (RuleCommandScope<object>)block;

            ruleBlock.IsValid.Should().BeSameAs(validCondition);
        }
    }
}
