namespace Validot.Tests.Unit.Specification.Commands
{
    using System;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Specification.Commands;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class MemberCommandTests
    {
        private class SomeModel
        {
            public object SomeReferenceProperty { get; set; }

            public int SomeValueProperty { get; set; }

            public object SomeReferenceVariable;

            public int SomeValueVariable;

            public object SomeFunctionReturningReference() => null;

            public int SomeFunctionReturningValue() => 888;
        }

        [Fact]
        public void Should_Get_BlockBuilder()
        {
            Specification<object> specification = s => s;

            var command = new MemberCommand<SomeModel, object>(m => m.SomeReferenceProperty, specification);

            var blockBuilder = command.GetScopeBuilder();

            blockBuilder.Should().NotBeNull();
        }

        [Fact]
        public void Should_GetOrRegisterSpecification_And_ModelBlock()
        {
            Specification<object> specification = s => s;

            var command = new MemberCommand<SomeModel, object>(m => m.SomeReferenceProperty, specification);

            var blockBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            buildingContext.GetOrRegisterSpecificationScope(Arg.Is<Specification<object>>(arg => ReferenceEquals(arg, specification))).Returns(666);

            var block = blockBuilder.Build(buildingContext);

            buildingContext.Received(1).GetOrRegisterSpecificationScope(Arg.Is<Specification<object>>(arg => ReferenceEquals(arg, specification)));

            block.Should().BeOfType<MemberCommandScope<SomeModel, object>>();

            var modelBlock = (MemberCommandScope<SomeModel, object>)block;

            modelBlock.ScopeId.Should().Be(666);
        }

        [Fact]
        public void Should_Process_Reference_Property()
        {
            Specification<object> specification = s => s;

            var command = new MemberCommand<SomeModel, object>(m => m.SomeReferenceProperty, specification);

            var blockBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            buildingContext.GetOrRegisterSpecificationScope(Arg.Any<Specification<object>>()).Returns(666);

            var block = blockBuilder.Build(buildingContext);

            block.Should().BeOfType<MemberCommandScope<SomeModel, object>>();

            var modelBlock = (MemberCommandScope<SomeModel, object>)block;

            modelBlock.Name = "SomeReferenceProperty";
            modelBlock.GetMemberValue.Should().BeOfType<Func<SomeModel, object>>();

            var someModel = new SomeModel()
            {
                SomeReferenceProperty = new object()
            };

            var memberValue = modelBlock.GetMemberValue(someModel);

            memberValue.Should().BeSameAs(someModel.SomeReferenceProperty);
        }

        [Fact]
        public void Should_Process_Reference_Variable()
        {
            Specification<object> specification = s => s;

            var command = new MemberCommand<SomeModel, object>(m => m.SomeReferenceVariable, specification);

            var blockBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            buildingContext.GetOrRegisterSpecificationScope(Arg.Any<Specification<object>>()).Returns(666);

            var block = blockBuilder.Build(buildingContext);

            block.Should().BeOfType<MemberCommandScope<SomeModel, object>>();

            var modelBlock = (MemberCommandScope<SomeModel, object>)block;

            modelBlock.Name = "SomeReferenceVariable";
            modelBlock.GetMemberValue.Should().BeOfType<Func<SomeModel, object>>();

            var someModel = new SomeModel()
            {
                SomeReferenceVariable = new object()
            };

            var memberValue = modelBlock.GetMemberValue(someModel);

            memberValue.Should().BeSameAs(someModel.SomeReferenceVariable);
        }

        [Fact]
        public void Should_Process_ValueType_Property()
        {
            Specification<int> specification = s => s;

            var command = new MemberCommand<SomeModel, int>(m => m.SomeValueProperty, specification);

            var blockBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            buildingContext.GetOrRegisterSpecificationScope(Arg.Any<Specification<int>>()).Returns(666);

            var block = blockBuilder.Build(buildingContext);

            block.Should().BeOfType<MemberCommandScope<SomeModel, int>>();

            var modelBlock = (MemberCommandScope<SomeModel, int>)block;

            modelBlock.Name = "SomeValueProperty";
            modelBlock.GetMemberValue.Should().BeOfType<Func<SomeModel, int>>();

            var someModel = new SomeModel()
            {
                SomeValueProperty = 777
            };

            var memberValue = modelBlock.GetMemberValue(someModel);

            memberValue.Should().Be(777);
        }

        [Fact]
        public void Should_Process_ValueType_Variable()
        {
            Specification<int> specification = s => s;

            var command = new MemberCommand<SomeModel, int>(m => m.SomeValueVariable, specification);

            var blockBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            buildingContext.GetOrRegisterSpecificationScope(Arg.Any<Specification<int>>()).Returns(666);

            var block = blockBuilder.Build(buildingContext);

            block.Should().BeOfType<MemberCommandScope<SomeModel, int>>();

            var modelBlock = (MemberCommandScope<SomeModel, int>)block;

            modelBlock.Name = "SomeValueVariable";
            modelBlock.GetMemberValue.Should().BeOfType<Func<SomeModel, int>>();

            var someModel = new SomeModel()
            {
                SomeValueVariable = 777
            };

            var memberValue = modelBlock.GetMemberValue(someModel);

            memberValue.Should().Be(777);
        }

        [Fact]
        public void Should_ThrowException_When_MemberIsFunction_ReturningValueType()
        {
            Specification<int> specification = s => s;

            var command = new MemberCommand<SomeModel, int>(m => m.SomeFunctionReturningValue(), specification);
            var blockBuilder = command.GetScopeBuilder();
            var buildingContext = Substitute.For<IScopeBuilderContext>();
            buildingContext.GetOrRegisterSpecificationScope(Arg.Any<Specification<int>>()).Returns(666);

            Action action = () => blockBuilder.Build(buildingContext);

            action.Should().ThrowExactly<InvalidOperationException>().WithMessage("Only properties and variables are valid members to validate");
        }

        [Fact]
        public void Should_ThrowException_When_MemberIsFunction_ReturningReferenceType()
        {
            Specification<object> specification = s => s;

            var command = new MemberCommand<SomeModel, object>(m => m.SomeFunctionReturningReference(), specification);
            var blockBuilder = command.GetScopeBuilder();
            var buildingContext = Substitute.For<IScopeBuilderContext>();
            buildingContext.GetOrRegisterSpecificationScope(Arg.Any<Specification<object>>()).Returns(666);

            Action action = () => blockBuilder.Build(buildingContext);

            action.Should().ThrowExactly<InvalidOperationException>().WithMessage("Only properties and variables are valid members to validate");
        }
    }
}
