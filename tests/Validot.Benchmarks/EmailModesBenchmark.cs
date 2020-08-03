namespace Validot.Benchmarks
{
    using System.Collections.Generic;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    using Bogus;
    using Validot.Results;

    [MemoryDiagnoser]
    public class EmailModesBenchmark
    {
        private IValidator<string> _complexRegexEmailValidator;

        private IValidator<string> _dataAnnotationsCompatibleEmailValidator;

        private IReadOnlyList<string> _emails;
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            _complexRegexEmailValidator = Validator.Factory.Create<string>(e => e.Email(EmailValidationMode.ComplexRegex));
            _dataAnnotationsCompatibleEmailValidator = Validator.Factory.Create<string>(e => e.Email(EmailValidationMode.DataAnnotationsCompatible));

            _emails = new Faker<EmailHolder>().RuleFor(m => m.Email, m => m.Person.Email).GenerateLazy(1_000_000).Select(m => m.Email).ToList();
        }

        [Benchmark]
        public object ComplexRegex()
        {
            IValidationResult t = null;
            
            for(var i = 0; i < _emails.Count; ++i)
            {
                t = _complexRegexEmailValidator.Validate(_emails[i]);
            }

            return t;
        }
        
        [Benchmark]
        public object DataAnnotationsCompatible()
        {
            IValidationResult t = null;
            
            for(var i = 0; i < _emails.Count; ++i)
            {
                t = _dataAnnotationsCompatibleEmailValidator.Validate(_emails[i]);
            }

            return t;
        }
        
        private class EmailHolder
        {
            public string Email { get; set; }
        }
    }
}
