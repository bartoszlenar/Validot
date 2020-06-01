namespace Validot
{
    using System.Collections.Generic;

    public interface ITranslationHolder
    {
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; }
    }
}
