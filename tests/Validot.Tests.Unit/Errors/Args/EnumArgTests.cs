namespace Validot.Tests.Unit.Errors.Args
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using FluentAssertions;

    using Validot.Errors.Args;

    using Xunit;

    public class EnumArgTests
    {
        [Theory]
        [InlineData(StringComparison.Ordinal, "G", "Ordinal")]
        [InlineData(StringComparison.Ordinal, "D", "4")]
        [InlineData(StringComparison.Ordinal, "X", "00000004")]
        public void Should_Stringify_UsingFormat(StringComparison stringComparison, string format, string expectedString)
        {
            IArg arg = Arg.Enum("name", stringComparison);

            var stringified = arg.ToString(new Dictionary<string, string>
            {
                ["format"] = format
            });

            stringified.Should().Be(expectedString);
        }

        [Fact]
        public void Should_Initialize()
        {
            IArg arg = Arg.Enum("name", StringComparison.CurrentCulture);

            arg.Name.Should().Be("name");
            arg.AllowedParameters.Count.Should().Be(2);
            arg.AllowedParameters.Should().Contain("translation");
            arg.AllowedParameters.Should().Contain("format");

            arg.Should().BeOfType<EnumArg<StringComparison>>();

            ((EnumArg<StringComparison>)arg).Value.Should().Be(StringComparison.CurrentCulture);
        }

        [Fact]
        public void Should_NotStringify_Translation_When_ParameterValueIsNotTrue()
        {
            IArg arg1 = Arg.Enum("name", StringComparison.CurrentCulture);

            var stringified1 = arg1.ToString(new Dictionary<string, string>
            {
                ["translation"] = "false"
            });

            IArg arg2 = Arg.Enum("name", FileMode.OpenOrCreate);

            var stringified2 = arg2.ToString(new Dictionary<string, string>
            {
                ["translation"] = "somevalue"
            });

            stringified1.Should().Be("CurrentCulture");
            stringified2.Should().Be("OpenOrCreate");
        }

        [Fact]
        public void Should_Stringify_Translation()
        {
            IArg arg1 = Arg.Enum("name", StringComparison.CurrentCulture);

            var stringified1 = arg1.ToString(new Dictionary<string, string>
            {
                ["translation"] = "true"
            });

            IArg arg2 = Arg.Enum("name", FileMode.OpenOrCreate);

            var stringified2 = arg2.ToString(new Dictionary<string, string>
            {
                ["translation"] = "true"
            });

            stringified1.Should().Be("{_translation|key=Enum.System.StringComparison.CurrentCulture}");
            stringified2.Should().Be("{_translation|key=Enum.System.IO.FileMode.OpenOrCreate}");
        }

        [Fact]
        public void Should_StringifyDefaultValues()
        {
            IArg arg = Arg.Enum("name", StringComparison.CurrentCulture);

            arg.Name.Should().Be("name");
            arg.ToString(null).Should().Be("CurrentCulture");
        }

        [Fact]
        public void Should_StringifyUsingTranslation_When_BothFormatAndTranslationPresent()
        {
            IArg arg1 = Arg.Enum("name", StringComparison.CurrentCulture);

            var stringified1 = arg1.ToString(new Dictionary<string, string>
            {
                ["format"] = "D",
                ["translation"] = "true",
            });

            stringified1.Should().Be("{_translation|key=Enum.System.StringComparison.CurrentCulture}");
        }

        [Fact]
        public void Should_ThrowException_When_NullName()
        {
            Action action = () => { Arg.Enum(null, StringComparison.CurrentCulture); };

            action.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
