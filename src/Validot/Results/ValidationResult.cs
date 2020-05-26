namespace Validot.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Validot.Errors;

    internal class ValidationResult : IValidationResult, IValidationResultDetails
    {
        private const string NoErrorsString = "(no error output)";

        private const string PathSeparator = ": ";

        private const string CodeSeparator = ", ";

        private static readonly IReadOnlyDictionary<string, IReadOnlyList<IError>> EmptyErrorOutput = new Dictionary<string, IReadOnlyList<IError>>();

        private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> EmptyDictionary = new Dictionary<string, IReadOnlyList<string>>();

        private static readonly IReadOnlyDictionary<string, string> EmptyTranslation = new Dictionary<string, string>();

        private readonly IMessagesService _messagesService;

        private readonly Dictionary<string, List<int>> _resultErrors;

        private readonly IReadOnlyDictionary<int, IError> _errorRegistry;

        private IReadOnlyDictionary<string, IReadOnlyList<string>> _errorCodes;
        private IReadOnlyDictionary<string, IReadOnlyList<IError>> _errorOutput;
        private IReadOnlyCollection<string> _errorCodeList;

        public ValidationResult(Dictionary<string, List<int>> resultErrors, IReadOnlyDictionary<int, IError> errorRegistry, IMessagesService messagesService)
        {
            AnyErrors = resultErrors.Count != 0;

            _resultErrors = resultErrors;
            _errorRegistry = errorRegistry;

            _messagesService = messagesService;
        }

        public static ValidationResult NoErrorsResult { get; } = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

        public bool AnyErrors { get; }

        public IValidationResultDetails Details => this;

        public IReadOnlyCollection<string> Paths => _resultErrors.Keys;

        public IReadOnlyList<string> TranslationNames => _messagesService?.TranslationNames ?? Array.Empty<string>();

        public IReadOnlyDictionary<string, IReadOnlyList<string>> GetErrorCodes()
        {
            if (_errorCodes is null)
            {
                if (!AnyErrors)
                {
                    _errorCodes = EmptyDictionary;
                }
                else
                {
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
                        _errorCodes = EmptyDictionary;
                    }
                    else
                    {
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

                        _errorCodes = dictionary;
                    }
                }
            }

            return _errorCodes;
        }

        public IReadOnlyCollection<string> GetErrorCodeList()
        {
            if (_errorCodeList is null)
            {
                if (!AnyErrors)
                {
                    _errorCodeList = Array.Empty<string>();
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

                _errorCodeList = result;
            }

            return _errorCodeList;
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> GetErrorMessages(string translationName = null)
        {
            return AnyErrors
                ? _messagesService.GetErrorsMessages(_resultErrors, translationName)
                : EmptyDictionary;
        }

        public IReadOnlyDictionary<string, IReadOnlyList<IError>> GetErrorOutput()
        {
            if (!AnyErrors)
            {
                return EmptyErrorOutput;
            }

            if (_errorOutput is null)
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

                _errorOutput = result;
            }

            return _errorOutput;
        }

        public IReadOnlyDictionary<string, string> GetTranslation(string translationName)
        {
            return _messagesService is null
                ? EmptyTranslation
                : _messagesService.GetTranslation(translationName);
        }

        public override string ToString() => ToString(null);

        public string ToString(string translationName)
        {
            if (!AnyErrors)
            {
                return NoErrorsString;
            }

            var errorMessages = GetErrorMessages(translationName);

            var errorCodes = GetErrorCodeList();

            var capacity = GetCapacity(errorMessages, errorCodes);

            var builder = new StringBuilder(capacity);

            if (errorCodes.Count > 0)
            {
                var errorCodeCounter = 0;

                foreach (var errorCode in errorCodes)
                {
                    builder.Append(errorCode);

                    if (++errorCodeCounter < errorCodes.Count)
                    {
                        builder.Append(CodeSeparator);
                    }
                }
            }

            if (errorMessages.Count > 0)
            {
                if (errorCodes.Count > 0)
                {
                    builder.Append(Environment.NewLine);
                    builder.Append(Environment.NewLine);
                }

                foreach (var pair in errorMessages)
                {
                    foreach (var error in pair.Value)
                    {
                        if (pair.Key.Length == 0)
                        {
                            builder.Append($"{error}");
                        }
                        else
                        {
                            builder.Append($"{pair.Key}{PathSeparator}{error}");
                        }

                        if (builder.Length < capacity)
                        {
                            builder.Append(Environment.NewLine);
                        }
                    }
                }
            }

            return builder.ToString();

            int GetCapacity(IReadOnlyDictionary<string, IReadOnlyList<string>> messagesDictionary, IReadOnlyCollection<string> codesCollection)
            {
                var result = 0;

                foreach (var pair in messagesDictionary)
                {
                    foreach (var error in pair.Value)
                    {
                        if (pair.Key.Length > 0)
                        {
                            result += pair.Key.Length + PathSeparator.Length;
                        }

                        result += error.Length;
                    }

                    result += (pair.Value.Count - 1) * Environment.NewLine.Length;
                }

                if (codesCollection.Count > 0)
                {
                    foreach (var code in codesCollection)
                    {
                        result += code.Length;
                    }

                    result += (codesCollection.Count - 1) * CodeSeparator.Length;
                }

                if (messagesDictionary.Count > 0 && codesCollection.Count > 0)
                {
                    result += 2 * Environment.NewLine.Length;
                }

                return result;
            }
        }
    }
}
