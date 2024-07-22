namespace Validot.Errors.Args;

using System.Globalization;

public sealed class NumberArg<T> : NumberArg, IArg<T>
{
    private readonly Func<T, string?, CultureInfo?, string> _stringify;

    public NumberArg(string name, T value, Func<T, string?, CultureInfo?, string> stringify)
    {
        ThrowHelper.NullArgument(name, nameof(name));
        ThrowHelper.NullArgument(stringify, nameof(stringify));

        Name = name;
        Value = value;
        _stringify = stringify;
    }

    public T Value { get; }

    public string Name { get; }

    public IReadOnlyCollection<string> AllowedParameters { get; } = new[]
    {
        FormatParameter,
        CultureParameter,
    };

    public override string ToString(IReadOnlyDictionary<string, string>? parameters)
    {
        var format = parameters?.ContainsKey(FormatParameter) == true
            ? parameters[FormatParameter]
            : null;

        var culture = parameters?.ContainsKey(CultureParameter) == true
            ? CultureInfo.GetCultureInfo(parameters[CultureParameter])
            : null;

        if (format == null && culture == null)
        {
            format = DefaultFormat;
            culture = DefaultCultureInfo;
        }
        else if (format != null && culture == null)
        {
            culture = DefaultCultureInfo;
        }

        return _stringify(Value, format, culture);
    }
}
