namespace Validot.Tests.Functional.Documentation
{
    using Validot.Tests.Functional.Documentation.Models;

    using Xunit;

    public class SpecificationFuncTests
    {
        [Fact]
        public void ChainCommands()
        {
            Specification<int> yearSpecification = s => s
                .GreaterThan(-10000)
                .NotEqualTo(0).WithMessage("There is no such year as 0")
                .LessThan(3000);

            _ = Validator.Factory.Create(yearSpecification);
        }

        [Fact]
        public void Scopes()
        {
            Specification<int> yearSpecification = s => s
                .GreaterThan(-10000)
                .NotEqualTo(0).WithMessage("There is no such year as 0")
                .LessThan(3000);

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.YearOfFirstAnnouncement, yearSpecification)
                .Member(m => m.YearOfPublication, m => m
                    .Positive()
                )
                .Rule(m => m.YearOfPublication == m.YearOfFirstAnnouncement).WithMessage("Same year in both places is invalid");

            _ = Validator.Factory.Create(bookSpecification);
        }
    }
}
