namespace Validot.Tests.Unit.Validation.Stack
{
    using System;

    using FluentAssertions;

    using Validot.Validation.Stacks;

    using Xunit;

    public class ReferenceLoopExceptionTests
    {
        [Fact]
        public void Should_Initialize_WithPaths()
        {
            var exception = new ReferenceLoopException("zxc", "zxc.nested", 123, typeof(DateTimeOffset?));

            exception.Path.Should().Be("zxc");
            exception.NestedPath.Should().Be("zxc.nested");
            exception.ScopeId.Should().Be(123);
            exception.Type.Should().Be(typeof(DateTimeOffset?));
            exception.Message.Should().Be($"Reference loop detected: object of type Nullable<DateTimeOffset> has been detected twice in the reference graph, effectively creating an infinite references loop (at first under the path 'zxc' and then under the nested path 'zxc.nested')");
        }

        [Fact]
        public void Should_Initialize_WithPaths_And_WithRootPath()
        {
            var exception = new ReferenceLoopException(null, "zxc.nested", 123, typeof(DateTimeOffset?));

            exception.Path.Should().BeNull();
            exception.NestedPath.Should().Be("zxc.nested");
            exception.ScopeId.Should().Be(123);
            exception.Type.Should().Be(typeof(DateTimeOffset?));
            exception.Message.Should().Be($"Reference loop detected: object of type Nullable<DateTimeOffset> has been detected twice in the reference graph, effectively creating an infinite references loop (at first under the root path, so the validated object itself, and then under the nested path 'zxc.nested')");
        }

        [Fact]
        public void Should_Initialize_WithoutPaths()
        {
            var exception = new ReferenceLoopException(123, typeof(DateTimeOffset?));

            exception.Path.Should().BeNull();
            exception.NestedPath.Should().BeNull();
            exception.ScopeId.Should().Be(123);
            exception.Type.Should().Be(typeof(DateTimeOffset?));
            exception.Message.Should().Be($"Reference loop detected: object of type {typeof(DateTimeOffset?).GetFriendlyName()} has been detected twice in the reference graph, effectively creating the infinite references loop (where exactly, that information is not available - is that validation comes from IsValid method, please repeat it using the Validate method and examine the exception thrown)");
        }
    }
}
