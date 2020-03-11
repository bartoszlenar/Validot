namespace Validot.Tests.Unit.Rules.Numbers
{
    using System;
    using System.Collections.Generic;

    using Validot.Testing;
    using Validot.Translations;

    using Xunit;

    public class CharNumbersRulesTests
    {
        private static readonly Func<int, char> Convert = i => (char)i;

        public static IEnumerable<object[]> EqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.EqualTo_Unsigned(Convert),
                NumbersTestData.EqualTo_Limits(char.MinValue, char.MaxValue, 1));
        }

        [Theory]
        [MemberData(nameof(EqualTo_Should_CollectError_Data))]
        public void EqualTo_Should_CollectError(char model, char value, bool shouldBeValid)
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
        public void EqualTo_Should_CollectError_FromNullable(char model, char value, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
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
                NumbersTestData.NotEqualTo_Limits(char.MinValue, char.MaxValue, 1));
        }

        [Theory]
        [MemberData(nameof(NotEqualTo_Should_CollectError_Data))]
        public void NotEqualTo_Should_CollectError(char model, char value, bool shouldBeValid)
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
        public void NotEqualTo_Should_CollectError_FromNullable(char model, char value, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
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
                NumbersTestData.GreaterThan_Limits(char.MinValue, char.MaxValue, 1));
        }

        [Theory]
        [MemberData(nameof(GreaterThan_Should_CollectError_Data))]
        public void GreaterThan_Should_CollectError(char model, char min, bool shouldBeValid)
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
        public void GreaterThan_Should_CollectError_FromNullable(char model, char min, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
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
                NumbersTestData.GreaterThanOrEqualTo_Limits(char.MinValue, char.MaxValue, 1));
        }

        [Theory]
        [MemberData(nameof(GreaterThanOrEqualTo_Should_CollectError_Data))]
        public void GreaterThanOrEqualTo_Should_CollectError(char model, char min, bool shouldBeValid)
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
        public void GreaterThanOrEqualTo_Should_CollectError_FromNullable(char model, char min, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
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
                NumbersTestData.LessThan_Limits(char.MinValue, char.MaxValue, 1));
        }

        [Theory]
        [MemberData(nameof(LessThan_Should_CollectError_Data))]
        public void LessThan_Should_CollectError(char model, char max, bool shouldBeValid)
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
        public void LessThan_Should_CollectError_FromNullable(char model, char max, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
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
                NumbersTestData.LessThanOrEqualTo_Limits(char.MinValue, char.MaxValue, 1));
        }

        [Theory]
        [MemberData(nameof(LessThanOrEqualTo_Should_CollectError_Data))]
        public void LessThanOrEqualTo_Should_CollectError(char model, char max, bool shouldBeValid)
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
        public void LessThanOrEqualTo_Should_CollectError_FromNullable(char model, char max, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
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
                NumbersTestData.Between_Limits(char.MinValue, char.MaxValue, 1));
        }

        [Theory]
        [MemberData(nameof(Between_Should_CollectError_Data))]
        public void Between_Should_CollectError(char min, char model, char max, bool shouldBeValid)
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
        public void Between_Should_CollectError_FromNullable(char min, char model, char max, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
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
                NumbersTestData.Between_InvalidRange(Convert, char.MinValue, char.MaxValue));
        }

        [Theory]
        [MemberData(nameof(Between_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void Between_Should_ThrowException_When_MinLargerThanMax(char min, char max)
        {
            Tester.TestExceptionOnInit<char>(
                s => s.Between(min, max),
                typeof(ArgumentException));
        }

        [Theory]
        [MemberData(nameof(Between_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void Between_Should_ThrowException_When_MinLargerThanMax_FromNullable(char min, char max)
        {
            Tester.TestExceptionOnInit<char?>(
                s => s.Between(min, max),
                typeof(ArgumentException));
        }

        public static IEnumerable<object[]> BetweenOrEqualTo_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.BetweenOrEqualTo_Unsigned(Convert),
                NumbersTestData.BetweenOrEqualTo_Limits(char.MinValue, char.MaxValue, 1));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_CollectError_Data))]
        public void BetweenOrEqualTo_Should_CollectError(char min, char model, char max, bool shouldBeValid)
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
        public void BetweenOrEqualTo_Should_CollectError_FromNullable(char min, char model, char max, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
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
                NumbersTestData.Between_InvalidRange(Convert, char.MinValue, char.MaxValue));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax(char min, char max)
        {
            Tester.TestExceptionOnInit<char>(
                s => s.BetweenOrEqualTo(min, max),
                typeof(ArgumentException));
        }

        [Theory]
        [MemberData(nameof(BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_Data))]
        public void BetweenOrEqualTo_Should_ThrowException_When_MinLargerThanMax_FromNullable(char min, char max)
        {
            Tester.TestExceptionOnInit<char?>(
                s => s.BetweenOrEqualTo(min, max),
                typeof(ArgumentException));
        }

        public static IEnumerable<object[]> NonZero_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonZero_Unsigned(Convert),
                NumbersTestData.NonZero_Unsigned_Limits(char.MaxValue));
        }

        [Theory]
        [MemberData(nameof(NonZero_Should_CollectError_Data))]
        public void NonZero_Should_CollectError(char model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonZero(),
                shouldBeValid,
                MessageKey.Numbers.NonZero);
        }

        [Theory]
        [MemberData(nameof(NonZero_Should_CollectError_Data))]
        public void NonZero_Should_CollectError_FromNullable(char model, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
                model,
                m => m.NonZero(),
                shouldBeValid,
                MessageKey.Numbers.NonZero);
        }

        public static IEnumerable<object[]> Positive_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.Positive_Unsigned(Convert));
        }

        [Theory]
        [MemberData(nameof(Positive_Should_CollectError_Data))]
        public void Positive_Should_CollectError(char model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.Positive(),
                shouldBeValid,
                MessageKey.Numbers.Positive);
        }

        [Theory]
        [MemberData(nameof(Positive_Should_CollectError_Data))]
        public void Positive_Should_CollectError_FromNullable(char model, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
                model,
                m => m.Positive(),
                shouldBeValid,
                MessageKey.Numbers.Positive);
        }

        public static IEnumerable<object[]> NonPositive_Should_CollectError_Data()
        {
            return RulesHelper.GetTestDataCombined(
                NumbersTestData.NonPositive_Unsigned(Convert));
        }

        [Theory]
        [MemberData(nameof(NonPositive_Should_CollectError_Data))]
        public void NonPositive_Should_CollectError(char model, bool shouldBeValid)
        {
            Tester.TestSingleRule(
                model,
                m => m.NonPositive(),
                shouldBeValid,
                MessageKey.Numbers.NonPositive);
        }

        [Theory]
        [MemberData(nameof(NonPositive_Should_CollectError_Data))]
        public void NonPositive_Should_CollectError_FromNullable(char model, bool shouldBeValid)
        {
            Tester.TestSingleRule<char?>(
                model,
                m => m.NonPositive(),
                shouldBeValid,
                MessageKey.Numbers.NonPositive);
        }
    }
}
