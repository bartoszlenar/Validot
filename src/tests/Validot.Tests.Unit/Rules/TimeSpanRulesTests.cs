namespace Validot.Tests.Unit.Rules
{
    using System;
    using System.Collections.Generic;

    using Validot;
    using Validot.Testing;
    using Validot.Tests.Unit.Rules.Numbers;
    using Validot.Translations;

    using Xunit;

    public class TimeSpanRulesTests
    {
        private static readonly Func<int, TimeSpan> Convert = i => new TimeSpan(i);

        public static IEnumerable<object[]> EqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.EqualTo_Unsigned(Convert),
                NumbersTestData.EqualTo_Signed(Convert),
                NumbersTestData.EqualTo_Limits(TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError(TimeSpan model, TimeSpan value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.TimeSpanType.EqualTo,
                Arg.Time("value", value));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError_FromNullable(TimeSpan model, TimeSpan value, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.TimeSpanType.EqualTo,
                Arg.Time("value", value));
        }

        public static IEnumerable<object[]> NotEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NotEqualTo_Unsigned(Convert),
                NumbersTestData.NotEqualTo_Signed(Convert),
                NumbersTestData.NotEqualTo_Limits(TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError(TimeSpan model, TimeSpan value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.TimeSpanType.NotEqualTo,
                Arg.Time("value", value));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError_FromNullable(TimeSpan model, TimeSpan value, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.TimeSpanType.NotEqualTo,
                Arg.Time("value", value));
        }

        public static IEnumerable<object[]> GreaterThan_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.GreaterThan_Unsigned(Convert),
                NumbersTestData.GreaterThan_Signed(Convert),
                NumbersTestData.GreaterThan_Limits(TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero));
        }

        [Theory]
        [MemberData(nameof(GreaterThan_Should_CollectError_Data))]
        public void GreaterThan_Should_CollectError(TimeSpan model, TimeSpan min, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.GreaterThan(min),
                shouldBeValid,
                MessageKey.TimeSpanType.GreaterThan,
                Arg.Time("min", min));
        }

        [Theory]
        [MemberData(nameof(GreaterThan_Should_CollectError_Data))]
        public void GreaterThan_Should_CollectError_FromNullable(TimeSpan model, TimeSpan min, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.GreaterThan(min),
                shouldBeValid,
                MessageKey.TimeSpanType.GreaterThan,
                Arg.Time("min", min));
        }

        public static IEnumerable<object[]> GreaterThanOrEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.GreaterThanOrEqualTo_Unsigned(Convert),
                NumbersTestData.GreaterThanOrEqualTo_Signed(Convert),
                NumbersTestData.GreaterThanOrEqualTo_Limits(TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero));
        }

        [Theory]
        [MemberData(nameof(GreaterThanOrEqualTo_Should_CollectError_Data))]
        public void GreaterThanOrEqualTo_Should_CollectError(TimeSpan model, TimeSpan min, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.GreaterThanOrEqualTo(min),
                shouldBeValid,
                MessageKey.TimeSpanType.GreaterThanOrEqualTo,
                Arg.Time("min", min));
        }

        [Theory]
        [MemberData(nameof(GreaterThanOrEqualTo_Should_CollectError_Data))]
        public void GreaterThanOrEqualTo_Should_CollectError_FromNullable(TimeSpan model, TimeSpan min, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.GreaterThanOrEqualTo(min),
                shouldBeValid,
                MessageKey.TimeSpanType.GreaterThanOrEqualTo,
                Arg.Time("min", min));
        }

        public static IEnumerable<object[]> LessThan_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.LessThan_Unsigned(Convert),
                NumbersTestData.LessThan_Signed(Convert),
                NumbersTestData.LessThan_Limits(TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero));
        }

        [Theory]
        [MemberData(nameof(LessThan_Should_CollectError_Data))]
        public void LessThan_Should_CollectError(TimeSpan model, TimeSpan max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.LessThan(max),
                shouldBeValid,
                MessageKey.TimeSpanType.LessThan,
                Arg.Time("max", max));
        }

        [Theory]
        [MemberData(nameof(LessThan_Should_CollectError_Data))]
        public void LessThan_Should_CollectError_FromNullable(TimeSpan model, TimeSpan max, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.LessThan(max),
                shouldBeValid,
                MessageKey.TimeSpanType.LessThan,
                Arg.Time("max", max));
        }

        public static IEnumerable<object[]> LessThanOrEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.LessThanOrEqualTo_Unsigned(Convert),
                NumbersTestData.LessThanOrEqualTo_Signed(Convert),
                NumbersTestData.LessThanOrEqualTo_Limits(TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero));
        }

        [Theory]
        [MemberData(nameof(LessThanOrEqualTo_Should_CollectError_Data))]
        public void LessThanOrEqualTo_Should_CollectError(TimeSpan model, TimeSpan max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.LessThanOrEqualTo(max),
                shouldBeValid,
                MessageKey.TimeSpanType.LessThanOrEqualTo,
                Arg.Time("max", max));
        }

        [Theory]
        [MemberData(nameof(LessThanOrEqualTo_Should_CollectError_Data))]
        public void LessThanOrEqualTo_Should_CollectError_FromNullable(TimeSpan model, TimeSpan max, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.LessThanOrEqualTo(max),
                shouldBeValid,
                MessageKey.TimeSpanType.LessThanOrEqualTo,
                Arg.Time("max", max));
        }

        public static IEnumerable<object[]> Between_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Between_Unsigned(Convert),
                NumbersTestData.Between_Signed(Convert),
                NumbersTestData.Between_Limits(TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero));
        }

        [Theory]
        [MemberData(nameof(Between_Should_CollectError_Data))]
        public void Between_Should_CollectError(TimeSpan min, TimeSpan model, TimeSpan max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Between(min, max),
                shouldBeValid,
                MessageKey.TimeSpanType.Between,
                Arg.Time("min", min),
                Arg.Time("max", max));
        }

        [Theory]
        [MemberData(nameof(Between_Should_CollectError_Data))]
        public void Between_Should_CollectError_FromNullable(TimeSpan min, TimeSpan model, TimeSpan max, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.Between(min, max),
                shouldBeValid,
                MessageKey.TimeSpanType.Between,
                Arg.Time("min", min),
                Arg.Time("max", max));
        }

        public static IEnumerable<object[]> Between_Should_ThrowException_When_MinLargerThanMax_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Between_InvalidRange(Convert, TimeSpan.MinValue, TimeSpan.MaxValue));
        }

        [Theory]
        [MemberData(nameof(Between_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void Between_Should_ThrowException_When_MinLargerThanMax(TimeSpan min, TimeSpan max)
        {
            Tester.TestExceptionOnInit<TimeSpan>(
                s => s.Between(min, max),
                typeof(ArgumentException));
        }

        [Theory]
        [MemberData(nameof(Between_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void Between_Should_ThrowException_When_MinLargerThanMax_FromNullable(TimeSpan min, TimeSpan max)
        {
            Tester.TestExceptionOnInit<TimeSpan?>(
                s => s.Between(min, max),
                typeof(ArgumentException));
        }

        public static IEnumerable<object[]> BetweenOrEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.BetweenOrEqualTo_Unsigned(Convert),
                NumbersTestData.BetweenOrEqualTo_Signed(Convert),
                NumbersTestData.BetweenOrEqualTo_Limits(TimeSpan.MinValue, TimeSpan.MaxValue, TimeSpan.Zero));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_Data))]
        public void BetweenOrEqualTo_Should_CollectError(TimeSpan min, TimeSpan model, TimeSpan max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.BetweenOrEqualTo(min, max),
                shouldBeValid,
                MessageKey.TimeSpanType.BetweenOrEqualTo,
                Arg.Time("min", min),
                Arg.Time("max", max));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_Data))]
        public void BetweenOrEqualTo_Should_CollectError_FromNullable(TimeSpan min, TimeSpan model, TimeSpan max, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.BetweenOrEqualTo(min, max),
                shouldBeValid,
                MessageKey.TimeSpanType.BetweenOrEqualTo,
                Arg.Time("min", min),
                Arg.Time("max", max));
        }

        public static IEnumerable<object[]> BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Between_InvalidRange(Convert, TimeSpan.MinValue, TimeSpan.MaxValue));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax(TimeSpan min, TimeSpan max)
        {
            Tester.TestExceptionOnInit<TimeSpan>(
                s => s.BetweenOrEqualTo(min, max),
                typeof(ArgumentException));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_FromNullable(TimeSpan min, TimeSpan max)
        {
            Tester.TestExceptionOnInit<TimeSpan?>(
                s => s.BetweenOrEqualTo(min, max),
                typeof(ArgumentException));
        }

        public static IEnumerable<object[]> NonZero_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonZero_Unsigned(Convert),
                NumbersTestData.NonZero_Unsigned_Limits(TimeSpan.MaxValue),
                NumbersTestData.NonZero_Signed(Convert),
                NumbersTestData.NonZero_Signed_Limits(TimeSpan.MinValue, TimeSpan.MaxValue));
        }

        [Theory]
        [MemberData(nameof(NonZero_Should_CollectError_Data))]
        public void NonZero_Should_CollectError(TimeSpan model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonZero(),
                shouldBeValid,
                MessageKey.TimeSpanType.NonZero);
        }

        [Theory]
        [MemberData(nameof(NonZero_Should_CollectError_Data))]
        public void NonZero_Should_CollectError_FromNullable(TimeSpan model, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.NonZero(),
                shouldBeValid,
                MessageKey.TimeSpanType.NonZero);
        }

        public static IEnumerable<object[]> Positive_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Positive_Unsigned(Convert),
                NumbersTestData.Positive_Signed(Convert));
        }

        [Theory]
        [MemberData(nameof(Positive_Should_CollectError_Data))]
        public void Positive_Should_CollectError(TimeSpan model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Positive(),
                shouldBeValid,
                MessageKey.TimeSpanType.Positive);
        }

        [Theory]
        [MemberData(nameof(Positive_Should_CollectError_Data))]
        public void Positive_Should_CollectError_FromNullable(TimeSpan model, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.Positive(),
                shouldBeValid,
                MessageKey.TimeSpanType.Positive);
        }

        public static IEnumerable<object[]> NonPositive_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonPositive_Unsigned(Convert),
                NumbersTestData.NonPositive_Signed(Convert));
        }

        [Theory]
        [MemberData(nameof(NonPositive_Should_CollectError_Data))]
        public void NonPositive_Should_CollectError(TimeSpan model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonPositive(),
                shouldBeValid,
                MessageKey.TimeSpanType.NonPositive);
        }

        [Theory]
        [MemberData(nameof(NonPositive_Should_CollectError_Data))]
        public void NonPositive_Should_CollectError_FromNullable(TimeSpan model, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.NonPositive(),
                shouldBeValid,
                MessageKey.TimeSpanType.NonPositive);
        }

        public static IEnumerable<object[]> Negative_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Negative(Convert));
        }

        [Theory]
        [MemberData(nameof(Negative_Should_CollectError_Data))]
        public void Negative_Should_CollectError(TimeSpan model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Negative(),
                shouldBeValid,
                MessageKey.TimeSpanType.Negative);
        }

        [Theory]
        [MemberData(nameof(Negative_Should_CollectError_Data))]
        public void Negative_Should_CollectError_FromNullable(TimeSpan model, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.Negative(),
                shouldBeValid,
                MessageKey.TimeSpanType.Negative);
        }

        public static IEnumerable<object[]> NonNegative_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonNegative(Convert));
        }

        [Theory]
        [MemberData(nameof(NonNegative_Should_CollectError_Data))]
        public void NonNegative_Should_CollectError(TimeSpan model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonNegative(),
                shouldBeValid,
                MessageKey.TimeSpanType.NonNegative);
        }

        [Theory]
        [MemberData(nameof(NonNegative_Should_CollectError_Data))]
        public void NonNegative_Should_CollectError_FromNullable(TimeSpan model, bool shouldBeValid)
        {
            Tester.TestSingleRule<TimeSpan?>(
                model,
                m => m.NonNegative(),
                shouldBeValid,
                MessageKey.TimeSpanType.NonNegative);
        }
    }
}
