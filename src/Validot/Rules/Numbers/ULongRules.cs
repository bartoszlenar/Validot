namespace Validot
{
    using Validot.Specification;
    using Validot.Translations;

    public static class ULongRules
    {
        public static IRuleOut<ulong> EqualTo(this IRuleIn<ulong> @this, ulong value)
        {
            return @this.RuleTemplate(m => m == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<ulong?> EqualTo(this IRuleIn<ulong?> @this, ulong value)
        {
            return @this.RuleTemplate(m => m.Value == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<ulong> NotEqualTo(this IRuleIn<ulong> @this, ulong value)
        {
            return @this.RuleTemplate(m => m != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<ulong?> NotEqualTo(this IRuleIn<ulong?> @this, ulong value)
        {
            return @this.RuleTemplate(m => m.Value != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<ulong> GreaterThan(this IRuleIn<ulong> @this, ulong min)
        {
            return @this.RuleTemplate(m => m > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<ulong?> GreaterThan(this IRuleIn<ulong?> @this, ulong min)
        {
            return @this.RuleTemplate(m => m.Value > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<ulong> GreaterThanOrEqualTo(this IRuleIn<ulong> @this, ulong min)
        {
            return @this.RuleTemplate(m => m >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<ulong?> GreaterThanOrEqualTo(this IRuleIn<ulong?> @this, ulong min)
        {
            return @this.RuleTemplate(m => m.Value >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<ulong> LessThan(this IRuleIn<ulong> @this, ulong max)
        {
            return @this.RuleTemplate(m => m < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<ulong?> LessThan(this IRuleIn<ulong?> @this, ulong max)
        {
            return @this.RuleTemplate(m => m.Value < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<ulong> LessThanOrEqualTo(this IRuleIn<ulong> @this, ulong max)
        {
            return @this.RuleTemplate(m => m <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<ulong?> LessThanOrEqualTo(this IRuleIn<ulong?> @this, ulong max)
        {
            return @this.RuleTemplate(m => m.Value <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<ulong> Between(this IRuleIn<ulong> @this, ulong min, ulong max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m > min && m < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<ulong?> Between(this IRuleIn<ulong?> @this, ulong min, ulong max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value > min && m.Value < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<ulong> BetweenOrEqualTo(this IRuleIn<ulong> @this, ulong min, ulong max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m >= min && m <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<ulong?> BetweenOrEqualTo(this IRuleIn<ulong?> @this, ulong min, ulong max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value >= min && m.Value <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<ulong> NonZero(this IRuleIn<ulong> @this)
        {
            return @this.RuleTemplate(m => m != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<ulong?> NonZero(this IRuleIn<ulong?> @this)
        {
            return @this.RuleTemplate(m => m.Value != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<ulong> Positive(this IRuleIn<ulong> @this)
        {
            return @this.RuleTemplate(m => m > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<ulong?> Positive(this IRuleIn<ulong?> @this)
        {
            return @this.RuleTemplate(m => m.Value > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<ulong> NonPositive(this IRuleIn<ulong> @this)
        {
            return @this.RuleTemplate(m => m <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<ulong?> NonPositive(this IRuleIn<ulong?> @this)
        {
            return @this.RuleTemplate(m => m.Value <= 0, MessageKey.Numbers.NonPositive);
        }
    }
}
