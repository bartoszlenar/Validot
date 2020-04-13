namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Settings;
    using Validot.Settings.Capacities;
    using Validot.Tests.Unit.Settings;
    using Validot.Translations;

    using Xunit;

    public class ValidatorTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new Validator<object>(_ => _);
        }

        [Fact]
        public void Should_ThrowException_When_NullSpecification()
        {
            Action action = () => _ = new Validator<object>(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        public class Settings
        {
            public static IEnumerable<object[]> Should_ThrowException_When_InvalidSettings_Data()
            {
                yield return new object[] { ValidatorSettingsTestData.InvalidBecause_TranslationDictionaryIsNull() };
                yield return new object[] { ValidatorSettingsTestData.InvalidBecause_TranslationValueIsNull() };
                yield return new object[] { ValidatorSettingsTestData.InvalidBecause_CapacityInfoIsNull() };
            }

            [Theory]
            [MemberData(nameof(Should_ThrowException_When_InvalidSettings_Data))]
            public void Should_ThrowException_When_InvalidSettings(IValidatorSettings invalidSettings)
            {
                Action action = () => _ = new Validator<object>(_ => _, invalidSettings);

                action.Should().Throw<Exception>();
            }

            [Fact]
            public void Should_SetSettings_IfProvided()
            {
                var settings = Substitute.For<IValidatorSettings>();

                var capacityInfo = Substitute.For<ICapacityInfo>();

                settings.CapacityInfo.Returns(capacityInfo);

                settings.Translations.Returns(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["test1"] = new Dictionary<string, string>()
                    {
                        ["nested11"] = "n11",
                        ["nested12"] = "n12",
                    },
                    ["test2"] = new Dictionary<string, string>()
                    {
                        ["nested21"] = "n21",
                        ["nested22"] = "n22",
                    },
                });

                var validator = new Validator<object>(_ => _, settings);

                validator.Settings.Should().NotBeNull();

                validator.Settings.Should().BeSameAs(settings);
                validator.Settings.Translations.Should().BeSameAs(settings.Translations);
                validator.Settings.Translations.Should().HaveCount(2);
                validator.Settings.Translations["test1"].Should().HaveCount(2);
                validator.Settings.Translations["test1"]["nested11"].Should().Be("n11");
                validator.Settings.Translations["test1"]["nested12"].Should().Be("n12");
                validator.Settings.Translations["test2"].Should().HaveCount(2);
                validator.Settings.Translations["test2"]["nested21"].Should().Be("n21");
                validator.Settings.Translations["test2"]["nested22"].Should().Be("n22");
                validator.Settings.CapacityInfo.Should().BeSameAs(settings.CapacityInfo);
            }

            [Fact]
            public void Should_SetSettings_AsDefault_IfNotProvided()
            {
                var validator = new Validator<object>(_ => _);

                validator.Settings.Should().NotBeNull();
                validator.Settings.CapacityInfo.Should().BeOfType<DisabledCapacityInfo>();

                validator.Settings.Translations.Keys.Should().ContainSingle("English");

                var defaultEnglishTranslation = validator.Settings.Translations["English"];

                defaultEnglishTranslation.Should().NotBeSameAs(Translation.English);

                defaultEnglishTranslation.Should().HaveCount(Translation.English.Count);

                foreach (var pair in Translation.English)
                {
                    defaultEnglishTranslation[pair.Key].Should().Be(pair.Value);
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForErrorsMap_Data), MemberType = typeof(ValidationTestData))]
        public void Should_HaveErrorMap(string name, Specification<ValidationTestData.TestClass> specification, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> rawErrorsExpectations)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldHaveErrorMap(rawErrorsExpectations);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForValidation_Data), MemberType = typeof(ValidationTestData))]
        public void Should_Validate(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass testClass, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> rawErrorsExpectations, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldValidateAndHaveResult(testClass, false, rawErrorsExpectations, exceptionCase);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForValidationWithFailFast_Data), MemberType = typeof(ValidationTestData))]
        public void Should_Validate_AndFailFast(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass testClass, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> rawErrorsExpectations, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldValidateAndHaveResult(testClass, true, rawErrorsExpectations, exceptionCase);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForIsValid_Data), MemberType = typeof(ValidationTestData))]
        public void Should_IsValid_Return_True_If_NoErrors(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass testClass, bool expectedIsValid, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldHaveIsValueTrueIfNoErrors(testClass, expectedIsValid, exceptionCase);
        }

        [Fact]
        public void Factory_Should_NotBeNull()
        {
            Validator.Factory.Should().NotBeNull();
        }
    }
}
