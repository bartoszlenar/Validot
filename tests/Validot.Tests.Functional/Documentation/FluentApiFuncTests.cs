namespace Validot.Tests.Functional.Documentation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using FluentAssertions;

    using Validot.Testing;
    using Validot.Tests.Functional.Documentation.Models;

    using Xunit;

    public class FluentApiFuncTests
    {
        [Fact]
        public void Rule()
        {
            Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

            Specification<int> ageSpecification = m => m.Rule(isAgeValid);

            var ageValidator = Validator.Factory.Create(ageSpecification);

            ageValidator.IsValid(12); // true
            ageValidator.IsValid(20); // false

            ageValidator.Validate(32).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Error");
        }

        [Fact]
        public void Rule_WithMessage()
        {
            Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);
            Specification<int> ageSpecification = m => m.Rule(isAgeValid).WithMessage("The age is invalid");

            var ageValidator = Validator.Factory.Create(ageSpecification);

            ageValidator.Validate(32).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The age is invalid");
        }

        [Fact]
        public void Rule_Relation()
        {
            Specification<BookModel> bookSpecification = m => m
                .Rule(book => book.IsSelfPublished == (book.Publisher is null)).WithMessage("Book must have a publisher or be self-published.");

            var bookValidator = Validator.Factory.Create(bookSpecification);

            bookValidator.Validate(new BookModel() { IsSelfPublished = true, Publisher = new PublisherModel() }).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Book must have a publisher or be self-published."
            );

            bookValidator.Validate(new BookModel() { IsSelfPublished = true, Publisher = null }).AnyErrors.Should().BeFalse();
        }

        [Fact]
        public void Rule_NullMembers()
        {
            Specification<PublisherModel> publisherSpecification = m => m
                .Rule(publisher =>
                {
                    if (publisher.Name.Contains(publisher.CompanyId))
                    {
                        return false;
                    }

                    return true;
                });

            var validator = Validator.Factory.Create(publisherSpecification);

            Action action = () =>
            {
                validator.Validate(new PublisherModel());
            };

            action.Should().ThrowExactly<NullReferenceException>();
        }

        [Fact]
        public void Rule_ExceptionBubbledUp()
        {
            var verySpecialException = new VerySpecialException();

            Specification<BookModel> bookSpecification = m => m.Rule(book => throw verySpecialException);

            var bookValidator = Validator.Factory.Create(bookSpecification);

            try
            {
                bookValidator.Validate(new BookModel());
            }
            catch (VerySpecialException exception)
            {
                object.ReferenceEquals(exception, verySpecialException).Should().BeTrue();
            }
        }

        [Fact]
        public void RuleTemplate_RuleTemplateVsRule()
        {
            Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

            Specification<int> ageSpecification1 = m => m.Rule(isAgeValid).WithMessage("The age is invalid");

            Specification<int> ageSpecification2 = m => m.RuleTemplate(isAgeValid, "The age is invalid");

            var ageValidator1 = Validator.Factory.Create(ageSpecification1);

            var ageValidator2 = Validator.Factory.Create(ageSpecification2);

            ageValidator1.Validate(32).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The age is invalid");

            ageValidator2.Validate(32).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The age is invalid");
        }

        [Fact]
        public void RuleTemplate_Args()
        {
            Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

            Specification<int> ageSpecification = m => m
                .RuleTemplate(isAgeValid, "Age must be between {minAge} and {maxAge}", Arg.Number("minAge", 0), Arg.Number("maxAge", 18));

            var ageValidator = Validator.Factory.Create(ageSpecification);

            ageValidator.Validate(32).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Age must be between 0 and 18"
            );
        }

        [Fact]
        public void RuleTemplate_ArgsWithParameters()
        {
            Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

            Specification<int> ageSpecification = m => m
                .RuleTemplate(
                    isAgeValid,
                    "Age must be between {minAge|format=0.00} and {maxAge|format=0.00|culture=pl-PL}",
                    Arg.Number("minAge", 0),
                    Arg.Number("maxAge", 18)
                );

            var ageValidator = Validator.Factory.Create(ageSpecification);

            ageValidator.Validate(32).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Age must be between 0.00 and 18,00"
            );
        }

        [Fact]
        public void RuleTemplate_InvalidPlaceholders()
        {
            Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

            Specification<int> ageSpecification = m => m
                .RuleTemplate(
                    isAgeValid,
                    "Age must be between {minAge|format=0.00} and {maximumAge|format=0.00|culture=pl-PL}",
                    Arg.Number("minAge", 0),
                    Arg.Number("maxAge", 18)
                );

            var ageValidator = Validator.Factory.Create(ageSpecification);

            ageValidator.Validate(32).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Age must be between 0.00 and {maximumAge|format=0.00|culture=pl-PL}"
            );
        }

        [Fact]
        public void RuleTemplate_MultipleParameters()
        {
            Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

            Specification<int> ageSpecification = m => m
                .RuleTemplate(
                    isAgeValid,
                    "Age must be between {minAge|format=0.00} and {maxAge|format=0.00|culture=pl-PL}",
                    Arg.Number("minAge", 0),
                    Arg.Number("maxAge", 18)
                )
                .WithExtraMessage("Must be more than {minAge}")
                    .WithExtraMessage("Must be below {maxAge|format=0.00}! {maxAge}!");

            var ageValidator = Validator.Factory.Create(ageSpecification);

            ageValidator.Validate(32).ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Age must be between 0.00 and 18,00",
                    "Must be more than 0",
                    "Must be below 18.00! 18!"
                );
        }

        [Fact]
        public void RuleTemplate_WithMessageUsingArgs()
        {
            Predicate<int> isAgeValid = age => (age >= 0) && (age < 18);

            Specification<int> ageSpecification = m => m
                .RuleTemplate(
                    isAgeValid,
                    "Age must be between {minAge|format=0.00} and {maxAge|format=0.00|culture=pl-PL}",
                    Arg.Number("minAge", 0),
                    Arg.Number("maxAge", 18)
                )
                .WithMessage("Only {minAge}-{maxAge}!");

            var ageValidator = Validator.Factory.Create(ageSpecification);

            ageValidator.Validate(32).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Only 0-18!"
            );
        }

        [Fact]
        public void Member()
        {
            Specification<string> nameSpecification = s => s
                .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
                .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!");

            var nameValidator = Validator.Factory.Create(nameSpecification);

            nameValidator.Validate("Adam !!!").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must consist of letters only!",
                "Must not contain whitespace!"
            );

            Specification<PublisherModel> publisherSpecification = s => s
                .Member(m => m.Name, nameSpecification);

            var publisherValidator = Validator.Factory.Create(publisherSpecification);

            var publisher = new PublisherModel()
            {
                Name = "Adam !!!"
            };

            publisherValidator.Validate(publisher).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Must consist of letters only!",
                "Name: Must not contain whitespace!"
            );
        }

        [Fact]
        public void Member_NextLevel()
        {
            Specification<string> nameSpecification = s => s
                .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
                .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!");

            Specification<PublisherModel> publisherSpecification = s => s
                .Member(m => m.Name, nameSpecification);

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Publisher, publisherSpecification);

            var bookValidator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Publisher = new PublisherModel()
                {
                    Name = "Adam !!!"
                }
            };

            bookValidator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Publisher.Name: Must consist of letters only!",
                "Publisher.Name: Must not contain whitespace!"
            );
        }

        [Fact]
        public void Member_Inline()
        {
            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Publisher, m => m
                    .Member(m1 => m1.Name, m1 => m1
                        .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
                        .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!")
                    )
                );

            var bookValidator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Publisher = new PublisherModel()
                {
                    Name = "Adam !!!"
                }
            };

            bookValidator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Publisher.Name: Must consist of letters only!",
                "Publisher.Name: Must not contain whitespace!"
            );
        }

        [Fact]
        public void Member_InvalidMemberSelector()
        {
            Specification<string> nameSpecification = s => s
                .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
                .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!");

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Publisher.Name, nameSpecification);

            Action action = () =>
            {
                _ = Validator.Factory.Create(bookSpecification);
            };

            action.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Member_HandlingNulls()
        {
            Specification<PublisherModel> publisherSpecification = s => s
                .Member(m => m.Name, m => m
                    .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
                    .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!")
                );

            Specification<PublisherModel> publisherSpecificationRequired = s => s
                .Member(m => m.Name, m => m
                    .Required().WithMessage("Must be filled in!")
                    .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
                    .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!")
                );

            Specification<PublisherModel> publisherSpecificationOptional = s => s
                .Member(m => m.Name, m => m
                    .Optional()
                    .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
                    .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!")
                );

            var publisherValidator = Validator.Factory.Create(publisherSpecification);
            var publisherValidatorRequired = Validator.Factory.Create(publisherSpecificationRequired);
            var publisherValidatorOptional = Validator.Factory.Create(publisherSpecificationOptional);

            var publisher = new PublisherModel()
            {
                Name = null
            };

            publisherValidator.Validate(publisher).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Required"
            );

            publisherValidatorRequired.Validate(publisher).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Must be filled in!"
            );

            publisherValidatorOptional.Validate(publisher).AnyErrors.Should().BeFalse();
        }

        [Fact]
        public void AsModel()
        {
            Specification<string> emailSpecification = s => s
                .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!");

            Specification<string> emailAsModelSpecification = s => s
                .AsModel(emailSpecification);

            var emailValidator = Validator.Factory.Create(emailSpecification);
            var emailAsModelValidator = Validator.Factory.Create(emailAsModelSpecification);

            emailValidator.Validate("invalid email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character!"
            );

            emailAsModelValidator.Validate("invalid email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character!"
            );
        }

        [Fact]
        public void AsModel_NestedLevels()
        {
            Specification<string> emailSpecification = s => s
                .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!");

            Specification<string> emailNestedAsModelSpecification = s => s
                .AsModel(s1 => s1
                    .AsModel(s2 => s2
                        .AsModel(emailSpecification)
                    )
                );

            var emailValidator = Validator.Factory.Create(emailSpecification);
            var emailAsModelValidator = Validator.Factory.Create(emailNestedAsModelSpecification);

            emailValidator.Validate("invalid email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character!"
            );

            emailAsModelValidator.Validate("invalid email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character!"
            );
        }

        [Fact]
        public void AsModel_MergingSpecifications()
        {
            Specification<string> atRequiredSpecification = s => s
                .Rule(text => text.Contains('@')).WithMessage("Must contain @ character!");

            Specification<string> allLettersLowerCaseSpecification = s => s
                .Rule(text => !text.Any(c => !char.IsLetter(c) || char.IsLower(c))).WithMessage("All letters need to be lower case!");

            Specification<string> lengthSpecification = s => s
                .Rule(text => text.Length > 5).WithMessage("Must be longer than 5 characters")
                .Rule(text => text.Length < 20).WithMessage("Must be shorter than 20 characters");

            Specification<string> emailSpecification = s => s
                .AsModel(atRequiredSpecification)
                .AsModel(allLettersLowerCaseSpecification)
                .AsModel(lengthSpecification);

            var emailValidator = Validator.Factory.Create(emailSpecification);

            emailValidator.Validate("Email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character!",
                "All letters need to be lower case!",
                "Must be longer than 5 characters"
            );
        }

        [Fact]
        public void AsModel_ChangingPresenceRule_ToOptional()
        {
            Specification<string> atRequiredSpecification = s => s
                .Rule(text => text.Contains('@')).WithMessage("Must contain @ character!");

            Specification<string> allLettersLowerCaseSpecification = s => s
                .Rule(text => !text.Any(c => !char.IsLetter(c) || char.IsLower(c))).WithMessage("All letters need to be lower case!");

            Specification<string> emailSpecification = s => s
                .Optional()
                .AsModel(atRequiredSpecification)
                .AsModel(allLettersLowerCaseSpecification)
                .Rule(text => text.Length > 5).WithMessage("Must be longer than 5 characters")
                .Rule(text => text.Length < 20).WithMessage("Must be shorter than 20 characters");

            var emailValidator = Validator.Factory.Create(emailSpecification);

            emailValidator.Validate("Email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character!",
                "All letters need to be lower case!",
                "Must be longer than 5 characters"
            );

            emailValidator.Validate(null).AnyErrors.Should().BeFalse();
        }

        [Fact]
        public void AsModel_ChangingPresenceRule_ToRequired()
        {
            Specification<string> emailOptionalSpecification = s => s
                .Optional()
                .Rule(text => text.Contains('@')).WithMessage("Must contain @ character!");

            Specification<string> emailSpecification = s => s
                .AsModel(emailOptionalSpecification);

            var emailOptionalValidator = Validator.Factory.Create(emailOptionalSpecification);

            var emailValidator = Validator.Factory.Create(emailSpecification);

            emailOptionalValidator.Validate(null).AnyErrors.Should().BeFalse();

            emailOptionalValidator.Validate("Email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character!"
            );

            emailValidator.Validate(null).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required"
            );

            emailValidator.Validate("Email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character!"
            );
        }

        [Fact]
        public void AsModel_BundleRules()
        {
            Specification<string> emailSpecification = s => s
                .Rule(text => text.Contains('@')).WithMessage("Must contain @ character!")
                .Rule(text => !text.Any(c => !char.IsLetter(c) || char.IsLower(c))).WithMessage("All letters need to be lower case!")
                .Rule(text => text.Length > 5).WithMessage("Must be longer than 5 characters")
                .Rule(text => text.Length < 20).WithMessage("Must be shorter than 20 characters");

            Specification<string> emailWrapperSpecification = s => s
                .AsModel(emailSpecification).WithMessage("This value is invalid as email address");

            var emailValidator = Validator.Factory.Create(emailSpecification);

            var emailWrapperValidator = Validator.Factory.Create(emailWrapperSpecification);

            emailValidator.Validate("Email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character!",
                "All letters need to be lower case!",
                "Must be longer than 5 characters"
            );

            emailWrapperValidator.Validate("Email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "This value is invalid as email address"
            );
        }

        [Fact]
        public void AsModel_BundleRules_InlineAndWithoutMessages()
        {
            Specification<string> emailSpecification = s => s
                .AsModel(s1 => s1
                    .Rule(text => text.Contains('@'))
                    .Rule(text => !text.Any(c => !char.IsLetter(c) || char.IsLower(c)))
                    .Rule(text => text.Length > 5)
                    .Rule(text => text.Length < 20)
                ).WithMessage("This value is invalid as email address");

            var emailValidator = Validator.Factory.Create(emailSpecification);

            emailValidator.Validate("Email").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "This value is invalid as email address");
        }

        [Fact]
        public void AsCollection()
        {
            Specification<int> evenNumberSpecification = s => s
                .Rule(number => (number % 2) == 0).WithMessage("Number must be even");

            Specification<int[]> specification = s => s
                .AsCollection(evenNumberSpecification);

            var validator = Validator.Factory.Create(specification);

            var numbers = new[] { 1, 2, 3, 4, 5 };

            validator.Validate(numbers).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "#0: Number must be even",
                "#2: Number must be even",
                "#4: Number must be even"
            );
        }

        private class NumberCollection : IEnumerable<int>, IEnumerable<double>
        {
            public IEnumerable<int> Ints { get; set; }

            public IEnumerable<double> Doubles { get; set; }

            IEnumerator<double> IEnumerable<double>.GetEnumerator() => Doubles.GetEnumerator();

            IEnumerator<int> IEnumerable<int>.GetEnumerator() => Ints.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<int>)this).GetEnumerator();
        }

        [Fact]
        public void AsCollection_CustomIEnumerable()
        {
            Specification<int> evenNumberSpecification = s => s
                .Rule(number => (number % 2) == 0).WithMessage("Number must be even");

            Specification<double> smallDecimalSpecification = s => s
                .Rule(number => Math.Floor(number) < 0.5).WithMessage("Decimal part must be below 0.5");

            Specification<NumberCollection> specification = s => s
                .Optional()
                .AsCollection<NumberCollection, int>(evenNumberSpecification)
                .AsCollection<NumberCollection, double>(smallDecimalSpecification);

            var validator = Validator.Factory.Create(specification);

            var numberCollection = new NumberCollection()
            {
                Ints = new[] { 1, 2, 3, 4, 5 },
                Doubles = new[] { 0.1, 2.8, 3.3, 4.6, 5.9 }
            };

            validator.Validate(numberCollection).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "#0: Number must be even",
                "#1: Decimal part must be below 0.5",
                "#2: Decimal part must be below 0.5",
                "#2: Number must be even",
                "#3: Decimal part must be below 0.5",
                "#4: Number must be even",
                "#4: Decimal part must be below 0.5"
            );
        }

        [Fact]
        public void AsCollection_NullItem()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Member(m => m.Email, m => m
                    .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!")
                );

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Authors, m => m.AsCollection(authorSpecification));

            var bookValidator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Authors = new[]
                {
                    null,
                    new AuthorModel() { Email = "foo@bar" },
                    new AuthorModel() { Email = null },
                    null,
                    new AuthorModel() { Email = "InvalidEmail" },
                    null,
                }
            };

            bookValidator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Authors.#0: Required",
                "Authors.#2.Email: Required",
                "Authors.#3: Required",
                "Authors.#4.Email: Must contain @ character!",
                "Authors.#5: Required"
            );
        }

        [Fact]
        public void AsCollection_NullItem_Optional()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Optional()
                .Member(m => m.Email, m => m
                    .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!")
                );

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Authors, m => m.AsCollection(authorSpecification));

            var bookValidator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Authors = new[]
                {
                    null,
                    new AuthorModel() { Email = "foo@bar" },
                    new AuthorModel() { Email = null },
                    null,
                    new AuthorModel() { Email = "InvalidEmail" },
                    null,
                }
            };

            bookValidator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Authors.#2.Email: Required",
                "Authors.#4.Email: Must contain @ character!"
            );
        }

        [Fact]
        public void AsCollection_CollectionAndItems()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Optional()
                .Member(m => m.Email, m => m
                    .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!")
                );

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Authors, m => m
                    .AsCollection(authorSpecification)
                    .Rule(authors => authors.Count() <= 5).WithMessage("Book can have max 5 authors.")
                );

            var bookValidator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Authors = new[]
                {
                    null,
                    new AuthorModel() { Email = "foo@bar" },
                    new AuthorModel() { Email = null },
                    null,
                    new AuthorModel() { Email = "InvalidEmail" },
                    null,
                }
            };

            bookValidator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Authors.#2.Email: Required",
                "Authors.#4.Email: Must contain @ character!",
                "Authors: Book can have max 5 authors."
            );
        }

        [Fact]
        public void AsNullable()
        {
            Specification<int> numberSpecification = s => s
                .Rule(number => number < 10).WithMessage("Number must be less than 10");

            Specification<int?> nullableSpecification = s => s
                .AsNullable(numberSpecification);

            var validator = Validator.Factory.Create(nullableSpecification);

            validator.Validate(5).AnyErrors.Should().BeFalse();

            validator.Validate(15).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Number must be less than 10"
            );

            validator.Validate(null).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required"
            );
        }

        [Fact]
        public void AsNullable_Optional()
        {
            Specification<int> numberSpecification = s => s
                .Rule(number => number < 10).WithMessage("Number must be less than 10");

            Specification<int?> nullableSpecification = s => s
                .Optional()
                .AsNullable(numberSpecification);

            var validator = Validator.Factory.Create(nullableSpecification);

            validator.Validate(5).AnyErrors.Should().BeFalse();

            validator.Validate(null).AnyErrors.Should().BeFalse();

            validator.Validate(15).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Number must be less than 10"
            );
        }

        [Fact]
        public void AsNullable_BuiltInRules()
        {
            Specification<int> numberSpecification = s => s.GreaterThan(0).LessThan(10);

            Specification<int?> nullableSpecification = s => s.GreaterThan(0).LessThan(10);

            var numberValidator = Validator.Factory.Create(numberSpecification);
            var nullableValidator = Validator.Factory.Create(nullableSpecification);

            numberValidator.Validate(5).AnyErrors.Should().BeFalse();
            nullableValidator.Validate(5).AnyErrors.Should().BeFalse();

            numberValidator.Validate(15).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must be less than 10"
            );

            nullableValidator.Validate(15).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must be less than 10"
            );
        }

        [Fact]
        public void AsNullable_ReuseSpecification()
        {
            Specification<int> yearSpecification = s => s
                .Rule(year => year >= -3000).WithMessage("Minimum year is 3000 B.C.")
                .Rule(year => year <= 3000).WithMessage("Maximum year is 3000 A.D.");

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.YearOfFirstAnnouncement, yearSpecification)
                .Member(m => m.YearOfPublication, m => m
                    .Optional()
                    .AsNullable(yearSpecification)
                );

            var bookValidator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                YearOfFirstAnnouncement = -4000,
                YearOfPublication = 4000
            };

            bookValidator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "YearOfFirstAnnouncement: Minimum year is 3000 B.C.",
                "YearOfPublication: Maximum year is 3000 A.D."
            );
        }

        [Fact]
        public void AsConverted_SameType()
        {
            Specification<string> nameSpecification = s => s
                .Rule(name => char.IsUpper(name.First())).WithMessage("Must start with a capital letter!")
                .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!");

            Converter<string, string> sanitizeName = firstName => firstName.Trim();

            Specification<string> nameValueSpecification = s => s
                .AsConverted(sanitizeName, nameSpecification);

            var nameValidator = Validator.Factory.Create(nameValueSpecification);

            nameValidator.Validate("Bartosz").AnyErrors.Should().BeFalse();

            nameValidator.Validate("      Bartosz    ").AnyErrors.Should().BeFalse();

            nameValidator.Validate("      bartosz    ").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must start with a capital letter!"
            );

            nameValidator.Validate("      Bart osz    ").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must not contain whitespace!"
            );
        }

        [Fact]
        public void AsConverted_DifferentTypeAndInline()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Member(m => m.Name, m => m.AsConverted(
                    name => name.Length,
                    nameLength => nameLength.Rule(l => l % 2 == 0).WithMessage("Characters amount must be even"))
                );

            var nameValidator = Validator.Factory.Create(authorSpecification);

            var author = new AuthorModel()
            {
                Name = "Bartosz"
            };

            nameValidator.Validate(author).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name: Characters amount must be even"
            );
        }

        [Fact]
        public void AsConverted_RequiredForValueTypeInTemplate()
        {
            Specification<int> specification1 = s => s
                .AsConverted(
                    value => value.ToString(CultureInfo.InvariantCulture),
                    c => c.MaxLength(10).WithMessage("Number must be max 5 digits length")
                );

            Validator.Factory.Create(specification1).Template.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required",
                "Number must be max 5 digits length"
            );

            Specification<int> specification2 = s => s
                .AsConverted(
                    value => value.ToString(CultureInfo.InvariantCulture),
                    c => c.Optional().MaxLength(10).WithMessage("Number must be max 5 digits length")
                );

            Validator.Factory.Create(specification2).Template.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Number must be max 5 digits length"
            );
        }

        private class Animal
        {
            public int AnimalId { get; set; }
        }

        private class Mammal : Animal
        {
            public int MammalId { get; set; }
        }

        private class Elephant : Mammal
        {
            public int ElephantId { get; set; }
        }

        [Fact]
        public void AsType_SpecificationOfParentInSpecificationOfChild()
        {
            Specification<int> idSpecification = s => s.NonZero();

            Specification<Animal> animalSpecification = s => s
                .Member(m => m.AnimalId, idSpecification);

            Specification<Elephant> elephantSpecification = s => s
                .Member(m => m.ElephantId, idSpecification)
                .AsType(animalSpecification);

            var elephantValidator = Validator.Factory.Create(elephantSpecification);

            elephantValidator.Validate(new Elephant() { ElephantId = 10, AnimalId = 10 }).AnyErrors.Should().BeFalse();

            elephantValidator.Validate(new Elephant() { ElephantId = 0, AnimalId = 10 }).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "ElephantId: Must not be zero"
            );

            elephantValidator.Validate(new Elephant() { ElephantId = 10, AnimalId = 0 }).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "AnimalId: Must not be zero"
            );
        }

        [Fact]
        public void AsType_SpecificationOfChildInSpecificationOfParent()
        {
            Specification<int> idSpecification = s => s.NonZero();

            Specification<Elephant> elephantSpecification = s => s
                .Member(m => m.ElephantId, idSpecification);

            Specification<Animal> animalSpecification = s => s
                .Member(m => m.AnimalId, idSpecification)
                .AsType(elephantSpecification);

            var animalValidator = Validator.Factory.Create(animalSpecification);

            animalValidator.Validate(new Elephant() { ElephantId = 10, AnimalId = 10 }).AnyErrors.Should().BeFalse();

            animalValidator.Validate(new Elephant() { ElephantId = 0, AnimalId = 10 }).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "ElephantId: Must not be zero"
            );

            animalValidator.Validate(new Elephant() { ElephantId = 10, AnimalId = 0 }).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "AnimalId: Must not be zero"
            );
        }

        [Fact]
        public void AsType_NonRelatedTypes()
        {
            Specification<object> specification = s => s
                .AsType(new Specification<int>(number => number.NonZero()))
                .AsType(new Specification<string>(text => text.NotEmpty()));

            var validator = Validator.Factory.Create(specification);

            validator.Validate(12).AnyErrors.Should().BeFalse();
            validator.Validate("test").AnyErrors.Should().BeFalse();
            validator.Validate(0L).AnyErrors.Should().BeFalse();

            validator.Validate(0).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must not be zero"
            );

            validator.Validate("").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must not be empty"
            );
        }

        [Fact]
        public void AsType_SummingAllLevels()
        {
            Specification<int> idSpecification = s => s.NonZero();

            Specification<Animal> animalSpecification = s => s
                .Member(m => m.AnimalId, idSpecification);

            Specification<Mammal> mammalSpecification = s => s
                .Member(m => m.MammalId, idSpecification)
                .And()
                .Member(m => m.AnimalId, idSpecification)
                .WithMessage("Something wrong with animal from mammal perspective")
                .And()
                .AsType(animalSpecification);

            Specification<Elephant> elephantSpecification = s => s
                .Member(m => m.ElephantId, idSpecification)
                .And()
                .Member(m => m.MammalId, idSpecification)
                .WithMessage("Something wrong with mammal from elephant perspective")
                .And()
                .Member(m => m.AnimalId, idSpecification)
                .WithMessage("Something wrong with animal from elephant perspective")
                .And()
                .AsType(mammalSpecification);

            var elephantValidator = Validator.Factory.Create(elephantSpecification);

            elephantValidator.Validate(new Elephant() { ElephantId = 10, MammalId = 10, AnimalId = 10 }).AnyErrors.Should().BeFalse();

            elephantValidator.Validate(new Elephant() { ElephantId = 0, MammalId = 10, AnimalId = 10 }).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "ElephantId: Must not be zero"
            );

            elephantValidator.Validate(new Elephant() { ElephantId = 10, MammalId = 0, AnimalId = 10 }).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "MammalId: Must not be zero",
                "MammalId: Something wrong with mammal from elephant perspective"
            );

            elephantValidator.Validate(new Elephant() { ElephantId = 0, MammalId = 0, AnimalId = 0 }).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "ElephantId: Must not be zero",
                "MammalId: Must not be zero",
                "MammalId: Something wrong with mammal from elephant perspective",
                "AnimalId: Must not be zero",
                "AnimalId: Something wrong with animal from mammal perspective",
                "AnimalId: Something wrong with animal from elephant perspective"
            );
        }

        [Fact]
        public void AsDictionary_Simple()
        {
            Specification<int> intValueSpecification = s => s
                .Rule(p => p % 2 == 0).WithMessage("Value must be even");

            Specification<Dictionary<string, int>> specification = s => s
                .AsDictionary(intValueSpecification);

            var validator = Validator.Factory.Create(specification);

            var dictionary = new Dictionary<string, int>()
            {
                ["One"] = 11,
                ["Two"] = 22,
                ["Three"] = 33,
                ["Four"] = 44,
                ["Five"] = 55
            };

            validator.Validate(dictionary).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "One: Value must be even",
                "Three: Value must be even",
                "Five: Value must be even"
            );
        }

        [Fact]
        public void AsDictionary_KeyStringifier()
        {
            Specification<Dictionary<string, int>> specification = s => s
                .AsDictionary(
                    s => s.Rule(p => p % 2 == 0).WithMessage("Value must be even"),
                    k => k.ToUpperInvariant()
                );

            var validator = Validator.Factory.Create(specification);

            var dictionary = new Dictionary<string, int>()
            {
                ["One"] = 11,
                ["Two"] = 22,
                ["Three"] = 33,
                ["Four"] = 44,
                ["Five"] = 55
            };

            validator.Validate(dictionary).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "ONE: Value must be even",
                "THREE: Value must be even",
                "FIVE: Value must be even"
            );
        }

        [Fact]
        public void AsDictionary_NullValues()
        {
            Specification<Dictionary<string, string>> specification = s => s
                .AsDictionary(s => s
                    .Rule(p => p.Length % 2 == 0).WithMessage("Value length must be even")
                );

            var validator = Validator.Factory.Create(specification);

            var dictionary = new Dictionary<string, string>()
            {
                ["One"] = "11",
                ["Two"] = "22222",
                ["Three"] = null,
                ["Four"] = null,
                ["Five"] = "55"
            };

            validator.Validate(dictionary).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Two: Value length must be even",
                "Three: Required",
                "Four: Required"
            );
        }

        [Fact]
        public void AsDictionary_NullValues_Optional()
        {
            Specification<Dictionary<string, string>> specification = s => s
                .AsDictionary(s => s
                    .Optional()
                    .Rule(p => p.Length % 2 == 0).WithMessage("Value length must be even")
                );

            var validator = Validator.Factory.Create(specification);

            var dictionary = new Dictionary<string, string>()
            {
                ["One"] = "11",
                ["Two"] = "22222",
                ["Three"] = null,
                ["Four"] = null,
                ["Five"] = "55"
            };

            validator.Validate(dictionary).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Two: Value length must be even"
            );
        }

        [Fact]
        public void AsDictionary_Template()
        {
            Specification<Dictionary<string, string>> specification = s => s
                .AsDictionary(s => s
                    .Rule(p => p.Length % 2 == 0).WithMessage("Value length must be even")
                );

            var validator = Validator.Factory.Create(specification);

            validator.Template.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required",
                "#: Required",
                "#: Value length must be even"
            );
        }

        [Fact]
        public void AsDictionary_KeyNormalization()
        {
            Specification<Dictionary<string, int>> specification = s => s
                .AsDictionary(
                    s => s.Rule(p => p % 2 == 0).WithMessage("Value must be even"),
                    k => k.ToLowerInvariant()
                );

            var validator = Validator.Factory.Create(specification);

            var dictionary = new Dictionary<string, int>()
            {
                ["OnE..."] = 11,
                ["ThR...eE"] = 33,
                ["<<<...FiVe..."] = 55,
                ["...SeVeN"] = 77,
                ["<<<NiNe"] = 99,
            };

            validator.Validate(dictionary).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "one: Value must be even",
                "thr.ee: Value must be even",
                "five: Value must be even",
                "seven: Value must be even",
                "nine: Value must be even"
            );
        }

        private class SimpleDictionary : IEnumerable<KeyValuePair<int, int>>
        {
            public SimpleDictionary(Dictionary<int, int> items)
            {
                Items = items;
            }

            private IEnumerable<KeyValuePair<int, int>> Items { get; }

            IEnumerator<KeyValuePair<int, int>> IEnumerable<KeyValuePair<int, int>>.GetEnumerator() => Items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<int, int>>)this).GetEnumerator();
        }

        [Fact]
        public void AsDictionary_CustomClass()
        {
            Specification<int> valueSpecification = s => s
                .Rule(p => p % 2 == 0).WithMessage("Value must be even");

            Func<int, string> keyStringifier = key =>
            {
                var keyString = "";

                for (var i = 0; i < key; i++)
                {
                    keyString += "X";
                }

                return keyString;
            };

            Specification<SimpleDictionary> specification = s => s
                .AsDictionary(valueSpecification, keyStringifier);

            var validator = Validator.Factory.Create(specification);

            var dictionary = new SimpleDictionary(new Dictionary<int, int>()
            {
                [1] = 11,
                [2] = 22,
                [3] = 33,
                [4] = 44,
                [5] = 55
            });

            validator.Validate(dictionary).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "X: Value must be even",
                "XXX: Value must be even",
                "XXXXX: Value must be even"
            );
        }

        private class DoubleDictionary : IEnumerable<KeyValuePair<int, int>>, IEnumerable<KeyValuePair<string, string>>
        {
            private readonly IEnumerable<KeyValuePair<int, int>> _ints;

            private readonly IEnumerable<KeyValuePair<string, string>> _strings;

            public DoubleDictionary(Dictionary<int, int> ints, Dictionary<string, string> strings)
            {
                _ints = ints;
                _strings = strings;
            }

            IEnumerator<KeyValuePair<int, int>> IEnumerable<KeyValuePair<int, int>>.GetEnumerator() => _ints.GetEnumerator();

            IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() => _strings.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        }

        [Fact]
        public void AsDictionary_CustomClassDoubleDictionary()
        {
            Specification<int> intSpecification = s => s
                .Rule(p => p % 2 == 0).WithMessage("Value must be even");

            Func<int, string> intKeyStringifier = key =>
            {
                var keyString = "";

                for (var i = 0; i < key; i++)
                {
                    keyString += "X";
                }

                return keyString;
            };

            Specification<string> stringSpecification = s => s
                .Rule(p => p.Length < 3).WithMessage("Value must be shorter than 3 characters");

            Func<string, string> stringKeyStringifier = key => key.ToUpperInvariant();

            Specification<DoubleDictionary> specification = s => s
                .AsDictionary(intSpecification, intKeyStringifier)
                .AsDictionary(stringSpecification, stringKeyStringifier);

            var validator = Validator.Factory.Create(specification);

            var dictionary = new DoubleDictionary(
                new Dictionary<int, int>()
                {
                    [1] = 11,
                    [2] = 22,
                    [3] = 33,
                    [4] = 44,
                    [5] = 55
                },
                new Dictionary<string, string>()
                {
                    ["One"] = "11",
                    ["Two"] = "222",
                    ["Three"] = "33",
                    ["Four"] = "444",
                    ["Five"] = "555"
                });

            validator.Validate(dictionary).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "X: Value must be even",
                "XXX: Value must be even",
                "XXXXX: Value must be even",
                "TWO: Value must be shorter than 3 characters",
                "FOUR: Value must be shorter than 3 characters",
                "FIVE: Value must be shorter than 3 characters"
            );
        }

        [Fact]
        public void WithCondition()
        {
            Predicate<string> isValidEmail = email => email.Substring(0, email.IndexOf('@')).All(char.IsLetterOrDigit);

            Specification<string> emailSpecification = s => s
                .Rule(isValidEmail)
                    .WithCondition(email => email.Contains('@'))
                    .WithMessage("Email username must contain only letters and digits.");

            var validator = Validator.Factory.Create(emailSpecification);

            validator.Validate("John.Doe-at-gmail.com").AnyErrors.Should().BeFalse();

            validator.Validate("John.Doe@gmail.com").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Email username must contain only letters and digits."
            );
        }

        [Fact]
        public void WithCondition_PreVerification()
        {
            Predicate<BookModel> isAuthorAPublisher = book =>
            {
                return book.Authors.Any(a => a.Name == book.Publisher.Name);
            };

            Specification<BookModel> bookSpecification = s => s
                .Rule(isAuthorAPublisher)
                    .WithCondition(book =>
                        book.IsSelfPublished &&
                        book.Authors?.Any() == true &&
                        book.Publisher?.Name != null
                    )
                    .WithMessage("Self-published book must have author as a publisher.");

            var validator = Validator.Factory.Create(bookSpecification);

            // 1: Condition is met, but the rule fails:
            var bookModel1 = new BookModel()
            {
                IsSelfPublished = true,
                Authors = new[] { new AuthorModel() { Name = "Bart" } },
                Publisher = new PublisherModel() { Name = "Adam" }
            };

            // 2: Condition is met, and the rule doesn't fail:
            var bookModel2 = new BookModel()
            {
                IsSelfPublished = true,
                Authors = new[] { new AuthorModel() { Name = "Bart" } },
                Publisher = new PublisherModel() { Name = "Bart" }
            };

            // 3: Condition is not met:
            var bookModel3 = new BookModel()
            {
                IsSelfPublished = false,
                Authors = new[] { new AuthorModel() { Name = "Bart" } },
                Publisher = null
            };

            validator.Validate(bookModel1).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Self-published book must have author as a publisher."
            );

            validator.Validate(bookModel2).AnyErrors.Should().BeFalse();

            validator.Validate(bookModel3).AnyErrors.Should().BeFalse();
        }

        [Fact]
        public void WithCondition_SelectiveSpecification()
        {
            Specification<string> gmailSpecification = s => s
                .Rule(email =>
                {
                    var username = email.Substring(0, email.Length - "@gmail.com".Length);

                    return !username.Contains('.');
                }).WithMessage("Gmail username must not contain dots.");

            Specification<string> outlookSpecification = s => s
                .Rule(email =>
                {
                    var username = email.Substring(0, email.Length - "@outlook.com".Length);

                    return username.All(char.IsLower);
                }).WithMessage("Outlook username must be all lower case.");

            Specification<string> emailSpecification = s => s
                .Rule(email => email.Contains('@')).WithMessage("Must contain @ character!");

            Predicate<AuthorModel> hasGmailAddress = a => a.Email?.EndsWith("@gmail.com") == true;

            Predicate<AuthorModel> hasOutlookAddress = a => a.Email?.EndsWith("@outlook.com") == true;

            Specification<AuthorModel> authorSpecification = s => s
                .Member(m => m.Email, gmailSpecification).WithCondition(hasGmailAddress)
                .Member(m => m.Email, outlookSpecification).WithCondition(hasOutlookAddress)
                .Member(m => m.Email, emailSpecification)
                    .WithCondition(author => !hasGmailAddress(author) && !hasOutlookAddress(author));

            var validator = Validator.Factory.Create(authorSpecification);

            var outlookAuthor = new AuthorModel() { Email = "John.Doe@outlook.com" };

            var gmailAuthor = new AuthorModel() { Email = "John.Doe@gmail.com" };

            var author1 = new AuthorModel() { Email = "JohnDoe" };

            var author2 = new AuthorModel() { Email = "John.Doe@yahoo.com" };

            validator.Validate(outlookAuthor).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Email: Outlook username must be all lower case.");

            validator.Validate(gmailAuthor).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Email: Gmail username must not contain dots.");

            validator.Validate(author1).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Email: Must contain @ character!");

            validator.Validate(author2).AnyErrors.Should().BeFalse();
        }

        [Fact]
        public void WithPath()
        {
            Specification<string> specification1 = s => s
            .Rule(email => email.Contains('@'))
            .WithMessage("Must contain @ character!");

            Specification<string> specification2 = s => s
                .Rule(email => email.Contains('@'))
                .WithPath("Characters")
                .WithMessage("Must contain @ character!");

            var validator1 = Validator.Factory.Create(specification1);
            var validator2 = Validator.Factory.Create(specification2);

            validator1.Validate("invalidemail").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character!"
            );

            validator2.Validate("invalidemail").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Characters: Must contain @ character!"
            );
        }

        [Fact]
        public void WithPath_GoingUp()
        {
            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Publisher, m => m
                    .Member(m1 => m1.Name, m1 => m1
                        .Rule(name => name.All(char.IsLetter))
                            .WithPath("<<NameOfPublisher")
                            .WithMessage("Must consist of letters only!")
                    )
                );

            var bookValidator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Publisher = new PublisherModel()
                {
                    Name = "Adam !!!"
                }
            };

            bookValidator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "NameOfPublisher: Must consist of letters only!"
            );
        }

        [Fact]
        public void WithPath_Restrictions()
        {
            Specification<string> specification = s => s
                .Rule(email => email.Contains('@'))
                .WithPath("Characters.")
                .WithMessage("Must contain @ character!");

            Action action = () =>
            {
                _ = Validator.Factory.Create(specification);
            };

            action.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void WithPath_Member()
        {
            Specification<string> nameSpecification = s => s
                .Rule(name => name.All(char.IsLetter)).WithMessage("Must consist of letters only!")
                .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Must not contain whitespace!");

            Specification<PublisherModel> publisherSpecification = s => s
                .Member(m => m.Name, nameSpecification).WithPath("FirstName");

            var publisherValidator = Validator.Factory.Create(publisherSpecification);

            var publisher = new PublisherModel()
            {
                Name = "Adam !!!"
            };

            publisherValidator.Validate(publisher).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "FirstName: Must consist of letters only!",
                "FirstName: Must not contain whitespace!"
            );
        }

        [Fact]
        public void WithPath_MergeErrorOutputs()
        {
            Specification<string> nameSpecification = s => s
                .Rule(name => name.All(char.IsLetter)).WithMessage("Name must consist of letters only!")
                .Rule(name => !name.Any(char.IsWhiteSpace)).WithMessage("Name must not contain whitespace!");

            Specification<string> companyIdSpecification = s => s
                .Rule(name => name.Any()).WithMessage("Company Id must not be empty!");

            Specification<PublisherModel> publisherSpecification = s => s
                .Member(m => m.Name, nameSpecification).WithPath("<Info")
                .Member(m => m.CompanyId, companyIdSpecification).WithPath("<Info");

            var publisherValidator = Validator.Factory.Create(publisherSpecification);

            var publisher = new PublisherModel()
            {
                Name = "Adam !!!",
                CompanyId = ""
            };

            publisherValidator.Validate(publisher).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Info: Name must consist of letters only!",
                "Info: Name must not contain whitespace!",
                "Info: Company Id must not be empty!"
            );
        }

        [Fact]
        public void WithPath_SplitErrorOutputs()
        {
            Specification<string> nameSpecification = s => s
                .Rule(name => name.All(char.IsLetter))
                .WithPath("Characters")
                .WithMessage("Must consist of letters only!")

                .Rule(name => char.IsUpper(name.First()))
                .WithPath("Grammar")
                .WithMessage("First letter must be capital!");

            Specification<PublisherModel> publisherSpecification = s => s
                .Member(m => m.Name, nameSpecification);

            var publisherValidator = Validator.Factory.Create(publisherSpecification);

            var publisher = new PublisherModel()
            {
                Name = "adam !!!",
            };

            publisherValidator.Validate(publisher).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Name.Characters: Must consist of letters only!",
                "Name.Grammar: First letter must be capital!"
            );
        }

        [Fact]
        public void WithMessage()
        {
            Specification<int> specification = s => s
                .Rule(year => year != 0);

            var validator = Validator.Factory.Create(specification);

            validator.Validate(0).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Error"
            );

            Specification<int> specificationWithMessage = s => s
                .Rule(year => year != 0)
                .WithMessage("Year 0 is invalid");

            var validatorWithMessage = Validator.Factory.Create(specificationWithMessage);

            validatorWithMessage.Validate(0).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Year 0 is invalid"
            );
        }

        [Fact]
        public void WithMessage_Args()
        {
            Specification<int> specification = s => s
                .Between(min: 10, max: 20)
                .WithMessage("Minimum value is {min}. Maximum value is {max}");

            var validator = Validator.Factory.Create(specification);

            validator.Validate(0).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Minimum value is 10. Maximum value is 20"
            );
        }

        [Fact]
        public void WithMessage_OverrideMultipleMessages()
        {
            Specification<AuthorModel> authorSpecification = s => s.Member(m => m.Email, m => m.Email());

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Authors, m => m
                    .AsCollection(authorSpecification).WithMessage("Contains author with invalid email")
                );

            var validator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Authors = new[]
                {
                    new AuthorModel() { Email = "InvalidEmail1" },
                    new AuthorModel() { Email = "InvalidEmail2" },
                    new AuthorModel() { Email = "john.doe@gmail.com" },
                    new AuthorModel() { Email = "InvalidEmail3" },
                }
            };

            validator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Authors: Contains author with invalid email"
            );
        }

        [Fact]
        public void WithExtraMessage()
        {
            Specification<int> specification = s => s
                .Rule(year => year != 0)
                .WithMessage("Year 0 is invalid")
                .WithExtraMessage("Year 0 didn't exist")
                .WithExtraMessage("Please change to 1 B.C. or 1 A.D.");

            var validator = Validator.Factory.Create(specification);

            validator.Validate(0).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Year 0 is invalid",
                "Year 0 didn't exist",
                "Please change to 1 B.C. or 1 A.D."
            );
        }

        [Fact]
        public void WithExtraMessage_AddToEntireErrorOutput()
        {
            Specification<AuthorModel> authorSpecification = s => s.Member(m => m.Email, m => m.Email());

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Authors, m => m
                    .AsCollection(authorSpecification).WithExtraMessage("Contains author with invalid email")
                );

            var validator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Authors = new[]
                {
                    new AuthorModel() { Email = "InvalidEmail1" },
                    new AuthorModel() { Email = "InvalidEmail2" },
                    new AuthorModel() { Email = "john.doe@gmail.com" },
                    new AuthorModel() { Email = "InvalidEmail3" },
                }
            };

            validator.Validate(book).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Authors.#0.Email: Must be a valid email address",
                "Authors.#1.Email: Must be a valid email address",
                "Authors.#3.Email: Must be a valid email address",
                "Authors: Contains author with invalid email"
            );
        }

        [Fact]
        public void WithExtraMessage_Args()
        {
            Specification<int> specification = s => s
                .Between(min: 10, max: 20)
                .WithExtraMessage("Minimum value is {min}. Maximum value is {max}.");

            var validator = Validator.Factory.Create(specification);

            validator.Validate(0).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must be between 10 and 20 (exclusive)",
                "Minimum value is 10. Maximum value is 20."
            );
        }

        [Fact]
        public void WithCode()
        {
            Specification<int> specification = s => s
                .Rule(year => year != 0)
                .WithCode("YEAR_ZERO");

            var validator = Validator.Factory.Create(specification);

            var result = validator.Validate(0);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Codes,
                "YEAR_ZERO"
            );
        }

        [Fact]
        public void WithCode_Results()
        {
            Specification<int[]> specification = s => s
                .AsCollection(m => m
                    .Rule(year => year % 2 == 0).WithCode("IS_EVEN")
                    .Rule(year => year % 2 != 0).WithCode("IS_ODD")
                );

            var validator = Validator.Factory.Create(specification);

            var result = validator.Validate(new[] { 0, 1, 2, 3, 4 });

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Codes,
                "IS_EVEN, IS_ODD"
            );

            result.Codes.Should().Contain("IS_EVEN", "IS_ODD");

            result.CodeMap["#0"].Should().ContainSingle("IS_EVEN");
            result.CodeMap["#1"].Should().ContainSingle("IS_ODD");
            result.CodeMap["#2"].Should().ContainSingle("IS_EVEN");
            result.CodeMap["#3"].Should().ContainSingle("IS_ODD");
            result.CodeMap["#4"].Should().ContainSingle("IS_EVEN");
        }

        [Fact]
        public void WithCode_OverrideEntireErrorOutput()
        {
            Specification<AuthorModel> authorSpecification = s => s.Member(m => m.Email, m => m.Email());

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Authors, m => m
                    .AsCollection(authorSpecification).WithCode("INVALID_AUTHORS")
                );

            var validator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Authors = new[]
                {
                    new AuthorModel() { Email = "InvalidEmail1" },
                    new AuthorModel() { Email = "InvalidEmail2" },
                    new AuthorModel() { Email = "john.doe@gmail.com" },
                    new AuthorModel() { Email = "InvalidEmail3" },
                }
            };

            var result = validator.Validate(book);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Codes,
                "INVALID_AUTHORS"
            );
        }

        [Fact]
        public void WithExtraCode()
        {
            Specification<int> specification = s => s
                .Rule(year => year != 0)
                .WithCode("YEAR_ZERO")
                .WithExtraCode("INVALID_YEAR");

            var validator = Validator.Factory.Create(specification);

            var result = validator.Validate(0);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Codes,
                "YEAR_ZERO, INVALID_YEAR"
            );
        }

        [Fact]
        public void WithExtraCode_Mix()
        {
            Specification<AuthorModel> authorSpecification = s => s.Member(m => m.Email, m => m.Email());

            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Authors, m => m
                    .AsCollection(authorSpecification).WithExtraCode("INVALID_AUTHORS")
                );

            var validator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel()
            {
                Authors = new[]
                {
                    new AuthorModel() { Email = "InvalidEmail1" },
                    new AuthorModel() { Email = "InvalidEmail2" },
                    new AuthorModel() { Email = "john.doe@gmail.com" },
                    new AuthorModel() { Email = "InvalidEmail3" },
                }
            };

            var result = validator.Validate(book);

            result.CodeMap["Authors"].Should().ContainSingle("INVALID_AUTHORS");

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.MessagesAndCodes,
                "INVALID_AUTHORS",
                "",
                "Authors.#0.Email: Must be a valid email address",
                "Authors.#1.Email: Must be a valid email address",
                "Authors.#3.Email: Must be a valid email address"
            );
        }

        [Fact]
        public void Optional()
        {
            Specification<string> specification1 = s => s
                .Optional()
                .Rule(title => title.Length > 3)
                .WithMessage("The minimum length is 3");

            var validator1 = Validator.Factory.Create(specification1);

            validator1.Validate(null).AnyErrors.Should().BeFalse();

            Specification<string> specification2 = s => s
                .Rule(title => title.Length > 3)
                .WithMessage("The minimum length is 3");

            var validator2 = Validator.Factory.Create(specification2);

            var result2 = validator2.Validate(null);

            result2.AnyErrors.Should().BeTrue();

            result2.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required");

            validator1.Validate("a").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The minimum length is 3"
            );

            validator2.Validate("a").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The minimum length is 3"
            );

            validator1.Validate("abc").AnyErrors.Should().BeTrue();

            validator2.Validate("abc").AnyErrors.Should().BeTrue();
        }

        [Fact]
        public void Optional_Member()
        {
            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Title, m => m
                    .Optional()
                    .Rule(title => title.Length > 3).WithMessage("The minimum length is 3")
                );

            var validator = Validator.Factory.Create(bookSpecification);

            var book1 = new BookModel() { Title = null };

            validator.Validate(book1).AnyErrors.Should().BeFalse();

            var book2 = new BookModel() { Title = "a" };

            validator.Validate(book2).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Title: The minimum length is 3"
            );
        }

        [Fact]
        public void Required()
        {
            Specification<string> specification1 = s => s
                .Required()
                .Rule(title => title.Length > 3)
                .WithMessage("The minimum length is 3");

            var validator1 = Validator.Factory.Create(specification1);

            var result1 = validator1.Validate(null);

            result1.AnyErrors.Should().BeTrue();

            result1.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required");

            Specification<string> specification2 = s => s
                .Rule(title => title.Length > 3)
                .WithMessage("The minimum length is 3");

            var validator2 = Validator.Factory.Create(specification2);

            var result2 = validator2.Validate(null);

            result2.AnyErrors.Should().BeTrue();

            result2.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Required");

            validator1.Validate("a").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The minimum length is 3"
            );

            validator2.Validate("a").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The minimum length is 3"
            );

            validator1.Validate("abc").AnyErrors.Should().BeTrue();

            validator2.Validate("abc").AnyErrors.Should().BeTrue();
        }

        [Fact]
        public void Required_WithMessageAndCodes()
        {
            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Title, m => m
                    .Required().WithMessage("Title is required").WithExtraCode("MISSING_TITLE")
                    .Rule(title => title.Length > 3).WithMessage("The minimum length is 3")
                );

            var validator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel() { Title = null };

            var result = validator.Validate(book);

            result.Codes.Should().ContainSingle("MISSING_TITLE");

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.MessagesAndCodes,
                "MISSING_TITLE",
                "",
                "Title: Title is required"
            );
        }

        [Fact]
        public void Forbidden()
        {
            Specification<string> specification = s => s
                .Forbidden();

            var validator = Validator.Factory.Create(specification);

            validator.Validate(null).AnyErrors.Should().BeFalse();

            validator.Validate("some value").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Forbidden");
        }

        [Fact]
        public void Forbidden_WithMessageAndCodes()
        {
            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Title, m => m
                    .Forbidden().WithMessage("Title will be autogenerated").WithExtraCode("TITLE_EXISTS")
                );

            var validator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel() { Title = "Aliens" };

            var result = validator.Validate(book);

            result.Codes.Should().ContainSingle("TITLE_EXISTS");

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.MessagesAndCodes,
                "TITLE_EXISTS",
                "",
                "Title: Title will be autogenerated"
            );
        }

        [Fact]
        public void Required_WithPathWorkaround()
        {
            Specification<BookModel> bookSpecification = s => s
                .Member(m => m.Title, m => m
                    .Optional()
                    .Rule(title => title.Length > 3).WithMessage("The minimum length is 3")
                )
                .Rule(m => m.Title != null)
                .WithPath("BookTitle")
                .WithMessage("Title is required")
                .WithExtraCode("MISSING_TITLE");

            var validator = Validator.Factory.Create(bookSpecification);

            var book = new BookModel() { Title = null };

            var result = validator.Validate(book);

            result.Codes.Should().ContainSingle("MISSING_TITLE");

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.MessagesAndCodes,
                "MISSING_TITLE",
                "",
                "BookTitle: Title is required"
            );
        }

        [Fact]
        public void And_WithPathWorkaround()
        {
            Specification<BookModel> bookSpecificationPlain = s => s
                .Member(m => m.Title, m => m
                    .Optional()
                    .Rule(title => title.Length > 5).WithMessage("The minimum length is 5")
                    .Rule(title => title.Length < 10).WithMessage("The maximum length is 10")
                )
                .Rule(m => !m.Title.Contains("title"))
                .WithPath("Title")
                .WithCode("TITLE_IN_TITLE")
                .Rule(m => m.YearOfFirstAnnouncement < 3000)
                .WithMessage("Maximum year value is 3000");

            Specification<BookModel> bookSpecificationAnd = s => s
                .Member(m => m.Title, m => m
                    .Optional()
                    .And()
                    .Rule(title => title.Length > 5).WithMessage("The minimum length is 5")
                    .And()
                    .Rule(title => title.Length < 10).WithMessage("The maximum length is 10")
                )
                .And()
                .Rule(m => !m.Title.Contains("title"))
                .WithPath("Title")
                .WithCode("TITLE_IN_TITLE")
                .And()
                .Rule(m => m.YearOfFirstAnnouncement < 3000)
                .WithMessage("Maximum year value is 3000");

            var book = new BookModel() { Title = "Super long title", YearOfFirstAnnouncement = 3001 };

            var resultPlain = Validator.Factory.Create(bookSpecificationPlain).Validate(book);

            resultPlain.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.MessagesAndCodes,
                "TITLE_IN_TITLE",
                "",
                "Title: The maximum length is 10",
                "Maximum year value is 3000"
            );

            var resultAnd = Validator.Factory.Create(bookSpecificationAnd).Validate(book);

            resultAnd.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.MessagesAndCodes,
                "TITLE_IN_TITLE",
                "",
                "Title: The maximum length is 10",
                "Maximum year value is 3000"
            );

            resultPlain.ToString().Should().Be(resultAnd.ToString());
        }

        public class VerySpecialException : Exception
        {
        }
    }
}
