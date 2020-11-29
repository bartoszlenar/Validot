namespace Validot.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Errors;
    using Validot.Errors.Args;
    using Validot.Results;

    public static class Tester
    {
        public static TestResult TestSpecification<T>(T value, Specification<T> specification, IReadOnlyDictionary<string, IReadOnlyCollection<IError>> expectedErrors = null)
        {
            var validator = Validator.Factory.Create(specification);

            var result = (ValidationResult)validator.Validate(value);

            var shouldBeValid = expectedErrors == null;

            if (!result.AnyErrors && shouldBeValid)
            {
                return TestResult.Passed();
            }

            if (result.AnyErrors == shouldBeValid)
            {
                return TestResult.Failed($"Expected result IsValid: {shouldBeValid}, but AnyErrors: {result.AnyErrors}");
            }

            var errors = result.GetErrorOutput();

            if (expectedErrors == null)
            {
                ThrowHelper.Fatal("By this point all of the checks in TestSpecification method should prevent this logical path.");

                return TestResult.Failed(null);
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
                _ = Validator.Factory.Create(specification);
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

        public static TestResult TestResultToString(string toStringOutput, ToStringContentType toStringContentType, params string[] expectedLines)
        {
            ThrowHelper.NullArgument(toStringOutput, nameof(toStringOutput));
            ThrowHelper.NullArgument(expectedLines, nameof(expectedLines));

            if (expectedLines.Length == 0)
            {
                throw new ArgumentException("Empty list of expected lines", nameof(expectedLines));
            }

            if (toStringContentType == ToStringContentType.Codes)
            {
                if (expectedLines.Length != 1)
                {
                    throw new ArgumentException($"Expected codes only (all in the single line), but found lines: {expectedLines.Length}", nameof(expectedLines));
                }
            }

            if (toStringContentType == ToStringContentType.MessagesAndCodes)
            {
                if (expectedLines.Length < 3)
                {
                    throw new ArgumentException($"Expected codes and messages (so at least 3 lines), but found lines: {expectedLines.Length}", nameof(expectedLines));
                }

                if (!string.IsNullOrEmpty(expectedLines[1]))
                {
                    throw new ArgumentException($"Expected codes and messages (divided by a single empty line), but found in second line: {expectedLines[1]}", nameof(expectedLines));
                }

                if (expectedLines.Skip(2).Any(string.IsNullOrEmpty))
                {
                    throw new ArgumentException($"Expected codes and messages (divided by a single empty line), also another empty line", nameof(expectedLines));
                }
            }

            if (toStringContentType == ToStringContentType.Messages)
            {
                if (expectedLines.Any(string.IsNullOrEmpty))
                {
                    throw new ArgumentException($"Expected messages only, but found empty line", nameof(expectedLines));
                }
            }

            var hasCodes = toStringContentType == ToStringContentType.Codes || toStringContentType == ToStringContentType.MessagesAndCodes;

            var lines = toStringOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            if (lines.Length != expectedLines.Length)
            {
                return TestResult.Failed($"Expected amount of lines: {expectedLines.Length}, but found: {lines.Length}");
            }

            if (hasCodes)
            {
                var codes = lines[0].Split(new[] { ", " }, StringSplitOptions.None);

                var expectedCodes = expectedLines[0].Split(new[] { ", " }, StringSplitOptions.None);

                var missingCodes = expectedCodes.Where(expectedCode => codes.All(c => !string.Equals(c, expectedCode, StringComparison.Ordinal))).OrderBy(a => a).ToArray();

                if (missingCodes.Any())
                {
                    return TestResult.Failed($"Expected codes that are missing: {string.Join(", ", missingCodes)}");
                }

                if (codes.Length != expectedCodes.Length)
                {
                    return TestResult.Failed($"Expected amount of codes: {expectedCodes.Length}, but found: {codes.Length}");
                }
            }

            if (toStringContentType == ToStringContentType.MessagesAndCodes)
            {
                if (!string.IsNullOrEmpty(lines[1]))
                {
                    return TestResult.Failed($"Expected codes and messages (divided by a single line), but found in second line: {lines[1]}");
                }
            }

            var messageLines = toStringContentType == ToStringContentType.Messages
                ? lines
                : lines.Skip(2).ToArray();

            var expectedMessageLines = toStringContentType == ToStringContentType.Messages
                ? expectedLines
                : expectedLines.Skip(2).ToArray();

            var missingMessages = expectedMessageLines.Where(expectedMessageLine => messageLines.All(c => !string.Equals(c, expectedMessageLine, StringComparison.Ordinal))).ToArray();

            if (missingMessages.Any())
            {
                return TestResult.Failed($"Expected messages that are missing: {string.Join(", ", missingMessages.OrderBy(s => s).Select(s => $"`{s}`"))}");
            }

            return TestResult.Passed();
        }

        public static void ShouldResultToStringHaveLines(this string @this, ToStringContentType toStringContentType, params string[] expectedLines) => TestResultToString(@this, toStringContentType, expectedLines).ThrowExceptionIfFailed();
    }
}
