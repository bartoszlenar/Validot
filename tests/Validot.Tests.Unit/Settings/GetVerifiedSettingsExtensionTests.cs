namespace Validot.Tests.Unit.Settings
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Settings;
    using Validot.Settings.Capacities;

    using Xunit;

    public class GetVerifiedSettingsExtensionTests
    {
        [Fact]
        public void Should_ThrowException_When_NullSettings()
        {
            Action action = () => (null as IValidatorSettings).GetVerified();

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_TranslationsDictionaryIsNull()
        {
            var settings = Substitute.For<IValidatorSettings>();

            settings.Translations.Returns(null as IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>);

            Action action = () => settings.GetVerified();

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_Pass_IfAllValid()
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

            var verified = settings.GetVerified();

            verified.Should().NotBeNull();

            verified.Should().BeSameAs(settings);
            verified.Translations.Should().BeSameAs(settings.Translations);
            verified.Translations.Should().HaveCount(2);
            verified.Translations["test1"].Should().HaveCount(2);
            verified.Translations["test1"]["nested11"].Should().Be("n11");
            verified.Translations["test1"]["nested12"].Should().Be("n12");
            verified.Translations["test2"].Should().HaveCount(2);
            verified.Translations["test2"]["nested21"].Should().Be("n21");
            verified.Translations["test2"]["nested22"].Should().Be("n22");
            verified.CapacityInfo.Should().BeSameAs(capacityInfo);
        }

        [Fact]
        public void Should_ThrowException_When_TranslationDictionaryIsNull()
        {
            var settings = ValidatorSettingsTestData.InvalidBecause_TranslationDictionaryIsNull();

            Action action = () => settings.GetVerified();

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_TranslationValueIsNull()
        {
            var settings = ValidatorSettingsTestData.InvalidBecause_TranslationValueIsNull();

            Action action = () => settings.GetVerified();

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_CapacityInfoIsNull()
        {
            var settings = ValidatorSettingsTestData.InvalidBecause_CapacityInfoIsNull();

            Action action = () => settings.GetVerified();

            action.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
