namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using Validot.Specification;

    public static class ErrorSetupApiHelper
    {
        public class ExpectedErrorSetup<T>
        {
            public Predicate<T> ShouldExecute { get; set; }

            public string Name { get; set; }
        }

        public static IEnumerable<object[]> AllCases<T>()
        {
            yield return new object[]
            {
                "S0",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    return target;
                }),

                new ExpectedErrorSetup<T>()
                {
                    Name = null
                }
            };

            yield return new object[]
            {
                "S1",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithName<T>(target, "name123");

                    return target;
                }),

                new ExpectedErrorSetup<T>()
                {
                    Name = "name123"
                }
            };

            Predicate<T> predicate = x => true;

            yield return new object[]
            {
                "S2",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithCondition<T>(target, predicate);

                    return target;
                }),

                new ExpectedErrorSetup<T>()
                {
                    ShouldExecute = predicate
                }
            };

            yield return new object[]
            {
                "S3",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithCondition<T>(target, predicate);

                    target = WithName<T>(target, "name123");

                    return target;
                }),

                new ExpectedErrorSetup<T>()
                {
                    ShouldExecute = predicate,
                    Name = "name123"
                }
            };
        }

        private static dynamic WithName<T>(dynamic api, string message)
        {
            if (api is IWithNameIn<T> withNameIn)
            {
                return WithNameExtension.WithName<T>(withNameIn, message);
            }

            throw new InvalidOperationException("Dynamic api tests failed");
        }

        private static dynamic WithCondition<T>(dynamic api, Predicate<T> predicate)
        {
            if (api is IWithConditionIn<T> withNameIn)
            {
                return WithConditionExtension.WithCondition<T>(withNameIn, predicate);
            }

            throw new InvalidOperationException("Dynamic api tests failed");
        }
    }
}
