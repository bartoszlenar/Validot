namespace Validot.Tests.Unit.Specification
{
    using System;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class AsModelExtensionTests
    {
        [Fact]
        public void Should_Add_AsModelCommand()
        {
            Specification<object> modelSpecification = s => s;

            ApiTester.TestSingleCommand<object, IRuleIn<object>, IRuleOut<object>, AsModelCommand<object>>(
                s => s.AsModel(modelSpecification),
                command =>
                {
                    command.Specification.Should().NotBeNull();
                    command.Specification.Should().BeSameAs(modelSpecification);
                });
        }

        [Fact]
        public void Should_ThrowException_When_NullModelSpecification()
        {
            ApiTester.TextException<object, IRuleIn<object>, IRuleOut<object>>(
                s => s.AsModel(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}
