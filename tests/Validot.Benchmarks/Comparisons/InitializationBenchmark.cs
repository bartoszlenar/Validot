namespace Validot.Benchmarks.Comparisons
{
    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public class InitializationBenchmark
    {
        [Benchmark]
        public object Initialization_FluentValidation()
        {
            return new ComparisonDataSet.FullModelValidator();
        }
        
        [Benchmark]
        public object Initialization_Validot()
        {
            return Validator.Factory.Create(ComparisonDataSet.FullModelSpecification);
        }
    }
}
