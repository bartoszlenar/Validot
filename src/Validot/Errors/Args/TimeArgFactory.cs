namespace Validot;

using Validot.Errors.Args;

public static partial class Arg
{
    public static IArg Time(string name, DateTime value)
    {
        return new TimeArg<DateTime>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo))
        {
            DefaultFormat = DateTimeFormats.DateAndTimeFormat,
        };
    }

    public static IArg Time(string name, DateTimeOffset value)
    {
        return new TimeArg<DateTimeOffset>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo))
        {
            DefaultFormat = DateTimeFormats.DateAndTimeFormat,
        };
    }

    public static IArg Time(string name, TimeSpan value)
    {
        return new TimeArg<TimeSpan>(name, value, (v, format, cultureInfo) => v.ToString(format, cultureInfo))
        {
            DefaultFormat = DateTimeFormats.TimeSpanFormat,
        };
    }
}
