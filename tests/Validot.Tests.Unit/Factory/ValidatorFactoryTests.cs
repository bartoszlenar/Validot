namespace Validot.Tests.Unit.Factory
{
    using System.Collections.Generic;

    using Validot.Factory;
    using Validot.Tests.Unit.Settings;
    using Validot.Tests.Unit.Translations;
    using Validot.Translations;

    using Xunit;

    public class ValidatorFactoryTests
    {
        public class ValidationWhenFromSpecification
        {
            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForErrorMap_Data), MemberType = typeof(ValidationTestData))]
            public void Should_HaveErrorMap(string name, Specification<ValidationTestData.TestClass> specification, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases)
            {
                _ = name;

                var validator = Validator.Factory.Create(specification);

                validator.ShouldHaveErrorMap(errorCases);
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

                validator.ShouldHaveIsValueTrueIfNoErrors(model, expectedIsValid, exceptionCase);
            }
        }

        public class ValidationWhenFromHolder
        {
            public class TestClassSpecificationHolder : ISpecificationHolder<ValidationTestData.TestClass>
            {
                public Specification<ValidationTestData.TestClass> Specification { get; set; }
            }

            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForErrorMap_Data), MemberType = typeof(ValidationTestData))]
            public void Should_HaveErrorMap(string name, Specification<ValidationTestData.TestClass> specification, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases)
            {
                _ = name;

                var holder = new TestClassSpecificationHolder()
                {
                    Specification = specification
                };

                var validator = Validator.Factory.Create(holder);

                validator.ShouldHaveErrorMap(errorCases);
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

                validator.ShouldHaveIsValueTrueIfNoErrors(model, expectedIsValid, exceptionCase);
            }
        }

        [Fact]
        public void Should_HaveDefaultSettings()
        {
            var validator = Validator.Factory.Create<object>(s => s);

            validator.Settings.ShouldBeLikeDefault();
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
            public void Should_AddTranslationFromHolder()
            {
                var holder = new TestClassSpecificationAndTranslationHolder()
                {
                    Specification = c => c,
                    Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
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
                    }
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
            public void Should_AddTranslationFromHolder_AndModifyItByWithTranslation()
            {
                var holder = new TestClassSpecificationAndTranslationHolder()
                {
                    Specification = c => c,
                    Translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
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
                    }
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

            public class TestClassSpecificationAndTranslationHolder : ISpecificationHolder<ValidationTestData.TestClass>, ITranslationHolder
            {
                public Specification<ValidationTestData.TestClass> Specification { get; set; }

                public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; set; }
            }
        }
    }
}
