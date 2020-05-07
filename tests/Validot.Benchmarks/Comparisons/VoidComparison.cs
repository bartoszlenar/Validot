namespace Validot.Benchmarks.Comparisons
{
    using System.Collections.Generic;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    using FluentValidation;

    [MemoryDiagnoser]
    public class VoidComparison
    {
        private IReadOnlyList<VoidModel> _voidModels;
        
        private Validot.IValidator<VoidModel> _validotSingleRuleValidator;

        private Validot.IValidator<VoidModel> _validotTenRulesValidator;
        
        private VoidModelSingleRuleValidator _fluentValidationSingleRuleValidator;

        private VoidModelTenRulesValidator _fluentValidationTenRulesValidator;

        public class VoidModel
        {
            public object Member { get; set; }
        }
        
        public class VoidModelSingleRuleValidator : AbstractValidator<VoidModel> 
        {
            public VoidModelSingleRuleValidator()
            {
                RuleFor(m => m.Member).Must(o => true);
            }
        }
        
        public class VoidModelTenRulesValidator : AbstractValidator<VoidModel> 
        {
            public VoidModelTenRulesValidator()
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
        
        [Params(1000, 100000)]
        public int N { get; set; }
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            _validotSingleRuleValidator = Validator.Factory.Create<VoidModel>(_ => _
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
            );
            
            _validotTenRulesValidator = Validator.Factory.Create<VoidModel>(_ => _
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
                .Member(m => m.Member, m => m.Optional().Rule(m => true))
            );

            _fluentValidationSingleRuleValidator = new VoidModelSingleRuleValidator();
            _fluentValidationTenRulesValidator = new VoidModelTenRulesValidator();
            
            _voidModels = Enumerable.Range(0, N).Select(m => new VoidModel() { Member = new object() }).ToList();
        }
        
        [Benchmark]
        public bool IsValid_SingleRule_FluentValidation()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationSingleRuleValidator.Validate(_voidModels[i]).IsValid;
            }

            return t;
        }
        
        [Benchmark]
        public bool IsValid_TenRules_FluentValidation()
        {
            _fluentValidationTenRulesValidator.CascadeMode = CascadeMode.StopOnFirstFailure;
            
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationTenRulesValidator.Validate(_voidModels[i]).IsValid;
            }

            return t;
        }
        
        [Benchmark]
        public bool IsValid_SingleRule_Validot()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotSingleRuleValidator.IsValid(_voidModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public bool IsValid_TenRules_Validot()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotTenRulesValidator.IsValid(_voidModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public object Validate_SingleRule_FluentValidation()
        {
            object t = null;
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationSingleRuleValidator.Validate(_voidModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public object Validate_TenRules_FluentValidation()
        {
            object t = null;
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationTenRulesValidator.Validate(_voidModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public object Validate_SingleRule_Validot()
        {
            object t = null;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotSingleRuleValidator.Validate(_voidModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public object Validate_TenRules_Validot()
        {
            object t = null;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotSingleRuleValidator.Validate(_voidModels[i]);
            }

            return t;
        }
    }
}
