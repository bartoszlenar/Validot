namespace Validot.Tests.Unit.Translations.Spanish
{
    using System.Linq;

    using FluentAssertions;

    using Validot.Settings;
    using Validot.Translations;

    using Xunit;

    public class SpanishTranslationsExtensionsTests
    {
        [Fact]
        public void Spanish_Should_HaveValues_NonNullNorEmptyNorWhiteSpace()
        {
            Translation.Spanish.Values.All(m => !string.IsNullOrWhiteSpace(m)).Should().BeTrue();
        }

        [Fact]
        public void Spanish_Should_HaveValues_OnlyFromMessageKeys()
        {
            MessageKey.All.Should().Contain(Translation.Spanish.Keys);
        }

        [Fact]
        public void WithSpanishTranslation_Should_AddTranslation()
        {
            var settings = new ValidatorSettings();
            settings.WithSpanishTranslation();
            TranslationTestHelpers.ShouldContainSingleTranslation(settings.Translations, "Spanish", Translation.Spanish);
        }
    }
}