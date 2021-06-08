namespace Validot.Tests.Unit.Translations.Portuguese
{
    using System.Linq;

    using FluentAssertions;

    using Validot.Settings;
    using Validot.Translations;

    using Xunit;

    public class PortugueseTranslationsExtensionsTests
    {
        [Fact]
        public void Portuguese_Should_HaveValues_NonNullNorEmptyNorWhiteSpace()
        {
            Translation.Portuguese.Values.All(m => !string.IsNullOrWhiteSpace(m)).Should().BeTrue();
        }

        [Fact]
        public void Portuguese_Should_HaveValues_OnlyFromMessageKeys()
        {
            MessageKey.All.Should().Contain(Translation.Portuguese.Keys);
        }

        [Fact]
        public void WithPortugueseTranslation_Should_AddTranslation()
        {
            var settings = new ValidatorSettings();
            settings.WithPortugueseTranslation();
            TranslationTestHelpers.ShouldContainSingleTranslation(settings.Translations, "Portuguese", Translation.Portuguese);
        }
    }
}