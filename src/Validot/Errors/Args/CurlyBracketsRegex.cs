namespace Validot.Errors.Args;

using System.Text.RegularExpressions;

internal static partial class CurlyBracketsRegex
{
    [GeneratedRegex(@"(?<=\{)[^}]*(?=\})", RegexOptions.Compiled, matchTimeoutMilliseconds: 1000)]
    public static partial Regex Instance();
}
