namespace Validot.Errors
{
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Errors.Translator;
    using Validot.Translations;

    internal class MessagesService : IMessagesService
    {
        private readonly MessagesCache _cache;

        private readonly MessagesTranslator _translator;

        public MessagesService(
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations,
            IReadOnlyDictionary<int, IError> errors,
            IReadOnlyDictionary<string, IReadOnlyList<int>> errorsMap)
        {
            _translator = new MessagesTranslator(translations);

            _cache = BuildMessagesCache(_translator, errors, errorsMap);
        }

        public IReadOnlyList<string> TranslationsNames => _translator.TranslationsNames;

        public IReadOnlyDictionary<string, string> GetTranslation(string translationName)
        {
            return _translator.Translations[translationName];
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> GetErrorsMessages(Dictionary<string, List<int>> errors, string translationName = null)
        {
            var results = new Dictionary<string, IReadOnlyList<string>>(errors.Count);

            translationName = translationName ?? nameof(Translation.English);

            foreach (var pair in errors)
            {
                var path = pair.Key;
                var errorsIds = pair.Value;

                var capacity = _cache.GetErrorsMessagesAmount(errorsIds);

                var allErrorMessages = new string[capacity];

                var index = 0;

                for (var i = 0; i < errorsIds.Count; ++i)
                {
                    var errorId = errorsIds[i];

                    IReadOnlyList<string> messages;

                    if (!_cache.ContainsPathArgs(translationName, errorId))
                    {
                        messages = _cache.GetMessages(translationName, errorId);
                    }
                    else if (_cache.IsMessageWithPathArgsCached(translationName, path, errorId))
                    {
                        messages = _cache.GetMessagesWithPathArgs(translationName, path, errorId);
                    }
                    else
                    {
                        var cachedMessages = _cache.GetMessages(translationName, errorId);
                        var indexedPathPlaceholders = _cache.GetIndexedPathPlaceholders(translationName, errorId);

                        messages = MessagesTranslator.TranslateErrorMessagesWithPathPlaceholders(path, cachedMessages, indexedPathPlaceholders);
                    }

                    CopyMessages(messages, allErrorMessages, ref index);
                }

                results.Add(path, allErrorMessages);
            }

            return results;
        }

        private void CopyMessages(IReadOnlyList<string> source, string[] target, ref int targetIndex)
        {
            for (var i = 0; i < source.Count; ++i)
            {
                target[targetIndex + i] = source[i];
            }

            targetIndex += source.Count;
        }

        private MessagesCache BuildMessagesCache(MessagesTranslator translator, IReadOnlyDictionary<int, IError> errors, IReadOnlyDictionary<string, IReadOnlyList<int>> errorsMap)
        {
            ThrowHelper.NullArgument(errors, nameof(errors));
            ThrowHelper.NullArgument(errorsMap, nameof(errorsMap));
            ThrowHelper.NullInCollection(errorsMap.Values.ToArray(), $"{nameof(errorsMap)}.{nameof(errorsMap.Values)}");

            var uniqueErrorsIds = errorsMap.SelectMany(b => b.Value).Distinct().ToArray();

            var cache = new MessagesCache();

            foreach (var translationName in TranslationsNames)
            {
                foreach (var errorId in uniqueErrorsIds)
                {
                    var translationResult = translator.TranslateErrorMessages(translationName, errors[errorId]);

                    cache.AddMessage(translationName, errorId, translationResult.Messages);

                    if (translationResult.AnyPathPlaceholders)
                    {
                        cache.AddIndexedPathPlaceholders(translationName, errorId, translationResult.IndexedPathPlaceholders);
                    }
                }
            }

            foreach (var translationName in TranslationsNames)
            {
                foreach (var errorsMapPair in errorsMap)
                {
                    var path = errorsMapPair.Key;

                    foreach (var errorId in errorsMapPair.Value)
                    {
                        if (!cache.ContainsPathArgs(translationName, errorId) || PathsHelper.ContainsIndexes(path))
                        {
                            continue;
                        }

                        var cachedMessages = cache.GetMessages(translationName, errorId);
                        var indexedPlaceholders = cache.GetIndexedPathPlaceholders(translationName, errorId);

                        var errorMessagesWithSpecials = MessagesTranslator.TranslateErrorMessagesWithPathPlaceholders(path, cachedMessages, indexedPlaceholders);

                        cache.AddMessageWithPathArgs(translationName, path, errorId, errorMessagesWithSpecials);
                    }
                }
            }

            return cache;
        }
    }
}
