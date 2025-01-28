namespace Validot.Tests.Unit.Translations.Chinese
{
    using System.Linq;

    using FluentAssertions;

    using Validot.Settings;
    using Validot.Translations;

    using Xunit;

    public class ChineseTranslationsExtensionsTests
    {
        [Fact]
        public void Chinese_Should_HaveValues_NonNullNorEmptyNorWhiteSpace()
        {
            Translation.Chinese.Values.All(m => !string.IsNullOrWhiteSpace(m)).Should().BeTrue();
        }

        [Fact]
        public void Chinese_Should_HaveValues_OnlyFromMessageKeys()
        {
            MessageKey.All.Should().Contain(Translation.Chinese.Keys);
        }

        [Fact]
        public void Chinese_Should_HaveValues_OnlyWithAllowedPlaceholders()
        {
            Translation.Chinese.ShouldContainOnlyValidPlaceholders();
        }

        [Fact]
        public void WithChineseTranslation_Should_AddTranslation()
        {
            var settings = new ValidatorSettings();
            settings.WithChineseTranslation();
            TranslationTestHelpers.ShouldContainSingleTranslation(settings.Translations, "Chinese", Translation.Chinese);
        }
    }
}