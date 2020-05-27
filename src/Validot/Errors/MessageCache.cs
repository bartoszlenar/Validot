namespace Validot.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Errors.Args;

    internal class MessageCache
    {
        private readonly Dictionary<string, Dictionary<int, IReadOnlyList<string>>> _messages = new Dictionary<string, Dictionary<int, IReadOnlyList<string>>>();

        private readonly Dictionary<int, int> _messagesAmount = new Dictionary<int, int>();

        private readonly Dictionary<string, Dictionary<string, Dictionary<int, IReadOnlyList<string>>>> _messagesWithPathArgs = new Dictionary<string, Dictionary<string, Dictionary<int, IReadOnlyList<string>>>>();

        private readonly Dictionary<string, Dictionary<int, IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>>>> _placeholders = new Dictionary<string, Dictionary<int, IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>>>>();

        public void AddMessage(string translationName, int errorId, IReadOnlyList<string> messages)
        {
            ThrowHelper.NullArgument(translationName, nameof(translationName));

            ThrowHelper.NullInCollection(messages, nameof(messages));

            if (!_messages.ContainsKey(translationName))
            {
                _messages.Add(translationName, new Dictionary<int, IReadOnlyList<string>>());
            }

            _messages[translationName].Add(errorId, messages);

            if (!_messagesAmount.ContainsKey(errorId))
            {
                _messagesAmount.Add(errorId, messages.Count);
            }
        }

        public void AddIndexedPathPlaceholders(string translationName, int errorId, IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>> indexedPlaceholders)
        {
            ThrowHelper.NullArgument(translationName, nameof(translationName));
            ThrowHelper.NullArgument(indexedPlaceholders, nameof(indexedPlaceholders));

            foreach (var pair in indexedPlaceholders)
            {
                ThrowHelper.NullInCollection(pair.Value, $"{nameof(indexedPlaceholders)}[{pair.Key}]");

                if (pair.Value.Any(IsNullInArgPlaceholder))
                {
                    throw new ArgumentNullException(nameof(indexedPlaceholders), $"Null in {nameof(ArgPlaceholder)}");
                }
            }

            if (!_placeholders.ContainsKey(translationName))
            {
                _placeholders.Add(translationName, new Dictionary<int, IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>>>());
            }

            _placeholders[translationName].Add(errorId, indexedPlaceholders);
        }

        public void AddMessageWithPathArgs(string translationName, string path, int errorId, IReadOnlyList<string> messages)
        {
            ThrowHelper.NullArgument(translationName, nameof(translationName));
            ThrowHelper.NullArgument(path, nameof(path));
            ThrowHelper.NullInCollection(messages, nameof(messages));

            if (!_messagesWithPathArgs.ContainsKey(translationName))
            {
                _messagesWithPathArgs.Add(translationName, new Dictionary<string, Dictionary<int, IReadOnlyList<string>>>());
            }

            if (!_messagesWithPathArgs[translationName].ContainsKey(path))
            {
                _messagesWithPathArgs[translationName].Add(path, new Dictionary<int, IReadOnlyList<string>>());
            }

            _messagesWithPathArgs[translationName][path].Add(errorId, messages);
        }

        public int GetMessageAmount(List<int> errorsIds)
        {
            var amount = 0;

            for (var i = 0; i < errorsIds.Count; ++i)
            {
                amount += _messagesAmount[errorsIds[i]];
            }

            return amount;
        }

        public IReadOnlyList<string> GetMessages(string translationName, int errorId)
        {
            return _messages[translationName][errorId];
        }

        public IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>> GetIndexedPathPlaceholders(string translationName, int errorId)
        {
            return _placeholders[translationName][errorId];
        }

        public IReadOnlyList<string> GetMessagesWithPathArgs(string translationName, string path, int errorId)
        {
            return _messagesWithPathArgs[translationName][path][errorId];
        }

        public bool IsMessageWithPathArgsCached(string translationName, string path, int errorId)
        {
            return _messagesWithPathArgs.ContainsKey(translationName) &&
                   _messagesWithPathArgs[translationName].ContainsKey(path) &&
                   _messagesWithPathArgs[translationName][path].ContainsKey(errorId);
        }

        public bool ContainsPathArgs(string translationName, int errorId)
        {
            return _placeholders.ContainsKey(translationName) && _placeholders[translationName].ContainsKey(errorId);
        }

        public void VerifyIntegrity()
        {
            var allErrorsIds = _messages.Any()
                ? _messages.FirstOrDefault().Value.Keys.ToArray()
                : Array.Empty<int>();

            foreach (var pair in _messages)
            {
                var translation = pair.Key;

                foreach (var errorPair in pair.Value)
                {
                    var errorId = errorPair.Key;

                    if (!allErrorsIds.Contains(errorId))
                    {
                        throw new CacheIntegrityException($"ErrorId {errorId} is not present in all translations");
                    }

                    var errorMessages = errorPair.Value;

                    if (errorMessages.Count != _messagesAmount[errorId])
                    {
                        throw new CacheIntegrityException($"ErrorId {errorId}, messages amount is expected to be {_messagesAmount[errorId]} but found {errorMessages.Count} in translation `{translation}`");
                    }
                }
            }

            foreach (var pair in _placeholders)
            {
                var translation = pair.Key;

                if (!_messages.ContainsKey(translation))
                {
                    throw new CacheIntegrityException($"Translation `{translation}` is not expected in path placeholders");
                }

                foreach (var pair2 in pair.Value)
                {
                    var errorId = pair2.Key;

                    if (!allErrorsIds.Contains(errorId))
                    {
                        throw new CacheIntegrityException($"ErrorId {errorId} is not expected in path placeholders (translation `{translation}`)");
                    }

                    var indexes = pair2.Value.Keys;

                    var maxIndex = _messagesAmount[errorId] - 1;

                    if (indexes.Any(i => i > maxIndex))
                    {
                        var aboveMax = indexes.First(i => i > maxIndex);

                        throw new CacheIntegrityException($"ErrorId {errorId} max index for path placeholder is {maxIndex}, but found {aboveMax} (translation `{translation}`)");
                    }
                }
            }

            foreach (var pair in _messagesWithPathArgs)
            {
                var translation = pair.Key;

                if (!_messages.ContainsKey(translation))
                {
                    throw new CacheIntegrityException($"Translation `{translation}` is not expected in messages with path args");
                }

                foreach (var pair2 in pair.Value)
                {
                    var path = pair2.Key;

                    foreach (var pair3 in pair2.Value)
                    {
                        var errorId = pair3.Key;

                        if (!allErrorsIds.Contains(errorId))
                        {
                            throw new CacheIntegrityException($"Error ID {errorId} in translation `{translation}` is not expected in messages with path args");
                        }

                        if (pair3.Value.Count > _messagesAmount[errorId])
                        {
                            throw new CacheIntegrityException($"Error ID {errorId} is expected to have max {_messagesAmount[errorId]} messages, but found {pair3.Value.Count} in messages with path args (for translation `{translation}` and path `{path}`)");
                        }
                    }
                }
            }
        }

        private bool IsNullInArgPlaceholder(ArgPlaceholder argPlaceholder)
        {
            return argPlaceholder.Name == null ||
                   argPlaceholder.Placeholder == null ||
                   argPlaceholder.Parameters == null ||
                   (argPlaceholder.Parameters.Count > 0 && argPlaceholder.Parameters.Values.Any(p => p is null));
        }
    }
}
