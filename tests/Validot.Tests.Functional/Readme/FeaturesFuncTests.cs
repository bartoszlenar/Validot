namespace Validot.Tests.Functional.Readme
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Testing;

    using Xunit;

    public static class RulesExtensions
    {
        public static IRuleOut<string> ExactLinesCount(this IRuleIn<string> @this, int count)
        {
            return @this.RuleTemplate(
                value => value.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length == count,
                "Must contain exactly {count} lines",
                Arg.Number("count", count)
            );
        }
    }

    public class FeaturesFuncTests
    {
        public class UserModel
        {
            public string Name { get; set; }

            public string LastName { get; set; }

            public string PrimaryEmail { get; set; }

            public IEnumerable<string> AlternativeEmails { get; set; }
        }

        public class BookModel
        {
            public string Title { get; set; }

            public string AuthorEmail { get; set; }

            public decimal Price { get; set; }
        }

        [Fact]
        public void FluentApi()
        {
            Specification<string> nameSpecification = s => s
                .LengthBetween(5, 50)
                .SingleLine()
                .Rule(name => name.All(char.IsLetterOrDigit));

            Specification<string> emailSpecification = s => s
                .Email()
                .Rule(email => email.All(char.IsLower)).WithMessage("Must contain only lower case characters");

            Specification<UserModel> userSpecification = s => s
                .Member(m => m.Name, nameSpecification).WithMessage("Must comply with name rules")
                .Member(m => m.PrimaryEmail, emailSpecification)
                .Member(m => m.AlternativeEmails, m => m
                    .Optional()
                    .MaxCollectionSize(3).WithMessage("Must not contain more than 3 addresses")
                    .AsCollection(emailSpecification)
                )
                .Rule(user =>
                {
                    return user.PrimaryEmail == null || user.AlternativeEmails?.Contains(user.PrimaryEmail) == false;
                }).WithMessage("Alternative emails must not contain the primary email address");

            _ = Validator.Factory.Create(userSpecification);
        }

        [Fact]
        public void Validators()
        {
            Specification<BookModel> bookSpecification = s => s
                .Optional()
                .Member(m => m.AuthorEmail, m => m.Optional().Email())
                .Member(m => m.Title, m => m.NotEmpty().LengthBetween(1, 100))
                .Member(m => m.Price, m => m.NonNegative());

            var bookValidator = Validator.Factory.Create(bookSpecification);

            var bookModel = new BookModel() { AuthorEmail = "inv@lid_em@il", Price = 10 };

            bookValidator.IsValid(bookModel).Should().BeFalse();

            bookValidator.Validate(bookModel).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "AuthorEmail: Must be a valid email address",
                "Title: Required"
            );

            bookValidator.Validate(bookModel, failFast: true).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "AuthorEmail: Must be a valid email address"
            );

            bookValidator.Template.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "AuthorEmail: Must be a valid email address",
                "Title: Required",
                "Title: Must not be empty",
                "Title: Must be between 1 and 100 characters in length",
                "Price: Must not be negative"
            );
        }

        [Fact]
        public void Rules()
        {
            Specification<string> specification1 = s => s
                .ExactLinesCount(4);

            Validator.Factory.Create(specification1).Validate(string.Empty).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain exactly 4 lines");

            Specification<string> specification2 = s => s
                .ExactLinesCount(4).WithMessage("Required lines count: {count}");

            Validator.Factory.Create(specification2).Validate(string.Empty).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required lines count: 4");

            Specification<string> specification3 = s => s
                .ExactLinesCount(4).WithMessage("Required lines count: {count|format=000.00|culture=pl-PL}");

            Validator.Factory.Create(specification3).Validate(string.Empty).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required lines count: 004,00");
        }

        [Fact]
        public void Translations()
        {
            Specification<UserModel> specification = s => s
                .Member(m => m.PrimaryEmail, m => m.Email())
                .Member(m => m.Name, m => m.LengthBetween(3, 50));

            var validator = Validator.Factory.Create(specification, settings => settings.WithPolishTranslation());

            var model = new UserModel() { PrimaryEmail = "in@lid_em@il", Name = "X" };

            var result = validator.Validate(model);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "PrimaryEmail: Must be a valid email address",
                "Name: Must be between 3 and 50 characters in length"
            );

            result.ToString(translationName: "Polish").ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "PrimaryEmail: Musi być poprawnym adresem email",
                "Name: Musi być długości pomiędzy 3 a 50 znaków"
            );
        }

        [Fact]
        public void HandlingNulls()
        {
            Specification<UserModel> specification1 = s => s
                .Member(m => m.LastName, m => m
                        .Rule(lastName => lastName.Length < 50)
                        .Rule(lastName => lastName.All(char.IsLetter))
                );

            var validationResult1 = Validator.Factory.Create(specification1).Validate(null);

            validationResult1.AnyErrors.Should().BeTrue();

            validationResult1.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required"
            );

            Specification<UserModel> specification2 = s => s
                .Optional()
                .Member(m => m.LastName, m => m
                    .Rule(lastName => lastName.Length < 50)
                    .Rule(lastName => lastName.All(char.IsLetter))
                );

            var validationResult2 = Validator.Factory.Create(specification2).Validate(null);

            validationResult2.AnyErrors.Should().BeFalse();
        }
    }
}
