namespace Validot.Tests.Functional.Documentation
{
    using System.Collections.Generic;

    using Validot.Testing;
    using Validot.Tests.Functional.Documentation.Models;

    using Xunit;

    public class FactoryFuncTests
    {
        [Fact]
        public void SpecificationHolder()
        {
            var validator = Validator.Factory.Create(new BookSpecificationHolder());

            var book = new BookModel()
            {
                Title = "   ",
                Authors = new[]
                {
                    new AuthorModel() { Email = "john.doe@gmail.com" },
                    new AuthorModel() { Email = "john.doe@outlook.com" },
                    new AuthorModel() { Email = "inv@lidem@il" },
                }
            };

            validator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Title: Must not consist only of whitespace characters",
                "Authors.#1.Email: Only gmail accounts are accepted",
                "Authors.#2.Email: Must be a valid email address",
                "Authors.#2.Email: Only gmail accounts are accepted"
            );
        }

        [Fact]
        public void TranslationHolder()
        {
            var validator = Validator.Factory.Create(new AuthorSpecificationHolder());

            var author = new AuthorModel()
            {
                Name = "",
                Email = "john.doe@outlook.com",
            };

            var result = validator.Validate(author);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Name must not be empty",
                "Email: Invalid email");

            result.ToString("Polish").ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Imię nie może być puste",
                "Email: Nieprawidłowy email");
        }

        public class BookSpecificationHolder : ISpecificationHolder<BookModel>
        {
            public BookSpecificationHolder()
            {
                Specification<string> titleSpecification = s => s
                    .NotEmpty()
                    .NotWhiteSpace();

                Specification<string> emailSpecification = s => s
                    .Email()
                    .EndsWith("@gmail.com").WithMessage("Only gmail accounts are accepted");

                Specification<AuthorModel> authorSpecification = s => s
                    .Member(m => m.Email, emailSpecification);

                Specification<BookModel> bookSpecification = s => s
                    .Member(m => m.Title, titleSpecification)
                    .Member(m => m.Authors, m => m.AsCollection(authorSpecification));

                Specification = bookSpecification;
            }

            public Specification<BookModel> Specification { get; }
        }

        public class AuthorSpecificationHolder : ISpecificationHolder<AuthorModel>, ITranslationHolder
        {
            public AuthorSpecificationHolder()
            {
                Specification<string> emailSpecification = s => s
                    .Email()
                    .EndsWith("@gmail.com");

                Specification<AuthorModel> authorSpecification = s => s
                    .Member(m => m.Email, emailSpecification).WithMessage("Invalid email")
                    .Member(m => m.Name, m => m.NotEmpty()).WithMessage("Name.EmptyValue");

                Specification = authorSpecification;

                Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["English"] = new Dictionary<string, string>()
                    {
                        ["Name.EmptyValue"] = "Name must not be empty"
                    },
                    ["Polish"] = new Dictionary<string, string>()
                    {
                        ["Invalid email"] = "Nieprawidłowy email",
                        ["Name.EmptyValue"] = "Imię nie może być puste"
                    }
                };
            }

            public Specification<AuthorModel> Specification { get; }

            public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; }
        }
    }
}
