namespace Validot.Translations
{
    using System.Collections.Generic;

    public interface ITranslationsCompiler
    {
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; }

        void Add(string name, string messageKey, string translation);
    }
}
