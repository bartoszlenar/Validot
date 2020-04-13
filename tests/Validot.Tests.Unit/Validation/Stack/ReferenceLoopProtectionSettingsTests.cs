namespace Validot.Tests.Unit.Validation.Stack
{
    using FluentAssertions;

    using Validot.Validation.Stacks;

    using Xunit;

    public class ReferenceLoopProtectionSettingsTests
    {
        [Fact]
        public void Should_Initialize_RootReferenceModel_WithNull()
        {
            var settings = new ReferenceLoopProtectionSettings();

            settings.RootModelReference.Should().BeNull();
        }

        [Fact]
        public void Should_Initialize_RootReferenceModel()
        {
            var model = new object();

            var settings = new ReferenceLoopProtectionSettings(model);

            settings.RootModelReference.Should().BeSameAs(model);
        }
    }
}
