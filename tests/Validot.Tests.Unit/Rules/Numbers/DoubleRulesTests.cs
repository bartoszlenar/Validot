namespace Validot.Tests.Unit.Rules.Numbers
{
    using System;
    using System.Collections.Generic;

    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class DoubleRulesTests
    {
        private static readonly Func<int, double> Convert = i => i;

        public static IEnumerable<object[]> EqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.EqualTo_Unsigned(Convert),
                NumbersTestData.EqualTo_Signed(Convert),
                NumbersTestData.EqualTo_Limits(double.MinValue, double.MaxValue, 0),
                new[] { new object[] { 0.999999D, 0D, false } },
                new[] { new object[] { 1.000001D, 0D, false } },
                new[] { new object[] { 1.123456D, 1.123456D, true } });
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError(double model, double value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.EqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", 0.0000001D));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError_FromNullable(double model, double value, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.EqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", 0.0000001D));
        }

        public static IEnumerable<object[]> EqualTo_WithTolerance_Should_CollectError_MemberData()
        {
            return RulesHelper.GetTestDataCombined(
                new[] { new object[] { 1.000100D, 1.000199D, 0.0000001D, false } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.000001D, false } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.00001D, false } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.0001D, true } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.001D, true } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.01D, true } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.1D, true } },
                new[] { new object[] { 1.000100D, 1.000199D, 1D, true } });
        }

        [Theory]
        [MemberData(nameof(EqualTo_WithTolerance_Should_CollectError_MemberData))]
        public void EqualTo_WithTolerance_Should_CollectError(double model, double value, double tolerance, bool expectedIsValid)
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
        public void EqualTo_WithTolerance_Should_CollectError_FromNullable(double model, double value, double tolerance, bool expectedIsValid)
        {
            Tester.TestSingleRule<double?>(
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
                NumbersTestData.NotEqualTo_Limits(double.MinValue, double.MaxValue, 0),
                new[] { new object[] { 0.999999D, 0D, true } },
                new[] { new object[] { 1.000001D, 0D, true } },
                new[] { new object[] { 1.123456D, 1.123456D, false } });
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError(double model, double value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.NotEqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", 0.0000001D));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError_FromNullable(double model, double value, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.NotEqualTo,
                Arg.Number("value", value),
                Arg.Number("tolerance", 0.0000001D));
        }

        public static IEnumerable<object[]> NotEqualTo_WithTolerance_Should_CollectError_MemberData()
        {
            return RulesHelper.GetTestDataCombined(
                new[] { new object[] { 1.000100D, 1.000199D, 0.0000001D, true } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.000001D, true } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.00001D, true } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.0001D, false } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.001D, false } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.01D, false } },
                new[] { new object[] { 1.000100D, 1.000199D, 0.1D, false } },
                new[] { new object[] { 1.000100D, 1.000199D, 1D, false } });
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_WithTolerance_Should_CollectError_MemberData))]
        public void NotEqualTo_WithTolerance_Should_CollectError(double model, double value, double tolerance, bool shouldBeValid)
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
        public void NotEqualTo_WithTolerance_Should_CollectError_FromNullable(double model, double value, double tolerance, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
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
                NumbersTestData.GreaterThan_Limits(double.MinValue, double.MaxValue, 0),
                new[] { new object[] { 0.999999D, 1D, false } },
                new[] { new object[] { 1.000001D, 1D, true } },
                new[] { new object[] { 0.999999D, 0.999999D, false } },
                new[] { new object[] { 1D, 1.000001D, false } },
                new[] { new object[] { 1.000001D, 1.000001D, false } });
        }

        [Theory]
        [MemberData(nameof(GreaterThan_Should_CollectError_Data))]
        public void GreaterThan_Should_CollectError(double model, double min, bool shouldBeValid)
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
        public void GreaterThan_Should_CollectError_FromNullable(double model, double min, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
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
                NumbersTestData.LessThan_Limits(double.MinValue, double.MaxValue, 0),
                new[] { new object[] { 0.999999D, 1D, true } },
                new[] { new object[] { 1.000001D, 1D, false } },
                new[] { new object[] { 0.999999D, 0.999999D, false } },
                new[] { new object[] { 1D, 1.000001D, true } },
                new[] { new object[] { 1.000001D, 1.000001D, false } });
        }

        [Theory]
        [MemberData(nameof(LessThan_Should_CollectError_Data))]
        public void LessThan_Should_CollectError(double model, double max, bool shouldBeValid)
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
        public void LessThan_Should_CollectError_FromNullable(double model, double max, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
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
                NumbersTestData.Between_Limits(double.MinValue, double.MaxValue, 0),
                new[] { new object[] { 0.999999D, 1, 1.000001D, true } },
                new[] { new object[] { 0.999999D, 0.999999D, 1.000001D, false } },
                new[] { new object[] { 0.999999D, 1.000001D, 1.000001D, false } });
        }

        [Theory]
        [MemberData(nameof(Between_Should_CollectError_Data))]
        public void Between_Should_CollectError(double min, double model, double max, bool shouldBeValid)
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
        public void Between_Should_CollectError_FromNullable(double min, double model, double max, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
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
                NumbersTestData.Between_InvalidRange(Convert, double.MinValue, double.MaxValue));
        }

        [Theory]
        [MemberData(nameof(Between_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void Between_Should_ThrowException_When_MinLargerThanMax(double min, double max)
        {
            Tester.TestExceptionOnInit<double>(
                s => s.Between(min, max),
                typeof(ArgumentException));
        }

        [Theory]
        [MemberData(nameof(Between_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void Between_Should_ThrowException_When_MinLargerThanMax_FromNullable(double min, double max)
        {
            Tester.TestExceptionOnInit<double?>(
                s => s.Between(min, max),
                typeof(ArgumentException));
        }

        public static IEnumerable<object[]> NonZero_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonZero_Unsigned(Convert),
                NumbersTestData.NonZero_Signed(Convert),
                NumbersTestData.NonZero_Signed_Limits(double.MinValue, double.MaxValue));
        }

        [Theory]
        [MemberData(nameof(NonZero_Should_CollectError_Data))]
        public void NonZero_Should_CollectError(double model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonZero(),
                shouldBeValid,
                MessageKey.Numbers.NonZero,
                Arg.Number("tolerance", 0.0000001D));
        }

        [Theory]
        [MemberData(nameof(NonZero_Should_CollectError_Data))]
        public void NonZero_Should_CollectError_FromNullable(double model, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
                model,
                m => m.NonZero(),
                shouldBeValid,
                MessageKey.Numbers.NonZero,
                Arg.Number("tolerance", 0.0000001D));
        }

        public static IEnumerable<object[]> NonZero_WithTolerance_Should_CollectError_MemberData()
        {
            return RulesHelper.GetTestDataCombined(
                new[] { new object[] { 0.000100D, 0.0000001D, true } },
                new[] { new object[] { 0.000100D, 0.000001D, true } },
                new[] { new object[] { 0.000100D, 0.00001D, true } },
                new[] { new object[] { 0.000100D, 0.0001D, true } },
                new[] { new object[] { 0.000100D, 0.001D, false } },
                new[] { new object[] { 0.000100D, 0.01D, false } },
                new[] { new object[] { 0.000100D, 0.1D, false } },
                new[] { new object[] { 0.000100D, 1D, false } });
        }

        [Theory]
        [MemberData(nameof(NonZero_WithTolerance_Should_CollectError_MemberData))]
        public void NonZero_WithTolerance_Should_CollectError(double model, double tolerance, bool shouldBeValid)
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
        public void NonZero_WithTolerance_Should_CollectError_FromNullable(double model, double tolerance, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
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
                new[] { new object[] { 0.000001D, true } },
                new[] { new object[] { -0.000001D, false } });
        }

        [Theory]
        [MemberData(nameof(Positive_Should_CollectError_Data))]
        public void Positive_Should_CollectError(double model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Positive(),
                shouldBeValid,
                MessageKey.Numbers.Positive);
        }

        [Theory]
        [MemberData(nameof(Positive_Should_CollectError_Data))]
        public void Positive_Should_CollectError_FromNullable(double model, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
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
                new[] { new object[] { 0.000001D, false } },
                new[] { new object[] { -0.000001D, true } });
        }

        [Theory]
        [MemberData(nameof(NonPositive_Should_CollectError_Data))]
        public void NonPositive_Should_CollectError(double model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonPositive(),
                shouldBeValid,
                MessageKey.Numbers.NonPositive);
        }

        [Theory]
        [MemberData(nameof(NonPositive_Should_CollectError_Data))]
        public void NonPositive_Should_CollectError_FromNullable(double model, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
                model,
                m => m.NonPositive(),
                shouldBeValid,
                MessageKey.Numbers.NonPositive);
        }

        public static IEnumerable<object[]> Negative_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Negative(Convert),
                new[] { new object[] { 0.000001D, false } },
                new[] { new object[] { -0.000001D, true } });
        }

        [Theory]
        [MemberData(nameof(Negative_Should_CollectError_Data))]
        public void Negative_Should_CollectError(double model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Negative(),
                shouldBeValid,
                MessageKey.Numbers.Negative);
        }

        [Theory]
        [MemberData(nameof(Negative_Should_CollectError_Data))]
        public void Negative_Should_CollectError_FromNullable(double model, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
                model,
                m => m.Negative(),
                shouldBeValid,
                MessageKey.Numbers.Negative);
        }

        public static IEnumerable<object[]> NonNegative_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonNegative(Convert),
                new[] { new object[] { 0.000001D, true } },
                new[] { new object[] { -0.000001D, false } });
        }

        [Theory]
        [MemberData(nameof(NonNegative_Should_CollectError_Data))]
        public void NonNegative_Should_CollectError(double model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonNegative(),
                shouldBeValid,
                MessageKey.Numbers.NonNegative);
        }

        [Theory]
        [MemberData(nameof(NonNegative_Should_CollectError_Data))]
        public void NonNegative_Should_CollectError_FromNullable(double model, bool shouldBeValid)
        {
            Tester.TestSingleRule<double?>(
                model,
                m => m.NonNegative(),
                shouldBeValid,
                MessageKey.Numbers.NonNegative);
        }
    }
}
