namespace Validot.Tests.Unit.Validation.Scheme
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Errors.Args;
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
            _ = ModelSchemeFactory.Create<TestClass>(m => m);
        }

        [Fact]
        public void Should_ThrowException_When_NullSpecification()
        {
            Action action = () => ModelSchemeFactory.Create<TestClass>(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_CreateModelScheme_With_Error()
        {
            Specification<TestClass> classSpecification = c => c.Rule(x => false).WithMessage("Invalid value custom message");

            var modelScheme = ModelSchemeFactory.Create(classSpecification);

            var error = modelScheme.ErrorRegistry.Where(e => e.Value.Args.Count == 0 && e.Value.Codes.Count == 0 && e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Invalid value custom message");

            error.Should().HaveCount(1);

            modelScheme.Template.Keys.Should().HaveCount(1);
            modelScheme.Template.Keys.Should().Contain("");
            modelScheme.Template[""].Should().Contain(error.Single().Key);
        }

        [Fact]
        public void Should_CreateModelScheme_With_Errors()
        {
            Specification<TestClass> classSpecification = c => c
                .RuleTemplate(x => false, "Invalid value template message {argName}", Arg.Number("argName", 666L))
                .Rule(x => false).WithMessage("Invalid value custom message")
                .Rule(x => false).WithCode("CODE1");

            var modelScheme = ModelSchemeFactory.Create(classSpecification);

            var error1Candidates = modelScheme.ErrorRegistry.Where(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Invalid value custom message");

            error1Candidates.Should().HaveCount(1);
            var error1 = error1Candidates.Single();
            error1.Value.Codes.Should().BeEmpty();
            error1.Value.Args.Should().BeEmpty();

            var error2Candidates = modelScheme.ErrorRegistry.Where(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Invalid value template message {argName}");
            error2Candidates.Should().HaveCount(1);
            var error2 = error2Candidates.Single();
            error2.Value.Codes.Should().BeEmpty();
            error2.Value.Args.Should().HaveCount(1);
            var error2Arg = error2.Value.Args.Single();
            error2Arg.Should().BeOfType<NumberArg<long>>();
            ((NumberArg<long>)error2Arg).Name.Should().Be("argName");
            ((NumberArg<long>)error2Arg).Value.Should().Be(666);

            var error3Candidates = modelScheme.ErrorRegistry.Where(e => e.Value.Codes.Count == 1 && e.Value.Codes.Single() == "CODE1");
            error3Candidates.Should().HaveCount(1);
            var error3 = error3Candidates.Single();
            error3.Value.Messages.Should().BeEmpty();
            error3.Value.Args.Should().BeEmpty();

            modelScheme.Template.Keys.Should().HaveCount(1);
            modelScheme.Template.Keys.Should().Contain("");
            modelScheme.Template[""].Should().Contain(error1.Key);
            modelScheme.Template[""].Should().Contain(error2.Key);
            modelScheme.Template[""].Should().Contain(error3.Key);
        }

        [Fact]
        public void Should_CreateModelScheme_With_Errors_And_NestedSpecifications()
        {
            Specification<TestMember> memberSpecification = c => c.Optional().RuleTemplate(x => false, "Nested template message", Arg.Number("nestedArg", 100M)).WithExtraCode("CODE_N");

            Specification<TestClass> classSpecification = c => c
                .Optional()
                .Member(m => m.Member, memberSpecification)
                .RuleTemplate(x => false, "Invalid value template message {argName}", Arg.Number("argName", 666L))
                .Rule(x => false).WithMessage("Invalid value custom message")
                .Rule(x => false).WithCode("CODE1");

            var modelScheme = ModelSchemeFactory.Create(classSpecification);

            var error1Candidates = modelScheme.ErrorRegistry.Where(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Invalid value template message {argName}");
            error1Candidates.Should().HaveCount(1);
            var error1 = error1Candidates.Single();
            error1.Value.Codes.Should().BeEmpty();
            error1.Value.Args.Should().HaveCount(1);
            var error1Arg = error1.Value.Args.Single();
            error1Arg.Should().BeOfType<NumberArg<long>>();
            ((NumberArg<long>)error1Arg).Name.Should().Be("argName");
            ((NumberArg<long>)error1Arg).Value.Should().Be(666);

            var error2Candidates = modelScheme.ErrorRegistry.Where(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Invalid value custom message");

            error2Candidates.Should().HaveCount(1);
            var error2 = error2Candidates.Single();
            error2.Value.Codes.Should().BeEmpty();
            error2.Value.Args.Should().BeEmpty();

            var error3Candidates = modelScheme.ErrorRegistry.Where(e => e.Value.Codes.Count == 1 && e.Value.Codes.Single() == "CODE1");
            error3Candidates.Should().HaveCount(1);
            var error3 = error3Candidates.Single();
            error3.Value.Messages.Should().BeEmpty();
            error3.Value.Args.Should().BeEmpty();

            var errorNestedCandidates = modelScheme.ErrorRegistry.Where(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Nested template message");
            errorNestedCandidates.Should().HaveCount(1);
            var errorNested = errorNestedCandidates.Single();
            errorNested.Value.Codes.Should().HaveCount(1);
            errorNested.Value.Codes.Single().Should().Be("CODE_N");
            errorNested.Value.Args.Should().HaveCount(1);
            var errorNestedArg = errorNested.Value.Args.Single();
            errorNestedArg.Should().BeOfType<NumberArg<decimal>>();
            (errorNestedArg as NumberArg<decimal>).Name.Should().Be("nestedArg");
            (errorNestedArg as NumberArg<decimal>).Value.Should().Be(100);

            modelScheme.Template.Keys.Should().HaveCount(2);
            modelScheme.Template.Keys.Should().Contain("");
            modelScheme.Template[""].Should().Contain(error2.Key);
            modelScheme.Template[""].Should().Contain(error2.Key);
            modelScheme.Template[""].Should().Contain(error3.Key);
            modelScheme.Template.Keys.Should().Contain("Member");
            modelScheme.Template["Member"].Should().Contain(errorNested.Key);
        }

        [Fact]
        public void Should_CreateModelScheme_With_PathResolved()
        {
            Specification<TestMember> memberSpecification = c => c.Rule(x => false).WithMessage("Member error");

            Specification<TestClass> classSpecification = c => c
                .Member(m => m.Member, memberSpecification).WithPath("TestNested")
                .Rule(x => false).WithPath("TestNested").WithMessage("Base error");

            var modelScheme = ModelSchemeFactory.Create(classSpecification);

            var memberError = modelScheme.ErrorRegistry.Single(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Member error");

            var baseError = modelScheme.ErrorRegistry.Single(e => e.Value.Messages.Count == 1 && e.Value.Messages.Single() == "Base error");

            modelScheme.Template.Keys.Should().HaveCount(2);
            modelScheme.Template.Keys.Should().Contain("");
            modelScheme.Template[""].Should().NotContain(memberError.Key);
            modelScheme.Template[""].Should().NotContain(baseError.Key);
            modelScheme.Template.Keys.Should().Contain("TestNested");
            modelScheme.Template["TestNested"].Should().Contain(memberError.Key);
            modelScheme.Template["TestNested"].Should().Contain(baseError.Key);
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

                var modelScheme = ModelSchemeFactory.Create(specificationA);

                modelScheme.IsReferenceLoopPossible.Should().BeFalse();
            }

            [Fact]
            public void Should_BeFalse_When_NoLoop_InDirectCase()
            {
                Specification<DirectLoopClassB> specificationB = c => c;

                Specification<DirectLoopClassA> specificationA = c => c.Member(m => m.B, m => m.AsModel(specificationB));

                var modelScheme = ModelSchemeFactory.Create(specificationA);

                modelScheme.IsReferenceLoopPossible.Should().BeFalse();
            }

            [Fact]
            public void Should_BeFalse_When_NoLoop_InNestedCase()
            {
                Specification<NestedLoopClassC> specificationC = c => c;

                Specification<NestedLoopClassB> specificationB = c => c.Member(m => m.C, m => m.AsModel(specificationC));

                Specification<NestedLoopClassA> specificationA = c => c.Member(m => m.B, m => m.AsModel(specificationB));

                var modelScheme = ModelSchemeFactory.Create(specificationA);

                modelScheme.IsReferenceLoopPossible.Should().BeFalse();
            }

            [Fact]
            public void Should_BeTrue_When_SelfLoop()
            {
                Specification<SelfLoop> specification = null;

                specification = c => c.Member(m => m.Self, m => m.AsModel(specification));

                var modelScheme = ModelSchemeFactory.Create(specification);

                modelScheme.IsReferenceLoopPossible.Should().BeTrue();
            }

            [Fact]
            public void Should_BeTrue_When_DirectLoop()
            {
                Specification<DirectLoopClassB> specificationB = null;

                Specification<DirectLoopClassA> specificationA = c => c.Member(m => m.B, m => m.AsModel(specificationB));

                specificationB = c => c.Member(m => m.A, m => m.AsModel(specificationA));

                var modelScheme = ModelSchemeFactory.Create(specificationA);

                modelScheme.IsReferenceLoopPossible.Should().BeTrue();
            }

            [Fact]
            public void Should_BeTrue_When_NestedLoop()
            {
                Specification<NestedLoopClassC> specificationC = null;

                Specification<NestedLoopClassB> specificationB = c => c.Member(m => m.C, m => m.AsModel(specificationC));

                Specification<NestedLoopClassA> specificationA = c => c.Member(m => m.B, m => m.AsModel(specificationB));

                specificationC = c => c.Member(m => m.A, m => m.AsModel(specificationA));

                var modelScheme = ModelSchemeFactory.Create(specificationA);

                modelScheme.IsReferenceLoopPossible.Should().BeTrue();
            }
        }
    }
}
