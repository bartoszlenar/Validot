namespace Validot.Tests.Unit.Settings
{
    using System.Collections.Generic;

    using NSubstitute;

    using Validot.Settings;
    using Validot.Settings.Capacities;

    public static class ValidatorSettingsTestData
    {
        public static IValidatorSettings InvalidBecause_TranslationDictionaryIsNull()
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
                ["test2"] = null
            });

            return settings;
        }

        public static IValidatorSettings InvalidBecause_TranslationValueIsNull()
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
                    ["nested21"] = null,
                    ["nested22"] = "n22",
                },
            });

            return settings;
        }

        public static IValidatorSettings InvalidBecause_CapacityInfoIsNull()
        {
            var settings = Substitute.For<IValidatorSettings>();

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

            settings.CapacityInfo.Returns(null as ICapacityInfo);

            return settings;
        }
    }
}
