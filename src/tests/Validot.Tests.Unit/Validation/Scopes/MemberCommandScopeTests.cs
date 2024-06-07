namespace Validot.Tests.Unit.Validation.Scopes
{
    using System;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Validation;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class MemberCommandScopeTests
    {
        public class TestMember
        {
        }

        public class TestClass
        {
            public TestMember Member { get; set; }

            public decimal ValueMember { get; set; }
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new MemberCommandScope<TestClass, TestMember>();
        }

        [Fact]
        public void Should_Initialize_WithDefaultValues()
        {
            var commandScope = new MemberCommandScope<TestClass, TestMember>();

            commandScope.ShouldHaveDefaultValues();
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunDiscovery(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path)
        {
            var commandScope = new MemberCommandScope<TestClass, TestMember>();

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestClass>)null
                : m =>
                {
                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var model = new TestClass();

            var getMemberValueCount = 0;

            commandScope.GetMemberValue = m =>
            {
                m.Should().BeSameAs(model);
                m.Member.Should().BeSameAs(model.Member);

                getMemberValueCount++;

                return m.Member;
            };

            var discoveryContext = Substitute.For<IDiscoveryContext>();

            commandScope.ShouldDiscover(discoveryContext, context =>
            {
                context.Received().EnterScope<TestMember>(Arg.Is(123));
            });

            getMemberValueCount.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunValidation_OnReferenceTypeMember(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path)
        {
            var commandScope = new MemberCommandScope<TestClass, TestMember>();

            var model = new TestClass()
            {
                Member = new TestMember()
            };

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestClass>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var getMemberValueCount = 0;

            commandScope.GetMemberValue = m =>
            {
                m.Should().BeSameAs(model);
                m.Member.Should().BeSameAs(model.Member);

                getMemberValueCount++;

                return m.Member;
            };

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    context.Received().EnterScope(Arg.Is(123), Arg.Is(model.Member));
                });

            getMemberValueCount.Should().Be(!shouldExecuteInfo.HasValue || shouldExecuteInfo.Value ? 1 : 0);

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }

        [Theory]
        [MemberData(nameof(CommandScopeTestHelper.CommandScopeParameters), MemberType = typeof(CommandScopeTestHelper))]
        public void Should_RunValidation_OnValueTypeMember(bool? shouldExecuteInfo, int? errorId, object errorModeBoxed, string path)
        {
            var commandScope = new MemberCommandScope<TestClass, decimal>();

            var model = new TestClass()
            {
                ValueMember = 987M
            };

            var shouldExecuteCount = 0;

            commandScope.ExecutionCondition = !shouldExecuteInfo.HasValue
                ? (Predicate<TestClass>)null
                : m =>
                {
                    m.Should().BeSameAs(model);
                    shouldExecuteCount++;

                    return shouldExecuteInfo.Value;
                };

            commandScope.ErrorId = errorId;

            commandScope.ErrorMode = (ErrorMode)errorModeBoxed;

            commandScope.Path = path;

            commandScope.ScopeId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var getMemberValueCount = 0;

            commandScope.GetMemberValue = m =>
            {
                m.Should().BeSameAs(model);
                m.ValueMember.Should().Be(987);

                getMemberValueCount++;

                return m.ValueMember;
            };

            commandScope.ShouldValidate(
                model,
                validationContext,
                shouldExecuteInfo,
                context =>
                {
                    context.Received().EnterScope(Arg.Is(123), Arg.Is(model.ValueMember));
                });

            getMemberValueCount.Should().Be(!shouldExecuteInfo.HasValue || shouldExecuteInfo.Value ? 1 : 0);

            shouldExecuteCount.Should().Be(shouldExecuteInfo.HasValue ? 1 : 0);
        }
    }
}
