namespace Validot.Tests.Unit.Errors.Args
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using FluentAssertions;

    using Validot.Errors.Args;

    using Xunit;

    public class TimeArgTests
    {
        public static IEnumerable<object[]> Should_Stringify_Times_WithFormatAndCulture_Data()
        {
            yield return new object[]
            {
                Arg.Time("name", new DateTime(2000, 01, 15, 16, 04, 05, 06)),
                "s",
                "en-US",
                "2000-01-15T16:04:05"
            };

            yield return new object[]
            {
                Arg.Time("name", new DateTimeOffset(2000, 01, 15, 16, 04, 05, 06, TimeSpan.Zero)),
                "s",
                "en-US",
                "2000-01-15T16:04:05"
            };

            yield return new object[]
            {
                Arg.Time("name", new TimeSpan(1, 2, 3)),
                "g",
                "en-US",
                "1:02:03"
            };
        }

        [Theory]
        [MemberData(nameof(Should_Stringify_Times_WithFormatAndCulture_Data))]
        public void Should_Stringify_Times_WithFormatAndCulture(TimeArg arg, string format, string culture, string expectedString)
        {
            var stringified = arg.ToString(new Dictionary<string, string>
            {
                {
                    "format", format
                },
                {
                    "culture", culture
                }
            });

            stringified.Should().Be(expectedString);
        }

        public static IEnumerable<object[]> Should_Stringify_Dates_WithCulture_Data()
        {
            yield return new object[]
            {
                Arg.Time("name", new DateTime(2000, 01, 15, 16, 04, 05, 06)),
                "en-US",
                new DateTime(2000, 01, 15, 16, 04, 05, 06).ToString(CultureInfo.GetCultureInfo("en-US"))
            };

            yield return new object[]
            {
                Arg.Time("name", new DateTimeOffset(2000, 01, 15, 16, 04, 05, 06, TimeSpan.Zero)),
                "en-US",
                new DateTimeOffset(2000, 01, 15, 16, 04, 05, 06, TimeSpan.Zero).ToString(CultureInfo.GetCultureInfo("en-US"))
            };

            yield return new object[]
            {
                Arg.Time("name", new TimeSpan(1, 2, 3)),
                "en-US",
                "01:02:03"
            };
        }

        [Theory]
        [MemberData(nameof(Should_Stringify_Dates_WithCulture_Data))]
        public void Should_Stringify_Dates_WithCulture(TimeArg arg, string culture, string expectedString)
        {
            var stringified = arg.ToString(new Dictionary<string, string>
            {
                {
                    "culture", culture
                }
            });

            stringified.Should().Be(expectedString);
        }

        public static IEnumerable<object[]> Should_Stringify_WithFormat_Data()
        {
            yield return new object[]
            {
                Arg.Time("name", new DateTime(2000, 01, 15, 16, 04, 05, 06)),
                "s",
                "2000-01-15T16:04:05"
            };

            yield return new object[]
            {
                Arg.Time("name", new DateTime(2000, 01, 15, 16, 04, 05, 06)),
                "yyyyMMdd",
                "20000115"
            };

            yield return new object[]
            {
                Arg.Time("name", new DateTimeOffset(2000, 01, 15, 16, 04, 05, 06, TimeSpan.Zero)),
                "s",
                "2000-01-15T16:04:05"
            };

            yield return new object[]
            {
                Arg.Time("name", new DateTimeOffset(2000, 01, 15, 16, 04, 05, 06, TimeSpan.Zero)),
                "yyyyMMdd",
                "20000115"
            };

            yield return new object[]
            {
                Arg.Time("name", new TimeSpan(1, 2, 3)),
                "g",
                "1:02:03"
            };
        }

        [Theory]
        [MemberData(nameof(Should_Stringify_WithFormat_Data))]
        public void Should_Stringify_WithFormat(TimeArg arg, string format, string expectedString)
        {
            var stringified = arg.ToString(new Dictionary<string, string>
            {
                {
                    "format", format
                }
            });

            stringified.Should().Be(expectedString);
        }

        public static IEnumerable<object[]> Should_Stringify_Default_Data()
        {
            yield return new object[]
            {
                Arg.Time("name", new DateTime(2000, 01, 15, 16, 04, 05, 06)),
                "2000-01-15 16:04:05.006"
            };

            yield return new object[]
            {
                Arg.Time("name", new DateTimeOffset(2000, 01, 15, 16, 04, 05, 06, TimeSpan.Zero)),
                "2000-01-15 16:04:05.006"
            };

            yield return new object[]
            {
                Arg.Time("name", new TimeSpan(1, 2, 3)),
                "01:02:03"
            };
        }

        [Theory]
        [MemberData(nameof(Should_Stringify_Default_Data))]
        public void Should_Stringify_Default(TimeArg arg, string expectedString)
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
        }

        [Fact]
        public void Should_ThrowException_When_NullName()
        {
            new Action(() =>
            {
                Arg.Time(null, TimeSpan.FromTicks(0));
            }).Should().ThrowExactly<ArgumentNullException>();

            new Action(() =>
            {
                Arg.Time(null, new DateTime(0));
            }).Should().ThrowExactly<ArgumentNullException>();

            new Action(() =>
            {
                Arg.Time(null, new DateTimeOffset(0, TimeSpan.Zero));
            }).Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
