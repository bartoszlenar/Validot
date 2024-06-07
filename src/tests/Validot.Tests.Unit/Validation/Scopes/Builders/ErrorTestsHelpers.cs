namespace Validot.Tests.Unit.Validation.Scopes.Builders
{
    using System.Linq;

    using FluentAssertions;

    using Validot.Errors;

    public static class ErrorTestsHelpers
    {
        public static void ShouldBeEqualTo(this IError @this, IError error)
        {
            @this.Codes.Should().NotBeNull();
            @this.Messages.Should().NotBeNull();

            if (error.Messages.Any())
            {
                @this.Messages.Should().NotBeEmpty();

                @this.Messages.Count.Should().Be(error.Messages.Count);

                for (var i = 0; i < error.Messages.Count; ++i)
                {
                    @this.Messages[i].Should().Be(error.Messages[i]);
                }
            }
            else
            {
                @this.Messages.Should().BeEmpty();
            }

            if (error.Codes.Any())
            {
                @this.Codes.Should().NotBeEmpty();

                @this.Codes.Count.Should().Be(error.Codes.Count);

                for (var i = 0; i < error.Codes.Count; ++i)
                {
                    @this.Codes[i].Should().Be(error.Codes[i]);
                }
            }
            else
            {
                @this.Codes.Should().BeEmpty();
            }

            @this.Args.Should().NotBeNull();

            if (error.Args.Any())
            {
                @this.Args.Count.Should().Be(error.Args.Count);

                for (var i = 0; i < error.Args.Count; ++i)
                {
                    @this.Args[i].Should().BeOfType(error.Args[i].GetType());
                    @this.Args[i].Name.Should().Be(error.Args[i].Name);

                    var thisStringified = @this.Args[i].ToString(null);
                    var errorStringified = error.Args[i].ToString(null);

                    thisStringified.Should().Be(errorStringified);
                }
            }
            else
            {
                @this.Args.Should().BeEmpty();
            }
        }
    }
}
