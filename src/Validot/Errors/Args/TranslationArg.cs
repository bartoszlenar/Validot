namespace Validot.Errors.Args;

public sealed class TranslationArg : IArg
{
    private const string KeyParameter = "key";

    private static readonly string[] KeyAsAllowedParameters =
    {
        KeyParameter,
    };

    private readonly IReadOnlyDictionary<string, string> _translation;

    public TranslationArg(IReadOnlyDictionary<string, string> translation)
    {
        ThrowHelper.NullArgument(translation, nameof(translation));

        _translation = translation;
    }

    public static string Name { get; } = "_translation";

    string IArg.Name { get; } = Name;

    public IReadOnlyCollection<string> AllowedParameters => KeyAsAllowedParameters;

    public static string CreatePlaceholder(string key)
    {
        return $"{{{Name}|key={key}}}";
    }

    public string ToString(IReadOnlyDictionary<string, string>? parameters)
    {
        var keyParameter = parameters?.ContainsKey(KeyParameter) == true
            ? parameters[KeyParameter]
            : null;

        if (keyParameter == null)
        {
            return Name;
        }

        return _translation.ContainsKey(keyParameter)
            ? _translation[keyParameter]
            : keyParameter;
    }
}
