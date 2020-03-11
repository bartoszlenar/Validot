namespace Validot.Tests.Unit.Specification.Commands
{
    using FluentAssertions;

    using NSubstitute;

    using Validot.Specification.Commands;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class AsModelCommandTests
    {
        [Fact]
        public void Should_Get_BlockBuilder()
        {
            Specification<object> specification = s => s;

            var command = new AsModelCommand<object>(specification);

            var blockBuilder = command.GetScopeBuilder();

            blockBuilder.Should().NotBeNull();
        }

        [Fact]
        public void Should_GetOrRegisterSpecification_And_AddModelBlock()
        {
            Specification<object> specification = s => s;

            var command = new AsModelCommand<object>(specification);

            var blockBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            buildingContext.GetOrRegisterSpecificationScope(Arg.Is<Specification<object>>(arg => ReferenceEquals(arg, specification))).Returns(666);

            var block = blockBuilder.Build(buildingContext);

            block.Should().BeOfType<ModelCommandScope<object>>();

            var modelBlock = (ModelCommandScope<object>)block;

            modelBlock.ScopeId.Should().Be(666);

            buildingContext.Received(1).GetOrRegisterSpecificationScope(Arg.Is<Specification<object>>(arg => ReferenceEquals(arg, specification)));
        }
    }
}
