namespace Validot.Errors.Translator;

using Validot.Errors.Args;

internal class MessageTranslator
{
    private const string NameArgName = "_name";

    private const string PathArgName = "_path";

    private static readonly IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>> EmptyIndexedPathPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>();

    public MessageTranslator(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations)
    {
        ThrowHelper.NullArgument(translations, nameof(translations));

        ThrowHelper.NullInCollection(translations.Values.ToArray(), $"{nameof(translations)}.Values");

        foreach (var pair in translations)
        {
            ThrowHelper.NullInCollection(pair.Value.Values.ToArray(), $"{nameof(translations)}[{pair.Key}].Values");
        }

        Translations = translations;
        TranslationArgs = BuildTranslationArgs(translations);

        TranslationNames = translations.Keys.ToArray();
    }

    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; }

    public IReadOnlyList<string> TranslationNames { get; }

    public IReadOnlyDictionary<string, IArg[]> TranslationArgs { get; }

    public static IReadOnlyList<string> TranslateMessagesWithPathPlaceholders(string path, IReadOnlyList<string> errorMessages, IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>> indexedPathsPlaceholders)
    {
        var pathArgs = CreatePathArgsForPath(path);

        var result = new string[errorMessages.Count];

        for (var i = 0; i < errorMessages.Count; ++i)
        {
            if (indexedPathsPlaceholders.ContainsKey(i))
            {
                result[i] = ArgHelper.FormatMessage(errorMessages[i], indexedPathsPlaceholders[i], pathArgs);
            }
            else
            {
                result[i] = errorMessages[i];
            }
        }

        return result;
    }

    public TranslationResult TranslateMessages(string translationName, IError error)
    {
        ThrowHelper.NullArgument(error, nameof(error));
        ThrowHelper.NullInCollection(error.Messages, $"{nameof(error)}.{nameof(error.Messages)}");
        ThrowHelper.NullInCollection(error.Args, $"{nameof(error)}.{nameof(error.Args)}");

        var translation = Translations[translationName];

        var messages = new string[error.Messages.Count];

        Dictionary<int, IReadOnlyList<ArgPlaceholder>>? indexedPathPlaceholders = null;

        for (var i = 0; i < error.Messages.Count; ++i)
        {
            var key = error.Messages[i];

            var message = translation.ContainsKey(key) ? translation[key] : key;

            var placeholders = ArgHelper.ExtractPlaceholders(message);

            messages[i] = ArgHelper.FormatMessage(message, placeholders, error.Args);

            if (TryExtractSpecialArgs(translationName, messages[i], out var specialPlaceholders, out var specialArgs))
            {
                messages[i] = ArgHelper.FormatMessage(messages[i], specialPlaceholders, specialArgs);
            }

            if (TryExtractPathPlaceholders(messages[i], out var pathPlaceholders))
            {
                indexedPathPlaceholders ??= new Dictionary<int, IReadOnlyList<ArgPlaceholder>>(messages.Length - i);

                indexedPathPlaceholders.Add(i, pathPlaceholders);
            }
        }

        return new TranslationResult
        {
            Messages = messages,
            IndexedPathPlaceholders = indexedPathPlaceholders ?? EmptyIndexedPathPlaceholders,
        };
    }

    private static IReadOnlyList<IArg> CreatePathArgsForPath(string path)
    {
        var name = PathHelper.GetLastLevel(path);

        return new[]
        {
            Arg.Text(PathArgName, path),
            new NameArg(name),
        };
    }

    private static IReadOnlyDictionary<string, IArg[]> BuildTranslationArgs(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations)
    {
        var translationArgs = new Dictionary<string, IArg[]>(translations.Count);

        foreach (var pair in translations)
        {
            var args = new IArg[]
            {
                new TranslationArg(pair.Value),
            };

            translationArgs.Add(pair.Key, args);
        }

        return translationArgs;
    }

    private static bool TryExtractPathPlaceholders(string message, out ArgPlaceholder[] placeholders)
    {
        placeholders = ArgHelper.ExtractPlaceholders(message).Where(p => p.Name is NameArgName or PathArgName).ToArray();

        return placeholders.Length != 0;
    }

    private bool TryExtractSpecialArgs(string translationName, string message, out IReadOnlyList<ArgPlaceholder> specialPlaceholders, out IReadOnlyList<IArg>? specialArgs)
    {
        specialPlaceholders = ArgHelper.ExtractPlaceholders(message).Where(p => p.Name == TranslationArg.Name).ToArray();

        if (specialPlaceholders.Any())
        {
            specialArgs = TranslationArgs[translationName];

            return true;
        }

        specialArgs = null;

        return false;
    }
}
