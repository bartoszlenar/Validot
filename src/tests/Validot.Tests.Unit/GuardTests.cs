namespace Validot.Tests.Unit
{
    using System;

    using FluentAssertions;

    using Xunit;

    public class GuardTests
    {
        public class NullArgument
        {
            [Fact]
            public void Should_Throw_When_ArgumentIsNull()
            {
                Action action = () =>
                {
                    ThrowHelper.NullArgument<object>(null, "some name");
                };

                action.Should()
                    .ThrowExactly<ArgumentNullException>()
                    .WithMessage("*some name*");
            }
        }
    }
}
