namespace Validot.Settings
{
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Factory;
    using Validot.Settings.Capacities;
    using Validot.Translations;

    public class ValidatorSettings : IValidatorSettings
    {
        private TranslationsCompiler _translationsCompiler = new TranslationsCompiler();

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations
        {
            get => _translationsCompiler.Translations;
            set
            {
                if (_translationsCompiler.Translations.Any())
                {
                    _translationsCompiler = new TranslationsCompiler();
                }

                _translationsCompiler.Add(value);
            }
        }

        public ICapacityInfo CapacityInfo { get; set; }

        public bool? ReferenceLoopProtection { get; set; }

        public static ValidatorSettings GetDefault()
        {
            var settings = new ValidatorSettings().WithEnglishTranslation();

            settings.CapacityInfo = new DisabledCapacityInfo();

            return settings;
        }

        public ValidatorSettings WithReferenceLoopProtection()
        {
            ReferenceLoopProtection = true;

            return this;
        }

        public ValidatorSettings WithoutReferenceLoopProtection()
        {
            ReferenceLoopProtection = false;

            return this;
        }

        public ValidatorSettings WithTranslation(string name, string messageKey, string translation)
        {
            _translationsCompiler.Add(name, messageKey, translation);

            return this;
        }

        public ValidatorSettings WithTranslation(string name, IReadOnlyDictionary<string, string> translation)
        {
            ThrowHelper.NullArgument(name, nameof(name));
            ThrowHelper.NullArgument(translation, nameof(translation));

            foreach (var entry in translation)
            {
                _translationsCompiler.Add(name, entry.Key, entry.Value);
            }

            return this;
        }

        public ValidatorSettings WithTranslation(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> translations)
        {
            ThrowHelper.NullArgument(translations, nameof(translations));

            foreach (var translation in translations)
            {
                _translationsCompiler.Add(translation.Key, translation.Value);
            }

            return this;
        }

        public ValidatorSettings WithTranslation(ITranslationsHolder translationsHolder)
        {
            ThrowHelper.NullArgument(translationsHolder, nameof(translationsHolder));

            _translationsCompiler.Add(translationsHolder.Translations);

            return this;
        }
    }
}
