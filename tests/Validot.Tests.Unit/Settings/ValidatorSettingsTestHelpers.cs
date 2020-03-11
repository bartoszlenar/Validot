namespace Validot.Tests.Unit.Settings
{
    using FluentAssertions;

    using Validot.Settings;
    using Validot.Settings.Capacities;
    using Validot.Translations;

    public static class ValidatorSettingsTestHelpers
    {
        public static void ShouldBeLikeDefault(this IValidatorSettings @this)
        {
            @this.Should().NotBeNull();
            @this.CapacityInfo.Should().NotBeNull();
            @this.CapacityInfo.Should().BeOfType<DisabledCapacityInfo>();

            @this.Translations.Keys.Should().HaveCount(1);
            @this.Translations.Keys.Should().Contain("English");
            @this.Translations["English"].Should().NotBeNull();

            foreach (var pair in Translation.English)
            {
                @this.Translations["English"].Keys.Should().Contain(pair.Key);
                @this.Translations["English"][pair.Key].Should().Be(pair.Value);
            }
        }
    }
}
