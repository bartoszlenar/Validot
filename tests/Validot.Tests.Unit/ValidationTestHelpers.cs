namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Errors;
    using Validot.Results;
    using Validot.Validation;
    using Validot.Validation.Stacks;

    public static class ValidationTestHelpers
    {
        public static void ShouldHaveErrorMap<T>(this IValidator<T> validator, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> rawErrorsExpectations)
        {
            validator.ErrorsMap.Should().NotBeNull();

            var rawErrors = validator.ErrorsMap.Details.GetRawErrors();

            rawErrors.ShouldMatchExpectations(rawErrorsExpectations);
        }

        public static void ShouldValidateAndHaveResult<T>(this IValidator<T> validator, T model, bool failFast, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> rawErrorsExpectations, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            IValidationResult result = null;

            Action action = () =>
            {
                result = validator.Validate(model, failFast);
            };

            if (exceptionCase is null)
            {
                action.Should().NotThrow();

                var rawErrors = result.Details.GetRawErrors();

                rawErrors.ShouldMatchExpectations(rawErrorsExpectations);
            }
            else
            {
                var exceptionThrown = action.Should().ThrowExactly<ReferenceLoopException>().Which;

                exceptionThrown.Type.Should().Be(exceptionCase.Type);
                exceptionThrown.Path.Should().Be(exceptionCase.Path);
                exceptionThrown.NestedPath.Should().Be(exceptionCase.NestedPath);
            }
        }

        public static void ShouldHaveIsValueTrueIfNoErrors<T>(this IValidator<T> validator, T model, bool expectedIsValid, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            bool isValid = false;

            Action action = () =>
            {
                isValid = validator.IsValid(model);
            };

            if (exceptionCase is null)
            {
                action.Should().NotThrow();

                isValid.Should().Be(expectedIsValid);
            }
            else
            {
                var exceptionThrown = action.Should().ThrowExactly<ReferenceLoopException>().Which;

                exceptionThrown.Type.Should().Be(exceptionCase.Type);
                exceptionThrown.Path.Should().Be(exceptionCase.Path);
                exceptionThrown.NestedPath.Should().Be(exceptionCase.NestedPath);
            }
        }

        public static void ShouldMatchExpectations(this IReadOnlyDictionary<string, IReadOnlyList<IError>> output, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> test)
        {
            output.Should().NotBeNull();
            test.Should().NotBeNull();

            output.Should().HaveCount(test.Count);

            foreach (var testPair in test)
            {
                output.Keys.Should().Contain(testPair.Key);
                output[testPair.Key].Should().NotBeNull();
                output[testPair.Key].Should().HaveCount(testPair.Value.Count);

                for (var i = 0; i < testPair.Value.Count; ++i)
                {
                    var testMessages = testPair.Value[i].Messages;
                    var outputMessages = output[testPair.Key][i].Messages;

                    if (testMessages is null)
                    {
                        outputMessages.Should().BeNullOrEmpty();
                    }
                    else
                    {
                        outputMessages.Should().HaveCount(testMessages.Count);

                        for (var j = 0; j < testMessages.Count; ++j)
                        {
                            outputMessages[j].Should().Be(testMessages[j]);
                        }
                    }

                    var testCodes = testPair.Value[i].Codes;
                    var outputCodes = output[testPair.Key][i].Codes;

                    if (testCodes is null)
                    {
                        outputCodes.Should().BeNullOrEmpty();
                    }
                    else
                    {
                        outputCodes.Should().HaveCount(testCodes.Count);

                        for (var j = 0; j < testCodes.Count; ++j)
                        {
                            outputCodes[j].Should().Be(testCodes[j]);
                        }
                    }

                    var testArgs = testPair.Value[i].Args;
                    var outputArgs = output[testPair.Key][i].Args;

                    if (testArgs is null)
                    {
                        outputArgs.Should().BeNullOrEmpty();
                    }
                    else
                    {
                        outputArgs.Should().HaveCount(testArgs.Count);

                        for (var j = 0; j < testArgs.Count; ++j)
                        {
                            outputArgs[j].Name.Should().Be(testArgs[j].Name);

                            dynamic arg = outputArgs[j];

                            ((object)arg.Value.GetType()).Should().Be(((object)testArgs[j].Value).GetType());

                            ((object)arg.Value).Should().Be((object)testArgs[j].Value);
                        }
                    }
                }
            }
        }

        public static void ShouldMatchAmounts(this IErrorsHolder errorsHolder, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> test)
        {
            errorsHolder.Should().NotBeNull();
            errorsHolder.Errors.Should().NotBeNull();
            test.Should().NotBeNull();

            errorsHolder.Errors.Should().HaveCount(test.Count);

            foreach (var testPair in test)
            {
                errorsHolder.Errors.Keys.Should().Contain(testPair.Key);
                errorsHolder.Errors[testPair.Key].Should().NotBeNull();
                errorsHolder.Errors[testPair.Key].Should().HaveCount(testPair.Value.Count);
            }
        }
    }
}
