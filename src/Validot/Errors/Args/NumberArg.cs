namespace Validot.Errors.Args;

using System.Globalization;

public abstract class NumberArg
{
    protected static string FormatParameter => "format";

    protected static string CultureParameter => "culture";

    protected CultureInfo DefaultCultureInfo { get; } = CultureInfo.InvariantCulture;

    protected string DefaultFormat { get; } = string.Empty;

    public abstract string ToString(IReadOnlyDictionary<string, string> parameters);
}
