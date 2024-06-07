namespace Validot.Tests.Unit.Translations.Russian
{
    using System.Linq;

    using FluentAssertions;

    using Validot.Settings;
    using Validot.Translations;

    using Xunit;

    public class RussianTranslationsExtensionsTests
    {
        [Fact]
        public void Russian_Should_HaveValues_NonNullNorEmptyNorWhiteSpace()
        {
            Translation.Russian.Values.All(m => !string.IsNullOrWhiteSpace(m)).Should().BeTrue();
        }

        [Fact]
        public void Russian_Should_HaveValues_OnlyFromMessageKeys()
        {
            MessageKey.All.Should().Contain(Translation.Russian.Keys);
        }

        [Fact]
        public void Russian_Should_HaveValues_OnlyWithAllowedPlaceholders()
        {
            Translation.Russian.ShouldContainOnlyValidPlaceholders();
        }

        [Fact]
        public void WithRussianTranslation_Should_AddTranslation()
        {
            var settings = new ValidatorSettings();
            settings.WithRussianTranslation();
            TranslationTestHelpers.ShouldContainSingleTranslation(settings.Translations, "Russian", Translation.Russian);
        }
    }
}