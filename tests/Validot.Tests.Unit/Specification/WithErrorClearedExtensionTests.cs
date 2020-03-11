namespace Validot.Tests.Unit.Specification
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class WithErrorClearedExtensionTests
    {
        [Fact]
        public void Should_ForbiddenWithErrorCleared_Add_WithErrorClearedCommand()
        {
            ApiTester.TestSingleCommand<object, IWithErrorClearedForbiddenIn<object>, IWithErrorClearedForbiddenOut<object>, WithErrorClearedCommand>(
                s => s.WithErrorCleared());
        }

        [Fact]
        public void Should_ForbiddenWithErrorCleared_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithErrorClearedForbiddenOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IWithMessageForbiddenIn<object>),
                typeof(IWithCodeForbiddenIn<object>),
            });
        }

        [Fact]
        public void Should_WithErrorCleared_Add_WithErrorClearedCommand()
        {
            ApiTester.TestSingleCommand<object, IWithErrorClearedIn<object>, IWithErrorClearedOut<object>, WithErrorClearedCommand>(
                s => s.WithErrorCleared());
        }

        [Fact]
        public void Should_WithErrorCleared_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IWithErrorClearedOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IRuleIn<object>),
                typeof(IWithMessageIn<object>),
                typeof(IWithCodeIn<object>)
            });
        }
    }
}
