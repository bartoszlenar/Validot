namespace Validot.Tests.Functional.Documentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AssemblyWithHolders;
    using FluentAssertions;
    using Validot.Factory;
    using Validot.Settings;
    using Validot.Testing;
    using Validot.Tests.Functional.Documentation.Models;
    using Xunit;

    public class FactoryFuncTests
    {
        [Fact]
        public void Specification()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Member(m => m.Email, m => m
                    .Email()
                    .And()
                    .EndsWith("@gmail.com")
                    .WithMessage("Only gmail accounts are accepted")
                );

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Title, m => m.NotEmpty().NotWhiteSpace())
                .Member(m => m.Authors, m => m.AsCollection(authorSpecification));

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

            var validator = Validator.Factory.Create(bookSpecification, s => s
                .WithTranslation("English", "Texts.Email", "This is not a valid email address!")
            );

            validator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Title: Must not consist only of whitespace characters",
                "Authors.#1.Email: Only gmail accounts are accepted",
                "Authors.#2.Email: This is not a valid email address!",
                "Authors.#2.Email: Only gmail accounts are accepted"
            );

            var validator2 = Validator.Factory.Create(bookSpecification, validator.Settings);

            validator2.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Title: Must not consist only of whitespace characters",
                "Authors.#1.Email: Only gmail accounts are accepted",
                "Authors.#2.Email: This is not a valid email address!",
                "Authors.#2.Email: Only gmail accounts are accepted"
            );
        }

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
        public void SettingsHolder()
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

            validator.Settings.Translations.Keys.Should().HaveCount(2);

            validator.Settings.Translations.Keys.Should().Contain(new[]
            {
                "English", "Polish"
            });

            validator.Settings.ReferenceLoopProtectionEnabled.Should().BeTrue();

            var validator2 = Validator.Factory.Create(
                new AuthorSpecificationHolder(),
                s => s
                    .WithReferenceLoopProtectionDisabled()
                    .WithTranslation("English", "Invalid email", "The email address is invalid")
            );

            validator2.Validate(author).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Name must not be empty",
                "Email: The email address is invalid");

            validator2.Settings.ReferenceLoopProtectionEnabled.Should().BeFalse();
        }

        [Fact]
        public void SettingsHolderWithInlineModification()
        {
            var validator = Validator.Factory.Create(
                new AuthorSpecificationHolder(),
                s => s
                    .WithReferenceLoopProtectionDisabled()
                    .WithTranslation("English", "Invalid email", "The email address is invalid")
            );

            var author = new AuthorModel()
            {
                Name = "",
                Email = "john.doe@outlook.com",
            };

            validator.Validate(author).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Name must not be empty",
                "Email: The email address is invalid");

            validator.Settings.ReferenceLoopProtectionEnabled.Should().BeFalse();
        }

        [Fact]
        public void ReusingSettings()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Member(m => m.Email, m => m.Email().EndsWith("@gmail.com"))
                .WithMessage("Invalid email")
                .And()
                .Member(m => m.Name, m => m.NotEmpty())
                .WithMessage("Name.EmptyValue");

            var validator1 = Validator.Factory.Create(
                    authorSpecification,
                    s => s
                    .WithTranslation("English", "Invalid email", "The email address is invalid")
                    .WithTranslation("English", "Name.EmptyValue", "Name must not be empty")
            );

            var validator2 = Validator.Factory.Create(authorSpecification, validator1.Settings);

            var author = new AuthorModel()
            {
                Name = "",
                Email = "john.doe@outlook.com",
            };

            validator1.Validate(author).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Name must not be empty",
                "Email: The email address is invalid");

            validator2.Validate(author).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Name must not be empty",
                "Email: The email address is invalid");

            validator1.Settings.Should().BeSameAs(validator2.Settings);
        }

        [Fact]
        public void FetchingHolders()
        {
            var assemblies = new[]
            {
                typeof(HolderOfIntSpecificationAndSettings).Assembly
            };

            var holder = Validator.Factory.FetchHolders(assemblies).Single(h => h.HolderType == typeof(HolderOfIntSpecificationAndSettings));

            var validator = (Validator<int>)holder.CreateValidator();

            validator.Validate(11).ToString(translationName: "BinaryEnglish").ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The maximum value is 0b1010"
            );
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

        public class AuthorSpecificationHolder : ISpecificationHolder<AuthorModel>, ISettingsHolder
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

                Settings = s => s
                    .WithReferenceLoopProtection()
                    .WithPolishTranslation()
                    .WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>()
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
                    });
            }

            public Specification<AuthorModel> Specification { get; }

            public Func<ValidatorSettings, ValidatorSettings> Settings { get; }
        }
    }
}
