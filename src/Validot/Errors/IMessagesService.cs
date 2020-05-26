namespace Validot.Errors
{
    using System.Collections.Generic;

    internal interface IMessagesService
    {
        IReadOnlyList<string> TranslationNames { get; }

        IReadOnlyDictionary<string, string> GetTranslation(string translationName);

        IReadOnlyDictionary<string, IReadOnlyList<string>> GetErrorsMessages(Dictionary<string, List<int>> errors, string translationName = null);
    }
}
