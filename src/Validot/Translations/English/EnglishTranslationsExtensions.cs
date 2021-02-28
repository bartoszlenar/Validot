namespace Validot
{
    using Validot.Settings;
    using Validot.Translations;

    public static partial class TranslationsExtensions
    {
        /// <summary>
        /// Adds English translation dictionary for the error messages used in the built-in rules.
        /// This added by default (for Validators created with Validator.Factory.Create).
        /// </summary>
        /// <param name="this">Settings fluent API builder - input.</param>
        /// <returns>Settings fluent API builder - output.</returns>
        public static ValidatorSettings WithEnglishTranslation(this ValidatorSettings @this)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return @this.WithTranslation(nameof(Translation.English), Translation.English);
        }
    }
}