namespace Validot.Benchmarks.Comparisons
{
    using System.Collections.Generic;
    using BenchmarkDotNet.Attributes;
    using FluentValidation;

    [MemoryDiagnoser]
    public class ReportingBenchmark
    {
        private Validot.IValidator<ComparisonDataSet.FullModel> _validotValidator;

        private ComparisonDataSet.FullModelValidator _fluentValidationValidator;

        private IReadOnlyDictionary<string, IReadOnlyList<ComparisonDataSet.FullModel>> _dataSets;

        [Params("ManyErrors", "HalfErrors")]
        public string DataSet { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _fluentValidationValidator = new ComparisonDataSet.FullModelValidator();

            _validotValidator = Validator.Factory.Create(ComparisonDataSet.FullModelSpecification);
            
            _dataSets = ComparisonDataSet.DataSets;
        }
        
        [Benchmark]
        public object Reporting_FluentValidation()
        {
            var models = _dataSets[DataSet];
            
            var t = new object();
            
            for(var i = 0; i < models.Count; ++i)
            {
                var result = _fluentValidationValidator.Validate(models[i]);

                if (!result.IsValid)
                {
                    t = result.ToString();
                }
            }
            
            return t;
        }
        
        [Benchmark]
        public object Reporting_Validot()
        {
            var models = _dataSets[DataSet];
            
            var t = new object();
            
            for(var i = 0; i < models.Count; ++i)
            {
                var model = models[i];

                if (!_validotValidator.IsValid(model))
                {
                    t = _validotValidator.Validate(model).ToString();
                }
            }
            
            return t;
        }
    }
}
