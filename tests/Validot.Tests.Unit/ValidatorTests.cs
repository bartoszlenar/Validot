namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Settings;
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
            [Fact]
            public void Should_HaveLockedSettings()
            {
                var validator = new Validator<object>(_ => _);

                validator.Settings.IsLocked.Should().BeTrue();
            }

            [Fact]
            public void Should_LockSettingsAfterPassingToValidator()
            {
                var settings = new ValidatorSettings();

                settings.IsLocked.Should().BeFalse();

                _ = new Validator<object>(_ => _, settings);

                settings.IsLocked.Should().BeTrue();
            }

            [Fact]
            public void Should_SetSettings_IfProvided()
            {
                var settings = new ValidatorSettings();

                settings.WithReferenceLoopProtectionDisabled();

                settings.WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>()
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
                validator.Settings.ReferenceLoopProtection.Should().BeFalse();

                validator.Settings.IsLocked.Should().BeTrue();
            }

            [Fact]
            public void Should_SetSettings_AsDefault_IfNotProvided()
            {
                var validator = new Validator<object>(_ => _);

                validator.Settings.Should().NotBeNull();

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
        [MemberData(nameof(ValidationTestData.CasesForTemplate_Data), MemberType = typeof(ValidationTestData))]
        public void Should_HaveTemplate(string name, Specification<ValidationTestData.TestClass> specification, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldHaveTemplate(errorCases);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForValidation_Data), MemberType = typeof(ValidationTestData))]
        public void Should_Validate(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldValidateAndHaveResult(model, false, errorCases, exceptionCase);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForValidationWithFailFast_Data), MemberType = typeof(ValidationTestData))]
        public void Should_Validate_AndFailFast(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldValidateAndHaveResult(model, true, errorCases, exceptionCase);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForIsValid_Data), MemberType = typeof(ValidationTestData))]
        public void Should_IsValid_Return_True_If_NoErrors(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, bool expectedIsValid, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldHaveIsValueTrueIfNoErrors(model, expectedIsValid, exceptionCase);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForReferenceLoop_Data), MemberType = typeof(ValidationTestData))]
        public void Should_Validate_With_ReferenceLoopProtection(string name, bool referenceLoopProtectionEnabled, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var settings = new ValidatorSettings();

            if (referenceLoopProtectionEnabled)
            {
                settings.WithReferenceLoopProtection();
            }
            else
            {
                settings.WithReferenceLoopProtectionDisabled();
            }

            var validator = new Validator<ValidationTestData.TestClass>(specification, settings);

            validator.ShouldValidateAndHaveResult(model, false, errorCases, exceptionCase);
        }

        [Fact]
        public void Factory_Should_NotBeNull()
        {
            Validator.Factory.Should().NotBeNull();
        }
    }
}
