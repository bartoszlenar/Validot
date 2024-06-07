namespace Validot.Tests.Unit.Errors.Args
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Errors.Args;

    using Xunit;

    public class TextArgTests
    {
        [Theory]
        [InlineData("TeSt", null, "TeSt")]
        [InlineData("TeSt", "upper", "TEST")]
        [InlineData("TeSt", "lower", "test")]
        [InlineData("TeSt", "something", "TeSt")]
        public void Should_Stringify_String(string value, string caseParameter, string expectedString)
        {
            var arg = Arg.Text("name", value);

            var stringified = arg.ToString(caseParameter != null
                ? new Dictionary<string, string>
                {
                    { "case", caseParameter }
                }
                : null);

            stringified.Should().Be(expectedString);
        }

        [Theory]
        [InlineData('t', null, "t")]
        [InlineData('t', "upper", "T")]
        [InlineData('T', "upper", "T")]
        [InlineData('t', "lower", "t")]
        [InlineData('T', "lower", "t")]
        public void Should_Stringify_Char(char value, string caseParameter, string expectedString)
        {
            var arg = Arg.Text("name", value);

            var stringified = arg.ToString(caseParameter != null
                ? new Dictionary<string, string>
                {
                    { "case", caseParameter },
                }
                : null);

            stringified.Should().Be(expectedString);
        }

        [Theory]
        [InlineData('t', "t")]
        [InlineData('T', "T")]
        public void Should_Stringify_Char_IntoSameValue_When_InvalidParameter(char value, string expectedString)
        {
            var arg = Arg.Text("name", value);

            var stringified = arg.ToString(new Dictionary<string, string>
            {
                { "invalidParameter", "test" },
            });

            stringified.Should().Be(expectedString);
        }

        [Fact]
        public void Should_Initialize()
        {
            var arg = Arg.Text("name", "value");

            arg.Name.Should().Be("name");
            arg.AllowedParameters.Count.Should().Be(1);
            arg.AllowedParameters.Should().Contain("case");

            arg.Should().BeOfType<TextArg>();

            ((TextArg)arg).Value.Should().Be("value");
        }

        [Fact]
        public void Should_Stringify_String_IntoSameValue_When_InvalidParameter()
        {
            var arg = Arg.Text("name", "nAmE");

            var stringified = arg.ToString(new Dictionary<string, string>
            {
                { "invalidParameter", "test" },
            });

            stringified.Should().Be("nAmE");
        }

        [Fact]
        public void Should_ThrowException_When_NullName()
        {
            Action action = () => { Arg.Text(null, "value"); };

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_NullValue()
        {
            Action action = () => { Arg.Text("name", null); };
            action.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
