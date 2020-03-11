namespace Validot.Tests.Unit.Specification
{
    using System;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class AsNullableExtensionTests
    {
        [Fact]
        public void Should_Add_AsNullableCommand()
        {
            Specification<int> modelSpecification = s => s;

            ApiTester.TestSingleCommand<int?, IRuleIn<int?>, IRuleOut<int?>, AsNullableCommand<int>>(
                s => s.AsNullable(modelSpecification),
                command =>
                {
                    command.Specification.Should().NotBeNull();
                    command.Specification.Should().BeSameAs(modelSpecification);
                });
        }

        [Fact]
        public void Should_ThrowException_When_NullModelSpecification()
        {
            ApiTester.TextException<int?, IRuleIn<int?>, IRuleOut<int?>>(
                s => s.AsNullable(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}
