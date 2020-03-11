namespace Validot
{
    using Validot.Specification;
    using Validot.Translations;

    public static class LongRules
    {
        public static IRuleOut<long> EqualTo(this IRuleIn<long> @this, long value)
        {
            return @this.RuleTemplate(m => m == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<long?> EqualTo(this IRuleIn<long?> @this, long value)
        {
            return @this.RuleTemplate(m => m.Value == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<long> NotEqualTo(this IRuleIn<long> @this, long value)
        {
            return @this.RuleTemplate(m => m != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<long?> NotEqualTo(this IRuleIn<long?> @this, long value)
        {
            return @this.RuleTemplate(m => m.Value != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<long> GreaterThan(this IRuleIn<long> @this, long min)
        {
            return @this.RuleTemplate(m => m > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<long?> GreaterThan(this IRuleIn<long?> @this, long min)
        {
            return @this.RuleTemplate(m => m.Value > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<long> GreaterThanOrEqualTo(this IRuleIn<long> @this, long min)
        {
            return @this.RuleTemplate(m => m >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<long?> GreaterThanOrEqualTo(this IRuleIn<long?> @this, long min)
        {
            return @this.RuleTemplate(m => m.Value >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<long> LessThan(this IRuleIn<long> @this, long max)
        {
            return @this.RuleTemplate(m => m < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<long?> LessThan(this IRuleIn<long?> @this, long max)
        {
            return @this.RuleTemplate(m => m.Value < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<long> LessThanOrEqualTo(this IRuleIn<long> @this, long max)
        {
            return @this.RuleTemplate(m => m <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<long?> LessThanOrEqualTo(this IRuleIn<long?> @this, long max)
        {
            return @this.RuleTemplate(m => m.Value <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<long> Between(this IRuleIn<long> @this, long min, long max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m > min && m < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<long?> Between(this IRuleIn<long?> @this, long min, long max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value > min && m.Value < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<long> BetweenOrEqualTo(this IRuleIn<long> @this, long min, long max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m >= min && m <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<long?> BetweenOrEqualTo(this IRuleIn<long?> @this, long min, long max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value >= min && m.Value <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<long> NonZero(this IRuleIn<long> @this)
        {
            return @this.RuleTemplate(m => m != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<long?> NonZero(this IRuleIn<long?> @this)
        {
            return @this.RuleTemplate(m => m.Value != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<long> Positive(this IRuleIn<long> @this)
        {
            return @this.RuleTemplate(m => m > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<long?> Positive(this IRuleIn<long?> @this)
        {
            return @this.RuleTemplate(m => m.Value > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<long> NonPositive(this IRuleIn<long> @this)
        {
            return @this.RuleTemplate(m => m <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<long?> NonPositive(this IRuleIn<long?> @this)
        {
            return @this.RuleTemplate(m => m.Value <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<long> Negative(this IRuleIn<long> @this)
        {
            return @this.RuleTemplate(m => m < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<long?> Negative(this IRuleIn<long?> @this)
        {
            return @this.RuleTemplate(m => m.Value < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<long> NonNegative(this IRuleIn<long> @this)
        {
            return @this.RuleTemplate(m => m >= 0, MessageKey.Numbers.NonNegative);
        }

        public static IRuleOut<long?> NonNegative(this IRuleIn<long?> @this)
        {
            return @this.RuleTemplate(m => m.Value >= 0, MessageKey.Numbers.NonNegative);
        }
    }
}
