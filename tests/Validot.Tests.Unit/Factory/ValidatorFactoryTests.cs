namespace Validot.Tests.Unit.Factory
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Factory;
    using Validot.Settings;
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

                validator.Settings.ReferenceLoopProtection.Should().BeFalse();
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

                validator.Settings.ReferenceLoopProtection.Should().BeTrue();
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

                validator.Settings.ReferenceLoopProtection.Should().BeTrue();
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

                validator.Settings.ReferenceLoopProtection.Should().BeTrue();
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

                validator.Settings.ReferenceLoopProtection.Should().BeFalse();
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

                public bool ReferenceLoopProtection { get; set; }
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
