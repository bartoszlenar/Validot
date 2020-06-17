namespace Validot.Benchmarks.Comparisons
{
    using System.Collections.Generic;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    using FluentValidation;

    [MemoryDiagnoser]
    public class EngineOnlyBenchmark
    {
        private IReadOnlyList<VoidModel> _noLogicModels;
        
        private Validot.IValidator<VoidModel> _validotSingleRuleValidator;

        private Validot.IValidator<VoidModel> _validotTenRulesValidator;
        
        private NoLogicModelSingleRuleValidator _fluentValidationSingleRuleValidator;

        private NoLogicModelTenRulesValidator _fluentValidationTenRulesValidator;

        public class VoidModel
        {
            public object Member { get; set; }
        }
        
        public class NoLogicModelSingleRuleValidator : AbstractValidator<VoidModel> 
        {
            public NoLogicModelSingleRuleValidator()
            {
                RuleFor(m => m.Member).Must(o => true);
            }
        }
        
        public class NoLogicModelTenRulesValidator : AbstractValidator<VoidModel> 
        {
            public NoLogicModelTenRulesValidator()
            {
                RuleFor(m => m.Member).Must(o => true);
                RuleFor(m => m.Member).Must(o => true);
                RuleFor(m => m.Member).Must(o => true);
                RuleFor(m => m.Member).Must(o => true);
                RuleFor(m => m.Member).Must(o => true);
                RuleFor(m => m.Member).Must(o => true);
                RuleFor(m => m.Member).Must(o => true);
                RuleFor(m => m.Member).Must(o => true);
                RuleFor(m => m.Member).Must(o => true);
                RuleFor(m => m.Member).Must(o => true);
            }
        }
        
        [Params(10000)]
        public int N { get; set; }
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            _validotSingleRuleValidator = Validator.Factory.Create<VoidModel>(_ => _
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
            );
            
            _validotTenRulesValidator = Validator.Factory.Create<VoidModel>(_ => _
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
                .Member(m => m.Member, m => m.Optional().Rule(n => true))
            );

            _fluentValidationSingleRuleValidator = new NoLogicModelSingleRuleValidator();
            _fluentValidationTenRulesValidator = new NoLogicModelTenRulesValidator();
            
            _noLogicModels = Enumerable.Range(0, N).Select(m => new VoidModel() { Member = new object() }).ToList();
        }
        
        [Benchmark]
        public bool IsValid_SingleRule_FluentValidation()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationSingleRuleValidator.Validate(_noLogicModels[i]).IsValid;
            }

            return t;
        }
        
        [Benchmark]
        public bool IsValid_TenRules_FluentValidation()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationTenRulesValidator.Validate(_noLogicModels[i]).IsValid;
            }

            return t;
        }
        
        [Benchmark]
        public bool IsValid_SingleRule_Validot()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotSingleRuleValidator.IsValid(_noLogicModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public bool IsValid_TenRules_Validot()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotTenRulesValidator.IsValid(_noLogicModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public object Validate_SingleRule_FluentValidation()
        {
            object t = null;
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationSingleRuleValidator.Validate(_noLogicModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public object Validate_TenRules_FluentValidation()
        {
            object t = null;
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationTenRulesValidator.Validate(_noLogicModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public object Validate_SingleRule_Validot()
        {
            object t = null;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotSingleRuleValidator.Validate(_noLogicModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public object Validate_TenRules_Validot()
        {
            object t = null;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotSingleRuleValidator.Validate(_noLogicModels[i]);
            }

            return t;
        }
    }
}
