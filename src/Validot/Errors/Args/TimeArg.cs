namespace Validot.Errors.Args;

using System;
using System.Collections.Generic;
using System.Globalization;

public abstract class TimeArg
{
    internal string DefaultFormat { get; set; } = string.Empty;

    protected static string FormatParameter => "format";

    protected static string CultureParameter => "culture";

    protected static CultureInfo DefaultCultureInfo { get; } = CultureInfo.InvariantCulture;

    public abstract string ToString(IReadOnlyDictionary<string, string> parameters);
}
