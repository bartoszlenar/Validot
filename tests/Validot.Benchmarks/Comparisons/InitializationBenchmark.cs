namespace Validot.Benchmarks.Comparisons
{
    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public class InitializationBenchmark
    {
        [Benchmark]
        public object Initialization_FluentValidation()
        {
            return new ComparisonSetup.FullModelValidator();
        }
        
        [Benchmark]
        public object Initialization_Validot()
        {
            return Validator.Factory.Create(ComparisonSetup.FullModelSpecification);
        }
    }
}
