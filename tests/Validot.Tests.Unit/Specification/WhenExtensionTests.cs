namespace Validot.Tests.Unit.Specification
{
    using System;
    using FluentAssertions;
    using Validot.Specification;
    using Validot.Specification.Commands;
    using Xunit;

    public class WhenExtensionTests
    {
        [Fact]
        public void Should_Add_WhenCommand()
        {
            Predicate<object> predicate = x => true;

            ApiTester.TestSingleCommand<object, IWhenIn<object>, IWhenOut<object>, WhenCommand<object>>(
                s => s.When(predicate),
                command =>
                {
                    command.ExecutionCondition.Should().NotBeNull();
                    command.ExecutionCondition.Should().BeSameAs(predicate);
                });
        }

        [Fact]
        public void Should_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWhenOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IWithErrorClearedIn<object>),
                typeof(IWithMessageIn<object>),
                typeof(IWithExtraMessageIn<object>),
                typeof(IWithCodeIn<object>),
                typeof(IWithExtraCodeIn<object>)
            });
        }

        [Fact]
        public void Should_ThrowException_When_NullExecutionCondition()
        {
            ApiTester.TextException<object, IWhenIn<object>, IWhenOut<object>>(
                s => s.When(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}
