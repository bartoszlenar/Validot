namespace Validot.Tests.Unit.Specification.Commands
{
    using System;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Specification.Commands;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class AsConvertedCommandTests
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

            var command = new AsConvertedCommand<SourceClass, TargetClass>(m => new TargetClass(), specification);

            var blockBuilder = command.GetScopeBuilder();

            blockBuilder.Should().NotBeNull();
        }

        [Fact]
        public void Should_GetOrRegisterSpecification_And_SaveConverter()
        {
            Specification<TargetClass> specification = s => s;

            Converter<SourceClass, TargetClass> converter = _ => new TargetClass();

            var command = new AsConvertedCommand<SourceClass, TargetClass>(converter, specification);

            var scopeBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            buildingContext.GetOrRegisterSpecificationScope(Arg.Is<Specification<TargetClass>>(arg => ReferenceEquals(arg, specification))).Returns(666);

            var scope = scopeBuilder.Build(buildingContext);

            scope.Should().BeOfType<ConvertedCommandScope<SourceClass, TargetClass>>();

            var convertedCommandscope = scope as ConvertedCommandScope<SourceClass, TargetClass>;

            convertedCommandscope.ScopeId.Should().Be(666);

            convertedCommandscope.Converter.Should().BeSameAs(converter);

            buildingContext.Received(1).GetOrRegisterSpecificationScope(Arg.Is<Specification<TargetClass>>(arg => ReferenceEquals(arg, specification)));
        }
    }
}