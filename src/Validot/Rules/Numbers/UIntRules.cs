namespace Validot
{
    using Validot.Specification;
    using Validot.Translations;

    public static class UIntRules
    {
        public static IRuleOut<uint> EqualTo(this IRuleIn<uint> @this, uint value)
        {
            return @this.RuleTemplate(m => m == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<uint?> EqualTo(this IRuleIn<uint?> @this, uint value)
        {
            return @this.RuleTemplate(m => m.Value == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<uint> NotEqualTo(this IRuleIn<uint> @this, uint value)
        {
            return @this.RuleTemplate(m => m != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<uint?> NotEqualTo(this IRuleIn<uint?> @this, uint value)
        {
            return @this.RuleTemplate(m => m.Value != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<uint> GreaterThan(this IRuleIn<uint> @this, uint min)
        {
            return @this.RuleTemplate(m => m > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<uint?> GreaterThan(this IRuleIn<uint?> @this, uint min)
        {
            return @this.RuleTemplate(m => m.Value > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<uint> GreaterThanOrEqualTo(this IRuleIn<uint> @this, uint min)
        {
            return @this.RuleTemplate(m => m >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<uint?> GreaterThanOrEqualTo(this IRuleIn<uint?> @this, uint min)
        {
            return @this.RuleTemplate(m => m.Value >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<uint> LessThan(this IRuleIn<uint> @this, uint max)
        {
            return @this.RuleTemplate(m => m < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<uint?> LessThan(this IRuleIn<uint?> @this, uint max)
        {
            return @this.RuleTemplate(m => m.Value < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<uint> LessThanOrEqualTo(this IRuleIn<uint> @this, uint max)
        {
            return @this.RuleTemplate(m => m <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<uint?> LessThanOrEqualTo(this IRuleIn<uint?> @this, uint max)
        {
            return @this.RuleTemplate(m => m.Value <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<uint> Between(this IRuleIn<uint> @this, uint min, uint max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m > min && m < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<uint?> Between(this IRuleIn<uint?> @this, uint min, uint max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value > min && m.Value < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<uint> BetweenOrEqualTo(this IRuleIn<uint> @this, uint min, uint max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m >= min && m <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<uint?> BetweenOrEqualTo(this IRuleIn<uint?> @this, uint min, uint max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value >= min && m.Value <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<uint> NonZero(this IRuleIn<uint> @this)
        {
            return @this.RuleTemplate(m => m != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<uint?> NonZero(this IRuleIn<uint?> @this)
        {
            return @this.RuleTemplate(m => m.Value != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<uint> Positive(this IRuleIn<uint> @this)
        {
            return @this.RuleTemplate(m => m > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<uint?> Positive(this IRuleIn<uint?> @this)
        {
            return @this.RuleTemplate(m => m.Value > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<uint> NonPositive(this IRuleIn<uint> @this)
        {
            return @this.RuleTemplate(m => m <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<uint?> NonPositive(this IRuleIn<uint?> @this)
        {
            return @this.RuleTemplate(m => m.Value <= 0, MessageKey.Numbers.NonPositive);
        }
    }
}
