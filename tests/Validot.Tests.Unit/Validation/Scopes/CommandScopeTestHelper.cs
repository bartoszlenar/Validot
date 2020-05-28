namespace Validot.Tests.Unit.Validation.Scopes
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Validation;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    internal static class CommandScopeTestHelper
    {
        public static void ShouldHaveDefaultValues<T>(this ICommandScope<T> commandScope)
        {
            commandScope.ExecutionCondition.Should().BeNull();
            commandScope.ErrorId.Should().NotHaveValue();
            commandScope.Path.Should().BeNull();
            commandScope.ErrorMode.Should().Be(ErrorMode.Append);
        }

        public static IEnumerable<object[]> CommandScopeParameters()
        {
            var shouldExecuteInfo = new bool?[]
            {
                true,
                false,
                null
            };

            var errorIdValues = new int?[]
            {
                null,
                1
            };

            var errorModeValues = new[]
            {
                ErrorMode.Append,
                ErrorMode.Override,
            };

            var nameValues = new[]
            {
                null,
                "someName"
            };

            foreach (var shouldExecute in shouldExecuteInfo)
            {
                foreach (var errorId in errorIdValues)
                {
                    foreach (var errorMode in errorModeValues)
                    {
                        foreach (var name in nameValues)
                        {
                            yield return new object[]
                            {
                                shouldExecute,
                                errorId,
                                errorMode,
                                name
                            };
                        }
                    }
                }
            }
        }

        public static void ShouldDiscover<T>(this ICommandScope<T> @this, IDiscoveryContext context, Action<IDiscoveryContext> callsAssertions)
        {
            @this.Discover(context);

            Received.InOrder(() =>
            {
                context.EnterPath(Arg.Is(@this.Path));

                if (!@this.ErrorId.HasValue || @this.ErrorMode == ErrorMode.Append)
                {
                    callsAssertions(context);
                }

                if (@this.ErrorId.HasValue)
                {
                    context.AddError(Arg.Is(@this.ErrorId.Value));
                }

                context.LeavePath();
            });
        }

        public static void ShouldValidate<T>(this ICommandScope<T> @this, T model, IValidationContext context, bool? shouldExecuteInfo, Action<IValidationContext> callsAssertions)
        {
            @this.Validate(model, context);

            var shouldExecute = !shouldExecuteInfo.HasValue || shouldExecuteInfo.Value;

            Received.InOrder(() =>
            {
                if (shouldExecute)
                {
                    context.EnterPath(Arg.Is(@this.Path));

                    if (@this.ErrorId.HasValue)
                    {
                        context.EnableErrorDetectionMode(Arg.Is(@this.ErrorMode), Arg.Is(@this.ErrorId.Value));
                    }

                    callsAssertions(context);

                    context.LeavePath();
                }
            });

            if (!shouldExecute)
            {
                context.DidNotReceiveWithAnyArgs().EnterPath(default);
                context.DidNotReceiveWithAnyArgs().EnableErrorDetectionMode(default, default);
                context.DidNotReceiveWithAnyArgs().AddError(default, default);
                context.DidNotReceiveWithAnyArgs().EnterCollectionItemPath(default);
                context.DidNotReceiveWithAnyArgs().LeavePath();
            }
        }
    }
}
