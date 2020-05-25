namespace Validot.Errors.Translator
{
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Errors.Args;

    internal class MessageTranslator
    {
        private const string NameArgName = "_name";

        private const string PathArgName = "_path";

        private static readonly IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>> _emptyIndexedPathPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>();

        public MessageTranslator(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations)
        {
            ThrowHelper.NullArgument(translations, nameof(translations));

            ThrowHelper.NullInCollection(translations.Values.ToArray(), $"{nameof(translations)}.Values");

            foreach (var pair in translations)
            {
                ThrowHelper.NullInCollection(pair.Value.Values.ToArray(), $"{nameof(translations)}[{pair.Key}].Values");
            }

            Translations = translations;
            TranslationsArgs = BuildTranslationArgs(translations);

            TranslationNames = translations.Keys.ToArray();
        }

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; }

        public IReadOnlyList<string> TranslationNames { get; }

        public IReadOnlyDictionary<string, IArg[]> TranslationsArgs { get; }

        public static IReadOnlyList<string> TranslateErrorMessagesWithPathPlaceholders(string path, IReadOnlyList<string> errorMessages, IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>> indexedPathsPlaceholders)
        {
            var pathArgs = CreatePathArgsForPath(path);

            var result = new string[errorMessages.Count];

            for (var i = 0; i < errorMessages.Count; ++i)
            {
                if (indexedPathsPlaceholders.ContainsKey(i))
                {
                    result[i] = ArgsHelper.FormatMessage(errorMessages[i], indexedPathsPlaceholders[i], pathArgs);
                }
                else
                {
                    result[i] = errorMessages[i];
                }
            }

            return result;
        }

        public ErrorTranslationResult TranslateErrorMessages(string translationName, IError error)
        {
            ThrowHelper.NullArgument(error, nameof(error));
            ThrowHelper.NullInCollection(error.Messages, $"{nameof(error)}.{nameof(error.Messages)}");
            ThrowHelper.NullInCollection(error.Args, $"{nameof(error)}.{nameof(error.Args)}");

            var translation = Translations[translationName];

            var messages = new string[error.Messages.Count];

            Dictionary<int, IReadOnlyList<ArgPlaceholder>> indexedPathPlaceholders = null;

            for (var i = 0; i < error.Messages.Count; ++i)
            {
                var key = error.Messages.ElementAt(i);

                var message = translation.ContainsKey(key) ? translation[key] : key;

                var placeholders = ArgsHelper.ExtractPlaceholders(message);

                messages[i] = ArgsHelper.FormatMessage(message, placeholders, error.Args);

                if (TryExtractSpecialArgs(translationName, messages[i], out var specialPlaceholders, out var specialArgs))
                {
                    messages[i] = ArgsHelper.FormatMessage(messages[i], specialPlaceholders, specialArgs);
                }

                if (TryExtractPathPlaceholders(messages[i], out var pathPlaceholders))
                {
                    if (indexedPathPlaceholders == null)
                    {
                        indexedPathPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>(messages.Length - i);
                    }

                    indexedPathPlaceholders.Add(i, pathPlaceholders);
                }
            }

            return new ErrorTranslationResult
            {
                Messages = messages,
                IndexedPathPlaceholders = indexedPathPlaceholders ?? _emptyIndexedPathPlaceholders
            };
        }

        private static IReadOnlyList<IArg> CreatePathArgsForPath(string path)
        {
            var name = PathHelper.GetLastLevel(path);

            return new[]
            {
                Arg.Text(PathArgName, path),
                Arg.Text(NameArgName, name)
            };
        }

        private static bool TryExtractPathPlaceholders(string message, out ArgPlaceholder[] placeholders)
        {
            placeholders = ArgsHelper.ExtractPlaceholders(message).Where(p => p.Name == NameArgName || p.Name == PathArgName).ToArray();

            return placeholders.Any();
        }

        private bool TryExtractSpecialArgs(string translationName, string message, out IReadOnlyList<ArgPlaceholder> specialPlaceholders, out IReadOnlyList<IArg> specialArgs)
        {
            specialPlaceholders = ArgsHelper.ExtractPlaceholders(message).Where(p => p.Name == TranslationArg.Name).ToArray();

            if (specialPlaceholders.Any())
            {
                specialArgs = TranslationsArgs[translationName];

                return true;
            }

            specialArgs = null;

            return false;
        }

        private IReadOnlyDictionary<string, IArg[]> BuildTranslationArgs(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations)
        {
            var translationArgs = new Dictionary<string, IArg[]>(translations.Count);

            foreach (var pair in translations)
            {
                var args = new IArg[]
                {
                    new TranslationArg(pair.Value)
                };

                translationArgs.Add(pair.Key, args);
            }

            return translationArgs;
        }
    }
}
