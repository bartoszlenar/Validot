namespace Validot.Tests.Functional.Documentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Validot.Testing;
    using Validot.Tests.Functional.Documentation.Models;

    using Xunit;

    public class ResultFuncTests
    {
        [Fact]
        public void AnyErrors()
        {
            Specification<string> specification = s => s
                .NotEmpty();

            var validator = Validator.Factory.Create(specification);

            var result1 = validator.Validate("test");

            result1.AnyErrors.Should().BeFalse();

            var result2 = validator.Validate("");

            result2.AnyErrors.Should().BeTrue();
        }

        [Fact]
        public void Paths()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Member(m => m.Email, m => m.Email().WithCode("EMAIL"))
                .Member(m => m.Name, m => m
                    .NotEmpty()
                    .MinLength(3)
                    .NotContains("X").WithMessage("X character is not allowed in name")
                );

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Title, m => m.NotWhiteSpace())
                .Member(m => m.Authors, m => m
                    .AsCollection(authorSpecification)
                )
                .Rule(m => m.IsSelfPublished == false).WithCode("ERROR_SELF_PUBLISHED");

            var bookValidator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Title = "",
                Authors = new[]
                {
                    new AuthorModel() { Email = "john.doe@gmail.com", Name = "X" },
                    new AuthorModel() { Email = "jane.doe@gmail.com", Name = "Jane" },
                    new AuthorModel() { Email = "inv@lidem@il", Name = "Jane" }
                },
                IsSelfPublished = true
            };

            var result = bookValidator.Validate(book);

            result.Paths.Should().HaveCount(4);
            result.Paths.Should().Contain("");
            result.Paths.Should().Contain("Title");
            result.Paths.Should().Contain("Authors.#0.Name");
            result.Paths.Should().Contain("Authors.#2.Email");
        }

        [Fact]
        public void Codes()
        {
            Specification<PublisherModel> specification = s => s
                .Member(m => m.Name, m => m
                    .NotEmpty().WithCode("EMPTY_FIELD").WithExtraCode("NAME_ERROR")
                    .MinLength(3).WithCode("SHORT_FIELD").WithExtraCode("NAME_ERROR")
                )
                .Member(m => m.CompanyId, m => m
                    .NotEmpty().WithCode("EMPTY_FIELD").WithExtraCode("COMPANYID_ERROR")
                    .NotContains("ID").WithCode("ID_IN_CONTENT")
                )
                .Rule(m => m.Name != m.CompanyId).WithCode("SAME_VALUES");

            var validator = Validator.Factory.Create(specification);

            var publisher = new PublisherModel()
            {
                Name = "",
                CompanyId = ""
            };

            var result = validator.Validate(publisher);

            result.Codes.Should().HaveCount(5);
            result.Codes.Should().Contain("EMPTY_FIELD", "NAME_ERROR", "SHORT_FIELD", "COMPANYID_ERROR", "SAME_VALUES");
        }

        [Fact]
        public void CodeMap()
        {
            Specification<PublisherModel> specification = s => s
                .Member(m => m.Name, m => m
                    .NotEmpty().WithCode("EMPTY_FIELD").WithExtraCode("NAME_ERROR")
                    .MinLength(3).WithCode("SHORT_FIELD").WithExtraCode("NAME_ERROR")
                )
                .Member(m => m.CompanyId, m => m
                    .NotEmpty().WithCode("EMPTY_FIELD").WithExtraCode("COMPANYID_ERROR")
                    .NotContains("company").WithCode("COPANY_IN_CONTENT")
                    .NotContains("id").WithMessage("Invalid company value")
                )
                .Rule(m => m.Name is null || m.CompanyId is null).WithCode("NULL_MEMBER");

            var validator = Validator.Factory.Create(specification);

            var publisher = new PublisherModel()
            {
                Name = "",
                CompanyId = "some_id"
            };

            var result = validator.Validate(publisher);

            result.CodeMap["Name"].Should().Contain(new[] { "EMPTY_FIELD", "NAME_ERROR", "SHORT_FIELD", "NAME_ERROR" });
            result.CodeMap["Name"].Where(n => n == "NAME_ERROR").Should().HaveCount(2);
            result.CodeMap[""].Should().ContainSingle("NULL_MEMBER");

            result.Paths.Contains("CompanyId").Should().BeTrue();

            result.CodeMap.Keys.Contains("CompanyId").Should().BeFalse();

            result.MessageMap.Keys.Contains("CompanyId").Should().BeTrue();
        }

        [Fact]
        public void MessageMap()
        {
            Specification<PublisherModel> specification = s => s
                .Member(m => m.Name, m => m
                    .NotEmpty().WithMessage("The field is empty").WithExtraMessage("Error in Name field")
                    .MinLength(3).WithMessage("The field is too short").WithExtraMessage("Error in Name field")
                )
                .Member(m => m.CompanyId, m => m
                    .NotEmpty().WithMessage("The field is empty").WithExtraMessage("Error in CompanyId field")
                    .NotContains("company").WithMessage("Company Id cannot contain 'company' word")
                    .NotContains("id").WithCode("ID_IN_COMPANY")
                )
                .Rule(m => m.Name is null || m.CompanyId is null)
                .WithMessage("All members must be present");

            var validator = Validator.Factory.Create(specification);

            var publisher = new PublisherModel()
            {
                Name = "",
                CompanyId = "some_id"
            };

            var result = validator.Validate(publisher);

            result.MessageMap["Name"].Should().Contain(new[] { "The field is empty", "Error in Name field", "The field is too short", "Error in Name field" });
            result.MessageMap["Name"].Where(n => n == "Error in Name field").Should().HaveCount(2);
            result.MessageMap[""].Should().ContainSingle("All members must be present");

            result.Paths.Contains("CompanyId").Should().BeTrue();

            result.MessageMap.Keys.Contains("CompanyId").Should().BeFalse();
            result.CodeMap.Keys.Contains("CompanyId").Should().BeTrue();
        }

        [Fact]
        public void GetTranslatedMessageMap()
        {
            Specification<AuthorModel> specification = s => s
                .Member(m => m.Name, m => m
                    .NotEmpty()
                    .MinLength(3).WithMessage("Name is too short")
                )
                .Member(m => m.Email, m => m
                    .Email()
                );

            var validator = Validator.Factory.Create(specification, settings => settings
                .WithPolishTranslation()
                .WithTranslation("Polish", "Name is too short", "Imię jest zbyt krótkie")
            );

            var author = new AuthorModel()
            {
                Name = "",
                Email = "inv@lidem@il"
            };

            var result = validator.Validate(author);

            var englishMessageMap = result.GetTranslatedMessageMap("English");

            englishMessageMap["Name"].Should().Contain(new[] { "Must not be empty", "Name is too short" });
            englishMessageMap["Email"].Should().ContainSingle("Must be a valid email address");

            var polishMessageMap = result.GetTranslatedMessageMap("Polish");

            polishMessageMap["Name"].Should().Contain(new[] { "Musi nie być puste", "Imię jest zbyt krótkie" });
            polishMessageMap["Email"].Should().ContainSingle("Musi być poprawnym adresem email");
        }

        [Fact]
        public void GetTranslatedMessageMap_InvalidTranslationName()
        {
            Specification<AuthorModel> specification = s => s
                .Member(m => m.Name, m => m
                    .NotEmpty()
                    .MinLength(3).WithMessage("Name is too short")
                )
                .Member(m => m.Email, m => m
                    .Email()
                );

            var author = new AuthorModel()
            {
                Name = "",
                Email = "inv@lidem@il"
            };

            var validator = Validator.Factory.Create(specification, settings => settings
                .WithPolishTranslation()
                .WithTranslation("Polish", "Name is too short", "Imię jest zbyt krótkie")
            );

            var result = validator.Validate(author);

            Action action = () =>
            {
                _ = result.GetTranslatedMessageMap("Russian");
            };

            action.Should().ThrowExactly<KeyNotFoundException>();
        }

        [Fact]
        public void TranslationNames_Default()
        {
            Specification<AuthorModel> specification = s => s.Rule(m => false);

            var model = new AuthorModel();

            var validator = Validator.Factory.Create(specification);

            var result = validator.Validate(model);

            result.TranslationNames.Should().ContainSingle("English");
        }

        [Fact]
        public void TranslationNames()
        {
            Specification<AuthorModel> specification = s => s.Rule(m => false);

            var model = new AuthorModel();

            var validator = Validator.Factory.Create(specification, settings => settings
                .WithPolishTranslation()
            );

            var result = validator.Validate(model);

            result.TranslationNames.Should().Contain(new[] { "Polish", "English" });
        }

        [Fact]
        public void ToStringMethod()
        {
            Specification<PublisherModel> specification = s => s
                .Member(m => m.Name, m => m
                    .NotEmpty()
                    .WithMessage("The field is empty")
                    .WithExtraMessage("Error in Name field")
                    .WithExtraCode("NAME_EMPTY")

                    .MinLength(3)
                    .WithMessage("The field is too short")
                    .WithExtraCode("NAME_TOO_SHORT")
                )
                .Member(m => m.CompanyId, m => m
                    .NotEmpty()
                    .NotContains("id")
                    .WithCode("ID_IN_COMPANY")
                )
                .Rule(m => m.Name is null || m.CompanyId is null)
                .WithMessage("All members must be present");

            var validator = Validator.Factory.Create(specification);

            var publisher = new PublisherModel()
            {
                Name = "",
                CompanyId = "some_id"
            };

            var result = validator.Validate(publisher);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.MessagesAndCodes,
                "NAME_EMPTY, NAME_TOO_SHORT, ID_IN_COMPANY",
                "",
                "Name: The field is empty",
                "Name: Error in Name field",
                "Name: The field is too short",
                "All members must be present"
            );
        }

        [Fact]
        public void ToStringMethod_Translation()
        {
            Specification<PublisherModel> specification = s => s
                .Member(m => m.Name, m => m
                    .NotEmpty()
                    .MinLength(3)
                )
                .Member(m => m.CompanyId, m => m
                    .NotEmpty().WithMessage("CompanyId field is required")
                );

            var validator = Validator.Factory.Create(specification, settings => settings
                .WithPolishTranslation()
                .WithTranslation("Polish", "CompanyId field is required", "Pole CompanyId jest wymagane")
            );

            var publisher = new PublisherModel()
            {
                Name = "",
                CompanyId = ""
            };

            var result = validator.Validate(publisher);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Must not be empty",
                "Name: Must be at least 3 characters in length",
                "CompanyId: CompanyId field is required"
            );

            result.ToString("Polish").ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Musi nie być puste",
                "Name: Musi być długości minimalnie 3 znaków",
                "CompanyId: Pole CompanyId jest wymagane"
            );

            Action action = () =>
            {
                result.ToString("Russian");
            };

            action.Should().ThrowExactly<KeyNotFoundException>();
        }

        [Fact]
        public void ToStringMethod_Valid()
        {
            Specification<PublisherModel> specification = s => s;

            var validator = Validator.Factory.Create(specification);

            var model = new PublisherModel();

            var result = validator.Validate(model);

            result.AnyErrors.Should().BeFalse();

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "OK"
            );
        }
    }
}
