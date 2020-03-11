namespace Validot.Tests.Unit.Validation.Scopes.Builders
{
    using System;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Errors;
    using Validot.Specification.Commands;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class CommandScopeBuilderTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new CommandScopeBuilder<TestClass>(new TestCommand(), (command, context) =>
            {
                return null;
            });
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_NullCommand()
        {
            Action action = () => new CommandScopeBuilder<TestClass>(
                null,
                (command, context) =>
                {
                    return null;
                });

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_NullCoreBuilder()
        {
            Action action = () => new CommandScopeBuilder<TestClass>(
                null,
                (command, context) =>
                {
                    return null;
                });

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        public class Build
        {
            [Fact]
            public void Should_PassContextAndCommandToCoreBuilder_And_ReturnResultOfCoreBuilder()
            {
                var command = new TestCommand();
                var context = Substitute.For<IScopeBuilderContext>();
                var commandScope = Substitute.For<ICommandScope<TestClass>>();

                var coreBuilderExecuted = 0;

                var builder = new CommandScopeBuilder<TestClass>(command, (cmd, ctx) =>
                {
                    cmd.Should().BeSameAs(command);
                    ctx.Should().BeSameAs(context);

                    coreBuilderExecuted++;

                    return commandScope;
                });

                var builtScope = builder.Build(context);

                coreBuilderExecuted.Should().Be(1);

                builtScope.Should().BeSameAs(commandScope);

                var cmdScope = (ICommandScope<TestClass>)builtScope;

                _ = cmdScope.DidNotReceive().Name;

                _ = cmdScope.DidNotReceive().ShouldExecute;
            }

            [Fact]
            public void Should_ThrowException_When_NullContext()
            {
                var command = new TestCommand();

                var builder = new CommandScopeBuilder<TestClass>(command, (cmd, ctx) =>
                {
                    return Substitute.For<ICommandScope<TestClass>>();
                });

                Action action = () => builder.Build(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_ReceivingNullFromCore()
            {
                var command = new TestCommand();

                var builder = new CommandScopeBuilder<TestClass>(command, (cmd, ctx) =>
                {
                    return null;
                });

                Action action = () => builder.Build(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class TryAdd
        {
            [Fact]
            public void Should_ThrowException_When_NullCommand()
            {
                var builder = new CommandScopeBuilder<TestClass>(new TestCommand(), (cmd, ctx) => Substitute.For<ICommandScope<TestClass>>());

                Action action = () => builder.TryAdd(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ReturnTrue_And_SetExecutionCondition_When_WhenCommand()
            {
                Predicate<TestClass> predicate = o => true;

                var context = Substitute.For<IScopeBuilderContext>();
                var command = new WhenCommand<TestClass>(predicate);

                var builder = new CommandScopeBuilder<TestClass>(new TestCommand(), (cmd, ctx) => Substitute.For<ICommandScope<TestClass>>());

                var tryAddResult = builder.TryAdd(command);

                tryAddResult.Should().BeTrue();

                var builtScope = (ICommandScope<TestClass>)builder.Build(context);

                builtScope.ShouldExecute.Should().BeSameAs(predicate);
            }

            [Fact]
            public void Should_ReturnTrue_And_SetName_When_WithNameCommand()
            {
                var context = Substitute.For<IScopeBuilderContext>();
                var command = new WithNameCommand("some_name");

                var builder = new CommandScopeBuilder<TestClass>(new TestCommand(), (cmd, ctx) => Substitute.For<ICommandScope<TestClass>>());

                var tryAddResult = builder.TryAdd(command);

                tryAddResult.Should().BeTrue();

                var builtScope = (ICommandScope<TestClass>)builder.Build(context);

                builtScope.Name.Should().Be("some_name");
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Keep_InvalidCommands_NotAffecting_ValidCommands), MemberType = typeof(ErrorBuilderTestData))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_When_SingleWithMessageCommand), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_When_SingleExtraMessageCommand), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_FromLatestCommand_When_WithMessageCommandIsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.ManyMessages_When_WithMessageCommand_IsFollowedBy_WithExtraMessageCommands), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.WithManyMessages_When_ClearErrorCommand_IsFollowedBy_MessageCommands), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_When_SingleWithCodeCommand), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_When_SingleExtraCodeCommand), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_FromLatestCommand_When_WithCodeCommandIsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.ManyCodes_When_WithCodeCommand_IsFollowedBy_WithExtraCodeCommands), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.WithManyCodes_When_ClearErrorCommand_IsFollowedBy_CodeCommands), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_ConstructErrorCodeAndMessages_And_RegisterItInContext_And_SetItsId(object cmds, IError error)
            {
                var registrationsCount = 0;

                var context = Substitute.For<IScopeBuilderContext>();

                context.RegisterError(Arg.Any<IError>()).Returns(info =>
                {
                    var registeredError = info.Arg<IError>();

                    registeredError.ShouldBeEqualTo(error);
                    registrationsCount++;

                    return 666;
                });

                var builder = new CommandScopeBuilder<TestClass>(new TestCommand(), (cmd, ctx) => Substitute.For<ICommandScope<TestClass>>());

                var commands = (ICommand[])cmds;

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                var builtScope = (ICommandScope<TestClass>)builder.Build(context);

                builtScope.ErrorId.Should().Be(666);
                registrationsCount.Should().Be(1);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Messages.NoMessages_When_ClearErrorCommand_IsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.NoCodes_When_ClearErrorCommand_IsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_ConstructErrorCodeAndMessages_And_RegisterItInContext_And_SetDefaultErrorId_When_ErrorCleared(object cmds, IError error)
            {
                _ = error;

                var context = Substitute.For<IScopeBuilderContext>();
                context.DefaultErrorId.Returns(321);
                context.RegisterError(Arg.Any<IError>()).Returns(info => 666);

                var builder = new CommandScopeBuilder<TestClass>(new TestCommand(), (cmd, ctx) => Substitute.For<ICommandScope<TestClass>>());

                var commands = (ICommand[])cmds;

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                var builtScope = (ICommandScope<TestClass>)builder.Build(context);

                builtScope.ErrorId.Should().Be(321);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_When_SingleExtraMessageCommand), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_When_SingleExtraCodeCommand), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_SetAppendMode(object cmds, IError error)
            {
                _ = error;

                var context = Substitute.For<IScopeBuilderContext>();
                context.RegisterError(Arg.Any<IError>()).Returns(info => 666);

                var builder = new CommandScopeBuilder<TestClass>(new TestCommand(), (cmd, ctx) => Substitute.For<ICommandScope<TestClass>>());

                var commands = (ICommand[])cmds;

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                var builtScope = (ICommandScope<TestClass>)builder.Build(context);

                builtScope.ErrorMode.Should().Be(ErrorMode.Append);
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_When_SingleWithMessageCommand), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_FromLatestCommand_When_WithMessageCommandIsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.ManyMessages_When_WithMessageCommand_IsFollowedBy_WithExtraMessageCommands), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.NoMessages_When_ClearErrorCommand_IsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.WithManyMessages_When_ClearErrorCommand_IsFollowedBy_MessageCommands), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_When_SingleWithCodeCommand), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_FromLatestCommand_When_WithCodeCommandIsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.ManyCodes_When_WithCodeCommand_IsFollowedBy_WithExtraCodeCommands), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.NoCodes_When_ClearErrorCommand_IsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.WithManyCodes_When_ClearErrorCommand_IsFollowedBy_CodeCommands), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_SetOverrideMode(object cmds, IError error)
            {
                _ = error;

                var context = Substitute.For<IScopeBuilderContext>();
                context.RegisterError(Arg.Any<IError>()).Returns(info => 666);

                var builder = new CommandScopeBuilder<TestClass>(new TestCommand(), (cmd, ctx) => Substitute.For<ICommandScope<TestClass>>());

                var commands = (ICommand[])cmds;

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                var builtScope = (ICommandScope<TestClass>)builder.Build(context);

                builtScope.ErrorMode.Should().Be(ErrorMode.Override);
            }
        }

        public class TestCommand : ICommand
        {
        }

        public class TestClass
        {
        }
    }
}
