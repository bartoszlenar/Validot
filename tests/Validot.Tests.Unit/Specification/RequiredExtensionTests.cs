namespace Validot.Tests.Unit.Specification
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class RequiredExtensionTests
    {
        [Fact]
        public void Should_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IRequiredOut<object>>(new[]
            {
                typeof(ISpecificationOut<object>),
                typeof(IRuleIn<object>),
                typeof(IWithMessageIn<object>),
                typeof(IWithExtraMessageIn<object>),
                typeof(IWithCodeIn<object>),
                typeof(IWithExtraCodeIn<object>),
            });
        }

        [Fact]
        public void Should_Required_Add_RequiredCommand()
        {
            ApiTester.TestSingleCommand<object, IRequiredIn<object>, IRequiredOut<object>, RequiredCommand>(
                s => s.Required());
        }

        [Fact]
        public void Should_Required_Add_RequiredCommand_When_Nullable()
        {
            ApiTester.TestSingleCommand<int?, IRequiredIn<int?>, IRequiredOut<int?>, RequiredCommand>(
                s => s.Required());
        }
    }
}
