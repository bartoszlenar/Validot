namespace Validot.Tests.Unit.Validation
{
    using System;
    using System.Globalization;
    using System.Linq;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Errors;
    using Validot.Errors.Args;
    using Validot.Validation;
    using Validot.Validation.Scopes;

    using Xunit;

    public class DiscoveryContextTests
    {
        [Fact]
        public void Should_Initialize()
        {
            var actions = Substitute.For<IDiscoveryContextActions>();

            _ = new DiscoveryContext(actions, 0);
        }

        [Fact]
        public void Should_Initialize_WithDefaultValues()
        {
            var actions = Substitute.For<IDiscoveryContextActions>();

            var context = new DiscoveryContext(actions, 0);

            context.Errors.Should().BeEmpty();
            context.Paths.Should().BeEmpty();
            context.InfiniteReferencesLoopRoots.Should().BeEmpty();
        }

        public class AddError
        {
            [Fact]
            public void Should_AddError_To_DefaultPath()
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors[string.Empty].Should().HaveCount(1);
                context.Errors[string.Empty].ElementAt(0).Should().Be(123);
            }

            [Fact]
            public void Should_AddErrors_To_DefaultPath()
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.AddError(123);
                context.AddError(1234);
                context.AddError(12345);

                context.Errors.Should().HaveCount(1);
                context.Errors[string.Empty].Should().HaveCount(3);
                context.Errors[string.Empty].ElementAt(0).Should().Be(123);
                context.Errors[string.Empty].ElementAt(1).Should().Be(1234);
                context.Errors[string.Empty].ElementAt(2).Should().Be(12345);
            }

            [Theory]
            [InlineData("path")]
            [InlineData("some.nested.path")]
            public void Should_AddError(string name)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(name);

                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors[name].Should().HaveCount(1);
                context.Errors[name].ElementAt(0).Should().Be(123);
            }

            [Theory]
            [InlineData("path")]
            [InlineData("some.nested.path")]
            public void Should_AddErrors(string name)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(name);

                context.AddError(123);
                context.AddError(1234);
                context.AddError(12345);

                context.Errors.Should().HaveCount(1);
                context.Errors[name].Should().HaveCount(3);
                context.Errors[name].ElementAt(0).Should().Be(123);
                context.Errors[name].ElementAt(1).Should().Be(1234);
                context.Errors[name].ElementAt(2).Should().Be(12345);
            }

            [Theory]
            [InlineData("")]
            [InlineData("path")]
            [InlineData("some.nested.path")]
            public void Should_AddError_When_AlreadyExistsUnderSamePath_And_SkipIfDuplicateInPath_Is_False(string name)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(name);

                context.AddError(123);
                context.AddError(123);
                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors[name].Should().HaveCount(3);
                context.Errors[name].ElementAt(0).Should().Be(123);
                context.Errors[name].ElementAt(1).Should().Be(123);
                context.Errors[name].ElementAt(2).Should().Be(123);
            }

            [Theory]
            [InlineData("")]
            [InlineData("path")]
            [InlineData("some.nested.path")]
            public void Should_NotAddError_When_AlreadyExistsUnderSamePath_And_SkipIfDuplicateInPath_Is_True(string name)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(name);

                context.AddError(123, true);
                context.AddError(123, true);
                context.AddError(123, true);

                context.Errors.Should().HaveCount(1);
                context.Errors[name].Should().HaveCount(1);
                context.Errors[name].ElementAt(0).Should().Be(123);
            }

            [Theory]
            [InlineData("")]
            [InlineData("path")]
            [InlineData("some.nested.path")]
            public void Should_AddError_When_NotExistsUnderSamePath_And_SkipIfDuplicateInPath_Is_True(string name)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(name);

                context.AddError(123, true);
                context.AddError(1234, true);
                context.AddError(12345, true);

                context.Errors.Should().HaveCount(1);
                context.Errors[name].Should().HaveCount(3);
                context.Errors[name].ElementAt(0).Should().Be(123);
                context.Errors[name].ElementAt(1).Should().Be(1234);
                context.Errors[name].ElementAt(2).Should().Be(12345);
            }

            [Fact]
            public void Should_AddError_OnlyWhen_NotExistsUnderSamePath_And_SkipIfDuplicateInPath_Is_True()
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath("test1");

                context.AddError(123, true);
                context.AddError(123, true);
                context.AddError(123);

                context.EnterPath("test2");
                context.AddError(123, true);
                context.AddError(123, true);
                context.AddError(123);

                context.EnterPath("test3");
                context.AddError(123, true);
                context.AddError(123);
                context.AddError(123);

                context.Errors.Should().HaveCount(3);

                context.Errors["test1"].Should().HaveCount(2);
                context.Errors["test1"].ElementAt(0).Should().Be(123);
                context.Errors["test1"].ElementAt(1).Should().Be(123);

                context.Errors["test1.test2"].Should().HaveCount(2);
                context.Errors["test1.test2"].ElementAt(0).Should().Be(123);
                context.Errors["test1.test2"].ElementAt(1).Should().Be(123);

                context.Errors["test1.test2.test3"].Should().HaveCount(3);
                context.Errors["test1.test2.test3"].ElementAt(0).Should().Be(123);
                context.Errors["test1.test2.test3"].ElementAt(1).Should().Be(123);
                context.Errors["test1.test2.test3"].ElementAt(2).Should().Be(123);
            }
        }

        public class EnterPath_And_AddingErrors
        {
            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath_AllCases), MemberType = typeof(PathsTestData))]
            public void AddErrors_Should_AddToEnteredPath_AfterStepIntoNextPath(string basePath, string newSegment, string expectedPath)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(basePath);
                context.EnterPath(newSegment);

                context.AddError(123);

                context.Errors.Keys.Should().ContainSingle(expectedPath);
                context.Errors.Should().HaveCount(1);
                context.Errors[expectedPath].Should().HaveCount(1);
                context.Errors[expectedPath].ElementAt(0).Should().Be(123);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath_AllCases), MemberType = typeof(PathsTestData))]
            public void AddErrors_Should_AddToBasePath_And_EnteredPath(string basePath, string newSegment, string expectedPath)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(basePath);

                context.AddError(123);

                context.Errors.Keys.Should().ContainSingle(expectedPath);
                context.Errors.Should().HaveCount(1);
                context.Errors[basePath].Should().HaveCount(1);
                context.Errors[basePath].ElementAt(0).Should().Be(123);

                context.EnterPath(newSegment);

                context.AddError(321);

                if (basePath == expectedPath)
                {
                    context.Errors.Keys.Should().HaveCount(1);
                    context.Errors.Keys.Should().Contain(basePath);
                    context.Errors[basePath].Should().HaveCount(2);
                    context.Errors[basePath].ElementAt(0).Should().Be(123);
                    context.Errors[basePath].ElementAt(1).Should().Be(321);
                }
                else
                {
                    context.Errors.Keys.Should().HaveCount(2);
                    context.Errors.Keys.Should().Contain(basePath);
                    context.Errors.Keys.Should().Contain(expectedPath);
                    context.Errors[basePath].Should().HaveCount(1);
                    context.Errors[basePath].ElementAt(0).Should().Be(123);
                    context.Errors[expectedPath].Should().HaveCount(1);
                    context.Errors[expectedPath].ElementAt(0).Should().Be(321);
                }
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath_AllCases), MemberType = typeof(PathsTestData))]
            public void AddErrors_Should_AddToPreviousPathAfterStepOut(string basePath, string newSegment, string expectedPath)
            {
                _ = expectedPath;

                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(basePath);
                context.EnterPath(newSegment);
                context.LeavePath();

                context.AddError(123);

                context.Errors.Keys.Should().ContainSingle(basePath);
                context.Errors.Should().HaveCount(1);
                context.Errors[basePath].Should().HaveCount(1);
                context.Errors[basePath].ElementAt(0).Should().Be(123);
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath_AllCases), MemberType = typeof(PathsTestData))]
            public void AddErrors_Should_AddToEnteredPath_And_ToPreviousPathAfterStepOut(string basePath, string newSegment, string expectedPath)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(basePath);
                context.EnterPath(newSegment);

                context.AddError(123);

                context.Errors.Keys.Should().ContainSingle(expectedPath);
                context.Errors.Should().HaveCount(1);
                context.Errors[expectedPath].Should().HaveCount(1);
                context.Errors[expectedPath].ElementAt(0).Should().Be(123);

                context.LeavePath();

                context.AddError(321);

                if (basePath == expectedPath)
                {
                    context.Errors.Keys.Should().HaveCount(1);
                    context.Errors.Keys.Should().Contain(basePath);
                    context.Errors[basePath].Should().HaveCount(2);
                    context.Errors[basePath].ElementAt(0).Should().Be(123);
                    context.Errors[basePath].ElementAt(1).Should().Be(321);
                }
                else
                {
                    context.Errors.Keys.Should().HaveCount(2);
                    context.Errors.Keys.Should().Contain(basePath);
                    context.Errors.Keys.Should().Contain(expectedPath);
                    context.Errors[expectedPath].Should().HaveCount(1);
                    context.Errors[expectedPath].ElementAt(0).Should().Be(123);
                    context.Errors[basePath].Should().HaveCount(1);
                    context.Errors[basePath].ElementAt(0).Should().Be(321);
                }
            }

            [Theory]
            [InlineData("", "#")]
            [InlineData("base.#.stuff", "base.#.stuff.#")]
            [InlineData("#.a.#.b.#", "#.a.#.b.#.#")]
            public void EnterCollectionItemPath_Should_EnterToCollectionIndexPrefix(string basePath, string expectedPath)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(basePath);
                context.EnterCollectionItemPath();

                context.AddError(123);

                context.Errors.Should().HaveCount(1);
                context.Errors.Keys.Should().Contain(expectedPath);
                context.Errors[expectedPath].Should().HaveCount(1);
                context.Errors[expectedPath].ElementAt(0).Should().Be(123);
            }

            [Fact]
            public void EnterCollectionItemPath_Should_LeavePathWithCollectionIndexPrefix()
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath("path");
                context.EnterCollectionItemPath();

                context.LeavePath();

                context.AddError(321);

                context.Errors.Keys.Should().ContainSingle("path");
                context.Errors.Should().HaveCount(1);
                context.Errors["path"].Should().HaveCount(1);
                context.Errors["path"].ElementAt(0).Should().Be(321);
            }
        }

        public class EnterPath
        {
            [Fact]
            public void Should_FillInitialPath()
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath("path");

                context.Paths.Keys.Should().HaveCount(1);
                context.Paths.Keys.Should().Contain("");
                context.Paths[""].Should().ContainKey("path");
                context.Paths[""]["path"].Should().Be("path");
            }

            [Theory]
            [MemberData(nameof(PathsTestData.ResolveNextLevelPath_AllCases), MemberType = typeof(PathsTestData))]
            public void Should_FillPaths(string basePath, string newSegment, string expectedPath)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath(basePath);
                context.EnterPath(newSegment);

                if (basePath == "")
                {
                    context.Paths.Keys.Should().ContainSingle(k => k == "");

                    if (newSegment == "")
                    {
                        context.Paths[""].Keys.Should().ContainSingle(k => k == "");
                        context.Paths[""][""].Should().Be("");
                    }
                    else
                    {
                        context.Paths[""].Keys.Should().HaveCount(2);
                        context.Paths[""].Keys.Should().Contain("");
                        context.Paths[""].Keys.Should().Contain(newSegment);
                        context.Paths[""][newSegment].Should().Be(expectedPath);
                    }
                }
                else
                {
                    context.Paths.Keys.Should().HaveCount(2);

                    context.Paths.Keys.Should().Contain("");
                    context.Paths.Keys.Should().Contain(basePath);

                    context.Paths[""].Keys.Should().ContainSingle(basePath);
                    context.Paths[""][basePath].Should().Be(basePath);

                    context.Paths[basePath].Keys.Should().ContainSingle(newSegment);
                    context.Paths[basePath][newSegment].Should().Be(expectedPath);
                }
            }

            [Fact]
            public void Should_FillPaths_And_NotModifyOnLeave()
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                context.EnterPath("base");
                context.EnterPath("path");
                context.EnterPath("nested");

                StateCheck(context);

                context.LeavePath();

                StateCheck(context);

                context.LeavePath();

                StateCheck(context);

                context.LeavePath();

                StateCheck(context);

                void StateCheck(DiscoveryContext ctx)
                {
                    ctx.Paths.Keys.Should().HaveCount(3);
                    ctx.Paths.Keys.Should().Contain("");
                    ctx.Paths.Keys.Should().Contain("base");
                    ctx.Paths.Keys.Should().Contain("base.path");

                    ctx.Paths[""].Keys.Should().ContainSingle("base");
                    ctx.Paths[""]["base"].Should().Be("base");

                    ctx.Paths["base"].Keys.Should().ContainSingle("path");
                    ctx.Paths["base"]["path"].Should().Be("base.path");

                    ctx.Paths["base.path"].Keys.Should().ContainSingle("nested");
                    ctx.Paths["base.path"]["nested"].Should().Be("base.path.nested");
                }
            }
        }

        public class EnterScope
        {
            public class TestScope
            {
            }

            [Fact]
            public void Should_GetDiscoverableSpecificationScope_And_ExecuteDiscover_With_ItselfAsParameter()
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                var discoverableSpecificationScope = Substitute.For<IDiscoverable>();

                actions.GetDiscoverableSpecificationScope(Arg.Is(123)).Returns(discoverableSpecificationScope);

                context.EnterScope<TestScope>(123);

                actions.Received(1).GetDiscoverableSpecificationScope(Arg.Is(123));
                discoverableSpecificationScope.Received(1).Discover(Arg.Is(context));
            }

            [Theory]
            [InlineData(2)]
            [InlineData(5)]
            [InlineData(10)]
            public void Should_GetDiscoverableSpecificationScope_And_ExecuteDiscover_With_ItselfAsParameter_ManyTimes(int levels)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                var scopes = Enumerable.Range(1, levels).Select(i => Substitute.For<IDiscoverable>()).ToArray();

                for (var i = 1; i < levels; ++i)
                {
                    actions.GetDiscoverableSpecificationScope(Arg.Is(i)).Returns(scopes[i]);
                }

                for (var i = 1; i < levels; ++i)
                {
                    context.EnterScope<TestScope>(i);
                }

                for (var i = 1; i < levels; ++i)
                {
                    actions.Received(1).GetDiscoverableSpecificationScope(i);
                    scopes[i].Received(1).Discover(Arg.Is(context));
                }
            }

            [Theory]
            [InlineData("")]
            [InlineData("path")]
            [InlineData("test.path")]
            public void Should_NotEnterScope_And_Register_InfiniteReferencesLoopRoot_InfiniteReferencesLoopExists(string name)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                var discoverableSpecificationScope = Substitute.For<IDiscoverable>();

                actions.GetDiscoverableSpecificationScope(Arg.Is(123)).Returns(discoverableSpecificationScope);

                actions.RegisterError(Arg.Is<IError>(a =>
                    a is CircularDependencyError &&
                    ((a as CircularDependencyError).Args.Single() as TypeArg).Value == typeof(TestScope))).Returns(666);

                context.EnterScope<TestScope>(123);

                actions.Received(1).GetDiscoverableSpecificationScope(Arg.Is(123));
                discoverableSpecificationScope.Received(1).Discover(Arg.Is(context));

                actions.DidNotReceiveWithAnyArgs().RegisterError(Arg.Any<IError>());

                context.EnterPath(name);
                context.EnterScope<TestScope>(123);

                actions.Received(1).GetDiscoverableSpecificationScope(Arg.Is(123));
                discoverableSpecificationScope.Received(1).Discover(Arg.Is(context));

                actions.Received(1).RegisterError(Arg.Is<IError>(a =>
                    a is CircularDependencyError &&
                    ((a as CircularDependencyError).Args.Single() as TypeArg).Value == typeof(TestScope)));

                context.Errors.Keys.Should().ContainSingle(name);
                context.Errors[name].Should().HaveCount(1);
                context.Errors[name].Single().Should().Be(666);

                context.InfiniteReferencesLoopRoots.Should().ContainSingle(name);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(5)]
            [InlineData(10)]
            public void Should_NotEnterScope_And_Register_InfiniteReferencesLoopRootDetected_WhenLevelsBetween_And_InfiniteReferencesLoopExists_NotFromRoot(int levels)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                actions.RegisterError(Arg.Is<IError>(a =>
                    a is CircularDependencyError &&
                    ((a as CircularDependencyError).Args.Single() as TypeArg).Value == typeof(TestScope))).Returns(666);

                var context = new DiscoveryContext(actions, 100);

                var scopes = Enumerable.Range(0, levels).Select(i => Substitute.For<IDiscoverable>()).ToArray();

                for (var i = 0; i < levels; ++i)
                {
                    actions.GetDiscoverableSpecificationScope(Arg.Is(i)).Returns(scopes[i]);
                }

                for (var i = 0; i < levels; ++i)
                {
                    context.EnterPath(i.ToString(CultureInfo.InvariantCulture));
                    context.EnterScope<TestScope>(i);
                }

                context.EnterPath(levels.ToString(CultureInfo.InvariantCulture));
                context.EnterScope<TestScope>(0);

                for (var i = 0; i < levels; ++i)
                {
                    actions.Received(1).GetDiscoverableSpecificationScope(i);
                    scopes[i].Received(1).Discover(Arg.Is(context));
                }

                var errorLevel = string.Join(".", Enumerable.Range(0, levels + 1).Select(i => i).ToArray());

                context.Errors.Keys.Should().ContainSingle(errorLevel);
                context.Errors[errorLevel].Should().HaveCount(1);
                context.Errors[errorLevel].Single().Should().Be(666);

                context.InfiniteReferencesLoopRoots.Should().ContainSingle(errorLevel);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(5)]
            [InlineData(10)]
            public void Should_NotEnterScope_And_Register_InfiniteReferencesLoopRootDetected_WhenLevelsBetween_And_InfiniteReferencesLoopExists_FromRoot(int levels)
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                actions.RegisterError(Arg.Is<IError>(a =>
                    a is CircularDependencyError &&
                    ((a as CircularDependencyError).Args.Single() as TypeArg).Value == typeof(TestScope))).Returns(666);

                var context = new DiscoveryContext(actions, 0);

                var scopes = Enumerable.Range(0, levels).Select(i => Substitute.For<IDiscoverable>()).ToArray();

                for (var i = 0; i < levels; ++i)
                {
                    actions.GetDiscoverableSpecificationScope(Arg.Is(i)).Returns(scopes[i]);
                }

                for (var i = 1; i < levels; ++i)
                {
                    context.EnterPath(i.ToString(CultureInfo.InvariantCulture));
                    context.EnterScope<TestScope>(i);
                }

                context.EnterPath(levels.ToString(CultureInfo.InvariantCulture));
                context.EnterScope<TestScope>(0);

                for (var i = 1; i < levels; ++i)
                {
                    actions.Received(1).GetDiscoverableSpecificationScope(i);
                    scopes[i].Received(1).Discover(Arg.Is(context));
                }

                var errorLevel = string.Join(".", Enumerable.Range(1, levels).Select(i => i).ToArray());

                context.Errors.Keys.Should().ContainSingle(errorLevel);
                context.Errors[errorLevel].Should().HaveCount(1);
                context.Errors[errorLevel].Single().Should().Be(666);

                context.InfiniteReferencesLoopRoots.Should().ContainSingle(errorLevel);
            }

            [Fact]
            public void Should_NotEnterScope_And_Populate_InfiniteReferencesLoopRootDetected_When_MultipleInfiniteReferencesLoopsExist()
            {
                var actions = Substitute.For<IDiscoveryContextActions>();

                var context = new DiscoveryContext(actions, 0);

                var discoverableSpecificationScope = Substitute.For<IDiscoverable>();

                actions.GetDiscoverableSpecificationScope(Arg.Is(123)).Returns(discoverableSpecificationScope);

                actions.RegisterError(Arg.Is<IError>(a =>
                    a is CircularDependencyError &&
                    ((a as CircularDependencyError).Args.Single() as TypeArg).Value == typeof(TestScope))).Returns(666);

                actions.RegisterError(Arg.Is<IError>(a =>
                    a is CircularDependencyError &&
                    ((a as CircularDependencyError).Args.Single() as TypeArg).Value == typeof(int))).Returns(667);

                actions.RegisterError(Arg.Is<IError>(a =>
                    a is CircularDependencyError &&
                    ((a as CircularDependencyError).Args.Single() as TypeArg).Value == typeof(DateTimeOffset?))).Returns(668);

                context.EnterScope<TestScope>(123);
                context.EnterScope<int>(321);

                context.EnterPath("base");

                context.EnterScope<TestScope>(1);
                context.EnterScope<int>(2);

                context.EnterPath("path");

                context.EnterScope<TestScope>(123);
                context.EnterScope<int>(321);

                context.EnterPath("nested");
                context.EnterScope<DateTimeOffset?>(333);
                context.EnterScope<DateTimeOffset?>(333);

                actions.Received(1).GetDiscoverableSpecificationScope(Arg.Is(123));
                actions.Received(1).GetDiscoverableSpecificationScope(Arg.Is(321));
                actions.Received(1).GetDiscoverableSpecificationScope(Arg.Is(333));

                actions.Received(1).RegisterError(Arg.Is<IError>(a =>
                    a is CircularDependencyError &&
                    ((a as CircularDependencyError).Args.Single() as TypeArg).Value == typeof(TestScope)));

                actions.Received(1).RegisterError(Arg.Is<IError>(a =>
                    a is CircularDependencyError &&
                    ((a as CircularDependencyError).Args.Single() as TypeArg).Value == typeof(int)));

                actions.Received(1).RegisterError(Arg.Is<IError>(a =>
                    a is CircularDependencyError &&
                    ((a as CircularDependencyError).Args.Single() as TypeArg).Value == typeof(DateTimeOffset?)));

                context.Errors.Keys.Should().Contain("base.path");
                context.Errors["base.path"].Should().HaveCount(2);
                context.Errors["base.path"].ElementAt(0).Should().Be(666);
                context.Errors["base.path"].ElementAt(1).Should().Be(667);
                context.Errors["base.path.nested"].Should().HaveCount(1);
                context.Errors["base.path.nested"].ElementAt(0).Should().Be(668);

                context.InfiniteReferencesLoopRoots.Should().HaveCount(2);
                context.InfiniteReferencesLoopRoots.Should().Contain("base.path");
                context.InfiniteReferencesLoopRoots.Should().Contain("base.path.nested");
            }
        }
    }
}
