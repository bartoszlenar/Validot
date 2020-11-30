namespace Validot.Settings
{
    using System;
    using System.Collections.Generic;

    using Validot.Translations;

    /// <summary>
    /// Settings that <see cref="IValidator{T}"/> uses to perform validations.
    /// </summary>
    public sealed class ValidatorSettings : IValidatorSettings
    {
        private readonly TranslationCompiler _translationCompiler = new TranslationCompiler();

        internal ValidatorSettings()
        {
        }

        /// <inheritdoc cref="IValidatorSettings.Translations"/>
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations => _translationCompiler.Translations;

        /// <inheritdoc cref="IValidatorSettings.ReferenceLoopProtection"/>
        public bool? ReferenceLoopProtection { get; private set; }

        bool IValidatorSettings.ReferenceLoopProtection => ReferenceLoopProtection == true;

        internal bool IsLocked { get; set; }

        /// <summary>
        /// Gets the <see cref="ValidatorSettings"/> instance, initialized with the default values (English translation).
        /// </summary>
        /// <returns><see cref="ValidatorSettings"/> with default values (English translation).</returns>
        public static ValidatorSettings GetDefault()
        {
            var settings = new ValidatorSettings().WithEnglishTranslation();

            return settings;
        }

        /// <summary>
        /// Enables reference loop protection. It will be enabled automatically if the reference loop occurence is theoretically possible (based on the specification).
        /// Reference loop protection is the mechanism that tracks self-references and prevents infinite loop traversing during the validation process.
        /// </summary>
        /// <returns>Settings fluent API builder - output.</returns>
        public ValidatorSettings WithReferenceLoopProtection()
        {
            ThrowIfLocked();

            ReferenceLoopProtection = true;

            return this;
        }

        /// <summary>
        /// Disables reference loop protection, even if the reference loop occurence is theoretically possible (based on the specification).
        /// Reference loop protection is the mechanism that tracks self-references and prevents infinite loop traversing during the validation process.
        /// If the validated payloads are coming from untrustworthy sources, it might be dangerous to disable it.
        /// </summary>
        /// <returns>Settings fluent API builder - output.</returns>
        public ValidatorSettings WithReferenceLoopProtectionDisabled()
        {
            ThrowIfLocked();

            ReferenceLoopProtection = false;

            return this;
        }

        /// <summary>
        /// Adds translation entry for error messages.
        /// </summary>
        /// <param name="name">Translation name, e.g., "English".</param>
        /// <param name="messageKey">Message key. For custom messages - same content that is in WithMessage/WithExtraMessage. For built-in messages, find their keys in the docs (e.g., 'Texts.Email' for Email string rule).</param>
        /// <param name="translation">Translation content.</param>
        /// <returns>Settings fluent API builder - output.</returns>
        public ValidatorSettings WithTranslation(string name, string messageKey, string translation)
        {
            ThrowIfLocked();

            _translationCompiler.Add(name, messageKey, translation);

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
