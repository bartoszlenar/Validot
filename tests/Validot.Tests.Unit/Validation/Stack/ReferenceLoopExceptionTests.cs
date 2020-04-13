namespace Validot.Tests.Unit.Validation.Stack
{
    using System;

    using FluentAssertions;

    using Validot.Validation.Stacks;

    using Xunit;

    public class ReferenceLoopExceptionTests
    {
        [Fact]
        public void Should_Initialize()
        {
            var exception = new ReferenceLoopException("zxc", "zxc.nested", 123, typeof(DateTimeOffset?));

            exception.Path.Should().Be("zxc");
            exception.NestedPath.Should().Be("zxc.nested");
            exception.ScopeId.Should().Be(123);
            exception.Type.Should().Be(typeof(DateTimeOffset?));
            exception.Message.Should().Be("Reference loop detected: object of type Nullable<DateTimeOffset> is both under the path zxc and in the nested path zxc.nested");
        }

        [Fact]
        public void Should_InitializeMessage()
        {
            var exception = new ReferenceLoopException(null, "zxc.nested", 123, typeof(DateTimeOffset?));

            exception.Path.Should().BeNull();
            exception.NestedPath.Should().Be("zxc.nested");
            exception.ScopeId.Should().Be(123);
            exception.Type.Should().Be(typeof(DateTimeOffset?));
            exception.Message.Should().Be("Reference loop detected: object of type Nullable<DateTimeOffset> is both under the root path and in the nested path zxc.nested");
        }
    }
}
