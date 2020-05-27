namespace Validot.Results
{
    using System.Collections.Generic;

    public interface IValidationResult
    {
        bool AnyErrors { get; }

        IReadOnlyCollection<string> Paths { get; }

        IReadOnlyCollection<string> Codes { get; }

        IReadOnlyDictionary<string, IReadOnlyList<string>> CodeMap { get; }

        IReadOnlyDictionary<string, IReadOnlyList<string>> MessageMap { get; }

        IReadOnlyList<string> TranslationNames { get; }

        IReadOnlyDictionary<string, IReadOnlyList<string>> GetTranslatedMessageMap(string translationName);
    }
}
