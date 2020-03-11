namespace Validot.Tests.Unit.Results.ToMessagesGroups
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Results;

    using Xunit;

    public class ToMessagesGroupsExtensionTests
    {
        [Fact]
        public void Should_ThrowException_If_NullResult()
        {
            Action action = () => (null as IValidationResult).ToMessagesGroups();

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_Return_EmptyArray_When_Valid()
        {
            var validationResult = Substitute.For<IValidationResult>();

            validationResult.IsValid.Returns(true);

            var messagesGroups = validationResult.ToMessagesGroups();

            validationResult.Details.Received(1).GetErrorMessages(Arg.Is(null as string));
            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorMessages();

            messagesGroups.Should().NotBeNull();
            messagesGroups.Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_ResultOf_GetErrorMessages_When_Invalid()
        {
            var validationResult = Substitute.For<IValidationResult>();

            var errorMessages = new Dictionary<string, IReadOnlyList<string>>
            {
                [""] = new[] { "a" },
                ["path"] = new[] { "b, c" }
            };

            validationResult.Details.GetErrorMessages(Arg.Is(null as string)).Returns(errorMessages);
            validationResult.IsValid.Returns(false);

            var messagesGroups = validationResult.ToMessagesGroups();

            validationResult.Details.Received(1).GetErrorMessages(Arg.Is(null as string));
            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorMessages();

            messagesGroups.Should().NotBeNull();
            messagesGroups.Should().BeSameAs(errorMessages);
        }

        [Fact]
        public void Should_Return_ResultOf_GetErrorMessages_WithTranslation_When_Invalid()
        {
            var validationResult = Substitute.For<IValidationResult>();

            var errorMessages1 = new Dictionary<string, IReadOnlyList<string>>
            {
                [""] = new[] { "a" },
                ["path"] = new[] { "b", "c" }
            };

            var errorMessages2 = new Dictionary<string, IReadOnlyList<string>>
            {
                [""] = new[] { "A" },
                ["path"] = new[] { "B", "C" }
            };

            validationResult.Details.GetErrorMessages(Arg.Is("translation1")).Returns(errorMessages1);
            validationResult.Details.GetErrorMessages(Arg.Is("translation2")).Returns(errorMessages2);
            validationResult.IsValid.Returns(false);

            var messagesGroups = validationResult.ToMessagesGroups("translation2");

            validationResult.Details.Received(1).GetErrorMessages(Arg.Is("translation2"));
            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorMessages();

            messagesGroups.Should().NotBeNull();
            messagesGroups.Should().BeSameAs(errorMessages2);
        }
    }
}
