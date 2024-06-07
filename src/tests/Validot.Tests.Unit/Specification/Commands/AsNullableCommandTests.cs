namespace Validot.Tests.Unit.Specification.Commands
{
    using FluentAssertions;

    using NSubstitute;

    using Validot.Specification.Commands;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class AsNullableCommandTests
    {
        [Fact]
        public void Should_Get_BlockBuilder()
        {
            Specification<int> specification = s => s;

            var command = new AsNullableCommand<int>(specification);

            var blockBuilder = command.GetScopeBuilder();

            blockBuilder.Should().NotBeNull();
        }

        [Fact]
        public void Should_GetOrRegisterSpecification_And_AddModelBlock()
        {
            Specification<int> specification = s => s;

            var command = new AsNullableCommand<int>(specification);

            var blockBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            buildingContext.GetOrRegisterSpecificationScope(Arg.Is<Specification<int>>(arg => ReferenceEquals(arg, specification))).Returns(666);

            var block = blockBuilder.Build(buildingContext);

            block.Should().BeOfType<NullableCommandScope<int>>();

            var modelBlock = block as NullableCommandScope<int>;

            modelBlock.ScopeId.Should().Be(666);

            buildingContext.Received(1).GetOrRegisterSpecificationScope(Arg.Is<Specification<int>>(arg => ReferenceEquals(arg, specification)));
        }
    }
}
