namespace Validot.Tests.Unit.Validation
{
    using System;

    using FluentAssertions;

    using Validot.Validation;

    using Xunit;

    public class ErrorFlagTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new ErrorFlag();
        }

        [Fact]
        public void Should_Initialize_WithCapacity()
        {
            _ = new ErrorFlag(10);
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_WithNegativeCapacity()
        {
            Action action = () => new ErrorFlag(-10);

            action.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        public class IsEnabledAtAnyLevel_After_SetEnabled
        {
            [Fact]
            public void Should_BeFalseByDefault()
            {
                var errorFlag = new ErrorFlag();

                errorFlag.IsEnabledAtAnyLevel.Should().BeFalse();
            }

            [Theory]
            [InlineData(1)]
            [InlineData(10)]
            [InlineData(666)]
            public void Should_BeTrueIfEnabled(int level)
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(level, 1);

                errorFlag.IsEnabledAtAnyLevel.Should().BeTrue();
            }

            [Fact]
            public void Should_BeTrueIfEnabled_MultipleTimes()
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(1, 1);
                errorFlag.SetEnabled(10, 1);
                errorFlag.SetEnabled(666, 1);

                errorFlag.IsEnabledAtAnyLevel.Should().BeTrue();
            }

            [Fact]
            public void Should_BeFalse_AfterLeavingLevel()
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(10, 1);
                errorFlag.LeaveLevelAndTryGetError(10, out _);

                errorFlag.IsEnabledAtAnyLevel.Should().BeFalse();
            }

            [Fact]
            public void Should_BeFalse_AfterLeavingLevel_MultipleTimes()
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(1, 1);
                errorFlag.SetEnabled(10, 1);
                errorFlag.SetEnabled(666, 1);

                errorFlag.IsEnabledAtAnyLevel.Should().BeTrue();

                errorFlag.LeaveLevelAndTryGetError(1, out _);
                errorFlag.IsEnabledAtAnyLevel.Should().BeTrue();

                errorFlag.LeaveLevelAndTryGetError(10, out _);
                errorFlag.IsEnabledAtAnyLevel.Should().BeTrue();

                errorFlag.LeaveLevelAndTryGetError(666, out _);

                errorFlag.IsEnabledAtAnyLevel.Should().BeFalse();
            }
        }

        public class LeaveLevelAndTryGetError
        {
            [Theory]
            [InlineData(10, 666)]
            [InlineData(666, 10)]
            [InlineData(0, 0)]
            public void Should_ReturnTrue_And_SameErrorIdWhenEnabled_When_LevelEnabledAndDetected(int level, int errorId)
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(level, errorId);
                errorFlag.SetDetected(level);

                var tryResult = errorFlag.LeaveLevelAndTryGetError(level, out var errorOnLeaving);

                errorOnLeaving.Should().Be(errorId);
                tryResult.Should().BeTrue();
            }

            [Fact]
            public void Should_ReturnTrue_And_FirstErrorIdWhenEnabled_When_LevelEnabledAndDetected_SameLevelMultipleTimes()
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(10, 1);
                errorFlag.SetEnabled(10, 2);
                errorFlag.SetEnabled(10, 3);
                errorFlag.SetDetected(10);
                errorFlag.SetDetected(10);
                errorFlag.SetDetected(10);

                var tryResult = errorFlag.LeaveLevelAndTryGetError(10, out var errorOnLeaving);

                errorOnLeaving.Should().Be(1);
                tryResult.Should().BeTrue();
            }

            [Fact]
            public void Should_ReturnFalse_When_LevelNotEnabledAndNotDetected()
            {
                var errorFlag = new ErrorFlag();

                var tryResult = errorFlag.LeaveLevelAndTryGetError(10, out var errorOnLeaving);

                errorOnLeaving.Should().Be(-1);
                tryResult.Should().BeFalse();
            }

            [Fact]
            public void Should_ReturnFalse_When_LevelEnabledAndNotDetected()
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(10, 1);

                var tryResult = errorFlag.LeaveLevelAndTryGetError(10, out var errorOnLeaving);

                errorOnLeaving.Should().Be(-1);
                tryResult.Should().BeFalse();
            }

            [Fact]
            public void Should_ReturnFalse_When_LevelNotEnabledAndDetected()
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetDetected(10);

                var tryResult = errorFlag.LeaveLevelAndTryGetError(10, out var errorOnLeaving);

                errorOnLeaving.Should().Be(-1);
                tryResult.Should().BeFalse();
            }
        }

        public class IsDetectedAtAnylevel_After_SetDetected
        {
            [Fact]
            public void Should_BeFalseByDefault()
            {
                var errorFlag = new ErrorFlag();

                errorFlag.IsDetectedAtAnyLevel.Should().BeFalse();
            }

            [Theory]
            [InlineData(1)]
            [InlineData(10)]
            [InlineData(666)]
            public void Should_BeFalse_WhenEnabled(int level)
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(level, 1);

                errorFlag.IsDetectedAtAnyLevel.Should().BeFalse();
            }

            [Theory]
            [InlineData(1)]
            [InlineData(10)]
            [InlineData(666)]
            public void Should_BeTrue_IfDetected_OnSameLevel(int level)
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(level, 1);
                errorFlag.SetDetected(level);

                errorFlag.IsDetectedAtAnyLevel.Should().BeTrue();
            }

            [Theory]
            [InlineData(1)]
            [InlineData(10)]
            [InlineData(666)]
            public void Should_BeTrue_IfDetected_OnHigherLevel(int level)
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(level, 1);
                errorFlag.SetDetected(level + 1);

                errorFlag.IsDetectedAtAnyLevel.Should().BeTrue();
            }

            [Theory]
            [InlineData(1)]
            [InlineData(10)]
            [InlineData(666)]
            public void Should_BeFalse_IfDetected_OnLowerLevel(int level)
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(level, 1);
                errorFlag.SetDetected(level - 1);

                errorFlag.IsDetectedAtAnyLevel.Should().BeFalse();
            }

            [Fact]
            public void Should_BeFalse_AfterLeavingLevel()
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(10, 1);
                errorFlag.SetDetected(10);
                errorFlag.LeaveLevelAndTryGetError(10, out _);

                errorFlag.IsDetectedAtAnyLevel.Should().BeFalse();
            }

            [Fact]
            public void Should_BeFalse_AfterLeavingLevel_MultipleTimes()
            {
                var errorFlag = new ErrorFlag();

                errorFlag.SetEnabled(1, 1);
                errorFlag.SetEnabled(10, 1);
                errorFlag.SetEnabled(666, 1);
                errorFlag.SetDetected(1000);

                errorFlag.IsDetectedAtAnyLevel.Should().BeTrue();

                errorFlag.LeaveLevelAndTryGetError(1, out _);
                errorFlag.IsDetectedAtAnyLevel.Should().BeTrue();

                errorFlag.LeaveLevelAndTryGetError(10, out _);
                errorFlag.IsDetectedAtAnyLevel.Should().BeTrue();

                errorFlag.LeaveLevelAndTryGetError(666, out _);

                errorFlag.IsDetectedAtAnyLevel.Should().BeFalse();
            }
        }
    }
}
