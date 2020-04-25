namespace Validot.Benchmarks.Comparisons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    using Bogus;
    using Bogus.Extensions;

    using FluentValidation;

    [MemoryDiagnoser]
    public class SimpleModelComparison
    {
        private IReadOnlyList<SimpleModel> _simpleModels;

        private Validot.IValidator<SimpleModel> _validotValidator;

        private SimpleModelValidator _fluentValidationValidator;

        public class SimpleModel
        {
            public string Name { get; set; }
            
            public string Email { get; set; }
            
            public string Password { get; set; }
        }
        
        public class SimpleModelValidator : AbstractValidator<SimpleModel> 
        {
            public SimpleModelValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
                RuleFor(x => x.Email).NotNull().EmailAddress();
                RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(8).MaximumLength(100);
            }
        }
        
        [Params(1, 1000, 10000, 100000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            Randomizer.Seed = new Random(666);

            var simpleModelFaker = new Faker<SimpleModel>()
                .RuleFor(m => m.Email, m => m.Internet.Email().OrNull(m, 0.1F))
                .RuleFor(m => m.Name, m => m.Name.FullName().OrNull(m, 0.2F))
                .RuleFor(m => m.Password, m => m.Random.String(length: 64).OrNull(m, 0.3F));

            _simpleModels = simpleModelFaker.GenerateLazy(N).ToList();

            _validotValidator = Validator.Factory.Create<SimpleModel>(_ => _
                .Member(m => m.Name, m => m.Optional().NotEmpty().MaxLength(50))
                .Member(m => m.Email, m => m.Email().MaxLength(100))
                .Member(m => m.Password, m => m.NotEmpty().MinLength(8).MaxLength(100))
            );
            
            _fluentValidationValidator = new SimpleModelValidator();
        }
        
        [Benchmark]
        public bool IsValid_FluentValidation()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationValidator.Validate(_simpleModels[i]).IsValid;
            }

            return t;
        }
        
        [Benchmark]
        public bool IsValid_Validot()
        {
            var t = true;
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotValidator.IsValid(_simpleModels[i]);
            }

            return t;
        }
        
        [Benchmark]
        public string Messages_FluentValidation()
        {
            var t = "";
            
            for(var i = 0; i < N; ++i)
            {
                t = _fluentValidationValidator.Validate(_simpleModels[i]).ToString();
            }

            return t;
        }
        
        [Benchmark]
        public string Messages_Validot()
        {
            var t = "";
            
            for(var i = 0; i < N; ++i)
            {
                t = _validotValidator.Validate(_simpleModels[i]).ToMessagesString();
            }

            return t;
        }
        
    }
}
