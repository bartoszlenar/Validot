namespace Validot.Tests.Unit.Specification
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class OptionalExtensionTests
    {
        [Fact]
        public void Should_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IOptionalOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IRuleIn<object>),
                typeof(IAndIn<object>)
            });
        }

        [Fact]
        public void Should_Add_OptionalCommand()
        {
            ApiTester.TestSingleCommand<object, IOptionalIn<object>, IOptionalOut<object>, OptionalCommand>(
                s => s.Optional());
        }

        [Fact]
        public void Should_Add_OptionalCommand_When_Nullable()
        {
            ApiTester.TestSingleCommand<int?, IOptionalIn<int?>, IOptionalOut<int?>, OptionalCommand>(
                s => s.Optional());
        }
    }
}
