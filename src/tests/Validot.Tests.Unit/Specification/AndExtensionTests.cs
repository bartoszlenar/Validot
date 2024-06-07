namespace Validot.Tests.Unit.Specification
{
    using System;
    using FluentAssertions;
    using Validot.Specification;
    using Validot.Specification.Commands;
    using Xunit;

    public class AndExtensionTests
    {
        [Fact]
        public void Should_BeEntryPoint()
        {
            ApiTester.TestOutputPossibilities<IAndOut<object>>(new[]
            {
                typeof(IRuleIn<object>)
            });
        }

        [Fact]
        public void Should_NotAddAnyCommand()
        {
            Func<ISpecificationIn<object>, ISpecificationOut<object>> fluentApi = s => s.Optional().And().Rule(x => true).And().AsModel(m => m);

            var result = fluentApi(new SpecificationApi<object>());

            var processedApi = (SpecificationApi<object>)result;

            processedApi.Commands.Count.Should().Be(3);

            processedApi.Commands[0].Should().BeOfType<OptionalCommand>();
            processedApi.Commands[1].Should().BeOfType<RuleCommand<object>>();
            processedApi.Commands[2].Should().BeOfType<AsModelCommand<object>>();
        }
    }
}
