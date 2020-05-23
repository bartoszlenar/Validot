namespace Validot.Tests.Unit.Specification
{
    using System;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class RuleExtensionTests
    {
        [Fact]
        public void Should_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IRuleOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IRuleIn<object>),
                typeof(IWithConditionIn<object>),
                typeof(IWithPathIn<object>),
                typeof(IWithErrorClearedIn<object>),
                typeof(IWithMessageIn<object>),
                typeof(IWithExtraMessageIn<object>),
                typeof(IWithCodeIn<object>),
                typeof(IWithExtraCodeIn<object>),
            });
        }

        [Fact]
        public void Should_Rule_Add_Command()
        {
            Predicate<object> predicate = x => true;

            ApiTester.TestSingleCommand<object, IRuleIn<object>, IRuleIn<object>, RuleCommand<object>>(
                s => s.Rule(predicate),
                command =>
                {
                    command.Args.Should().BeNull();
                    command.Message.Should().BeNull();
                    command.ValidCondition.Should().BeSameAs(predicate);
                });
        }

        [Fact]
        public void Should_Rule_ThrowException_When_NullPredicate()
        {
            ApiTester.TextException<object, IRuleIn<object>, IRuleIn<object>>(
                s => s.Rule(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Fact]
        public void Should_RuleTemplate_Add_Command()
        {
            Predicate<object> predicate = x => true;

            var args = new[]
            {
                Arg.Text("1", "1"),
                Arg.Number("2", 2),
                Arg.Type("3", typeof(Guid))
            };

            ApiTester.TestSingleCommand<object, IRuleIn<object>, IRuleIn<object>, RuleCommand<object>>(
                s => s.RuleTemplate(predicate, "messageKey", args),
                command =>
                {
                    command.Args.Should().BeSameAs(args);
                    command.Args.Count.Should().Be(3);
                    command.Args.Should().Contain(args);

                    command.Message.Should().Be("messageKey");

                    command.ValidCondition.Should().BeSameAs(predicate);
                });
        }

        [Fact]
        public void Should_RuleTemplate_Add_Command_WithoutArgs()
        {
            Predicate<object> predicate = x => true;

            ApiTester.TestSingleCommand<object, IRuleIn<object>, IRuleIn<object>, RuleCommand<object>>(
                s => s.RuleTemplate(predicate, "messageKey", null),
                command =>
                {
                    command.Args.Should().BeNull();

                    command.Message.Should().Be("messageKey");

                    command.ValidCondition.Should().BeSameAs(predicate);
                });
        }

        [Fact]
        public void Should_RuleTemplate_ThrowException_When_NullPredicate()
        {
            var args = new[]
            {
                Arg.Text("1", "1"),
                Arg.Number("2", 2),
                Arg.Type("3", typeof(Guid))
            };

            ApiTester.TextException<object, IRuleIn<object>, IRuleIn<object>>(
                s => s.RuleTemplate(null, "message", args),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Fact]
        public void Should_RuleTemplate_ThrowException_When_NullMessage()
        {
            Predicate<object> predicate = x => true;

            var args = new[]
            {
                Arg.Text("1", "1"),
                Arg.Number("2", 2),
                Arg.Type("3", typeof(Guid))
            };

            ApiTester.TextException<object, IRuleIn<object>, IRuleIn<object>>(
                s => s.RuleTemplate(predicate, null, args),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Fact]
        public void Should_RuleTemplate_ThrowException_When_NullInArgs()
        {
            Predicate<object> predicate = x => true;

            var args = new[]
            {
                Arg.Text("1", "1"),
                null,
                Arg.Type("3", typeof(Guid))
            };

            ApiTester.TextException<object, IRuleIn<object>, IRuleIn<object>>(
                s => s.RuleTemplate(predicate, "message", args),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}
