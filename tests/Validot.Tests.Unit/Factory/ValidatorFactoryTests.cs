namespace Validot.Tests.Unit.Factory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AssemblyWithHolders;
    using FluentAssertions;
    using Validot.Factory;
    using Validot.Settings;
    using Validot.Testing;
    using Validot.Tests.Unit.Settings;
    using Validot.Tests.Unit.Translations;
    using Validot.Translations;
    using Xunit;

    public class ValidatorFactoryTests
    {
        public class ValidationWhenFromSpecification
        {
            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForTemplate_Data), MemberType = typeof(ValidationTestData))]
            public void Should_HaveTemplate(string name, Specification<ValidationTestData.TestClass> specification, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases)
            {
                _ = name;

                var validator = Validator.Factory.Create(specification);

                validator.ShouldHaveTemplate(errorCases);
            }

            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForValidation_Data), MemberType = typeof(ValidationTestData))]
            public void Should_Validate(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
            {
                _ = name;

                var validator = Validator.Factory.Create(specification);

                validator.ShouldValidateAndHaveResult(model, false, errorCases, exceptionCase);
            }

            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForValidationWithFailFast_Data), MemberType = typeof(ValidationTestData))]
            public void Should_Validate_AndFailFast(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
            {
                _ = name;

                var validator = Validator.Factory.Create(specification);

                validator.ShouldValidateAndHaveResult(model, true, errorCases, exceptionCase);
            }

            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForIsValid_Data), MemberType = typeof(ValidationTestData))]
            public void Should_IsValid_Return_True_If_NoErrors(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, bool expectedIsValid, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
            {
                _ = name;

                var validator = Validator.Factory.Create(specification);

                validator.ShouldHaveIsValidTrueIfNoErrors(model, expectedIsValid, exceptionCase);
            }
        }

        public class ValidationWhenFromHolder
        {
            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForTemplate_Data), MemberType = typeof(ValidationTestData))]
            public void Should_HaveTemplate(string name, Specification<ValidationTestData.TestClass> specification, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases)
            {
                _ = name;

                var holder = new TestClassSpecificationHolder()
                {
                    Specification = specification
                };

                var validator = Validator.Factory.Create(holder);

                validator.ShouldHaveTemplate(errorCases);
            }

            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForValidation_Data), MemberType = typeof(ValidationTestData))]
            public void Should_Validate(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
            {
                _ = name;

                var holder = new TestClassSpecificationHolder()
                {
                    Specification = specification
                };

                var validator = Validator.Factory.Create(holder);

                validator.ShouldValidateAndHaveResult(model, false, errorCases, exceptionCase);
            }

            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForValidationWithFailFast_Data), MemberType = typeof(ValidationTestData))]
            public void Should_Validate_AndFailFast(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
            {
                _ = name;

                var holder = new TestClassSpecificationHolder()
                {
                    Specification = specification
                };

                var validator = Validator.Factory.Create(holder);

                validator.ShouldValidateAndHaveResult(model, true, errorCases, exceptionCase);
            }

            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForIsValid_Data), MemberType = typeof(ValidationTestData))]
            public void Should_IsValid_Return_True_If_NoErrors(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, bool expectedIsValid, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
            {
                _ = name;

                var holder = new TestClassSpecificationHolder()
                {
                    Specification = specification
                };

                var validator = Validator.Factory.Create(holder);

                validator.ShouldHaveIsValidTrueIfNoErrors(model, expectedIsValid, exceptionCase);
            }
        }

        public class SettingsFromInlineBuilder
        {
            [Fact]
            public void Should_LockSettings()
            {
                var validator = Validator.Factory.Create<object>(s => s, s => s);

                ((ValidatorSettings)validator.Settings).IsLocked.Should().BeTrue();
            }

            [Fact]
            public void Should_SetSettings()
            {
                var validator = Validator.Factory.Create<object>(
                    s => s,
                    s => s
                        .WithTranslation("a", "a", "a")
                        .WithTranslation("a", "b", "c")
                        .WithTranslation("x", "y", "z")
                        .WithReferenceLoopProtectionDisabled()
                );

                validator.Settings.Should().NotBeNull();
                validator.Settings.Translations.Should().NotBeNull();
                validator.Settings.Translations.ShouldBeLikeTranslations(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["English"] = Translation.English,
                    ["a"] = new Dictionary<string, string>()
                    {
                        ["a"] = "a",
                        ["b"] = "c"
                    },
                    ["x"] = new Dictionary<string, string>()
                    {
                        ["y"] = "z"
                    }
                });

                validator.Settings.ReferenceLoopProtectionEnabled.Should().BeFalse();
            }

            [Fact]
            public void Should_SetSettings_WhenSpecificationIsFromHolder()
            {
                var holder = new TestClassSpecificationHolder()
                {
                    Specification = s => s
                };

                var validator = Validator.Factory.Create(
                    holder,
                    s => s
                        .WithTranslation("a", "a", "a")
                        .WithTranslation("a", "b", "c")
                        .WithTranslation("x", "y", "z")
                        .WithReferenceLoopProtection()
                );

                validator.Settings.Should().NotBeNull();
                validator.Settings.Translations.Should().NotBeNull();
                validator.Settings.Translations.ShouldBeLikeTranslations(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["English"] = Translation.English,
                    ["a"] = new Dictionary<string, string>()
                    {
                        ["a"] = "a",
                        ["b"] = "c"
                    },
                    ["x"] = new Dictionary<string, string>()
                    {
                        ["y"] = "z"
                    }
                });

                validator.Settings.ReferenceLoopProtectionEnabled.Should().BeTrue();
            }

            [Fact]
            public void Should_ThrowException_When_PassingExternalSettings()
            {
                Action action = () => _ = Validator.Factory.Create<object>(s => s, s => new ValidatorSettings());

                var exception = action.Should().ThrowExactly<InvalidOperationException>().And;
                exception.Message.Should().Be("Validator settings fluent API should return the same reference as received.");
            }
        }

        public class SettingsFromHolder
        {
            [Fact]
            public void Should_LockSettings()
            {
                var holder = new TestClassSpecificationAndSettingsHolder()
                {
                    Specification = s => s,
                    Settings = s => s
                };

                var validator = Validator.Factory.Create(holder);

                ((ValidatorSettings)validator.Settings).IsLocked.Should().BeTrue();
            }

            [Fact]
            public void Should_LockSettings_When_OverridenByInlineBuilder()
            {
                var holder = new TestClassSpecificationAndSettingsHolder()
                {
                    Specification = s => s,
                    Settings = s => s
                };

                var validator = Validator.Factory.Create(holder, s => s);

                ((ValidatorSettings)validator.Settings).IsLocked.Should().BeTrue();
            }

            [Fact]
            public void Should_SetSettings()
            {
                var holder = new TestClassSpecificationAndSettingsHolder()
                {
                    Specification = s => s,
                    Settings = s => s
                        .WithTranslation("a", "a", "a")
                        .WithTranslation("a", "b", "c")
                        .WithTranslation("x", "y", "z")
                        .WithReferenceLoopProtection()
                };

                var validator = Validator.Factory.Create(holder);

                validator.Settings.Should().NotBeNull();
                validator.Settings.Translations.Should().NotBeNull();
                validator.Settings.Translations.ShouldBeLikeTranslations(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["English"] = Translation.English,
                    ["a"] = new Dictionary<string, string>()
                    {
                        ["a"] = "a",
                        ["b"] = "c"
                    },
                    ["x"] = new Dictionary<string, string>()
                    {
                        ["y"] = "z"
                    }
                });

                validator.Settings.ReferenceLoopProtectionEnabled.Should().BeTrue();
            }

            [Fact]
            public void Should_InlineSettings_Overwrite_SettingsFromHolder()
            {
                var holder = new TestClassSpecificationAndSettingsHolder()
                {
                    Specification = s => s,
                    Settings = s => s
                        .WithTranslation("a", "a", "AAA")
                        .WithTranslation("x", "y", "ZZZ")
                        .WithReferenceLoopProtectionDisabled()
                };

                var validator = Validator.Factory.Create(holder, s => s
                    .WithTranslation("a", "a", "a")
                    .WithTranslation("a", "b", "c")
                    .WithTranslation("x", "y", "z")
                    .WithReferenceLoopProtection()
                );

                validator.Settings.Should().NotBeNull();
                validator.Settings.Translations.Should().NotBeNull();
                validator.Settings.Translations.ShouldBeLikeTranslations(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["English"] = Translation.English,
                    ["a"] = new Dictionary<string, string>()
                    {
                        ["a"] = "a",
                        ["b"] = "c"
                    },
                    ["x"] = new Dictionary<string, string>()
                    {
                        ["y"] = "z"
                    }
                });

                validator.Settings.ReferenceLoopProtectionEnabled.Should().BeTrue();
            }

            [Fact]
            public void Should_ThrowException_When_PassingExternalSettings_UsingHolder()
            {
                var holder = new TestClassSpecificationAndSettingsHolder()
                {
                    Specification = s => s,
                    Settings = s => new ValidatorSettings()
                };

                Action action = () => _ = Validator.Factory.Create(holder);

                var exception = action.Should().ThrowExactly<InvalidOperationException>().And;
                exception.Message.Should().Be("Validator settings fluent API should return the same reference as received.");
            }

            [Fact]
            public void Should_ThrowException_When_PassingExternalSettings_UsingHolder_AndInlineBuilder()
            {
                var holder = new TestClassSpecificationAndSettingsHolder()
                {
                    Specification = s => s,
                    Settings = s => s
                };

                Action action = () => _ = Validator.Factory.Create(holder, s => new ValidatorSettings());

                var exception = action.Should().ThrowExactly<InvalidOperationException>().And;
                exception.Message.Should().Be("Validator settings fluent API should return the same reference as received.");
            }

            [Fact]
            public void Should_ThrowException_When_SpecificationHolder_IsNull()
            {
                Action action = () => _ = Validator.Factory.Create(null as ISpecificationHolder<object>);

                action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("specificationHolder");
            }

            [Fact]
            public void Should_ThrowException_When_SpecificationHolder_ContainsNullSpecification()
            {
                var holder = new TestClassSpecificationAndSettingsHolder()
                {
                    Specification = null
                };

                Action action = () => _ = Validator.Factory.Create(holder);

                var exception = action.Should().Throw<ArgumentException>().And;

                exception.ParamName.Should().Be("specificationHolder");
                exception.Message.Should().StartWith("ISettingsHolder can't have null Settings");
            }
        }

        public class SettingsFromObject
        {
            [Fact]
            public void Should_LockSettings()
            {
                var validatorWithSettings = Validator.Factory.Create<object>(
                    s => s,
                    s => s
                );

                var validator = Validator.Factory.Create<object>(
                    s => s,
                    validatorWithSettings.Settings
                );

                ((ValidatorSettings)validator.Settings).IsLocked.Should().BeTrue();
            }

            [Fact]
            public void Should_SetSettings()
            {
                var validatorWithSettings = Validator.Factory.Create<object>(
                    s => s,
                    s => s
                        .WithTranslation("a", "a", "a")
                        .WithTranslation("a", "b", "c")
                        .WithTranslation("x", "y", "z")
                        .WithReferenceLoopProtectionDisabled()
                );

                var validator = Validator.Factory.Create<object>(
                    s => s,
                    validatorWithSettings.Settings
                );

                validator.Settings.Should().NotBeNull();
                validator.Settings.Should().BeSameAs(validatorWithSettings.Settings);

                validator.Settings.Translations.Should().NotBeNull();
                validator.Settings.Translations.ShouldBeLikeTranslations(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["English"] = Translation.English,
                    ["a"] = new Dictionary<string, string>()
                    {
                        ["a"] = "a",
                        ["b"] = "c"
                    },
                    ["x"] = new Dictionary<string, string>()
                    {
                        ["y"] = "z"
                    }
                });

                validator.Settings.ReferenceLoopProtectionEnabled.Should().BeFalse();
            }

            [Fact]
            public void Should_ThrowException_When_NullSettings()
            {
                Action action = () => _ = Validator.Factory.Create<object>(s => s, null as IValidatorSettings);

                action.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("settings");
            }

            [Fact]
            public void Should_ThrowException_When_CustomSettingsInterfaceImplementation()
            {
                Action action = () => _ = Validator.Factory.Create<object>(s => s, new CustomSettings());

                var exception = action.Should().ThrowExactly<ArgumentException>().And;
                exception.ParamName.Should().Be("settings");
                exception.Message.Should().StartWith("Custom IValidatorSettings implementations are not supported.");
            }

            internal class CustomSettings : IValidatorSettings
            {
                public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; set; }

                public bool ReferenceLoopProtectionEnabled { get; set; }
            }
        }

        public class Translating
        {
            [Fact]
            public void Should_WithTranslation_AddNewTranslations_AddingNamesWithDictionaries()
            {
                var dictionary1 = new Dictionary<string, string>()
                {
                    ["k11"] = "v11",
                    ["k12"] = "v12",
                    ["k13"] = "WILL_BE_OVERWRITTEN",
                    ["k14"] = "WILL_BE_OVERWRITTEN",
                };

                var dictionary2 = new Dictionary<string, string>()
                {
                    ["k21"] = "WILL_BE_OVERWRITTEN",
                    ["k22"] = "WILL_BE_OVERWRITTEN",
                    ["k23"] = "v23",
                    ["k24"] = "v24",
                };

                var dictionary3 = new Dictionary<string, string>()
                {
                    ["k13"] = "v13",
                    ["k14"] = "v14",
                };

                var dictionary4 = new Dictionary<string, string>()
                {
                    ["k21"] = "v21",
                    ["k22"] = "v22",
                };

                var validator = Validator.Factory.Create<object>(s => s, o => o
                    .WithTranslation("name1", dictionary1)
                    .WithTranslation("name2", dictionary2)
                    .WithTranslation("name1", dictionary3)
                    .WithTranslation("name2", dictionary4));

                validator.Settings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["English"] = Translation.English,
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
            public void Should_WithTranslation_AddNewTranslations_AddingFullDictionary()
            {
                var dictionary1 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name1"] = new Dictionary<string, string>()
                    {
                        ["k11"] = "WILL_BE_OVERWRITTEN",
                        ["k12"] = "v12",
                    },
                    ["name2"] = new Dictionary<string, string>()
                    {
                        ["k23"] = "v23",
                        ["k24"] = "v24",
                    }
                };

                var dictionary2 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name1"] = new Dictionary<string, string>()
                    {
                        ["k13"] = "v13",
                        ["k14"] = "v14",
                    },
                    ["name2"] = new Dictionary<string, string>()
                    {
                        ["k21"] = "v21",
                        ["k22"] = "WILL_BE_OVERWRITTEN",
                    }
                };

                var dictionary3 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name1"] = new Dictionary<string, string>()
                    {
                        ["k11"] = "v11",
                    },
                    ["name2"] = new Dictionary<string, string>()
                    {
                        ["k22"] = "v22",
                    }
                };

                var validator = Validator.Factory.Create<object>(s => s, o => o
                    .WithTranslation(dictionary1)
                    .WithTranslation(dictionary2)
                    .WithTranslation(dictionary3));

                validator.Settings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["English"] = Translation.English,
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
            public void Should_WithTranslation_ChangeDefaultTranslation()
            {
                var dictionary1 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name"] = new Dictionary<string, string>()
                    {
                        ["k1"] = "v1",
                    },
                    ["English"] = new Dictionary<string, string>()
                    {
                        ["Global.Required"] = "OVERWRITTEN_REQUIRED",
                        ["TimeSpanType.GreaterThanOrEqualTo"] = "OVERWRITTEN_1"
                    }
                };

                var dictionary2 = new Dictionary<string, string>()
                {
                    ["Global.Required"] = "DOUBLE_OVERWRITTEN_REQUIRED",
                    ["Tests.Email"] = "OVERWRITTEN_2",
                };

                var dictionary3 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name"] = new Dictionary<string, string>()
                    {
                        ["k2"] = "v2",
                    },
                    ["English"] = new Dictionary<string, string>()
                    {
                        ["TotallyNewKey"] = "NEW_KEY",
                        ["Tests.Email"] = "DOUBLE_OVERWRITTEN_2",
                    }
                };

                var validator = Validator.Factory.Create<object>(s => s, o => o
                    .WithTranslation(dictionary1)
                    .WithTranslation("English", dictionary2)
                    .WithTranslation(dictionary3));

                var expectedModifiedEnglish = new Dictionary<string, string>();

                foreach (var pair in Translation.English)
                {
                    expectedModifiedEnglish.Add(pair.Key, pair.Value);
                }

                expectedModifiedEnglish.Add("TotallyNewKey", "NEW_KEY");

                expectedModifiedEnglish["Tests.Email"] = "DOUBLE_OVERWRITTEN_2";
                expectedModifiedEnglish["Global.Required"] = "DOUBLE_OVERWRITTEN_REQUIRED";
                expectedModifiedEnglish["TimeSpanType.GreaterThanOrEqualTo"] = "OVERWRITTEN_1";

                validator.Settings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["English"] = expectedModifiedEnglish,
                        ["name"] = new Dictionary<string, string>()
                        {
                            ["k1"] = "v1",
                            ["k2"] = "v2",
                        },
                    });
            }

            [Fact]
            public void Should_AddTranslationFromSettingsHolder()
            {
                var holder = new TestClassSpecificationAndTranslationHolder()
                {
                    Specification = c => c,
                    Settings = s => s
                        .WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                        {
                            ["name"] = new Dictionary<string, string>()
                            {
                                ["k1"] = "v1",
                            },
                            ["English"] = new Dictionary<string, string>()
                            {
                                ["Global.Required"] = "OVERWRITTEN",
                                ["TotallyNewKey"] = "NEW",
                            }
                        })
                };

                var validator = Validator.Factory.Create(holder);

                var expectedModifiedEnglish = new Dictionary<string, string>();

                foreach (var pair in Translation.English)
                {
                    expectedModifiedEnglish.Add(pair.Key, pair.Value);
                }

                expectedModifiedEnglish.Add("TotallyNewKey", "NEW");
                expectedModifiedEnglish["Global.Required"] = "OVERWRITTEN";

                validator.Settings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["English"] = expectedModifiedEnglish,
                        ["name"] = new Dictionary<string, string>()
                        {
                            ["k1"] = "v1",
                        },
                    });
            }

            [Fact]
            public void Should_AddTranslationFromSettingsHolder_AndModifyItByWithTranslationFromSettings()
            {
                var holder = new TestClassSpecificationAndTranslationHolder()
                {
                    Specification = c => c,
                    Settings = s => s
                        .WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                        {
                            ["name"] = new Dictionary<string, string>()
                            {
                                ["k1"] = "v1",
                            },
                            ["English"] = new Dictionary<string, string>()
                            {
                                ["Global.Required"] = "OVERWRITTEN",
                                ["TotallyNewKey"] = "NEW",
                            }
                        })
                };

                var dictionary = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["name"] = new Dictionary<string, string>()
                    {
                        ["k2"] = "v2",
                    },
                    ["English"] = new Dictionary<string, string>()
                    {
                        ["Global.Required"] = "DOUBLE_OVERWRITTEN",
                        ["TimeSpanType.GreaterThanOrEqualTo"] = "ANOTHER_OVERWRITTEN",
                        ["TotallyNewKey2"] = "NEW2",
                    }
                };

                var validator = Validator.Factory.Create(holder, o => o.WithTranslation(dictionary));

                var expectedModifiedEnglish = new Dictionary<string, string>();

                foreach (var pair in Translation.English)
                {
                    expectedModifiedEnglish.Add(pair.Key, pair.Value);
                }

                expectedModifiedEnglish.Add("TotallyNewKey", "NEW");
                expectedModifiedEnglish.Add("TotallyNewKey2", "NEW2");
                expectedModifiedEnglish["Global.Required"] = "DOUBLE_OVERWRITTEN";
                expectedModifiedEnglish["TimeSpanType.GreaterThanOrEqualTo"] = "ANOTHER_OVERWRITTEN";

                validator.Settings.Translations.ShouldBeLikeTranslations(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>()
                    {
                        ["English"] = expectedModifiedEnglish,
                        ["name"] = new Dictionary<string, string>()
                        {
                            ["k1"] = "v1",
                            ["k2"] = "v2",
                        },
                    });
            }

            public class TestClassSpecificationAndTranslationHolder : ISpecificationHolder<ValidationTestData.TestClass>, ISettingsHolder
            {
                public Specification<ValidationTestData.TestClass> Specification { get; set; }

                public Func<ValidatorSettings, ValidatorSettings> Settings { get; set; }
            }
        }

        public class FetchHolders
        {
            [Fact]
            public void Should_FetchAllHoldersFromAssembly()
            {
                var holders = Validator.Factory.FetchHolders(typeof(AssemblyWithHoldersHook).Assembly);

                holders.Should().HaveCount(13);

                holders.Should().Contain(h => h.HolderType == typeof(HolderOfDecimalSpecification) && h.SpecifiedType == typeof(decimal) && !h.HoldsSettings);
                holders.Should().Contain(h => h.HolderType == typeof(HolderOfIntSpecificationAndSettings) && h.SpecifiedType == typeof(int) && h.HoldsSettings);

                holders.Should().Contain(h => h.HolderType == typeof(HolderOfStringSpecification) && h.SpecifiedType == typeof(string) && !h.HoldsSettings);
                holders.Should().Contain(h => h.HolderType == typeof(HolderOfStringSpecificationAndSettings) && h.SpecifiedType == typeof(string) && h.HoldsSettings);

                holders.Should().Contain(h => h.HolderType == typeof(HolderOfMultipleSpecifications) && h.SpecifiedType == typeof(DateTime) && !h.HoldsSettings);
                holders.Should().Contain(h => h.HolderType == typeof(HolderOfMultipleSpecifications) && h.SpecifiedType == typeof(DateTimeOffset) && !h.HoldsSettings);

                holders.Should().Contain(h => h.HolderType == typeof(HolderOfMultipleSpecificationsAndSettings) && h.SpecifiedType == typeof(float) && h.HoldsSettings);
                holders.Should().Contain(h => h.HolderType == typeof(HolderOfMultipleSpecificationsAndSettings) && h.SpecifiedType == typeof(double) && h.HoldsSettings);

                holders.Should().Contain(h => h.HolderType == typeof(NestedHolders.NestedHolderOfBoolSpecification) && h.SpecifiedType == typeof(bool) && !h.HoldsSettings);
                holders.Should().Contain(h => h.HolderType == typeof(NestedHolders.NestedHolderOfStringSpecification) && h.SpecifiedType == typeof(string) && !h.HoldsSettings);
                holders.Should().Contain(h => h.HolderType == typeof(NestedHolders.NestedHolderOfStringSpecificationAndSettings) && h.SpecifiedType == typeof(string) && h.HoldsSettings);

                holders.Should().Contain(h => h.HolderType == typeof(PrivateSpecificationHolder) && h.SpecifiedType == typeof(string) && !h.HoldsSettings);
                holders.Should().Contain(h => h.HolderType == typeof(PrivateSpecificationAndSettingsHolder) && h.SpecifiedType == typeof(string) && h.HoldsSettings);
            }

            [Fact]
            public void Should_FetchAllHoldersFromMultipleAssemblies()
            {
                var thisTestsHolders = Validator.Factory.FetchHolders(typeof(ValidatorFactoryTests).Assembly);

                var separateAssemblyHolders = Validator.Factory.FetchHolders(typeof(AssemblyWithHoldersHook).Assembly);

                var holders = Validator.Factory.FetchHolders(typeof(AssemblyWithHoldersHook).Assembly, typeof(ValidatorFactoryTests).Assembly);

                holders.Should().HaveCount(separateAssemblyHolders.Count + thisTestsHolders.Count);

                foreach (var holder in separateAssemblyHolders)
                {
                    holders.Should().Contain(h => h.HolderType == holder.HolderType && h.SpecifiedType == holder.SpecifiedType && h.HoldsSettings == holder.HoldsSettings && h.ValidatorType == holder.ValidatorType);
                }

                foreach (var holder in thisTestsHolders)
                {
                    holders.Should().Contain(h => h.HolderType == holder.HolderType && h.SpecifiedType == holder.SpecifiedType && h.HoldsSettings == holder.HoldsSettings && h.ValidatorType == holder.ValidatorType);
                }
            }

            [Fact]
            public void Should_FetchAllHoldersFromDomainAssemblies()
            {
                var thisTestsHolders = Validator.Factory.FetchHolders(typeof(ValidatorFactoryTests).Assembly);

                var separateAssemblyHolders = Validator.Factory.FetchHolders(typeof(AssemblyWithHoldersHook).Assembly);

                var holders = Validator.Factory.FetchHolders();

                holders.Should().HaveCount(separateAssemblyHolders.Count + thisTestsHolders.Count);

                foreach (var holder in separateAssemblyHolders)
                {
                    holders.Should().Contain(h => h.HolderType == holder.HolderType && h.SpecifiedType == holder.SpecifiedType && h.HoldsSettings == holder.HoldsSettings && h.ValidatorType == holder.ValidatorType);
                }

                foreach (var holder in thisTestsHolders)
                {
                    holders.Should().Contain(h => h.HolderType == holder.HolderType && h.SpecifiedType == holder.SpecifiedType && h.HoldsSettings == holder.HoldsSettings && h.ValidatorType == holder.ValidatorType);
                }
            }

            [Fact]
            public void Should_FetchAllHolders_And_CreateValidatorsOutOfThem()
            {
                var holders = Validator.Factory.FetchHolders(typeof(AssemblyWithHoldersHook).Assembly);

                var holderOfDecimalSpecificationValidator = (Validator<decimal>)holders.Single(h => h.HolderType == typeof(HolderOfDecimalSpecification) && h.SpecifiedType == typeof(decimal) && !h.HoldsSettings).CreateValidator();
                holderOfDecimalSpecificationValidator.Validate(10.01M).ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Max value is 10");

                var holderOfIntSpecificationAndSettingsValidator = (Validator<int>)holders.Single(h => h.HolderType == typeof(HolderOfIntSpecificationAndSettings) && h.SpecifiedType == typeof(int) && h.HoldsSettings).CreateValidator();
                holderOfIntSpecificationAndSettingsValidator.Validate(11).ToString("BinaryEnglish").ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "The maximum value is 0b1010");

                var holderOfStringSpecificationValidator = (Validator<string>)holders.Single(h => h.HolderType == typeof(HolderOfStringSpecification) && h.SpecifiedType == typeof(string) && !h.HoldsSettings).CreateValidator();
                holderOfStringSpecificationValidator.Validate("!").ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Text shorter than 3 characters not allowed",
                    "Text containing exclamation mark not allowed");

                var holderOfStringSpecificationAndSettingsValidator = (Validator<string>)holders.Single(h => h.HolderType == typeof(HolderOfStringSpecificationAndSettings) && h.SpecifiedType == typeof(string) && h.HoldsSettings).CreateValidator();
                holderOfStringSpecificationAndSettingsValidator.Validate("").ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Empty string is invalid!",
                    "Only strings of length from 3 to 10 are allowed"
                );

                var holderOfMultipleSpecificationsDateTimeValidator = (Validator<DateTime>)holders.Single(h => h.HolderType == typeof(HolderOfMultipleSpecifications) && h.SpecifiedType == typeof(DateTime) && !h.HoldsSettings).CreateValidator();
                holderOfMultipleSpecificationsDateTimeValidator.Validate(new DateTime(1999, 10, 10)).ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Dates after 1st of Jan'00 are allowed"
                );

                var holderOfMultipleSpecificationsDateTimeOffsetValidator = (Validator<DateTimeOffset>)holders.Single(h => h.HolderType == typeof(HolderOfMultipleSpecifications) && h.SpecifiedType == typeof(DateTimeOffset) && !h.HoldsSettings).CreateValidator();
                holderOfMultipleSpecificationsDateTimeOffsetValidator.Validate(new DateTimeOffset(2077, 10, 10, 10, 10, 10, TimeSpan.Zero)).ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Dates before midnight 1st of Jan'21 are allowed");

                var holderOfMultipleSpecificationsAndSettingsFloatValidator = (Validator<float>)holders.Single(h => h.HolderType == typeof(HolderOfMultipleSpecificationsAndSettings) && h.SpecifiedType == typeof(float) && h.HoldsSettings).CreateValidator();
                holderOfMultipleSpecificationsAndSettingsFloatValidator.Validate(0.99F).ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Minimum value is 1");

                var holderOfMultipleSpecificationsAndSettingsDoubleValidator = (Validator<double>)holders.Single(h => h.HolderType == typeof(HolderOfMultipleSpecificationsAndSettings) && h.SpecifiedType == typeof(double) && h.HoldsSettings).CreateValidator();
                holderOfMultipleSpecificationsAndSettingsDoubleValidator.Validate(10.001D).ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Maximum value is 10"
                );

                var nestedHolderOfBoolSpecificationValidator = (Validator<bool>)holders.Single(h => h.HolderType == typeof(NestedHolders.NestedHolderOfBoolSpecification) && h.SpecifiedType == typeof(bool) && !h.HoldsSettings).CreateValidator();
                nestedHolderOfBoolSpecificationValidator.Validate(false).ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Must be true");

                var nestedHolderOfStringSpecificationValidator = (Validator<string>)holders.Single(h => h.HolderType == typeof(NestedHolders.NestedHolderOfStringSpecification) && h.SpecifiedType == typeof(string) && !h.HoldsSettings).CreateValidator();
                nestedHolderOfStringSpecificationValidator.Validate("").ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Must not be empty");

                var nestedHolderOfStringSpecificationAndSettingsValidator = (Validator<string>)holders.Single(h => h.HolderType == typeof(NestedHolders.NestedHolderOfStringSpecificationAndSettings) && h.SpecifiedType == typeof(string) && h.HoldsSettings).CreateValidator();
                nestedHolderOfStringSpecificationAndSettingsValidator.Validate("").ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Cannot be empty!");

                var privateSpecificationHolderValidator = (Validator<string>)holders.Single(h => h.HolderType == typeof(PrivateSpecificationHolder) && h.SpecifiedType == typeof(string) && !h.HoldsSettings).CreateValidator();
                privateSpecificationHolderValidator.Validate("").ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Must not be empty"
                );

                var privateSpecificationAndSettingsHolderValidator = (Validator<string>)holders.Single(h => h.HolderType == typeof(PrivateSpecificationAndSettingsHolder) && h.SpecifiedType == typeof(string) && h.HoldsSettings).CreateValidator();
                privateSpecificationAndSettingsHolderValidator.Validate("").ToString().ShouldResultToStringHaveLines(
                    ToStringContentType.Messages,
                    "Must not be empty"
                );
                privateSpecificationAndSettingsHolderValidator.Settings.ReferenceLoopProtectionEnabled.Should().BeTrue();
            }

            [Fact]
            public void Should_FetchHolders_And_InitializeValidatorsWithSettings()
            {
                var holders = Validator.Factory.FetchHolders(typeof(AssemblyWithHoldersHook).Assembly);

                var holderOfIntSpecificationAndSettingsValidator = (Validator<int>)holders.Single(h => h.HolderType == typeof(HolderOfIntSpecificationAndSettings) && h.SpecifiedType == typeof(int) && h.HoldsSettings).CreateValidator();

                holderOfIntSpecificationAndSettingsValidator.Settings.ReferenceLoopProtectionEnabled.Should().BeTrue();
                holderOfIntSpecificationAndSettingsValidator.Settings.Translations.Keys.Should().Contain("English");
                holderOfIntSpecificationAndSettingsValidator.Settings.Translations["English"]["Min value is 1"].Should().Be("The minimum value is 1");
                holderOfIntSpecificationAndSettingsValidator.Settings.Translations["English"]["Max value is 10"].Should().Be("The maximum value is 10");

                holderOfIntSpecificationAndSettingsValidator.Settings.Translations.Keys.Should().Contain("BinaryEnglish");
                holderOfIntSpecificationAndSettingsValidator.Settings.Translations["BinaryEnglish"].Should().HaveCount(2);
                holderOfIntSpecificationAndSettingsValidator.Settings.Translations["BinaryEnglish"].Keys.Should().HaveCount(2);
                holderOfIntSpecificationAndSettingsValidator.Settings.Translations["BinaryEnglish"]["Min value is 1"].Should().Be("The minimum value is 0b0001");
                holderOfIntSpecificationAndSettingsValidator.Settings.Translations["BinaryEnglish"]["Max value is 10"].Should().Be("The maximum value is 0b1010");
            }
        }

        [Fact]
        public void Should_HaveDefaultSettings()
        {
            var validator = Validator.Factory.Create<object>(s => s);

            validator.Settings.ShouldBeLikeDefault();
        }

        [Fact]
        public void Should_HaveDefaultSettingsLocked()
        {
            var validator = Validator.Factory.Create<object>(s => s);

            ((ValidatorSettings)validator.Settings).IsLocked.Should().BeTrue();
        }

        public class TestClassSpecificationAndSettingsHolder : ISpecificationHolder<ValidationTestData.TestClass>, ISettingsHolder
        {
            public Specification<ValidationTestData.TestClass> Specification { get; set; }

            public Func<ValidatorSettings, ValidatorSettings> Settings { get; set; }
        }

        public class TestClassSpecificationHolder : ISpecificationHolder<ValidationTestData.TestClass>
        {
            public Specification<ValidationTestData.TestClass> Specification { get; set; }
        }
    }
}
