namespace Validot.Tests.Unit.Specification
{
    using System;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class WithPathExtensionTests
    {
        [Fact]
        public void Should_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithPathOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IRuleIn<object>),
                typeof(IWithErrorClearedIn<object>),
                typeof(IWithMessageIn<object>),
                typeof(IWithExtraMessageIn<object>),
                typeof(IWithCodeIn<object>),
                typeof(IWithExtraCodeIn<object>),
            });
        }

        [Fact]
        public void Should_Add_WithPathCommand()
        {
            ApiTester.TestSingleCommand<object, IWithPathIn<object>, IWithPathOut<object>, WithPathCommand>(
                s => s.WithPath("path"),
                command =>
                {
                    command.Path.Should().Be("path");
                });
        }

        [Fact]
        public void Should_ThrowException_When_NullName()
        {
            ApiTester.TextException<object, IWithPathIn<object>, IWithPathOut<object>>(
                s => s.WithPath(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Theory]
        [MemberData(nameof(PathTestData.ValidPaths), MemberType = typeof(PathTestData))]
        public void Should_Accept_ValidPaths(string path)
        {
            ApiTester.TestSingleCommand<object, IWithPathIn<object>, IWithPathOut<object>, WithPathCommand>(
                s => s.WithPath(path),
                command =>
                {
                    command.Path.Should().Be(path);
                });
        }

        [Theory]
        [MemberData(nameof(PathTestData.InvalidPaths), MemberType = typeof(PathTestData))]
        public void Should_ReturnFalse_For_InvalidPaths(string path)
        {
            ApiTester.TextException<object, IWithPathIn<object>, IWithPathOut<object>>(
                s => s.WithPath(path),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentException>().WithMessage("Invalid path*");
                });
        }
    }
}
