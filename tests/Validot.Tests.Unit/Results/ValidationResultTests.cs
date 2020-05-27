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
            _ = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), Substitute.For<IMessageService>());
        }

        [Fact]
        public void AnyErrors_Should_BeFalse_When_NoErrors()
        {
            var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), Substitute.For<IMessageService>());

            validationResult.AnyErrors.Should().BeFalse();
        }

        [Fact]
        public void AnyErrors_Should_BeTrue_When_ResultContainsErrors()
        {
            var resultErrors = new Dictionary<string, List<int>>();
            resultErrors.Add("path", new List<int>() { 1 });

            var validationResult = new ValidationResult(resultErrors, new Dictionary<int, IError>(), Substitute.For<IMessageService>());

            validationResult.AnyErrors.Should().BeTrue();
        }

        [Fact]
        public void NoErrorsResult_Should_BeResultWithoutErrors()
        {
            ValidationResult.NoErrorsResult.AnyErrors.Should().BeFalse();
            ValidationResult.NoErrorsResult.Paths.Should().BeEmpty();
            ValidationResult.NoErrorsResult.TranslationNames.Should().BeEmpty();
            ValidationResult.NoErrorsResult.Codes.Should().BeEmpty();
            ValidationResult.NoErrorsResult.CodeMap.Should().BeEmpty();
            ValidationResult.NoErrorsResult.MessageMap.Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetErrorOutput().Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetTranslatedMessageMap(null).Should().BeEmpty();
            ValidationResult.NoErrorsResult.GetTranslatedMessageMap("English").Should().BeEmpty();
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
            var validationResult = new ValidationResult(resultsErrors, new Dictionary<int, IError>(), Substitute.For<IMessageService>());

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
                var messageService = Substitute.For<IMessageService>();

                var translationNames = new[]
                {
                    "translation1",
                    "translation2"
                };

                messageService.TranslationNames.Returns(translationNames);

                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), messageService);

                validationResult.TranslationNames.Should().BeSameAs(translationNames);
            }

            [Fact]
            public void Should_Return_EmptyTranslationNames_When_NullTranslationName_InMessageService()
            {
                var messageService = Substitute.For<IMessageService>();

                messageService.TranslationNames.Returns(null as IReadOnlyList<string>);

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

        public class GetTranslatedMessageMap
        {
            [Fact]
            public void Should_Return_Messages_FromMessageService_WithDefaultTranslation_WhenNullTranslationName()
            {
                var messageService = Substitute.For<IMessageService>();

                var errorMessages = new Dictionary<string, IReadOnlyList<string>>
                {
                    ["path1"] = new[] { "message11" },
                    ["path2"] = new[] { "message12", "message22" }
                };

                var resultErrors = new Dictionary<string, List<int>>()
                {
                    ["path1"] = new List<int>() { 1 }
                };

                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string)).Returns(errorMessages);

                var validationResult = new ValidationResult(resultErrors, new Dictionary<int, IError>(), messageService);

                var resultErrorMessages = validationResult.GetTranslatedMessageMap(null);

                messageService.Received(1).GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string));
                messageService.ReceivedWithAnyArgs(1).GetMessages(default);

                resultErrorMessages.Should().BeSameAs(errorMessages);
            }

            [Fact]
            public void Should_Return_Messages_FromMessageService_WithSpecifiedTranslation()
            {
                var messageService = Substitute.For<IMessageService>();

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

                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation1")).Returns(errorMessages1);
                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2")).Returns(errorMessages2);

                var validationResult = new ValidationResult(resultErrors, new Dictionary<int, IError>(), messageService);

                var resultErrorMessages = validationResult.GetTranslatedMessageMap("translation2");

                messageService.Received(1).GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2"));
                messageService.ReceivedWithAnyArgs(1).GetMessages(default);

                resultErrorMessages.Should().BeSameAs(errorMessages2);
            }

            [Fact]
            public void Should_Return_EmptyMessageMap_When_Valid()
            {
                var messageService = Substitute.For<IMessageService>();

                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

                var resultErrorMessages = validationResult.GetTranslatedMessageMap(null);

                messageService.DidNotReceiveWithAnyArgs().GetMessages(default);

                resultErrorMessages.Should().NotBeNull();
                resultErrorMessages.Should().BeEmpty();
            }
        }

        public class CodeMap
        {
            [Fact]
            public void Should_Return_EmptyCodeMap_When_Valid()
            {
                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), null);

                validationResult.CodeMap.Should().NotBeNull();
                validationResult.CodeMap.Should().BeEmpty();
            }

            [Fact]
            public void Should_Return_AllCodes()
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

                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessageService>());

                validationResult.CodeMap.Should().NotBeNull();

                validationResult.CodeMap.Should().HaveCount(4);

                validationResult.CodeMap.Keys.Should().Contain("");
                validationResult.CodeMap[""].Should().HaveCount(1);
                validationResult.CodeMap[""].Should().Contain("Message1");

                validationResult.CodeMap.Keys.Should().Contain("test1");
                validationResult.CodeMap["test1"].Should().HaveCount(3);
                validationResult.CodeMap["test1"].Should().Contain("Message1", "Message2", "Message3");

                validationResult.CodeMap.Keys.Should().Contain("test2");
                validationResult.CodeMap["test2"].Should().HaveCount(3);
                validationResult.CodeMap["test2"].Should().Contain("Message2", "Message41", "Message42");

                validationResult.CodeMap.Keys.Should().Contain("test3");
                validationResult.CodeMap["test3"].Should().HaveCount(1);
                validationResult.CodeMap["test3"].Should().Contain("Message3");
            }

            public static IEnumerable<object[]> Should_Return_AllCodes_MoreExamples_Data()
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
            [MemberData(nameof(Should_Return_AllCodes_MoreExamples_Data))]
            public void Should_Return_AllCodes_MoreExamples(Dictionary<string, List<int>> resultsErrors, Dictionary<int, IError> errorRegistry, Dictionary<string, IReadOnlyList<string>> expectedCodes)
            {
                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessageService>());

                validationResult.CodeMap.Should().NotBeNull();

                validationResult.CodeMap.Should().HaveCount(expectedCodes.Count);

                foreach (var pair in expectedCodes)
                {
                    validationResult.CodeMap.Keys.Should().Contain(pair.Key);
                    validationResult.CodeMap[pair.Key].Should().HaveCount(pair.Value.Count);
                    validationResult.CodeMap[pair.Key].Should().Contain(pair.Value);
                }
            }
        }

        public class Codes
        {
            [Fact]
            public void Should_ReturnAllCodesFromErrors_WithoutDuplicates()
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

                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessageService>());

                validationResult.Codes.Should().NotBeNull();

                validationResult.Codes.Should().HaveCount(5);

                validationResult.Codes.Should().Contain("CODE1");
                validationResult.Codes.Should().Contain("CODE2");
                validationResult.Codes.Should().Contain("CODE3");
                validationResult.Codes.Should().Contain("CODE41");
                validationResult.Codes.Should().Contain("CODE42");
            }

            public static IEnumerable<object[]> Should_ReturnAllCodesFromErrors_WithoutDuplicates_MoreExamples_Data()
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
            [MemberData(nameof(Should_ReturnAllCodesFromErrors_WithoutDuplicates_MoreExamples_Data))]
            public void Should_ReturnAllCodesFromErrors_WithoutDuplicates_MoreExamples(Dictionary<string, List<int>> resultsErrors, Dictionary<int, IError> errorRegistry, IReadOnlyList<string> expectedCodes)
            {
                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessageService>());

                validationResult.Codes.Should().NotBeNull();

                validationResult.Codes.Should().HaveCount(expectedCodes.Count);
                validationResult.Codes.Should().Contain(expectedCodes);
                expectedCodes.Should().Contain(validationResult.Codes);
            }

            [Fact]
            public void Should_ReturnEmptyList_When_Valid()
            {
                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), Substitute.For<IMessageService>());

                validationResult.Codes.Should().NotBeNull();
                validationResult.Codes.Should().BeEmpty();
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

                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessageService>());

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
            public void Should_ReturnErrorOutput_MoreExamples(Dictionary<string, List<int>> resultsErrors, Dictionary<int, IError> errorRegistry, IReadOnlyDictionary<string, IReadOnlyList<IError>> expectedErrors)
            {
                var validationResult = new ValidationResult(resultsErrors, errorRegistry, Substitute.For<IMessageService>());

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
                var validationResult = new ValidationResult(new Dictionary<string, List<int>>(), new Dictionary<int, IError>(), Substitute.For<IMessageService>());

                var rawErrors = validationResult.GetErrorOutput();

                rawErrors.Should().NotBeNull();
                rawErrors.Should().BeEmpty();
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
            public void Should_Return_Messages_FromMessageService_WithDefaultTranslation()
            {
                var messageService = Substitute.For<IMessageService>();

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

                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string)).Returns(errorMessages);

                var validationResult = new ValidationResult(resultErrors, errorsRegistry, messageService);

                ShouldHaveMessagesOnly(validationResult.ToString(), new[]
                {
                    "path1: message11",
                    "path2: message12",
                    "path2: message22"
                });

                messageService.Received(1).GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string));
                messageService.ReceivedWithAnyArgs(1).GetMessages(default);
            }

            [Fact]
            public void Should_Return_Messages_FromMessageService_WithDefaultTranslation_And_Codes()
            {
                var messageService = Substitute.For<IMessageService>();

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

                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string)).Returns(errorMessages);

                var validationResult = new ValidationResult(resultErrors, errorRegistry, messageService);

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

                messageService.Received(1).GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string));
                messageService.ReceivedWithAnyArgs(1).GetMessages(default);
            }

            [Fact]
            public void Should_Return_Messages_FromMessageService_WithSpecifiedTranslation()
            {
                var messageService = Substitute.For<IMessageService>();

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

                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation1")).Returns(errorMessages1);
                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2")).Returns(errorMessages2);

                var validationResult = new ValidationResult(resultErrors, errorRegistry, messageService);

                ShouldHaveMessagesOnly(
                    validationResult.ToString("translation2"),
                    new[]
                    {
                        "path1: MESSAGE11",
                        "path2: MESSAGE12",
                        "path2: MESSAGE22"
                    });

                messageService.Received(1).GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2"));
                messageService.ReceivedWithAnyArgs(1).GetMessages(default);
            }

            [Fact]
            public void Should_Return_Messages_FromMessageService_WithSpecifiedTranslation_And_Codes()
            {
                var messageService = Substitute.For<IMessageService>();

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

                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation1")).Returns(errorMessages1);
                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2")).Returns(errorMessages2);

                var validationResult = new ValidationResult(resultErrors, errorRegistry, messageService);

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

                messageService.Received(1).GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>("translation2"));
                messageService.ReceivedWithAnyArgs(1).GetMessages(default);
            }

            [Fact]
            public void Should_Return_Codes()
            {
                var messageService = Substitute.For<IMessageService>();

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

                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string)).Returns(errorMessages);

                var validationResult = new ValidationResult(resultErrors, errorRegistry, messageService);

                ShouldHaveCodesOnly(
                    validationResult.ToString(),
                    new[]
                    {
                        "CODE1",
                        "CODE2"
                    });

                messageService.Received(1).GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string));
                messageService.ReceivedWithAnyArgs(1).GetMessages(default);
            }

            [Fact]
            public void Should_Return_Codes_WithoutDuplicates()
            {
                var messageService = Substitute.For<IMessageService>();

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

                messageService.GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string)).Returns(errorMessages);

                var validationResult = new ValidationResult(resultErrors, errorRegistry, messageService);

                ShouldHaveCodesOnly(
                    validationResult.ToString(),
                    new[]
                    {
                        "CODE1",
                        "CODE2",
                        "CODE3",
                        "CODE4"
                    });

                messageService.Received(1).GetMessages(Arg.Is<Dictionary<string, List<int>>>(a => ReferenceEquals(a, resultErrors)), Arg.Is<string>(null as string));
                messageService.ReceivedWithAnyArgs(1).GetMessages(default);
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
