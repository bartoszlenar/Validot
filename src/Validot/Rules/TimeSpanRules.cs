namespace Validot
{
    using System;

    using Validot.Specification;
    using Validot.Translations;

    public static class TimeSpanRules
    {
        public static IRuleOut<TimeSpan> EqualTo(this IRuleIn<TimeSpan> @this, TimeSpan value)
        {
            return @this.RuleTemplate(m => m == value, MessageKey.TimeSpanType.EqualTo, Arg.Time(nameof(value), value));
        }

        public static IRuleOut<TimeSpan?> EqualTo(this IRuleIn<TimeSpan?> @this, TimeSpan value)
        {
            return @this.RuleTemplate(m => m.Value == value, MessageKey.TimeSpanType.EqualTo, Arg.Time(nameof(value), value));
        }

        public static IRuleOut<TimeSpan> NotEqualTo(this IRuleIn<TimeSpan> @this, TimeSpan value)
        {
            return @this.RuleTemplate(m => m != value, MessageKey.TimeSpanType.NotEqualTo, Arg.Time(nameof(value), value));
        }

        public static IRuleOut<TimeSpan?> NotEqualTo(this IRuleIn<TimeSpan?> @this, TimeSpan value)
        {
            return @this.RuleTemplate(m => m.Value != value, MessageKey.TimeSpanType.NotEqualTo, Arg.Time(nameof(value), value));
        }

        public static IRuleOut<TimeSpan> GreaterThan(this IRuleIn<TimeSpan> @this, TimeSpan min)
        {
            return @this.RuleTemplate(m => m > min, MessageKey.TimeSpanType.GreaterThan, Arg.Time(nameof(min), min));
        }

        public static IRuleOut<TimeSpan?> GreaterThan(this IRuleIn<TimeSpan?> @this, TimeSpan min)
        {
            return @this.RuleTemplate(m => m.Value > min, MessageKey.TimeSpanType.GreaterThan, Arg.Time(nameof(min), min));
        }

        public static IRuleOut<TimeSpan> GreaterThanOrEqualTo(this IRuleIn<TimeSpan> @this, TimeSpan min)
        {
            return @this.RuleTemplate(m => m >= min, MessageKey.TimeSpanType.GreaterThanOrEqualTo, Arg.Time(nameof(min), min));
        }

        public static IRuleOut<TimeSpan?> GreaterThanOrEqualTo(this IRuleIn<TimeSpan?> @this, TimeSpan min)
        {
            return @this.RuleTemplate(m => m.Value >= min, MessageKey.TimeSpanType.GreaterThanOrEqualTo, Arg.Time(nameof(min), min));
        }

        public static IRuleOut<TimeSpan> LessThan(this IRuleIn<TimeSpan> @this, TimeSpan max)
        {
            return @this.RuleTemplate(m => m < max, MessageKey.TimeSpanType.LessThan, Arg.Time(nameof(max), max));
        }

        public static IRuleOut<TimeSpan?> LessThan(this IRuleIn<TimeSpan?> @this, TimeSpan max)
        {
            return @this.RuleTemplate(m => m.Value < max, MessageKey.TimeSpanType.LessThan, Arg.Time(nameof(max), max));
        }

        public static IRuleOut<TimeSpan> LessThanOrEqualTo(this IRuleIn<TimeSpan> @this, TimeSpan max)
        {
            return @this.RuleTemplate(m => m <= max, MessageKey.TimeSpanType.LessThanOrEqualTo, Arg.Time(nameof(max), max));
        }

        public static IRuleOut<TimeSpan?> LessThanOrEqualTo(this IRuleIn<TimeSpan?> @this, TimeSpan max)
        {
            return @this.RuleTemplate(m => m.Value <= max, MessageKey.TimeSpanType.LessThanOrEqualTo, Arg.Time(nameof(max), max));
        }

        public static IRuleOut<TimeSpan> Between(this IRuleIn<TimeSpan> @this, TimeSpan min, TimeSpan max)
        {
            ThrowHelper.InvalidRange(min.Ticks, nameof(min), max.Ticks, nameof(max));

            return @this.RuleTemplate(m => m > min && m < max, MessageKey.TimeSpanType.Between, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max));
        }

        public static IRuleOut<TimeSpan?> Between(this IRuleIn<TimeSpan?> @this, TimeSpan min, TimeSpan max)
        {
            ThrowHelper.InvalidRange(min.Ticks, nameof(min), max.Ticks, nameof(max));

            return @this.RuleTemplate(m => m.Value > min && m.Value < max, MessageKey.TimeSpanType.Between, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max));
        }

        public static IRuleOut<TimeSpan> BetweenOrEqualTo(this IRuleIn<TimeSpan> @this, TimeSpan min, TimeSpan max)
        {
            ThrowHelper.InvalidRange(min.Ticks, nameof(min), max.Ticks, nameof(max));

            return @this.RuleTemplate(m => m >= min && m <= max, MessageKey.TimeSpanType.BetweenOrEqualTo, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max));
        }

        public static IRuleOut<TimeSpan?> BetweenOrEqualTo(this IRuleIn<TimeSpan?> @this, TimeSpan min, TimeSpan max)
        {
            ThrowHelper.InvalidRange(min.Ticks, nameof(min), max.Ticks, nameof(max));

            return @this.RuleTemplate(m => m.Value >= min && m.Value <= max, MessageKey.TimeSpanType.BetweenOrEqualTo, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max));
        }

        public static IRuleOut<TimeSpan> NonZero(this IRuleIn<TimeSpan> @this)
        {
            return @this.RuleTemplate(m => m.Ticks != 0, MessageKey.TimeSpanType.NonZero);
        }

        public static IRuleOut<TimeSpan?> NonZero(this IRuleIn<TimeSpan?> @this)
        {
            return @this.RuleTemplate(m => m.Value.Ticks != 0, MessageKey.TimeSpanType.NonZero);
        }

        public static IRuleOut<TimeSpan> Positive(this IRuleIn<TimeSpan> @this)
        {
            return @this.RuleTemplate(m => m.Ticks > 0, MessageKey.TimeSpanType.Positive);
        }

        public static IRuleOut<TimeSpan?> Positive(this IRuleIn<TimeSpan?> @this)
        {
            return @this.RuleTemplate(m => m.Value.Ticks > 0, MessageKey.TimeSpanType.Positive);
        }

        public static IRuleOut<TimeSpan> NonPositive(this IRuleIn<TimeSpan> @this)
        {
            return @this.RuleTemplate(m => m.Ticks <= 0, MessageKey.TimeSpanType.NonPositive);
        }

        public static IRuleOut<TimeSpan?> NonPositive(this IRuleIn<TimeSpan?> @this)
        {
            return @this.RuleTemplate(m => m.Value.Ticks <= 0, MessageKey.TimeSpanType.NonPositive);
        }

        public static IRuleOut<TimeSpan> Negative(this IRuleIn<TimeSpan> @this)
        {
            return @this.RuleTemplate(m => m.Ticks < 0, MessageKey.TimeSpanType.Negative);
        }

        public static IRuleOut<TimeSpan?> Negative(this IRuleIn<TimeSpan?> @this)
        {
            return @this.RuleTemplate(m => m.Value.Ticks < 0, MessageKey.TimeSpanType.Negative);
        }

        public static IRuleOut<TimeSpan> NonNegative(this IRuleIn<TimeSpan> @this)
        {
            return @this.RuleTemplate(m => m.Ticks >= 0, MessageKey.TimeSpanType.NonNegative);
        }

        public static IRuleOut<TimeSpan?> NonNegative(this IRuleIn<TimeSpan?> @this)
        {
            return @this.RuleTemplate(m => m.Value.Ticks >= 0, MessageKey.TimeSpanType.NonNegative);
        }
    }
}
