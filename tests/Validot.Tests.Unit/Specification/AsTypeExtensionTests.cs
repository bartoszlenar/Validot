namespace Validot.Tests.Unit.Specification
{
    using System;
    using System.Linq.Expressions;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class AsTypeExtensionTests
    {
        private class SourceClass
        {
        }

        private class TargetClass
        {
        }

        [Fact]
        public void Should_Add_AsTypeCommand()
        {
            Specification<TargetClass> targetSpecifiction = s => s;

            ApiTester.TestSingleCommand<SourceClass, IRuleIn<SourceClass>, IRuleOut<SourceClass>, AsTypeCommand<SourceClass, TargetClass>>(
                s => s.AsType(targetSpecifiction),
                command =>
                {
                    command.Specification.Should().NotBeNull();
                    command.Specification.Should().BeSameAs(targetSpecifiction);
                });
        }

        [Fact]
        public void Should_ThrowException_When_NullSpecification()
        {
            ApiTester.TextException<SourceClass, IRuleIn<SourceClass>, IRuleOut<SourceClass>>(
                s => s.AsType(null as Specification<TargetClass>),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}
