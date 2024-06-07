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

    public class ValidationContextTests
    {
        public class Initializing
        {
            [Fact]
            public void Should_Initialize()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                _ = new ValidationContext(modelScheme, default, default);
            }

            [Fact]
            public void Should_Initialize_WithDefaultValues()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                var validationContext = new ValidationContext(modelScheme, default, default);

                validationContext.FailFast.Should().BeFalse();
                validationContext.ReferenceLoopProtectionSettings.Should().BeNull();
                validationContext.Errors.Should().BeNull();
                validationContext.ShouldFallBack.Should().BeFalse();
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public void Should_Initialize_WithValues(bool failFast, bool passLoopProtectionSettings)
            {
                var modelScheme = Substitute.For<IModelScheme>();

                ReferenceLoopProtectionSettings referenceLoopProtectionSettings = null;

                if (passLoopProtectionSettings)
                {
                    modelScheme.RootModelType.Returns(typeof(object));
                    referenceLoopProtectionSettings = new ReferenceLoopProtectionSettings();
                }

                var validationContext = new ValidationContext(modelScheme, failFast, referenceLoopProtectionSettings);

                validationContext.FailFast.Should().Be(failFast);

                if (passLoopProtectionSettings)
                {
                    validationContext.ReferenceLoopProtectionSettings.Should().BeSameAs(referenceLoopProtectionSettings);
                }
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public void Should_Initialize_ReferencesStack_With_Null_When_LoopProtectionSettings_Is_Null(bool failFast, bool rootModelTypeIsReference)
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.RootModelType.Returns(rootModelTypeIsReference ? typeof(object) : typeof(int));

                var validationContext = new ValidationContext(
                    modelScheme,
                    failFast,
                    null);

                validationContext.GetLoopProtectionReferencesStackCount().Should().BeNull();
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public void Should_Initialize_ReferencesStack_With_Zero_When_LoopProtectionSettings_Has_NullRootModel(bool failFast, bool rootModelTypeIsReference)
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.RootModelType.Returns(rootModelTypeIsReference ? typeof(object) : typeof(int));

                var validationContext = new ValidationContext(
                    modelScheme,
                    failFast,
                    new ReferenceLoopProtectionSettings());

                validationContext.GetLoopProtectionReferencesStackCount().Should().Be(0);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Initialize_ReferencesStack_With_One_When_LoopProtectionSettings_Has_RootModel_And_RootModelTypeInSchemeIsReferenceType(bool failFast)
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.RootModelType.Returns(typeof(object));

                var validationContext = new ValidationContext(
                    modelScheme,
                    failFast,
                    new ReferenceLoopProtectionSettings(new object()));

                validationContext.GetLoopProtectionReferencesStackCount().Should().Be(1);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Initialize_ReferencesStack_With_Zero_When_LoopProtectionSettings_Has_RootModel_And_RootModelTypeInSchemeIsValueType(bool failFast)
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.RootModelType.Returns(typeof(int));

                var validationContext = new ValidationContext(
                    modelScheme,
                    failFast,
                    new ReferenceLoopProtectionSettings(new object()));

                validationContext.GetLoopProtectionReferencesStackCount().Should().Be(0);
            }
        }

        public class AddError
        {
            [Fact]
            public void Should_AddError_ToDefaultPath()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var context = new ValidationContext(modelScheme, default, default);

                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors[string.Empty].Should().HaveCount(1);
                context.Errors[string.Empty].ElementAt(0).Should().Be(123);
            }

            [Fact]
            public void Should_AddErrors_ToDefaultPath()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var context = new ValidationContext(modelScheme, default, default);

                context.AddError(123);
                context.AddError(321);
                context.AddError(666);

                context.Errors.Should().HaveCount(1);
                context.Errors[string.Empty].Should().HaveCount(3);
                context.Errors[string.Empty].Should().ContainInOrder(123, 321, 666);
            }

            [Fact]
            public void Should_AddError_ToEnteredPath()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(123);
            }

            [Fact]
            public void Should_AddErrors_ToEnteredPath()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.AddError(123);
                context.AddError(321);
                context.AddError(666);

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(3);
                context.Errors["entered.path"].Should().ContainInOrder(123, 321, 666);
            }

            [Fact]
            public void Should_AddError_When_AlreadyExistsUnderSamePath_And_SkipIfDuplicateInPath_Is_False()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.AddError(123);
                context.AddError(123);
                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(3);
                context.Errors["entered.path"].Should().ContainInOrder(123, 123, 123);
            }

            [Fact]
            public void Should_NotAddError_When_AlreadyExistsUnderSamePath_And_SkipIfDuplicateInPath_Is_True()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.AddError(123, true);
                context.AddError(123, true);
                context.AddError(123, true);

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(123);
            }

            [Fact]
            public void Should_AddErrors_When_NotExistsUnderSamePath_And_SkipIfDuplicateInPath_Is_True()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.AddError(123, true);
                context.AddError(321, true);
                context.AddError(666, true);

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(3);
                context.Errors["entered.path"].Should().ContainInOrder(123, 321, 666);
            }

            [Fact]
            public void Should_AddError_OnlyWhen_NotExistsUnderSamePath_And_SkipIfDuplicateInPath_Is_True()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("test1")).Returns("test1");
                modelScheme.ResolvePath(Arg.Is("test1"), Arg.Is("test2")).Returns("test1.test2");
                modelScheme.ResolvePath(Arg.Is("test1.test2"), Arg.Is("test3")).Returns("test1.test2.test3");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("test1");

                context.AddError(123, true);
                context.AddError(123, true);
                context.AddError(123, false);

                context.EnterPath("test2");
                context.AddError(123, true);
                context.AddError(123, true);
                context.AddError(123, false);

                context.EnterPath("test3");
                context.AddError(123, true);
                context.AddError(123, false);
                context.AddError(123, false);

                context.Errors.Should().HaveCount(3);

                context.Errors["test1"].Should().HaveCount(2);
                context.Errors["test1"].Should().ContainInOrder(123, 123);

                context.Errors["test1.test2"].Should().HaveCount(2);
                context.Errors["test1.test2"].Should().ContainInOrder(123, 123);

                context.Errors["test1.test2.test3"].Should().HaveCount(3);
                context.Errors["test1.test2.test3"].Should().ContainInOrder(123, 123, 123);
            }
        }

        public class EnterPath_And_AddingErrors
        {
            [Theory]
            [InlineData("some.path")]
            [InlineData("")]
            [InlineData("#2.#123.path.#321")]
            public void AddErrors_Should_AddToEnteredPath_AfterStepIntoNextPath_ResolvedByModelScheme(string pathForScope)
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entry.path")).Returns(pathForScope);

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entry.path");

                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors[pathForScope].Should().HaveCount(1);
                context.Errors[pathForScope].Should().ContainInOrder(123);

                modelScheme.Received(1).ResolvePath(Arg.Is(""), Arg.Is("entry.path"));
            }

            [Fact]
            public void AddErrors_Should_AddToSamePath_AfterEnteredPathIsNull()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entry.path")).Returns("entry.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entry.path");
                context.EnterPath(null);

                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors["entry.path"].Should().HaveCount(1);
                context.Errors["entry.path"].Should().ContainInOrder(123);

                modelScheme.Received(1).ResolvePath(Arg.Is(""), Arg.Is("entry.path"));
                modelScheme.DidNotReceive().ResolvePath(Arg.Is(""), Arg.Is((string)null));
            }

            [Fact]
            public void AddErrors_Should_AddToEnteredPath_AfterStepIntoNextPath()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entry.path")).Returns("entry.path");
                modelScheme.ResolvePath(Arg.Is("entry.path"), Arg.Is("next")).Returns("entry.path.next");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entry.path");
                context.AddError(123);
                context.Errors.Should().HaveCount(1);
                context.Errors["entry.path"].Should().HaveCount(1);
                context.Errors["entry.path"].Should().ContainInOrder(123);

                context.EnterPath("next");
                context.AddError(321);
                context.Errors.Should().HaveCount(2);
                context.Errors["entry.path"].Should().HaveCount(1);
                context.Errors["entry.path"].Should().ContainInOrder(123);
                context.Errors["entry.path.next"].Should().HaveCount(1);
                context.Errors["entry.path.next"].Should().ContainInOrder(321);

                modelScheme.Received(1).ResolvePath(Arg.Is(""), Arg.Is("entry.path"));
                modelScheme.Received(1).ResolvePath(Arg.Is("entry.path"), Arg.Is("next"));
            }

            [Fact]
            public void AddErrors_Should_AddToPreviousPathAfterStepOut()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entry.path")).Returns("entry.path");
                modelScheme.ResolvePath(Arg.Is("entry.path"), Arg.Is("next")).Returns("entry.path.next");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entry.path");
                context.EnterPath("next");
                context.LeavePath();
                context.AddError(123);
                context.Errors.Should().HaveCount(1);
                context.Errors["entry.path"].Should().HaveCount(1);
                context.Errors["entry.path"].Should().ContainInOrder(123);

                modelScheme.Received(1).ResolvePath(Arg.Is(""), Arg.Is("entry.path"));
                modelScheme.Received(1).ResolvePath(Arg.Is("entry.path"), Arg.Is("next"));
            }

            [Fact]
            public void AddErrors_Should_AddToEnteredPath_And_ToPreviousPathAfterStepOut()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entry.path")).Returns("entry.path");
                modelScheme.ResolvePath(Arg.Is("entry.path"), Arg.Is("next")).Returns("entry.path.next");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entry.path");
                context.EnterPath("next");
                context.AddError(321);
                context.Errors.Should().HaveCount(1);
                context.Errors["entry.path.next"].Should().HaveCount(1);
                context.Errors["entry.path.next"].Should().ContainInOrder(321);

                context.LeavePath();

                context.AddError(123);
                context.Errors.Should().HaveCount(2);
                context.Errors["entry.path"].Should().HaveCount(1);
                context.Errors["entry.path"].Should().ContainInOrder(123);
                context.Errors["entry.path.next"].Should().HaveCount(1);
                context.Errors["entry.path.next"].Should().ContainInOrder(321);

                modelScheme.Received(1).ResolvePath(Arg.Is(""), Arg.Is("entry.path"));
                modelScheme.Received(1).ResolvePath(Arg.Is("entry.path"), Arg.Is("next"));
            }

            [Fact]
            public void Should_AddError_ToEnteredCollectionItemPath()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("#")).Returns($"#");
                modelScheme.GetPathWithIndexes(Arg.Is("#"), Arg.Is<IReadOnlyCollection<string>>(a => a.Single() == "666")).Returns("#666");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterCollectionItemPath(666);

                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors["#666"].Should().HaveCount(1);
                context.Errors["#666"].Should().ContainInOrder(123);

                modelScheme.Received(1).ResolvePath(Arg.Is(""), Arg.Is("#"));
                modelScheme.Received(1).GetPathWithIndexes(Arg.Is("#"), Arg.Is<IReadOnlyCollection<string>>(a => a.Single() == "666"));
            }

            [Fact]
            public void Should_AddError_ToEnteredCollectionItemPath_InSecondLevel()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("#")).Returns($"entered.path.#");
                modelScheme.GetPathWithIndexes(Arg.Is("entered.path.#"), Arg.Is<IReadOnlyCollection<string>>(a => a.Single() == "666")).Returns("entered.path.#666");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnterCollectionItemPath(666);

                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path.#666"].Should().HaveCount(1);
                context.Errors["entered.path.#666"].Should().ContainInOrder(123);

                modelScheme.Received(1).ResolvePath(Arg.Is(""), Arg.Is("entered.path"));
                modelScheme.Received(1).ResolvePath(Arg.Is("entered.path"), Arg.Is("#"));
                modelScheme.Received(1).GetPathWithIndexes(Arg.Is("entered.path.#"), Arg.Is<IReadOnlyCollection<string>>(a => a.Single() == "666"));
            }

            [Fact]
            public void Should_AddError_AddToPreviousPath_After_LeavingEnteredCollectionItemPath()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("#")).Returns($"entered.path.#666");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnterCollectionItemPath(666);
                context.LeavePath();

                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(123);

                modelScheme.Received(1).ResolvePath(Arg.Is(""), Arg.Is("entered.path"));
                modelScheme.Received(1).ResolvePath(Arg.Is("entered.path"), Arg.Is("#"));
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

                var context = new ValidationContext(modelScheme, default, default);

                var model = new TestClass();

                context.EnterScope(1234, model);

                Received.InOrder(() =>
                {
                    modelScheme.GetSpecificationScope<TestClass>(Arg.Is(1234));
                    specificationScope.Validate(Arg.Is(model), Arg.Is(context));
                });

                specificationScope.DidNotReceive().Discover(Arg.Any<IDiscoveryContext>());

                specificationScope.Received(1).Validate(Arg.Any<TestClass>(), Arg.Any<ValidationContext>());
            }

            [Fact]
            public void Should_UseModelScheme_MultipleTimes()
            {
                var specificationScope = Substitute.For<ISpecificationScope<TestClass>>();

                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.GetSpecificationScope<TestClass>(Arg.Is(1234)).Returns(specificationScope);

                var context = new ValidationContext(modelScheme, default, default);

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

                specificationScope.Received(2).Validate(Arg.Any<TestClass>(), Arg.Any<ValidationContext>());
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

                var context = new ValidationContext(modelScheme, default, default);

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

                var context = new ValidationContext(modelScheme, default, default);

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

                var modelScheme = ModelSchemeFactory.Create(specification);

                var context = new ValidationContext(modelScheme, default, new ReferenceLoopProtectionSettings());

                Action action = () => context.EnterScope(modelScheme.RootSpecificationScopeId, model);

                var exception = action.Should().ThrowExactly<ReferenceLoopException>().And;

                exception.Path.Should().Be(path);
                exception.NestedPath.Should().Be(infiniteLoopNestedPath);
                exception.Type.Should().Be(type);

                var pathStringified = string.IsNullOrEmpty(path)
                    ? "the root path, so the validated object itself,"
                    : $"the path '{path}'";

                exception.Message.Should().Be($"Reference loop detected: object of type {type.GetFriendlyName()} has been detected twice in the reference graph, effectively creating an infinite references loop (at first under {pathStringified} and then under the nested path '{infiniteLoopNestedPath}')");
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

                var context = new ValidationContext(modelScheme, default, new ReferenceLoopProtectionSettings());

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

                var context = new ValidationContext(modelScheme, default, new ReferenceLoopProtectionSettings(new object()));

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

                var context = new ValidationContext(modelScheme, default, default);

                context.GetLoopProtectionReferencesStackCount().Should().BeNull();

                context.EnterScope(modelScheme.RootSpecificationScopeId, model);

                context.GetLoopProtectionReferencesStackCount().Should().BeNull();
            }
        }

        public class ShouldFallback
        {
            [Fact]
            public void Should_BeFalse_If_FailFastIsFalse_And_ErrorDetectionModeIsOff()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var validationContext = new ValidationContext(modelScheme, default, default);

                validationContext.ShouldFallBack.Should().BeFalse();
            }

            [Fact]
            public void Should_BeFalse_If_FailFastIsTrue_And_NoErrorsAdded()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var validationContext = new ValidationContext(modelScheme, true, default);

                validationContext.ShouldFallBack.Should().BeFalse();
            }

            [Fact]
            public void Should_BeFalse_If_FailFastIsFalse_And_AnyErrorsAdded()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var validationContext = new ValidationContext(modelScheme, false, default);

                validationContext.AddError(123);

                validationContext.ShouldFallBack.Should().BeFalse();
            }

            [Fact]
            public void Should_BeTrue_If_FailFastIsTrue_And_AnyErrorsAdded()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var validationContext = new ValidationContext(modelScheme, true, default);

                validationContext.AddError(123);

                validationContext.ShouldFallBack.Should().BeTrue();
            }

            [Theory]
            [InlineData(true, ErrorMode.Append)]
            [InlineData(true, ErrorMode.Override)]
            [InlineData(false, ErrorMode.Append)]
            [InlineData(false, ErrorMode.Override)]
            public void Should_BeFalse_When_ErrorModeEnabled(bool failFast, object errorModeBoxed)
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var validationContext = new ValidationContext(modelScheme, failFast, default);

                validationContext.EnableErrorDetectionMode((ErrorMode)errorModeBoxed, 123);

                validationContext.ShouldFallBack.Should().BeFalse();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_BeTrue_If_OverridingModeEnabled_And_AnyErrorsAdded(bool failFast)
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var validationContext = new ValidationContext(modelScheme, failFast, default);

                validationContext.EnableErrorDetectionMode(ErrorMode.Override, 321);
                validationContext.AddError(123);

                validationContext.ShouldFallBack.Should().BeTrue();
            }

            [Fact]
            public void Should_BeTrue_When_FailFastIsFalse_OnlyAfter_OverridingModeEnabled()
            {
                var modelScheme = Substitute.For<IModelScheme>();

                var validationContext = new ValidationContext(modelScheme, default, default);

                validationContext.AddError(1);

                validationContext.ShouldFallBack.Should().BeFalse();

                validationContext.EnableErrorDetectionMode(ErrorMode.Override, 321);

                validationContext.AddError(2);

                validationContext.ShouldFallBack.Should().BeTrue();
            }
        }

        public class AppendErrorsMode
        {
            [Fact]
            public void Should_NotAppendErrors_When_AppendErrorModeDisabled_And_LeavingPathWithErrors()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.AddError(1);
                context.AddError(2);
                context.AddError(3);
                context.LeavePath();

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(3);
                context.Errors["entered.path"].Should().ContainInOrder(1, 2, 3);
            }

            [Fact]
            public void Should_NotAppendErrors_When_AppendErrorModeEnabled_And_LeavingPathWithoutErrors()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Append, 321);

                context.LeavePath();

                context.Errors.Should().BeNull();
            }

            [Fact]
            public void Should_NotAppendErrors_When_AppendErrorModeEnabled_And_LeavingPathWithoutErrors_AtManyLevels()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.EnableErrorDetectionMode(ErrorMode.Append, 1);

                context.EnterPath("entered.path.nested");

                context.EnableErrorDetectionMode(ErrorMode.Append, 2);

                context.EnterPath("entered.path.nested.more");

                context.EnableErrorDetectionMode(ErrorMode.Append, 3);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().BeNull();
            }

            [Fact]
            public void Should_NotAppendErrors_When_ModeEnabledAfterErrorIsAdded()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.AddError(1);
                context.EnableErrorDetectionMode(ErrorMode.Append, 321);

                context.LeavePath();

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(1);
            }

            [Fact]
            public void Should_AppendErrors_When_AppendErrorModeEnabled_And_LeavingPathWithError()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Append, 321);

                context.AddError(1);
                context.LeavePath();

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(2);
                context.Errors["entered.path"].Should().ContainInOrder(1, 321);
            }

            [Fact]
            public void Should_AppendErrors_When_AppendErrorModeEnabled_And_LeavingPathWithErrors()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Append, 321);

                context.AddError(1);
                context.AddError(2);
                context.AddError(3);
                context.LeavePath();

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(4);
                context.Errors["entered.path"].Should().ContainInOrder(1, 2, 3, 321);
            }

            [Fact]
            public void Should_AppendErrors_ToLevelsWithModeEnabled_When_ErrorsOnEveryLevel()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.AddError(1);

                context.EnterPath("entered.path.nested");
                context.EnableErrorDetectionMode(ErrorMode.Append, 321);
                context.AddError(2);

                context.LeavePath();

                context.LeavePath();

                context.Errors.Should().HaveCount(2);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(1);

                context.Errors["entered.path.nested"].Should().HaveCount(2);
                context.Errors["entered.path.nested"].Should().ContainInOrder(2, 321);
            }

            [Fact]
            public void Should_AppendErrors_ToLevelsWithModeEnabled_When_ModeEnabledAtLowLevel_And_ErrorIsNested()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.EnableErrorDetectionMode(ErrorMode.Append, 321);

                context.EnterPath("entered.path.nested");
                context.EnterPath("entered.path.nested.more");

                context.AddError(1);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(2);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(321);

                context.Errors["entered.path.nested.more"].Should().HaveCount(1);
                context.Errors["entered.path.nested.more"].Should().ContainInOrder(1);
            }

            [Fact]
            public void Should_AppendErrors_ToLevelsWithModeEnabled_When_ModeEnabledOnDifferentLevels_And_ErrorOnAllLevels()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Append, 321);
                context.AddError(1);

                context.EnterPath("entered.path.nested");
                context.AddError(2);

                context.EnterPath("entered.path.nested.more");
                context.EnableErrorDetectionMode(ErrorMode.Append, 123);
                context.AddError(3);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(3);

                context.Errors["entered.path"].Should().HaveCount(2);
                context.Errors["entered.path"].Should().ContainInOrder(1, 321);

                context.Errors["entered.path.nested"].Should().HaveCount(1);
                context.Errors["entered.path.nested"].Should().ContainInOrder(2);

                context.Errors["entered.path.nested.more"].Should().HaveCount(2);
                context.Errors["entered.path.nested.more"].Should().ContainInOrder(3, 123);
            }

            [Fact]
            public void Should_AppendErrors_ToLevelsWithModeEnabled_AndErrorsNested_When_ModeEnabledOnAllLevels_And_ErrorOnLowLevel()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Append, 321);
                context.AddError(1);

                context.EnterPath("entered.path.nested");
                context.EnableErrorDetectionMode(ErrorMode.Append, 661);

                context.EnterPath("entered.path.nested.more");
                context.EnableErrorDetectionMode(ErrorMode.Append, 662);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(1);

                context.Errors["entered.path"].Should().HaveCount(2);
                context.Errors["entered.path"].Should().ContainInOrder(1, 321);
            }

            [Fact]
            public void Should_AppendErrors_ToLevelsWithModeEnabled_AndErrorsNested_When_ModeEnabledOnAllLevels_And_ErrorOnNestedLevel()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Append, 111);

                context.EnterPath("entered.path.nested");
                context.EnableErrorDetectionMode(ErrorMode.Append, 112);

                context.EnterPath("entered.path.nested.more");
                context.EnableErrorDetectionMode(ErrorMode.Append, 113);
                context.AddError(1);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(3);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(111);

                context.Errors["entered.path.nested"].Should().HaveCount(1);
                context.Errors["entered.path.nested"].Should().ContainInOrder(112);

                context.Errors["entered.path.nested.more"].Should().HaveCount(2);
                context.Errors["entered.path.nested.more"].Should().ContainInOrder(1, 113);
            }

            [Fact]
            public void Should_AppendErrors_WhenTravelingUpAndDownTheTree_OnlyToEnabledLevelsBelowErrorDetectedAtTheTimeOfDetection_When_HavingNestedBranchWithEnabledLevelWithoutError()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more1")).Returns("entered.path.nested.more1");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more2")).Returns("entered.path.nested.more2");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Append, 111);

                context.EnterPath("entered.path.nested");
                context.EnterPath("entered.path.nested.more1");
                context.AddError(1);

                context.LeavePath();
                context.EnableErrorDetectionMode(ErrorMode.Append, 112);

                context.EnterPath("entered.path.nested.more2");

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(2);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(111);

                context.Errors["entered.path.nested.more1"].Should().HaveCount(1);
                context.Errors["entered.path.nested.more1"].Should().ContainInOrder(1);
            }

            [Fact]
            public void Should_AppendErrors_WhenTravelingUpAndDownTheTree_OnlyToEnabledLevelsBelowErrorDetectedAtTheTimeOfDetection_When_HavingNestedBranchWithEnabledLevelWithError()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more1")).Returns("entered.path.nested.more1");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more2")).Returns("entered.path.nested.more2");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Append, 111);

                context.EnterPath("entered.path.nested");
                context.EnterPath("entered.path.nested.more1");
                context.AddError(1);

                context.LeavePath();
                context.EnableErrorDetectionMode(ErrorMode.Append, 112);

                context.EnterPath("entered.path.nested.more2");
                context.AddError(2);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(4);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(111);

                context.Errors["entered.path.nested.more1"].Should().HaveCount(1);
                context.Errors["entered.path.nested.more1"].Should().ContainInOrder(1);

                context.Errors["entered.path.nested.more2"].Should().HaveCount(1);
                context.Errors["entered.path.nested.more2"].Should().ContainInOrder(2);

                context.Errors["entered.path.nested"].Should().HaveCount(1);
                context.Errors["entered.path.nested"].Should().ContainInOrder(112);
            }

            [Fact]
            public void Should_AppendErrors_WhenTravelingUpAndDownTheTree_OnlyToEnabledLevelsBelowErrorDetectedAtTheTimeOfDetection_When_HavingNestedBranchWithEnabledLevelWithError_OnlyHere()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more1")).Returns("entered.path.nested.more1");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more2")).Returns("entered.path.nested.more2");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Append, 111);

                context.EnterPath("entered.path.nested");
                context.EnterPath("entered.path.nested.more1");

                context.LeavePath();
                context.EnableErrorDetectionMode(ErrorMode.Append, 112);

                context.EnterPath("entered.path.nested.more2");
                context.AddError(2);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(3);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(111);

                context.Errors["entered.path.nested.more2"].Should().HaveCount(1);
                context.Errors["entered.path.nested.more2"].Should().ContainInOrder(2);

                context.Errors["entered.path.nested"].Should().HaveCount(1);
                context.Errors["entered.path.nested"].Should().ContainInOrder(112);
            }

            [Fact]
            public void Should_AppendErrorMode_SetDisabled_When_ErrorsAddedAndLevelLeft()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("first")).Returns("first");
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("second")).Returns("second");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("first");
                context.EnableErrorDetectionMode(ErrorMode.Append, 321);
                context.AddError(1);
                context.LeavePath();

                context.EnterPath("second");
                context.AddError(2);
                context.LeavePath();

                context.Errors.Should().HaveCount(2);

                context.Errors["first"].Should().HaveCount(2);
                context.Errors["first"].Should().ContainInOrder(1, 321);

                context.Errors["second"].Should().HaveCount(1);
                context.Errors["second"].Should().ContainInOrder(2);
            }

            [Fact]
            public void Should_AppendErrorMode_SetDisabledAndEnableAgain()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("first")).Returns("first");
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("second")).Returns("second");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("first");
                context.EnableErrorDetectionMode(ErrorMode.Append, 321);
                context.AddError(1);
                context.LeavePath();

                context.EnterPath("second");
                context.EnableErrorDetectionMode(ErrorMode.Append, 123);
                context.AddError(2);
                context.LeavePath();

                context.Errors.Should().HaveCount(2);

                context.Errors["first"].Should().HaveCount(2);
                context.Errors["first"].Should().ContainInOrder(1, 321);

                context.Errors["second"].Should().HaveCount(2);
                context.Errors["second"].Should().ContainInOrder(2, 123);
            }
        }

        public class OverrideErrorsMode
        {
            [Fact]
            public void Should_NotOverrideErrors_When_ModeDisabled_And_LeavingPathWithErrors()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.AddError(1);
                context.AddError(2);
                context.AddError(3);
                context.LeavePath();

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(3);
                context.Errors["entered.path"].Should().ContainInOrder(1, 2, 3);
            }

            [Fact]
            public void Should_NotOverrideErrors_When_ModeEnabled_And_LeavingPathWithoutErrors()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Override, 321);

                context.LeavePath();

                context.Errors.Should().BeNull();
            }

            [Fact]
            public void Should_NotOverrideErrors_When_ModeEnabled_And_LeavingPathWithoutErrors_AtManyLevels()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.EnableErrorDetectionMode(ErrorMode.Override, 1);

                context.EnterPath("entered.path.nested");

                context.EnableErrorDetectionMode(ErrorMode.Override, 2);

                context.EnterPath("entered.path.nested.more");

                context.EnableErrorDetectionMode(ErrorMode.Override, 3);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().BeNull();
            }

            [Fact]
            public void Should_NotOverrideErrors_When_ModeEnabledAfterErrorIsAdded()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.AddError(1);

                context.EnableErrorDetectionMode(ErrorMode.Override, 321);

                context.LeavePath();

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(1);
            }

            [Fact]
            public void Should_OverrideErrors_When_ModeEnabled_And_LeavingPathWithError()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Override, 321);

                context.AddError(1);
                context.LeavePath();

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(321);
            }

            [Fact]
            public void Should_OverrideErrors_When_ModeEnabled_And_LeavingPathWithErrors()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Override, 321);

                context.AddError(1);
                context.AddError(2);
                context.AddError(3);
                context.LeavePath();

                context.Errors.Should().HaveCount(1);
                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(321);
            }

            [Fact]
            public void Should_OverrideErrors_OnLevelsWithModeEnabled_When_ErrorsOnEveryLevel()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.AddError(1);

                context.EnterPath("entered.path.nested");
                context.EnableErrorDetectionMode(ErrorMode.Override, 321);
                context.AddError(2);

                context.LeavePath();

                context.LeavePath();

                context.Errors.Should().HaveCount(2);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(1);

                context.Errors["entered.path.nested"].Should().HaveCount(1);
                context.Errors["entered.path.nested"].Should().ContainInOrder(321);
            }

            [Fact]
            public void Should_OverrideErrors_OnLowestLevelWithModeEnabled_When_ModeEnabledAtLowLevel_And_ErrorIsNested()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");

                context.EnableErrorDetectionMode(ErrorMode.Override, 321);

                context.EnterPath("entered.path.nested");
                context.EnterPath("entered.path.nested.more");

                context.AddError(1);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(1);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(321);
            }

            [Fact]
            public void Should_OverrideErrors_OnLowestLevelWithModeEnabled_When_ModeEnabledOnDifferentLevels_And_ErrorOnAllLevels()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Override, 321);
                context.AddError(1);

                context.EnterPath("entered.path.nested");
                context.AddError(2);

                context.EnterPath("entered.path.nested.more");
                context.EnableErrorDetectionMode(ErrorMode.Override, 123);
                context.AddError(3);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(1);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(321);
            }

            [Fact]
            public void Should_OverrideErrors_OnLowestLevelWithModeEnabled_When_ModeEnabledOnAllLevels_And_ErrorOnLowLevel()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Override, 321);
                context.AddError(1);

                context.EnterPath("entered.path.nested");
                context.EnableErrorDetectionMode(ErrorMode.Override, 661);

                context.EnterPath("entered.path.nested.more");
                context.EnableErrorDetectionMode(ErrorMode.Override, 662);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(1);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(321);
            }

            [Fact]
            public void Should_OverrideErrors_OnLowestLevelWithModeEnabled_When_ModeEnabledOnAllLevels_And_ErrorOnNestedLevel()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Override, 111);

                context.EnterPath("entered.path.nested");
                context.EnableErrorDetectionMode(ErrorMode.Override, 112);

                context.EnterPath("entered.path.nested.more");
                context.EnableErrorDetectionMode(ErrorMode.Override, 113);
                context.AddError(1);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(1);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(111);
            }

            [Fact]
            public void Should_OverrideErrors_WhenTravelingUpAndDownTheTree_ToRootEnabledLevel_When_HavingNestedBranchWithEnabledLevelWithoutError()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more1")).Returns("entered.path.nested.more1");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more2")).Returns("entered.path.nested.more2");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Override, 111);

                context.EnterPath("entered.path.nested");
                context.EnterPath("entered.path.nested.more1");
                context.AddError(1);

                context.LeavePath();
                context.EnableErrorDetectionMode(ErrorMode.Append, 112);

                context.EnterPath("entered.path.nested.more2");

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(1);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(111);
            }

            [Fact]
            public void Should_OverrideErrors_WhenTravelingUpAndDownTheTree_ToRootEnabledLevel_When_HavingNestedBranchWithEnabledLevelWithError()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more1")).Returns("entered.path.nested.more1");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more2")).Returns("entered.path.nested.more2");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Override, 111);

                context.EnterPath("entered.path.nested");
                context.EnterPath("entered.path.nested.more1");
                context.AddError(1);

                context.LeavePath();
                context.EnableErrorDetectionMode(ErrorMode.Append, 112);

                context.EnterPath("entered.path.nested.more2");
                context.AddError(2);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(1);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(111);
            }

            [Fact]
            public void Should_OverrideErrors_WhenTravelingUpAndDownTheTree_ToRootEnabledLevel_When_HavingNestedBranchWithEnabledLevelWithError_OnlyHere()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more1")).Returns("entered.path.nested.more1");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more2")).Returns("entered.path.nested.more2");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Override, 111);

                context.EnterPath("entered.path.nested");
                context.EnterPath("entered.path.nested.more1");

                context.LeavePath();
                context.EnableErrorDetectionMode(ErrorMode.Append, 112);

                context.EnterPath("entered.path.nested.more2");
                context.AddError(2);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(1);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(111);
            }

            [Fact]
            public void Should_Mode_SetDisabled_When_ErrorsAddedAndLevelLeft()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("first")).Returns("first");
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("second")).Returns("second");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("first");
                context.EnableErrorDetectionMode(ErrorMode.Override, 321);
                context.AddError(1);
                context.LeavePath();

                context.EnterPath("second");
                context.AddError(2);
                context.LeavePath();

                context.Errors.Should().HaveCount(2);

                context.Errors["first"].Should().HaveCount(1);
                context.Errors["first"].Should().ContainInOrder(321);

                context.Errors["second"].Should().HaveCount(1);
                context.Errors["second"].Should().ContainInOrder(2);
            }

            [Fact]
            public void Should_Mode_SetDisabledAndEnableAgain()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("first")).Returns("first");
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("second")).Returns("second");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("first");
                context.EnableErrorDetectionMode(ErrorMode.Override, 321);
                context.AddError(1);
                context.LeavePath();

                context.EnterPath("second");
                context.EnableErrorDetectionMode(ErrorMode.Override, 123);
                context.AddError(2);
                context.LeavePath();

                context.Errors.Should().HaveCount(2);

                context.Errors["first"].Should().HaveCount(1);
                context.Errors["first"].Should().ContainInOrder(321);

                context.Errors["second"].Should().HaveCount(1);
                context.Errors["second"].Should().ContainInOrder(123);
            }

            [Fact]
            public void Should_Mode_NotBeAffectedByOtherModes()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Override, 321);
                context.AddError(1);

                context.EnterPath("entered.path.nested");
                context.EnableErrorDetectionMode(ErrorMode.Append, 661);
                context.AddError(2);

                context.EnterPath("entered.path.nested.more");
                context.EnableErrorDetectionMode(ErrorMode.Override, 662);
                context.AddError(3);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(1);

                context.Errors["entered.path"].Should().HaveCount(1);
                context.Errors["entered.path"].Should().ContainInOrder(321);
            }

            [Fact]
            public void Should_Mode_WorkWithinAppendMode()
            {
                var modelScheme = Substitute.For<IModelScheme>();
                modelScheme.ResolvePath(Arg.Is(""), Arg.Is("entered.path")).Returns("entered.path");
                modelScheme.ResolvePath(Arg.Is("entered.path"), Arg.Is("entered.path.nested")).Returns("entered.path.nested");
                modelScheme.ResolvePath(Arg.Is("entered.path.nested"), Arg.Is("entered.path.nested.more")).Returns("entered.path.nested.more");

                var context = new ValidationContext(modelScheme, default, default);

                context.EnterPath("entered.path");
                context.EnableErrorDetectionMode(ErrorMode.Append, 321);
                context.AddError(1);

                context.EnterPath("entered.path.nested");
                context.EnableErrorDetectionMode(ErrorMode.Override, 661);
                context.AddError(2);

                context.EnterPath("entered.path.nested.more");
                context.EnableErrorDetectionMode(ErrorMode.Override, 662);
                context.AddError(3);

                context.LeavePath();
                context.LeavePath();
                context.LeavePath();

                context.Errors.Should().HaveCount(2);

                context.Errors["entered.path"].Should().HaveCount(2);
                context.Errors["entered.path"].Should().ContainInOrder(1, 321);

                context.Errors["entered.path.nested"].Should().HaveCount(1);
                context.Errors["entered.path.nested"].Should().ContainInOrder(661);
            }
        }
    }
}
