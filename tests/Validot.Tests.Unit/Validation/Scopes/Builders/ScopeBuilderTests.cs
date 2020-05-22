namespace Validot.Tests.Unit.Validation.Scopes.Builders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Errors.Args;
    using Validot.Specification;
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    using Xunit;

    using Arg = Validot.Arg;

    public class ScopeBuilderTests
    {
        public class TestClass
        {
        }

        public static IEnumerable<object[]> AllErrorSetupAndContentCases<T>()
        {
            foreach (var contentCase in ErrorContentApiHelper.AllCases<T>())
            {
                foreach (var setupCase in ErrorSetupApiHelper.AllCases<T>())
                {
                    yield return new object[]
                    {
                        $"{contentCase[0]}_{setupCase[0]}",
                        contentCase[1],
                        contentCase[2],
                        setupCase[1],
                        setupCase[2],
                    };
                }
            }
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new ScopeBuilder();
        }

        [Fact]
        public void Should_ThrowException_When_NullSpecification()
        {
            var builder = new ScopeBuilder();

            var context = Substitute.For<IScopeBuilderContext>();

            Action action = () => builder.Build<TestClass>(null, context);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_NullContext()
        {
            var builder = new ScopeBuilder();

            Specification<TestClass> specification = m => m;

            Action action = () => builder.Build(specification, null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_BeEmpty_When_NoCommand()
        {
            var context = new ScopeBuilderContext();

            var builder = new ScopeBuilder();

            Specification<TestClass> specification = m => m;

            var scope = builder.Build(specification, context);

            scope.CommandScopes.Should().BeEmpty();
        }

        public class BuildRule
        {
            public static IEnumerable<object[]> AllSetupCommandCases()
            {
                return ErrorSetupApiHelper.AllCases<TestClass>();
            }

            public static IEnumerable<object[]> AllErrorSetupAndContentCases()
            {
                return AllErrorSetupAndContentCases<TestClass>();
            }

            [Theory]
            [MemberData(nameof(AllSetupCommandCases))]
            public void Should_Build_RuleCommandScope_When_Rule(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestClass> expectedErrorSetup)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Predicate<TestClass> predicate = x => true;

                Specification<TestClass> specification = m => appendSetupCommands(m.Rule(predicate));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<RuleCommandScope<TestClass>>();

                var ruleCommandScope = (RuleCommandScope<TestClass>)scope.CommandScopes[0];

                ruleCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                ruleCommandScope.ExecutionCondition.Should().Be(expectedErrorSetup.ShouldExecute);
                ruleCommandScope.ErrorMode.Should().Be(ErrorMode.Append);
                ruleCommandScope.IsValid.Should().BeSameAs(predicate);

                ruleCommandScope.ErrorId.Should().Be(context.DefaultErrorId);
            }

            [Theory]
            [MemberData(nameof(AllErrorSetupAndContentCases))]
            public void Should_Build_RuleCommandScope_When_Rule_WithCustomError(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendContentCommands, ErrorContentApiHelper.ExpectedErrorContent expectedErrorContent, Func<dynamic, ISpecificationOut<TestClass>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestClass> expectedErrorSetup)
            {
                testId.Should().NotBeEmpty();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Predicate<TestClass> predicate = x => true;

                Specification<TestClass> specification = m => appendContentCommands(appendSetupCommands(m.Rule(predicate)));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<RuleCommandScope<TestClass>>();

                var ruleCommandScope = (RuleCommandScope<TestClass>)scope.CommandScopes[0];

                ruleCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                ruleCommandScope.ExecutionCondition.Should().BeSameAs(expectedErrorSetup.ShouldExecute);
                ruleCommandScope.ErrorMode.Should().Be(expectedErrorContent.Mode);
                ruleCommandScope.IsValid.Should().BeSameAs(predicate);

                ruleCommandScope.ErrorId.Should().BeGreaterOrEqualTo(0);

                if (expectedErrorContent.ShouldBeEmpty(0))
                {
                    ruleCommandScope.ErrorId.Should().Be(context.DefaultErrorId);
                }
                else
                {
                    ruleCommandScope.ErrorId.Should().NotBe(context.DefaultErrorId);
                    context.Errors.Keys.Should().Contain(ruleCommandScope.ErrorId);
                    expectedErrorContent.Match(context.Errors[ruleCommandScope.ErrorId], initialMessagesAmount: 0);
                }
            }
        }

        public class BuildRuleTemplate
        {
            public static IEnumerable<object[]> AllSetupCommandCases()
            {
                return ErrorSetupApiHelper.AllCases<TestClass>();
            }

            public static IEnumerable<object[]> AllErrorSetupAndContentCases()
            {
                return AllErrorSetupAndContentCases<TestClass>();
            }

            [Theory]
            [MemberData(nameof(AllSetupCommandCases))]
            public void Should_Build_RuleCommandScope_When_RuleTemplate_WithArgs(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestClass> expectedErrorSetup)
            {
                _ = testId;

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Predicate<TestClass> predicate = x => true;

                var args = new IArg[]
                {
                    Arg.Text("argName1", "argValue"),
                    Arg.Number("argName2", 2),
                };

                Specification<TestClass> specification = m => appendSetupCommands(m.RuleTemplate(predicate, "ruleKey", args));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<RuleCommandScope<TestClass>>();

                var ruleCommandScope = (RuleCommandScope<TestClass>)scope.CommandScopes[0];

                ruleCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                ruleCommandScope.ExecutionCondition.Should().Be(expectedErrorSetup.ShouldExecute);
                ruleCommandScope.ErrorMode.Should().Be(ErrorMode.Append);
                ruleCommandScope.IsValid.Should().BeSameAs(predicate);

                ruleCommandScope.ErrorId.Should().BeGreaterOrEqualTo(0);
                ruleCommandScope.ErrorId.Should().NotBe(context.DefaultErrorId);

                context.Errors.Keys.Should().Contain(ruleCommandScope.ErrorId);

                var error = context.Errors[ruleCommandScope.ErrorId];

                error.Messages.Count.Should().Be(1);
                error.Messages[0].Should().Be("ruleKey");
                error.Args.Should().BeSameAs(args);
                error.Codes.Should().BeEmpty();
            }

            [Theory]
            [MemberData(nameof(AllErrorSetupAndContentCases))]
            public void Should_Build_RuleCommandScope_When_RuleTemplate_WithArgs_AndCustomError(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendContentCommands, ErrorContentApiHelper.ExpectedErrorContent expectedErrorContent, Func<dynamic, ISpecificationOut<TestClass>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestClass> expectedErrorSetup)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Predicate<TestClass> predicate = x => true;

                var args = new IArg[]
                {
                    Arg.Text("argName1", "argValue"),
                    Arg.Number("argName2", 2),
                };

                Specification<TestClass> specification = m => appendSetupCommands(appendContentCommands(m.RuleTemplate(predicate, "ruleKey", args)));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<RuleCommandScope<TestClass>>();

                var ruleCommandScope = (RuleCommandScope<TestClass>)scope.CommandScopes[0];

                ruleCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                ruleCommandScope.ExecutionCondition.Should().Be(expectedErrorSetup.ShouldExecute);
                ruleCommandScope.ErrorMode.Should().Be(expectedErrorContent.Mode);
                ruleCommandScope.IsValid.Should().BeSameAs(predicate);

                ruleCommandScope.ErrorId.Should().BeGreaterOrEqualTo(0);

                if (expectedErrorContent.ShouldBeEmpty())
                {
                    ruleCommandScope.ErrorId.Should().Be(context.DefaultErrorId);
                }
                else
                {
                    ruleCommandScope.ErrorId.Should().NotBe(context.DefaultErrorId);

                    context.Errors.Keys.Should().Contain(ruleCommandScope.ErrorId);
                    var error = context.Errors[ruleCommandScope.ErrorId];

                    expectedErrorContent.Match(error);

                    error.Args.Should().BeSameAs(args);
                }
            }

            [Theory]
            [MemberData(nameof(AllSetupCommandCases))]
            public void Should_Build_RuleCommandScope_When_RuleTemplate_WithoutArgs(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestClass> expectedErrorSetup)
            {
                _ = testId;

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Predicate<TestClass> predicate = x => true;

                Specification<TestClass> specification = m => appendSetupCommands(m.RuleTemplate(predicate, "ruleKey"));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<RuleCommandScope<TestClass>>();

                var ruleCommandScope = (RuleCommandScope<TestClass>)scope.CommandScopes[0];

                ruleCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                ruleCommandScope.ExecutionCondition.Should().Be(expectedErrorSetup.ShouldExecute);
                ruleCommandScope.ErrorMode.Should().Be(ErrorMode.Append);
                ruleCommandScope.IsValid.Should().BeSameAs(predicate);

                ruleCommandScope.ErrorId.Should().BeGreaterOrEqualTo(0);
                ruleCommandScope.ErrorId.Should().NotBe(context.DefaultErrorId);

                context.Errors.Keys.Should().Contain(ruleCommandScope.ErrorId);

                var error = context.Errors[ruleCommandScope.ErrorId];

                error.Messages.Count.Should().Be(1);
                error.Messages[0].Should().Be("ruleKey");
                error.Args.Should().BeEmpty();
                error.Codes.Should().BeEmpty();
            }

            [Theory]
            [MemberData(nameof(AllErrorSetupAndContentCases))]
            public void Should_Build_RuleCommandScope_When_RuleTemplate_WithoutArgs_And_CustomError(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendContentCommands, ErrorContentApiHelper.ExpectedErrorContent expectedErrorContent, Func<dynamic, ISpecificationOut<TestClass>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestClass> expectedErrorSetup)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Predicate<TestClass> predicate = x => true;

                Specification<TestClass> specification = m => appendSetupCommands(appendContentCommands(m.RuleTemplate(predicate, "ruleKey")));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<RuleCommandScope<TestClass>>();

                var ruleCommandScope = (RuleCommandScope<TestClass>)scope.CommandScopes[0];

                ruleCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                ruleCommandScope.ExecutionCondition.Should().Be(expectedErrorSetup.ShouldExecute);
                ruleCommandScope.ErrorMode.Should().Be(expectedErrorContent.Mode);
                ruleCommandScope.IsValid.Should().BeSameAs(predicate);

                ruleCommandScope.ErrorId.Should().BeGreaterOrEqualTo(0);

                if (expectedErrorContent.ShouldBeEmpty())
                {
                    ruleCommandScope.ErrorId.Should().Be(context.DefaultErrorId);
                }
                else
                {
                    ruleCommandScope.ErrorId.Should().NotBe(context.DefaultErrorId);

                    context.Errors.Keys.Should().Contain(ruleCommandScope.ErrorId);
                    var error = context.Errors[ruleCommandScope.ErrorId];

                    expectedErrorContent.Match(error);

                    error.Args.Should().BeEmpty();
                }
            }
        }

        public class BuildAsModel
        {
            public static IEnumerable<object[]> AllSetupCommandCases()
            {
                return ErrorSetupApiHelper.AllCases<TestClass>();
            }

            public static IEnumerable<object[]> AllErrorSetupAndContentCases()
            {
                return AllErrorSetupAndContentCases<TestClass>();
            }

            [Theory]
            [MemberData(nameof(AllSetupCommandCases))]
            public void Should_Build_AsModelScope_When_AsModel(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestClass> expectedErrorSetup)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> innerSpecification = m => m;

                Specification<TestClass> specification = m => appendSetupCommands(m.AsModel(innerSpecification));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<ModelCommandScope<TestClass>>();

                var modelCommandScope = (ModelCommandScope<TestClass>)scope.CommandScopes[0];

                modelCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                modelCommandScope.ErrorMode.Should().Be(ErrorMode.Append);
                modelCommandScope.ErrorId.Should().BeNull();
                modelCommandScope.ExecutionCondition.Should().BeSameAs(expectedErrorSetup.ShouldExecute);

                context.Scopes.Keys.Should().Contain(modelCommandScope.ScopeId);

                context.Scopes[modelCommandScope.ScopeId].Should().BeOfType<SpecificationScope<TestClass>>();
            }

            [Theory]
            [MemberData(nameof(AllErrorSetupAndContentCases))]
            public void Should_Build_AsModelScope_When_AsModel_WithCustomError(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendContentCommands, ErrorContentApiHelper.ExpectedErrorContent expectedErrorContent, Func<dynamic, ISpecificationOut<TestClass>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestClass> expectedErrorSetup)
            {
                _ = testId;

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> innerSpecification = m => m;

                Specification<TestClass> specification = m => appendContentCommands(appendSetupCommands(m.AsModel(innerSpecification)));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<ModelCommandScope<TestClass>>();

                var modelCommandScope = (ModelCommandScope<TestClass>)scope.CommandScopes[0];

                modelCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                modelCommandScope.ExecutionCondition.Should().BeSameAs(expectedErrorSetup.ShouldExecute);
                modelCommandScope.ErrorMode.Should().Be(expectedErrorContent.Mode);

                modelCommandScope.ErrorId.Should().HaveValue();
                context.Errors.Keys.Should().Contain(modelCommandScope.ErrorId.Value);

                if (expectedErrorContent.ShouldBeEmpty(0))
                {
                    modelCommandScope.ErrorId.Should().Be(context.DefaultErrorId);
                }
                else
                {
                    modelCommandScope.ErrorId.Should().NotBe(context.DefaultErrorId);
                    expectedErrorContent.Match(context.Errors[modelCommandScope.ErrorId.Value], 0);
                }

                context.Scopes.Keys.Should().Contain(modelCommandScope.ScopeId);

                context.Scopes[modelCommandScope.ScopeId].Should().BeOfType<SpecificationScope<TestClass>>();
            }
        }

        public class BuildAsNullable
        {
            public static IEnumerable<object[]> AllSetupCommandCases()
            {
                return ErrorSetupApiHelper.AllCases<int?>();
            }

            public static IEnumerable<object[]> AllErrorSetupAndContentCases()
            {
                return AllErrorSetupAndContentCases<int?>();
            }

            [Theory]
            [MemberData(nameof(AllSetupCommandCases))]
            public void Should_Build_AsNullableScope_When_AsNullable(string testId, Func<dynamic, ISpecificationOut<int?>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<int?> expectedErrorSetup)
            {
                _ = testId;

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<int> innerSpecification = m => m;

                Specification<int?> specification = m => appendSetupCommands(m.AsNullable(innerSpecification));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<NullableCommandScope<int>>();

                var nullableCommandScope = (NullableCommandScope<int>)scope.CommandScopes[0];

                nullableCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                nullableCommandScope.ErrorMode.Should().Be(ErrorMode.Append);
                nullableCommandScope.ErrorId.Should().BeNull();
                nullableCommandScope.ExecutionCondition.Should().BeSameAs(expectedErrorSetup.ShouldExecute);

                context.Scopes.Keys.Should().Contain(nullableCommandScope.ScopeId);

                context.Scopes[nullableCommandScope.ScopeId].Should().BeOfType<SpecificationScope<int>>();
            }

            [Theory]
            [MemberData(nameof(AllErrorSetupAndContentCases))]
            public void Should_Build_AsNullableScope_When_AsNullable_WithCustomError(string testId, Func<dynamic, ISpecificationOut<int?>> appendContentCommands, ErrorContentApiHelper.ExpectedErrorContent expectedErrorContent, Func<dynamic, ISpecificationOut<int?>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<int?> expectedErrorSetup)
            {
                _ = testId;

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<int> innerSpecification = m => m;

                Specification<int?> specification = m => appendContentCommands(appendSetupCommands(m.AsNullable(innerSpecification)));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<NullableCommandScope<int>>();

                var nullableCommandScope = (NullableCommandScope<int>)scope.CommandScopes[0];

                nullableCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                nullableCommandScope.ExecutionCondition.Should().BeSameAs(expectedErrorSetup.ShouldExecute);
                nullableCommandScope.ErrorMode.Should().Be(expectedErrorContent.Mode);

                nullableCommandScope.ErrorId.Should().HaveValue();
                context.Errors.Keys.Should().Contain(nullableCommandScope.ErrorId.Value);

                if (expectedErrorContent.ShouldBeEmpty(0))
                {
                    nullableCommandScope.ErrorId.Should().Be(context.DefaultErrorId);
                }
                else
                {
                    nullableCommandScope.ErrorId.Should().NotBe(context.DefaultErrorId);
                    expectedErrorContent.Match(context.Errors[nullableCommandScope.ErrorId.Value], 0);
                }

                context.Scopes.Keys.Should().Contain(nullableCommandScope.ScopeId);

                context.Scopes[nullableCommandScope.ScopeId].Should().BeOfType<SpecificationScope<int>>();
            }
        }

        public class BuildAsCollection
        {
            public class TestCollection : IEnumerable<TestClass>
            {
                private List<TestClass> _list = new List<TestClass>();

                public IEnumerator<TestClass> GetEnumerator()
                {
                    return _list.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }

            public static IEnumerable<object[]> AllSetupCommandCases()
            {
                return ErrorSetupApiHelper.AllCases<TestCollection>();
            }

            public static IEnumerable<object[]> AllErrorSetupAndContentCases()
            {
                return AllErrorSetupAndContentCases<TestCollection>();
            }

            [Theory]
            [MemberData(nameof(AllSetupCommandCases))]
            public void Should_Build_AsCollectionScope_When_AsCollection(string testId, Func<dynamic, ISpecificationOut<TestCollection>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestCollection> expectedErrorSetup)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> innerSpecification = m => m;

                Specification<TestCollection> specification = m => appendSetupCommands(m.AsCollection(innerSpecification));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<CollectionCommandScope<TestCollection, TestClass>>();

                var collectionCommandScope = (CollectionCommandScope<TestCollection, TestClass>)scope.CommandScopes[0];

                collectionCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                collectionCommandScope.ErrorMode.Should().Be(ErrorMode.Append);
                collectionCommandScope.ErrorId.Should().BeNull();
                collectionCommandScope.ExecutionCondition.Should().BeSameAs(expectedErrorSetup.ShouldExecute);

                context.Scopes.Keys.Should().Contain(collectionCommandScope.ScopeId);

                context.Scopes[collectionCommandScope.ScopeId].Should().BeOfType<SpecificationScope<TestClass>>();
            }

            [Theory]
            [MemberData(nameof(AllErrorSetupAndContentCases))]
            public void Should_Build_AsCollectionScope_When_AsCollection_WithCustomError(string testId, Func<dynamic, ISpecificationOut<TestCollection>> appendContentCommands, ErrorContentApiHelper.ExpectedErrorContent expectedErrorContent, Func<dynamic, ISpecificationOut<TestCollection>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestCollection> expectedErrorSetup)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> innerSpecification = m => m;

                Specification<TestCollection> specification = m => appendContentCommands(appendSetupCommands(m.AsCollection(innerSpecification)));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<CollectionCommandScope<TestCollection, TestClass>>();

                var collectionCommandScope = (CollectionCommandScope<TestCollection, TestClass>)scope.CommandScopes[0];

                collectionCommandScope.Name.Should().Be(expectedErrorSetup.Name);
                collectionCommandScope.ExecutionCondition.Should().BeSameAs(expectedErrorSetup.ShouldExecute);
                collectionCommandScope.ErrorMode.Should().Be(expectedErrorContent.Mode);

                collectionCommandScope.ErrorId.Should().HaveValue();
                context.Errors.Keys.Should().Contain(collectionCommandScope.ErrorId.Value);

                if (expectedErrorContent.ShouldBeEmpty(0))
                {
                    collectionCommandScope.ErrorId.Should().Be(context.DefaultErrorId);
                }
                else
                {
                    collectionCommandScope.ErrorId.Should().NotBe(context.DefaultErrorId);
                    expectedErrorContent.Match(context.Errors[collectionCommandScope.ErrorId.Value], 0);
                }

                context.Scopes.Keys.Should().Contain(collectionCommandScope.ScopeId);

                context.Scopes[collectionCommandScope.ScopeId].Should().BeOfType<SpecificationScope<TestClass>>();
            }
        }

        public class BuildMember
        {
            public class TestParent
            {
                public TestClass TestMember { get; set; }
            }

            public static IEnumerable<object[]> AllSetupCommandCases()
            {
                return ErrorSetupApiHelper.AllCases<TestParent>();
            }

            public static IEnumerable<object[]> AllErrorSetupAndContentCases()
            {
                return AllErrorSetupAndContentCases<TestParent>();
            }

            [Theory]
            [MemberData(nameof(AllSetupCommandCases))]
            public void Should_Build_AsMemberScope_When_AsMemberScope(string testId, Func<dynamic, ISpecificationOut<TestParent>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestParent> expectedErrorSetup)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> innerSpecification = m => m;

                Specification<TestParent> specification = m => appendSetupCommands(m.Member(m1 => m1.TestMember, innerSpecification));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<MemberCommandScope<TestParent, TestClass>>();

                var memberCommandScope = (MemberCommandScope<TestParent, TestClass>)scope.CommandScopes[0];

                memberCommandScope.Name.Should().Be(expectedErrorSetup.Name ?? "TestMember");
                memberCommandScope.ErrorMode.Should().Be(ErrorMode.Append);
                memberCommandScope.ErrorId.Should().BeNull();
                memberCommandScope.ExecutionCondition.Should().BeSameAs(expectedErrorSetup.ShouldExecute);

                context.Scopes.Keys.Should().Contain(memberCommandScope.ScopeId);

                context.Scopes[memberCommandScope.ScopeId].Should().BeOfType<SpecificationScope<TestClass>>();
            }

            [Theory]
            [MemberData(nameof(AllErrorSetupAndContentCases))]
            public void Should_Build_AsCollectionScope_When_AsCollection_WithCustomError(string testId, Func<dynamic, ISpecificationOut<TestParent>> appendContentCommands, ErrorContentApiHelper.ExpectedErrorContent expectedErrorContent, Func<dynamic, ISpecificationOut<TestParent>> appendSetupCommands, ErrorSetupApiHelper.ExpectedErrorSetup<TestParent> expectedErrorSetup)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> innerSpecification = m => m;

                Specification<TestParent> specification = m => appendContentCommands(appendSetupCommands(m.Member(m1 => m1.TestMember, innerSpecification)));

                var scope = builder.Build(specification, context);

                scope.CommandScopes.Should().NotBeEmpty();
                scope.CommandScopes.Count.Should().Be(1);
                scope.CommandScopes[0].Should().BeOfType<MemberCommandScope<TestParent, TestClass>>();

                var memberCommandScope = (MemberCommandScope<TestParent, TestClass>)scope.CommandScopes[0];

                memberCommandScope.Name.Should().Be(expectedErrorSetup.Name ?? "TestMember");
                memberCommandScope.ExecutionCondition.Should().BeSameAs(expectedErrorSetup.ShouldExecute);
                memberCommandScope.ErrorMode.Should().Be(expectedErrorContent.Mode);

                var testParent = new TestParent()
                {
                    TestMember = new TestClass()
                };

                memberCommandScope.GetMemberValue(testParent).Should().BeSameAs(testParent.TestMember);

                memberCommandScope.ErrorId.Should().HaveValue();
                context.Errors.Keys.Should().Contain(memberCommandScope.ErrorId.Value);

                if (expectedErrorContent.ShouldBeEmpty(0))
                {
                    memberCommandScope.ErrorId.Should().Be(context.DefaultErrorId);
                }
                else
                {
                    memberCommandScope.ErrorId.Should().NotBe(context.DefaultErrorId);
                    expectedErrorContent.Match(context.Errors[memberCommandScope.ErrorId.Value], 0);
                }

                context.Scopes.Keys.Should().Contain(memberCommandScope.ScopeId);

                context.Scopes[memberCommandScope.ScopeId].Should().BeOfType<SpecificationScope<TestClass>>();
            }
        }

        public class PresenceDetection
        {
            public static IEnumerable<object[]> AllCommandCases()
            {
                return ErrorContentApiHelper.AllCases<TestClass>();
            }

            [Fact]
            public void Should_Return_Required_When_NoCommand()
            {
                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => m;

                var scope = builder.Build(specification, context);

                scope.Presence.Should().Be(Presence.Required);
                scope.RequiredErrorId.Should().Be(context.RequiredErrorId);
            }

            [Fact]
            public void Should_Return_Required_When_NoPresenceCommand()
            {
                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => m
                    .Rule(m1 => true);

                var scope = builder.Build(specification, context);

                scope.Presence.Should().Be(Presence.Required);
                scope.RequiredErrorId.Should().Be(context.RequiredErrorId);
            }

            [Fact]
            public void Should_Return_Required_When_NotNull()
            {
                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => m
                    .NotNull();

                var scope = builder.Build(specification, context);

                scope.Presence.Should().Be(Presence.Required);
                scope.RequiredErrorId.Should().Be(context.RequiredErrorId);
            }

            [Fact]
            public void Should_Return_Required_When_NotNull_And_Rules()
            {
                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => m
                    .NotNull()
                    .Rule(m1 => false);

                var scope = builder.Build(specification, context);

                scope.Presence.Should().Be(Presence.Required);
                scope.RequiredErrorId.Should().Be(context.RequiredErrorId);
            }

            [Fact]
            public void Should_Return_Required_When_Required()
            {
                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => m
                    .Required();

                var scope = builder.Build(specification, context);

                scope.Presence.Should().Be(Presence.Required);
                scope.RequiredErrorId.Should().Be(context.RequiredErrorId);
            }

            [Fact]
            public void Should_Return_Required_When_Required_And_Rules()
            {
                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => m
                    .Required()
                    .Rule(m1 => false);

                var scope = builder.Build(specification, context);

                scope.Presence.Should().Be(Presence.Required);
                scope.RequiredErrorId.Should().Be(context.RequiredErrorId);
            }

            [Theory]
            [MemberData(nameof(AllCommandCases))]
            public void Should_Return_Required_WithCustomError(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendErrorCommands, ErrorContentApiHelper.ExpectedErrorContent expected)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => appendErrorCommands(m.Required());

                var scope = builder.Build(specification, context);

                if (expected.ShouldBeEmpty(0))
                {
                    scope.RequiredErrorId.Should().Be(context.RequiredErrorId);
                }
                else
                {
                    scope.RequiredErrorId.Should().NotBe(context.RequiredErrorId);
                    context.Errors.Keys.Should().Contain(scope.RequiredErrorId);

                    expected.Match(context.Errors[scope.RequiredErrorId]);
                }
            }

            [Theory]
            [MemberData(nameof(AllCommandCases))]
            public void Should_Return_Required_WithCustomError_And_Rules(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendErrorCommands, ErrorContentApiHelper.ExpectedErrorContent expected)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => AppendRule(appendErrorCommands(m.Required()));

                var scope = builder.Build(specification, context);

                if (expected.ShouldBeEmpty(0))
                {
                    scope.RequiredErrorId.Should().Be(context.RequiredErrorId);
                }
                else
                {
                    scope.RequiredErrorId.Should().NotBe(context.RequiredErrorId);
                    context.Errors.Keys.Should().Contain(scope.RequiredErrorId);

                    expected.Match(context.Errors[scope.RequiredErrorId]);
                }

                dynamic AppendRule(dynamic api)
                {
                    if (api is IRuleIn<TestClass> ruleIn)
                    {
                        return RuleExtension.Rule(ruleIn, m => false);
                    }

                    throw new InvalidOperationException("Dynamic api tests failed");
                }
            }

            [Fact]
            public void Should_Return_Optional_When_Optional()
            {
                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => m
                    .Optional();

                var scope = builder.Build(specification, context);

                scope.Presence.Should().Be(Presence.Optional);
            }

            [Fact]
            public void Should_Return_Optional_When_Optional_And_Rules()
            {
                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => m
                    .Optional()
                    .Rule(m1 => false);

                var scope = builder.Build(specification, context);

                scope.Presence.Should().Be(Presence.Optional);
            }

            [Fact]
            public void Should_Return_Forbidden_When_Forbidden()
            {
                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => m
                    .Forbidden();

                var scope = builder.Build(specification, context);

                scope.Presence.Should().Be(Presence.Forbidden);
                scope.ForbiddenErrorId.Should().Be(context.ForbiddenErrorId);
            }

            [Fact]
            public void Should_Return_Forbidden_When_Null()
            {
                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => m
                    .Null();

                var scope = builder.Build(specification, context);

                scope.Presence.Should().Be(Presence.Forbidden);
                scope.ForbiddenErrorId.Should().Be(context.ForbiddenErrorId);
            }

            [Theory]
            [MemberData(nameof(AllCommandCases))]
            public void Should_Return_Forbidden_WithCustomError(string testId, Func<dynamic, ISpecificationOut<TestClass>> appendErrorCommands, ErrorContentApiHelper.ExpectedErrorContent expected)
            {
                testId.Should().NotBeNull();

                var context = new ScopeBuilderContext();

                var builder = new ScopeBuilder();

                Specification<TestClass> specification = m => appendErrorCommands(m.Forbidden());

                var scope = builder.Build(specification, context);

                if (expected.ShouldBeEmpty(0))
                {
                    scope.ForbiddenErrorId.Should().Be(context.ForbiddenErrorId);
                }
                else
                {
                    scope.ForbiddenErrorId.Should().NotBe(context.ForbiddenErrorId);

                    context.Errors.Keys.Should().Contain(scope.ForbiddenErrorId);

                    expected.Match(context.Errors[scope.ForbiddenErrorId]);
                }
            }
        }
    }
}
