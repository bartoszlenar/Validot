namespace Validot.Tests.Functional.Documentation
{
    using FluentAssertions;

    using Validot.Testing;
    using Validot.Tests.Functional.Documentation.Models;

    using Xunit;

    public class ErrorOutputFuncTests
    {
        [Fact]
        public void ErrorMessages()
        {
            Specification<int> yearSpecification = s => s
                .Rule(year => year > -300)
                    .WithMessage("Minimum year is 300 B.C.")
                    .WithExtraMessage("Ancient history date is invalid.")
                .Rule(year => year != 0)
                    .WithMessage("The year 0 is invalid.")
                    .WithExtraMessage("There is no such year as 0.")
                .Rule(year => year < 10000)
                    .WithMessage("Maximum year is 10000 A.D.");

            var validator = Validator.Factory.Create(yearSpecification);

            var result = validator.Validate(-500);

            result.MessageMap[""][0].Should().Be("Minimum year is 300 B.C.");
            result.MessageMap[""][1].Should().Be("Ancient history date is invalid.");

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Minimum year is 300 B.C.",
                "Ancient history date is invalid."
            );
        }

        [Fact]
        public void ErrorMessages_Paths()
        {
            Specification<int> yearSpecification = s => s
                .Rule(year => year > -300)
                    .WithMessage("Minimum year is 300 B.C.")
                    .WithExtraMessage("Ancient history date is invalid.")
                .Rule(year => year != 0)
                    .WithMessage("The year 0 is invalid.")
                    .WithExtraMessage("There is no such year as 0.")
                .Rule(year => year < 10000)
                    .WithMessage("Maximum year is 10000 A.D.");

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.YearOfFirstAnnouncement, yearSpecification)
                .Member(m => m.YearOfPublication, m => m.AsNullable(yearSpecification))
                .Rule(m => m.YearOfFirstAnnouncement <= m.YearOfPublication)
                    .WithCondition(m => m.YearOfPublication.HasValue)
                    .WithMessage("Year of publication must be after the year of first announcement");

            var validator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel() { YearOfFirstAnnouncement = 0, YearOfPublication = -100 };

            var result = validator.Validate(book);

            result.MessageMap["YearOfFirstAnnouncement"][0].Should().Be("The year 0 is invalid.");
            result.MessageMap["YearOfFirstAnnouncement"][1].Should().Be("There is no such year as 0.");
            result.MessageMap[""][0].Should().Be("Year of publication must be after the year of first announcement");

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Year of publication must be after the year of first announcement",
                "YearOfFirstAnnouncement: The year 0 is invalid.",
                "YearOfFirstAnnouncement: There is no such year as 0.");
        }

        [Fact]
        public void ErrorCodes()
        {
            Specification<int> yearSpecification = s => s
                .Rule(year => year > -300)
                    .WithCode("MIN_YEAR")
                .Rule(year => year != 0)
                    .WithCode("ZERO_YEAR")
                    .WithExtraCode("INVALID_VALUE")
                .Rule(year => year < 10000)
                    .WithPath("YearOfPublication")
                    .WithCode("MAX_YEAR");

            var validator = Validator.Factory.Create(yearSpecification);

            var result = validator.Validate(0);

            result.Codes.Should().Contain("ZERO_YEAR", "INVALID_VALUE");

            result.CodeMap[""][0].Should().Be("ZERO_YEAR");
            result.CodeMap[""][1].Should().Be("INVALID_VALUE");

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Codes,
                "ZERO_YEAR, INVALID_VALUE"
            );
        }
    }
}
