namespace Validot.Tests.Unit.Specification
{
    using System;
    using FluentAssertions;
    using Validot.Specification;
    using Validot.Specification.Commands;
    using Xunit;

    public class WithExtraMessageExtensionTests
    {
        [Fact]
        public void Should_ForbiddenWithExtraMessage_Add_WithExtraMessageCommand()
        {
            ApiTester.TestSingleCommand<object, IWithExtraMessageForbiddenIn<object>, IWithExtraMessageForbiddenOut<object>, WithExtraMessageCommand>(
                s => s.WithExtraMessage("message"),
                command =>
                {
                    command.Message.Should().NotBeNull();
                    command.Message.Should().Be("message");
                });
        }

        [Fact]
        public void Should_ForbiddenWithExtraMessage_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithExtraMessageForbiddenOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IWithExtraMessageForbiddenIn<object>),
                typeof(IWithExtraCodeForbiddenIn<object>)
            });
        }

        [Fact]
        public void Should_ForbiddenWithExtraMessage_ThrowException_When_NullMessage()
        {
            ApiTester.TextException<object, IWithExtraMessageForbiddenIn<object>, IWithExtraMessageForbiddenOut<object>>(
                s => s.WithExtraMessage(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Fact]
        public void Should_WithExtraMessage_Add_WithExtraMessageCommand()
        {
            ApiTester.TestSingleCommand<object, IWithExtraMessageIn<object>, IWithExtraMessageOut<object>, WithExtraMessageCommand>(
                s => s.WithExtraMessage("message"),
                command =>
                {
                    command.Message.Should().NotBeNull();
                    command.Message.Should().Be("message");
                });
        }

        [Fact]
        public void Should_WithExtraMessage_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithExtraMessageOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IRuleIn<object>),
                typeof(IWithExtraMessageIn<object>),
                typeof(IWithExtraCodeIn<object>)
            });
        }

        [Fact]
        public void Should_WithExtraMessage_ThrowException_When_NullMessage()
        {
            ApiTester.TextException<object, IWithExtraMessageIn<object>, IWithExtraMessageOut<object>>(
                s => s.WithExtraMessage(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}
