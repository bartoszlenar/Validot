namespace Validot.Settings
{
    using System.Collections.Generic;

    using Validot.Settings.Capacities;
    using Validot.Translations;

    public class ValidatorSettings
    {
        private static readonly DisabledCapacityInfo _disabledCapacityInfo = new DisabledCapacityInfo();

        private readonly TranslationCompiler _translationCompiler = new TranslationCompiler();

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations
        {
            get => _translationCompiler.Translations;
        }

        public bool? ReferenceLoopProtection { get; private set; }

        internal ICapacityInfo CapacityInfo { get; private set; } = _disabledCapacityInfo;

        public static ValidatorSettings GetDefault()
        {
            var settings = new ValidatorSettings().WithEnglishTranslation();

            return settings;
        }

        public ValidatorSettings WithReferenceLoopProtection()
        {
            ReferenceLoopProtection = true;

            return this;
        }

        public ValidatorSettings WithReferenceLoopProtectionDisabled()
        {
            ReferenceLoopProtection = false;

            return this;
        }

        public ValidatorSettings WithTranslation(string name, string messageKey, string translation)
        {
            _translationCompiler.Add(name, messageKey, translation);

            return this;
        }

        public ValidatorSettings WithCapacityInfo(ICapacityInfo capacityInfo)
        {
            ThrowHelper.NullArgument(capacityInfo, nameof(capacityInfo));

            CapacityInfo = capacityInfo;

            return this;
        }
    }
}
