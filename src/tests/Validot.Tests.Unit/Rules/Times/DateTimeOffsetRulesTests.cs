namespace Validot.Tests.Unit.Rules.Times
{
    using System;
    using System.Collections.Generic;

    using Validot.Testing;
    using Validot.Tests.Unit.Rules.Numbers;
    using Validot.Translations;

    using Xunit;

    public class DateTimeOffsetRulesTests
    {
        private static readonly Func<int, DateTimeOffset> Convert = ticks => new DateTimeOffset(ticks, TimeSpan.Zero);

        private static readonly TimesTestData.DateTimeConvert<DateTimeOffset> ConvertDateTimeOffset = (year, month, day, hour, minute, second, millisecond) => new DateTimeOffset(year, month, day, hour, minute, second, millisecond, TimeSpan.Zero);

        public static IEnumerable<object[]> EqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.EqualTo_Unsigned(Convert),
                NumbersTestData.EqualTo_Limits(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, new DateTimeOffset(1, TimeSpan.Zero)));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError(DateTimeOffset model, DateTimeOffset value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.Times.EqualTo,
                Arg.Time("value", value),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError_FromNullable(DateTimeOffset model, DateTimeOffset value, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTimeOffset?>(
                model,
                m => m.EqualTo(value),
                shouldBeValid,
                MessageKey.Times.EqualTo,
                Arg.Time("value", value),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        public static IEnumerable<object[]> NotEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NotEqualTo_Unsigned(Convert),
                NumbersTestData.NotEqualTo_Limits(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, new DateTimeOffset(1, TimeSpan.Zero)));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError(DateTimeOffset model, DateTimeOffset value, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.Times.NotEqualTo,
                Arg.Time("value", value),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError_FromNullable(DateTimeOffset model, DateTimeOffset value, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTimeOffset?>(
                model,
                m => m.NotEqualTo(value),
                shouldBeValid,
                MessageKey.Times.NotEqualTo,
                Arg.Time("value", value),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        public static IEnumerable<object[]> After_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.GreaterThan_Unsigned(Convert),
                NumbersTestData.GreaterThan_Limits(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, new DateTimeOffset(1, TimeSpan.Zero)));
        }

        [Theory]
        [MemberData(nameof(After_Should_CollectError_Data))]
        public void After_Should_CollectError(DateTimeOffset model, DateTimeOffset min, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.After(min),
                shouldBeValid,
                MessageKey.Times.After,
                Arg.Time("min", min),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        [Theory]
        [MemberData(nameof(After_Should_CollectError_Data))]
        public void After_Should_CollectError_FromNullable(DateTimeOffset model, DateTimeOffset min, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTimeOffset?>(
                model,
                m => m.After(min),
                shouldBeValid,
                MessageKey.Times.After,
                Arg.Time("min", min),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        public static IEnumerable<object[]> AfterOrEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.GreaterThanOrEqualTo_Unsigned(Convert),
                NumbersTestData.GreaterThanOrEqualTo_Limits(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, new DateTimeOffset(1, TimeSpan.Zero)));
        }

        [Theory]
        [MemberData(nameof(AfterOrEqualTo_Should_CollectError_Data))]
        public void AfterOrEqualTo_Should_CollectError(DateTimeOffset model, DateTimeOffset min, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.AfterOrEqualTo(min),
                shouldBeValid,
                MessageKey.Times.AfterOrEqualTo,
                Arg.Time("min", min),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        [Theory]
        [MemberData(nameof(AfterOrEqualTo_Should_CollectError_Data))]
        public void AfterOrEqualTo_Should_CollectError_FromNullable(DateTimeOffset model, DateTimeOffset min, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTimeOffset?>(
                model,
                m => m.AfterOrEqualTo(min),
                shouldBeValid,
                MessageKey.Times.AfterOrEqualTo,
                Arg.Time("min", min),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        public static IEnumerable<object[]> Before_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.LessThan_Unsigned(Convert),
                NumbersTestData.LessThan_Limits(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, new DateTimeOffset(1, TimeSpan.Zero)));
        }

        [Theory]
        [MemberData(nameof(Before_Should_CollectError_Data))]
        public void Before_Should_CollectError(DateTimeOffset model, DateTimeOffset max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Before(max),
                shouldBeValid,
                MessageKey.Times.Before,
                Arg.Time("max", max),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        [Theory]
        [MemberData(nameof(Before_Should_CollectError_Data))]
        public void Before_Should_CollectError_FromNullable(DateTimeOffset model, DateTimeOffset max, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTimeOffset?>(
                model,
                m => m.Before(max),
                shouldBeValid,
                MessageKey.Times.Before,
                Arg.Time("max", max),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        public static IEnumerable<object[]> BeforeOrEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.LessThanOrEqualTo_Unsigned(Convert),
                NumbersTestData.LessThanOrEqualTo_Limits(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, new DateTimeOffset(1, TimeSpan.Zero)));
        }

        [Theory]
        [MemberData(nameof(BeforeOrEqualTo_Should_CollectError_Data))]
        public void BeforeOrEqualTo_Should_CollectError(DateTimeOffset model, DateTimeOffset max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.BeforeOrEqualTo(max),
                shouldBeValid,
                MessageKey.Times.BeforeOrEqualTo,
                Arg.Time("max", max),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        [Theory]
        [MemberData(nameof(BeforeOrEqualTo_Should_CollectError_Data))]
        public void BeforeOrEqualTo_Should_CollectError_FromNullable(DateTimeOffset model, DateTimeOffset max, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTimeOffset?>(
                model,
                m => m.BeforeOrEqualTo(max),
                shouldBeValid,
                MessageKey.Times.BeforeOrEqualTo,
                Arg.Time("max", max),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        public static IEnumerable<object[]> Between_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Between_Unsigned(Convert),
                NumbersTestData.Between_Limits(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, new DateTimeOffset(1, TimeSpan.Zero)));
        }

        [Theory]
        [MemberData(nameof(Between_Should_CollectError_Data))]
        public void Between_Should_CollectError(DateTimeOffset min, DateTimeOffset model, DateTimeOffset max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Between(min, max),
                shouldBeValid,
                MessageKey.Times.Between,
                Arg.Time("min", min),
                Arg.Time("max", max),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        [Theory]
        [MemberData(nameof(Between_Should_CollectError_Data))]
        public void Between_Should_CollectError_FromNullable(DateTimeOffset min, DateTimeOffset model, DateTimeOffset max, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTimeOffset?>(
                model,
                m => m.Between(min, max),
                shouldBeValid,
                MessageKey.Times.Between,
                Arg.Time("min", min),
                Arg.Time("max", max),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        public static IEnumerable<object[]> BetweenOrEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.BetweenOrEqualTo_Unsigned(Convert),
                NumbersTestData.BetweenOrEqualTo_Limits(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, new DateTimeOffset(1, TimeSpan.Zero)));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_Data))]
        public void BetweenOrEqualTo_Should_CollectError(DateTimeOffset min, DateTimeOffset model, DateTimeOffset max, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.BetweenOrEqualTo(min, max),
                shouldBeValid,
                MessageKey.Times.BetweenOrEqualTo,
                Arg.Time("min", min),
                Arg.Time("max", max),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_Data))]
        public void BetweenOrEqualTo_Should_CollectError_FromNullable(DateTimeOffset min, DateTimeOffset model, DateTimeOffset max, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTimeOffset?>(
                model,
                m => m.BetweenOrEqualTo(min, max),
                shouldBeValid,
                MessageKey.Times.BetweenOrEqualTo,
                Arg.Time("min", min),
                Arg.Time("max", max),
                Arg.Enum("timeComparison", TimeComparison.All));
        }

        public class ComparisonModesTests
        {
            public static IEnumerable<object[]> EqualTo_Should_CollectError_When_TimeComparisonSet_Data()
            {
                return RulesHelper.GetTestDataCombined(
                    TimesTestData.EqualTo(ConvertDateTimeOffset));
            }

            [Theory]
            [MemberData(nameof(EqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void EqualTo_Should_CollectError_When_TimeComparisonSet(DateTimeOffset model, DateTimeOffset value, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule(
                    model,
                    m => m.EqualTo(value, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.EqualTo,
                    Arg.Time("value", value),
                    Arg.Enum("timeComparison", timeComparison));
            }

            [Theory]
            [MemberData(nameof(EqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void EqualTo_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTimeOffset model, DateTimeOffset value, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTimeOffset?>(
                    model,
                    m => m.EqualTo(value, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.EqualTo,
                    Arg.Time("value", value),
                    Arg.Enum("timeComparison", timeComparison));
            }

            public static IEnumerable<object[]> NotEqualTo_Should_CollectError_When_TimeComparisonSet_Data()
            {
                return RulesHelper.GetTestDataCombined(
                    TimesTestData.NotEqualTo(ConvertDateTimeOffset));
            }

            [Theory]
            [MemberData(nameof(NotEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void NotEqualTo_Should_CollectError_When_TimeComparisonSet(DateTimeOffset model, DateTimeOffset value, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule(
                    model,
                    m => m.NotEqualTo(value, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.NotEqualTo,
                    Arg.Time("value", value),
                    Arg.Enum("timeComparison", timeComparison));
            }

            [Theory]
            [MemberData(nameof(NotEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void NotEqualTo_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTimeOffset model, DateTimeOffset value, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTimeOffset?>(
                    model,
                    m => m.NotEqualTo(value, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.NotEqualTo,
                    Arg.Time("value", value),
                    Arg.Enum("timeComparison", timeComparison));
            }

            public static IEnumerable<object[]> After_Should_CollectError_When_TimeComparisonSet_Data()
            {
                return RulesHelper.GetTestDataCombined(
                    TimesTestData.After(ConvertDateTimeOffset));
            }

            [Theory]
            [MemberData(nameof(After_Should_CollectError_When_TimeComparisonSet_Data))]
            public void After_Should_CollectError_When_TimeComparisonSet(DateTimeOffset model, DateTimeOffset min, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule(
                    model,
                    m => m.After(min, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.After,
                    Arg.Time("min", min),
                    Arg.Enum("timeComparison", timeComparison));
            }

            [Theory]
            [MemberData(nameof(After_Should_CollectError_When_TimeComparisonSet_Data))]
            public void After_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTimeOffset model, DateTimeOffset min, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTimeOffset?>(
                    model,
                    m => m.After(min, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.After,
                    Arg.Time("min", min),
                    Arg.Enum("timeComparison", timeComparison));
            }

            public static IEnumerable<object[]> AfterOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data()
            {
                return RulesHelper.GetTestDataCombined(
                    TimesTestData.AfterOrEqualTo(ConvertDateTimeOffset));
            }

            [Theory]
            [MemberData(nameof(AfterOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void AfterOrEqualTo_Should_CollectError_When_TimeComparisonSet(DateTimeOffset model, DateTimeOffset min, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule(
                    model,
                    m => m.AfterOrEqualTo(min, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.AfterOrEqualTo,
                    Arg.Time("min", min),
                    Arg.Enum("timeComparison", timeComparison));
            }

            [Theory]
            [MemberData(nameof(AfterOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void AfterOrEqualTo_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTimeOffset model, DateTimeOffset min, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTimeOffset?>(
                    model,
                    m => m.AfterOrEqualTo(min, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.AfterOrEqualTo,
                    Arg.Time("min", min),
                    Arg.Enum("timeComparison", timeComparison));
            }

            public static IEnumerable<object[]> Before_Should_CollectError_When_TimeComparisonSet_Data()
            {
                return RulesHelper.GetTestDataCombined(
                    TimesTestData.Before(ConvertDateTimeOffset));
            }

            [Theory]
            [MemberData(nameof(Before_Should_CollectError_When_TimeComparisonSet_Data))]
            public void Before_Should_CollectError_When_TimeComparisonSet(DateTimeOffset model, DateTimeOffset max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule(
                    model,
                    m => m.Before(max, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.Before,
                    Arg.Time("max", max),
                    Arg.Enum("timeComparison", timeComparison));
            }

            [Theory]
            [MemberData(nameof(Before_Should_CollectError_When_TimeComparisonSet_Data))]
            public void Before_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTimeOffset model, DateTimeOffset max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTimeOffset?>(
                    model,
                    m => m.Before(max, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.Before,
                    Arg.Time("max", max),
                    Arg.Enum("timeComparison", timeComparison));
            }

            public static IEnumerable<object[]> BeforeOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data()
            {
                return RulesHelper.GetTestDataCombined(
                    TimesTestData.BeforeOrEqualTo(ConvertDateTimeOffset));
            }

            [Theory]
            [MemberData(nameof(BeforeOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void BeforeOrEqualTo_Should_CollectError_When_TimeComparisonSet(DateTimeOffset model, DateTimeOffset max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule(
                    model,
                    m => m.BeforeOrEqualTo(max, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.BeforeOrEqualTo,
                    Arg.Time("max", max),
                    Arg.Enum("timeComparison", timeComparison));
            }

            [Theory]
            [MemberData(nameof(BeforeOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void BeforeOrEqualTo_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTimeOffset model, DateTimeOffset max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTimeOffset?>(
                    model,
                    m => m.BeforeOrEqualTo(max, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.BeforeOrEqualTo,
                    Arg.Time("max", max),
                    Arg.Enum("timeComparison", timeComparison));
            }

            public static IEnumerable<object[]> Between_Should_CollectError_When_TimeComparisonSet_Data()
            {
                return RulesHelper.GetTestDataCombined(
                    TimesTestData.Between(ConvertDateTimeOffset));
            }

            [Theory]
            [MemberData(nameof(Between_Should_CollectError_When_TimeComparisonSet_Data))]
            public void Between_Should_CollectError_When_TimeComparisonSet(DateTimeOffset min, DateTimeOffset model, DateTimeOffset max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule(
                    model,
                    m => m.Between(min, max, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.Between,
                    Arg.Time("min", min),
                    Arg.Time("max", max),
                    Arg.Enum("timeComparison", timeComparison));
            }

            [Theory]
            [MemberData(nameof(Between_Should_CollectError_When_TimeComparisonSet_Data))]
            public void Between_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTimeOffset min, DateTimeOffset model, DateTimeOffset max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTimeOffset?>(
                    model,
                    m => m.Between(min, max, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.Between,
                    Arg.Time("min", min),
                    Arg.Time("max", max),
                    Arg.Enum("timeComparison", timeComparison));
            }

            public static IEnumerable<object[]> BetweenOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data()
            {
                return RulesHelper.GetTestDataCombined(
                    TimesTestData.BetweenOrEqualTo(ConvertDateTimeOffset));
            }

            [Theory]
            [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void BetweenOrEqualTo_Should_CollectError_When_TimeComparisonSet(DateTimeOffset min, DateTimeOffset model, DateTimeOffset max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule(
                    model,
                    m => m.BetweenOrEqualTo(min, max, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.BetweenOrEqualTo,
                    Arg.Time("min", min),
                    Arg.Time("max", max),
                    Arg.Enum("timeComparison", timeComparison));
            }

            [Theory]
            [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void BetweenOrEqualTo_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTimeOffset min, DateTimeOffset model, DateTimeOffset max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTimeOffset?>(
                    model,
                    m => m.BetweenOrEqualTo(min, max, timeComparison),
                    shouldBeValid,
                    MessageKey.Times.BetweenOrEqualTo,
                    Arg.Time("min", min),
                    Arg.Time("max", max),
                    Arg.Enum("timeComparison", timeComparison));
            }
        }
    }
}
