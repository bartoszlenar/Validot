namespace Validot
{
    using Validot.Specification;
    using Validot.Translations;

    public static class DecimalRules
    {
        public static IRuleOut<decimal> EqualTo(this IRuleIn<decimal> @this, decimal value)
        {
            return @this.RuleTemplate(m => m == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<decimal?> EqualTo(this IRuleIn<decimal?> @this, decimal value)
        {
            return @this.RuleTemplate(m => m.Value == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<decimal> NotEqualTo(this IRuleIn<decimal> @this, decimal value)
        {
            return @this.RuleTemplate(m => m != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<decimal?> NotEqualTo(this IRuleIn<decimal?> @this, decimal value)
        {
            return @this.RuleTemplate(m => m.Value != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<decimal> GreaterThan(this IRuleIn<decimal> @this, decimal min)
        {
            return @this.RuleTemplate(m => m > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<decimal?> GreaterThan(this IRuleIn<decimal?> @this, decimal min)
        {
            return @this.RuleTemplate(m => m.Value > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<decimal> GreaterThanOrEqualTo(this IRuleIn<decimal> @this, decimal min)
        {
            return @this.RuleTemplate(m => m >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<decimal?> GreaterThanOrEqualTo(this IRuleIn<decimal?> @this, decimal min)
        {
            return @this.RuleTemplate(m => m.Value >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<decimal> LessThan(this IRuleIn<decimal> @this, decimal max)
        {
            return @this.RuleTemplate(m => m < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<decimal?> LessThan(this IRuleIn<decimal?> @this, decimal max)
        {
            return @this.RuleTemplate(m => m.Value < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<decimal> LessThanOrEqualTo(this IRuleIn<decimal> @this, decimal max)
        {
            return @this.RuleTemplate(m => m <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<decimal?> LessThanOrEqualTo(this IRuleIn<decimal?> @this, decimal max)
        {
            return @this.RuleTemplate(m => m.Value <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<decimal> Between(this IRuleIn<decimal> @this, decimal min, decimal max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m > min && m < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<decimal?> Between(this IRuleIn<decimal?> @this, decimal min, decimal max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value > min && m.Value < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<decimal> BetweenOrEqualTo(this IRuleIn<decimal> @this, decimal min, decimal max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m >= min && m <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<decimal?> BetweenOrEqualTo(this IRuleIn<decimal?> @this, decimal min, decimal max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value >= min && m.Value <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<decimal> NonZero(this IRuleIn<decimal> @this)
        {
            return @this.RuleTemplate(m => m != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<decimal?> NonZero(this IRuleIn<decimal?> @this)
        {
            return @this.RuleTemplate(m => m.Value != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<decimal> Positive(this IRuleIn<decimal> @this)
        {
            return @this.RuleTemplate(m => m > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<decimal?> Positive(this IRuleIn<decimal?> @this)
        {
            return @this.RuleTemplate(m => m.Value > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<decimal> NonPositive(this IRuleIn<decimal> @this)
        {
            return @this.RuleTemplate(m => m <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<decimal?> NonPositive(this IRuleIn<decimal?> @this)
        {
            return @this.RuleTemplate(m => m.Value <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<decimal> Negative(this IRuleIn<decimal> @this)
        {
            return @this.RuleTemplate(m => m < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<decimal?> Negative(this IRuleIn<decimal?> @this)
        {
            return @this.RuleTemplate(m => m.Value < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<decimal> NonNegative(this IRuleIn<decimal> @this)
        {
            return @this.RuleTemplate(m => m >= 0, MessageKey.Numbers.NonNegative);
        }

        public static IRuleOut<decimal?> NonNegative(this IRuleIn<decimal?> @this)
        {
            return @this.RuleTemplate(m => m.Value >= 0, MessageKey.Numbers.NonNegative);
        }
    }
}
