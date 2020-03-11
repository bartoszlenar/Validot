namespace Validot
{
    public static class DateTimeFormats
    {
        public static string DateFormat { get; } = "yyyy-MM-dd";

        public static string TimeFormat { get; } = "HH:mm:ss.FFFFFFF";

        public static string DateAndTimeFormat { get; } = $"{DateFormat} {TimeFormat}";

        public static string TimeSpanFormat { get; } = "c";
    }
}
