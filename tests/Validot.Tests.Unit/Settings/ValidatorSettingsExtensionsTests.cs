namespace Validot.Tests.Unit.Settings
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Settings;
    using Validot.Tests.Unit.Translations;

    using Xunit;

    public class ValidatorSettingsExtensionsTests
    {
        public class WithTranslation_NameAndDictionary
        {
            [Fact]
            public void Should_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var result = validatorSettings.WithTranslation("name", new Dictionary<string, string>());

                result.Should().BeSameAs(validatorSettings);
            }

            [Fact]
            public void Should_PassAllEntries()
            {
                var validatorSettings = new ValidatorSettings();

                var dictionary = new Dictionary<string, string>()
                {
                    ["k1"] = "v1",
                    ["k2"] = "v2",
                    ["k3"] = "v3",
                    ["k4"] = "v4",
                };

                validatorSettings.WithTranslation("name", dictionary);

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name"] = new Dictionary<string, string>()
                        {
                            ["k1"] = "v1",
                            ["k2"] = "v2",
                            ["k3"] = "v3",
                            ["k4"] = "v4",
                        },
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullName()
            {
                var validatorSettings = new ValidatorSettings();

                Action action = () => validatorSettings.WithTranslation(null, new Dictionary<string, string>());

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullDictionary()
            {
                var validatorSettings = new ValidatorSettings();

                Action action = () => validatorSettings.WithTranslation("name", null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullEntryInDictionary()
            {
                var validatorSettings = new ValidatorSettings();

                var dictionary = new Dictionary<string, string>()
                {
                    ["k11"] = "v11",
                    ["k12"] = null,
                    ["k13"] = "v13",
                    ["k14"] = "v14",
                };

                Action action = () => validatorSettings.WithTranslation("name", dictionary);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_PassAllEntries_FromMultipleDictionaries()
            {
                var validatorSettings = new ValidatorSettings();

                var dictionary1 = new Dictionary<string, string>()
                {
                    ["k11"] = "v11",
                    ["k12"] = "v12",
                    ["k13"] = "v13",
                    ["k14"] = "v14",
                };

                var dictionary2 = new Dictionary<string, string>()
                {
                    ["k21"] = "v21",
                    ["k22"] = "v22",
                    ["k23"] = "v23",
                    ["k24"] = "v24",
                };

                validatorSettings.WithTranslation("name1", dictionary1);
                validatorSettings.WithTranslation("name2", dictionary2);

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name1"] = new Dictionary<string, string>()
                        {
                            ["k11"] = "v11",
                            ["k12"] = "v12",
                            ["k13"] = "v13",
                            ["k14"] = "v14",
                        },
                        ["name2"] = new Dictionary<string, string>()
                        {
                            ["k21"] = "v21",
                            ["k22"] = "v22",
                            ["k23"] = "v23",
                            ["k24"] = "v24",
                        },
                    });
            }
        }

        public class WithTranslation_DictionaryOfDictionaries
        {
            [Fact]
            public void Should_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var result = validatorSettings.WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>());

                result.Should().BeSameAs(validatorSettings);
            }

            [Fact]
            public void Should_PassAllEntries()
            {
                var validatorSettings = new ValidatorSettings();

                var dictionary = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name"] = new Dictionary<string, string>()
                    {
                        ["k1"] = "v1",
                        ["k2"] = "v2",
                        ["k3"] = "v3",
                        ["k4"] = "v4",
                    }
                };

                validatorSettings.WithTranslation(dictionary);

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name"] = new Dictionary<string, string>()
                        {
                            ["k1"] = "v1",
                            ["k2"] = "v2",
                            ["k3"] = "v3",
                            ["k4"] = "v4",
                        },
                    });
            }

            [Fact]
            public void Should_PassAllEntries_WhenMoreThanOneKeysAtTopLevelDefined()
            {
                var validatorSettings = new ValidatorSettings();

                var dictionary = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name1"] = new Dictionary<string, string>()
                    {
                        ["k11"] = "v11",
                        ["k12"] = "v12",
                        ["k13"] = "v13",
                        ["k14"] = "v14",
                    },
                    ["name2"] = new Dictionary<string, string>()
                    {
                        ["k21"] = "v21",
                        ["k22"] = "v22",
                        ["k23"] = "v23",
                        ["k24"] = "v24",
                    }
                };

                validatorSettings.WithTranslation(dictionary);

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name1"] = new Dictionary<string, string>()
                        {
                            ["k11"] = "v11",
                            ["k12"] = "v12",
                            ["k13"] = "v13",
                            ["k14"] = "v14",
                        },
                        ["name2"] = new Dictionary<string, string>()
                        {
                            ["k21"] = "v21",
                            ["k22"] = "v22",
                            ["k23"] = "v23",
                            ["k24"] = "v24",
                        },
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullEntryInDictionary()
            {
                var validatorSettings = new ValidatorSettings();

                var dictionary = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name1"] = new Dictionary<string, string>()
                    {
                        ["k11"] = "v11",
                        ["k12"] = "v12",
                        ["k13"] = null,
                        ["k14"] = "v14",
                    },
                    ["name2"] = new Dictionary<string, string>()
                    {
                        ["k21"] = "v21",
                        ["k22"] = "v22",
                        ["k23"] = "v23",
                        ["k24"] = "v24",
                    }
                };

                Action action = () => validatorSettings.WithTranslation(dictionary);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullDictionary()
            {
                var validatorSettings = new ValidatorSettings();

                var dictionary = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name1"] = null,
                    ["name2"] = new Dictionary<string, string>()
                    {
                        ["k21"] = "v21",
                        ["k22"] = "v22",
                        ["k23"] = "v23",
                        ["k24"] = "v24",
                    }
                };

                Action action = () => validatorSettings.WithTranslation(dictionary);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_PassAllEntries_MultipleTimes()
            {
                var validatorSettings = new ValidatorSettings();

                var dictionary1 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name1"] = new Dictionary<string, string>()
                    {
                        ["k11"] = "v11",
                        ["k12"] = "v12",
                        ["k13"] = "v13",
                        ["k14"] = "v14",
                    }
                };

                var dictionary2 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name2"] = new Dictionary<string, string>()
                    {
                        ["k21"] = "v21",
                        ["k22"] = "v22",
                        ["k23"] = "v23",
                        ["k24"] = "v24",
                    }
                };

                validatorSettings.WithTranslation(dictionary1);
                validatorSettings.WithTranslation(dictionary2);

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name1"] = new Dictionary<string, string>()
                        {
                            ["k11"] = "v11",
                            ["k12"] = "v12",
                            ["k13"] = "v13",
                            ["k14"] = "v14",
                        },
                        ["name2"] = new Dictionary<string, string>()
                        {
                            ["k21"] = "v21",
                            ["k22"] = "v22",
                            ["k23"] = "v23",
                            ["k24"] = "v24",
                        },
                    });
            }
        }

        public class WithTranslation_TranslationHolder
        {
            [Fact]
            public void Should_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var holder = new ValidatorSettingsTests.WithTranslation.TestTranslationHolder()
                {
                    Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name1"] = new Dictionary<string, string>()
                        {
                            ["k21"] = "v21",
                        }
                    }
                };

                var result = validatorSettings.WithTranslation(holder);

                result.Should().BeSameAs(validatorSettings);
            }

            [Fact]
            public void Should_ThrowException_When_NullHolder()
            {
                var validatorSettings = new ValidatorSettings();

                Action action = () => validatorSettings.WithTranslation(null as ITranslationHolder);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullDictionaryInHolder()
            {
                var validatorSettings = new ValidatorSettings();

                var holder = new ValidatorSettingsTests.WithTranslation.TestTranslationHolder()
                {
                    Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name1"] = null,
                        ["name2"] = new Dictionary<string, string>()
                        {
                            ["k21"] = "v21",
                            ["k22"] = "v22",
                            ["k23"] = "v23",
                            ["k24"] = "v24",
                        }
                    }
                };

                Action action = () => validatorSettings.WithTranslation(holder);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullEntryInDictionaryInHolder()
            {
                var validatorSettings = new ValidatorSettings();

                var holder = new ValidatorSettingsTests.WithTranslation.TestTranslationHolder()
                {
                    Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name1"] = new Dictionary<string, string>()
                        {
                            ["k11"] = "v11",
                            ["k12"] = "v12",
                            ["k13"] = null,
                            ["k14"] = "v14",
                        },
                        ["name2"] = new Dictionary<string, string>()
                        {
                            ["k21"] = "v21",
                            ["k22"] = "v22",
                            ["k23"] = "v23",
                            ["k24"] = "v24",
                        }
                    }
                };

                Action action = () => validatorSettings.WithTranslation(holder);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_PassAllEntries()
            {
                var validatorSettings = new ValidatorSettings();

                var holder = new ValidatorSettingsTests.WithTranslation.TestTranslationHolder()
                {
                    Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name"] = new Dictionary<string, string>()
                        {
                            ["k1"] = "v1",
                            ["k2"] = "v2",
                            ["k3"] = "v3",
                            ["k4"] = "v4",
                        }
                    }
                };

                validatorSettings.WithTranslation(holder);

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name"] = new Dictionary<string, string>()
                        {
                            ["k1"] = "v1",
                            ["k2"] = "v2",
                            ["k3"] = "v3",
                            ["k4"] = "v4",
                        },
                    });
            }

            [Fact]
            public void Should_PassAllEntries_WhenMoreThanOneKeysAtTopLevelDefined()
            {
                var validatorSettings = new ValidatorSettings();

                var holder = new ValidatorSettingsTests.WithTranslation.TestTranslationHolder()
                {
                    Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name1"] = new Dictionary<string, string>()
                        {
                            ["k11"] = "v11",
                            ["k12"] = "v12",
                            ["k13"] = "v13",
                            ["k14"] = "v14",
                        },
                        ["name2"] = new Dictionary<string, string>()
                        {
                            ["k21"] = "v21",
                            ["k22"] = "v22",
                            ["k23"] = "v23",
                            ["k24"] = "v24",
                        }
                    }
                };

                validatorSettings.WithTranslation(holder);

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name1"] = new Dictionary<string, string>()
                        {
                            ["k11"] = "v11",
                            ["k12"] = "v12",
                            ["k13"] = "v13",
                            ["k14"] = "v14",
                        },
                        ["name2"] = new Dictionary<string, string>()
                        {
                            ["k21"] = "v21",
                            ["k22"] = "v22",
                            ["k23"] = "v23",
                            ["k24"] = "v24",
                        },
                    });
            }
        }
    }
}
