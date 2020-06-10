namespace Validot.Tests.Functional.Documentation
{
    using FluentAssertions;

    using Validot.Settings;
    using Validot.Testing;
    using Validot.Tests.Functional.Documentation.Models;

    using Xunit;

    public class ValidatorFuncTests
    {
        [Fact]
        public void Validator_Create()
        {
            Specification<BookModel> specification = s => s
                .Member(m => m.Title, m => m.NotEmpty())

                .Rule(m => m.YearOfPublication > m.YearOfFirstAnnouncement)
                .WithCondition(m => m.YearOfPublication.HasValue);

            var validator = new Validator<BookModel>(specification);

            _ = validator;
        }

        [Fact]
        public void Validator_CreateWithSettings()
        {
            Specification<BookModel> specification = s => s
                .Member(m => m.Title, m => m.NotEmpty())

                .Rule(m => m.YearOfPublication > m.YearOfFirstAnnouncement)
                .WithCondition(m => m.YearOfPublication.HasValue);

            var settings = new ValidatorSettings().WithPolishTranslation();

            var validator = new Validator<BookModel>(specification, settings);

            _ = validator;
        }

        [Fact]
        public void Validator_ExecutionOptimization()
        {
            Specification<BookModel> specification = s => s
                .Member(m => m.Title, m => m
                    .NotEmpty()
                    .NotWhiteSpace()
                    .NotEqualTo("blank")
                    .Rule(t => !t.StartsWith(" ")).WithMessage("Can't start with whitespace")
                )
                .WithMessage("Contains errors!");

            var validator = Validator.Factory.Create(specification);

            var book = new BookModel() { Title = "     " };

            validator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Title: Contains errors!");
        }

        [Fact]
        public void Validate()
        {
            Specification<BookModel> specification = s => s
                .Member(m => m.Title, m => m.NotEmpty())
                .Member(m => m.YearOfFirstAnnouncement, m => m.BetweenOrEqualTo(1000, 3000))

                .Rule(m => m.YearOfPublication >= m.YearOfFirstAnnouncement)
                .WithCondition(m => m.YearOfPublication.HasValue)
                .WithMessage("Year of publication needs to be after the first announcement");

            var validator = new Validator<BookModel>(specification);

            var book = new BookModel()
            {
                Title = "",
                YearOfPublication = 600,
                YearOfFirstAnnouncement = 666
            };

            var result = validator.Validate(book);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Title: Must not be empty",
                "YearOfFirstAnnouncement: Must be between 1000 and 3000 (inclusive)",
                "Year of publication needs to be after the first announcement");

            var failFastResult = validator.Validate(book, failFast: true);

            failFastResult.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Title: Must not be empty");
        }

        [Fact]
        public void Validate_IsValid()
        {
            Specification<BookModel> specification = s => s
                .Member(m => m.Title, m => m.NotEmpty())
                .Member(m => m.YearOfFirstAnnouncement, m => m.BetweenOrEqualTo(1000, 3000))

                .Rule(m => m.YearOfPublication >= m.YearOfFirstAnnouncement)
                .WithCondition(m => m.YearOfPublication.HasValue)
                .WithMessage("Year of publication needs to be after the first announcement");

            var validator = new Validator<BookModel>(specification);

            var book1 = new BookModel()
            {
                Title = "",
                YearOfPublication = 600,
                YearOfFirstAnnouncement = 666
            };

            validator.IsValid(book1).Should().BeFalse();

            var book2 = new BookModel()
            {
                Title = "test",
                YearOfPublication = 1666,
                YearOfFirstAnnouncement = 1600
            };

            validator.IsValid(book2).Should().BeTrue();
        }

        [Fact]
        public void Template()
        {
            Specification<string> specification = s => s
                .NotEmpty()
                .NotWhiteSpace().WithMessage("White space is not allowed")
                .Rule(m => m.Contains("@")).WithMessage("Must contain @ character");

            var validator = Validator.Factory.Create(specification);

            validator.Template.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required",
                "Must not be empty",
                "White space is not allowed",
                "Must contain @ character");
        }

        [Fact]
        public void Template_AsCollection()
        {
            Specification<BookModel> specification = s => s
                .Member(m => m.Authors, m => m
                    .AsCollection(m1 => m1
                        .Member(m2 => m2.Name, m2 => m2.NotEmpty())
                    )
                );

            var validator = Validator.Factory.Create(specification);

            validator.Template.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required",
                "Authors: Required",
                "Authors.#: Required",
                "Authors.#.Name: Required",
                "Authors.#.Name: Must not be empty");
        }

        [Fact]
        public void Template_ReferenceLoop()
        {
            Specification<B> specificationB = null;

            Specification<A> specificationA = s => s
                .Member(m => m.B, specificationB);

            specificationB = s => s
                .Member(m => m.A, specificationA);

            var validator = Validator.Factory.Create(specificationA);

            validator.Template.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required",
                "B: Required",
                "B.A: (reference loop)");

            var a = new A()
            {
                B = new B()
            };

            a.B.A = a;
        }

        [Fact]
        public void Template_Complex()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Member(m => m.Email, m => m
                    .NotWhiteSpace().WithMessage("Email cannot be whitespace")
                    .Email()
                )
                .Member(m => m.Name, m => m
                    .NotEmpty()
                    .NotWhiteSpace()
                    .MinLength(2)
                );

            Specification<BookModel> specification = s => s
                .Member(m => m.Title, m => m.NotEmpty()).WithExtraCode("EMPTY_TITLE")
                .Member(m => m.YearOfFirstAnnouncement, m => m.BetweenOrEqualTo(1000, 3000))
                .Member(m => m.Authors, m => m
                    .AsCollection(authorSpecification)
                    .MaxCollectionSize(4).WithMessage("Book shouldn't have more than 4 authors").WithExtraCode("MANY_AUTHORS")
                )
                .Rule(m => m.YearOfPublication >= m.YearOfFirstAnnouncement)
                .WithCondition(m => m.YearOfPublication.HasValue)
                .WithMessage("Year of publication needs to be after the first announcement");

            var validator = new Validator<BookModel>(specification);

            validator.Template.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.MessagesAndCodes,
                "EMPTY_TITLE, MANY_AUTHORS",
                "",
                "Required",
                "Year of publication needs to be after the first announcement",
                "Title: Required",
                "Title: Must not be empty",
                "YearOfFirstAnnouncement: Must be between 1000 and 3000 (inclusive)",
                "Authors: Required",
                "Authors: Book shouldn't have more than 4 authors",
                "Authors.#: Required",
                "Authors.#.Email: Required",
                "Authors.#.Email: Email cannot be whitespace",
                "Authors.#.Email: Must be a valid email address",
                "Authors.#.Name: Required",
                "Authors.#.Name: Must not be empty",
                "Authors.#.Name: Must not consist only of whitespace characters",
                "Authors.#.Name: Must be at least 2 characters in length");
        }
    }
}
