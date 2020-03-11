namespace Validot
{
    using Validot.Settings;
    using Validot.Translations;

    public static partial class TranslationsExtensions
    {
        public static ValidatorSettings WithPolishTranslation(this ValidatorSettings @this)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return @this.WithTranslation(nameof(Translation.Polish), Translation.Polish);
        }
    }
}
