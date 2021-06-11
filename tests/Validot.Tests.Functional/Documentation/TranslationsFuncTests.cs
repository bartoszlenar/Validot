namespace Validot.Tests.Functional.Documentation
{
    using Validot.Testing;

    using Xunit;

    public class TranslationsFuncTests
    {
        [Fact]
        public void Translation_PrintingMessageKey()
        {
            Specification<string> specification = s => s
                .Rule(m => m.Contains("@")).WithMessage("Must contain @ character");

            var validator = Validator.Factory.Create(specification);

            validator.Validate("").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain @ character");
        }

        [Fact]
        public void Translation_DeliveringTranslation()
        {
            Specification<string> specification = s => s
                .Rule(m => m.Contains("@")).WithMessage("Must contain @ character");

            var validator = Validator.Factory.Create(specification, settings => settings
                .WithTranslation("Polish", "Must contain @ character", "Musi zawierać znak: @")
                .WithTranslation("English", "Must contain @ character", "Must contain character: @")
            );

            var result = validator.Validate("");

            result.ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Must contain character: @");

            result.ToString("Polish").ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Musi zawierać znak: @");
        }

        [Fact]
        public void OverridingMessages()
        {
            Specification<string> specification = s => s
                .NotEmpty();

            var validator = Validator.Factory.Create(specification, settings => settings
                .WithTranslation("English", "Global.Required", "String cannot be null!")
                .WithTranslation("English", "Texts.NotEmpty", "String cannot be empty!")
            );

            validator.Validate(null).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "String cannot be null!"
            );

            validator.Validate("").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "String cannot be empty!"
            );
        }

        [Fact]
        public void OverridingMessages_Arguments()
        {
            Specification<decimal> specification = s => s
                .BetweenOrEqualTo(16.66M, 666.666M);

            var validator = Validator.Factory.Create(specification, settings => settings
                .WithTranslation(
                    "English",
                    "Numbers.BetweenOrEqualTo",
                    "Only numbers between {min|format=000.0000} and {max|format=000.0000} are valid!")
            );

            validator.Validate(10).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Only numbers between 016.6600 and 666.6660 are valid!"
            );
        }

        public class BuiltInTranslationsTests
        {
            [Fact]
            public void WithPolishTranslation()
            {
                Specification<string> specification = s => s
                    .NotEmpty()
                    .MaxLength(5);

                var validator = Validator.Factory.Create(specification, settings => settings
                    .WithPolishTranslation()
                );

                validator.Validate(null).ToString(translationName: "Polish").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Wymagane"
                );

                validator.Validate("").ToString(translationName: "Polish").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Musi nie być puste"
                );

                validator.Validate("1234567890").ToString(translationName: "Polish").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Musi być długości maksymalnie 5 znaków"
                );
            }

            [Fact]
            public void WithSpanishTranslation()
            {
                Specification<string> specification = s => s
                    .NotEmpty()
                    .MaxLength(5);

                var validator = Validator.Factory.Create(specification, settings => settings
                    .WithSpanishTranslation()
                );

                validator.Validate(null).ToString(translationName: "Spanish").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Requerido"
                );

                validator.Validate("").ToString(translationName: "Spanish").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "No debe estar vacío"
                );

                validator.Validate("1234567890").ToString(translationName: "Spanish").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Debe tener como máximo 5 caracteres"
                );
            }

            [Fact]
            public void WithRussianTranslation()
            {
                Specification<string> specification = s => s
                    .NotEmpty()
                    .MaxLength(5);

                var validator = Validator.Factory.Create(specification, settings => settings
                    .WithRussianTranslation()
                );

                validator.Validate(null).ToString(translationName: "Russian").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Требуется"
                );

                validator.Validate("").ToString(translationName: "Russian").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Не должен быть пуст"
                );

                validator.Validate("1234567890").ToString(translationName: "Russian").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Должен быть не больше 5 символов в длину"
                );
            }

            [Fact]
            public void WithPortugueseTranslation()
            {
                Specification<string> specification = s => s
                    .NotEmpty()
                    .MaxLength(5);

                var validator = Validator.Factory.Create(specification, settings => settings
                    .WithPortugueseTranslation()
                );

                validator.Validate(null).ToString(translationName: "Portuguese").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Obrigatório"
                );

                validator.Validate("").ToString(translationName: "Portuguese").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Não deve estar vazio"
                );

                validator.Validate("1234567890").ToString(translationName: "Portuguese").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Deve ter no máximo 5 caracteres"
                );
            }
        }
    }

    namespace CustomTranslation
    {
        using System.Collections.Generic;

        using Validot.Settings;

        public static class WithYodaEnglishExtension
        {
            public static ValidatorSettings WithYodaEnglish(this ValidatorSettings @this)
            {
                var dictionary = new Dictionary<string, string>()
                {
                    ["Global.Required"] = "Exist, it must.",
                    ["Numbers.LessThan"] = "Greater than {max}, the number must, be not."
                };

                return @this.WithTranslation("YodaEnglish", dictionary);
            }
        }

        public class CustomTranslationsFuncTests
        {
            [Fact]
            public void CustomTranslation_Extension()
            {
                Specification<int?> specification = s => s
                    .LessThan(10);

                var validator = Validator.Factory.Create(specification, settings => settings
                    .WithYodaEnglish()
                );

                validator.Validate(null).ToString("YodaEnglish").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Exist, it must."
                );

                validator.Validate(20).ToString("YodaEnglish").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Greater than 10, the number must, be not."
                );
            }
        }
    }
}