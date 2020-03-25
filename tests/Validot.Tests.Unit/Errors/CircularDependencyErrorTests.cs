namespace Validot.Tests.Unit.Errors
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using Validot.Errors;
    using Validot.Translations;

    using Xunit;

    public class CircularDependencyErrorTests
    {
        [Fact]
        public void Should_Initialize()
        {
            var error = new CircularDependencyError(typeof(DateTimeOffset?));

            error.Messages.Should().NotBeNull();
            error.Messages.Count.Should().Be(1);
            error.Messages.Single().Should().Be(MessageKey.Global.CircularDependency);
            error.Codes.Should().NotBeNull();
            error.Codes.Should().BeEmpty();
        }
    }
}
