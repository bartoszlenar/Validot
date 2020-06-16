namespace Validot.Benchmarks.Comparisons
{
    using System.Collections.Generic;
    using BenchmarkDotNet.Attributes;
    using FluentValidation;

    [MemoryDiagnoser]
    public class ValidationBenchmark
    {
        private Validot.IValidator<ComparisonDataSet.FullModel> _validotValidator;

        private ComparisonDataSet.FullModelValidator _fluentValidationValidator;

        private IReadOnlyDictionary<string, IReadOnlyList<ComparisonDataSet.FullModel>> _dataSets;

        [Params("ManyErrors", "HalfErrors", "NoErrors")]
        public string DataSet { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _fluentValidationValidator = new ComparisonDataSet.FullModelValidator();

            _validotValidator = Validator.Factory.Create(ComparisonDataSet.FullModelSpecification);
            
            _dataSets = ComparisonDataSet.DataSets;
        }

        [Benchmark]
        public bool IsValid_FluentValidation()
        {   
            _fluentValidationValidator.CascadeMode = CascadeMode.StopOnFirstFailure;
            var models = _dataSets[DataSet];
            
            var t = true;
            
            for (var i = 0; i < models.Count; ++i)
            {
                t = _fluentValidationValidator.Validate(models[i]).IsValid;
            }
            
            return t;
        }
        
        [Benchmark]
        public bool IsValid_Validot()
        {
            var models = _dataSets[DataSet];
            
            var t = true;
            
            for (var i = 0; i < models.Count; ++i)
            {
                t = _validotValidator.IsValid(models[i]);
            }
            
            return t;
        }
      
        [Benchmark]
        public object FailFast_FluentValidation()
        {
            _fluentValidationValidator.CascadeMode = CascadeMode.StopOnFirstFailure;
            var models = _dataSets[DataSet];

            object t = null;
            
            for (var i = 0; i < models.Count; ++i)
            {
                t = _fluentValidationValidator.Validate(models[i]);
            }

            return t;
        }
        
        [Benchmark]
        public object FailFast_Validot()
        {
            var models = _dataSets[DataSet];
            
            var t = new object();
            
            for (var i = 0; i < models.Count; ++i)
            {
                t = _validotValidator.Validate(models[i], true);
            }

            return t;
        }
        
        [Benchmark]
        public object Validate_FluentValidation()
        {
            var models = _dataSets[DataSet];
            
            var t = new object();
            
            for(var i = 0; i < models.Count; ++i)
            {
                t = _fluentValidationValidator.Validate(models[i]);
            }
            
            return t;
        }
        
        [Benchmark]
        public object Validate_Validot()
        {
            var models = _dataSets[DataSet];
            
            var t = new object();
            
            for(var i = 0; i < models.Count; ++i)
            {
                t = _validotValidator.Validate(models[i]);
            }
            
            return t;
        }
    }
}
