namespace Validot.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Errors;
    using Validot.Errors.Args;

    public static class Tester
    {
        public static TestResult TestSpecification<T>(T value, Specification<T> specification, IReadOnlyDictionary<string, IReadOnlyCollection<IError>> expectedErrors = null)
        {
            var validator = Validator.Factory.Create(specification);

            var result = validator.Validate(value);

            var shouldBeValid = expectedErrors == null;

            if (result.IsValid && shouldBeValid)
            {
                return TestResult.Passed();
            }

            if (result.IsValid != shouldBeValid)
            {
                return TestResult.Failed($"Expected result IsValid: {shouldBeValid}, but found {result.IsValid}");
            }

            var errors = result.Details.GetRawErrors();

            if (expectedErrors == null)
            {
                if (errors.Count == 0)
                {
                    return TestResult.Passed();
                }

                return TestResult.Failed($"No errors expected, but found errors under {errors.Count} paths");
            }

            if (errors.Count != expectedErrors.Count)
            {
                return TestResult.Failed($"Expected amount of paths with errors: {expectedErrors.Count}, but found: {errors.Count}");
            }

            var missingPath = expectedErrors.Keys.FirstOrDefault(expectedKey =>
            {
                return errors.Keys.All(key => !string.Equals(expectedKey, key, StringComparison.Ordinal));
            });

            if (missingPath != null)
            {
                return TestResult.Failed($"Expected error path is missing: `{missingPath}`");
            }

            foreach (var errorPair in errors)
            {
                var path = errorPair.Key;

                var pathErrors = errorPair.Value;

                var pathExpectedErrors = expectedErrors[path];

                if (pathErrors.Count != pathExpectedErrors.Count)
                {
                    return TestResult.Failed($"Expected errors amount (for path `{path}`): {pathExpectedErrors.Count}, but found {pathErrors.Count}");
                }

                for (var j = 0; j < pathErrors.Count; ++j)
                {
                    var error = pathErrors.ElementAt(j);
                    var expectedError = pathExpectedErrors.ElementAt(j);

                    var expectedErrorMessagesCount = expectedError.Messages?.Count ?? 0;

                    if (error.Messages.Count != expectedErrorMessagesCount)
                    {
                        return TestResult.Failed($"Expected error (for path `{path}`, index {j}) messages amount to be {expectedErrorMessagesCount}, but found {error.Messages.Count}");
                    }

                    for (var k = 0; k < error.Messages.Count; ++k)
                    {
                        var errorMessage = error.Messages.ElementAt(k);

                        var expectedErrorMessage = expectedError.Messages.ElementAt(k);

                        if (!string.Equals(errorMessage, expectedErrorMessage, StringComparison.Ordinal))
                        {
                            return TestResult.Failed($"Expected error (for path `{path}`, index {j}) message (index {k}) to be `{expectedErrorMessage}`, but found `{errorMessage}`");
                        }
                    }

                    var expectedErrorCodesCount = expectedError.Codes?.Count ?? 0;

                    if (error.Codes.Count != expectedErrorCodesCount)
                    {
                        return TestResult.Failed($"Expected error (for path `{path}`, index {j}) codes amount to be {expectedErrorCodesCount}, but found {error.Codes.Count}");
                    }

                    for (var k = 0; k < error.Codes.Count; ++k)
                    {
                        var errorCode = error.Codes.ElementAt(k);

                        var expectedErrorCode = expectedError.Codes.ElementAt(k);

                        if (!string.Equals(errorCode, expectedErrorCode, StringComparison.Ordinal))
                        {
                            return TestResult.Failed($"Expected error (for path `{path}`, index {j}) code (index {k}) to be `{expectedErrorCode}`, but found `{errorCode}`");
                        }
                    }

                    var expectedErrorArgsCount = expectedError.Args?.Count ?? 0;

                    if (error.Args.Count != expectedErrorArgsCount)
                    {
                        return TestResult.Failed($"Expected error (for path `{path}`, index {j}) args amount to be {expectedErrorArgsCount}, but found {error.Args.Count}");
                    }

                    if (expectedErrorArgsCount == 0)
                    {
                        continue;
                    }

                    var missingArg = expectedError.Args.FirstOrDefault(a =>
                    {
                        return error.Args.All(ea => !string.Equals(a.Name, ea.Name, StringComparison.Ordinal));
                    });

                    if (missingArg != null)
                    {
                        return TestResult.Failed($"Expected error (for path `{path}`, index {j}) arg is missing: `{missingArg.Name}`");
                    }

                    foreach (var errorArg in error.Args)
                    {
                        var expectedErrorArg = expectedError.Args.Single(a => a.Name == errorArg.Name);

                        var argType = errorArg.GetType();

                        var expectedArgType = expectedErrorArg.GetType();

                        if (!expectedArgType.IsAssignableFrom(argType))
                        {
                            return TestResult.Failed($"Expected error (for path `{path}`, index {j}) arg (name `{errorArg.Name}`) type to be `{expectedArgType.GetFriendlyName(true)}`, but found `{argType.GetFriendlyName(true)}`");
                        }

                        var argValue = argType.GetProperties().Single(p => p.Name == "Value").GetValue(errorArg);
                        var expectedArgValue = argType.GetProperties().Single(p => p.Name == "Value").GetValue(expectedErrorArg);

                        if (argValue is double d)
                        {
                            if (Math.Abs(d - (double)expectedArgValue) > 0.0000001d)
                            {
                                return TestResult.Failed($"Expected error (for path `{path}`, index {j}) arg (name `{errorArg.Name}`) double value to be `{expectedArgValue}`, but found `{argValue}`");
                            }
                        }
                        else if (argValue is float f)
                        {
                            if (Math.Abs(f - (float)expectedArgValue) > 0.0000001f)
                            {
                                return TestResult.Failed($"Expected error (for path `{path}`, index {j}) arg (name `{errorArg.Name}`) float value to be `{expectedArgValue}`, but found `{argValue}`");
                            }
                        }
                        else if (!expectedArgValue.Equals(argValue))
                        {
                            return TestResult.Failed($"Expected error (for path `{path}`, index {j}) arg (name `{errorArg.Name}`) value to be `{expectedArgValue}`, but found `{argValue}`");
                        }
                    }
                }
            }

            return TestResult.Passed();
        }

        public static void TestSingleRule<T>(T value, Specification<T> specification, bool shouldBeValid, string message = null, params IArg[] args)
        {
            if (message == null && args?.Any() == true)
            {
                throw new ArgumentException($"{nameof(message)} cannot be null if {nameof(args)} is non-null", nameof(message));
            }

            var expectedErrors = shouldBeValid
                ? null
                : new Dictionary<string, IReadOnlyCollection<IError>>
                {
                    [string.Empty] = new[]
                    {
                        new Error
                        {
                            Messages = new[]
                            {
                                message
                            },
                            Args = args,
                            Codes = Array.Empty<string>()
                        }
                    }
                };

            TestSpecification(value, specification, expectedErrors).ThrowExceptionIfFailed();
        }

        public static Exception TestExceptionOnInit<TMember>(Specification<TMember> specification, Type expectedException)
        {
            ThrowHelper.NullArgument(specification, nameof(specification));
            ThrowHelper.NullArgument(expectedException, nameof(expectedException));

            try
            {
                Validator.Factory.Create(specification);
            }
            catch (Exception exception)
            {
                if (!expectedException.IsInstanceOfType(exception))
                {
                    throw new TestFailedException($"Exception of type {expectedException.FullName} was expected, but found {exception.GetType().FullName}.");
                }

                return exception;
            }

            throw new TestFailedException($"Exception of type {expectedException.FullName} was expected, but no exception has been thrown.");
        }
    }
}
