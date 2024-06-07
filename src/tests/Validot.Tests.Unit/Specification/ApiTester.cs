namespace Validot.Tests.Unit.Specification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Validot.Specification;

    internal static class ApiTester
    {
        internal static void TestOutputPossibilities<TOut>(IReadOnlyList<Type> types)
        {
            var followedBy = typeof(TOut).GetInterfaces();

            followedBy.Length.Should().Be(types.Count);

            followedBy.Should().Contain(types);
        }

        internal static void TestSingleCommand<TModel, TIn, TOut, TCommand>(Func<TIn, TOut> fluentApi, Action<TCommand> validateCommand = null)
            where TIn : class
            where TCommand : class
        {
            var api = new SpecificationApi<TModel>();

            api.Should().BeAssignableTo<TIn>();

            var result = fluentApi(api as TIn);

            result.Should().BeSameAs(api);

            var processedApi = result as SpecificationApi<TModel>;

            processedApi.Commands.Count.Should().Be(1);

            var command = processedApi.Commands.Single();

            command.Should().NotBeNull();
            command.Should().BeOfType<TCommand>();

            if (validateCommand != null)
            {
                validateCommand(command as TCommand);
            }
        }

        internal static void TextException<TModel, TIn, TOut>(Func<TIn, TOut> fluentApi, Action<Action> addingAction = null)
            where TIn : class
        {
            var api = new SpecificationApi<TModel>();

            api.Should().BeAssignableTo<TIn>();

            Action action = () => fluentApi(api as TIn);

            action.Should().Throw<Exception>();

            if (addingAction != null)
            {
                addingAction(action);
            }
        }
    }
}
