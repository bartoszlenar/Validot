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

            public string Path { get; set; }
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
                    Path = null
                }
            };

            yield return new object[]
            {
                "S1",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithPath<T>(target, "name123");

                    return target;
                }),

                new ExpectedErrorSetup<T>()
                {
                    Path = "name123"
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

                    target = WithPath<T>(target, "name123");

                    return target;
                }),

                new ExpectedErrorSetup<T>()
                {
                    ShouldExecute = predicate,
                    Path = "name123"
                }
            };
        }

        private static dynamic WithPath<T>(dynamic api, string message)
        {
            if (api is IWithPathIn<T> withPathIn)
            {
                return WithPathExtension.WithPath<T>(withPathIn, message);
            }

            throw new InvalidOperationException("Dynamic api tests failed");
        }

        private static dynamic WithCondition<T>(dynamic api, Predicate<T> predicate)
        {
            if (api is IWithConditionIn<T> withConditionIn)
            {
                return WithConditionExtension.WithCondition<T>(withConditionIn, predicate);
            }

            throw new InvalidOperationException("Dynamic api tests failed");
        }
    }
}
