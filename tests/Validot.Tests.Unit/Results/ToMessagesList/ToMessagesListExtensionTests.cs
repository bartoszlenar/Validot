namespace Validot.Tests.Unit.Results.ToMessagesList
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Results;

    using Xunit;

    public class ToMessagesListExtensionTests
    {
        [Fact]
        public void Should_ThrowException_If_NullResult()
        {
            Action action = () => (null as IValidationResult).ToMessagesList();

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_Return_EmptyArray_When_AnyErrors_Is_False()
        {
            var validationResult = Substitute.For<IValidationResult>();

            validationResult.AnyErrors.Returns(false);

            var messagesList = validationResult.ToMessagesList();

            validationResult.Details.DidNotReceiveWithAnyArgs().GetErrorMessages();

            messagesList.Should().NotBeNull();
            messagesList.Should().BeEmpty();
        }

        [Fact]
        public void Should_GetErrorMessage_And_CreateList_WithPathsByDefault()
        {
            var validationResult = Substitute.For<IValidationResult>();

            validationResult.AnyErrors.Returns(true);

            validationResult.Details.GetErrorMessages(Arg.Is(null as string)).Returns(new Dictionary<string, IReadOnlyList<string>>()
            {
                [""] = new[] { "p" },
                ["p1"] = new[] { "p 11", "p 12", "duplicate" },
                ["p2"] = new[] { "p 21", "p 22", "duplicate" }
            });

            var messagesList = validationResult.ToMessagesList();

            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorMessages();
            validationResult.Details.Received(1).GetErrorMessages(Arg.Is(null as string));

            messagesList.Should().NotBeNull();
            messagesList.Should().HaveCount(7);

            messagesList.Should().Contain("p");
            messagesList.Should().Contain("p1: p 11");
            messagesList.Should().Contain("p1: p 12");
            messagesList.Should().Contain("p1: duplicate");
            messagesList.Should().Contain("p2: p 21");
            messagesList.Should().Contain("p2: p 22");
            messagesList.Should().Contain("p2: duplicate");
        }

        [Fact]
        public void Should_GetErrorMessage_And_CreateList_WithoutPaths_When_IncludePathsIsFalse()
        {
            var validationResult = Substitute.For<IValidationResult>();

            validationResult.AnyErrors.Returns(true);

            validationResult.Details.GetErrorMessages(Arg.Is(null as string)).Returns(new Dictionary<string, IReadOnlyList<string>>()
            {
                [""] = new[] { "p" },
                ["p1"] = new[] { "p 11", "p 12", "duplicate" },
                ["p2"] = new[] { "p 21", "p 22", "duplicate" }
            });

            var messagesList = validationResult.ToMessagesList(false);

            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorMessages();
            validationResult.Details.Received(1).GetErrorMessages(Arg.Is(null as string));

            messagesList.Should().NotBeNull();
            messagesList.Should().HaveCount(7);

            messagesList.Should().Contain("p");
            messagesList.Should().Contain("p 11");
            messagesList.Should().Contain("p 12");
            messagesList.Should().Contain("duplicate");
            messagesList.Should().Contain("p 21");
            messagesList.Should().Contain("p 22");
            messagesList.Should().Contain("duplicate");
        }

        [Fact]
        public void Should_GetErrorMessage_And_CreateList_WithTranslation()
        {
            var validationResult = Substitute.For<IValidationResult>();

            validationResult.AnyErrors.Returns(true);

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

            var messagesList = validationResult.ToMessagesList(translation: "translation2");

            validationResult.Details.ReceivedWithAnyArgs(1).GetErrorMessages();
            validationResult.Details.Received(1).GetErrorMessages(Arg.Is("translation2"));

            messagesList.Should().NotBeNull();
            messagesList.Should().HaveCount(7);

            messagesList.Should().Contain("p");
            messagesList.Should().Contain("p1: p 11");
            messagesList.Should().Contain("p1: p 12");
            messagesList.Should().Contain("p1: duplicate");
            messagesList.Should().Contain("p2: p 21");
            messagesList.Should().Contain("p2: p 22");
            messagesList.Should().Contain("p2: duplicate");
        }
    }
}
