namespace Validot
{
    using Validot.Specification;
    using Validot.Translations;

    public static class SByteRules
    {
        public static IRuleOut<sbyte> EqualTo(this IRuleIn<sbyte> @this, sbyte value)
        {
            return @this.RuleTemplate(m => m == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<sbyte?> EqualTo(this IRuleIn<sbyte?> @this, sbyte value)
        {
            return @this.RuleTemplate(m => m.Value == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<sbyte> NotEqualTo(this IRuleIn<sbyte> @this, sbyte value)
        {
            return @this.RuleTemplate(m => m != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<sbyte?> NotEqualTo(this IRuleIn<sbyte?> @this, sbyte value)
        {
            return @this.RuleTemplate(m => m.Value != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<sbyte> GreaterThan(this IRuleIn<sbyte> @this, sbyte min)
        {
            return @this.RuleTemplate(m => m > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<sbyte?> GreaterThan(this IRuleIn<sbyte?> @this, sbyte min)
        {
            return @this.RuleTemplate(m => m.Value > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<sbyte> GreaterThanOrEqualTo(this IRuleIn<sbyte> @this, sbyte min)
        {
            return @this.RuleTemplate(m => m >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<sbyte?> GreaterThanOrEqualTo(this IRuleIn<sbyte?> @this, sbyte min)
        {
            return @this.RuleTemplate(m => m.Value >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<sbyte> LessThan(this IRuleIn<sbyte> @this, sbyte max)
        {
            return @this.RuleTemplate(m => m < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<sbyte?> LessThan(this IRuleIn<sbyte?> @this, sbyte max)
        {
            return @this.RuleTemplate(m => m.Value < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<sbyte> LessThanOrEqualTo(this IRuleIn<sbyte> @this, sbyte max)
        {
            return @this.RuleTemplate(m => m <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<sbyte?> LessThanOrEqualTo(this IRuleIn<sbyte?> @this, sbyte max)
        {
            return @this.RuleTemplate(m => m.Value <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<sbyte> Between(this IRuleIn<sbyte> @this, sbyte min, sbyte max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m > min && m < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<sbyte?> Between(this IRuleIn<sbyte?> @this, sbyte min, sbyte max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value > min && m.Value < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<sbyte> BetweenOrEqualTo(this IRuleIn<sbyte> @this, sbyte min, sbyte max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m >= min && m <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<sbyte?> BetweenOrEqualTo(this IRuleIn<sbyte?> @this, sbyte min, sbyte max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value >= min && m.Value <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<sbyte> NonZero(this IRuleIn<sbyte> @this)
        {
            return @this.RuleTemplate(m => m != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<sbyte?> NonZero(this IRuleIn<sbyte?> @this)
        {
            return @this.RuleTemplate(m => m.Value != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<sbyte> Positive(this IRuleIn<sbyte> @this)
        {
            return @this.RuleTemplate(m => m > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<sbyte?> Positive(this IRuleIn<sbyte?> @this)
        {
            return @this.RuleTemplate(m => m.Value > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<sbyte> NonPositive(this IRuleIn<sbyte> @this)
        {
            return @this.RuleTemplate(m => m <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<sbyte?> NonPositive(this IRuleIn<sbyte?> @this)
        {
            return @this.RuleTemplate(m => m.Value <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<sbyte> Negative(this IRuleIn<sbyte> @this)
        {
            return @this.RuleTemplate(m => m < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<sbyte?> Negative(this IRuleIn<sbyte?> @this)
        {
            return @this.RuleTemplate(m => m.Value < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<sbyte> NonNegative(this IRuleIn<sbyte> @this)
        {
            return @this.RuleTemplate(m => m >= 0, MessageKey.Numbers.NonNegative);
        }

        public static IRuleOut<sbyte?> NonNegative(this IRuleIn<sbyte?> @this)
        {
            return @this.RuleTemplate(m => m.Value >= 0, MessageKey.Numbers.NonNegative);
        }
    }
}
