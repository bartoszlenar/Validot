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

    public class RuleCommandScopeBuilderTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new CommandScopeBuilder<object>(new TestClass(), (command, context) =>
            {
                return null;
            });
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_NullCommand()
        {
            Action action = () => new RuleCommandScopeBuilder<object>(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        public class Build
        {
            [Fact]
            public void Should_ReturnRuleCommandScope()
            {
                var command = new RuleCommand<TestClass>(x => true);
                var context = Substitute.For<IScopeBuilderContext>();

                var builder = new RuleCommandScopeBuilder<TestClass>(command);

                var builtScope = builder.Build(context);

                builtScope.Should().BeOfType<RuleCommandScope<TestClass>>();
            }

            [Fact]
            public void Should_ReturnRuleCommandScope_WithIsValidPredicate()
            {
                Predicate<TestClass> predicate = x => true;

                var command = new RuleCommand<TestClass>(predicate);
                var context = Substitute.For<IScopeBuilderContext>();

                var builder = new RuleCommandScopeBuilder<TestClass>(command);

                var builtScope = builder.Build(context);

                builtScope.Should().BeOfType<RuleCommandScope<TestClass>>();

                var ruleCommandScope = (RuleCommandScope<TestClass>)builtScope;

                ruleCommandScope.IsValid.Should().Be(predicate);
            }

            [Fact]
            public void Should_ThrowException_When_NullContext()
            {
                Action action = () => _ = new RuleCommandScopeBuilder<TestClass>(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class TryAdd
        {
            [Fact]
            public void Should_ThrowException_When_NullCommand()
            {
                var command = new RuleCommand<TestClass>(x => true);

                var builder = new RuleCommandScopeBuilder<TestClass>(command);

                Action action = () => builder.TryAdd(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ReturnTrue_And_SetCondition_When_WithConditionCommand()
            {
                Predicate<object> condition = o => true;

                var context = Substitute.For<IScopeBuilderContext>();
                var command = new WithConditionCommand<TestClass>(condition);

                var builder = new RuleCommandScopeBuilder<TestClass>(new RuleCommand<TestClass>(x => true));

                var tryAddResult = builder.TryAdd(command);

                tryAddResult.Should().BeTrue();

                var builtScope = (ICommandScope<TestClass>)builder.Build(context);

                builtScope.ExecutionCondition.Should().BeSameAs(condition);
            }

            [Fact]
            public void Should_ReturnTrue_And_SetName_When_WithPathCommand()
            {
                var context = Substitute.For<IScopeBuilderContext>();
                var command = new WithPathCommand("some_path");

                var builder = new RuleCommandScopeBuilder<TestClass>(new RuleCommand<TestClass>(x => true));

                var tryAddResult = builder.TryAdd(command);

                tryAddResult.Should().BeTrue();

                var builtScope = (ICommandScope<TestClass>)builder.Build(context);

                builtScope.Path.Should().Be("some_path");
            }

            [Theory]
            [MemberData(nameof(ErrorBuilderTestData.Keep_InvalidCommands_NotAffecting_ValidCommands), MemberType = typeof(ErrorBuilderTestData))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_When_SingleWithMessageCommand), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_When_SingleExtraMessageCommand), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_FromLatestCommand_When_WithMessageCommandIsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Messages.ManyMessages_When_WithMessageCommand_IsFollowedBy_WithExtraMessageCommands), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_When_SingleWithCodeCommand), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_When_SingleExtraCodeCommand), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_FromLatestCommand_When_WithCodeCommandIsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.ManyCodes_When_WithCodeCommand_IsFollowedBy_WithExtraCodeCommands), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.MessagesAndCodes.SingleMessage_With_SingleCode), MemberType = typeof(ErrorBuilderTestData.MessagesAndCodes))]
            [MemberData(nameof(ErrorBuilderTestData.MessagesAndCodes.ManyMessages_With_ManyCodes), MemberType = typeof(ErrorBuilderTestData.MessagesAndCodes))]
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

                var builder = new RuleCommandScopeBuilder<TestClass>(new RuleCommand<TestClass>(x => true));

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
            [MemberData(nameof(ErrorBuilderTestData.Messages.SingleMessage_When_SingleExtraMessageCommand), MemberType = typeof(ErrorBuilderTestData.Messages))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_When_SingleExtraCodeCommand), MemberType = typeof(ErrorBuilderTestData.Codes))]
            public void Should_SetAppendMode(object cmds, IError error)
            {
                _ = error;

                var context = Substitute.For<IScopeBuilderContext>();
                context.RegisterError(Arg.Any<IError>()).Returns(info => 666);

                var builder = new RuleCommandScopeBuilder<TestClass>(new RuleCommand<TestClass>(x => true));

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
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_When_SingleWithCodeCommand), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.SingleCode_FromLatestCommand_When_WithCodeCommandIsTheLastOne), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.Codes.ManyCodes_When_WithCodeCommand_IsFollowedBy_WithExtraCodeCommands), MemberType = typeof(ErrorBuilderTestData.Codes))]
            [MemberData(nameof(ErrorBuilderTestData.MessagesAndCodes.SingleMessage_With_SingleCode), MemberType = typeof(ErrorBuilderTestData.MessagesAndCodes))]
            [MemberData(nameof(ErrorBuilderTestData.MessagesAndCodes.ManyMessages_With_ManyCodes), MemberType = typeof(ErrorBuilderTestData.MessagesAndCodes))]
            public void Should_SetOverrideMode(object cmds, IError error)
            {
                _ = error;

                var context = Substitute.For<IScopeBuilderContext>();
                context.RegisterError(Arg.Any<IError>()).Returns(info => 666);

                var builder = new RuleCommandScopeBuilder<TestClass>(new RuleCommand<TestClass>(x => true));

                var commands = (ICommand[])cmds;

                foreach (var command in commands)
                {
                    builder.TryAdd(command);
                }

                var builtScope = (ICommandScope<TestClass>)builder.Build(context);

                builtScope.ErrorMode.Should().Be(ErrorMode.Override);
            }
        }

        private class TestClass : ICommand
        {
        }
    }
}
