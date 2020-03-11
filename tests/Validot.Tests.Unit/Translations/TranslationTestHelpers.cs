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
    }
}
