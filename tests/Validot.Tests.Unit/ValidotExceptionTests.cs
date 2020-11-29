namespace Validot.Tests.Unit
{
    using System;

    using FluentAssertions;

    using Xunit;

    public class ValidotExceptionTests
    {
        [Fact]
        public void Should_Initialize_With_Message()
        {
            var exception = new ValidotException("a message");

            exception.Message.Should().Be("a message");
        }

        [Fact]
        public void Should_Initialize_With_Message_And_InnerException()
        {
            var innerException = new InvalidOperationException();

            var exception = new ValidotException("a message", innerException);

            exception.Message.Should().Be("a message");
            exception.InnerException.Should().BeSameAs(innerException);
        }
    }
}
