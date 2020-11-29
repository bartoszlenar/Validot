namespace Validot
{
    using System.Collections.Generic;

    using Validot.Settings;

    public static class ValidatorSettingsExtensions
    {
        /// <summary>
        /// Adds translation dictionary for error messages.
        /// </summary>
        /// <param name="this">Settings fluent API builder - input.</param>
        /// <param name="name">Translation name, e.g., "English".</param>
        /// <param name="translation">Translation dictionary. The key is message key, the value is message content.</param>
        /// <returns>Settings fluent API builder - output.</returns>
        public static ValidatorSettings WithTranslation(this ValidatorSettings @this, string name, IReadOnlyDictionary<string, string> translation)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));
            ThrowHelper.NullArgument(name, nameof(name));
            ThrowHelper.NullArgument(translation, nameof(translation));

            foreach (var entry in translation)
            {
                _ = @this.WithTranslation(name, entry.Key, entry.Value);
            }

            return @this;
        }

        /// <summary>
        /// Adds translation dictionaries for error messages.
        /// </summary>
        /// <param name="this">Settings fluent API builder - input.</param>
        /// <param name="translations">Translation dictionaries. The key is the translation name, the value is another dictionary where the key is message key and the value is message content.</param>
        /// <returns>Settings fluent API builder - output.</returns>
        public static ValidatorSettings WithTranslation(this ValidatorSettings @this, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));
            ThrowHelper.NullArgument(translations, nameof(translations));

            foreach (var translation in translations)
            {
                _ = @this.WithTranslation(translation.Key, translation.Value);
            }

            return @this;
        }

        /// <summary>
        /// Adds translation dictionaries for error messages.
        /// </summary>
        /// <param name="this">Settings fluent API builder - input.</param>
        /// <param name="translationHolder"><see cref="ITranslationHolder"/> instance that contains the translation dictionaries to be added.</param>
        /// <returns>Settings fluent API builder - output.</returns>
        public static ValidatorSettings WithTranslation(this ValidatorSettings @this, ITranslationHolder translationHolder)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));
            ThrowHelper.NullArgument(translationHolder, nameof(translationHolder));

            return @this.WithTranslation(translationHolder.Translations);
        }
    }
}
