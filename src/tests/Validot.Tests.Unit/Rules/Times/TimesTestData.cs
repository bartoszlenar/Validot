namespace Validot.Tests.Unit.Rules.Times
{
    using System.Collections.Generic;

    public static class TimesTestData
    {
        public delegate T DateTimeConvert<T>(int year, int month, int day, int hour, int minute, int second, int millisecond);

        public static IEnumerable<object[]> EqualTo<T>(DateTimeConvert<T> dateTimeConvert)
        {
            // all equal
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // dates equal
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), TimeComparison.JustTime, false };

            // times equal
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // all different
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 9, 5, 4, 3, 2, 9), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 9, 5, 4, 3, 2, 9), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 9, 5, 4, 3, 2, 9), TimeComparison.JustTime, false };
        }

        public static IEnumerable<object[]> NotEqualTo<T>(DateTimeConvert<T> dateTimeConvert)
        {
            // all equal
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // dates equal
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), TimeComparison.JustTime, true };

            // times equal
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // all different
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 9, 5, 4, 3, 2, 9), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 9, 5, 4, 3, 2, 9), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 9, 5, 4, 3, 2, 9), TimeComparison.JustTime, true };
        }

        public static IEnumerable<object[]> After<T>(DateTimeConvert<T> dateTimeConvert)
        {
            // all equal
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // time before
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // time after
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // date before
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // date after
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // all before
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // all after
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };
        }

        public static IEnumerable<object[]> AfterOrEqualTo<T>(DateTimeConvert<T> dateTimeConvert)
        {
            // all equal
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // time before
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // time after
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // date before
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // date after
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // all before
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // all after
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };
        }

        public static IEnumerable<object[]> Before<T>(DateTimeConvert<T> dateTimeConvert)
        {
            // all equal
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // time before
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // time after
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // date before
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // date after
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // all before
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // all after
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };
        }

        public static IEnumerable<object[]> BeforeOrEqualTo<T>(DateTimeConvert<T> dateTimeConvert)
        {
            // all equal
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // time before
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // time after
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 5, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };

            // date before
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // date after
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 1), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // all before
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2007, 6, 1, 4, 3, 2, 0), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, true };

            // all after
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2007, 6, 9, 4, 3, 2, 9), dateTimeConvert(2007, 6, 5, 4, 3, 2, 1), TimeComparison.JustTime, false };
        }

        public static IEnumerable<object[]> Between<T>(DateTimeConvert<T> dateTimeConvert)
        {
            // all equal
            yield return new object[] { dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), TimeComparison.JustTime, false };

            // all between
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // dates equal min
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 9, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 9, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 9, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // dates equal max
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // time equal min
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, false };

            // time equal max
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 11), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 11), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 11), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, false };

            // dates before min
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // dates after max
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // time before min
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, false };

            // time after max
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, false };

            // all before min
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, false };

            // all after max
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, false };
        }

        public static IEnumerable<object[]> BetweenOrEqualTo<T>(DateTimeConvert<T> dateTimeConvert)
        {
            // all equal
            yield return new object[] { dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), TimeComparison.JustTime, true };

            // all between
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // dates equal min
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 9, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 9, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 9, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // dates equal max
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // time equal min
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 9), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // time equal max
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 11), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 11), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 11), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // dates before min
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // dates after max
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 10), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, true };

            // time before min
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, false };

            // time after max
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, true };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 10, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, false };

            // all before min
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 8, 10, 10, 10, 8), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, false };

            // all after max
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.All, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustDate, false };
            yield return new object[] { dateTimeConvert(2000, 10, 9, 10, 10, 10, 9), dateTimeConvert(2000, 10, 12, 10, 10, 10, 12), dateTimeConvert(2000, 10, 11, 10, 10, 10, 11), TimeComparison.JustTime, false };
        }
    }
}
