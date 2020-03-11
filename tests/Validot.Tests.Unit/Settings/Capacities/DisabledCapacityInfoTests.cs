namespace Validot.Tests.Unit.Settings.Capacities
{
    using System;

    using FluentAssertions;

    using Validot.Settings.Capacities;

    using Xunit;

    public class DisabledCapacityInfoTests
    {
        [Fact]
        public void Should_NotBeFeedableCapacityInfo()
        {
            typeof(DisabledCapacityInfo).Should().Implement<ICapacityInfo>();
            typeof(DisabledCapacityInfo).Should().NotImplement<IFeedableCapacityInfo>();
        }

        [Fact]
        public void Should_ShouldUse_Be_False()
        {
            new DisabledCapacityInfo().ShouldRead.Should().BeFalse();
        }

        [Fact]
        public void Should_ErrorsPathsCapacity_Be_Zero()
        {
            Action action = () => _ = new DisabledCapacityInfo().ErrorsPathsCapacity;

            action.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Should_TryGetErrorsCapacityForPath_Be_Empty()
        {
            Action action = () => _ = new DisabledCapacityInfo().TryGetErrorsCapacityForPath("test", out _);

            action.Should().ThrowExactly<InvalidOperationException>();
        }
    }
}
