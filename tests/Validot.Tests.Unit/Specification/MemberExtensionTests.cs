namespace Validot.Tests.Unit.Specification
{
    using System;
    using System.Linq.Expressions;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class MemberExtensionTests
    {
        private class TestClass
        {
            public string TestProperty { get; set; }
        }

        [Fact]
        public void Should_Add_MemberCommand()
        {
            Specification<string> memberSpecification = s => s;
            Expression<Func<TestClass, string>> memberSelector = m => m.TestProperty;

            ApiTester.TestSingleCommand<TestClass, IRuleIn<TestClass>, IRuleOut<TestClass>, MemberCommand<TestClass, string>>(
                s => s.Member(memberSelector, memberSpecification),
                command =>
                {
                    command.Specification.Should().NotBeNull();
                    command.Specification.Should().BeSameAs(memberSpecification);

                    command.MemberSelector.Should().NotBeNull();
                    command.MemberSelector.Should().BeSameAs(memberSelector);
                });
        }

        [Fact]
        public void Should_ThrowException_When_NullMemberSelector()
        {
            Specification<string> memberSpecification = s => s;

            ApiTester.TextException<TestClass, IRuleIn<TestClass>, IRuleOut<TestClass>>(
                s => s.Member(null, memberSpecification),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }

        [Fact]
        public void Should_ThrowException_When_NullMemberSpecification()
        {
            Expression<Func<TestClass, string>> memberSelector = m => m.TestProperty;

            ApiTester.TextException<TestClass, IRuleIn<TestClass>, IRuleOut<TestClass>>(
                s => s.Member(memberSelector, null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}
