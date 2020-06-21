using System;

namespace Validot.MemoryLeak
{
    using System.Collections;
    using System.Collections.Generic;

    using Bogus;

    class Program
    {
        static int Main(string[] args)
        {
            Randomizer.Seed = new Random(666);

            var validator = Validator.Factory.Create(StreamDataSet.Specification);

            var value = "";
            
            foreach (var model in StreamDataSet.Faker.GenerateForever())
            {
                if (!validator.IsValid(model))
                {
                    var result = validator.Validate(model);

                    if (!result.AnyErrors)
                    {
                        continue;
                    }

                    value = result.Codes.Count.ToString() +
                            result.Paths.Count.ToString() +
                            result.CodeMap.Count.ToString() +
                            result.MessageMap.Count.ToString() +
                            result.TranslationNames.ToString() +
                            result.ToString();
                }

                value = null;
            }

            return value is null ? 0 : 1;
        }
    }
}
