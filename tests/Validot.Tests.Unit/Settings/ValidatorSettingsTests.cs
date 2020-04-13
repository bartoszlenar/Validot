namespace Validot.Tests.Unit.Settings
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Factory;
    using Validot.Settings;
    using Validot.Tests.Unit.Translations;

    using Xunit;

    public class ValidatorSettingsTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new ValidatorSettings();
        }

        [Fact]
        public void Should_Initialize_WithDefaultValues()
        {
            var validatorSettings = new ValidatorSettings();

            validatorSettings.Translations.Should().BeEmpty();
            validatorSettings.CapacityInfo.Should().BeNull();
            validatorSettings.ReferenceLoopProtection.Should().BeNull();
        }

        [Fact]
        public void Default_Should_HaveEnglishTransation_And_DisabledStats()
        {
            var defaultSettings = ValidatorSettings.GetDefault();

            defaultSettings.ShouldBeLikeDefault();
        }

        public class ReferenceLoopProtection
        {
            [Fact]
            public void Should_WithReferenceLoopProtection_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var result = validatorSettings.WithReferenceLoopProtection();

                result.Should().BeSameAs(validatorSettings);
            }

            [Fact]
            public void Should_WithReferenceLoopProtection_Set_ReferenceLoopProtection_To_True()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.WithReferenceLoopProtection();

                validatorSettings.ReferenceLoopProtection.Should().BeTrue();
            }

            [Fact]
            public void Should_WithoutReferenceLoopProtection_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var result = validatorSettings.WithoutReferenceLoopProtection();

                result.Should().BeSameAs(validatorSettings);
            }

            [Fact]
            public void Should_WithoutReferenceLoopProtection_Set_ReferenceLoopProtection_To_False()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.WithoutReferenceLoopProtection();

                validatorSettings.ReferenceLoopProtection.Should().BeFalse();
            }
        }

        public class Translating
        {
            [Fact]
            public void Should_WithNameKeyTranslation_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var result = validatorSettings.WithTranslation("name", "key", "translation");

                result.Should().BeSameAs(validatorSettings);
            }

            [Fact]
            public void Should_WithNameKeyTranslation_AddEntry()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.WithTranslation("name", "k1", "v1");

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name"] = new Dictionary<string, string>()
                        {
                            ["k1"] = "v1",
                        },
                    });
            }

            [Fact]
            public void Should_WithNameKeyTranslation_AddMultipleEntries()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.WithTranslation("name1", "k1", "v1");
                validatorSettings.WithTranslation("name1", "k2", "v2");
                validatorSettings.WithTranslation("name2", "k3", "v3");
                validatorSettings.WithTranslation("name2", "k4", "v4");

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name1"] = new Dictionary<string, string>()
                        {
                            ["k1"] = "v1",
                            ["k2"] = "v2",
                        },
                        ["name2"] = new Dictionary<string, string>()
                        {
                            ["k3"] = "v3",
                            ["k4"] = "v4",
                        },
                    });
            }

            [Theory]
            [InlineData(null, "key", "value")]
            [InlineData("name", null, "value")]
            [InlineData("name", "key", null)]
            public void Should_WithNameKeyTranslation_ThrowException_When_NullArgument(string name, string key, string translation)
            {
                var validatorSettings = new ValidatorSettings();

                Action action = () => validatorSettings.WithTranslation(name, key, translation);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_WithNameAndDictionary_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var result = validatorSettings.WithTranslation("name", new Dictionary<string, string>());

                result.Should().BeSameAs(validatorSettings);
            }

            [Fact]
            public void Should_WithNameAndDictionary_PassAllEntries()
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
            public void Should_WithNameAndDictionary_ThrowException_When_NullName()
            {
                var validatorSettings = new ValidatorSettings();

                Action action = () => validatorSettings.WithTranslation(null, new Dictionary<string, string>());

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_WithNameAndDictionary_ThrowException_When_NullDictionary()
            {
                var validatorSettings = new ValidatorSettings();

                Action action = () => validatorSettings.WithTranslation("name", null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_WithNameAndDictionary_ThrowException_When_NullEntryInDictionary()
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
            public void Should_WithNameAndDictionary_PassAllEntries_FromMultipleDictionaries()
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

            [Fact]
            public void Should_WithDictionaryOfDictionaries_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var result = validatorSettings.WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>());

                result.Should().BeSameAs(validatorSettings);
            }

            [Fact]
            public void Should_WithDictionaryOfDictionaries_PassAllEntries()
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
            public void Should_WithDictionaryOfDictionaries_PassAllEntries_WhenMoreThanOneKeysAtTopLevelDefined()
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
            public void Should_WithDictionaryOfDictionaries_ThrowException_When_NullEntryInDictionary()
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
            public void Should_WithDictionaryOfDictionaries_ThrowException_When_NullDictionary()
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
            public void Should_WithDictionaryOfDictionaries_PassAllEntries_MultipleTimes()
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

            [Fact]
            public void Should_WithTranslationHolder_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var holder = new TestTranslationHolder()
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
            public void Should_WithTranslationHolder_ThrowException_When_NullHolder()
            {
                var validatorSettings = new ValidatorSettings();

                Action action = () => validatorSettings.WithTranslation(null as ITranslationsHolder);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_WithTranslationHolder_ThrowException_When_NullDictionaryInHolder()
            {
                var validatorSettings = new ValidatorSettings();

                var holder = new TestTranslationHolder()
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
            public void Should_WithTranslationHolder_ThrowException_When_NullEntryInDictionaryInHolder()
            {
                var validatorSettings = new ValidatorSettings();

                var holder = new TestTranslationHolder()
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
            public void Should_WithTranslationHolder_PassAllEntries()
            {
                var validatorSettings = new ValidatorSettings();

                var holder = new TestTranslationHolder()
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
            public void Should_WithTranslationHolder_PassAllEntries_WhenMoreThanOneKeysAtTopLevelDefined()
            {
                var validatorSettings = new ValidatorSettings();

                var holder = new TestTranslationHolder()
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

            [Fact]
            public void Should_Translations_ThrowException_When_NullEntryInDictionary()
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

                Action action = () =>
                {
                    validatorSettings.Translations = dictionary;
                };

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_Translations_ThrowException_When_Null()
            {
                var validatorSettings = new ValidatorSettings();

                Action action = () =>
                {
                    validatorSettings.Translations = null;
                };

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_Translations_ThrowException_When_NullInnerDictionary()
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

                Action action = () =>
                {
                    validatorSettings.Translations = dictionary;
                };

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_Translations_AddEntry()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name"] = new Dictionary<string, string>()
                    {
                        ["k1"] = "v1",
                    },
                };

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name"] = new Dictionary<string, string>()
                        {
                            ["k1"] = "v1",
                        },
                    });
            }

            [Fact]
            public void Should_Translations_OverwritePreviousValue()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name1"] = new Dictionary<string, string>()
                    {
                        ["k11"] = "v11",
                    },
                    ["name2"] = new Dictionary<string, string>()
                    {
                        ["k24"] = "v24",
                    }
                };

                validatorSettings.Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name2"] = new Dictionary<string, string>()
                    {
                        ["k21"] = "v21",
                        ["k24"] = "v24",
                    }
                };

                validatorSettings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["name2"] = new Dictionary<string, string>()
                        {
                            ["k21"] = "v21",
                            ["k24"] = "v24",
                        }
                    });
            }

            [Fact]
            public void Should_Translations_PassAllEntries_WhenMoreThanOneKeysAtTopLevelDefined()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
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

            public class TestTranslationHolder : ITranslationsHolder
            {
                public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; set; }
            }
        }
    }
}
