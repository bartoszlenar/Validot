namespace Validot.Translations
{
    using System.Collections.Generic;

    public static class TranslationCompilerExtensions
    {
        public static void Add(this ITranslationCompiler @this, string name, IReadOnlyDictionary<string, string> translation)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));
            ThrowHelper.NullArgument(translation, nameof(translation));

            foreach (var entry in translation)
            {
                @this.Add(name, entry.Key, entry.Value);
            }
        }

        public static void Add(this ITranslationCompiler @this, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));
            ThrowHelper.NullArgument(translations, nameof(translations));

            foreach (var translation in translations)
            {
                @this.Add(translation.Key, translation.Value);
            }
        }
    }
}
