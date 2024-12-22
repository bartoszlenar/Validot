namespace Validot.Errors;

using System.Collections.Generic;
using System.Linq;

using Validot.Errors.Translator;
using Validot.Translations;

internal class EmptyMessageService : IMessageService
{
    public IReadOnlyList<string> TranslationNames => Array.Empty<string>();

    public IReadOnlyDictionary<string, IReadOnlyList<string>> GetMessages(Dictionary<string, List<int>> errors, string? translationName = null) => new Dictionary<string, IReadOnlyList<string>>();
    public IReadOnlyDictionary<string, string> GetTranslation(string translationName) => new Dictionary<string, string>();
}
