namespace Validot.Tests.Unit.Translations.Polish
{
    using System.Linq;

    using FluentAssertions;

    using Validot.Settings;
    using Validot.Translations;

    using Xunit;

    public class PolishTranslationsExtensionsTests
    {
        [Fact]
        public void Polish_Should_HaveValues_NonNullNorEmptyNorWhiteSpace()
        {
            Translation.Polish.Values.All(m => !string.IsNullOrWhiteSpace(m)).Should().BeTrue();
        }

        [Fact]
        public void Polish_Should_HaveValues_OnlyFromMessageKeys()
        {
            MessageKey.All.Should().Contain(Translation.Polish.Keys);
        }

        [Fact]
        public void Polish_Should_HaveValues_OnlyWithAllowedPlaceholders()
        {
            Translation.Polish.ShouldContainOnlyValidPlaceholders();
        }

        [Fact]
        public void WithPolishTranslation_Should_AddTranslation()
        {
            var settings = new ValidatorSettings();
            settings.WithPolishTranslation();
            TranslationTestHelpers.ShouldContainSingleTranslation(settings.Translations, "Polish", Translation.Polish);
        }
    }
}