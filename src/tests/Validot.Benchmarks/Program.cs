namespace Validot.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using BenchmarkDotNet.Running;

    using Bogus;

    using Validot.Benchmarks.Comparisons;

    class Program
    {
        static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

        // static void Main()
        // {
        //     ErrorMessagesDataTest();
        // }
        
        // static void Main()
        // {
        //     SetupStats();
        // }

        static void SetupStats()
        {
            int N = 10000;
            
            Randomizer.Seed = new Random(666);
            
            var fluentValidationValidator = new ComparisonDataSet.FullModelValidator();

            var validotValidator = Validator.Factory.Create(ComparisonDataSet.FullModelSpecification);

            var manyErrorsModels = ComparisonDataSet.FullModelManyErrorsFaker.GenerateLazy(N).ToList();
            var halfErrorsModels = ComparisonDataSet.FullModelHalfErrorsFaker.GenerateLazy(N).ToList();
            var noErrorsModels = ComparisonDataSet.FullModelNoErrorsFaker.GenerateLazy(N).ToList();

            var manyErrorsInvalidV = manyErrorsModels.Count(m => !validotValidator.IsValid(m));
            var manyErrorsInvalidFV = manyErrorsModels.Count(m => !fluentValidationValidator.Validate(m).IsValid);
            
            var halfErrorsInvalidV = halfErrorsModels.Count(m => !validotValidator.IsValid(m));
            var halfErrorsInvalidFV = halfErrorsModels.Count(m => !fluentValidationValidator.Validate(m).IsValid);

            var noErrorsInvalidV = noErrorsModels.Count(m => !validotValidator.IsValid(m));
            var noErrorsInvalidFV = noErrorsModels.Count(m => !fluentValidationValidator.Validate(m).IsValid);
            
            Console.WriteLine($"ManyErrors invalid: FV={manyErrorsInvalidFV} V={manyErrorsInvalidV}");
            Console.WriteLine($"HalfErrors invalid: FV={halfErrorsInvalidFV} V={halfErrorsInvalidV}");
            Console.WriteLine($"NoErrors invalid: FV={noErrorsInvalidFV} V={noErrorsInvalidV}");
        }

        static void ErrorMessagesDataTest()
        {
            int N = 1000;
            
            Randomizer.Seed = new Random(666);
            
            var fluentValidationValidator = new ComparisonDataSet.FullModelValidator();

            var validotValidator = Validator.Factory.Create(ComparisonDataSet.FullModelSpecification);

            var manyErrorsModels = ComparisonDataSet.FullModelManyErrorsFaker.GenerateLazy(N).ToList();
            
            var halfErrorsModels = ComparisonDataSet.FullModelHalfErrorsFaker.GenerateLazy(N).ToList();
           
            var noErrorsModels = ComparisonDataSet.FullModelNoErrorsFaker.GenerateLazy(N).ToList();
            
            WriteResults(manyErrorsModels, "many");
            WriteResults(halfErrorsModels, "half");
            WriteResults(noErrorsModels, "no");

            void WriteResults(IReadOnlyList<ComparisonDataSet.FullModel> fullModels, string name)
            {
                for(var i = 0; i < N; ++i)
                {
                    var validotIsValid = validotValidator.IsValid(fullModels[i]);
                
                    if (validotIsValid)
                    {
                        File.AppendAllText(GetFile($"{name}.validot.valid"), i + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(GetFile($"{name}.validot.invalid"), Environment.NewLine + $"-{i}-" + Environment.NewLine + validotValidator.Validate(fullModels[i]));
                    }
                    
                    var fluentValidationResult = fluentValidationValidator.Validate(fullModels[i]);
                    
                    if (fluentValidationResult.IsValid)
                    {
                        File.AppendAllText(GetFile($"{name}.fluent.valid"), i + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(GetFile($"{name}.fluent.invalid"), Environment.NewLine + Environment.NewLine + $"-{i}-" + Environment.NewLine + fluentValidationResult);
                    }
                }
            }
            
            string GetFile(string name)
            {
                return Path.Combine(Directory.GetCurrentDirectory(), $"{name}.{N}.txt");
            }
        }
    }
}
