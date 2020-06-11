namespace Validot.Tests.Functional.Documentation
{
    using System;

    using Validot.Rules;
    using Validot.Testing;

    using Xunit;

    public class MessageArgumentsFuncTests
    {
        [Fact]
        public void MessageArguments()
        {
            Specification<decimal> specification = s => s
                .Between(min: 0.123M, max: 100.123M)
                .WithMessage("The number needs to fit between {min} and {max}");

            var validator = Validator.Factory.Create(specification);

            validator.Validate(105).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The number needs to fit between 0.123 and 100.123");
        }

        [Fact]
        public void MessageArguments_Arguments()
        {
            Specification<decimal> specification = s => s
                .Between(min: 0.123M, max: 100.123M)
                .WithMessage("The maximum value is {max|format=000.000}")
                .WithExtraMessage("The minimum value is {min|format=000.000|culture=pl-PL}");

            var validator = Validator.Factory.Create(specification);

            validator.Validate(105).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The maximum value is 100.123",
                "The minimum value is 000,123");
        }

        [Fact]
        public void EnumArgument()
        {
            Specification<string> gmailSpecification = s => s
                .EndsWith("@gmail.com", stringComparison: StringComparison.OrdinalIgnoreCase)
                .WithMessage("Must ends with @gmail.com {stringComparison|translation=true}");

            var validator = Validator.Factory.Create(gmailSpecification, settings => settings
                .WithTranslation("English", "Enum.System.StringComparison.OrdinalIgnoreCase", "(ignoring case!)")
            );

            validator.Validate("john.doe@outlook.com").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must ends with @gmail.com (ignoring case!)");
        }

        [Fact]
        public void GuidArgument()
        {
            Specification<Guid> specification = s => s
                .NotEqualTo(new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa"))
                .WithMessage("Must not be equal to: {value|format=X|case=upper}");

            var validator = Validator.Factory.Create(specification);

            validator.Validate(new Guid("c2ce1f3b-17e5-412e-923b-6b4e268f31aa")).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must not be equal to: {0XC2CE1F3B,0X17E5,0X412E,{0X92,0X3B,0X6B,0X4E,0X26,0X8F,0X31,0XAA}}");
        }

        [Fact]
        public void NumberArgument()
        {
            Specification<decimal> specification = s => s
                .EqualTo(666.666M)
                .WithMessage("Needs to be equal to {value|format=0.0|culture=pl-PL}");

            var validator = Validator.Factory.Create(specification);

            validator.Validate(10).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Needs to be equal to 666,7");
        }

        [Fact]
        public void TextArgument()
        {
            Specification<string> gmailSpecification = s => s
                .EndsWith("@gmail.com")
                .WithMessage("Must ends with {value|case=upper}");

            var validator = Validator.Factory.Create(gmailSpecification);

            validator.Validate("john.doe@outlook.com").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must ends with @GMAIL.COM");
        }

        [Fact]
        public void TimeArgument()
        {
            Specification<DateTime> specification = s => s
                .Before(new DateTime(2000, 1, 2, 3, 4, 5, 6))
                .WithMessage("Must not be before: {max|format=yyyy MM dd + HH:mm}");

            var validator = Validator.Factory.Create(specification);

            validator.Validate(new DateTime(2001, 1, 1, 1, 1, 1, 1)).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must not be before: 2000 01 02 + 03:04");
        }

        [Fact]
        public void TranslationArgument()
        {
            Specification<int> specification = s => s
                .NotEqualTo(666)
                .WithMessage("!!! {_translation|key=TripleSix} !!!");

            var validator = Validator.Factory.Create(specification, settings => settings
                .WithTranslation("English", "TripleSix", "six six six")
                .WithTranslation("Polish", "TripleSix", "sześć sześć sześć")
            );

            var result = validator.Validate(666);

            result.ToString(translationName: "English").ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "!!! six six six !!!");

            result.ToString(translationName: "Polish").ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "!!! sześć sześć sześć !!!");
        }

        [Fact]
        public void PathArgument()
        {
            Specification<decimal> specification = s => s
                .Positive()
                .WithPath("Number.Value")
                .WithMessage("Number value under {_path} needs to be positive!");

            var validator = Validator.Factory.Create(specification);

            var result = validator.Validate(-1);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Number.Value: Number value under Number.Value needs to be positive!");
        }

        [Fact]
        public void PathArgument_Root()
        {
            Specification<decimal> specification = s => s
                .Positive()
                .WithMessage("Number value under {_path} needs to be positive!");

            var validator = Validator.Factory.Create(specification);

            var result = validator.Validate(-1);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Number value under  needs to be positive!");
        }

        [Fact]
        public void NameArgument()
        {
            Specification<decimal> specification = s => s
                .Positive()
                .WithPath("Number.Primary.SuperValue")
                .WithMessage("The {_name} needs to be positive!");

            var validator = Validator.Factory.Create(specification);

            var result = validator.Validate(-1);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Number.Primary.SuperValue: The SuperValue needs to be positive!");
        }

        [Fact]
        public void NameArgument_Root()
        {
            Specification<decimal> specification = s => s
                .Positive()
                .WithMessage("The {_name} needs to be positive!");

            var validator = Validator.Factory.Create(specification);

            var result = validator.Validate(-1);

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "The  needs to be positive!");
        }
    }
}
