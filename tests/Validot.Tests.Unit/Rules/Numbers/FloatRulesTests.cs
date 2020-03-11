namespace Validot.Tests.Unit.Rules.Numbers
{
    using System;
    using System.Collections.Generic;

    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class FloatRulesTests
    {
        private static readonly Func<int, float> Convert = i => i;

        public static IEnumerable<object[]> EqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.EqualTo_Unsigned(Convert),
                NumbersTestData.EqualTo_Signed(Convert),
                NumbersTestData.EqualTo_Limits(float.MinValue, float.MaxValue, 0),
                new[] { new object[] { 0.999999F, 0F, false } },
                new[] { new object[] { 1.000001F, 0F, false } },
                new[] { new object[] { 1.123456F, 1.123456F, true } });
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError(float model, float value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.EqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", 0.0000001F));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError_FromNullable(float model, float value, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.EqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", 0.0000001F));
        }

        public static IEnumerable<object[]> EqualTo_WithTolerance_Should_CollectError_MemberData()
        {
            return RulesHelper.GetTestDataCombined(
                new[] { new object[] { 1.000100F, 1.000199F, 0.0000001F, false } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.000001F, false } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.00001F, false } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.0001F, true } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.001F, true } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.01F, true } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.1F, true } },
                new[] { new object[] { 1.000100F, 1.000199F, 1F, true } });
        }

        [Theory]
        [MemberData(nameof(EqualTo_WithTolerance_Should_CollectError_MemberData))]
        public void EqualTo_WithTolerance_Should_CollectError(float model, float value, float tolerance, bool expectedIsValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.EqualTo(value, tolerance),
                expectedIsValid,
                MessageKey.Numbers.EqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", tolerance));
        }

        [Theory]
        [MemberData(nameof(EqualTo_WithTolerance_Should_CollectError_MemberData))]
        public void EqualTo_WithTolerance_Should_CollectError_FromNullable(float model, float value, float tolerance, bool expectedIsValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.EqualTo(value, tolerance),
                expectedIsValid,
                MessageKey.Numbers.EqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", tolerance));
        }

        public static IEnumerable<object[]> NotEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NotEqualTo_Unsigned(Convert),
                NumbersTestData.NotEqualTo_Signed(Convert),
                NumbersTestData.NotEqualTo_Limits(float.MinValue, float.MaxValue, 0),
                new[] { new object[] { 0.999999F, 0F, true } },
                new[] { new object[] { 1.000001F, 0F, true } },
                new[] { new object[] { 1.123456F, 1.123456F, false } });
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError(float model, float value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.NotEqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", 0.0000001F));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError_FromNullable(float model, float value, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.NotEqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", 0.0000001F));
        }

        public static IEnumerable<object[]> NotEqualTo_WithTolerance_Should_CollectError_MemberData()
        {
            return RulesHelper.GetTestDataCombined(
                new[] { new object[] { 1.000100F, 1.000199F, 0.0000001F, true } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.000001F, true } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.00001F, true } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.0001F, false } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.001F, false } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.01F, false } },
                new[] { new object[] { 1.000100F, 1.000199F, 0.1F, false } },
                new[] { new object[] { 1.000100F, 1.000199F, 1F, false } });
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_WithTolerance_Should_CollectError_MemberData))]
        public void NotEqualTo_WithTolerance_Should_CollectError(float model, float value, float tolerance, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEqualTo(value, tolerance),
                shouldBeValid,
                MessageKey.Numbers.NotEqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", tolerance));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_WithTolerance_Should_CollectError_MemberData))]
        public void NotEqualTo_WithTolerance_Should_CollectError_FromNullable(float model, float value, float tolerance, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.NotEqualTo(value, tolerance),
                shouldBeValid,
                MessageKey.Numbers.NotEqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", tolerance));
        }

        public static IEnumerable<object[]> GreaterThan_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.GreaterThan_Unsigned(Convert),
                NumbersTestData.GreaterThan_Signed(Convert),
                NumbersTestData.GreaterThan_Limits(float.MinValue, float.MaxValue, 0),
                new[] { new object[] { 0.999999F, 1F, false } },
                new[] { new object[] { 1.000001F, 1F, true } },
                new[] { new object[] { 0.999999F, 0.999999F, false } },
                new[] { new object[] { 1F, 1.000001F, false } },
                new[] { new object[] { 1.000001F, 1.000001F, false } });
        }

        [Theory]
        [MemberData(nameof(GreaterThan_Should_CollectError_Data))]
        public void GreaterThan_Should_CollectError(float model, float min, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.GreaterThan(min),
                shouldBeValid,
                MessageKey.Numbers.GreaterThan,
                Arg.Number("min", min));
        }

        [Theory]
        [MemberData(nameof(GreaterThan_Should_CollectError_Data))]
        public void GreaterThan_Should_CollectError_FromNullable(float model, float min, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.GreaterThan(min),
                shouldBeValid,
                MessageKey.Numbers.GreaterThan,
                Arg.Number("min", min));
        }

        public static IEnumerable<object[]> LessThan_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.LessThan_Unsigned(Convert),
                NumbersTestData.LessThan_Signed(Convert),
                NumbersTestData.LessThan_Limits(float.MinValue, float.MaxValue, 0),
                new[] { new object[] { 0.999999F, 1F, true } },
                new[] { new object[] { 1.000001F, 1F, false } },
                new[] { new object[] { 0.999999F, 0.999999F, false } },
                new[] { new object[] { 1F, 1.000001F, true } },
                new[] { new object[] { 1.000001F, 1.000001F, false } });
        }

        [Theory]
        [MemberData(nameof(LessThan_Should_CollectError_Data))]
        public void LessThan_Should_CollectError(float model, float max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.LessThan(max),
                shouldBeValid,
                MessageKey.Numbers.LessThan,
                Arg.Number("max", max));
        }

        [Theory]
        [MemberData(nameof(LessThan_Should_CollectError_Data))]
        public void LessThan_Should_CollectError_FromNullable(float model, float max, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.LessThan(max),
                shouldBeValid,
                MessageKey.Numbers.LessThan,
                Arg.Number("max", max));
        }

        public static IEnumerable<object[]> Between_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Between_Unsigned(Convert),
                NumbersTestData.Between_Signed(Convert),
                NumbersTestData.Between_Limits(float.MinValue, float.MaxValue, 0),
                new[] { new object[] { 0.999999F, 1, 1.000001F, true } },
                new[] { new object[] { 0.999999F, 0.999999F, 1.000001F, false } },
                new[] { new object[] { 0.999999F, 1.000001F, 1.000001F, false } });
        }

        [Theory]
        [MemberData(nameof(Between_Should_CollectError_Data))]
        public void Between_Should_CollectError(float min, float model, float max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Between(min, max),
                shouldBeValid,
                MessageKey.Numbers.Between,
                Arg.Number("min", min),
                Arg.Number("max", max));
        }

        [Theory]
        [MemberData(nameof(Between_Should_CollectError_Data))]
        public void Between_Should_CollectError_FromNullable(float min, float model, float max, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.Between(min, max),
                shouldBeValid,
                MessageKey.Numbers.Between,
                Arg.Number("min", min),
                Arg.Number("max", max));
        }

        public static IEnumerable<object[]> Between_Should_ThrowException_When_MinLargerThanMax_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Between_InvalidRange(Convert, float.MinValue, float.MaxValue));
        }

        [Theory]
        [MemberData(nameof(Between_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void Between_Should_ThrowException_When_MinLargerThanMax(float min, float max)
        {
            Tester.TestExceptionOnInit<float>(
                s => s.Between(min, max),
                typeof(ArgumentException));
        }

        [Theory]
        [MemberData(nameof(Between_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void Between_Should_ThrowException_When_MinLargerThanMax_FromNullable(float min, float max)
        {
            Tester.TestExceptionOnInit<float?>(
                s => s.Between(min, max),
                typeof(ArgumentException));
        }

        public static IEnumerable<object[]> NonZero_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonZero_Unsigned(Convert),
                NumbersTestData.NonZero_Signed(Convert),
                NumbersTestData.NonZero_Signed_Limits(float.MinValue, float.MaxValue));
        }

        [Theory]
        [MemberData(nameof(NonZero_Should_CollectError_Data))]
        public void NonZero_Should_CollectError(float model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonZero(),
                shouldBeValid,
                MessageKey.Numbers.NonZero,
                Arg.Number("tolerance", 0.0000001F));
        }

        [Theory]
        [MemberData(nameof(NonZero_Should_CollectError_Data))]
        public void NonZero_Should_CollectError_FromNullable(float model, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.NonZero(),
                shouldBeValid,
                MessageKey.Numbers.NonZero,
                Arg.Number("tolerance", 0.0000001F));
        }

        public static IEnumerable<object[]> NonZero_WithTolerance_Should_CollectError_MemberData()
        {
            return RulesHelper.GetTestDataCombined(
                new[] { new object[] { 0.000100F, 0.0000001F, true } },
                new[] { new object[] { 0.000100F, 0.000001F, true } },
                new[] { new object[] { 0.000100F, 0.00001F, true } },
                new[] { new object[] { 0.000100F, 0.0001F, true } },
                new[] { new object[] { 0.000100F, 0.001F, false } },
                new[] { new object[] { 0.000100F, 0.01F, false } },
                new[] { new object[] { 0.000100F, 0.1F, false } },
                new[] { new object[] { 0.000100F, 1F, false } });
        }

        [Theory]
        [MemberData(nameof(NonZero_WithTolerance_Should_CollectError_MemberData))]
        public void NonZero_WithTolerance_Should_CollectError(float model, float tolerance, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonZero(tolerance),
                shouldBeValid,
                MessageKey.Numbers.NonZero,
                Arg.Number("tolerance", tolerance));
        }

        [Theory]
        [MemberData(nameof(NonZero_WithTolerance_Should_CollectError_MemberData))]
        public void NonZero_WithTolerance_Should_CollectError_FromNullable(float model, float tolerance, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.NonZero(tolerance),
                shouldBeValid,
                MessageKey.Numbers.NonZero,
                Arg.Number("tolerance", tolerance));
        }

        public static IEnumerable<object[]> Positive_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Positive_Unsigned(Convert),
                NumbersTestData.Positive_Signed(Convert),
                new[] { new object[] { 0.000001F, true } },
                new[] { new object[] { -0.000001F, false } });
        }

        [Theory]
        [MemberData(nameof(Positive_Should_CollectError_Data))]
        public void Positive_Should_CollectError(float model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Positive(),
                shouldBeValid,
                MessageKey.Numbers.Positive);
        }

        [Theory]
        [MemberData(nameof(Positive_Should_CollectError_Data))]
        public void Positive_Should_CollectError_FromNullable(float model, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.Positive(),
                shouldBeValid,
                MessageKey.Numbers.Positive);
        }

        public static IEnumerable<object[]> NonPositive_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonPositive_Unsigned(Convert),
                NumbersTestData.NonPositive_Signed(Convert),
                new[] { new object[] { 0.000001F, false } },
                new[] { new object[] { -0.000001F, true } });
        }

        [Theory]
        [MemberData(nameof(NonPositive_Should_CollectError_Data))]
        public void NonPositive_Should_CollectError(float model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonPositive(),
                shouldBeValid,
                MessageKey.Numbers.NonPositive);
        }

        [Theory]
        [MemberData(nameof(NonPositive_Should_CollectError_Data))]
        public void NonPositive_Should_CollectError_FromNullable(float model, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.NonPositive(),
                shouldBeValid,
                MessageKey.Numbers.NonPositive);
        }

        public static IEnumerable<object[]> Negative_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Negative(Convert),
                new[] { new object[] { 0.000001F, false } },
                new[] { new object[] { -0.000001F, true } });
        }

        [Theory]
        [MemberData(nameof(Negative_Should_CollectError_Data))]
        public void Negative_Should_CollectError(float model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Negative(),
                shouldBeValid,
                MessageKey.Numbers.Negative);
        }

        [Theory]
        [MemberData(nameof(Negative_Should_CollectError_Data))]
        public void Negative_Should_CollectError_FromNullable(float model, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.Negative(),
                shouldBeValid,
                MessageKey.Numbers.Negative);
        }

        public static IEnumerable<object[]> NonNegative_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonNegative(Convert),
                new[] { new object[] { 0.000001F, true } },
                new[] { new object[] { -0.000001F, false } });
        }

        [Theory]
        [MemberData(nameof(NonNegative_Should_CollectError_Data))]
        public void NonNegative_Should_CollectError(float model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonNegative(),
                shouldBeValid,
                MessageKey.Numbers.NonNegative);
        }

        [Theory]
        [MemberData(nameof(NonNegative_Should_CollectError_Data))]
        public void NonNegative_Should_CollectError_FromNullable(float model, bool shouldBeValid)
        {
            Tester.TestSingleRule<float?>(
                model,
                m => m.NonNegative(),
                shouldBeValid,
                MessageKey.Numbers.NonNegative);
        }
    }
}
