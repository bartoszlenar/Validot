namespace Validot.Tests.Unit.Specification
{
    using System;
    using FluentAssertions;
    using Validot.Specification;
    using Validot.Specification.Commands;
    using Xunit;

    public class WithConditionExtensionTests
    {
        [Fact]
        public void Should_Add_WithConditionCommand()
        {
            Predicate<object> predicate = x => true;

            ApiTester.TestSingleCommand<object, IWithConditionIn<object>, IWithConditionOut<object>, WithConditionCommand<object>>(
                s => s.WithCondition(predicate),
                command =>
                {
                    command.ExecutionCondition.Should().NotBeNull();
                    command.ExecutionCondition.Should().BeSameAs(predicate);
                });
        }

        [Fact]
        public void Should_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithConditionOut<object>>(new[]
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
            ApiTester.TextException<object, IWithConditionIn<object>, IWithConditionOut<object>>(
                s => s.WithCondition(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}
