namespace Validot
{
    using Validot.Specification;
    using Validot.Translations;

    public static class ByteRules
    {
        public static IRuleOut<byte> EqualTo(this IRuleIn<byte> @this, byte value)
        {
            return @this.RuleTemplate(m => m == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<byte?> EqualTo(this IRuleIn<byte?> @this, byte value)
        {
            return @this.RuleTemplate(m => m.Value == value, MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<byte> NotEqualTo(this IRuleIn<byte> @this, byte value)
        {
            return @this.RuleTemplate(m => m != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<byte?> NotEqualTo(this IRuleIn<byte?> @this, byte value)
        {
            return @this.RuleTemplate(m => m.Value != value, MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value));
        }

        public static IRuleOut<byte> GreaterThan(this IRuleIn<byte> @this, byte min)
        {
            return @this.RuleTemplate(m => m > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<byte?> GreaterThan(this IRuleIn<byte?> @this, byte min)
        {
            return @this.RuleTemplate(m => m.Value > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<byte> GreaterThanOrEqualTo(this IRuleIn<byte> @this, byte min)
        {
            return @this.RuleTemplate(m => m >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<byte?> GreaterThanOrEqualTo(this IRuleIn<byte?> @this, byte min)
        {
            return @this.RuleTemplate(m => m.Value >= min, MessageKey.Numbers.GreaterThanOrEqualTo, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<byte> LessThan(this IRuleIn<byte> @this, byte max)
        {
            return @this.RuleTemplate(m => m < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<byte?> LessThan(this IRuleIn<byte?> @this, byte max)
        {
            return @this.RuleTemplate(m => m.Value < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<byte> LessThanOrEqualTo(this IRuleIn<byte> @this, byte max)
        {
            return @this.RuleTemplate(m => m <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<byte?> LessThanOrEqualTo(this IRuleIn<byte?> @this, byte max)
        {
            return @this.RuleTemplate(m => m.Value <= max, MessageKey.Numbers.LessThanOrEqualTo, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<byte> Between(this IRuleIn<byte> @this, byte min, byte max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m > min && m < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<byte?> Between(this IRuleIn<byte?> @this, byte min, byte max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value > min && m.Value < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<byte> BetweenOrEqualTo(this IRuleIn<byte> @this, byte min, byte max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m >= min && m <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<byte?> BetweenOrEqualTo(this IRuleIn<byte?> @this, byte min, byte max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value >= min && m.Value <= max, MessageKey.Numbers.BetweenOrEqualTo, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<byte> NonZero(this IRuleIn<byte> @this)
        {
            return @this.RuleTemplate(m => m != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<byte?> NonZero(this IRuleIn<byte?> @this)
        {
            return @this.RuleTemplate(m => m.Value != 0, MessageKey.Numbers.NonZero);
        }

        public static IRuleOut<byte> Positive(this IRuleIn<byte> @this)
        {
            return @this.RuleTemplate(m => m > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<byte?> Positive(this IRuleIn<byte?> @this)
        {
            return @this.RuleTemplate(m => m.Value > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<byte> NonPositive(this IRuleIn<byte> @this)
        {
            return @this.RuleTemplate(m => m <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<byte?> NonPositive(this IRuleIn<byte?> @this)
        {
            return @this.RuleTemplate(m => m.Value <= 0, MessageKey.Numbers.NonPositive);
        }
    }
}
