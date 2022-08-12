namespace Validot.Tests.Unit.Specification
{
    using System;
    using System.Linq.Expressions;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class AsConvertedExtensionTests
    {
        private class SourceClass
        {
        }

        private class TargetClass
        {
        }

        [Fact]
        public void Should_Add_ConvertedCommand()
        {
            Converter<SourceClass, TargetClass> converter = s => new TargetClass();

            Specification<TargetClass> targetSpecifiction = s => s;

            ApiTester.TestSingleCommand<SourceClass, IRuleIn<SourceClass>, IRuleOut<SourceClass>, AsConvertedCommand<SourceClass, TargetClass>>(
                s => s.AsConverted(converter, targetSpecifiction),
                command =>
                {
                    command.Specification.Should().NotBeNull();
                    command.Specification.Should().BeSameAs(targetSpecifiction);

                    command.Converter.Should().NotBeNull();
                    command.Converter.Should().BeSameAs(converter);
                });
        }

        [Fact]
        public void Should_ThrowException_When_NullConvert()
        {
            Specification<TargetClass> targetSpecifiction = s => s;

            ApiTester.TextException<SourceClass, IRuleIn<SourceClass>, IRuleOut<SourceClass>>(
                s => s.AsConverted(null, targetSpecifiction),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Fact]
        public void Should_ThrowException_When_NullTargetSpecification()
        {
            Converter<SourceClass, TargetClass> converter = s => new TargetClass();

            ApiTester.TextException<SourceClass, IRuleIn<SourceClass>, IRuleOut<SourceClass>>(
                s => s.AsConverted(converter, null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}