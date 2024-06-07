namespace Validot.Errors.Args;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

public sealed class NameArg : IArg
{
    private const string FormatParameter = "format";

    private const string TitleCaseParameterValue = "titleCase";

    private static readonly string[] KeyAsAllowedParameters =
    [
        FormatParameter,
    ];

    private static readonly IReadOnlyList<Regex> TitleCaseRegexes = new List<Regex>()
    {
        new("([a-z])([A-Z][a-z])", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200)),
        new("([A-Z][a-z])([A-Z])", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200)),
        new("([a-z])([A-Z]+[a-z])", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200)),
        new("([A-Z]+)([A-Z][a-z][a-z])", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200)),
        new("([a-z]+)([A-Z0-9]+)", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200)),
        new("([A-Z]+)([A-Z][a-rt-z][a-z]*)", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200)),
        new("([0-9])([A-Z][a-z]+)", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200)),
        new("([A-Z]{2,})([0-9]{2,})", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200)),
        new("([0-9]{2,})([A-Z]{2,})", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200)),
    };

    private readonly string _name;

    public NameArg(string name)
    {
        ThrowHelper.NullArgument(name, nameof(name));

        _name = name;
    }

    public static string Name { get; } = "_name";

    string IArg.Name => Name;

    public IReadOnlyCollection<string> AllowedParameters => KeyAsAllowedParameters;

    public string ToString(IReadOnlyDictionary<string, string>? parameters)
    {
        var formatParameter = parameters?.ContainsKey(FormatParameter) == true
            ? parameters[FormatParameter]
            : null;

        if (formatParameter == null)
        {
            return _name;
        }

        return Stringify(_name, formatParameter);
    }

    private static string Stringify(string value, string formatParameter)
    {
        if (string.Equals(formatParameter, TitleCaseParameterValue, StringComparison.Ordinal))
        {
            return ConvertToTitleCase(value);
        }

        return value;
    }

    private static string ConvertToTitleCase(string input)
    {
        // Title case method taken from https://stackoverflow.com/a/35953318/1633913
        if (input.Contains('_', StringComparison.Ordinal))
        {
            input = input.Replace('_', ' ');
            input = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input);
        }

        foreach (var regex in TitleCaseRegexes)
        {
            input = regex.Replace(input, "$1 $2");
        }

        input = input.Trim();

        if (input.Length == 1)
        {
            return char.ToUpperInvariant(input[0]).ToString(CultureInfo.InvariantCulture);
        }

        return string.Concat(char.ToUpperInvariant(input[0]).ToString(CultureInfo.InvariantCulture), input.AsSpan(1));
    }
}
