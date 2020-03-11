namespace Validot.Factory
{
    using System.Collections.Generic;

    public interface ITranslationsHolder
    {
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; }
    }
}
