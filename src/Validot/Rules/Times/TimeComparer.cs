namespace Validot
{
    using System;

    internal static class TimeComparer
    {
        public static int Compare(DateTime a, DateTime b, TimeComparison mode)
        {
            switch (mode)
            {
                case TimeComparison.JustDate:
                    return DateTime.Compare(a.Date, b.Date);

                case TimeComparison.JustTime:
                    return TimeSpan.Compare(a.TimeOfDay, b.TimeOfDay);

                case TimeComparison.All:
                default:
                    return DateTime.Compare(a, b);
            }
        }

        public static int Compare(DateTimeOffset a, DateTimeOffset b, TimeComparison mode)
        {
            switch (mode)
            {
                case TimeComparison.JustDate:
                    return DateTimeOffset.Compare(a.Date, b.Date);

                case TimeComparison.JustTime:
                    return TimeSpan.Compare(a.TimeOfDay, b.TimeOfDay);

                case TimeComparison.All:
                default:
                    return DateTimeOffset.Compare(a, b);
            }
        }
    }
}
