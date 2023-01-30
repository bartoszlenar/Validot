namespace Validot.Tests.Unit.Translations.English
{
    using System.Linq;

    using FluentAssertions;

    using Validot.Settings;
    using Validot.Translations;

    using Xunit;

    public class EnglishTranslationsExtensionsTests
    {
        [Fact]
        public void Polish_Should_HaveValues_NonNullNorEmptyNorWhiteSpace()
        {
            Translation.English.Values.All(m => !string.IsNullOrWhiteSpace(m)).Should().BeTrue();
        }

        [Fact]
        public void English_Should_HaveValues_ForAllMessageKeys()
        {
            Translation.English.Keys.Should().Contain(MessageKey.All);
            Translation.English.Keys.Should().HaveCount(MessageKey.All.Count);
        }

        [Fact]
        public void English_Should_HaveValues_OnlyFromMessageKeys()
        {
            MessageKey.All.Should().Contain(Translation.English.Keys);
        }

        [Fact]
        public void English_Should_HaveValues_OnlyWithAllowedPlaceholders()
        {
            Translation.English.ShouldContainOnlyValidPlaceholders();
        }

        [Fact]
        public void WithEnglishTranslation_Should_AddTranslation()
        {
            var settings = new ValidatorSettings();
            settings.WithEnglishTranslation();
            TranslationTestHelpers.ShouldContainSingleTranslation(settings.Translations, "English", Translation.English);
        }
    }
}