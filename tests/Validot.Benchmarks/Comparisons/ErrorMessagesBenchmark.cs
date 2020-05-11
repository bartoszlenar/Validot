namespace Validot.Benchmarks.Comparisons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    using Bogus;

    using FluentValidation;

    [MemoryDiagnoser]
    public class ErrorMessagesBenchmark
    {
        private IReadOnlyList<ComparisonSetup.FullModel> _manyErrorsModels;
        
        private IReadOnlyList<ComparisonSetup.FullModel> _halfErrorsModels;

        private Validot.IValidator<ComparisonSetup.FullModel> _validotValidator;

        private ComparisonSetup.FullModelValidator _fluentValidationValidator;

        [Params(1000, 100000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            Randomizer.Seed = new Random(666);

            _manyErrorsModels = ComparisonSetup.FullModelManyErrorsFaker.GenerateLazy(N).ToList();
            _halfErrorsModels = ComparisonSetup.FullModelHalfErrorsFaker.GenerateLazy(N).ToList();

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
                t = _fluentValidationValidator.Validate(_manyErrorsModels[i]).IsValid;
            }
            
            return t;
        }
        
        [Benchmark]
        public bool IsValid_Validot()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotValidator.IsValid(_manyErrorsModels[i]);
            }
            
            return t;
        }
        
        [Benchmark]
        public string IsValidAndValidate_FluentValidation()
        {
            var t = "";
            
            for(var i = 0; i < N; ++i)
            {
                var result = _fluentValidationValidator.Validate(_halfErrorsModels[i]);
                
                if (!result.IsValid)
                {
                    t = result.ToString();
                }
            }

            return t;
        }
        
        [Benchmark]
        public string IsValidAndValidate_Validot()
        {
            var t = "";
            
            for(var i = 0; i < N; ++i)
            {
                if (_validotValidator.IsValid(_halfErrorsModels[i]))
                {
                    t = _validotValidator.Validate(_halfErrorsModels[i]).ToMessagesString(includePaths: false);
                }
            }

            return t;
        }

        [Benchmark]
        public string FullReport_FluentValidation()
        {
            var t = "";
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationValidator.Validate(_manyErrorsModels[i]).ToString();
            }

            return t;
        }
        
        [Benchmark]
        public string FullReport_Validot()
        {
            var t = "";
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotValidator.Validate(_manyErrorsModels[i]).ToMessagesString(includePaths: false);
            }

            return t;
        }
      
        [Benchmark]
        public string FailFast_FluentValidation()
        {
            _fluentValidationValidator.CascadeMode = CascadeMode.StopOnFirstFailure;
            
            var t = "";
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationValidator.Validate(_manyErrorsModels[i]).ToString();
            }

            return t;
        }
        
        [Benchmark]
        public string FailFast_Validot()
        {
            var t = "";
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotValidator.Validate(_manyErrorsModels[i], true).ToMessagesString(includePaths: false);
            }

            return t;
        }
    }
}
