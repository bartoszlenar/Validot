namespace Validot.Settings
{
    public static class GetVerifiedSettingsExtension
    {
        public static IValidatorSettings GetVerified(this IValidatorSettings @this)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            VerifyTranslations(@this);
            VerifyCapacityInfo(@this);

            return @this;
        }

        private static void VerifyCapacityInfo(IValidatorSettings @this)
        {
            ThrowHelper.NullArgument(@this.CapacityInfo, nameof(@this.CapacityInfo));
        }

        private static void VerifyTranslations(IValidatorSettings validatorSettings)
        {
            ThrowHelper.NullArgument(validatorSettings.Translations, nameof(validatorSettings.Translations));

            ThrowHelper.NullInCollection(validatorSettings.Translations.Values, nameof(validatorSettings.Translations));

            foreach (var pair in validatorSettings.Translations)
            {
                ThrowHelper.NullInCollection(pair.Value.Values, nameof(validatorSettings.Translations));
            }
        }
    }
}
