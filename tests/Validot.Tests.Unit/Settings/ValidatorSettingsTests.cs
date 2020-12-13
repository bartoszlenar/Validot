namespace Validot.Tests.Unit.Settings
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

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
            validatorSettings.ReferenceLoopProtectionEnabled.Should().BeNull();
            validatorSettings.IsLocked.Should().BeFalse();
        }

        [Fact]
        public void Default_Should_HaveEnglishTranslation_And_DisabledStats()
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
            public void Should_WithReferenceLoopProtection_Set_ReferenceLoopProtectionEnabled_To_True()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.WithReferenceLoopProtection();

                validatorSettings.ReferenceLoopProtectionEnabled.Should().BeTrue();
            }

            [Fact]
            public void Should_WithReferenceLoopProtectionDisabled_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var result = validatorSettings.WithReferenceLoopProtectionDisabled();

                result.Should().BeSameAs(validatorSettings);
            }

            [Fact]
            public void Should_WithReferenceLoopProtectionDisabled_Set_ReferenceLoopProtectionEnabled_To_False()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.WithReferenceLoopProtectionDisabled();

                validatorSettings.ReferenceLoopProtectionEnabled.Should().BeFalse();
            }

            [Fact]
            public void Should_WithReferenceLoopProtection_ThrowException_When_Locked()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.IsLocked = true;

                Action action = () =>
                {
                    validatorSettings.WithReferenceLoopProtection();
                };

                action.Should().ThrowExactly<InvalidOperationException>();
            }

            [Fact]
            public void Should_WithReferenceLoopProtectionDisabled_ThrowException_When_Locked()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.IsLocked = true;

                Action action = () =>
                {
                    validatorSettings.WithReferenceLoopProtectionDisabled();
                };

                action.Should().ThrowExactly<InvalidOperationException>();
            }
        }

        public class WithTranslation
        {
            [Fact]
            public void Should_ReturnSelf()
            {
                var validatorSettings = new ValidatorSettings();

                var result = validatorSettings.WithTranslation("name", "key", "translation");

                result.Should().BeSameAs(validatorSettings);
            }

            [Fact]
            public void Should_AddEntry()
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
            public void Should_AddMultipleEntries()
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
            public void Should_ThrowException_When_NullArgument(string name, string key, string translation)
            {
                var validatorSettings = new ValidatorSettings();

                Action action = () => validatorSettings.WithTranslation(name, key, translation);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_Locked()
            {
                var validatorSettings = new ValidatorSettings();

                validatorSettings.IsLocked = true;

                Action action = () =>
                {
                    validatorSettings.WithTranslation("a", "b", "c");
                };

                action.Should().ThrowExactly<InvalidOperationException>();
            }
        }
    }
}
