namespace Validot.Tests.Unit.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using Validot.Validation;
    using Validot.Validation.Scheme;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;
    using Validot.Validation.Stacks;

    using Xunit;

    public class IsValidValidationContextTests
    {
        public class Initializing
        {
            [Fact]
            public void Should_Initialize()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                _ = new IsValidValidationContext(modelScheme, default);
            }

            [Fact]
            public void Should_Initialize_WithDefaultValues()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                var validationContext = new IsValidValidationContext(modelScheme, default);

                validationContext.ReferenceLoopProtectionSettings.Should().BeNull();
                validationContext.ErrorFound.Should().BeFalse();
                validationContext.ShouldFallBack.Should().BeFalse();
            }

            [Fact]
            public void Should_Initialize_WithReferenceLoopProtectionSettings()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                ReferenceLoopProtectionSettings referenceLoopProtectionSettings = null;

                modelScheme.RootModelType.Returns(typeof(object));
                referenceLoopProtectionSettings = new ReferenceLoopProtectionSettings();

                var validationContext = new IsValidValidationContext(modelScheme, referenceLoopProtectionSettings);

                validationContext.ReferenceLoopProtectionSettings.Should().BeSameAs(referenceLoopProtectionSettings);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Initialize_ReferencesStack_With_Null_When_LoopProtectionSettings_Is_Null(bool rootModelTypeIsReference)
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.RootModelType.Returns(rootModelTypeIsReference ? typeof(object) : typeof(int));

                var validationContext = new IsValidValidationContext(
                    modelScheme,
                    null);

                validationContext.GetLoopProtectionReferencesStackCount().Should().BeNull();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Initialize_ReferencesStack_With_Zero_When_LoopProtectionSettings_Has_NullRootModel(bool rootModelTypeIsReference)
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.RootModelType.Returns(rootModelTypeIsReference ? typeof(object) : typeof(int));

                var validationContext = new IsValidValidationContext(
                    modelScheme,
                    new ReferenceLoopProtectionSettings());

                validationContext.GetLoopProtectionReferencesStackCount().Should().Be(0);
            }

            [Fact]
            public void Should_Initialize_ReferencesStack_With_One_When_LoopProtectionSettings_Has_RootModel_And_RootModelTypeInSchemeIsReferenceType()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.RootModelType.Returns(typeof(object));

                var validationContext = new IsValidValidationContext(
                    modelScheme,
                    new ReferenceLoopProtectionSettings(new object()));

                validationContext.GetLoopProtectionReferencesStackCount().Should().Be(1);
            }

            [Fact]
            public void Should_Initialize_ReferencesStack_With_Zero_When_LoopProtectionSettings_Has_RootModel_And_RootModelTypeInSchemeIsValueType()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.RootModelType.Returns(typeof(int));

                var validationContext = new IsValidValidationContext(
                    modelScheme,
                    new ReferenceLoopProtectionSettings(new object()));

                validationContext.GetLoopProtectionReferencesStackCount().Should().Be(0);
            }
        }

        [Fact]
        public void ShouldFallback_Should_BeSameAs_ErrorsFound()
        {
            var modelScheme = Substitute.For<IModelScheme>();

            var validationContext = new IsValidValidationContext(modelScheme, default);

            validationContext.ErrorFound.Should().BeFalse();
            validationContext.ShouldFallBack.Should().BeFalse();

            validationContext.AddError(123);

            validationContext.ErrorFound.Should().BeTrue();
            validationContext.ShouldFallBack.Should().BeTrue();
        }

        public class ErrorFound
        {
            [Fact]
            public void Should_BeFalse_Initially()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var context = new IsValidValidationContext(modelScheme, default);

                context.ErrorFound.Should().BeFalse();
            }

            [Fact]
            public void Should_BeFalse_When_AddErrorNotCalled()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var context = new IsValidValidationContext(modelScheme, default);

                context.EnterPath("asd");
                context.EnterPath("123");
                context.LeavePath();
                context.EnterCollectionItemPath(0);
                context.EnableErrorDetectionMode(ErrorMode.Append, 1);
                context.EnableErrorDetectionMode(ErrorMode.Override, 2);

                context.ErrorFound.Should().BeFalse();
            }

            [Fact]
            public void Should_BeTrue_When_ErrorAdded()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var context = new IsValidValidationContext(modelScheme, default);

                context.AddError(123);

                context.ErrorFound.Should().BeTrue();
            }

            [Fact]
            public void Should_BeTrue_When_ErrorAdded_AndOtherMethodsDoesntMakeAnyDifference()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var context = new IsValidValidationContext(modelScheme, default);

                context.AddError(123);

                context.EnterPath("asd");
                context.EnterPath("123");
                context.LeavePath();
                context.EnterCollectionItemPath(0);
                context.EnableErrorDetectionMode(ErrorMode.Append, 1);
                context.EnableErrorDetectionMode(ErrorMode.Override, 2);

                context.ErrorFound.Should().BeTrue();
            }

            [Fact]
            public void Should_BeTrue_When_ErrorAdded_MultipleTimes()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var context = new IsValidValidationContext(modelScheme, default);

                context.AddError(123);
                context.AddError(123);
                context.AddError(321);
                context.AddError(666);

                context.ErrorFound.Should().BeTrue();
            }
        }

        public class EnterScope
        {
            public class TestClass
            {
            }

            [Fact]
            public void Should_UseModelScheme()
            {
                var specificationScope = Substitute.For<ISpecificationScope<TestClass>>();

                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.GetSpecificationScope<TestClass>(Arg.Is(1234)).Returns(specificationScope);

                var context = new IsValidValidationContext(modelScheme, default);

                var model = new TestClass();

                context.EnterScope(1234, model);

                Received.InOrder(() =>
                {
                    modelScheme.GetSpecificationScope<TestClass>(Arg.Is(1234));
                    specificationScope.Validate(Arg.Is(model), Arg.Is(context));
                });

                specificationScope.DidNotReceive().Discover(Arg.Any<IDiscoveryContext>());

                specificationScope.Received(1).Validate(Arg.Any<TestClass>(), Arg.Any<IsValidValidationContext>());
            }

            [Fact]
            public void Should_UseModelScheme_MultipleTimes()
            {
                var specificationScope = Substitute.For<ISpecificationScope<TestClass>>();

                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.GetSpecificationScope<TestClass>(Arg.Is(1234)).Returns(specificationScope);

                var context = new IsValidValidationContext(modelScheme, default);

                var model = new TestClass();

                context.EnterScope(1234, model);
                context.EnterScope(1234, model);

                Received.InOrder(() =>
                {
                    modelScheme.GetSpecificationScope<TestClass>(Arg.Is(1234));
                    specificationScope.Validate(Arg.Is(model), Arg.Is(context));

                    modelScheme.GetSpecificationScope<TestClass>(Arg.Is(1234));
                    specificationScope.Validate(Arg.Is(model), Arg.Is(context));
                });

                specificationScope.DidNotReceive().Discover(Arg.Any<IDiscoveryContext>());

                specificationScope.Received(2).Validate(Arg.Any<TestClass>(), Arg.Any<IsValidValidationContext>());
            }

            [Fact]
            public void Should_Return_DifferentScopesForDifferentTypesAndIds()
            {
                var model1 = new TestClass();
                var specificationScope1 = Substitute.For<ISpecificationScope<TestClass>>();

                var model2 = new TestClass();
                var specificationScope2 = Substitute.For<ISpecificationScope<TestClass>>();

                var model3 = new DateTimeOffset?(DateTimeOffset.FromUnixTimeSeconds(3));
                var specificationScope3 = Substitute.For<ISpecificationScope<DateTimeOffset?>>();

                var model4 = 4M;
                var specificationScope4 = Substitute.For<ISpecificationScope<decimal>>();

                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.GetSpecificationScope<TestClass>(Arg.Is(1)).Returns(specificationScope1);
                modelScheme.GetSpecificationScope<TestClass>(Arg.Is(2)).Returns(specificationScope2);
                modelScheme.GetSpecificationScope<DateTimeOffset?>(Arg.Is(3)).Returns(specificationScope3);
                modelScheme.GetSpecificationScope<decimal>(Arg.Is(4)).Returns(specificationScope4);

                var context = new IsValidValidationContext(modelScheme, default);

                context.EnterScope(1, model1);
                context.EnterScope(2, model2);
                context.EnterScope(3, model3);
                context.EnterScope(4, model4);

                Received.InOrder(() =>
                {
                    modelScheme.GetSpecificationScope<TestClass>(Arg.Is(1));
                    specificationScope1.Validate(Arg.Is(model1), Arg.Is(context));

                    modelScheme.GetSpecificationScope<TestClass>(Arg.Is(2));
                    specificationScope2.Validate(Arg.Is(model2), Arg.Is(context));

                    modelScheme.GetSpecificationScope<DateTimeOffset?>(Arg.Is(3));
                    specificationScope3.Validate(Arg.Is(model3), Arg.Is(context));

                    modelScheme.GetSpecificationScope<decimal>(Arg.Is(4));
                    specificationScope4.Validate(Arg.Is(model4), Arg.Is(context));
                });

                modelScheme.Received(1).GetSpecificationScope<TestClass>(Arg.Is(1));
                modelScheme.Received(1).GetSpecificationScope<TestClass>(Arg.Is(2));
                modelScheme.Received(1).GetSpecificationScope<DateTimeOffset?>(Arg.Is(3));
                modelScheme.Received(1).GetSpecificationScope<decimal>(Arg.Is(4));
            }

            [Fact]
            public void Should_RethrowException_When_ModelSchemeThrows()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var exception = new KeyNotFoundException();
                modelScheme.GetSpecificationScope<TestClass>(Arg.Is(1234)).Throws(exception);

                var context = new IsValidValidationContext(modelScheme, default);

                Action action = () => context.EnterScope(1234, new TestClass());

                action.Should().ThrowExactly<KeyNotFoundException>();
            }
        }

        public class EnterScope_And_Fail_When_ReferenceLoopExists
        {
            [Theory]
            [MemberData(nameof(TraversingTestCases.Loop_Self), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.Loop_Simple), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.Loop_ThroughMembers), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.Loop_ThroughTypes), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.Loop_ThroughIndexes), MemberType = typeof(TraversingTestCases))]
            public void Should_ThrowException_InfiniteReferencesLoopException_WithDetectedLoopInfo_When_ReferencesLoopDetected(string testId, Specification<TraversingTestCases.LoopClassA> specification, TraversingTestCases.LoopClassA model, string path, string infiniteLoopNestedPath, Type type)
            {
                _ = testId;
                _ = path;
                _ = infiniteLoopNestedPath;

                var modelScheme = ModelSchemeFactory.Create(specification);

                var context = new IsValidValidationContext(modelScheme, new ReferenceLoopProtectionSettings());

                Action action = () => context.EnterScope(modelScheme.RootSpecificationScopeId, model);

                var exception = action.Should().ThrowExactly<ReferenceLoopException>().And;

                exception.Path.Should().BeNull();
                exception.NestedPath.Should().BeNull();
                exception.Type.Should().Be(type);

                exception.Message.Should().Be($"Reference loop detected: object of type {type.GetFriendlyName()} has been detected twice in the reference graph, effectively creating the infinite references loop (where exactly, that information is not available - is that validation comes from IsValid method, please repeat it using the Validate method and examine the exception thrown)");
            }
        }

        public class EnterScope_And_TrackingReferencesLoops
        {
            [Theory]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Common), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Struct), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Collections), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Nullable), MemberType = typeof(TraversingTestCases))]
            public void Should_GetLoopProtectionReferencesStackCount_BeZero_BeforeAndAfterEnteringRootScope_When_NoRootReferenceInSettings(string id, Specification<TraversingTestCases.TestClassA> rootSpecification, TraversingTestCases.TestClassA model)
            {
                _ = id;

                var modelScheme = ModelSchemeFactory.Create(rootSpecification);

                var context = new IsValidValidationContext(modelScheme, new ReferenceLoopProtectionSettings());

                context.GetLoopProtectionReferencesStackCount().Should().Be(0);

                context.EnterScope(modelScheme.RootSpecificationScopeId, model);

                context.GetLoopProtectionReferencesStackCount().Should().Be(0);
            }

            [Theory]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Common), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Struct), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Collections), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Nullable), MemberType = typeof(TraversingTestCases))]
            public void Should_GetLoopProtectionReferencesStackCount_BeOne_BeforeAndAfterEnteringRootScope_When_RootModelReference_Exists(string id, Specification<TraversingTestCases.TestClassA> rootSpecification, TraversingTestCases.TestClassA model)
            {
                _ = id;

                var modelScheme = ModelSchemeFactory.Create(rootSpecification);

                var context = new IsValidValidationContext(modelScheme, new ReferenceLoopProtectionSettings(new object()));

                context.GetLoopProtectionReferencesStackCount().Should().Be(1);

                context.EnterScope(modelScheme.RootSpecificationScopeId, model);

                context.GetLoopProtectionReferencesStackCount().Should().Be(1);
            }

            [Theory]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Common), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Struct), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Collections), MemberType = typeof(TraversingTestCases))]
            [MemberData(nameof(TraversingTestCases.TreesExamples_Nullable), MemberType = typeof(TraversingTestCases))]
            public void Should_GetLoopProtectionReferencesStackCount_BeNull_BeforeAndAfterEnteringRootScope_When_RootModelReference_IsNull(string id, Specification<TraversingTestCases.TestClassA> rootSpecification, TraversingTestCases.TestClassA model)
            {
                _ = id;

                var modelScheme = ModelSchemeFactory.Create(rootSpecification);

                var context = new IsValidValidationContext(modelScheme, default);

                context.GetLoopProtectionReferencesStackCount().Should().BeNull();

                context.EnterScope(modelScheme.RootSpecificationScopeId, model);

                context.GetLoopProtectionReferencesStackCount().Should().BeNull();
            }
        }
    }
}
