namespace Validot.Tests.Unit.Validation.Scopes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NSubstitute;

    using Validot.Validation;
    using Validot.Validation.Scopes;

    using Xunit;

    public class SpecificationScopeTests
    {
        public class TestClass
        {
        }

        public static IEnumerable<object[]> SpecificationScopeParameters_Data()
        {
            var presences = new[]
            {
                Presence.Optional,
                Presence.Required,
                Presence.Forbidden
            };

            var commandScopesCount = new[]
            {
                0,
                1,
                5
            };

            foreach (var presence in presences)
            {
                foreach (var count in commandScopesCount)
                {
                    yield return new object[]
                    {
                        presence,
                        count
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(SpecificationScopeParameters_Data))]
        public void Should_Discover_ReferenceType(object presenceObj, int commandScopesCount)
        {
            var presence = (Presence)presenceObj;

            var commandScopes = Enumerable.Range(0, commandScopesCount).Select(m =>
            {
                return Substitute.For<ICommandScope<TestClass>>();
            }).ToList();

            var specificationScope = new SpecificationScope<TestClass>();

            specificationScope.Presence = presence;
            specificationScope.CommandScopes = commandScopes;
            specificationScope.ForbiddenErrorId = 321;
            specificationScope.RequiredErrorId = 123;

            var discoveryContext = Substitute.For<IDiscoveryContext>();

            specificationScope.Discover(discoveryContext);

            Received.InOrder(() =>
            {
                if (presence == Presence.Forbidden)
                {
                    discoveryContext.AddError(321, true);
                }
                else
                {
                    if (presence == Presence.Required)
                    {
                        discoveryContext.AddError(123, true);
                    }

                    for (var i = 0; i < commandScopesCount; ++i)
                    {
                        commandScopes[i].Discover(Arg.Is(discoveryContext));
                    }
                }
            });

            if (presence == Presence.Optional)
            {
                discoveryContext.DidNotReceiveWithAnyArgs().AddError(default);
            }

            if (presence == Presence.Forbidden)
            {
                for (var i = 0; i < commandScopesCount; ++i)
                {
                    commandScopes[i].DidNotReceiveWithAnyArgs().Discover(default);
                }
            }
            else if (presence == Presence.Required)
            {
                discoveryContext.Received(1).AddError(123, true);
            }

            discoveryContext.DidNotReceiveWithAnyArgs().LeavePath();
            discoveryContext.DidNotReceiveWithAnyArgs().EnterPath(default);
            discoveryContext.DidNotReceiveWithAnyArgs().EnterCollectionItemPath();
            discoveryContext.DidNotReceiveWithAnyArgs().EnterScope<TestClass>(default);
        }

        [Theory]
        [MemberData(nameof(SpecificationScopeParameters_Data))]
        public void Should_Discover_ValueType(object presenceObj, int commandScopesCount)
        {
            var presence = (Presence)presenceObj;

            var commandScopes = Enumerable.Range(0, commandScopesCount).Select(m =>
            {
                return Substitute.For<ICommandScope<decimal>>();
            }).ToList();

            var specificationScope = new SpecificationScope<decimal>();

            specificationScope.Presence = presence;
            specificationScope.CommandScopes = commandScopes;
            specificationScope.ForbiddenErrorId = 321;
            specificationScope.RequiredErrorId = 123;

            var discoveryContext = Substitute.For<IDiscoveryContext>();

            specificationScope.Discover(discoveryContext);

            Received.InOrder(() =>
            {
                for (var i = 0; i < commandScopesCount; ++i)
                {
                    commandScopes[i].Discover(Arg.Is(discoveryContext));
                }
            });

            discoveryContext.DidNotReceiveWithAnyArgs().AddError(default);
            discoveryContext.DidNotReceiveWithAnyArgs().LeavePath();
            discoveryContext.DidNotReceiveWithAnyArgs().EnterPath(default);
            discoveryContext.DidNotReceiveWithAnyArgs().EnterCollectionItemPath();
            discoveryContext.DidNotReceiveWithAnyArgs().EnterScope<TestClass>(default);
        }

        public static IEnumerable<object[]> Should_Validate_ReferenceType_Data()
        {
            var parametersData = SpecificationScopeParameters_Data();

            foreach (var parameters in parametersData)
            {
                yield return new object[]
                {
                    parameters[0],
                    parameters[1],
                    true
                };

                yield return new object[]
                {
                    parameters[0],
                    parameters[1],
                    false
                };
            }
        }

        [Theory]
        [MemberData(nameof(Should_Validate_ReferenceType_Data))]
        public void Should_Validate_ReferenceType(object presenceObj, int commandScopesCount, bool nullModel)
        {
            var presence = (Presence)presenceObj;

            var commandScopes = Enumerable.Range(0, commandScopesCount).Select(m =>
            {
                return Substitute.For<ICommandScope<TestClass>>();
            }).ToList();

            var specificationScope = new SpecificationScope<TestClass>();

            specificationScope.Presence = presence;
            specificationScope.CommandScopes = commandScopes;
            specificationScope.ForbiddenErrorId = 321;
            specificationScope.RequiredErrorId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            TestClass model = nullModel ? null : new TestClass();

            specificationScope.Validate(model, validationContext);

            Received.InOrder(() =>
            {
                if (nullModel)
                {
                    if (presence == Presence.Required)
                    {
                        validationContext.AddError(Arg.Is(123), true);
                    }
                }
                else
                {
                    if (presence == Presence.Forbidden)
                    {
                        validationContext.AddError(Arg.Is(321), true);
                    }
                    else
                    {
                        for (var i = 0; i < commandScopesCount; ++i)
                        {
                            commandScopes[i].Validate(Arg.Is(model), Arg.Is(validationContext));
                        }
                    }
                }
            });

            if (presence == Presence.Optional)
            {
                validationContext.DidNotReceiveWithAnyArgs().AddError(default, default);
            }

            if (nullModel)
            {
                for (var i = 0; i < commandScopesCount; ++i)
                {
                    commandScopes[i].DidNotReceiveWithAnyArgs().Validate(default, default);
                }
            }

            validationContext.DidNotReceiveWithAnyArgs().LeavePath();
            validationContext.DidNotReceiveWithAnyArgs().EnterPath(default);
            validationContext.DidNotReceiveWithAnyArgs().EnterCollectionItemPath(default);
            validationContext.DidNotReceiveWithAnyArgs().EnableErrorDetectionMode(default, default);
        }

        [Theory]
        [MemberData(nameof(SpecificationScopeParameters_Data))]
        public void Should_Validate_ValueType(object presenceObj, int commandScopesCount)
        {
            var presence = (Presence)presenceObj;

            var commandScopes = Enumerable.Range(0, commandScopesCount).Select(m =>
            {
                return Substitute.For<ICommandScope<decimal>>();
            }).ToList();

            var specificationScope = new SpecificationScope<decimal>();

            specificationScope.Presence = presence;
            specificationScope.CommandScopes = commandScopes;
            specificationScope.ForbiddenErrorId = 321;
            specificationScope.RequiredErrorId = 123;

            var validationContext = Substitute.For<IValidationContext>();

            var model = 234M;

            specificationScope.Validate(model, validationContext);

            Received.InOrder(() =>
            {
                for (var i = 0; i < commandScopesCount; ++i)
                {
                    commandScopes[i].Validate(Arg.Is(model), Arg.Is(validationContext));
                }
            });

            validationContext.DidNotReceiveWithAnyArgs().AddError(default);
            validationContext.DidNotReceiveWithAnyArgs().LeavePath();
            validationContext.DidNotReceiveWithAnyArgs().EnterPath(default);
            validationContext.DidNotReceiveWithAnyArgs().EnterCollectionItemPath(default);
            validationContext.DidNotReceiveWithAnyArgs().EnableErrorDetectionMode(default, default);
        }

        [Theory]
        [MemberData(nameof(SpecificationScopeParameters_Data))]
        public void Should_Validate_And_FallBack(object presenceObj, int fallBackIndex)
        {
            var presence = (Presence)presenceObj;

            var validateCount = 0;

            var shouldFallBack = false;
            var validationContext = Substitute.For<IValidationContext>();

            validationContext.ShouldFallBack.Returns(c => shouldFallBack);

            var commandScopes = Enumerable.Range(0, 5).Select(m =>
            {
                var cmdScope = Substitute.For<ICommandScope<decimal>>();

                cmdScope.When(x => x.Validate(Arg.Any<decimal>(), Arg.Any<IValidationContext>())).Do(callInfo =>
                {
                    shouldFallBack = ++validateCount > fallBackIndex;
                });

                return cmdScope;
            }).ToList();

            var specificationScope = new SpecificationScope<decimal>();

            specificationScope.Presence = presence;
            specificationScope.CommandScopes = commandScopes;
            specificationScope.ForbiddenErrorId = 321;
            specificationScope.RequiredErrorId = 123;

            var model = 234M;

            specificationScope.Validate(model, validationContext);

            Received.InOrder(() =>
            {
                var limit = Math.Min(fallBackIndex + 1, commandScopes.Count);

                for (var i = 0; i < limit; ++i)
                {
                    commandScopes[i].Validate(Arg.Is(model), Arg.Is(validationContext));
                }
            });

            validationContext.DidNotReceiveWithAnyArgs().AddError(default);
            validationContext.DidNotReceiveWithAnyArgs().LeavePath();
            validationContext.DidNotReceiveWithAnyArgs().EnterPath(default);
            validationContext.DidNotReceiveWithAnyArgs().EnterCollectionItemPath(default);
            validationContext.DidNotReceiveWithAnyArgs().EnableErrorDetectionMode(default, default);
        }
    }
}
