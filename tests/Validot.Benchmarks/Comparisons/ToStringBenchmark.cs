namespace Validot.Benchmarks.Comparisons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    using Bogus;

    using FluentValidation;

    using Validot.Results;

    using ValidationResult = FluentValidation.Results.ValidationResult;

    [MemoryDiagnoser]
    public class ToStringBenchmark
    {
        private IReadOnlyDictionary<string, IReadOnlyList<IValidationResult>> _validotResults;
        private IReadOnlyDictionary<string, IReadOnlyList<ValidationResult>> _fluentValidationResults;
        
        [Params("ManyErrors", "HalfErrors", "NoErrors")]
        public string DataSet { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var fluentValidationValidator = new ComparisonDataSet.FullModelValidator();

            var validotValidator = Validator.Factory.Create(ComparisonDataSet.FullModelSpecification);

            _validotResults = new Dictionary<string, IReadOnlyList<IValidationResult>>()
            {
                ["ManyErrors"] = GetValidotResults(ComparisonDataSet.ManyErrorsDataSet),
                ["HalfErrors"] = GetValidotResults(ComparisonDataSet.HalfErrorsDataSet),
                ["NoErrors"] = GetValidotResults(ComparisonDataSet.NoErrorsDataSet),
            };
            
            _fluentValidationResults = new Dictionary<string, IReadOnlyList<ValidationResult>>()
            {
                ["ManyErrors"] = GetFluentValidationResults(ComparisonDataSet.ManyErrorsDataSet),
                ["HalfErrors"] = GetFluentValidationResults(ComparisonDataSet.HalfErrorsDataSet),
                ["NoErrors"] = GetFluentValidationResults(ComparisonDataSet.NoErrorsDataSet),
            };

            IReadOnlyList<IValidationResult> GetValidotResults(IReadOnlyList<ComparisonDataSet.FullModel> models) => models.Select(m => validotValidator.Validate(m)).ToList();
            IReadOnlyList<ValidationResult> GetFluentValidationResults(IReadOnlyList<ComparisonDataSet.FullModel> models) => models.Select(m => fluentValidationValidator.Validate(m)).ToList();
        }

        [Benchmark]
        public string ToString_FluentValidation()
        {
            var models = _fluentValidationResults[DataSet];
            
            var t = "";
            
            for(var i = 0; i < models.Count; ++i)
            {
                t = models[i].ToString();
            }

            return t;
        }
        
        [Benchmark]
        public string ToString_Validot()
        {
            var models = _validotResults[DataSet];
            
            var t = "";
            
            for(var i = 0; i < models.Count; ++i)
            {
                t = models[i].ToString();
            }

            return t;
        }
    }
}
