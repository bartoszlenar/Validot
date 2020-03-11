namespace Validot
{
    using System;
    using System.Text.RegularExpressions;

    using Validot.Specification;
    using Validot.Translations;

    public static class StringRules
    {
        public static IRuleOut<string> EqualTo(this IRuleIn<string> @this, string value, StringComparison stringComparison = StringComparison.Ordinal)
        {
            ThrowHelper.NullArgument(value, nameof(value));

            return @this.RuleTemplate(v => string.Equals(v, value, stringComparison), MessageKey.Texts.EqualTo, Arg.Text(nameof(value), value), Arg.Enum(nameof(stringComparison), stringComparison));
        }

        public static IRuleOut<string> NotEqualTo(this IRuleIn<string> @this, string value, StringComparison stringComparison = StringComparison.Ordinal)
        {
            ThrowHelper.NullArgument(value, nameof(value));

            return @this.RuleTemplate(v => !string.Equals(v, value, stringComparison), MessageKey.Texts.NotEqualTo, Arg.Text(nameof(value), value), Arg.Enum(nameof(stringComparison), stringComparison));
        }

        public static IRuleOut<string> Contains(this IRuleIn<string> @this, string value, StringComparison stringComparison = StringComparison.Ordinal)
        {
            ThrowHelper.NullArgument(value, nameof(value));

            return @this.RuleTemplate(v => v.IndexOf(value, stringComparison) >= 0, MessageKey.Texts.Contains, Arg.Text(nameof(value), value), Arg.Enum(nameof(stringComparison), stringComparison));
        }

        public static IRuleOut<string> NotContains(this IRuleIn<string> @this, string value, StringComparison stringComparison = StringComparison.Ordinal)
        {
            ThrowHelper.NullArgument(value, nameof(value));

            return @this.RuleTemplate(v => v.IndexOf(value, stringComparison) < 0, MessageKey.Texts.NotContains, Arg.Text(nameof(value), value), Arg.Enum(nameof(stringComparison), stringComparison));
        }

        public static IRuleOut<string> NotEmpty(this IRuleIn<string> @this)
        {
            return @this.RuleTemplate(v => !string.IsNullOrEmpty(v), MessageKey.Texts.NotEmpty);
        }

        public static IRuleOut<string> NotWhiteSpace(this IRuleIn<string> @this)
        {
            return @this.RuleTemplate(v => !string.IsNullOrWhiteSpace(v), MessageKey.Texts.NotWhiteSpace);
        }

        public static IRuleOut<string> SingleLine(this IRuleIn<string> @this)
        {
            return @this.RuleTemplate(v => !v.Contains(Environment.NewLine), MessageKey.Texts.SingleLine);
        }

        public static IRuleOut<string> ExactLength(this IRuleIn<string> @this, int length)
        {
            ThrowHelper.BelowZero(length, nameof(length));

            return @this.RuleTemplate(v => v.Replace(Environment.NewLine, " ").Length == length, MessageKey.Texts.ExactLength, Arg.Number(nameof(length), length));
        }

        public static IRuleOut<string> MaxLength(this IRuleIn<string> @this, int max)
        {
            ThrowHelper.BelowZero(max, nameof(max));

            return @this.RuleTemplate(v => v.Replace(Environment.NewLine, " ").Length <= max, MessageKey.Texts.MaxLength, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<string> MinLength(this IRuleIn<string> @this, int min)
        {
            ThrowHelper.BelowZero(min, nameof(min));

            return @this.RuleTemplate(v => v.Replace(Environment.NewLine, " ").Length >= min, MessageKey.Texts.MinLength, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<string> LengthBetween(this IRuleIn<string> @this, int min, int max)
        {
            ThrowHelper.BelowZero(min, nameof(min));
            ThrowHelper.BelowZero(max, nameof(max));
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(
                v =>
                {
                    var squashedLength = v.Replace(Environment.NewLine, " ").Length;

                    return squashedLength >= min && squashedLength <= max;
                },
                MessageKey.Texts.LengthBetween,
                Arg.Number(nameof(min), min),
                Arg.Number(nameof(max), max));
        }

        public static IRuleOut<string> Matches(this IRuleIn<string> @this, string pattern)
        {
            ThrowHelper.NullArgument(pattern, nameof(pattern));

            return @this.RuleTemplate(v => Regex.IsMatch(v, pattern, RegexOptions.CultureInvariant), MessageKey.Texts.Matches, Arg.Text(nameof(pattern), pattern));
        }

        public static IRuleOut<string> Matches(this IRuleIn<string> @this, Regex pattern)
        {
            ThrowHelper.NullArgument(pattern, nameof(pattern));

            return @this.RuleTemplate(pattern.IsMatch, MessageKey.Texts.Matches, Arg.Text(nameof(pattern), pattern.ToString()));
        }

        public static IRuleOut<string> StartsWith(this IRuleIn<string> @this, string value, StringComparison stringComparison = StringComparison.Ordinal)
        {
            ThrowHelper.NullArgument(value, nameof(value));

            return @this.RuleTemplate(v => v.StartsWith(value, stringComparison), MessageKey.Texts.StartsWith, Arg.Text(nameof(value), value), Arg.Enum(nameof(stringComparison), stringComparison));
        }

        public static IRuleOut<string> EndsWith(this IRuleIn<string> @this, string value, StringComparison stringComparison = StringComparison.Ordinal)
        {
            ThrowHelper.NullArgument(value, nameof(value));

            return @this.RuleTemplate(v => v.EndsWith(value, stringComparison), MessageKey.Texts.EndsWith, Arg.Text(nameof(value), value), Arg.Enum(nameof(stringComparison), stringComparison));
        }
    }
}
