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

        static void ErrorMessagesDataTest()
        {
            int N = 1000;
            
            Randomizer.Seed = new Random(666);
            
            var fluentValidationValidator = new ComparisonSetup.FullModelValidator();

            var validotValidator = Validator.Factory.Create(ComparisonSetup.FullModelSpecification);

            
            var manyErrorsModels = ComparisonSetup.FullModelManyErrorsFaker.GenerateLazy(N).ToList();
            
            var halfErrorsModels = ComparisonSetup.FullModelHalfErrorsFaker.GenerateLazy(N).ToList();
           
            var noErrorsModels = ComparisonSetup.FullModelNoErrorsFaker.GenerateLazy(N).ToList();
            
            WriteResults(manyErrorsModels, "many");
            WriteResults(halfErrorsModels, "half");
            WriteResults(noErrorsModels, "no");

            void WriteResults(IReadOnlyList<ComparisonSetup.FullModel> fullModels, string name)
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
