namespace Validot.Tests.Unit.Specification.Commands
{
    using System;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Specification.Commands;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class AsTypeCommandTests
    {
        private class SourceClass
        {
        }

        private class TargetClass
        {
        }

        [Fact]
        public void Should_Get_BlockBuilder()
        {
            Specification<TargetClass> specification = s => s;

            var command = new AsTypeCommand<SourceClass, TargetClass>(specification);

            var blockBuilder = command.GetScopeBuilder();

            blockBuilder.Should().NotBeNull();
        }

        [Fact]
        public void Should_GetOrRegisterSpecification()
        {
            Specification<TargetClass> specification = s => s;

            var command = new AsTypeCommand<SourceClass, TargetClass>(specification);

            var scopeBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            buildingContext.GetOrRegisterSpecificationScope(Arg.Is<Specification<TargetClass>>(arg => ReferenceEquals(arg, specification))).Returns(666);

            var scope = scopeBuilder.Build(buildingContext);

            scope.Should().BeOfType<TypeCommandScope<SourceClass, TargetClass>>();

            var typeCommandScope = scope as TypeCommandScope<SourceClass, TargetClass>;

            typeCommandScope.ScopeId.Should().Be(666);

            buildingContext.Received(1).GetOrRegisterSpecificationScope(Arg.Is<Specification<TargetClass>>(arg => ReferenceEquals(arg, specification)));
        }
    }
}
