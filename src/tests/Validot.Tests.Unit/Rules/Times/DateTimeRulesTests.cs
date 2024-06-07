namespace Validot.Tests.Unit.Rules.Times
{
    using System;
    using System.Collections.Generic;

    using Validot.Testing;
    using Validot.Tests.Unit.Rules.Numbers;
    using Validot.Translations;

    using Xunit;

    public class DateTimeRulesTests
    {
        private static readonly Func<int, DateTime> Convert = ticks => new DateTime(ticks);

        private static readonly TimesTestData.DateTimeConvert<DateTime> ConvertDateTime = (year, month, day, hour, minute, second, millisecond) => new DateTime(year, month, day, hour, minute, second, millisecond);

        public static IEnumerable<object[]> EqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.EqualTo_Unsigned(Convert),
                NumbersTestData.EqualTo_Limits(DateTime.MinValue, DateTime.MaxValue, new DateTime(1)));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError(DateTime model, DateTime value, bool shouldBeValid)
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
        public void EqualTo_Should_CollectError_FromNullable(DateTime model, DateTime value, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTime?>(
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
                NumbersTestData.NotEqualTo_Limits(DateTime.MinValue, DateTime.MaxValue, new DateTime(1)));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError(DateTime model, DateTime value, bool shouldBeValid)
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
        public void NotEqualTo_Should_CollectError_FromNullable(DateTime model, DateTime value, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTime?>(
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
                NumbersTestData.GreaterThan_Limits(DateTime.MinValue, DateTime.MaxValue, new DateTime(1)));
        }

        [Theory]
        [MemberData(nameof(After_Should_CollectError_Data))]
        public void After_Should_CollectError(DateTime model, DateTime min, bool shouldBeValid)
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
        public void After_Should_CollectError_FromNullable(DateTime model, DateTime min, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTime?>(
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
                NumbersTestData.GreaterThanOrEqualTo_Limits(DateTime.MinValue, DateTime.MaxValue, new DateTime(1)));
        }

        [Theory]
        [MemberData(nameof(AfterOrEqualTo_Should_CollectError_Data))]
        public void AfterOrEqualTo_Should_CollectError(DateTime model, DateTime min, bool shouldBeValid)
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
        public void AfterOrEqualTo_Should_CollectError_FromNullable(DateTime model, DateTime min, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTime?>(
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
                NumbersTestData.LessThan_Limits(DateTime.MinValue, DateTime.MaxValue, new DateTime(1)));
        }

        [Theory]
        [MemberData(nameof(Before_Should_CollectError_Data))]
        public void Before_Should_CollectError(DateTime model, DateTime max, bool shouldBeValid)
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
        public void Before_Should_CollectError_FromNullable(DateTime model, DateTime max, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTime?>(
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
                NumbersTestData.LessThanOrEqualTo_Limits(DateTime.MinValue, DateTime.MaxValue, new DateTime(1)));
        }

        [Theory]
        [MemberData(nameof(BeforeOrEqualTo_Should_CollectError_Data))]
        public void BeforeOrEqualTo_Should_CollectError(DateTime model, DateTime max, bool shouldBeValid)
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
        public void BeforeOrEqualTo_Should_CollectError_FromNullable(DateTime model, DateTime max, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTime?>(
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
                NumbersTestData.Between_Limits(DateTime.MinValue, DateTime.MaxValue, new DateTime(1)));
        }

        [Theory]
        [MemberData(nameof(Between_Should_CollectError_Data))]
        public void Between_Should_CollectError(DateTime min, DateTime model, DateTime max, bool shouldBeValid)
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
        public void Between_Should_CollectError_FromNullable(DateTime min, DateTime model, DateTime max, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTime?>(
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
                NumbersTestData.BetweenOrEqualTo_Limits(DateTime.MinValue, DateTime.MaxValue, new DateTime(1)));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_Data))]
        public void BetweenOrEqualTo_Should_CollectError(DateTime min, DateTime model, DateTime max, bool shouldBeValid)
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
        public void BetweenOrEqualTo_Should_CollectError_FromNullable(DateTime min, DateTime model, DateTime max, bool shouldBeValid)
        {
            Tester.TestSingleRule<DateTime?>(
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
                    TimesTestData.EqualTo(ConvertDateTime));
            }

            [Theory]
            [MemberData(nameof(EqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void EqualTo_Should_CollectError_When_TimeComparisonSet(DateTime model, DateTime value, TimeComparison timeComparison, bool shouldBeValid)
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
            public void EqualTo_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTime model, DateTime value, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTime?>(
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
                    TimesTestData.NotEqualTo(ConvertDateTime));
            }

            [Theory]
            [MemberData(nameof(NotEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void NotEqualTo_Should_CollectError_When_TimeComparisonSet(DateTime model, DateTime value, TimeComparison timeComparison, bool shouldBeValid)
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
            public void NotEqualTo_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTime model, DateTime value, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTime?>(
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
                    TimesTestData.After(ConvertDateTime));
            }

            [Theory]
            [MemberData(nameof(After_Should_CollectError_When_TimeComparisonSet_Data))]
            public void After_Should_CollectError_When_TimeComparisonSet(DateTime model, DateTime min, TimeComparison timeComparison, bool shouldBeValid)
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
            public void After_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTime model, DateTime min, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTime?>(
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
                    TimesTestData.AfterOrEqualTo(ConvertDateTime));
            }

            [Theory]
            [MemberData(nameof(AfterOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void AfterOrEqualTo_Should_CollectError_When_TimeComparisonSet(DateTime model, DateTime min, TimeComparison timeComparison, bool shouldBeValid)
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
            public void AfterOrEqualTo_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTime model, DateTime min, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTime?>(
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
                    TimesTestData.Before(ConvertDateTime));
            }

            [Theory]
            [MemberData(nameof(Before_Should_CollectError_When_TimeComparisonSet_Data))]
            public void Before_Should_CollectError_When_TimeComparisonSet(DateTime model, DateTime max, TimeComparison timeComparison, bool shouldBeValid)
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
            public void Before_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTime model, DateTime max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTime?>(
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
                    TimesTestData.BeforeOrEqualTo(ConvertDateTime));
            }

            [Theory]
            [MemberData(nameof(BeforeOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void BeforeOrEqualTo_Should_CollectError_When_TimeComparisonSet(DateTime model, DateTime max, TimeComparison timeComparison, bool shouldBeValid)
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
            public void BeforeOrEqualTo_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTime model, DateTime max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTime?>(
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
                    TimesTestData.Between(ConvertDateTime));
            }

            [Theory]
            [MemberData(nameof(Between_Should_CollectError_When_TimeComparisonSet_Data))]
            public void Between_Should_CollectError_When_TimeComparisonSet(DateTime min, DateTime model, DateTime max, TimeComparison timeComparison, bool shouldBeValid)
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
            public void Between_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTime min, DateTime model, DateTime max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTime?>(
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
                    TimesTestData.BetweenOrEqualTo(ConvertDateTime));
            }

            [Theory]
            [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_When_TimeComparisonSet_Data))]
            public void BetweenOrEqualTo_Should_CollectError_When_TimeComparisonSet(DateTime min, DateTime model, DateTime max, TimeComparison timeComparison, bool shouldBeValid)
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
            public void BetweenOrEqualTo_Should_CollectError_When_TimeComparisonSet_FromNullable(DateTime min, DateTime model, DateTime max, TimeComparison timeComparison, bool shouldBeValid)
            {
                Tester.TestSingleRule<DateTime?>(
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
