namespace Validot.Tests.Unit.Translations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Validot.Errors.Args;
    using Validot.Translations;

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

        public static void ShouldContainOnlyValidPlaceholders(this IReadOnlyDictionary<string, string> translation)
        {
            TestPlaceholders(translation, MessageKey.Global.Error);
            TestPlaceholders(translation, MessageKey.Global.Required);
            TestPlaceholders(translation, MessageKey.Global.Forbidden);
            TestPlaceholders(translation, MessageKey.Global.ReferenceLoop);
            TestPlaceholders(translation, MessageKey.CharType.EqualToIgnoreCase, "value");
            TestPlaceholders(translation, MessageKey.CharType.NotEqualToIgnoreCase, "value");
            TestPlaceholders(translation, MessageKey.GuidType.EqualTo, "value");
            TestPlaceholders(translation, MessageKey.GuidType.NotEqualTo, "value");
            TestPlaceholders(translation, MessageKey.GuidType.NotEmpty, "value");
            TestPlaceholders(translation, MessageKey.Collections.EmptyCollection);
            TestPlaceholders(translation, MessageKey.Collections.NotEmptyCollection);
            TestPlaceholders(translation, MessageKey.Collections.ExactCollectionSize, "size");
            TestPlaceholders(translation, MessageKey.Collections.MaxCollectionSize, "max");
            TestPlaceholders(translation, MessageKey.Collections.MinCollectionSize, "min");
            TestPlaceholders(translation, MessageKey.Collections.CollectionSizeBetween, "min", "max");
            TestPlaceholders(translation, MessageKey.Numbers.EqualTo, "value");
            TestPlaceholders(translation, MessageKey.Numbers.NotEqualTo, "value");
            TestPlaceholders(translation, MessageKey.Numbers.GreaterThan, "min");
            TestPlaceholders(translation, MessageKey.Numbers.GreaterThanOrEqualTo, "min");
            TestPlaceholders(translation, MessageKey.Numbers.LessThan, "max");
            TestPlaceholders(translation, MessageKey.Numbers.LessThanOrEqualTo, "max");
            TestPlaceholders(translation, MessageKey.Numbers.Between, "min", "max");
            TestPlaceholders(translation, MessageKey.Numbers.BetweenOrEqualTo, "min", "max");
            TestPlaceholders(translation, MessageKey.Numbers.NonZero);
            TestPlaceholders(translation, MessageKey.Numbers.Positive);
            TestPlaceholders(translation, MessageKey.Numbers.NonPositive);
            TestPlaceholders(translation, MessageKey.Numbers.Negative);
            TestPlaceholders(translation, MessageKey.Numbers.NonNegative);
            TestPlaceholders(translation, MessageKey.Numbers.NonNaN);
            TestPlaceholders(translation, MessageKey.Texts.Email, "value", "stringComparison");
            TestPlaceholders(translation, MessageKey.Texts.EqualTo, "value", "stringComparison");
            TestPlaceholders(translation, MessageKey.Texts.NotEqualTo, "value", "stringComparison");
            TestPlaceholders(translation, MessageKey.Texts.Contains, "value", "stringComparison");
            TestPlaceholders(translation, MessageKey.Texts.NotContains, "value", "stringComparison");
            TestPlaceholders(translation, MessageKey.Texts.NotEmpty);
            TestPlaceholders(translation, MessageKey.Texts.NotWhiteSpace);
            TestPlaceholders(translation, MessageKey.Texts.SingleLine);
            TestPlaceholders(translation, MessageKey.Texts.ExactLength, "length");
            TestPlaceholders(translation, MessageKey.Texts.MaxLength, "max");
            TestPlaceholders(translation, MessageKey.Texts.MinLength, "min");
            TestPlaceholders(translation, MessageKey.Texts.LengthBetween, "min", "max");
            TestPlaceholders(translation, MessageKey.Texts.Matches, "pattern");
            TestPlaceholders(translation, MessageKey.Texts.StartsWith, "value", "stringComparison");
            TestPlaceholders(translation, MessageKey.Texts.EndsWith, "value", "stringComparison");
            TestPlaceholders(translation, MessageKey.Times.EqualTo, "value", "timeComparison");
            TestPlaceholders(translation, MessageKey.Times.NotEqualTo, "value", "timeComparison");
            TestPlaceholders(translation, MessageKey.Times.After, "min", "timeComparison");
            TestPlaceholders(translation, MessageKey.Times.AfterOrEqualTo, "min", "timeComparison");
            TestPlaceholders(translation, MessageKey.Times.Before, "max", "timeComparison");
            TestPlaceholders(translation, MessageKey.Times.BeforeOrEqualTo, "max", "timeComparison");
            TestPlaceholders(translation, MessageKey.Times.Between, "min", "max", "timeComparison");
            TestPlaceholders(translation, MessageKey.Times.BetweenOrEqualTo, "min", "max", "timeComparison");
            TestPlaceholders(translation, MessageKey.TimeSpanType.EqualTo, "value");
            TestPlaceholders(translation, MessageKey.TimeSpanType.NotEqualTo, "value");
            TestPlaceholders(translation, MessageKey.TimeSpanType.GreaterThan, "min");
            TestPlaceholders(translation, MessageKey.TimeSpanType.GreaterThanOrEqualTo, "min");
            TestPlaceholders(translation, MessageKey.TimeSpanType.LessThan, "max");
            TestPlaceholders(translation, MessageKey.TimeSpanType.LessThanOrEqualTo, "max");
            TestPlaceholders(translation, MessageKey.TimeSpanType.Between, "min", "max");
            TestPlaceholders(translation, MessageKey.TimeSpanType.BetweenOrEqualTo, "min", "max");
            TestPlaceholders(translation, MessageKey.TimeSpanType.NonZero);
            TestPlaceholders(translation, MessageKey.TimeSpanType.Positive);
            TestPlaceholders(translation, MessageKey.TimeSpanType.NonPositive);
            TestPlaceholders(translation, MessageKey.TimeSpanType.Negative);
            TestPlaceholders(translation, MessageKey.TimeSpanType.NonNegative);
        }

        private static void TestPlaceholders(IReadOnlyDictionary<string, string> translation, string key, params string[] allowedRulePlaceholders)
        {
            if (!translation.ContainsKey(key))
            {
                return;
            }

            var message = translation[key];

            var placeholders = ArgHelper.ExtractPlaceholders(message);

            var globalPlaceholders = new[]
            {
                "_name", "_translation"
            };

            foreach (var placehodler in placeholders)
            {
                var placeholderIsAllowed = allowedRulePlaceholders.Concat(globalPlaceholders).Any(p => string.Equals(placehodler.Name, p, StringComparison.Ordinal));

                placeholderIsAllowed.Should().BeTrue($"Placeholder `{placehodler.Name}` is not allowed in message `{message}`");
            }
        }
    }
}