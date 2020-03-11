namespace Validot.Tests.Unit.Results.ToCodesList
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Results;

    using Xunit;

    public class ToCodesListExtensionTests
    {
        [Fact]
        public void Should_ThrowException_If_NullResult()
        {
            Action action = () => (null as IValidationResult).ToCodesList();

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_Return_EmptyArray_When_Valid()
        {
            var validationResult = Substitute.For<IValidationResult>();

            validationResult.IsValid.Returns(true);

            var errorCodes = validationResult.ToCodesList();

            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorCodes();

            errorCodes.Should().NotBeNull();
            errorCodes.Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_ResultOf_GetErrorCodes_When_Invalid()
        {
            var validationResult = Substitute.For<IValidationResult>();

            var detailsErrorCodes = new List<string>()
            {
                "test",
                "test2"
            };

            validationResult.Details.GetErrorCodes().Returns(detailsErrorCodes);
            validationResult.IsValid.Returns(false);

            var errorCodes = validationResult.ToCodesList();

            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorCodes();

            errorCodes.Should().NotBeNull();
            errorCodes.Should().BeSameAs(detailsErrorCodes);
        }
    }
}
