namespace Validot.Tests.Unit.Rules.Numbers
{
    using System;
    using System.Collections.Generic;

    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class ShortRulesTests
    {
        private static readonly Func<int, short> Convert = i => (short)i;

        public static IEnumerable<object[]> EqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.EqualTo_Unsigned(Convert),
                NumbersTestData.EqualTo_Signed(Convert),
                NumbersTestData.EqualTo_Limits(short.MinValue, short.MaxValue, 0));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError(short model, short value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.EqualTo,
                Arg.Number("value", value));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError_FromNullable(short model, short value, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.EqualTo,
                Arg.Number("value", value));
        }

        public static IEnumerable<object[]> NotEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NotEqualTo_Unsigned(Convert),
                NumbersTestData.NotEqualTo_Signed(Convert),
                NumbersTestData.NotEqualTo_Limits(short.MinValue, short.MaxValue, 0));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError(short model, short value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.NotEqualTo,
                Arg.Number("value", value));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError_FromNullable(short model, short value, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.Numbers.NotEqualTo,
                Arg.Number("value", value));
        }

        public static IEnumerable<object[]> GreaterThan_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.GreaterThan_Unsigned(Convert),
                NumbersTestData.GreaterThan_Signed(Convert),
                NumbersTestData.GreaterThan_Limits(short.MinValue, short.MaxValue, 0));
        }

        [Theory]
        [MemberData(nameof(GreaterThan_Should_CollectError_Data))]
        public void GreaterThan_Should_CollectError(short model, short min, bool shouldBeValid)
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
        public void GreaterThan_Should_CollectError_FromNullable(short model, short min, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.GreaterThan(min),
                shouldBeValid,
                MessageKey.Numbers.GreaterThan,
                Arg.Number("min", min));
        }

        public static IEnumerable<object[]> GreaterThanOrEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.GreaterThanOrEqualTo_Unsigned(Convert),
                NumbersTestData.GreaterThanOrEqualTo_Signed(Convert),
                NumbersTestData.GreaterThanOrEqualTo_Limits(short.MinValue, short.MaxValue, 0));
        }

        [Theory]
        [MemberData(nameof(GreaterThanOrEqualTo_Should_CollectError_Data))]
        public void GreaterThanOrEqualTo_Should_CollectError(short model, short min, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.GreaterThanOrEqualTo(min),
                shouldBeValid,
                MessageKey.Numbers.GreaterThanOrEqualTo,
                Arg.Number("min", min));
        }

        [Theory]
        [MemberData(nameof(GreaterThanOrEqualTo_Should_CollectError_Data))]
        public void GreaterThanOrEqualTo_Should_CollectError_FromNullable(short model, short min, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.GreaterThanOrEqualTo(min),
                shouldBeValid,
                MessageKey.Numbers.GreaterThanOrEqualTo,
                Arg.Number("min", min));
        }

        public static IEnumerable<object[]> LessThan_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.LessThan_Unsigned(Convert),
                NumbersTestData.LessThan_Signed(Convert),
                NumbersTestData.LessThan_Limits(short.MinValue, short.MaxValue, 0));
        }

        [Theory]
        [MemberData(nameof(LessThan_Should_CollectError_Data))]
        public void LessThan_Should_CollectError(short model, short max, bool shouldBeValid)
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
        public void LessThan_Should_CollectError_FromNullable(short model, short max, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.LessThan(max),
                shouldBeValid,
                MessageKey.Numbers.LessThan,
                Arg.Number("max", max));
        }

        public static IEnumerable<object[]> LessThanOrEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.LessThanOrEqualTo_Unsigned(Convert),
                NumbersTestData.LessThanOrEqualTo_Signed(Convert),
                NumbersTestData.LessThanOrEqualTo_Limits(short.MinValue, short.MaxValue, 0));
        }

        [Theory]
        [MemberData(nameof(LessThanOrEqualTo_Should_CollectError_Data))]
        public void LessThanOrEqualTo_Should_CollectError(short model, short max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.LessThanOrEqualTo(max),
                shouldBeValid,
                MessageKey.Numbers.LessThanOrEqualTo,
                Arg.Number("max", max));
        }

        [Theory]
        [MemberData(nameof(LessThanOrEqualTo_Should_CollectError_Data))]
        public void LessThanOrEqualTo_Should_CollectError_FromNullable(short model, short max, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.LessThanOrEqualTo(max),
                shouldBeValid,
                MessageKey.Numbers.LessThanOrEqualTo,
                Arg.Number("max", max));
        }

        public static IEnumerable<object[]> Between_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Between_Unsigned(Convert),
                NumbersTestData.Between_Signed(Convert),
                NumbersTestData.Between_Limits(short.MinValue, short.MaxValue, 0));
        }

        [Theory]
        [MemberData(nameof(Between_Should_CollectError_Data))]
        public void Between_Should_CollectError(short min, short model, short max, bool shouldBeValid)
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
        public void Between_Should_CollectError_FromNullable(short min, short model, short max, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
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
                NumbersTestData.Between_InvalidRange(Convert, short.MinValue, short.MaxValue));
        }

        [Theory]
        [MemberData(nameof(Between_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void Between_Should_ThrowException_When_MinLargerThanMax(short min, short max)
        {
            Tester.TestExceptionOnInit<short>(
                s => s.Between(min, max),
                typeof(ArgumentException));
        }

        [Theory]
        [MemberData(nameof(Between_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void Between_Should_ThrowException_When_MinLargerThanMax_FromNullable(short min, short max)
        {
            Tester.TestExceptionOnInit<short?>(
                s => s.Between(min, max),
                typeof(ArgumentException));
        }

        public static IEnumerable<object[]> BetweenOrEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.BetweenOrEqualTo_Unsigned(Convert),
                NumbersTestData.BetweenOrEqualTo_Signed(Convert),
                NumbersTestData.BetweenOrEqualTo_Limits(short.MinValue, short.MaxValue, 0));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_Data))]
        public void BetweenOrEqualTo_Should_CollectError(short min, short model, short max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.BetweenOrEqualTo(min, max),
                shouldBeValid,
                MessageKey.Numbers.BetweenOrEqualTo,
                Arg.Number("min", min),
                Arg.Number("max", max));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_Data))]
        public void BetweenOrEqualTo_Should_CollectError_FromNullable(short min, short model, short max, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.BetweenOrEqualTo(min, max),
                shouldBeValid,
                MessageKey.Numbers.BetweenOrEqualTo,
                Arg.Number("min", min),
                Arg.Number("max", max));
        }

        public static IEnumerable<object[]> BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Between_InvalidRange(Convert, short.MinValue, short.MaxValue));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax(short min, short max)
        {
            Tester.TestExceptionOnInit<short>(
                s => s.BetweenOrEqualTo(min, max),
                typeof(ArgumentException));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_FromNullable(short min, short max)
        {
            Tester.TestExceptionOnInit<short?>(
                s => s.BetweenOrEqualTo(min, max),
                typeof(ArgumentException));
        }

        public static IEnumerable<object[]> NonZero_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonZero_Unsigned(Convert),
                NumbersTestData.NonZero_Signed(Convert),
                NumbersTestData.NonZero_Signed_Limits(short.MinValue, short.MaxValue));
        }

        [Theory]
        [MemberData(nameof(NonZero_Should_CollectError_Data))]
        public void NonZero_Should_CollectError(short model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonZero(),
                shouldBeValid,
                MessageKey.Numbers.NonZero);
        }

        [Theory]
        [MemberData(nameof(NonZero_Should_CollectError_Data))]
        public void NonZero_Should_CollectError_FromNullable(short model, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.NonZero(),
                shouldBeValid,
                MessageKey.Numbers.NonZero);
        }

        public static IEnumerable<object[]> Positive_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Positive_Unsigned(Convert),
                NumbersTestData.Positive_Signed(Convert));
        }

        [Theory]
        [MemberData(nameof(Positive_Should_CollectError_Data))]
        public void Positive_Should_CollectError(short model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Positive(),
                shouldBeValid,
                MessageKey.Numbers.Positive);
        }

        [Theory]
        [MemberData(nameof(Positive_Should_CollectError_Data))]
        public void Positive_Should_CollectError_FromNullable(short model, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.Positive(),
                shouldBeValid,
                MessageKey.Numbers.Positive);
        }

        public static IEnumerable<object[]> NonPositive_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonPositive_Unsigned(Convert),
                NumbersTestData.NonPositive_Signed(Convert));
        }

        [Theory]
        [MemberData(nameof(NonPositive_Should_CollectError_Data))]
        public void NonPositive_Should_CollectError(short model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonPositive(),
                shouldBeValid,
                MessageKey.Numbers.NonPositive);
        }

        [Theory]
        [MemberData(nameof(NonPositive_Should_CollectError_Data))]
        public void NonPositive_Should_CollectError_FromNullable(short model, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.NonPositive(),
                shouldBeValid,
                MessageKey.Numbers.NonPositive);
        }

        public static IEnumerable<object[]> Negative_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Negative(Convert));
        }

        [Theory]
        [MemberData(nameof(Negative_Should_CollectError_Data))]
        public void Negative_Should_CollectError(short model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Negative(),
                shouldBeValid,
                MessageKey.Numbers.Negative);
        }

        [Theory]
        [MemberData(nameof(Negative_Should_CollectError_Data))]
        public void Negative_Should_CollectError_FromNullable(short model, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.Negative(),
                shouldBeValid,
                MessageKey.Numbers.Negative);
        }

        public static IEnumerable<object[]> NonNegative_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonNegative(Convert));
        }

        [Theory]
        [MemberData(nameof(NonNegative_Should_CollectError_Data))]
        public void NonNegative_Should_CollectError(short model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonNegative(),
                shouldBeValid,
                MessageKey.Numbers.NonNegative);
        }

        [Theory]
        [MemberData(nameof(NonNegative_Should_CollectError_Data))]
        public void NonNegative_Should_CollectError_FromNullable(short model, bool shouldBeValid)
        {
            Tester.TestSingleRule<short?>(
                model,
                m => m.NonNegative(),
                shouldBeValid,
                MessageKey.Numbers.NonNegative);
        }
    }
}
