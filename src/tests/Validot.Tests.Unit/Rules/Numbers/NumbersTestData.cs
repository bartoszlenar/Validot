namespace Validot.Tests.Unit.Rules.Numbers
{
    using System;
    using System.Collections.Generic;

    public static class NumbersTestData
    {
        public static IEnumerable<object[]> EqualTo_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(3), false };
            yield return new object[] { convert(2), convert(5), false };
            yield return new object[] { convert(1), convert(1), true };
        }

        public static IEnumerable<object[]> EqualTo_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(-1), false };
            yield return new object[] { convert(-2), convert(-5), false };
            yield return new object[] { convert(-1), convert(-1), true };
            yield return new object[] { convert(-2), convert(2), false };
        }

        public static IEnumerable<object[]> EqualTo_Limits<T>(T min, T max, T neutral)
        {
            yield return new object[] { max, max, true };
            yield return new object[] { min, max, false };
            yield return new object[] { min, min, true };
            yield return new object[] { min, neutral, false };
            yield return new object[] { max, neutral, false };
        }

        public static IEnumerable<object[]> NotEqualTo_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(3), true };
            yield return new object[] { convert(2), convert(5), true };
            yield return new object[] { convert(1), convert(1), false };
        }

        public static IEnumerable<object[]> NotEqualTo_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(-1), true };
            yield return new object[] { convert(-2), convert(-5), true };
            yield return new object[] { convert(-1), convert(-1), false };
            yield return new object[] { convert(-2), convert(2), true };
        }

        public static IEnumerable<object[]> NotEqualTo_Limits<T>(T min, T max, T neutral)
        {
            yield return new object[] { max, max, false };
            yield return new object[] { min, max, true };
            yield return new object[] { min, min, false };
            yield return new object[] { min, neutral, true };
            yield return new object[] { max, neutral, true };
        }

        public static IEnumerable<object[]> GreaterThan_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(3), false };
            yield return new object[] { convert(2), convert(1), true };
            yield return new object[] { convert(1), convert(1), false };
            yield return new object[] { convert(1), convert(0), true };
        }

        public static IEnumerable<object[]> GreaterThan_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(-1), true };
            yield return new object[] { convert(-2), convert(-1), false };
            yield return new object[] { convert(-1), convert(-1), false };
            yield return new object[] { convert(2), convert(-2), true };
        }

        public static IEnumerable<object[]> GreaterThan_Limits<T>(T min, T max, T neutral)
        {
            yield return new object[] { max, max, false };
            yield return new object[] { min, max, false };
            yield return new object[] { max, min, true };
            yield return new object[] { min, min, false };
            yield return new object[] { min, neutral, false };
            yield return new object[] { max, neutral, true };
        }

        public static IEnumerable<object[]> GreaterThanOrEqualTo_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(3), false };
            yield return new object[] { convert(2), convert(1), true };
            yield return new object[] { convert(1), convert(1), true };
            yield return new object[] { convert(1), convert(0), true };
        }

        public static IEnumerable<object[]> GreaterThanOrEqualTo_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(-1), true };
            yield return new object[] { convert(-2), convert(-1), false };
            yield return new object[] { convert(-1), convert(-1), true };
            yield return new object[] { convert(2), convert(-2), true };
        }

        public static IEnumerable<object[]> GreaterThanOrEqualTo_Limits<T>(T min, T max, T neutral)
        {
            yield return new object[] { max, max, true };
            yield return new object[] { min, max, false };
            yield return new object[] { max, min, true };
            yield return new object[] { min, min, true };
            yield return new object[] { min, neutral, false };
            yield return new object[] { max, neutral, true };
        }

        public static IEnumerable<object[]> LessThan_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(3), true };
            yield return new object[] { convert(2), convert(1), false };
            yield return new object[] { convert(1), convert(1), false };
            yield return new object[] { convert(1), convert(0), false };
        }

        public static IEnumerable<object[]> LessThan_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(-1), false };
            yield return new object[] { convert(-2), convert(-1), true };
            yield return new object[] { convert(-1), convert(-1), false };
            yield return new object[] { convert(2), convert(-2), false };
        }

        public static IEnumerable<object[]> LessThan_Limits<T>(T min, T max, T neutral)
        {
            yield return new object[] { max, max, false };
            yield return new object[] { min, max, true };
            yield return new object[] { max, min, false };
            yield return new object[] { min, min, false };
            yield return new object[] { min, neutral, true };
            yield return new object[] { max, neutral, false };
        }

        public static IEnumerable<object[]> LessThanOrEqualTo_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(3), true };
            yield return new object[] { convert(2), convert(1), false };
            yield return new object[] { convert(1), convert(1), true };
            yield return new object[] { convert(1), convert(0), false };
        }

        public static IEnumerable<object[]> LessThanOrEqualTo_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), convert(-1), false };
            yield return new object[] { convert(-2), convert(-1), true };
            yield return new object[] { convert(-1), convert(-1), true };
            yield return new object[] { convert(2), convert(-2), false };
        }

        public static IEnumerable<object[]> LessThanOrEqualTo_Limits<T>(T min, T max, T neutral)
        {
            yield return new object[] { max, max, true };
            yield return new object[] { min, max, true };
            yield return new object[] { max, min, false };
            yield return new object[] { min, min, true };
            yield return new object[] { min, neutral, true };
            yield return new object[] { max, neutral, false };
        }

        public static IEnumerable<object[]> Between_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(1), convert(1), convert(3), false };
            yield return new object[] { convert(1), convert(2), convert(3), true };
            yield return new object[] { convert(1), convert(3), convert(3), false };
            yield return new object[] { convert(1), convert(0), convert(3), false };
            yield return new object[] { convert(1), convert(4), convert(3), false };
            yield return new object[] { convert(3), convert(3), convert(3), false };
            yield return new object[] { convert(3), convert(4), convert(3), false };
        }

        public static IEnumerable<object[]> Between_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(-1), convert(0), convert(1), true };
            yield return new object[] { convert(-1), convert(-1), convert(1), false };
            yield return new object[] { convert(-1), convert(1), convert(1), false };
            yield return new object[] { convert(-1), convert(2), convert(1), false };
            yield return new object[] { convert(-1), convert(-2), convert(1), false };
            yield return new object[] { convert(1), convert(1), convert(1), false };
            yield return new object[] { convert(-3), convert(-2), convert(-3), false };
            yield return new object[] { convert(-3), convert(-2), convert(-1), true };
        }

        public static IEnumerable<object[]> Between_Limits<T>(T min, T max, T neutral)
        {
            yield return new object[] { max, neutral, max, false };
            yield return new object[] { min, neutral, max, true };
            yield return new object[] { min, max, max, false };
            yield return new object[] { min, min, max, false };
            yield return new object[] { min, min, min, false };
            yield return new object[] { max, max, max, false };
        }

        public static IEnumerable<object[]> Between_InvalidRange<T>(Func<int, T> convert, T min, T max)
        {
            yield return new object[] { convert(2), convert(1) };
            yield return new object[] { max, min };
        }

        public static IEnumerable<object[]> BetweenOrEqualTo_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(-1), convert(0), convert(1), true };
            yield return new object[] { convert(-1), convert(-1), convert(1), true };
            yield return new object[] { convert(-1), convert(1), convert(1), true };
            yield return new object[] { convert(-1), convert(2), convert(1), false };
            yield return new object[] { convert(-1), convert(-2), convert(1), false };
            yield return new object[] { convert(1), convert(1), convert(1), true };
            yield return new object[] { convert(-3), convert(-2), convert(-3), false };
            yield return new object[] { convert(-3), convert(-2), convert(-1), true };
        }

        public static IEnumerable<object[]> BetweenOrEqualTo_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(1), convert(1), convert(3), true };
            yield return new object[] { convert(1), convert(2), convert(3), true };
            yield return new object[] { convert(1), convert(3), convert(3), true };
            yield return new object[] { convert(1), convert(0), convert(3), false };
            yield return new object[] { convert(1), convert(4), convert(3), false };
            yield return new object[] { convert(3), convert(3), convert(3), true };
            yield return new object[] { convert(3), convert(4), convert(3), false };
        }

        public static IEnumerable<object[]> BetweenOrEqualTo_Limits<T>(T min, T max, T neutral)
        {
            yield return new object[] { max, neutral, max, false };
            yield return new object[] { min, neutral, max, true };
            yield return new object[] { min, max, max, true };
            yield return new object[] { min, min, max, true };
            yield return new object[] { min, min, min, true };
            yield return new object[] { max, max, max, true };
        }

        public static IEnumerable<object[]> NonZero_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(-1), true };
            yield return new object[] { convert(-10), true };
        }

        public static IEnumerable<object[]> NonZero_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), false };
            yield return new object[] { convert(1), true };
            yield return new object[] { convert(5), true };
        }

        public static IEnumerable<object[]> NonZero_Signed_Limits<T>(T min, T max)
        {
            yield return new object[] { min, true };
            yield return new object[] { max, true };
        }

        public static IEnumerable<object[]> NonZero_Unsigned_Limits<T>(T max)
        {
            yield return new object[] { max, true };
        }

        public static IEnumerable<object[]> Positive_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(-1), false };
            yield return new object[] { convert(-10), false };
        }

        public static IEnumerable<object[]> Positive_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), false };
            yield return new object[] { convert(1), true };
            yield return new object[] { convert(10), true };
        }

        public static IEnumerable<object[]> NonPositive_Signed<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(-1), true };
            yield return new object[] { convert(-10), true };
        }

        public static IEnumerable<object[]> NonPositive_Unsigned<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), true };
            yield return new object[] { convert(1), false };
            yield return new object[] { convert(10), false };
        }

        public static IEnumerable<object[]> Negative<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), false };
            yield return new object[] { convert(1), false };
            yield return new object[] { convert(10), false };
            yield return new object[] { convert(-1), true };
            yield return new object[] { convert(-10), true };
        }

        public static IEnumerable<object[]> NonNegative<T>(Func<int, T> convert)
        {
            yield return new object[] { convert(0), true };
            yield return new object[] { convert(1), true };
            yield return new object[] { convert(10), true };
            yield return new object[] { convert(-1), false };
            yield return new object[] { convert(-10), false };
        }
    }
}
