namespace Validot.Tests.Unit.Errors.Args
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Errors.Args;

    using Xunit;

    public class GuidArgTests
    {
        [Theory]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", null, "c2ce1f3b-17e5-412e-923b-6b4e268f31aa")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "upper", "C2CE1F3B-17E5-412E-923B-6B4E268F31AA")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "lower", "c2ce1f3b-17e5-412e-923b-6b4e268f31aa")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "something", "c2ce1f3b-17e5-412e-923b-6b4e268f31aa")]
        public void Should_Stringify_Case(string value, string caseParameter, string expectedString)
        {
            IArg arg = Arg.GuidValue("name", new Guid(value));

            var stringified = arg.ToString(caseParameter != null
                ? new Dictionary<string, string>
                {
                    {
                        "case", caseParameter
                    }
                }
                : null);

            stringified.Should().Be(expectedString);
        }

        [Theory]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", null, "c2ce1f3b-17e5-412e-923b-6b4e268f31aa")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "D", "c2ce1f3b-17e5-412e-923b-6b4e268f31aa")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "B", "{c2ce1f3b-17e5-412e-923b-6b4e268f31aa}")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "P", "(c2ce1f3b-17e5-412e-923b-6b4e268f31aa)")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "X", "{0xc2ce1f3b,0x17e5,0x412e,{0x92,0x3b,0x6b,0x4e,0x26,0x8f,0x31,0xaa}}")]
        public void Should_Stringify_Format(string value, string format, string expectedString)
        {
            IArg arg = Arg.GuidValue("name", new Guid(value));

            var stringified = arg.ToString(new Dictionary<string, string>
            {
                {
                    "format", format
                }
            });

            stringified.Should().Be(expectedString);
        }

        [Theory]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", null, null, "c2ce1f3b-17e5-412e-923b-6b4e268f31aa")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", null, "upper", "C2CE1F3B-17E5-412E-923B-6B4E268F31AA")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", null, "lower", "c2ce1f3b-17e5-412e-923b-6b4e268f31aa")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "B", null, "{c2ce1f3b-17e5-412e-923b-6b4e268f31aa}")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "B", "upper", "{C2CE1F3B-17E5-412E-923B-6B4E268F31AA}")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "B", "lower", "{c2ce1f3b-17e5-412e-923b-6b4e268f31aa}")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "X", null, "{0xc2ce1f3b,0x17e5,0x412e,{0x92,0x3b,0x6b,0x4e,0x26,0x8f,0x31,0xaa}}")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "X", "upper", "{0XC2CE1F3B,0X17E5,0X412E,{0X92,0X3B,0X6B,0X4E,0X26,0X8F,0X31,0XAA}}")]
        [InlineData("c2ce1f3b-17e5-412e-923b-6b4e268f31aa", "X", "lower", "{0xc2ce1f3b,0x17e5,0x412e,{0x92,0x3b,0x6b,0x4e,0x26,0x8f,0x31,0xaa}}")]
        public void Should_Stringify(string value, string format, string casing, string expectedString)
        {
            IArg arg = Arg.GuidValue("name", new Guid(value));

            var stringified = arg.ToString(new Dictionary<string, string>
            {
                {
                    "format", format
                },
                {
                    "case", casing
                }
            });

            stringified.Should().Be(expectedString);
        }

        [Fact]
        public void Should_Initialize()
        {
            IArg arg = Arg.GuidValue("name", new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"));

            arg.Name.Should().Be("name");
            arg.AllowedParameters.Count.Should().Be(2);
            arg.AllowedParameters.Should().Contain("case");

            arg.Name.Should().Be("name");
            arg.AllowedParameters.Count.Should().Be(2);

            arg.AllowedParameters.Should().Contain("case");
            arg.AllowedParameters.Should().Contain("format");

            arg.Should().BeOfType<GuidArg>();
            ((GuidArg)arg).Value.Should().Be(new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"));
        }

        [Fact]
        public void Should_Stringify_IntoSameValue_When_InvalidParameter()
        {
            IArg arg = Arg.GuidValue("name", new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"));

            var stringified = arg.ToString(new Dictionary<string, string>
            {
                {
                    "invalidParameter", "test"
                }
            });

            stringified.Should().Be("c2ce1f3b-17e5-412e-923b-6b4e268f31aa");
        }

        [Fact]
        public void Should_ThrowException_When_NullName()
        {
            Action action = () => Arg.GuidValue(null, new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"));

            action.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
