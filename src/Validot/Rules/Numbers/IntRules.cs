namespace Validot
{
    using Validot.Specification;
    using Validot.Translations;

    public static class IntRules
    {
        public static IRuleOut<int> EqualTo(this IRuleIn<int> @this, int value)
        {
            return @this.RuleTemplate(m => m == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<int?> EqualTo(this IRuleIn<int?> @this, int value)
        {
            return @this.RuleTemplate(m => m.Value == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<int> NotEqualTo(this IRuleIn<int> @this, int value)
        {
            return @this.RuleTemplate(m => m != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<int?> NotEqualTo(this IRuleIn<int?> @this, int value)
        {
            return @this.RuleTemplate(m => m.Value != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<int> GreaterThan(this IRuleIn<int> @this, int min)
        {
            return @this.RuleTemplate(m => m > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<int?> GreaterThan(this IRuleIn<int?> @this, int min)
        {
            return @this.RuleTemplate(m => m.Value > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<int> GreaterThanOrEqualTo(this IRuleIn<int> @this, int min)
        {
            return @this.RuleTemplate(m => m >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<int?> GreaterThanOrEqualTo(this IRuleIn<int?> @this, int min)
        {
            return @this.RuleTemplate(m => m.Value >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<int> LessThan(this IRuleIn<int> @this, int max)
        {
            return @this.RuleTemplate(m => m < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<int?> LessThan(this IRuleIn<int?> @this, int max)
        {
            return @this.RuleTemplate(m => m.Value < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<int> LessThanOrEqualTo(this IRuleIn<int> @this, int max)
        {
            return @this.RuleTemplate(m => m <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<int?> LessThanOrEqualTo(this IRuleIn<int?> @this, int max)
        {
            return @this.RuleTemplate(m => m.Value <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<int> Between(this IRuleIn<int> @this, int min, int max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m > min && m < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<int?> Between(this IRuleIn<int?> @this, int min, int max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value > min && m.Value < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<int> BetweenOrEqualTo(this IRuleIn<int> @this, int min, int max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m >= min && m <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<int?> BetweenOrEqualTo(this IRuleIn<int?> @this, int min, int max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value >= min && m.Value <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<int> NonZero(this IRuleIn<int> @this)
        {
            return @this.RuleTemplate(m => m != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<int?> NonZero(this IRuleIn<int?> @this)
        {
            return @this.RuleTemplate(m => m.Value != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<int> Positive(this IRuleIn<int> @this)
        {
            return @this.RuleTemplate(m => m > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<int?> Positive(this IRuleIn<int?> @this)
        {
            return @this.RuleTemplate(m => m.Value > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<int> NonPositive(this IRuleIn<int> @this)
        {
            return @this.RuleTemplate(m => m <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<int?> NonPositive(this IRuleIn<int?> @this)
        {
            return @this.RuleTemplate(m => m.Value <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<int> Negative(this IRuleIn<int> @this)
        {
            return @this.RuleTemplate(m => m < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<int?> Negative(this IRuleIn<int?> @this)
        {
            return @this.RuleTemplate(m => m.Value < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<int> NonNegative(this IRuleIn<int> @this)
        {
            return @this.RuleTemplate(m => m >= 0, MessageKey.Numbers.NonNegative);
        }

        public static IRuleOut<int?> NonNegative(this IRuleIn<int?> @this)
        {
            return @this.RuleTemplate(m => m.Value >= 0, MessageKey.Numbers.NonNegative);
        }
    }
}
