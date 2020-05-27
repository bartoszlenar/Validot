namespace Validot.Errors
{
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Errors.Translator;
    using Validot.Translations;

    internal class MessageService : IMessageService
    {
        private readonly MessageCache _cache;

        private readonly MessageTranslator _translator;

        public MessageService(
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations,
            IReadOnlyDictionary<int, IError> errors,
            IReadOnlyDictionary<string, IReadOnlyList<int>> errorMap)
        {
            _translator = new MessageTranslator(translations);

            _cache = BuildMessageCache(_translator, errors, errorMap);
        }

        public IReadOnlyList<string> TranslationNames => _translator.TranslationNames;

        public IReadOnlyDictionary<string, string> GetTranslation(string translationName)
        {
            return _translator.Translations[translationName];
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> GetMessages(Dictionary<string, List<int>> errors, string translationName = null)
        {
            var results = new Dictionary<string, IReadOnlyList<string>>(errors.Count);

            translationName = translationName ?? nameof(Translation.English);

            foreach (var pair in errors)
            {
                var path = pair.Key;
                var errorsIds = pair.Value;

                var capacity = _cache.GetMessageAmount(errorsIds);

                if (capacity == 0)
                {
                    continue;
                }

                var allMessages = new string[capacity];

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

                        messages = MessageTranslator.TranslateMessagesWithPathPlaceholders(path, cachedMessages, indexedPathPlaceholders);
                    }

                    CopyMessages(messages, allMessages, ref index);
                }

                results.Add(path, allMessages);
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

        private MessageCache BuildMessageCache(MessageTranslator translator, IReadOnlyDictionary<int, IError> errors, IReadOnlyDictionary<string, IReadOnlyList<int>> errorMap)
        {
            ThrowHelper.NullArgument(errors, nameof(errors));
            ThrowHelper.NullArgument(errorMap, nameof(errorMap));
            ThrowHelper.NullInCollection(errorMap.Values.ToArray(), $"{nameof(errorMap)}.{nameof(errorMap.Values)}");

            var uniqueErrorsIds = errorMap.SelectMany(b => b.Value).Distinct().ToArray();

            var cache = new MessageCache();

            foreach (var translationName in TranslationNames)
            {
                foreach (var errorId in uniqueErrorsIds)
                {
                    var translationResult = translator.TranslateMessages(translationName, errors[errorId]);

                    cache.AddMessage(translationName, errorId, translationResult.Messages);

                    if (translationResult.AnyPathPlaceholders)
                    {
                        cache.AddIndexedPathPlaceholders(translationName, errorId, translationResult.IndexedPathPlaceholders);
                    }
                }
            }

            foreach (var translationName in TranslationNames)
            {
                foreach (var errorMapPair in errorMap)
                {
                    var path = errorMapPair.Key;

                    foreach (var errorId in errorMapPair.Value)
                    {
                        if (!cache.ContainsPathArgs(translationName, errorId) || PathHelper.ContainsIndexes(path))
                        {
                            continue;
                        }

                        var cachedMessages = cache.GetMessages(translationName, errorId);
                        var indexedPlaceholders = cache.GetIndexedPathPlaceholders(translationName, errorId);

                        var errorMessagesWithSpecials = MessageTranslator.TranslateMessagesWithPathPlaceholders(path, cachedMessages, indexedPlaceholders);

                        cache.AddMessageWithPathArgs(translationName, path, errorId, errorMessagesWithSpecials);
                    }
                }
            }

            return cache;
        }
    }
}
