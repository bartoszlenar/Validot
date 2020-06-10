namespace Validot.Tests.Functional.Documentation
{
    using System;

    using FluentAssertions;

    using Validot.Testing;
    using Validot.Tests.Functional.Documentation.Models;
    using Validot.Validation.Stacks;

    using Xunit;

    public class ReferenceLoopFuncTests
    {
        [Fact]
        public void ReferenceLoop_Results()
        {
            Specification<B> specificationB = null;

            Specification<A> specificationA = s => s
                .Member(m => m.B, specificationB);

            specificationB = s => s
                .Member(m => m.A, specificationA);

            var validator = Validator.Factory.Create(specificationA);

            var a = new A()
            {
                B = new B()
                {
                    A = new A()
                    {
                        B = new B()
                        {
                            A = null
                        }
                    }
                }
            };

            validator.Validate(a).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "B.A.B.A: Required");
        }

        [Fact]
        public void ReferenceLoop_Exception()
        {
            Specification<B> specificationB = null;

            Specification<A> specificationA = s => s
                .Member(m => m.B, specificationB);

            specificationB = s => s
                .Member(m => m.A, specificationA);

            var validator = Validator.Factory.Create(specificationA);

            var a = new A()
            {
                B = new B()
                {
                    A = new A()
                    {
                        B = new B()
                        {
                            A = null
                        }
                    }
                }
            };

            a.B.A.B.A = a.B.A;

            bool exceptionPresent = false;

            try
            {
                validator.Validate(a);
            }
            catch (ReferenceLoopException exception)
            {
                exception.Path.Should().Be("B.A");
                exception.NestedPath.Should().Be("B.A.B.A");
                exception.Type.Should().Be(typeof(A));
                exceptionPresent = true;
            }

            exceptionPresent.Should().BeTrue();
        }
    }
}
