namespace Validot
{
    using Validot.Settings;
    using Validot.Translations;

    public static partial class TranslationsExtensions
    {
        /// <summary>
        /// Adds German translation dictionary for the error messages used in the built-in rules.
        /// </summary>
        /// <param name="this">Settings fluent API builder - input.</param>
        /// <returns>Settings fluent API builder - output.</returns>
        public static ValidatorSettings WithGermanTranslation(this ValidatorSettings @this)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return @this.WithTranslation(nameof(Translation.German), Translation.German);
        }
    }
}