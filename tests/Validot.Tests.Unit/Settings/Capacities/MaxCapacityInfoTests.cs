namespace Validot.Tests.Unit.Settings.Capacities
{
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Settings.Capacities;
    using Validot.Validation;
    using Validot.Validation.Scheme;

    using Xunit;

    public class MaxCapacityInfoTests
    {
        [Fact]
        public void Should_BeFeedableCapacityInfo()
        {
            typeof(MaxCapacityInfo).Should().Implement<ICapacityInfo>();
            typeof(MaxCapacityInfo).Should().Implement<IFeedableCapacityInfo>();
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new MaxCapacityInfo();
        }

        [Fact]
        public void Should_Initialize_WithDefaultValues()
        {
            var maxCapacityInfo = new MaxCapacityInfo();

            maxCapacityInfo.ShouldRead.Should().BeTrue();
            maxCapacityInfo.ShouldFeed.Should().BeTrue();
            maxCapacityInfo.ErrorsPathsCapacity.Should().Be(0);
        }

        [Fact]
        public void Should_Feed()
        {
            var maxCapacityInfo = new MaxCapacityInfo();

            var errorsHolder = Substitute.For<IErrorsHolder>();

            errorsHolder.Errors.Returns(new Dictionary<string, List<int>>()
            {
                [""] = new List<int>() { 1 },
                ["a"] = new List<int>() { 1, 2 },
                ["a.b"] = new List<int>() { 1, 2, 3 }
            });

            maxCapacityInfo.InjectHelpers(ModelSchemeFactory.CapacityInfoHelpers);

            maxCapacityInfo.Feed(errorsHolder);

            maxCapacityInfo.ErrorsPathsCapacity.Should().Be(3);

            maxCapacityInfo.TryGetErrorsCapacityForPath("", out var capacity1).Should().BeTrue();
            maxCapacityInfo.TryGetErrorsCapacityForPath("a", out var capacity2).Should().BeTrue();
            maxCapacityInfo.TryGetErrorsCapacityForPath("a.b", out var capacity3).Should().BeTrue();

            capacity1.Should().Be(1);
            capacity2.Should().Be(2);
            capacity3.Should().Be(3);
        }

        [Fact]
        public void Should_Feed_IgnoreIncomingData_When_AlreadyFed()
        {
            var maxCapacityInfo = new MaxCapacityInfo();

            var errorsHolder1 = Substitute.For<IErrorsHolder>();

            errorsHolder1.Errors.Returns(new Dictionary<string, List<int>>()
            {
                [""] = new List<int>() { 1 },
                ["a"] = new List<int>() { 1, 2 },
                ["a.b"] = new List<int>() { 1, 2, 3 }
            });

            var errorsHolder2 = Substitute.For<IErrorsHolder>();

            errorsHolder2.Errors.Returns(new Dictionary<string, List<int>>()
            {
                ["a.b.c"] = new List<int>() { 1, 2, 3, 4 },
                ["a.b.c.d.e.f"] = new List<int>() { 1, 2, 3, 4 },
                [""] = new List<int>() { 1, 2 },
                ["a"] = new List<int>() { 1, 2, 3, 5, 6, 7, 8, 9, 0 }
            });

            maxCapacityInfo.InjectHelpers(ModelSchemeFactory.CapacityInfoHelpers);

            maxCapacityInfo.Feed(errorsHolder1);
            maxCapacityInfo.ErrorsPathsCapacity.Should().Be(3);

            maxCapacityInfo.TryGetErrorsCapacityForPath("", out var capacity1).Should().BeTrue();
            maxCapacityInfo.TryGetErrorsCapacityForPath("a", out var capacity2).Should().BeTrue();
            maxCapacityInfo.TryGetErrorsCapacityForPath("a.b", out var capacity3).Should().BeTrue();

            capacity1.Should().Be(1);
            capacity2.Should().Be(2);
            capacity3.Should().Be(3);

            maxCapacityInfo.Feed(errorsHolder2);

            maxCapacityInfo.ErrorsPathsCapacity.Should().Be(3);

            maxCapacityInfo.TryGetErrorsCapacityForPath("", out var capacity11).Should().BeTrue();
            maxCapacityInfo.TryGetErrorsCapacityForPath("a", out var capacity22).Should().BeTrue();
            maxCapacityInfo.TryGetErrorsCapacityForPath("a.b", out var capacity33).Should().BeTrue();
            maxCapacityInfo.TryGetErrorsCapacityForPath("a.b.c", out _).Should().BeFalse();
            maxCapacityInfo.TryGetErrorsCapacityForPath("a.b.c.d.e.f", out _).Should().BeFalse();

            capacity11.Should().Be(1);
            capacity22.Should().Be(2);
            capacity33.Should().Be(3);
        }

        [Fact]
        public void Should_ShouldFeed_BeFalse_When_AlreadyFed()
        {
            var maxCapacityInfo = new MaxCapacityInfo();

            var errorsHolder = Substitute.For<IErrorsHolder>();

            errorsHolder.Errors.Returns(new Dictionary<string, List<int>>()
            {
                [""] = new List<int>() { 1 },
                ["a"] = new List<int>() { 1, 2 },
                ["a.b"] = new List<int>() { 1, 2, 3 }
            });

            maxCapacityInfo.ShouldFeed.Should().BeTrue();
            maxCapacityInfo.Feed(errorsHolder);

            maxCapacityInfo.ShouldFeed.Should().BeFalse();
            maxCapacityInfo.ShouldRead.Should().BeTrue();
        }
    }
}
