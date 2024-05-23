namespace Validot.Tests.Unit.Specification.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Specification.Commands;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class AsDictionaryCommandTests
    {
        [Fact]
        public void Should_Get_ScopeBuilder()
        {
            Specification<object> specification = s => s;
            Func<int, string> keyStringifier = i => i.ToString(CultureInfo.InvariantCulture);

            var command = new AsDictionaryCommand<IReadOnlyDictionary<int, object>, int, object>(specification, keyStringifier);
            var scopeBuilder = command.GetScopeBuilder();

            scopeBuilder.Should().NotBeNull();
        }

        [Fact]
        public void Should_GetOrRegisterSpecification_And_AddModelBlock()
        {
            Specification<object> specification = s => s;

            var command = new AsDictionaryCommand<IReadOnlyDictionary<int, object>, int, object>(specification, i => i.ToString(CultureInfo.InvariantCulture));

            var scopeBuilder = command.GetScopeBuilder();

            var buildingContext = Substitute.For<IScopeBuilderContext>();

            buildingContext.GetOrRegisterSpecificationScope(Arg.Is<Specification<object>>(arg => ReferenceEquals(arg, specification))).Returns(666);

            var scope = scopeBuilder.Build(buildingContext);

            scope.Should().BeOfType<DictionaryCommandScope<IReadOnlyDictionary<int, object>, int, object>>();

            var modelScope = (DictionaryCommandScope<IReadOnlyDictionary<int, object>, int, object>)scope;

            modelScope.ScopeId.Should().Be(666);

            buildingContext.Received(1).GetOrRegisterSpecificationScope(Arg.Is<Specification<object>>(arg => ReferenceEquals(arg, specification)));
        }
    }
}