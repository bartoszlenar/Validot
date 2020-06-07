namespace Validot.Settings
{
    using System;
    using System.Collections.Generic;

    using Validot.Settings.Capacities;
    using Validot.Translations;

    public sealed class ValidatorSettings
    {
        private static readonly DisabledCapacityInfo _disabledCapacityInfo = new DisabledCapacityInfo();

        private readonly TranslationCompiler _translationCompiler = new TranslationCompiler();

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations
        {
            get => _translationCompiler.Translations;
        }

        public bool? ReferenceLoopProtection { get; private set; }

        internal bool IsLocked { get; set; }

        internal ICapacityInfo CapacityInfo { get; private set; } = _disabledCapacityInfo;

        public static ValidatorSettings GetDefault()
        {
            var settings = new ValidatorSettings().WithEnglishTranslation();

            return settings;
        }

        public ValidatorSettings WithReferenceLoopProtection()
        {
            ThrowIfLocked();

            ReferenceLoopProtection = true;

            return this;
        }

        public ValidatorSettings WithReferenceLoopProtectionDisabled()
        {
            ThrowIfLocked();

            ReferenceLoopProtection = false;

            return this;
        }

        public ValidatorSettings WithTranslation(string name, string messageKey, string translation)
        {
            ThrowIfLocked();

            _translationCompiler.Add(name, messageKey, translation);

            return this;
        }

        internal ValidatorSettings WithCapacityInfo(ICapacityInfo capacityInfo)
        {
            ThrowHelper.NullArgument(capacityInfo, nameof(capacityInfo));

            ThrowIfLocked();

            CapacityInfo = capacityInfo;

            return this;
        }

        private void ThrowIfLocked()
        {
            if (IsLocked)
            {
                throw new InvalidOperationException("Settings object is locked and can't be modified.");
            }
        }
    }
}
