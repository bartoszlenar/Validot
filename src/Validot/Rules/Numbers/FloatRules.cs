namespace Validot
{
    using System;

    using Validot.Specification;
    using Validot.Translations;

    public static class FloatRules
    {
        public static IRuleOut<float> EqualTo(this IRuleIn<float> @this, float value, float tolerance = 0.0000001f)
        {
            return @this.RuleTemplate(m => AreEqual(m, value, tolerance), MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value), Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<float?> EqualTo(this IRuleIn<float?> @this, float value, float tolerance = 0.0000001f)
        {
            return @this.RuleTemplate(m => AreEqual(m.Value, value, tolerance), MessageKey.Numbers.EqualTo, Arg.Number(nameof(value), value), Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<float> NotEqualTo(this IRuleIn<float> @this, float value, float tolerance = 0.0000001f)
        {
            return @this.RuleTemplate(m => !AreEqual(m, value, tolerance), MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value), Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<float?> NotEqualTo(this IRuleIn<float?> @this, float value, float tolerance = 0.0000001f)
        {
            return @this.RuleTemplate(m => !AreEqual(m.Value, value, tolerance), MessageKey.Numbers.NotEqualTo, Arg.Number(nameof(value), value), Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<float> GreaterThan(this IRuleIn<float> @this, float min)
        {
            return @this.RuleTemplate(m => m > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<float?> GreaterThan(this IRuleIn<float?> @this, float min)
        {
            return @this.RuleTemplate(m => m.Value > min, MessageKey.Numbers.GreaterThan, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<float> LessThan(this IRuleIn<float> @this, float max)
        {
            return @this.RuleTemplate(m => m < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<float?> LessThan(this IRuleIn<float?> @this, float max)
        {
            return @this.RuleTemplate(m => m.Value < max, MessageKey.Numbers.LessThan, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<float> Between(this IRuleIn<float> @this, float min, float max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m > min && m < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<float?> Between(this IRuleIn<float?> @this, float min, float max)
        {
            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(m => m.Value > min && m.Value < max, MessageKey.Numbers.Between, Arg.Number(nameof(min), min), Arg.Number(nameof(max), max));
        }

        public static IRuleOut<float> NonZero(this IRuleIn<float> @this, float tolerance = 0.0000001f)
        {
            return @this.RuleTemplate(m => !AreEqual(m, 0f, tolerance), MessageKey.Numbers.NonZero, Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<float?> NonZero(this IRuleIn<float?> @this, float tolerance = 0.0000001f)
        {
            return @this.RuleTemplate(m => !AreEqual(m.Value, 0f, tolerance), MessageKey.Numbers.NonZero, Arg.Number(nameof(tolerance), tolerance));
        }

        public static IRuleOut<float> NonNaN(this IRuleIn<float> @this)
        {
            return @this.RuleTemplate(m => !float.IsNaN(m), MessageKey.Numbers.NonNaN);
        }

        public static IRuleOut<float?> NonNaN(this IRuleIn<float?> @this)
        {
            return @this.RuleTemplate(m => !float.IsNaN(m.Value), MessageKey.Numbers.NonNaN);
        }

        public static IRuleOut<float> Positive(this IRuleIn<float> @this)
        {
            return @this.RuleTemplate(m => m > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<float?> Positive(this IRuleIn<float?> @this)
        {
            return @this.RuleTemplate(m => m.Value > 0, MessageKey.Numbers.Positive);
        }

        public static IRuleOut<float> NonPositive(this IRuleIn<float> @this)
        {
            return @this.RuleTemplate(m => m <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<float?> NonPositive(this IRuleIn<float?> @this)
        {
            return @this.RuleTemplate(m => m.Value <= 0, MessageKey.Numbers.NonPositive);
        }

        public static IRuleOut<float> Negative(this IRuleIn<float> @this)
        {
            return @this.RuleTemplate(m => m < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<float?> Negative(this IRuleIn<float?> @this)
        {
            return @this.RuleTemplate(m => m.Value < 0, MessageKey.Numbers.Negative);
        }

        public static IRuleOut<float> NonNegative(this IRuleIn<float> @this)
        {
            return @this.RuleTemplate(m => m >= 0, MessageKey.Numbers.NonNegative);
        }

        public static IRuleOut<float?> NonNegative(this IRuleIn<float?> @this)
        {
            return @this.RuleTemplate(m => m.Value >= 0, MessageKey.Numbers.NonNegative);
        }

        private static bool AreEqual(float a, float b, float tolerance)
        {
            return Math.Abs(a - b) < tolerance;
        }
    }
}
