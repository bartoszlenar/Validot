namespace Validot.Results
{
    using System;
    using System.Collections.Generic;

    using Validot.Errors;

    internal class ValidationResult : IValidationResult, IValidationResultDetails
    {
        private static readonly IReadOnlyDictionary<string, IReadOnlyList<IError>> EmptyRawErrors = new Dictionary<string, IReadOnlyList<IError>>();

        private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> EmptyMessages = new Dictionary<string, IReadOnlyList<string>>();

        private static readonly IReadOnlyDictionary<string, string> EmptyTranslation = new Dictionary<string, string>();

        private readonly Lazy<IReadOnlyList<string>> _errorCodes;

        private readonly IMessagesService _messagesService;

        private readonly Lazy<IReadOnlyDictionary<string, IReadOnlyList<IError>>> _rawErrors;

        private readonly Dictionary<string, List<int>> _resultErrors;

        public ValidationResult(Dictionary<string, List<int>> resultErrors, IReadOnlyDictionary<int, IError> errorRegistry, IMessagesService messagesService)
        {
            AnyErrors = resultErrors.Count != 0;

            _resultErrors = resultErrors;

            _errorCodes = AnyErrors
                ? PrepareLazyLoadedErrorCodes(resultErrors, errorRegistry)
                : null;

            _rawErrors = AnyErrors
                ? PrepareLazyLoadedRawErrors(resultErrors, errorRegistry)
                : null;

            _messagesService = messagesService;
        }

        public static ValidationResult NoErrorsResult { get; } = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

        public bool AnyErrors { get; }

        public IValidationResultDetails Details => this;

        public IReadOnlyCollection<string> Paths => _resultErrors.Keys;

        public IReadOnlyList<string> TranslationNames => _messagesService?.TranslationNames ?? Array.Empty<string>();

        public IReadOnlyList<string> GetErrorCodes()
        {
            if (AnyErrors)
            {
                return _errorCodes.Value;
            }

            return Array.Empty<string>();
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> GetErrorMessages(string translationName = null)
        {
            return AnyErrors
                ? _messagesService.GetErrorsMessages(_resultErrors, translationName)
                : EmptyMessages;
        }

        public IReadOnlyDictionary<string, IReadOnlyList<IError>> GetRawErrors()
        {
            return AnyErrors
                ? _rawErrors.Value
                : EmptyRawErrors;
        }

        public IReadOnlyDictionary<string, string> GetTranslation(string translationName)
        {
            return _messagesService != null
                ? _messagesService.GetTranslation(translationName)
                : EmptyTranslation;
        }

        private static Lazy<IReadOnlyList<string>> PrepareLazyLoadedErrorCodes(Dictionary<string, List<int>> resultErrors, IReadOnlyDictionary<int, IError> errorRegistry)
        {
            return new Lazy<IReadOnlyList<string>>(() =>
            {
                var capacity = 0;

                foreach (var pair in resultErrors)
                {
                    for (var i = 0; i < pair.Value.Count; ++i)
                    {
                        capacity += errorRegistry[pair.Value[i]].Codes?.Count ?? 0;
                    }
                }

                var alErrorsCodes = new string[capacity];
                var pointer = 0;

                foreach (var pair in resultErrors)
                {
                    for (var i = 0; i < pair.Value.Count; ++i)
                    {
                        var singleErrorCodes = errorRegistry[pair.Value[i]].Codes;

                        if (!(singleErrorCodes is null))
                        {
                            for (var j = 0; j < singleErrorCodes.Count; ++j)
                            {
                                alErrorsCodes[pointer++] = singleErrorCodes[j];
                            }
                        }
                    }
                }

                return alErrorsCodes;
            });
        }

        private static Lazy<IReadOnlyDictionary<string, IReadOnlyList<IError>>> PrepareLazyLoadedRawErrors(Dictionary<string, List<int>> resultErrors, IReadOnlyDictionary<int, IError> errorRegistry)
        {
            return new Lazy<IReadOnlyDictionary<string, IReadOnlyList<IError>>>(() =>
            {
                var dictionary = new Dictionary<string, IReadOnlyList<IError>>(resultErrors.Count);

                foreach (var pair in resultErrors)
                {
                    var errors = new IError[pair.Value.Count];

                    for (var i = 0; i < pair.Value.Count; ++i)
                    {
                        errors[i] = errorRegistry[pair.Value[i]];
                    }

                    dictionary.Add(pair.Key, errors);
                }

                return dictionary;
            });
        }
    }
}
