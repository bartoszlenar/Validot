namespace Validot.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Validot.Errors;

    internal class ValidationResult : IValidationResult
    {
        private const string NoErrorsString = "(no error output)";

        private const string PathSeparator = ": ";

        private const string CodeSeparator = ", ";

        private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> EmptyDictionary = new Dictionary<string, IReadOnlyList<string>>();

        private readonly IMessagesService _messagesService;

        private readonly Dictionary<string, List<int>> _resultErrors;

        private readonly IReadOnlyDictionary<int, IError> _errorRegistry;

        private IReadOnlyCollection<string> _codes;
        private IReadOnlyDictionary<string, IReadOnlyList<string>> _codeMap;
        private IReadOnlyDictionary<string, IReadOnlyList<string>> _messageMap;

        public ValidationResult(Dictionary<string, List<int>> resultErrors, IReadOnlyDictionary<int, IError> errorRegistry, IMessagesService messagesService)
        {
            AnyErrors = resultErrors.Count != 0;

            _resultErrors = resultErrors;
            _errorRegistry = errorRegistry;
            _messagesService = messagesService;
        }

        public static ValidationResult NoErrorsResult { get; } = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

        public bool AnyErrors { get; }

        public IReadOnlyCollection<string> Paths => _resultErrors.Keys;

        public IReadOnlyCollection<string> Codes
        {
            get
            {
                return _codes ?? (_codes = GetCodes());
            }
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> CodeMap
        {
            get
            {
                return _codeMap ?? (_codeMap = GetCodeMap());
            }
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> MessageMap
        {
            get
            {
                return _messageMap ?? (_messageMap = GetTranslatedMessageMap(null));
            }
        }

        public IReadOnlyList<string> TranslationNames => _messagesService?.TranslationNames ?? Array.Empty<string>();

        public IReadOnlyDictionary<string, IReadOnlyList<string>> GetTranslatedMessageMap(string translationName)
        {
            return AnyErrors
                ? _messagesService.GetMessages(_resultErrors, translationName)
                : EmptyDictionary;
        }

        public override string ToString() => ToString(null);

        public string ToString(string translationName)
        {
            if (!AnyErrors)
            {
                return NoErrorsString;
            }

            var messageMap = translationName is null
                ? MessageMap
                : GetTranslatedMessageMap(translationName);

            var capacity = GetCapacity(messageMap, Codes);

            var stringBuilder = new StringBuilder(capacity);

            if (Codes.Count > 0)
            {
                var codeCounter = 0;

                foreach (var code in Codes)
                {
                    stringBuilder.Append(code);

                    if (++codeCounter < Codes.Count)
                    {
                        stringBuilder.Append(CodeSeparator);
                    }
                }
            }

            if (messageMap.Count > 0)
            {
                if (Codes.Count > 0)
                {
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append(Environment.NewLine);
                }

                foreach (var pair in messageMap)
                {
                    foreach (var message in pair.Value)
                    {
                        if (pair.Key.Length == 0)
                        {
                            stringBuilder.Append($"{message}");
                        }
                        else
                        {
                            stringBuilder.Append($"{pair.Key}{PathSeparator}{message}");
                        }

                        if (stringBuilder.Length < capacity)
                        {
                            stringBuilder.Append(Environment.NewLine);
                        }
                    }
                }
            }

            return stringBuilder.ToString();

            int GetCapacity(IReadOnlyDictionary<string, IReadOnlyList<string>> mMap, IReadOnlyCollection<string> cMap)
            {
                var result = 0;

                foreach (var pair in mMap)
                {
                    foreach (var message in pair.Value)
                    {
                        if (pair.Key.Length > 0)
                        {
                            result += pair.Key.Length + PathSeparator.Length;
                        }

                        result += message.Length;
                    }

                    result += (pair.Value.Count - 1) * Environment.NewLine.Length;
                }

                if (cMap.Count > 0)
                {
                    foreach (var code in cMap)
                    {
                        result += code.Length;
                    }

                    result += (cMap.Count - 1) * CodeSeparator.Length;
                }

                if (mMap.Count > 0 && cMap.Count > 0)
                {
                    result += 2 * Environment.NewLine.Length;
                }

                return result;
            }
        }

        internal IReadOnlyDictionary<string, IReadOnlyList<IError>> GetErrorOutput()
        {
            var result = new Dictionary<string, IReadOnlyList<IError>>(_resultErrors.Count);

            foreach (var pair in _resultErrors)
            {
                var errors = new IError[pair.Value.Count];

                for (var i = 0; i < pair.Value.Count; ++i)
                {
                    errors[i] = _errorRegistry[pair.Value[i]];
                }

                result.Add(pair.Key, errors);
            }

            return result;
        }

        private IReadOnlyCollection<string> GetCodes()
        {
            if (!AnyErrors)
            {
                return Array.Empty<string>();
            }

            var result = new HashSet<string>();

            foreach (var pair in _resultErrors)
            {
                for (var i = 0; i < pair.Value.Count; ++i)
                {
                    if (_errorRegistry[pair.Value[i]].Codes?.Any() == true)
                    {
                        for (var j = 0; j < _errorRegistry[pair.Value[i]].Codes.Count; ++j)
                        {
                            if (!result.Contains(_errorRegistry[pair.Value[i]].Codes[j]))
                            {
                                result.Add(_errorRegistry[pair.Value[i]].Codes[j]);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private IReadOnlyDictionary<string, IReadOnlyList<string>> GetCodeMap()
        {
            if (!AnyErrors)
            {
                return EmptyDictionary;
            }

            var pathCapacity = 0;

            foreach (var pair in _resultErrors)
            {
                for (var i = 0; i < pair.Value.Count; ++i)
                {
                    if (_errorRegistry[pair.Value[i]].Codes?.Any() == true)
                    {
                        ++pathCapacity;
                        break;
                    }
                }
            }

            if (pathCapacity == 0)
            {
                return EmptyDictionary;
            }

            var dictionary = new Dictionary<string, IReadOnlyList<string>>(pathCapacity);

            foreach (var pair in _resultErrors)
            {
                var codesCapacity = 0;

                for (var i = 0; i < pair.Value.Count; ++i)
                {
                    codesCapacity += _errorRegistry[pair.Value[i]].Codes?.Count ?? 0;
                }

                if (codesCapacity == 0)
                {
                    continue;
                }

                var codes = new string[codesCapacity];
                var codesPointer = 0;

                for (var i = 0; i < pair.Value.Count; ++i)
                {
                    if (_errorRegistry[pair.Value[i]].Codes is null ||
                        _errorRegistry[pair.Value[i]].Codes.Count == 0)
                    {
                        continue;
                    }

                    for (var j = 0; j < _errorRegistry[pair.Value[i]].Codes.Count; ++j)
                    {
                        codes[codesPointer++] = _errorRegistry[pair.Value[i]].Codes[j];
                    }
                }

                dictionary.Add(pair.Key, codes);
            }

            return dictionary;
        }
    }
}
