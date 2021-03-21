namespace Validot.Tests.Unit.Translations
{
    using System.Collections.Generic;

    using FluentAssertions;

    public static class TranslationTestHelpers
    {
        public static void ShouldBeLikeTranslations(this IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> @this, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> baseDictionary)
        {
            @this.Should().NotBeNull();
            baseDictionary.Should().NotBeNull();

            @this.Should().NotBeSameAs(baseDictionary);

            @this.Keys.Should().HaveCount(baseDictionary.Count);

            foreach (var baseKey in baseDictionary.Keys)
            {
                @this.Keys.Should().Contain(baseKey);
                @this[baseKey].Should().NotBeSameAs(baseDictionary[baseKey]);
                @this[baseKey].Keys.Should().HaveCount(baseDictionary[baseKey].Count);

                foreach (var baseEntryKey in baseDictionary[baseKey].Keys)
                {
                    @this[baseKey].Keys.Should().Contain(baseEntryKey);
                    @this[baseKey][baseEntryKey].Should().Be(baseDictionary[baseKey][baseEntryKey]);
                }
            }
        }

        public static void ShouldContainSingleTranslation(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> settingsTranslations, string translationName, IReadOnlyDictionary<string, string> translation)
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
    }
}