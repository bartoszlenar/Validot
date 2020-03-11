namespace Validot.Tests.Unit.Specification
{
    using System;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class WithNameExtensionTests
    {
        [Fact]
        public void Should_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithNameOut<object>>(new[]
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
        public void Should_Add_WithNameCommand()
        {
            ApiTester.TestSingleCommand<object, IWithNameIn<object>, IWithNameOut<object>, WithNameCommand>(
                s => s.WithName("name"),
                command =>
                {
                    command.Name.Should().Be("name");
                });
        }

        [Fact]
        public void Should_ThrowException_When_NullName()
        {
            ApiTester.TextException<object, IWithNameIn<object>, IWithNameOut<object>>(
                s => s.WithName(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Theory]
        [MemberData(nameof(PathsTestData.ValidPaths), MemberType = typeof(PathsTestData))]
        public void Should_Accept_ValidPaths(string path)
        {
            ApiTester.TestSingleCommand<object, IWithNameIn<object>, IWithNameOut<object>, WithNameCommand>(
                s => s.WithName(path),
                command =>
                {
                    command.Name.Should().Be(path);
                });
        }

        [Theory]
        [MemberData(nameof(PathsTestData.InvalidPaths), MemberType = typeof(PathsTestData))]
        public void Should_ReturnFalse_For_InvalidPaths(string path)
        {
            ApiTester.TextException<object, IWithNameIn<object>, IWithNameOut<object>>(
                s => s.WithName(path),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentException>().WithMessage("Invalid name*");
                });
        }
    }
}
