namespace Validot.Tests.Unit.Validation.Scopes.Builders
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using Validot.Errors;
    using Validot.Errors.Args;
    using Validot.Specification.Commands;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class ErrorBuilderTests
    {
        [Fact]
        public void Should_Constructor_ThrowException_When_NullKey()
        {
            Action action = () => new ErrorBuilder(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_Be_Empty_If_NothingAdded()
        {
            var builder = new ErrorBuilder();

            builder.IsEmpty.Should().BeTrue();
            builder.Mode.Should().Be(ErrorMode.Append);

            var error = builder.Build();

            error.Args.Should().BeEmpty();
            error.Messages.Should().BeEmpty();
            error.Codes.Should().BeEmpty();
        }

        [Fact]
        public void Should_Be_NotEmpty_If_SomethingAdded()
        {
            var builder = new ErrorBuilder();
            builder.TryAdd(new WithExtraCodeCommand("code"));

            builder.IsEmpty.Should().BeFalse();

            var error = builder.Build();

            error.Args.Should().BeEmpty();
            error.Messages.Should().BeEmpty();
            error.Codes.Should().NotBeEmpty();
        }

        [Fact]
        public void Should_Build_NewObjects()
        {
            var builder = new ErrorBuilder();

            var error1 = builder.Build();
            var error2 = builder.Build();
            var error3 = builder.Build();

            error1.Should().NotBeSameAs(error2);
            error1.Should().NotBeSameAs(error3);
            error2.Should().NotBeSameAs(error3);
        }

        [Fact]
        public void Should_Build_WithMessage_When_AddedOnInit()
        {
            var builder = new ErrorBuilder("key");

            builder.IsEmpty.Should().BeFalse();

            var error = builder.Build();

            error.Messages.Should().ContainSingle("key");
            error.Codes.Should().BeEmpty();
        }

        [Fact]
        public void Should_Build_WithArgs_When_AddedOnInit()
        {
            var args = new IArg[]
            {
                Arg.Text("name1", "value1"),
                Arg.Number("name2", 2)
            };

            var builder = new ErrorBuilder("key", args);

            builder.IsEmpty.Should().BeFalse();

            var error = builder.Build();

            error.Messages.Should().ContainSingle("key");
            error.Codes.Should().BeEmpty();

            error.Args.Should().BeSameAs(args);

            error.Args.ElementAt(0).Should().BeOfType<TextArg>();
            ((TextArg)error.Args.ElementAt(0)).Name.Should().Be("name1");
            ((TextArg)error.Args.ElementAt(0)).Value.Should().Be("value1");

            error.Args.ElementAt(1).Should().BeOfType<NumberArg<int>>();
            ((NumberArg<int>)error.Args.ElementAt(1)).Name.Should().Be("name2");
            ((NumberArg<int>)error.Args.ElementAt(1)).Value.Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(ErrorBuilderTestData.Modes.Should_BeIn_AppendMode_If_OnlyExtraCommands), MemberType = typeof(ErrorBuilderTestData.Modes))]
        public void Should_Build_WithArgs_When_AddedOnInit_And_CommandsAppended(object cmd, IError expectedError)
        {
            var args = new IArg[]
            {
                Arg.Text("name1", "value1"),
                Arg.Number("name2", 2)
            };

            var commands = (ICommand[])cmd;

            var builder = new ErrorBuilder("key", args);

            builder.IsEmpty.Should().BeFalse();

            foreach (var command in commands)
            {
                builder.TryAdd(command);
            }

            var error = builder.Build();

            var expectedErrorSettable = (Error)expectedError;
            expectedErrorSettable.Args = args;

            expectedErrorSettable.Messages = new[]
            {
                "key"
            }.Concat(expectedErrorSettable.Messages.ToArray()).ToArray();

            error.ShouldBeEqualTo(expectedErrorSettable);
        }

        [Theory]
        [MemberData(nameof(ErrorBuilderTestData.Modes.Should_BeIn_OverrideMode_If_AnyNonExtraCommand_Or_WithErrorClearedCommand), MemberType = typeof(ErrorBuilderTestData.Modes))]
        public void Should_Build_WithArgs_When_AddedOnInit_And_CommandsOverride(object cmd, IError expectedError)
        {
            var args = new IArg[]
            {
                Arg.Text("name1", "value1"),
                Arg.Number("name2", 2)
            };

            var commands = (ICommand[])cmd;

            var builder = new ErrorBuilder("key", args);

            builder.IsEmpty.Should().BeFalse();

            foreach (var command in commands)
            {
                builder.TryAdd(command);
            }

            var error = builder.Build();

            var expectedErrorSettable = (Error)expectedError;
            expectedErrorSettable.Args = args;

            error.ShouldBeEqualTo(expectedErrorSettable);
        }

        [Fact]
        public void Should_TryAdd_ThrowException_When_NullCommand()
        {
            var builder = new ErrorBuilder();

            Action action = () => builder.TryAdd(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(ErrorBuilderTestData.CommandsEnabled), MemberType = typeof(ErrorBuilderTestData))]
        public void Should_TryAdd_ReturnTrue_If_CommandAllowed(object cmd, bool expectedResult)
        {
            var command = cmd as ICommand;

            var builder = new ErrorBuilder();

            var result = builder.TryAdd(command);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [MemberData(nameof(ErrorBuilderTestData.Keep_InvalidCommands_NotAffecting_ValidCommands), MemberType = typeof(ErrorBuilderTestData))]
        public void Should_TryAdd_Keep_EnabledCommands_And_DisabledCommands_Independent(object cmd, IError error)
        {
            var commands = (ICommand[])cmd;
            var builder = new ErrorBuilder();

            foreach (var command in commands)
            {
                builder.TryAdd(command);
            }

            builder.Build().ShouldBeEqualTo(error);
        }

        public class TryAddMessages
        {
            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_When_SingleWithMessageCommand), MemberType = typeof(ErrorBuilderTestData.Messages))]
            public void Should_TryAdd_Build_SingleMessage_When_SingleWithMessageCommand(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_When_SingleExtraMessageCommand), MemberType = typeof(ErrorBuilderTestData.Messages))]
            public void Should_TryAdd_Build_SingleMessage_When_SingleExtraMessageCommand(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_FromLatestCommand_When_WithMessageCommandIsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Messages))]
            public void Should_TryAdd_Build_SingleMessage_FromLatestCommand_When_WithMessageCommandIsTheLastOne(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Messages.ManyMessages_When_WithMessageCommand_IsFollowedBy_WithExtraMessageCommands), MemberType = typeof(ErrorBuilderTestData.Messages))]
            public void Should_TryAdd_Build_ManyMessages_When_WithMessageCommand_IsFollowedBy_WithExtraMessageCommands(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Messages.NoMessages_When_ClearErrorCommand_IsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Messages))]
            public void Should_TryAdd_Build_NoMessages_When_ClearErrorCommand_IsTheLastOne(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Messages.WithManyMessages_When_ClearErrorCommand_IsFollowedBy_MessageCommands), MemberType = typeof(ErrorBuilderTestData.Messages))]
            public void Should_TryAdd_Build_WithManyMessages_When_ClearErrorCommand_IsFollowedBy_MessageCommands(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }
        }

        public class TryAddCodes
        {
            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_When_SingleWithCodeCommand), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_TryAdd_Build_SingleCode_When_SingleWithCodeCommand(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_When_SingleExtraCodeCommand), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_TryAdd_Build_SingleCode_When_SingleExtraCodeCommand(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_FromLatestCommand_When_WithCodeCommandIsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_TryAdd_Build_SingleCode_FromLatestCommand_When_WithCodeCommandIsTheLastOne(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Codes.ManyCodes_When_WithCodeCommand_IsFollowedBy_WithExtraCodeCommands), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_TryAdd_Build_ManyCodes_When_WithCodeCommand_IsFollowedBy_WithExtraCodeCommands(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Codes.NoCodes_When_ClearErrorCommand_IsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_TryAdd_Build_NoCodes_When_ClearErrorCommand_IsTheLastOne(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Codes.WithManyCodes_When_ClearErrorCommand_IsFollowedBy_CodeCommands), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_TryAdd_Build_WithManyCodes_When_ClearErrorCommand_IsFollowedBy_CodeCommands(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);
            }
        }

        public class Modes
        {
            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Modes.Should_BeIn_AppendMode_If_OnlyExtraCommands), MemberType = typeof(ErrorBuilderTestData.Modes))]
            public void Should_BeIn_AppendMode_If_OnlyExtraCommands(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);

                builder.Mode.Should().Be(ErrorMode.Append);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Modes.Should_BeIn_OverrideMode_If_AnyNonExtraCommand_Or_WithErrorClearedCommand), MemberType = typeof(ErrorBuilderTestData.Modes))]
            public void Should_BeIn_OverrideMode_If_AnyNonExtraCommand_Or_WithErrorClearedCommand(object cmd, IError error)
            {
                var commands = (ICommand[])cmd;
                var builder = new ErrorBuilder();

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                builder.Build().ShouldBeEqualTo(error);

                builder.Mode.Should().Be(ErrorMode.Override);
            }
        }
    }
}
