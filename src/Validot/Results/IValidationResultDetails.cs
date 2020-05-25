namespace Validot.Results
{
    using System.Collections.Generic;

    using Validot.Errors;

    public interface IValidationResultDetails
    {
        IReadOnlyList<string> TranslationNames { get; }

        IReadOnlyCollection<string> Paths { get; }

        IReadOnlyDictionary<string, string> GetTranslation(string translationName);

        IReadOnlyList<string> GetErrorCodes();

        IReadOnlyDictionary<string, IReadOnlyList<IError>> GetRawErrors();

        IReadOnlyDictionary<string, IReadOnlyList<string>> GetErrorMessages(string translationName = null);
    }
}
