namespace Validot.Tests.Unit.Validation.Scheme
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Errors.Args;
    using Validot.Settings.Capacities;
    using Validot.Validation;
    using Validot.Validation.Scheme;

    using Xunit;

    using Arg = Validot.Arg;

    public class ModelSchemeFactoryTests
    {
        public class TestMember
        {
        }

        public class TestClass
        {
            public TestMember Member { get; set; }
        }

        [Fact]
        public void Should_CreateModelScheme()
        {
            var capacityInfo = Substitute.For<ICapacityInfo>();

            _ = ModelSchemeFactory.Create<TestClass>(m => m, capacityInfo);
        }

        [Fact]
        public void Should_ThrowException_When_NullSpecification()
        {
            var capacityInfo = Substitute.For<ICapacityInfo>();

            Action action = () => ModelSchemeFactory.Create<TestClass>(null, capacityInfo);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_NullCapacityInfo()
        {
            Action action = () => ModelSchemeFactory.Create<TestClass>(m => m, null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_CreateModelScheme_With_Error()
        {
            Specification<TestClass> classSpecification = c => c.Rule(x => false).WithMessage("Invalid value custom message");

            var capacityInfo = Substitute.For<ICapacityInfo>();

            var modelScheme = ModelSchemeFactory.Create(classSpecification, capacityInfo);

            var error = modelScheme.ErrorsRegistry.Where(e => e.Value.Args.Count == 0 && e.Value.Codes.Count == 0 && e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Invalid value custom message");

            error.Should().HaveCount(1);

            modelScheme.ErrorMap.Keys.Should().HaveCount(1);
            modelScheme.ErrorMap.Keys.Should().Contain("");
            modelScheme.ErrorMap[""].Should().Contain(error.Single().Key);
        }

        [Fact]
        public void Should_CreateModelScheme_With_Errors()
        {
            Specification<TestClass> classSpecification = c => c
                .RuleTemplate(x => false, "Invalid value template message {argName}", Arg.Number("argName", 666L))
                .Rule(x => false).WithMessage("Invalid value custom message")
                .Rule(x => false).WithCode("CODE1");

            var capacityInfo = Substitute.For<ICapacityInfo>();

            var modelScheme = ModelSchemeFactory.Create(classSpecification, capacityInfo);

            var error1Candidates = modelScheme.ErrorsRegistry.Where(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Invalid value custom message");

            error1Candidates.Should().HaveCount(1);
            var error1 = error1Candidates.Single();
            error1.Value.Codes.Should().BeEmpty();
            error1.Value.Args.Should().BeEmpty();

            var error2Candidates = modelScheme.ErrorsRegistry.Where(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Invalid value template message {argName}");
            error2Candidates.Should().HaveCount(1);
            var error2 = error2Candidates.Single();
            error2.Value.Codes.Should().BeEmpty();
            error2.Value.Args.Should().HaveCount(1);
            var error2Arg = error2.Value.Args.Single();
            error2Arg.Should().BeOfType<NumberArg<long>>();
            ((NumberArg<long>)error2Arg).Name.Should().Be("argName");
            ((NumberArg<long>)error2Arg).Value.Should().Be(666);

            var error3Candidates = modelScheme.ErrorsRegistry.Where(e => e.Value.Codes.Count == 1 && e.Value.Codes.Single() == "CODE1");
            error3Candidates.Should().HaveCount(1);
            var error3 = error3Candidates.Single();
            error3.Value.Messages.Should().BeEmpty();
            error3.Value.Args.Should().BeEmpty();

            modelScheme.ErrorMap.Keys.Should().HaveCount(1);
            modelScheme.ErrorMap.Keys.Should().Contain("");
            modelScheme.ErrorMap[""].Should().Contain(error1.Key);
            modelScheme.ErrorMap[""].Should().Contain(error2.Key);
            modelScheme.ErrorMap[""].Should().Contain(error3.Key);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_CreateModelScheme_And_InjectCapacityInfoHelpers_When_CapacityInfo_Is_CapacityInfoHelpersConsumer(bool feedable)
        {
            Specification<TestClass> classSpecification = c => c
                .Optional()
                .RuleTemplate(x => false, "Invalid value template message {argName}", Arg.Number("argName", 666L))
                .Rule(x => false).WithMessage("Invalid value custom message")
                .Rule(x => false).WithCode("CODE1");

            var capacityInfo = feedable
                ? Substitute.For<IFeedableCapacityInfo, ICapacityInfoHelpersConsumer>()
                : Substitute.For<ICapacityInfo, ICapacityInfoHelpersConsumer>();

            ModelSchemeFactory.Create(classSpecification, capacityInfo);

            (capacityInfo as ICapacityInfoHelpersConsumer).Received(1).InjectHelpers(NSubstitute.Arg.Is(ModelSchemeFactory.CapacityInfoHelpers));
        }

        [Fact]
        public void Should_CreateModelScheme_And_FeedCapacityInfo_When_ShouldFeed_IsTrue()
        {
            Specification<TestClass> classSpecification = c => c
                .Optional()
                .RuleTemplate(x => false, "Invalid value template message {argName}", Arg.Number("argName", 666L))
                .Rule(x => false).WithMessage("Invalid value custom message")
                .Rule(x => false).WithCode("CODE1");

            var feedableCapacityInfo = Substitute.For<IFeedableCapacityInfo>();

            IErrorsHolder errorsHolders = null;

            feedableCapacityInfo.ShouldFeed.Returns(true);

            feedableCapacityInfo.When(x => x.Feed(NSubstitute.Arg.Any<IErrorsHolder>())).Do(callInfo =>
            {
                errorsHolders = callInfo.Arg<IErrorsHolder>();
            });

            ModelSchemeFactory.Create(classSpecification, feedableCapacityInfo);

            feedableCapacityInfo.ReceivedWithAnyArgs(1).Feed(default);

            errorsHolders.Should().NotBeNull();
            errorsHolders.Should().BeOfType<DiscoveryContext>();
            errorsHolders.Errors.Keys.Should().HaveCount(1);
            errorsHolders.Errors.Keys.Should().Contain("");
            errorsHolders.Errors[""].Count.Should().Be(3);
        }

        [Fact]
        public void Should_CreateModelScheme_And_NotFeedCapacityInfo_When_ShouldFeed_IsFalse()
        {
            Specification<TestClass> classSpecification = c => c
                .Optional()
                .RuleTemplate(x => false, "Invalid value template message", Arg.Number("argName", 666L))
                .Rule(x => false).WithMessage("Invalid value custom message")
                .Rule(x => false).WithCode("CODE1");

            var feedableCapacityInfo = Substitute.For<IFeedableCapacityInfo>();

            IErrorsHolder errorsHolders = null;

            feedableCapacityInfo.ShouldFeed.Returns(false);

            feedableCapacityInfo.When(x => x.Feed(NSubstitute.Arg.Any<IErrorsHolder>())).Do(callInfo =>
            {
                errorsHolders = callInfo.Arg<IErrorsHolder>();
            });

            ModelSchemeFactory.Create(classSpecification, feedableCapacityInfo);

            feedableCapacityInfo.ReceivedWithAnyArgs(0).Feed(default);

            errorsHolders.Should().BeNull();
        }

        [Fact]
        public void Should_CreateModelScheme_With_Errors_And_NestedSpecifications_And_StatsFed()
        {
            Specification<TestMember> memberSpecification = c => c.Optional().RuleTemplate(x => false, "Nested template message", Arg.Number("nestedArg", 100M)).WithExtraCode("CODE_N");

            Specification<TestClass> classSpecification = c => c
                .Optional()
                .Member(m => m.Member, memberSpecification)
                .RuleTemplate(x => false, "Invalid value template message {argName}", Arg.Number("argName", 666L))
                .Rule(x => false).WithMessage("Invalid value custom message")
                .Rule(x => false).WithCode("CODE1");

            var feedableCapacityInfo = Substitute.For<IFeedableCapacityInfo>();

            IErrorsHolder errorsHolders = null;

            feedableCapacityInfo.ShouldFeed.Returns(true);

            feedableCapacityInfo.When(x => x.Feed(NSubstitute.Arg.Any<IErrorsHolder>())).Do(callInfo =>
            {
                errorsHolders = callInfo.Arg<IErrorsHolder>();
            });

            var modelScheme = ModelSchemeFactory.Create(classSpecification, feedableCapacityInfo);

            var error1Candidates = modelScheme.ErrorsRegistry.Where(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Invalid value template message {argName}");
            error1Candidates.Should().HaveCount(1);
            var error1 = error1Candidates.Single();
            error1.Value.Codes.Should().BeEmpty();
            error1.Value.Args.Should().HaveCount(1);
            var error1Arg = error1.Value.Args.Single();
            error1Arg.Should().BeOfType<NumberArg<long>>();
            ((NumberArg<long>)error1Arg).Name.Should().Be("argName");
            ((NumberArg<long>)error1Arg).Value.Should().Be(666);

            var error2Candidates = modelScheme.ErrorsRegistry.Where(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Invalid value custom message");

            error2Candidates.Should().HaveCount(1);
            var error2 = error2Candidates.Single();
            error2.Value.Codes.Should().BeEmpty();
            error2.Value.Args.Should().BeEmpty();

            var error3Candidates = modelScheme.ErrorsRegistry.Where(e => e.Value.Codes.Count == 1 && e.Value.Codes.Single() == "CODE1");
            error3Candidates.Should().HaveCount(1);
            var error3 = error3Candidates.Single();
            error3.Value.Messages.Should().BeEmpty();
            error3.Value.Args.Should().BeEmpty();

            var errorNestedCandidates = modelScheme.ErrorsRegistry.Where(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Nested template message");
            errorNestedCandidates.Should().HaveCount(1);
            var errorNested = errorNestedCandidates.Single();
            errorNested.Value.Codes.Should().HaveCount(1);
            errorNested.Value.Codes.Single().Should().Be("CODE_N");
            errorNested.Value.Args.Should().HaveCount(1);
            var errorNestedArg = errorNested.Value.Args.Single();
            errorNestedArg.Should().BeOfType<NumberArg<decimal>>();
            (errorNestedArg as NumberArg<decimal>).Name.Should().Be("nestedArg");
            (errorNestedArg as NumberArg<decimal>).Value.Should().Be(100);

            modelScheme.ErrorMap.Keys.Should().HaveCount(2);
            modelScheme.ErrorMap.Keys.Should().Contain("");
            modelScheme.ErrorMap[""].Should().Contain(error2.Key);
            modelScheme.ErrorMap[""].Should().Contain(error2.Key);
            modelScheme.ErrorMap[""].Should().Contain(error3.Key);
            modelScheme.ErrorMap.Keys.Should().Contain("Member");
            modelScheme.ErrorMap["Member"].Should().Contain(errorNested.Key);

            feedableCapacityInfo.ReceivedWithAnyArgs(1).Feed(default);

            errorsHolders.Should().NotBeNull();
            errorsHolders.Should().BeOfType<DiscoveryContext>();
            errorsHolders.Errors.Keys.Should().HaveCount(2);
            errorsHolders.Errors.Keys.Should().Contain("");
            errorsHolders.Errors.Keys.Should().Contain("Member");
            errorsHolders.Errors[""].Count.Should().Be(3);
            errorsHolders.Errors[""][0].Should().Be(error1.Key);
            errorsHolders.Errors[""][1].Should().Be(error2.Key);
            errorsHolders.Errors[""][2].Should().Be(error3.Key);
            errorsHolders.Errors["Member"].Count.Should().Be(1);
            errorsHolders.Errors["Member"][0].Should().Be(errorNested.Key);
        }

        [Fact]
        public void Should_CreateModelScheme_With_PathResolved()
        {
            Specification<TestMember> memberSpecification = c => c.Rule(x => false).WithMessage("Member error");

            Specification<TestClass> classSpecification = c => c
                .Member(m => m.Member, memberSpecification).WithPath("TestNested")
                .Rule(x => false).WithPath("TestNested").WithMessage("Base error");

            var capacityInfo = Substitute.For<ICapacityInfo>();

            var modelScheme = ModelSchemeFactory.Create(classSpecification, capacityInfo);

            var memberError = modelScheme.ErrorsRegistry.Single(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Member error");

            var baseError = modelScheme.ErrorsRegistry.Single(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Base error");

            modelScheme.ErrorMap.Keys.Should().HaveCount(2);
            modelScheme.ErrorMap.Keys.Should().Contain("");
            modelScheme.ErrorMap[""].Should().NotContain(memberError.Key);
            modelScheme.ErrorMap[""].Should().NotContain(baseError.Key);
            modelScheme.ErrorMap.Keys.Should().Contain("TestNested");
            modelScheme.ErrorMap["TestNested"].Should().Contain(memberError.Key);
            modelScheme.ErrorMap["TestNested"].Should().Contain(baseError.Key);
        }

        public class ReferenceLoopDetected
        {
            public class SelfLoop
            {
                public SelfLoop Self { get; set; }
            }

            public class DirectLoopClassA
            {
                public DirectLoopClassB B { get; set; }
            }

            public class DirectLoopClassB
            {
                public DirectLoopClassA A { get; set; }
            }

            public class NestedLoopClassA
            {
                public NestedLoopClassB B { get; set; }
            }

            public class NestedLoopClassB
            {
                public NestedLoopClassC C { get; set; }
            }

            public class NestedLoopClassC
            {
                public NestedLoopClassA A { get; set; }
            }

            [Fact]
            public void Should_BeFalse_When_NoLoop_InSelfCase()
            {
                Specification<SelfLoop> specificationB = c => c;

                Specification<SelfLoop> specificationA = c => c.Member(m => m.Self, m => m.AsModel(specificationB));

                var capacityInfo = Substitute.For<ICapacityInfo>();

                var modelScheme = ModelSchemeFactory.Create(specificationA, capacityInfo);

                modelScheme.IsReferenceLoopPossible.Should().BeFalse();
            }

            [Fact]
            public void Should_BeFalse_When_NoLoop_InDirectCase()
            {
                Specification<DirectLoopClassB> specificationB = c => c;

                Specification<DirectLoopClassA> specificationA = c => c.Member(m => m.B, m => m.AsModel(specificationB));

                var capacityInfo = Substitute.For<ICapacityInfo>();

                var modelScheme = ModelSchemeFactory.Create(specificationA, capacityInfo);

                modelScheme.IsReferenceLoopPossible.Should().BeFalse();
            }

            [Fact]
            public void Should_BeFalse_When_NoLoop_InNestedCase()
            {
                Specification<NestedLoopClassC> specificationC = c => c;

                Specification<NestedLoopClassB> specificationB = c => c.Member(m => m.C, m => m.AsModel(specificationC));

                Specification<NestedLoopClassA> specificationA = c => c.Member(m => m.B, m => m.AsModel(specificationB));

                var capacityInfo = Substitute.For<ICapacityInfo>();

                var modelScheme = ModelSchemeFactory.Create(specificationA, capacityInfo);

                modelScheme.IsReferenceLoopPossible.Should().BeFalse();
            }

            [Fact]
            public void Should_BeTrue_When_SelfLoop()
            {
                Specification<SelfLoop> specification = null;

                specification = c => c.Member(m => m.Self, m => m.AsModel(specification));

                var capacityInfo = Substitute.For<ICapacityInfo>();

                var modelScheme = ModelSchemeFactory.Create(specification, capacityInfo);

                modelScheme.IsReferenceLoopPossible.Should().BeTrue();
            }

            [Fact]
            public void Should_BeTrue_When_DirectLoop()
            {
                Specification<DirectLoopClassB> specificationB = null;

                Specification<DirectLoopClassA> specificationA = c => c.Member(m => m.B, m => m.AsModel(specificationB));

                specificationB = c => c.Member(m => m.A, m => m.AsModel(specificationA));

                var capacityInfo = Substitute.For<ICapacityInfo>();

                var modelScheme = ModelSchemeFactory.Create(specificationA, capacityInfo);

                modelScheme.IsReferenceLoopPossible.Should().BeTrue();
            }

            [Fact]
            public void Should_BeTrue_When_NestedLoop()
            {
                Specification<NestedLoopClassC> specificationC = null;

                Specification<NestedLoopClassB> specificationB = c => c.Member(m => m.C, m => m.AsModel(specificationC));

                Specification<NestedLoopClassA> specificationA = c => c.Member(m => m.B, m => m.AsModel(specificationB));

                specificationC = c => c.Member(m => m.A, m => m.AsModel(specificationA));

                var capacityInfo = Substitute.For<ICapacityInfo>();

                var modelScheme = ModelSchemeFactory.Create(specificationA, capacityInfo);

                modelScheme.IsReferenceLoopPossible.Should().BeTrue();
            }
        }
    }
}
