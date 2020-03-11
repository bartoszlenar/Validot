namespace Validot
{
    using System;

    using Validot.Specification;
    using Validot.Translations;

    public static class DoubleRules
    {
        public static IRuleOut<double> EqualTo(this IRuleIn<double> @this, double value, double tolerance = 0.0000001d)
        {
            return @this.RuleTemplate(m => AreEqual(m, value, tolerance), MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value), Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<double?> EqualTo(this IRuleIn<double?> @this, double value, double tolerance = 0.0000001d)
        {
            return @this.RuleTemplate(m => AreEqual(m.Value, value, tolerance), MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value), Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<double> NotEqualTo(this IRuleIn<double> @this, double value, double tolerance = 0.0000001d)
        {
            return @this.RuleTemplate(m => !AreEqual(m, value, tolerance), MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value), Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<double?> NotEqualTo(this IRuleIn<double?> @this, double value, double tolerance = 0.0000001d)
        {
            return @this.RuleTemplate(m => !AreEqual(m.Value, value, tolerance), MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value), Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<double> GreaterThan(this IRuleIn<double> @this, double min)
        {
            return @this.RuleTemplate(m => m > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<double?> GreaterThan(this IRuleIn<double?> @this, double min)
        {
            return @this.RuleTemplate(m => m.Value > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<double> LessThan(this IRuleIn<double> @this, double max)
        {
            return @this.RuleTemplate(m => m < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<double?> LessThan(this IRuleIn<double?> @this, double max)
        {
            return @this.RuleTemplate(m => m.Value < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<double> Between(this IRuleIn<double> @this, double min, double max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m > min && m < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<double?> Between(this IRuleIn<double?> @this, double min, double max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value > min && m.Value < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<double> NonZero(this IRuleIn<double> @this, double tolerance = 0.0000001d)
        {
            return @this.RuleTemplate(m => !AreEqual(m, 0d, tolerance), MessageKey.Numbers.NonZero, Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<double?> NonZero(this IRuleIn<double?> @this, double tolerance = 0.0000001d)
        {
            return @this.RuleTemplate(m => !AreEqual(m.Value, 0d, tolerance), MessageKey.Numbers.NonZero, Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<double> NonNaN(this IRuleIn<double> @this)
        {
            return @this.RuleTemplate(m => !double.IsNaN(m), MessageKey.Numbers.NonNaN);
        }

        public static IRuleOut<double?> NonNaN(this IRuleIn<double?> @this)
        {
            return @this.RuleTemplate(m => !double.IsNaN(m.Value), MessageKey.Numbers.NonNaN);
        }

        public static IRuleOut<double> Positive(this IRuleIn<double> @this)
        {
            return @this.RuleTemplate(m => m > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<double?> Positive(this IRuleIn<double?> @this)
        {
            return @this.RuleTemplate(m => m.Value > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<double> NonPositive(this IRuleIn<double> @this)
        {
            return @this.RuleTemplate(m => m <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<double?> NonPositive(this IRuleIn<double?> @this)
        {
            return @this.RuleTemplate(m => m.Value <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<double> Negative(this IRuleIn<double> @this)
        {
            return @this.RuleTemplate(m => m < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<double?> Negative(this IRuleIn<double?> @this)
        {
            return @this.RuleTemplate(m => m.Value < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<double> NonNegative(this IRuleIn<double> @this)
        {
            return @this.RuleTemplate(m => m >= 0, MessageKey.Numbers.NonNegative);
        }

        public static IRuleOut<double?> NonNegative(this IRuleIn<double?> @this)
        {
            return @this.RuleTemplate(m => m.Value >= 0, MessageKey.Numbers.NonNegative);
        }

        private static bool AreEqual(double a, double b, double tolerance)
        {
            return Math.Abs(a - b) < tolerance;
        }
    }
}
