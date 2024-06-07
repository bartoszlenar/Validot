namespace Validot.Tests.Unit.Specification
{
    using System;
    using FluentAssertions;
    using Validot.Specification;
    using Validot.Specification.Commands;
    using Xunit;

    public class WithCodeExtensionTests
    {
        [Fact]
        public void Should_ForbiddenWithCode_Add_WithCodeCommand()
        {
            ApiTester.TestSingleCommand<object, IWithCodeForbiddenIn<object>, IWithCodeForbiddenOut<object>, WithCodeCommand>(
                s => s.WithCode("code"),
                command =>
                {
                    command.Code.Should().NotBeNull();
                    command.Code.Should().Be("code");
                });
        }

        [Fact]
        public void Should_ForbiddenWithCode_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithCodeForbiddenOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IWithExtraCodeForbiddenIn<object>)
            });
        }

        [Fact]
        public void Should_ForbiddenWithCode_ThrowException_When_NullMessage()
        {
            ApiTester.TextException<object, IWithCodeForbiddenIn<object>, IWithCodeForbiddenOut<object>>(
                s => s.WithCode(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Fact]
        public void Should_WithCode_Add_WithCodeCommand()
        {
            ApiTester.TestSingleCommand<object, IWithCodeIn<object>, IWithCodeOut<object>, WithCodeCommand>(
                s => s.WithCode("code"),
                command =>
                {
                    command.Code.Should().NotBeNull();
                    command.Code.Should().Be("code");
                });
        }

        [Fact]
        public void Should_WithCode_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithCodeOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IRuleIn<object>),
                typeof(IWithExtraCodeIn<object>),
                typeof(IAndIn<object>)
            });
        }

        [Fact]
        public void Should_WithCode_ThrowException_When_NullMessage()
        {
            ApiTester.TextException<object, IWithCodeIn<object>, IWithCodeOut<object>>(
                s => s.WithCode(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Theory]
        [MemberData(nameof(CodeHelperTests.Codes_Valid), MemberType = typeof(CodeHelperTests))]
        public void Should_Accept_ValidCodes(string code)
        {
            ApiTester.TestSingleCommand<object, IWithCodeIn<object>, IWithCodeOut<object>, WithCodeCommand>(
                s => s.WithCode(code),
                command =>
                {
                    command.Code.Should().Be(code);
                });
        }

        [Theory]
        [MemberData(nameof(CodeHelperTests.Codes_Invalid), MemberType = typeof(CodeHelperTests))]
        public void Should_ThrowException_When_InvalidCodes(string code)
        {
            ApiTester.TextException<object, IWithCodeIn<object>, IWithCodeOut<object>>(
                s => s.WithCode(code),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentException>().WithMessage("Invalid code*");
                });
        }
    }
}
