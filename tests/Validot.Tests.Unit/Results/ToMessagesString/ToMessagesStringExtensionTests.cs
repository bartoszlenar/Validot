namespace Validot.Tests.Unit.Results.ToMessagesString
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Results;

    using Xunit;

    public class ToMessagesStringExtensionTests
    {
        [Fact]
        public void Should_ThrowException_If_NullResult()
        {
            Action action = () => (null as IValidationResult).ToMessagesString();

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_Return_EmptyArray_When_Valid()
        {
            var validationResult = Substitute.For<IValidationResult>();

            validationResult.IsValid.Returns(true);

            var messagesList = validationResult.ToMessagesString();

            validationResult.Details.DidNotReceiveWithAnyArgs().GetErrorMessages();

            messagesList.Should().NotBeNull();
            messagesList.Should().BeEmpty();
        }

        [Fact]
        public void Should_GetErrorMessage_And_CreateString_WithPathsByDefault()
        {
            var validationResult = Substitute.For<IValidationResult>();

            validationResult.IsValid.Returns(false);

            validationResult.Details.GetErrorMessages(Arg.Is(null as string)).Returns(new Dictionary<string, IReadOnlyList<string>>()
            {
                [""] = new[] { "p" },
                ["p1"] = new[] { "p 11", "p 12", "duplicate" },
                ["p2"] = new[] { "p 21", "p 22", "duplicate" }
            });

            var messagesString = validationResult.ToMessagesString();

            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorMessages();
            validationResult.Details.Received(1).GetErrorMessages(Arg.Is(null as string));

            messagesString.Should().NotBeNull();
            messagesString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Should().HaveCount(7);

            messagesString.Should().Contain("p" + Environment.NewLine);
            messagesString.Should().Contain("p1: p 11" + Environment.NewLine + "p1: p 12" + Environment.NewLine + "p1: duplicate" + Environment.NewLine);
            messagesString.Should().Contain("p2: p 21" + Environment.NewLine + "p2: p 22" + Environment.NewLine + "p2: duplicate" + Environment.NewLine);
        }

        [Fact]
        public void Should_GetErrorMessage_And_CreateString_WithoutPaths_When_IncludePathsIsFalse()
        {
            var validationResult = Substitute.For<IValidationResult>();

            validationResult.IsValid.Returns(false);

            validationResult.Details.GetErrorMessages(Arg.Is(null as string)).Returns(new Dictionary<string, IReadOnlyList<string>>()
            {
                [""] = new[] { "p" },
                ["p1"] = new[] { "p 11", "p 12", "duplicate" },
                ["p2"] = new[] { "p 21", "p 22", "duplicate" }
            });

            var messagesString = validationResult.ToMessagesString(false);

            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorMessages();
            validationResult.Details.Received(1).GetErrorMessages(Arg.Is(null as string));

            messagesString.Should().NotBeNull();
            messagesString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Should().HaveCount(7);

            messagesString.Should().Contain("p" + Environment.NewLine);
            messagesString.Should().Contain("p 11" + Environment.NewLine + "p 12" + Environment.NewLine + "duplicate" + Environment.NewLine);
            messagesString.Should().Contain("p 21" + Environment.NewLine + "p 22" + Environment.NewLine + "duplicate" + Environment.NewLine);
        }

        [Fact]
        public void Should_GetErrorMessage_And_CreateString_WithTranslation()
        {
            var validationResult = Substitute.For<IValidationResult>();

            validationResult.IsValid.Returns(false);

            validationResult.Details.GetErrorMessages(Arg.Is("translation1")).Returns(new Dictionary<string, IReadOnlyList<string>>()
            {
                [""] = new[] { "X" },
                ["p1"] = new[] { "X 11", "X 12", "X duplicate" },
                ["p2"] = new[] { "X 21", "X 22", "X duplicate" }
            });

            validationResult.Details.GetErrorMessages(Arg.Is("translation2")).Returns(new Dictionary<string, IReadOnlyList<string>>()
            {
                [""] = new[] { "p" },
                ["p1"] = new[] { "p 11", "p 12", "duplicate" },
                ["p2"] = new[] { "p 21", "p 22", "duplicate" }
            });

            var messagesString = validationResult.ToMessagesString(translation: "translation2");

            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorMessages();
            validationResult.Details.Received(1).GetErrorMessages(Arg.Is("translation2"));

            messagesString.Should().NotBeNull();
            messagesString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Should().HaveCount(7);

            messagesString.Should().Contain("p" + Environment.NewLine);
            messagesString.Should().Contain("p1: p 11" + Environment.NewLine + "p1: p 12" + Environment.NewLine + "p1: duplicate" + Environment.NewLine);
            messagesString.Should().Contain("p2: p 21" + Environment.NewLine + "p2: p 22" + Environment.NewLine + "p2: duplicate" + Environment.NewLine);
        }
    }
}
