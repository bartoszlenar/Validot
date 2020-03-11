namespace Validot
{
    using System;

    using Validot.Specification;
    using Validot.Translations;

    public static class DateTimeOffsetRules
    {
        public static IRuleOut<DateTimeOffset> EqualTo(this IRuleIn<DateTimeOffset> @this, DateTimeOffset value, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, value, timeComparison) == 0, MessageKey.Times.EqualTo, Arg.Time(nameof(value), value), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset?> EqualTo(this IRuleIn<DateTimeOffset?> @this, DateTimeOffset value, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, value, timeComparison) == 0, MessageKey.Times.EqualTo, Arg.Time(nameof(value), value), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset> NotEqualTo(this IRuleIn<DateTimeOffset> @this, DateTimeOffset value, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, value, timeComparison) != 0, MessageKey.Times.NotEqualTo, Arg.Time(nameof(value), value), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset?> NotEqualTo(this IRuleIn<DateTimeOffset?> @this, DateTimeOffset value, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, value, timeComparison) != 0, MessageKey.Times.NotEqualTo, Arg.Time(nameof(value), value), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset> After(this IRuleIn<DateTimeOffset> @this, DateTimeOffset min, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, min, timeComparison) > 0, MessageKey.Times.After, Arg.Time(nameof(min), min), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset?> After(this IRuleIn<DateTimeOffset?> @this, DateTimeOffset min, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, min, timeComparison) > 0, MessageKey.Times.After, Arg.Time(nameof(min), min), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset> AfterOrEqualTo(this IRuleIn<DateTimeOffset> @this, DateTimeOffset min, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, min, timeComparison) >= 0, MessageKey.Times.AfterOrEqualTo, Arg.Time(nameof(min), min), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset?> AfterOrEqualTo(this IRuleIn<DateTimeOffset?> @this, DateTimeOffset min, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, min, timeComparison) >= 0, MessageKey.Times.AfterOrEqualTo, Arg.Time(nameof(min), min), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset> Before(this IRuleIn<DateTimeOffset> @this, DateTimeOffset max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, max, timeComparison) < 0, MessageKey.Times.Before, Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset?> Before(this IRuleIn<DateTimeOffset?> @this, DateTimeOffset max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, max, timeComparison) < 0, MessageKey.Times.Before, Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset> BeforeOrEqualTo(this IRuleIn<DateTimeOffset> @this, DateTimeOffset max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, max, timeComparison) <= 0, MessageKey.Times.BeforeOrEqualTo, Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset?> BeforeOrEqualTo(this IRuleIn<DateTimeOffset?> @this, DateTimeOffset max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, max, timeComparison) <= 0, MessageKey.Times.BeforeOrEqualTo, Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset> Between(this IRuleIn<DateTimeOffset> @this, DateTimeOffset min, DateTimeOffset max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, min, timeComparison) > 0 && TimeComparer.Compare(m, max, timeComparison) < 0, MessageKey.Times.Between, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset?> Between(this IRuleIn<DateTimeOffset?> @this, DateTimeOffset min, DateTimeOffset max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, min, timeComparison) > 0 && TimeComparer.Compare(m.Value, max, timeComparison) < 0, MessageKey.Times.Between, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset> BetweenOrEqualTo(this IRuleIn<DateTimeOffset> @this, DateTimeOffset min, DateTimeOffset max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, min, timeComparison) >= 0 && TimeComparer.Compare(m, max, timeComparison) <= 0, MessageKey.Times.BetweenOrEqualTo, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTimeOffset?> BetweenOrEqualTo(this IRuleIn<DateTimeOffset?> @this, DateTimeOffset min, DateTimeOffset max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, min, timeComparison) >= 0 && TimeComparer.Compare(m.Value, max, timeComparison) <= 0, MessageKey.Times.BetweenOrEqualTo, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }
    }
}
