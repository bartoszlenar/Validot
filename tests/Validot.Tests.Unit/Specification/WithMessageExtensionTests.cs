namespace Validot.Tests.Unit.Specification
{
    using System;
    using FluentAssertions;
    using Validot.Specification;
    using Validot.Specification.Commands;
    using Xunit;

    public class WithMessageExtensionTests
    {
        [Fact]
        public void Should_ForbiddenWithMessage_Add_WithMessageCommand()
        {
            ApiTester.TestSingleCommand<object, IWithMessageForbiddenIn<object>, IWithMessageForbiddenOut<object>, WithMessageCommand>(
                s => s.WithMessage("message"),
                command =>
                {
                    command.Message.Should().NotBeNull();
                    command.Message.Should().Be("message");
                });
        }

        [Fact]
        public void Should_ForbiddenWithMessage_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithMessageForbiddenOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IWithExtraMessageForbiddenIn<object>),
                typeof(IWithExtraCodeForbiddenIn<object>)
            });
        }

        [Fact]
        public void Should_ForbiddenWithMessage_ThrowException_When_NullMessage()
        {
            ApiTester.TextException<object, IWithMessageForbiddenIn<object>, IWithMessageForbiddenOut<object>>(
                s => s.WithMessage(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Fact]
        public void Should_WithMessage_Add_WithMessageCommand()
        {
            ApiTester.TestSingleCommand<object, IWithMessageIn<object>, IWithMessageOut<object>, WithMessageCommand>(
                s => s.WithMessage("message"),
                command =>
                {
                    command.Message.Should().NotBeNull();
                    command.Message.Should().Be("message");
                });
        }

        [Fact]
        public void Should_WithMessage_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithMessageOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IRuleIn<object>),
                typeof(IWithExtraMessageIn<object>),
                typeof(IWithExtraCodeIn<object>)
            });
        }

        [Fact]
        public void Should_WithMessage_ThrowException_When_NullMessage()
        {
            ApiTester.TextException<object, IWithMessageIn<object>, IWithMessageOut<object>>(
                s => s.WithMessage(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}
