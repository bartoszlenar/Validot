namespace Validot.Errors.Args;

using System.Globalization;

public sealed class TextArg : IArg<string>
{
    private const string CaseParameter = "case";

    private const string UpperCaseParameterValue = "upper";

    private const string LowerCaseParameterValue = "lower";

    private static readonly string[] StaticAllowedParameters = new[]
    {
        CaseParameter,
    };

    public TextArg(string name, string value)
    {
        ThrowHelper.NullArgument(name, nameof(name));
        ThrowHelper.NullArgument(value, nameof(value));

        Value = value;
        Name = name;
    }

    public TextArg(string name, char value)
        : this(name, value.ToString(CultureInfo.InvariantCulture))
    {
    }

    public string Name { get; }

    public string Value { get; }

    public IReadOnlyCollection<string> AllowedParameters => StaticAllowedParameters;

    public string ToString(IReadOnlyDictionary<string, string>? parameters)
    {
        var caseParameter = parameters?.ContainsKey(CaseParameter) == true
            ? parameters[CaseParameter]
            : null;

        if (caseParameter is not null and
            not UpperCaseParameterValue and
            not LowerCaseParameterValue)
        {
            caseParameter = null;
        }

        return Stringify(Value, caseParameter);
    }

    private static string Stringify(string value, string? caseParameter)
    {
        if (caseParameter == UpperCaseParameterValue)
        {
            return value.ToUpper(CultureInfo.InvariantCulture);
        }

        if (caseParameter == LowerCaseParameterValue)
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            return value.ToLower(CultureInfo.InvariantCulture);
#pragma warning restore CA1308 // Normalize strings to uppercase
        }

        return value;
    }
}
