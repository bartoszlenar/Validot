namespace Validot.Tests.Unit.Specification
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class ForbiddenExtensionTests
    {
        [Fact]
        public void Should_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IForbiddenOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IWithMessageForbiddenIn<object>),
                typeof(IWithExtraMessageForbiddenIn<object>),
                typeof(IWithCodeForbiddenIn<object>),
                typeof(IWithExtraCodeForbiddenIn<object>),
            });
        }

        [Fact]
        public void Should_Forbidden_Add_ForbiddenCommand()
        {
            ApiTester.TestSingleCommand<object, IForbiddenIn<object>, IForbiddenOut<object>, ForbiddenCommand>(
                s => s.Forbidden());
        }

        [Fact]
        public void Should_Forbidden_Add_ForbiddenCommand_When_Nullable()
        {
            ApiTester.TestSingleCommand<int?, IForbiddenIn<int?>, IForbiddenOut<int?>, ForbiddenCommand>(
                s => s.Forbidden());
        }
    }
}
