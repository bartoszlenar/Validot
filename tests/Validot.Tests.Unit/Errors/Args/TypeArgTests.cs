namespace Validot.Tests.Unit.Errors.Args
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Errors.Args;

    using Xunit;

    public class TypeArgTests
    {
        [Fact]
        public void Should_Initialize()
        {
            var arg = Arg.Type("name", typeof(int));

            arg.Name.Should().Be("name");
            arg.AllowedParameters.Count.Should().Be(2);
            arg.AllowedParameters.Should().Contain("translation");
            arg.AllowedParameters.Should().Contain("format");

            arg.Should().BeOfType<TypeArg>();

            ((TypeArg)arg).Value.Should().Be(typeof(int));
        }

        [Theory]
        [InlineData(typeof(int), "name", "Int32")]
        [InlineData(typeof(int), "fullName", "System.Int32")]
        [InlineData(typeof(int), "toString", "System.Int32")]
        [InlineData(typeof(Nullable<int>), "name", "Nullable<Int32>")]
        [InlineData(typeof(Nullable<int>), "fullName", "System.Nullable<System.Int32>")]
        [InlineData(typeof(Nullable<int>), "toString", "System.Nullable`1[System.Int32]")]
        public void Should_Stringify_Format(Type value, string format, string expectedString)
        {
            var arg = Arg.Type("name", value);

            var stringified = arg.ToString(new Dictionary<string, string>
            {
                ["format"] = format,
            });

            stringified.Should().Be(expectedString);
        }

        [Fact]
        public void Should_Stringify_Translation()
        {
            IArg arg1 = Arg.Type("name", typeof(StringComparison));

            var stringified1 = arg1.ToString(new Dictionary<string, string>
            {
                ["translation"] = "true",
            });

            IArg arg2 = Arg.Type("name", typeof(Nullable<int>));

            var stringified2 = arg2.ToString(new Dictionary<string, string>
            {
                ["translation"] = "true",
            });

            stringified1.Should().Be("{_translation|key=Type.System.StringComparison}");
            stringified2.Should().Be("{_translation|key=Type.System.Nullable<System.Int32>}");
        }

        [Fact]
        public void Should_NotStringify_Translation_When_ParameterValueIsNotTrue()
        {
            IArg arg1 = Arg.Type("name", typeof(StringComparison));

            var stringified1 = arg1.ToString(new Dictionary<string, string>
            {
                ["translation"] = "false",
            });

            IArg arg2 = Arg.Type("name", typeof(Nullable<int>));

            var stringified2 = arg2.ToString(new Dictionary<string, string>
            {
                ["translation"] = "TRUE",
            });

            stringified1.Should().Be("StringComparison");
            stringified2.Should().Be("Nullable<Int32>");
        }

        [Fact]
        public void Should_StringifyUsingTranslation_When_BothFormatAndTranslationPresent()
        {
            IArg arg1 = Arg.Type("name", typeof(Nullable<int>));

            var stringified1 = arg1.ToString(new Dictionary<string, string>
            {
                ["format"] = "toString",
                ["translation"] = "true",
            });

            stringified1.Should().Be("{_translation|key=Type.System.Nullable<System.Int32>}");
        }

        [Fact]
        public void Should_StringifyDefaultValues()
        {
            IArg arg = Arg.Type("name", typeof(Nullable<int>));

            arg.Name.Should().Be("name");
            arg.ToString(null).Should().Be("Nullable<Int32>");
        }
    }
}
