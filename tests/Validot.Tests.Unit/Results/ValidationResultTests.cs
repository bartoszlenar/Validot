namespace Validot.Tests.Unit.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Errors;
    using Validot.Results;

    using Xunit;

    public class ValidationResultTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), Substitute.For<IMessagesService>());
        }

        [Fact]
        public void Details_Should_NotBeNull()
        {
            var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), Substitute.For<IMessagesService>());

            validationResult.Details.Should().NotBeNull();
        }

        [Fact]
        public void Details_Should_BeSelf()
        {
            var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), Substitute.For<IMessagesService>());

            validationResult.Details.Should().BeSameAs(validationResult);
        }

        [Fact]
        public void AnyErrors_Should_BeFalse_When_NoErrors()
        {
            var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), Substitute.For<IMessagesService>());

            validationResult.AnyErrors.Should().BeFalse();
        }

        [Fact]
        public void AnyErrors_Should_BeTrue_When_ResultContainsErrors()
        {
            var resultErrors = new Dictionary<string, List<int>>();
            resultErrors.Add("path", new List<int>() { 1 });

            var validationResult = new ValidationResult(resultErrors, new Dictionary<int, IError>(), Substitute.For<IMessagesService>());

            validationResult.AnyErrors.Should().BeTrue();
        }

        [Fact]
        public void NoErrorsResult_Should_BeResultWithoutErrors()
        {
            ValidationResult.NoErrorsResult.AnyErrors.Should().BeFalse();
            ValidationResult.NoErrorsResult.Paths.Should().BeEmpty();
            ValidationResult.NoErrorsResult.TranslationNames.Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetErrorCodes().Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetErrorCodeList().Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetErrorMessages().Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetErrorOutput().Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetTranslation(null).Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetTranslation("English").Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetErrorMessages().Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetErrorMessages("English").Should().BeEmpty();
        }

        public static IEnumerable<object[]> Paths_Should_ReturnAllPaths_Data()
        {
            yield return new object[]
            {
                new Dictionary<string, List<int>>(),
                Array.Empty<string>()
            };

            yield return new object[]
            {
                new Dictionary<string, List<int>>()
                {
                    ["test1"] = new List<int>(),
                },
                new[] { "test1" }
            };

            yield return new object[]
            {
                new Dictionary<string, List<int>>()
                {
                    ["test1"] = new List<int>(),
                    ["test2"] = new List<int>(),
                    ["nested.test3"] = new List<int>()
                },
                new[] { "test1", "test2", "nested.test3" }
            };
        }

        [Theory]
        [MemberData(nameof(Paths_Should_ReturnAllPaths_Data))]
        public void Paths_Should_ReturnAllPaths(Dictionary<string, List<int>> resultsErrors, IReadOnlyList<string> expectedPaths)
        {
            var validationResult = new ValidationResult(resultsErrors, new Dictionary<int, IError>(), Substitute.For<IMessagesService>());

            validationResult.Paths.Should().NotBeNull();
            validationResult.Paths.Should().HaveCount(expectedPaths.Count);

            foreach (var expectedPath in expectedPaths)
            {
                validationResult.Paths.Should().Contain(expectedPath);
            }
        }

        public class TranslationNames
        {
            [Fact]
            public void Should_Return_TranslationNames_FromMessageService()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var translationNames = new[]
                {
                    "translation1",
                    "translation2"
                };

                messagesService.TranslationNames.Returns(translationNames);

                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), messagesService);

                validationResult.TranslationNames.Should().BeSameAs(translationNames);
            }

            [Fact]
            public void Should_Return_EmptyTranslationNames_When_NullTranslationName_InMessageService()
            {
                var messagesService = Substitute.For<IMessagesService>();

                messagesService.TranslationNames.Returns(null as IReadOnlyList<string>);

                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

                validationResult.TranslationNames.Should().BeEmpty();
            }

            [Fact]
            public void Should_Return_EmptyTranslationNames_When_NullMessageService()
            {
                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

                validationResult.TranslationNames.Should().BeEmpty();
            }
        }

        public class GetErrorMessages
        {
            [Fact]
            public void Should_Return_ErrorMessages_FromMessageService_WithDefaultTranslation()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var errorMessages = new Dictionary<string, IReadOnlyList<string>>
                {
                    ["path1"] = new[] { "message11" },
                    ["path2"] = new[] { "message12", "message22" }
                };

                var resultErrors = new Dictionary<string, List<int>>()
                {
                    ["path1"] = new List<int>() { 1 }
                };

                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string)).Returns(errorMessages);

                var validationResult = new ValidationResult(resultErrors, new Dictionary<int, IError>(), messagesService);

                var resultErrorMessages = validationResult.GetErrorMessages();

                messagesService.Received(1).GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string));
                messagesService.ReceivedWithAnyArgs(1).GetErrorsMessages(default);

                resultErrorMessages.Should().BeSameAs(errorMessages);
            }

            [Fact]
            public void Should_Return_ErrorMessages_FromMessageService_WithSpecifiedTranslation()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var errorMessages1 = new Dictionary<string, IReadOnlyList<string>>
                {
                    ["path1"] = new[] { "message11" },
                    ["path2"] = new[] { "message12", "message22" }
                };

                var errorMessages2 = new Dictionary<string, IReadOnlyList<string>>
                {
                    ["path1"] = new[] { "MESSAGE11" },
                    ["path2"] = new[] { "MESSAGE12", "MESSAGE22" }
                };

                var resultErrors = new Dictionary<string, List<int>>()
                {
                    ["path1"] = new List<int>() { 1 }
                };

                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation1")).Returns(errorMessages1);
                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2")).Returns(errorMessages2);

                var validationResult = new ValidationResult(resultErrors, new Dictionary<int, IError>(), messagesService);

                var resultErrorMessages = validationResult.GetErrorMessages("translation2");

                messagesService.Received(1).GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2"));
                messagesService.ReceivedWithAnyArgs(1).GetErrorsMessages(default);

                resultErrorMessages.Should().BeSameAs(errorMessages2);
            }

            [Fact]
            public void Should_Return_EmptyErrorMessages_When_Valid()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

                var resultErrorMessages = validationResult.GetErrorMessages();

                messagesService.DidNotReceiveWithAnyArgs().GetErrorsMessages(default);

                resultErrorMessages.Should().NotBeNull();
                resultErrorMessages.Should().BeEmpty();
            }
        }

        public class GetErrorCodes
        {
            [Fact]
            public void Should_Return_EmptyErrorCodes_When_Valid()
            {
                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

                var resultErrorCodes = validationResult.GetErrorCodes();

                resultErrorCodes.Should().NotBeNull();
                resultErrorCodes.Should().BeEmpty();
            }

            [Fact]
            public void Should_Return_AllErrorCodes()
            {
                var resultsErrors = new Dictionary<string, List<int>>()
                {
                    [""] = new List<int>() { 1 },
                    ["test1"] = new List<int>() { 1, 2, 3 },
                    ["test2"] = new List<int>() { 2, 4 },
                    ["test3"] = new List<int>() { 3 },
                };

                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error()
                    {
                        Codes = new[] { "Message1", }
                    },
                    [2] = new Error()
                    {
                        Codes = new[] { "Message2", }
                    },
                    [3] = new Error()
                    {
                        Codes = new[] { "Message3", }
                    },
                    [4] = new Error()
                    {
                        Codes = new[] { "Message41", "Message42" }
                    },
                };

                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessagesService>());

                var errorCodes = validationResult.GetErrorCodes();

                errorCodes.Should().NotBeNull();

                errorCodes.Should().HaveCount(4);

                errorCodes.Keys.Should().Contain("");
                errorCodes[""].Should().HaveCount(1);
                errorCodes[""].Should().Contain("Message1");

                errorCodes.Keys.Should().Contain("test1");
                errorCodes["test1"].Should().HaveCount(3);
                errorCodes["test1"].Should().Contain("Message1", "Message2", "Message3");

                errorCodes.Keys.Should().Contain("test2");
                errorCodes["test2"].Should().HaveCount(3);
                errorCodes["test2"].Should().Contain("Message2", "Message41", "Message42");

                errorCodes.Keys.Should().Contain("test3");
                errorCodes["test3"].Should().HaveCount(1);
                errorCodes["test3"].Should().Contain("Message3");
            }

            public static IEnumerable<object[]> Should_Return_AllErrorCodes_MoreExamples_Data()
            {
                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error()
                    {
                        Codes = new[] { "CODE1", }
                    },
                    [2] = new Error()
                    {
                        Codes = new[] { "CODE2", }
                    },
                    [3] = new Error()
                    {
                        Codes = new[] { "CODE3", }
                    },
                    [4] = new Error()
                    {
                        Codes = new[] { "CODE41", "CODE42" }
                    },
                    [5] = new Error()
                    {
                    },
                    [6] = new Error()
                    {
                        Codes = new[] { "CODE61", "CODE62", "CODE63" }
                    },
                    [10] = new Error()
                    {
                        Codes = new[] { "CODE1", "CODE2" }
                    },
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 1 },
                    },
                    errorRegistry,
                    new Dictionary<string, IReadOnlyList<string>>()
                    {
                        ["test1"] = new[] { "CODE1" }
                    }
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 1 },
                        ["test2"] = new List<int>() { 2, 4 },
                        ["test3"] = new List<int>() { 3, 4 },
                    },
                    errorRegistry,
                    new Dictionary<string, IReadOnlyList<string>>()
                    {
                        ["test1"] = new[] { "CODE1" },
                        ["test2"] = new[] { "CODE2", "CODE41", "CODE42" },
                        ["test3"] = new[] { "CODE3", "CODE41", "CODE42" }
                    }
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 5 },
                        ["test2"] = new List<int>() { 2, 5 },
                        ["test3"] = new List<int>() { 3, 5 },
                        ["test4"] = new List<int>() { 6, 5 },
                    },
                    errorRegistry,
                    new Dictionary<string, IReadOnlyList<string>>()
                    {
                        ["test2"] = new[] { "CODE2", },
                        ["test3"] = new[] { "CODE3", },
                        ["test4"] = new[] { "CODE61", "CODE62", "CODE63", },
                    }
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 5, 10 },
                        ["test2"] = new List<int>() { 2, 5 },
                        ["test3"] = new List<int>() { 3, 6 },
                        ["test4"] = new List<int>() { 5 },
                        ["test5"] = new List<int>() { 5 },
                    },
                    errorRegistry,
                    new Dictionary<string, IReadOnlyList<string>>()
                    {
                        ["test1"] = new[] { "CODE1", "CODE2" },
                        ["test2"] = new[] { "CODE2", },
                        ["test3"] = new[] { "CODE3", "CODE61", "CODE62", "CODE63", },
                    }
                };
            }

            [Theory]
            [MemberData(nameof(Should_Return_AllErrorCodes_MoreExamples_Data))]
            public void Should_Return_AllErrorCodes_MoreExamples(Dictionary<string, List<int>> resultsErrors, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<string>> expectedCodes)
            {
                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessagesService>());

                var errorCodes = validationResult.GetErrorCodes();

                errorCodes.Should().NotBeNull();

                errorCodes.Should().HaveCount(expectedCodes.Count);

                foreach (var pair in expectedCodes)
                {
                    errorCodes.Keys.Should().Contain(pair.Key);
                    errorCodes[pair.Key].Should().HaveCount(pair.Value.Count);
                    errorCodes[pair.Key].Should().Contain(pair.Value);
                }
            }
        }

        public class GetErrorCodeList
        {
            [Fact]
            public void Should_ReturnAllErrorCodesFromErrors()
            {
                var resultsErrors = new Dictionary<string, List<int>>()
                {
                    ["test1"] = new List<int>() { 1, 2, 3 },
                    ["test2"] = new List<int>() { 2, 4 },
                };

                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error()
                    {
                        Codes = new[] { "CODE1", }
                    },
                    [2] = new Error()
                    {
                        Codes = new[] { "CODE2", }
                    },
                    [3] = new Error()
                    {
                        Codes = new[] { "CODE3", }
                    },
                    [4] = new Error()
                    {
                        Codes = new[] { "CODE41", "CODE42" }
                    },
                };

                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessagesService>());

                var errorCodes = validationResult.GetErrorCodeList();

                errorCodes.Should().NotBeNull();

                errorCodes.Should().HaveCount(5);

                errorCodes.Should().Contain("CODE1");
                errorCodes.Should().Contain("CODE2");
                errorCodes.Should().Contain("CODE3");
                errorCodes.Should().Contain("CODE41");
                errorCodes.Should().Contain("CODE42");
            }

            public static IEnumerable<object[]> Should_ReturnAllErrorCodesFromErrors_MoreExamples_Data()
            {
                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error()
                    {
                        Codes = new[] { "CODE1", }
                    },
                    [2] = new Error()
                    {
                        Codes = new[] { "CODE2", }
                    },
                    [3] = new Error()
                    {
                        Codes = new[] { "CODE3", }
                    },
                    [4] = new Error()
                    {
                        Codes = new[] { "CODE41", "CODE42" }
                    },
                    [5] = new Error()
                    {
                    },
                    [6] = new Error()
                    {
                        Codes = new[] { "CODE61", "CODE62", "CODE63" }
                    },
                    [10] = new Error()
                    {
                        Codes = new[] { "CODE1", "CODE2" }
                    },
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 1 },
                    },
                    errorRegistry,
                    new[] { "CODE1" }
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 1, 1, 1 },
                    },
                    errorRegistry,
                    new[] { "CODE1" }
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 1 },
                        ["test2"] = new List<int>() { 1 },
                        ["test3"] = new List<int>() { 1 },
                    },
                    errorRegistry,
                    new[] { "CODE1" }
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 1 },
                        ["test2"] = new List<int>() { 2, 4 },
                        ["test3"] = new List<int>() { 3, 4 },
                    },
                    errorRegistry,
                    new[] { "CODE1", "CODE2", "CODE3", "CODE41", "CODE42" }
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 5 },
                        ["test2"] = new List<int>() { 2, 5 },
                        ["test3"] = new List<int>() { 3, 5 },
                        ["test4"] = new List<int>() { 6, 5 },
                    },
                    errorRegistry,
                    new[] { "CODE2", "CODE3", "CODE61", "CODE62", "CODE63" }
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 5, 10 },
                        ["test2"] = new List<int>() { 2, 5 },
                        ["test3"] = new List<int>() { 3, 6 },
                        ["test4"] = new List<int>() { 5 },
                        ["test5"] = new List<int>() { 5 },
                    },
                    errorRegistry,
                    new[] { "CODE1", "CODE2", "CODE3", "CODE61", "CODE62", "CODE63" }
                };
            }

            [Theory]
            [MemberData(nameof(Should_ReturnAllErrorCodesFromErrors_MoreExamples_Data))]
            public void Should_ReturnAllErrorCodesFromErrors_MoreExamples(Dictionary<string, List<int>> resultsErrors, Dictionary<int, IError> errorRegistry, IReadOnlyList<string> expectedCodes)
            {
                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessagesService>());

                var errorCodes = validationResult.GetErrorCodeList();

                errorCodes.Should().NotBeNull();

                errorCodes.Should().HaveCount(expectedCodes.Count);
                errorCodes.Should().Contain(expectedCodes);
                expectedCodes.Should().Contain(errorCodes);
            }

            [Fact]
            public void Should_ReturnEmptyList_When_Valid()
            {
                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), Substitute.For<IMessagesService>());

                var errorCodes = validationResult.GetErrorCodes();

                errorCodes.Should().NotBeNull();
                errorCodes.Should().BeEmpty();
            }
        }

        public class GetErrorOutput
        {
            [Fact]
            public void Should_ReturnErrorOutput()
            {
                var resultsErrors = new Dictionary<string, List<int>>()
                {
                    ["test1"] = new List<int>() { 1, 2, 3 },
                    ["test2"] = new List<int>() { 2, 4 },
                };

                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error(),
                    [2] = new Error(),
                    [3] = new Error(),
                    [4] = new Error(),
                };

                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessagesService>());

                var rawErrors = validationResult.GetErrorOutput();

                rawErrors.Should().NotBeNull();

                rawErrors.Keys.Should().HaveCount(2);

                rawErrors["test1"].Should().HaveCount(3);
                rawErrors["test1"].Should().Contain(x => ReferenceEquals(x, errorRegistry[1]));
                rawErrors["test1"].Should().Contain(x => ReferenceEquals(x, errorRegistry[2]));
                rawErrors["test1"].Should().Contain(x => ReferenceEquals(x, errorRegistry[3]));

                rawErrors["test2"].Should().HaveCount(2);
                rawErrors["test2"].Should().Contain(x => ReferenceEquals(x, errorRegistry[2]));
                rawErrors["test2"].Should().Contain(x => ReferenceEquals(x, errorRegistry[4]));
            }

            public static IEnumerable<object[]> Should_ReturnErrorOutput_MoreExamples_Data()
            {
                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error(),
                    [2] = new Error(),
                    [3] = new Error(),
                    [4] = new Error(),
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 1 },
                    },
                    errorRegistry,
                    new Dictionary<string, IReadOnlyList<IError>>()
                    {
                        ["test1"] = new[] { errorRegistry[1] }
                    },
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        [""] = new List<int>() { 4 },
                    },
                    errorRegistry,
                    new Dictionary<string, IReadOnlyList<IError>>()
                    {
                        [""] = new[] { errorRegistry[4] }
                    },
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>(),
                    },
                    errorRegistry,
                    new Dictionary<string, IReadOnlyList<IError>>()
                    {
                        ["test1"] = new IError[] { },
                    },
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 1 },
                        ["test2"] = new List<int>() { 2 },
                        ["test3"] = new List<int>() { 3 },
                        ["test4"] = new List<int>() { 4 },
                    },
                    errorRegistry,
                    new Dictionary<string, IReadOnlyList<IError>>()
                    {
                        ["test1"] = new[] { errorRegistry[1] },
                        ["test2"] = new[] { errorRegistry[2] },
                        ["test3"] = new[] { errorRegistry[3] },
                        ["test4"] = new[] { errorRegistry[4] },
                    },
                };

                yield return new object[]
                {
                    new Dictionary<string, List<int>>()
                    {
                        ["test1"] = new List<int>() { 1 },
                        ["test2"] = new List<int>() { 1, 2 },
                        ["test3"] = new List<int>() { 1, 3 },
                        ["test4"] = new List<int>() { 2, 3, 4 },
                    },
                    errorRegistry,
                    new Dictionary<string, IReadOnlyList<IError>>()
                    {
                        ["test1"] = new[] { errorRegistry[1] },
                        ["test2"] = new[] { errorRegistry[1], errorRegistry[2] },
                        ["test3"] = new[] { errorRegistry[1], errorRegistry[3] },
                        ["test4"] = new[] { errorRegistry[2], errorRegistry[3], errorRegistry[4] },
                    },
                };
            }

            [Theory]
            [MemberData(nameof(Should_ReturnErrorOutput_MoreExamples_Data))]
            public void Should_ReturnRawErrors_MoreExamples(Dictionary<string, List<int>> resultsErrors, Dictionary<int, IError> errorRegistry, IReadOnlyDictionary<string, IReadOnlyList<IError>> expectedErrors)
            {
                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessagesService>());

                var rawErrors = validationResult.GetErrorOutput();

                rawErrors.Should().NotBeNull();

                rawErrors.Keys.Should().HaveCount(resultsErrors.Count);

                foreach (var expectedErrorsPair in expectedErrors)
                {
                    rawErrors.Keys.Should().Contain(expectedErrorsPair.Key);
                    rawErrors[expectedErrorsPair.Key].Should().HaveCount(expectedErrorsPair.Value.Count);

                    foreach (var error in expectedErrorsPair.Value)
                    {
                        rawErrors[expectedErrorsPair.Key].Should().Contain(x => ReferenceEquals(x, error));
                    }
                }
            }

            [Fact]
            public void Should_ReturnEmptyDictionary_When_Valid()
            {
                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), Substitute.For<IMessagesService>());

                var rawErrors = validationResult.GetErrorOutput();

                rawErrors.Should().NotBeNull();
                rawErrors.Should().BeEmpty();
            }
        }

        public class GetTranslation
        {
            [Fact]
            public void Should_Return_Translation_FromMessageService()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var translation = new Dictionary<string, string>()
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                };

                messagesService.GetTranslation(Arg.Is("translationName1")).Returns(translation);

                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), messagesService);

                var resultTranslation = validationResult.GetTranslation("translationName1");

                messagesService.Received(1).GetTranslation(Arg.Is("translationName1"));
                messagesService.ReceivedWithAnyArgs(1).GetTranslation(default);

                resultTranslation.Should().BeSameAs(translation);
            }

            [Fact]
            public void Should_Return_EmptyTranslation_When_NullMessageService()
            {
                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

                var resultTranslation = validationResult.GetTranslation("translationName1");

                resultTranslation.Should().BeEmpty();
            }
        }

        public class ToStringTests
        {
            [Fact]
            public void Should_Return_NoErrorsString_When_Valid()
            {
                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

                var stringified = validationResult.ToString();

                stringified.Should().Be("(no error output)");
            }

            [Fact]
            public void Should_Return_ErrorMessages_FromMessageService_WithDefaultTranslation()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var errorMessages = new Dictionary<string, IReadOnlyList<string>>
                {
                    ["path1"] = new[] { "message11" },
                    ["path2"] = new[] { "message12", "message22" }
                };

                var resultErrors = new Dictionary<string, List<int>>()
                {
                    ["path1"] = new List<int>() { 1 }
                };

                var errorsRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error()
                };

                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string)).Returns(errorMessages);

                var validationResult = new ValidationResult(resultErrors, errorsRegistry, messagesService);

                ShouldHaveMessagesOnly(validationResult.ToString(), new[]
                {
                    "path1: message11",
                    "path2: message12",
                    "path2: message22"
                });

                messagesService.Received(1).GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string));
                messagesService.ReceivedWithAnyArgs(1).GetErrorsMessages(default);
            }

            [Fact]
            public void Should_Return_ErrorMessages_FromMessageService_WithDefaultTranslation_And_Codes()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var errorMessages = new Dictionary<string, IReadOnlyList<string>>
                {
                    ["path1"] = new[] { "message11" },
                    ["path2"] = new[] { "message12", "message22" }
                };

                var resultErrors = new Dictionary<string, List<int>>()
                {
                    ["path1"] = new List<int>() { 1 },
                    ["path2"] = new List<int>() { 2, 3 },
                    ["path3"] = new List<int>() { 1, 3 }
                };

                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error()
                    {
                        Codes = new[] { "CODE1", "CODE2" }
                    },
                    [2] = new Error()
                    {
                        Codes = new[] { "CODE2", "CODE3" }
                    },
                    [3] = new Error()
                    {
                        Codes = new[] { "CODE1", "CODE2", "CODE3", "CODE4" }
                    }
                };

                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string)).Returns(errorMessages);

                var validationResult = new ValidationResult(resultErrors, errorRegistry, messagesService);

                ShouldHaveCodesAndMessages(
                    validationResult.ToString(),
                    new[]
                    {
                        "CODE1",
                        "CODE2",
                        "CODE3",
                        "CODE4"
                    },
                    new[]
                    {
                        "path1: message11",
                        "path2: message12",
                        "path2: message22"
                    });

                messagesService.Received(1).GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string));
                messagesService.ReceivedWithAnyArgs(1).GetErrorsMessages(default);
            }

            [Fact]
            public void Should_Return_ErrorMessages_FromMessageService_WithSpecifiedTranslation()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var errorMessages1 = new Dictionary<string, IReadOnlyList<string>>
                {
                    ["path1"] = new[] { "message11" },
                    ["path2"] = new[] { "message12", "message22" }
                };

                var errorMessages2 = new Dictionary<string, IReadOnlyList<string>>
                {
                    ["path1"] = new[] { "MESSAGE11" },
                    ["path2"] = new[] { "MESSAGE12", "MESSAGE22" }
                };

                var resultErrors = new Dictionary<string, List<int>>()
                {
                    ["path1"] = new List<int>() { 1 }
                };

                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error()
                };

                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation1")).Returns(errorMessages1);
                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2")).Returns(errorMessages2);

                var validationResult = new ValidationResult(resultErrors, errorRegistry, messagesService);

                ShouldHaveMessagesOnly(
                    validationResult.ToString("translation2"),
                    new[]
                    {
                        "path1: MESSAGE11",
                        "path2: MESSAGE12",
                        "path2: MESSAGE22"
                    });

                messagesService.Received(1).GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2"));
                messagesService.ReceivedWithAnyArgs(1).GetErrorsMessages(default);
            }

            [Fact]
            public void Should_Return_ErrorMessages_FromMessageService_WithSpecifiedTranslation_And_Codes()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var errorMessages1 = new Dictionary<string, IReadOnlyList<string>>
                {
                    ["path1"] = new[] { "message11" },
                    ["path2"] = new[] { "message12", "message22" }
                };

                var errorMessages2 = new Dictionary<string, IReadOnlyList<string>>
                {
                    ["path1"] = new[] { "MESSAGE11" },
                    ["path2"] = new[] { "MESSAGE12", "MESSAGE22" }
                };

                var resultErrors = new Dictionary<string, List<int>>()
                {
                    ["path1"] = new List<int>() { 1 },
                    ["path2"] = new List<int>() { 2, 3 },
                    ["path3"] = new List<int>() { 1, 3 }
                };

                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error()
                    {
                        Codes = new[] { "CODE1", "CODE2" }
                    },
                    [2] = new Error()
                    {
                        Codes = new[] { "CODE2", "CODE3" }
                    },
                    [3] = new Error()
                    {
                        Codes = new[] { "CODE1", "CODE2", "CODE3", "CODE4" }
                    }
                };

                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation1")).Returns(errorMessages1);
                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2")).Returns(errorMessages2);

                var validationResult = new ValidationResult(resultErrors, errorRegistry, messagesService);

                ShouldHaveCodesAndMessages(
                    validationResult.ToString("translation2"),
                    new[]
                    {
                        "CODE1",
                        "CODE2",
                        "CODE3",
                        "CODE4"
                    },
                    new[]
                    {
                        "path1: MESSAGE11",
                        "path2: MESSAGE12",
                        "path2: MESSAGE22"
                    });

                messagesService.Received(1).GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2"));
                messagesService.ReceivedWithAnyArgs(1).GetErrorsMessages(default);
            }

            [Fact]
            public void Should_Return_Codes()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var errorMessages = new Dictionary<string, IReadOnlyList<string>>();

                var resultErrors = new Dictionary<string, List<int>>()
                {
                    ["path1"] = new List<int>() { 1 }
                };

                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error()
                    {
                        Codes = new[] { "CODE1", "CODE2" }
                    }
                };

                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string)).Returns(errorMessages);

                var validationResult = new ValidationResult(resultErrors, errorRegistry, messagesService);

                ShouldHaveCodesOnly(
                    validationResult.ToString(),
                    new[]
                    {
                        "CODE1",
                        "CODE2"
                    });

                messagesService.Received(1).GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string));
                messagesService.ReceivedWithAnyArgs(1).GetErrorsMessages(default);
            }

            [Fact]
            public void Should_Return_Codes_Distinct()
            {
                var messagesService = Substitute.For<IMessagesService>();

                var errorMessages = new Dictionary<string, IReadOnlyList<string>>();

                var resultErrors = new Dictionary<string, List<int>>()
                {
                    ["path1"] = new List<int>() { 1 },
                    ["path2"] = new List<int>() { 2, 3 },
                    ["path3"] = new List<int>() { 1, 3 }
                };

                var errorRegistry = new Dictionary<int, IError>()
                {
                    [1] = new Error()
                    {
                        Codes = new[] { "CODE1", "CODE2" }
                    },
                    [2] = new Error()
                    {
                        Codes = new[] { "CODE2", "CODE3" }
                    },
                    [3] = new Error()
                    {
                        Codes = new[] { "CODE1", "CODE2", "CODE3", "CODE4" }
                    }
                };

                messagesService.GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string)).Returns(errorMessages);

                var validationResult = new ValidationResult(resultErrors, errorRegistry, messagesService);

                ShouldHaveCodesOnly(
                    validationResult.ToString(),
                    new[]
                    {
                        "CODE1",
                        "CODE2",
                        "CODE3",
                        "CODE4"
                    });

                messagesService.Received(1).GetErrorsMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string));
                messagesService.ReceivedWithAnyArgs(1).GetErrorsMessages(default);
            }

            private static void ShouldHaveMessagesOnly(string validationResult, IReadOnlyList<string> messages) => ShouldHaveCodesAndMessages(validationResult, null, messages);

            private static void ShouldHaveCodesOnly(string validationResult, IReadOnlyList<string> codes) => ShouldHaveCodesAndMessages(validationResult, codes, null);

            private static void ShouldHaveCodesAndMessages(string validationResult, IReadOnlyList<string> codes, IReadOnlyList<string> messages)
            {
                validationResult.Should().NotBeNullOrEmpty();

                var anyCodes = codes?.Any() == true;
                var anyMessages = messages?.Any() == true;

                if (!anyCodes && !anyMessages)
                {
                    validationResult.Should().Be("(no error output)");

                    return;
                }

                if (anyCodes)
                {
                    if (anyMessages)
                    {
                        validationResult.Should().Contain(Environment.NewLine);
                    }
                    else
                    {
                        validationResult.Should().NotContain(Environment.NewLine);
                    }

                    var codesLine = anyMessages
                        ? validationResult.Substring(0, validationResult.IndexOf(Environment.NewLine, StringComparison.Ordinal))
                        : validationResult;

                    var extractedCodes = codesLine.Split(new[] { ", " }, StringSplitOptions.None);

                    extractedCodes.Should().HaveCount(codes.Count);
                    extractedCodes.Should().Contain(codes);
                    codes.Should().Contain(extractedCodes, because: "(reversed)");
                }

                if (anyMessages)
                {
                    string messagesPart;

                    if (anyCodes)
                    {
                        messagesPart = validationResult.Substring(validationResult.IndexOf(Environment.NewLine, StringComparison.Ordinal));

                        messagesPart.Should().StartWith(Environment.NewLine);
                        messagesPart = messagesPart.Substring(Environment.NewLine.Length);

                        messagesPart.Should().StartWith(Environment.NewLine);
                        messagesPart = messagesPart.Substring(Environment.NewLine.Length);
                    }
                    else
                    {
                        messagesPart = validationResult;
                    }

                    var lines = messagesPart.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    lines.Should().Contain(messages);
                    messages.Should().Contain(lines, because: "(reversed)");
                    lines.Should().HaveCount(messages.Count);
                }
            }
        }
    }
}
