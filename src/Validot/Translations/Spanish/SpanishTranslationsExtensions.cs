namespace Validot
{
    using Validot.Settings;
    using Validot.Translations;

    public static partial class TranslationsExtensions
    {
        /// <summary>
        /// Adds Spanish translation dictionary for the error messages used in the built-in rules.
        /// This added by default (for Validators created with Validator.Factory.Create).
        /// </summary>
        /// <param name="this"><see cref="ValidatorSettings"/>Settings fluent API builder - input.</param>
        /// <returns>Settings fluent API builder - output.</returns>
        public static ValidatorSettings WithSpanishTranslation(this ValidatorSettings @this)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return @this.WithTranslation(nameof(Translation.Spanish), Translation.Spanish);
        }
    }
}
