namespace Validot.Benchmarks.Comparisons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    using Bogus;

    using FluentValidation;

    [MemoryDiagnoser]
    public class NoErrorsBenchmark
    {
        private IReadOnlyList<ComparisonSetup.FullModel> _noErrorsModels;

        private Validot.IValidator<ComparisonSetup.FullModel> _validotValidator;

        private ComparisonSetup.FullModelValidator _fluentValidationValidator;

        [Params(1000, 100000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            Randomizer.Seed = new Random(666);

            _noErrorsModels = ComparisonSetup.FullModelNoErrorsFaker.GenerateLazy(N).ToList();

            _fluentValidationValidator = new ComparisonSetup.FullModelValidator();

            _validotValidator = Validator.Factory.Create(ComparisonSetup.FullModelSpecification);
        }

        [Benchmark]
        public bool IsValid_FluentValidation()
        {   
            _fluentValidationValidator.CascadeMode = CascadeMode.StopOnFirstFailure;
            
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationValidator.Validate(_noErrorsModels[i]).IsValid;
            }
            
            return t;
        }
        
        [Benchmark]
        public bool IsValid_Validot()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotValidator.IsValid(_noErrorsModels[i]);
            }
            
            return t;
        }
    }
}
