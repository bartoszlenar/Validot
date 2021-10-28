namespace Validot.Tests.Unit.Translations.German
{
    using System.Linq;

    using FluentAssertions;

    using Validot.Settings;
    using Validot.Translations;

    using Xunit;

    public class GermanTranslationsExtensionsTests
    {
        [Fact]
        public void German_Should_HaveValues_NonNullNorEmptyNorWhiteSpace()
        {
            Translation.German.Values.All(m => !string.IsNullOrWhiteSpace(m)).Should().BeTrue();
        }

        [Fact]
        public void German_Should_HaveValues_OnlyFromMessageKeys()
        {
            MessageKey.All.Should().Contain(Translation.German.Keys);
        }

        [Fact]
        public void WithGermanTranslation_Should_AddTranslation()
        {
            var settings = new ValidatorSettings();
            settings.WithGermanTranslation();
            TranslationTestHelpers.ShouldContainSingleTranslation(settings.Translations, "German", Translation.German);
        }
    }
}