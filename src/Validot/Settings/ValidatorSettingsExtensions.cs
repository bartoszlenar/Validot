namespace Validot
{
    using System.Collections.Generic;

    using Validot.Settings;

    public static class ValidatorSettingsExtensions
    {
        public static ValidatorSettings WithTranslation(this ValidatorSettings @this, string name, IReadOnlyDictionary<string, string> translation)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));
            ThrowHelper.NullArgument(name, nameof(name));
            ThrowHelper.NullArgument(translation, nameof(translation));

            foreach (var entry in translation)
            {
                @this.WithTranslation(name, entry.Key, entry.Value);
            }

            return @this;
        }

        public static ValidatorSettings WithTranslation(this ValidatorSettings @this, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));
            ThrowHelper.NullArgument(translations, nameof(translations));

            foreach (var translation in translations)
            {
                @this.WithTranslation(translation.Key, translation.Value);
            }

            return @this;
        }

        public static ValidatorSettings WithTranslation(this ValidatorSettings @this, ITranslationHolder translationHolder)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));
            ThrowHelper.NullArgument(translationHolder, nameof(translationHolder));

            return @this.WithTranslation(translationHolder.Translations);
        }
    }
}
