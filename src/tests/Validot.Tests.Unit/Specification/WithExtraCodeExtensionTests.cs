namespace Validot.Tests.Unit.Specification
{
    using System;
    using FluentAssertions;
    using Validot.Specification;
    using Validot.Specification.Commands;
    using Xunit;

    public class WithExtraCodeExtensionTests
    {
        [Fact]
        public void Should_ForbiddenWithExtraCode_Add_WithExtraCodeCommand()
        {
            ApiTester.TestSingleCommand<object, IWithExtraCodeForbiddenIn<object>, IWithExtraCodeForbiddenOut<object>, WithExtraCodeCommand>(
                s => s.WithExtraCode("code"),
                command =>
                {
                    command.Code.Should().NotBeNull();
                    command.Code.Should().Be("code");
                });
        }

        [Fact]
        public void Should_ForbiddenWithExtraCode_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithExtraCodeForbiddenOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IWithExtraCodeForbiddenIn<object>)
            });
        }

        [Fact]
        public void Should_ForbiddenWithExtraCode_ThrowException_When_NullMessage()
        {
            ApiTester.TextException<object, IWithExtraCodeForbiddenIn<object>, IWithExtraCodeForbiddenOut<object>>(
                s => s.WithExtraCode(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Fact]
        public void Should_WithExtraCode_Add_WithExtraCodeCommand()
        {
            ApiTester.TestSingleCommand<object, IWithExtraCodeIn<object>, IWithExtraCodeOut<object>, WithExtraCodeCommand>(
                s => s.WithExtraCode("code"),
                command =>
                {
                    command.Code.Should().NotBeNull();
                    command.Code.Should().Be("code");
                });
        }

        [Fact]
        public void Should_WithExtraCode_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithExtraCodeOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IRuleIn<object>),
                typeof(IWithExtraCodeIn<object>),
                typeof(IAndIn<object>)
            });
        }

        [Fact]
        public void Should_WithExtraCode_ThrowException_When_NullMessage()
        {
            ApiTester.TextException<object, IWithExtraCodeIn<object>, IWithExtraCodeOut<object>>(
                s => s.WithExtraCode(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Theory]
        [MemberData(nameof(CodeHelperTests.Codes_Valid), MemberType = typeof(CodeHelperTests))]
        public void Should_Accept_ValidCodes(string code)
        {
            ApiTester.TestSingleCommand<object, IWithExtraCodeIn<object>, IWithExtraCodeOut<object>, WithExtraCodeCommand>(
                s => s.WithExtraCode(code),
                command =>
                {
                    command.Code.Should().Be(code);
                });
        }

        [Theory]
        [MemberData(nameof(CodeHelperTests.Codes_Invalid), MemberType = typeof(CodeHelperTests))]
        public void Should_ThrowException_When_InvalidCodes(string code)
        {
            ApiTester.TextException<object, IWithExtraCodeIn<object>, IWithExtraCodeOut<object>>(
                s => s.WithExtraCode(code),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentException>().WithMessage("Invalid code*");
                });
        }
    }
}
