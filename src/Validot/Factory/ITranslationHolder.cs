namespace Validot
{
    using System.Collections.Generic;

    /// <summary>
    /// Holds translations.
    /// </summary>
    public interface ITranslationHolder
    {
        /// <summary>
        /// Gets translation dictionaries. The key is the translation name, the value is another dictionary where the key is message key and the value is message content.
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; }
    }
}
