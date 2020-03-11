namespace Validot.Tests.Unit.Validation.Scopes.Builders
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Errors;
    using Validot.Errors.Args;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    public class ScopeBuilderContextTests
    {
        public class TestClass
        {
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new ScopeBuilderContext();
        }

        [Fact]
        public void Should_Initialize_WithBasicErrorsRegistered_And_EmptyDictionaries()
        {
            var context = new ScopeBuilderContext();

            context.DefaultErrorId.Should().Be(0);
            context.ForbiddenErrorId.Should().Be(1);
            context.RequiredErrorId.Should().Be(2);

            context.Errors.Should().NotBeEmpty();
            context.Errors.Count.Should().Be(3);

            context.Errors[context.DefaultErrorId].ShouldBeEqualTo(new Error
            {
                Messages = new[]
                {
                    "Global.Default"
                },
                Codes = Array.Empty<string>(),
                Args = Array.Empty<IArg>(),
            });

            context.Errors[context.ForbiddenErrorId].ShouldBeEqualTo(new Error
            {
                Messages = new[]
                {
                    "Global.Forbidden"
                },
                Codes = Array.Empty<string>(),
                Args = Array.Empty<IArg>(),
            });

            context.Errors[context.RequiredErrorId].ShouldBeEqualTo(new Error
            {
                Messages = new[]
                {
                    "Global.Required"
                },
                Codes = Array.Empty<string>(),
                Args = Array.Empty<IArg>(),
            });

            context.Scopes.Should().BeEmpty();
            context.Types.Should().BeEmpty();
        }

        [Fact]
        public void RegisterError_Should_ThrowException_NullError()
        {
            var context = new ScopeBuilderContext();

            Action action = () => context.RegisterError(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void RegisterError_Should_AddError()
        {
            var context = new ScopeBuilderContext();

            var error = new Error();

            var errorId = context.RegisterError(error);

            context.Errors.Count.Should().Be(4);
            context.Errors.Keys.Should().Contain(errorId);

            context.Errors[errorId].Should().BeSameAs(error);

            context.Scopes.Should().BeEmpty();
            context.Types.Should().BeEmpty();
        }

        [Fact]
        public void RegisterError_Should_AddErrors_And_AssignDifferentIds()
        {
            var context = new ScopeBuilderContext();

            var error1 = new Error();
            var error2 = new Error();
            var error3 = new Error();

            var errorId1 = context.RegisterError(error1);
            var errorId2 = context.RegisterError(error2);
            var errorId3 = context.RegisterError(error3);

            context.Errors.Count.Should().Be(6);

            context.Errors.Keys.Should().Contain(errorId1);
            context.Errors[errorId1].Should().BeSameAs(error1);

            context.Errors.Keys.Should().Contain(errorId2);
            context.Errors[errorId2].Should().BeSameAs(error2);

            context.Errors.Keys.Should().Contain(errorId3);
            context.Errors[errorId3].Should().BeSameAs(error3);

            context.Scopes.Should().BeEmpty();
            context.Types.Should().BeEmpty();
        }

        public class GetOrRegisterSpecificationScope
        {
            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                var context = new ScopeBuilderContext();

                Action action = () => context.GetOrRegisterSpecificationScope<object>(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_CreateSpecificationScope_And_SaveType_And_ReturnItsId()
            {
                var context = new ScopeBuilderContext();

                Specification<TestClass> specification = m => m;

                var specificationScopeId = context.GetOrRegisterSpecificationScope(specification);

                specificationScopeId.Should().Be(0);

                context.Scopes.Should().NotBeEmpty();
                context.Scopes.Count.Should().Be(1);
                context.Scopes.Keys.Should().Contain(specificationScopeId);

                context.Scopes[specificationScopeId].Should().BeOfType<SpecificationScope<TestClass>>();

                context.Types.Should().NotBeEmpty();
                context.Types.Count.Should().Be(1);
                context.Types[specificationScopeId].Should().Be(typeof(TestClass));
            }

            [Fact]
            public void Should_CreateSpecificationScope_And_SaveType_And_ReturnIds_For_DifferentSpecifications()
            {
                var context = new ScopeBuilderContext();

                Specification<object> specification1 = m => m;
                Specification<DateTime?> specification2 = m => m;
                Specification<int> specification3 = m => m;

                var specificationScopeId1 = context.GetOrRegisterSpecificationScope(specification1);
                var specificationScopeId2 = context.GetOrRegisterSpecificationScope(specification2);
                var specificationScopeId3 = context.GetOrRegisterSpecificationScope(specification3);

                specificationScopeId1.Should().Be(0);
                specificationScopeId2.Should().Be(1);
                specificationScopeId3.Should().Be(2);

                context.Scopes.Should().NotBeEmpty();
                context.Scopes.Count.Should().Be(3);

                context.Scopes.Keys.Should().Contain(specificationScopeId1);
                context.Scopes[specificationScopeId1].Should().BeOfType<SpecificationScope<object>>();

                context.Scopes.Keys.Should().Contain(specificationScopeId2);
                context.Scopes[specificationScopeId2].Should().BeOfType<SpecificationScope<DateTime?>>();

                context.Scopes.Keys.Should().Contain(specificationScopeId3);
                context.Scopes[specificationScopeId3].Should().BeOfType<SpecificationScope<int>>();

                context.Types.Should().NotBeEmpty();
                context.Types.Count.Should().Be(3);
                context.Types[specificationScopeId1].Should().Be(typeof(object));
                context.Types[specificationScopeId2].Should().Be(typeof(DateTime?));
                context.Types[specificationScopeId3].Should().Be(typeof(int));
            }

            [Fact]
            public void Should_CreateSpecificationScope_And_SaveType_OnlyOnce_For_MultipleCallsWithSameSpecification()
            {
                var context = new ScopeBuilderContext();

                Specification<object> specification = m => m;

                var specificationScopeId1 = context.GetOrRegisterSpecificationScope(specification);
                var specificationScopeId2 = context.GetOrRegisterSpecificationScope(specification);
                var specificationScopeId3 = context.GetOrRegisterSpecificationScope(specification);

                specificationScopeId1.Should().Be(0);
                specificationScopeId2.Should().Be(0);
                specificationScopeId3.Should().Be(0);

                context.Scopes.Should().NotBeEmpty();
                context.Scopes.Count.Should().Be(1);

                context.Scopes.Keys.Should().Contain(specificationScopeId1);
                context.Scopes[specificationScopeId1].Should().BeOfType<SpecificationScope<object>>();

                context.Types.Should().NotBeEmpty();
                context.Types.Count.Should().Be(1);
                context.Types[specificationScopeId1].Should().Be(typeof(object));
            }
        }

        public class GetSpecificationScope
        {
            [Fact]
            public void Should_ThrowException_When_InvalidId()
            {
                var context = new ScopeBuilderContext();

                Action action = () =>
                {
                    context.GetDiscoverableSpecificationScope(321);
                };

                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_GetSpecificationScope()
            {
                var context = new ScopeBuilderContext();

                Specification<TestClass> specification = m => m;

                var specificationScopeId = context.GetOrRegisterSpecificationScope(specification);

                var discoverableSpecificationScope = context.GetDiscoverableSpecificationScope(specificationScopeId);

                discoverableSpecificationScope.Should().BeSameAs(context.Scopes[specificationScopeId]);
            }
        }
    }
}
