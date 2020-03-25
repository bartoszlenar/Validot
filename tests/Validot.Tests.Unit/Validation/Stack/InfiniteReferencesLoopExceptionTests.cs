namespace Validot.Tests.Unit.Validation.Stack
{
    using System;

    using FluentAssertions;

    using Validot.Validation.Stacks;

    using Xunit;

    public class InfiniteReferencesLoopExceptionTests
    {
        [Fact]
        public void Should_Initialize()
        {
            var exception = new InfiniteReferencesLoopException("zxc", "zxc.nested", 123, typeof(DateTimeOffset?));

            exception.Path.Should().Be("zxc");
            exception.InfiniteLoopNestedPath.Should().Be("zxc.nested");
            exception.ScopeId.Should().Be(123);
            exception.Type.Should().Be(typeof(DateTimeOffset?));
            exception.Message.Should().Be("Infinite references loop detected: object of type Nullable<DateTimeOffset> is both under the path zxc and in the nested path zxc.nested");
        }

        [Fact]
        public void Should_InitializeMessage()
        {
            var exception = new InfiniteReferencesLoopException(null, "zxc.nested", 123, typeof(DateTimeOffset?));

            exception.Path.Should().BeNull();
            exception.InfiniteLoopNestedPath.Should().Be("zxc.nested");
            exception.ScopeId.Should().Be(123);
            exception.Type.Should().Be(typeof(DateTimeOffset?));
            exception.Message.Should().Be("Infinite references loop detected: object of type Nullable<DateTimeOffset> is both under the root path and in the nested path zxc.nested");
        }
    }
}
