namespace Validot.Tests.Unit.Errors.Args
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Errors.Args;

    using Xunit;

    public class NumberArgTests
    {
        public static IEnumerable<object[]> Should_Stringify_Numbers_WithFormatAndCulture_Data()
        {
            yield return new object[]
            {
                Arg.Number("name", 123),
                "0.00",
                "pl-PL",
                "123,00",
            };

            yield return new object[]
            {
                Arg.Number("name", (uint)123),
                "0.00",
                "pl-PL",
                "123,00",
            };

            yield return new object[]
            {
                Arg.Number("name", (short)123),
                "0.00",
                "pl-PL",
                "123,00",
            };

            yield return new object[]
            {
                Arg.Number("name", (ushort)123),
                "0.00",
                "pl-PL",
                "123,00",
            };

            yield return new object[]
            {
                Arg.Number("name", (long)123),
                "0.00",
                "pl-PL",
                "123,00",
            };

            yield return new object[]
            {
                Arg.Number("name", (ulong)123),
                "0.00",
                "pl-PL",
                "123,00",
            };

            yield return new object[]
            {
                Arg.Number("name", (byte)123),
                "0.00",
                "pl-PL",
                "123,00",
            };

            yield return new object[]
            {
                Arg.Number("name", (sbyte)123),
                "0.00",
                "pl-PL",
                "123,00",
            };

            yield return new object[]
            {
                Arg.Number("name", (decimal)123.987),
                "0.00",
                "pl-PL",
                "123,99",
            };

            yield return new object[]
            {
                Arg.Number("name", 123.987),
                "0.00",
                "pl-PL",
                "123,99",
            };

            yield return new object[]
            {
                Arg.Number("name", (float)123.987),
                "0.00",
                "pl-PL",
                "123,99",
            };
        }

        [Theory]
        [MemberData(nameof(Should_Stringify_Numbers_WithFormatAndCulture_Data))]
        public void Should_Stringify_Numbers_WithFormatAndCulture(NumberArg arg, string format, string culture, string expectedString)
        {
            var stringified = arg.ToString(new Dictionary<string, string>
            {
                ["format"] = format,
                ["culture"] = culture,
            });

            stringified.Should().Be(expectedString);
        }

        public static IEnumerable<object[]> Should_Stringify_Dates_WithCulture_Data()
        {
            yield return new object[]
            {
                Arg.Number("name", 123),
                "pl-PL",
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (uint)123),
                "pl-PL",
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (short)123),
                "pl-PL",
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (ushort)123),
                "pl-PL",
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (long)123),
                "pl-PL",
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (ulong)123),
                "pl-PL",
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (byte)123),
                "pl-PL",
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (sbyte)123),
                "pl-PL",
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (decimal)123.987),
                "pl-PL",
                "123,987",
            };

            yield return new object[]
            {
                Arg.Number("name", 123.987),
                "pl-PL",
                "123,987",
            };

            yield return new object[]
            {
                Arg.Number("name", (float)123.987),
                "pl-PL",
                "123,987",
            };
        }

        [Theory]
        [MemberData(nameof(Should_Stringify_Dates_WithCulture_Data))]
        public void Should_Stringify_Dates_WithCulture(NumberArg arg, string culture, string expectedString)
        {
            var stringified = arg.ToString(new Dictionary<string, string>
            {
                ["culture"] = culture,
            });

            stringified.Should().Be(expectedString);
        }

        public static IEnumerable<object[]> Should_Stringify_WithFormat_Data()
        {
            yield return new object[]
            {
                Arg.Number("name", 123),
                "0.00",
                "123.00",
            };

            yield return new object[]
            {
                Arg.Number("name", (uint)123),
                "0.00",
                "123.00",
            };

            yield return new object[]
            {
                Arg.Number("name", (short)123),
                "0.00",
                "123.00",
            };

            yield return new object[]
            {
                Arg.Number("name", (ushort)123),
                "0.00",
                "123.00",
            };

            yield return new object[]
            {
                Arg.Number("name", (long)123),
                "0.00",
                "123.00",
            };

            yield return new object[]
            {
                Arg.Number("name", (ulong)123),
                "0.00",
                "123.00",
            };

            yield return new object[]
            {
                Arg.Number("name", (byte)123),
                "0.00",
                "123.00",
            };

            yield return new object[]
            {
                Arg.Number("name", (sbyte)123),
                "0.00",
                "123.00",
            };

            yield return new object[]
            {
                Arg.Number("name", (decimal)123.987),
                "0.00",
                "123.99",
            };

            yield return new object[]
            {
                Arg.Number("name", 123.987),
                "0.00",
                "123.99",
            };

            yield return new object[]
            {
                Arg.Number("name", (float)123.987),
                "0.00",
                "123.99",
            };
        }

        [Theory]
        [MemberData(nameof(Should_Stringify_WithFormat_Data))]
        public void Should_Stringify_WithFormat(NumberArg arg, string format, string expectedString)
        {
            var stringified = arg.ToString(new Dictionary<string, string>
            {
                ["format"] = format,
            });

            stringified.Should().Be(expectedString);
        }

        public static IEnumerable<object[]> Should_Stringify_Default_Data()
        {
            yield return new object[]
            {
                Arg.Number("name", 123),
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (uint)123),
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (short)123),
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (ushort)123),
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (long)123),
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (ulong)123),
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (byte)123),
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (sbyte)123),
                "123",
            };

            yield return new object[]
            {
                Arg.Number("name", (decimal)123.987),
                "123.987",
            };

            yield return new object[]
            {
                Arg.Number("name", 123.987),
                "123.987",
            };

            yield return new object[]
            {
                Arg.Number("name", (float)123.987),
                "123.987",
            };
        }

        [Theory]
        [MemberData(nameof(Should_Stringify_Default_Data))]
        public void Should_Stringify_Default(NumberArg arg, string expectedString)
        {
            var stringified = arg.ToString(null);

            stringified.Should().Be(expectedString);
        }

        [Fact]
        public void Should_Initialize()
        {
            IArg arg = Arg.Number("name", 1);

            arg.Name.Should().Be("name");

            arg.AllowedParameters.Count.Should().Be(2);

            arg.AllowedParameters.Should().Contain("format");
            arg.AllowedParameters.Should().Contain("culture");

            arg.Should().BeOfType<NumberArg<int>>();

            ((NumberArg<int>)arg).Value.Should().Be(1);
        }

        [Fact]
        public void Should_Initialize_Values()
        {
            var argInt = Arg.Number("name", 1);
            argInt.Should().BeOfType<NumberArg<int>>();
            ((NumberArg<int>)argInt).Value.Should().Be(1);

            var argUInt = Arg.Number("name", (uint)1);
            argUInt.Should().BeOfType<NumberArg<uint>>();
            ((NumberArg<uint>)argUInt).Value.Should().Be(1);

            var argShort = Arg.Number("name", (short)1);
            argShort.Should().BeOfType<NumberArg<short>>();
            ((NumberArg<short>)argShort).Value.Should().Be(1);

            var argLong = Arg.Number("name", (long)1);
            argLong.Should().BeOfType<NumberArg<long>>();
            ((NumberArg<long>)argLong).Value.Should().Be(1);

            var argUShort = Arg.Number("name", (ushort)1);
            argUShort.Should().BeOfType<NumberArg<ushort>>();
            ((NumberArg<ushort>)argUShort).Value.Should().Be(1);

            var argULong = Arg.Number("name", (ulong)1);
            argULong.Should().BeOfType<NumberArg<ulong>>();
            ((NumberArg<ulong>)argULong).Value.Should().Be(1);

            var argDouble = Arg.Number("name", (double)1);
            argDouble.Should().BeOfType<NumberArg<double>>();
            ((NumberArg<double>)argDouble).Value.Should().Be(1);

            var argFloat = Arg.Number("name", (float)1);
            argFloat.Should().BeOfType<NumberArg<float>>();
            ((NumberArg<float>)argFloat).Value.Should().Be(1);

            var argByte = Arg.Number("name", (byte)1);
            argByte.Should().BeOfType<NumberArg<byte>>();
            ((NumberArg<byte>)argByte).Value.Should().Be(1);

            var argSByte = Arg.Number("name", (sbyte)1);
            argSByte.Should().BeOfType<NumberArg<sbyte>>();
            ((NumberArg<sbyte>)argSByte).Value.Should().Be(1);

            var argDecimal = Arg.Number("name", (decimal)1);
            argDecimal.Should().BeOfType<NumberArg<decimal>>();
            ((NumberArg<decimal>)argDecimal).Value.Should().Be(1);
        }

        [Fact]
        public void Should_ThrowException_When_NullName()
        {
            new Action(() => { Arg.Number(null, 0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, 0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, (uint)0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, (short)0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, (ushort)0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, (long)0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, (ulong)0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, (double)0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, (float)0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, (byte)0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, (sbyte)0); }).Should().ThrowExactly<ArgumentNullException>();
            new Action(() => { Arg.Number(null, (decimal)0); }).Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
