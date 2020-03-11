namespace Validot
{
    using System;
    using System.Globalization;

    using Validot.Specification;
    using Validot.Translations;

    public static class CharRules
    {
        public static IRuleOut<char> EqualToIgnoreCase(this IRuleIn<char> @this, char value)
        {
            return @this.RuleTemplate(v => string.Compare(v.ToString(CultureInfo.InvariantCulture).ToUpperInvariant(), value.ToString(CultureInfo.InvariantCulture).ToUpperInvariant(), StringComparison.Ordinal) == 0, MessageKey.CharType.EqualToIgnoreCase, Arg.Text(nameof(value), value));
        }

        public static IRuleOut<char?> EqualToIgnoreCase(this IRuleIn<char?> @this, char value)
        {
            return @this.RuleTemplate(v => string.Compare(v.Value.ToString(CultureInfo.InvariantCulture).ToUpperInvariant(), value.ToString(CultureInfo.InvariantCulture).ToUpperInvariant(), StringComparison.Ordinal) == 0, MessageKey.CharType.EqualToIgnoreCase, Arg.Text(nameof(value), value));
        }

        public static IRuleOut<char> NotEqualToIgnoreCase(this IRuleIn<char> @this, char value)
        {
            return @this.RuleTemplate(v => string.Compare(v.ToString(CultureInfo.InvariantCulture).ToUpperInvariant(), value.ToString(CultureInfo.InvariantCulture).ToUpperInvariant(), StringComparison.Ordinal) != 0, MessageKey.CharType.NotEqualToIgnoreCase, Arg.Text(nameof(value), value));
        }

        public static IRuleOut<char?> NotEqualToIgnoreCase(this IRuleIn<char?> @this, char value)
        {
            return @this.RuleTemplate(v => string.Compare(v.Value.ToString(CultureInfo.InvariantCulture).ToUpperInvariant(), value.ToString(CultureInfo.InvariantCulture).ToUpperInvariant(), StringComparison.Ordinal) != 0, MessageKey.CharType.NotEqualToIgnoreCase, Arg.Text(nameof(value), value));
        }
    }
}
