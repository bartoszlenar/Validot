namespace Validot.Tests.Unit.Translations
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Settings;
    using Validot.Translations;

    using Xunit;

    public class TranslationTests
    {
        private static void ShouldAddSingleTranslation(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> settingsTranslations, string translationName, IReadOnlyDictionary<string, string> translation)
        {
            settingsTranslations.Should().NotBeEmpty();
            settingsTranslations.Should().HaveCount(1);
            settingsTranslations.Keys.Should().ContainSingle(translationName);

            var selectedTranslation = settingsTranslations[translationName];

            selectedTranslation.Should().HaveCount(translation.Count);

            foreach (var pair in translation)
            {
                selectedTranslation.Keys.Should().Contain(pair.Key);

                selectedTranslation[pair.Key].Should().Be(pair.Value);
            }
        }

        public class Polish
        {
            [Fact]
            public void Polish_Should_HaveValues()
            {
                Translation.Polish.Values.Should().NotContainNulls();
            }

            [Fact]
            public void WithPolishTranslation_Should_AddTranslation()
            {
                var settings = new ValidatorSettings();
                settings.WithPolishTranslation();
                ShouldAddSingleTranslation(settings.Translations, "Polish", Translation.Polish);
            }
        }

        public class English
        {
            [Fact]
            public void English_Should_HaveValues()
            {
                Translation.English.Values.Should().NotContainNulls();
            }

            [Fact]
            public void WithEnglishTranslation_Should_AddTranslation()
            {
                var settings = new ValidatorSettings();
                settings.WithEnglishTranslation();
                ShouldAddSingleTranslation(settings.Translations, "English", Translation.English);
            }
        }
    }
}
