namespace Validot.Tests.Unit.Errors
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using Validot.Errors;
    using Validot.Translations;

    using Xunit;

    public class ReferenceLoopErrorTests
    {
        [Fact]
        public void Should_Initialize()
        {
            var error = new ReferenceLoopError(typeof(DateTimeOffset?));

            error.Messages.Should().NotBeNull();
            error.Messages.Count.Should().Be(1);
            error.Messages.Single().Should().Be(MessageKey.Global.ReferenceLoop);
            error.Codes.Should().NotBeNull();
            error.Codes.Should().BeEmpty();
        }
    }
}
