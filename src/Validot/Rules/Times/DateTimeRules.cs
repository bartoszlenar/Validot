namespace Validot
{
    using System;

    using Validot.Specification;
    using Validot.Translations;

    public static class DateTimeRules
    {
        public static IRuleOut<DateTime> EqualTo(this IRuleIn<DateTime> @this, DateTime value, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, value, timeComparison) == 0, MessageKey.Times.EqualTo, Arg.Time(nameof(value), value), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime?> EqualTo(this IRuleIn<DateTime?> @this, DateTime value, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, value, timeComparison) == 0, MessageKey.Times.EqualTo, Arg.Time(nameof(value), value), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime> NotEqualTo(this IRuleIn<DateTime> @this, DateTime value, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, value, timeComparison) != 0, MessageKey.Times.NotEqualTo, Arg.Time(nameof(value), value), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime?> NotEqualTo(this IRuleIn<DateTime?> @this, DateTime value, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, value, timeComparison) != 0, MessageKey.Times.NotEqualTo, Arg.Time(nameof(value), value), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime> After(this IRuleIn<DateTime> @this, DateTime min, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, min, timeComparison) > 0, MessageKey.Times.After, Arg.Time(nameof(min), min), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime?> After(this IRuleIn<DateTime?> @this, DateTime min, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, min, timeComparison) > 0, MessageKey.Times.After, Arg.Time(nameof(min), min), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime> AfterOrEqualTo(this IRuleIn<DateTime> @this, DateTime min, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, min, timeComparison) >= 0, MessageKey.Times.AfterOrEqualTo, Arg.Time(nameof(min), min), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime?> AfterOrEqualTo(this IRuleIn<DateTime?> @this, DateTime min, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, min, timeComparison) >= 0, MessageKey.Times.AfterOrEqualTo, Arg.Time(nameof(min), min), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime> Before(this IRuleIn<DateTime> @this, DateTime max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, max, timeComparison) < 0, MessageKey.Times.Before, Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime?> Before(this IRuleIn<DateTime?> @this, DateTime max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, max, timeComparison) < 0, MessageKey.Times.Before, Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime> BeforeOrEqualTo(this IRuleIn<DateTime> @this, DateTime max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, max, timeComparison) <= 0, MessageKey.Times.BeforeOrEqualTo, Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime?> BeforeOrEqualTo(this IRuleIn<DateTime?> @this, DateTime max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, max, timeComparison) <= 0, MessageKey.Times.BeforeOrEqualTo, Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime> Between(this IRuleIn<DateTime> @this, DateTime min, DateTime max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, min, timeComparison) > 0 && TimeComparer.Compare(m, max, timeComparison) < 0, MessageKey.Times.Between, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime?> Between(this IRuleIn<DateTime?> @this, DateTime min, DateTime max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, min, timeComparison) > 0 && TimeComparer.Compare(m.Value, max, timeComparison) < 0, MessageKey.Times.Between, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime> BetweenOrEqualTo(this IRuleIn<DateTime> @this, DateTime min, DateTime max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m, min, timeComparison) >= 0 && TimeComparer.Compare(m, max, timeComparison) <= 0, MessageKey.Times.BetweenOrEqualTo, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }

        public static IRuleOut<DateTime?> BetweenOrEqualTo(this IRuleIn<DateTime?> @this, DateTime min, DateTime max, TimeComparison timeComparison = TimeComparison.All)
        {
            return @this.RuleTemplate(m => TimeComparer.Compare(m.Value, min, timeComparison) >= 0 && TimeComparer.Compare(m.Value, max, timeComparison) <= 0, MessageKey.Times.BetweenOrEqualTo, Arg.Time(nameof(min), min), Arg.Time(nameof(max), max), Arg.Enum(nameof(timeComparison), timeComparison));
        }
    }
}
