namespace Validot.Tests.Functional.Documentation
{
    using Validot.Specification;

    using Xunit;

    namespace Step1
    {
        using FluentAssertions;

        using Validot.Testing;

        public static class MyCustomValidotRules
        {
            public static IRuleOut<string> HasCharacter(this IRuleIn<string> @this)
            {
                return @this.RuleTemplate(
                    value => value.Length > 0,
                    "Must have at least one character!"
                );
            }
        }

        public class CustomRulesFuncTests
        {
            [Fact]
            public void CustomRule()
            {
                Specification<string> specification = s => s
                    .HasCharacter();

                var validator = Validator.Factory.Create(specification);

                validator.Validate("test").AnyErrors.Should().BeFalse();

                validator.Validate("").ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Must have at least one character!");
            }
        }
    }

    namespace Step2
    {
        using System.Linq;

        using FluentAssertions;

        using Validot.Testing;

        public static class MyCustomValidotRules
        {
            public static IRuleOut<string> HasCharacter(
                this IRuleIn<string> @this,
                char character,
                int count = 1)
            {
                return @this.RuleTemplate(
                    value => value.Count(c => c == character) == count,
                    "Must have character '{character}' in the amount of {count}",
                    Arg.Text(nameof(character), character),
                    Arg.Number(nameof(count), count)
                );
            }
        }

        public class CustomRulesFuncTests
        {
            [Fact]
            public void CustomRule()
            {
                Specification<string> specification = s => s
                    .HasCharacter('t', 2);

                var validator = Validator.Factory.Create(specification);

                validator.Validate("test").AnyErrors.Should().BeFalse();

                validator.Validate("").ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Must have character 't' in the amount of 2");
            }
        }
    }

    namespace Step3
    {
        using System.Linq;

        using FluentAssertions;

        using Validot.Testing;

        public static class MyCustomValidotRules
        {
            public static IRuleOut<string> HasCharacter(
                this IRuleIn<string> @this,
                char character,
                int count = 1)
            {
                return @this.RuleTemplate(
                    value => value.Count(c => c == character) == count,
                    "Text.HasCharacter",
                    Arg.Text(nameof(character), character),
                    Arg.Number(nameof(count), count)
                );
            }
        }

        public class CustomRulesFuncTests
        {
            [Fact]
            public void CustomRule()
            {
                Specification<string> specification = s => s
                    .HasCharacter('t', 2);

                var validator = Validator.Factory.Create(specification, settings => settings
                    .WithTranslation("English", "Text.HasCharacter", "Must have character '{character}' in the amount of {count}")
                    .WithTranslation("Polish", "Text.HasCharacter", "Musi zawierać znak '{character}' w ilości {count|culture=pl-PL}")
                );

                validator.Validate("test").AnyErrors.Should().BeFalse();

                var result = validator.Validate("");

                result.ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Must have character 't' in the amount of 2");

                result.ToString(translationName: "Polish").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Musi zawierać znak 't' w ilości 2");
            }
        }
    }
}
