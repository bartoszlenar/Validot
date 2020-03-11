namespace Validot.Tests.Unit.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Validot.Errors;
    using Validot.Errors.Args;

    using Xunit;

    public class MessagesServiceTests
    {
        public static readonly Dictionary<string, IReadOnlyDictionary<string, string>> DefaultTranslations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
        {
            ["translation1"] = new Dictionary<string, string>()
            {
                ["key11"] = "message11",
                ["key12"] = "message12",
                ["key21"] = "message21 {numberArg} {textArg}",
                ["key22"] = "message22 {textArg|case=lower} {textArg|case=upper}"
            },
            ["translation2"] = new Dictionary<string, string>()
            {
                ["key11"] = "MESSAGE_11",
                ["key12"] = "MESSAGE_12",
                ["key21"] = "MESSAGE_21 {numberArg|format=0000} {textArg|case=upper}",
                ["key22"] = "MESSAGE_22 {textArg} {textArg}"
            }
        };

        public static readonly Dictionary<int, IError> DefaultErrors = new Dictionary<int, IError>()
        {
            [0] = new Error()
            {
                Messages = new[]
                {
                    "key11",
                    "key12"
                },
                Args = Array.Empty<IArg>()
            },
            [1] = new Error()
            {
                Messages = new[]
                {
                    "key21",
                    "key22"
                },
                Args = new[]
                {
                    Arg.Number("numberArg", 123),
                    Arg.Text("textArg", "textArgValue")
                }
            }
        };

        public static readonly Dictionary<string, IReadOnlyList<int>> DefaultErrorsMap = new Dictionary<string, IReadOnlyList<int>>()
        {
            ["name"] = new[]
            {
                0
            },
            ["path.name"] = new[]
            {
                1
            },
            ["new.path.name"] = new[]
            {
                0,
                1
            },
        };

        [Fact]
        public void Should_Initialize()
        {
            // ReSharper disable once AssignmentIsFullyDiscarded
            _ = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);
        }

        [Fact]
        public void Should_Initialize_WithEmptyTranslations()
        {
            _ = new MessagesService(new Dictionary<string, IReadOnlyDictionary<string, string>>(), DefaultErrors, DefaultErrorsMap);
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_Translations_Null()
        {
            Action action = () => new MessagesService(null, DefaultErrors, DefaultErrorsMap);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_Translations_WithNull()
        {
            var translations1 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
            {
                ["translation1"] = DefaultTranslations["translation2"],
                ["translation2"] = null
            };

            var translations2 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
            {
                ["translation1"] = DefaultTranslations["translation2"],
                ["translation2"] = new Dictionary<string, string>()
                {
                    ["key11"] = null
                }
            };

            Action action1 = () => new MessagesService(translations1, DefaultErrors, DefaultErrorsMap);
            Action action2 = () => new MessagesService(translations2, DefaultErrors, DefaultErrorsMap);

            action1.Should().ThrowExactly<ArgumentNullException>();
            action2.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_Errors_Null()
        {
            Action action = () => new MessagesService(DefaultTranslations, null, DefaultErrorsMap);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_Errors_WithNull()
        {
            var errors0 = new Dictionary<int, IError>()
            {
                [0] = null,
                [1] = DefaultErrors[1]
            };

            var errors1 = new Dictionary<int, IError>()
            {
                [0] = new Error()
                {
                    Messages = null,
                    Args = Array.Empty<IArg>()
                },
                [1] = DefaultErrors[1]
            };

            var errors2 = new Dictionary<int, IError>()
            {
                [0] = new Error()
                {
                    Messages = new[]
                    {
                        "message",
                        null
                    },
                    Args = Array.Empty<IArg>()
                },
                [1] = DefaultErrors[1]
            };

            var errors3 = new Dictionary<int, IError>()
            {
                [0] = new Error()
                {
                    Messages = new[]
                    {
                        "message",
                        null
                    },
                    Args = null
                },
                [1] = DefaultErrors[1]
            };

            var errors4 = new Dictionary<int, IError>()
            {
                [0] = new Error()
                {
                    Messages = new[]
                    {
                        "message",
                        null
                    },
                    Args = new IArg[]
                    {
                        Arg.Text("a", "a"),
                        null
                    }
                },
                [1] = DefaultErrors[1]
            };

            Action action0 = () => new MessagesService(DefaultTranslations, errors0, DefaultErrorsMap);
            Action action1 = () => new MessagesService(DefaultTranslations, errors1, DefaultErrorsMap);
            Action action2 = () => new MessagesService(DefaultTranslations, errors2, DefaultErrorsMap);
            Action action3 = () => new MessagesService(DefaultTranslations, errors3, DefaultErrorsMap);
            Action action4 = () => new MessagesService(DefaultTranslations, errors4, DefaultErrorsMap);

            action0.Should().ThrowExactly<ArgumentNullException>();
            action1.Should().ThrowExactly<ArgumentNullException>();
            action2.Should().ThrowExactly<ArgumentNullException>();
            action3.Should().ThrowExactly<ArgumentNullException>();
            action4.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_ErrorMap_Null()
        {
            Action action = () => new MessagesService(DefaultTranslations, DefaultErrors, null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_ErrorMap_WithNull()
        {
            var errorsMap = new Dictionary<string, IReadOnlyList<int>>()
            {
                ["name"] = new[]
                {
                    0
                },
                ["path.name"] = new[]
                {
                    1
                },
                ["new.path.name"] = null
            };

            Action action = () => new MessagesService(DefaultTranslations, DefaultErrors, errorsMap);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_ErrorMap_WithInvalidErrorId()
        {
            var errorsMap = new Dictionary<string, IReadOnlyList<int>>()
            {
                ["name"] = new[]
                {
                    0
                },
                ["path.name"] = new[]
                {
                    2
                }
            };

            Action action = () => new MessagesService(DefaultTranslations, DefaultErrors, errorsMap);

            action.Should().ThrowExactly<KeyNotFoundException>();
        }

        [Fact]
        public void Should_Load_TranslationNames()
        {
            var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

            messagesService.TranslationsNames.Should().NotBeNull();
            messagesService.TranslationsNames.Count.Should().Be(2);

            messagesService.TranslationsNames.Should().Contain("translation1");
            messagesService.TranslationsNames.Should().Contain("translation2");
        }

        public class GetTranslation
        {
            [Fact]
            public void Should_GetTranslation()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                var translation1 = messagesService.GetTranslation("translation1");

                translation1.Should().BeSameAs(DefaultTranslations["translation1"]);

                translation1["key11"].Should().Be("message11");
                translation1["key12"].Should().Be("message12");
                translation1["key21"].Should().Be("message21 {numberArg} {textArg}");
                translation1["key22"].Should().Be("message22 {textArg|case=lower} {textArg|case=upper}");

                var translation2 = messagesService.GetTranslation("translation2");

                translation2.Should().BeSameAs(DefaultTranslations["translation2"]);

                translation2["key11"].Should().Be("MESSAGE_11");
                translation2["key12"].Should().Be("MESSAGE_12");
                translation2["key21"].Should().Be("MESSAGE_21 {numberArg|format=0000} {textArg|case=upper}");
                translation2["key22"].Should().Be("MESSAGE_22 {textArg} {textArg}");
            }

            [Fact]
            public void Should_ThrowException_When_InvalidTranslationName()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                Action action1 = () => messagesService.GetTranslation("translationX");

                Action action2 = () => messagesService.GetTranslation("TRANSLATION1");

                action1.Should().ThrowExactly<KeyNotFoundException>();
                action2.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullTranslationName()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                Action action = () => messagesService.GetTranslation(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class GetErrorsMessages
        {
            [Fact]
            public void Should_ThrowException_When_Errors()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                Action action = () => messagesService.GetErrorsMessages(null);

                action.Should().ThrowExactly<NullReferenceException>();
            }

            [Fact]
            public void Should_ThrowException_When_Errors_WithNullErrorsIds()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                Action action = () => messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["name"] = new List<int>()
                        {
                            0
                        },
                        ["path.name"] = null
                    },
                    "translation1");

                action.Should().ThrowExactly<NullReferenceException>();
            }

            [Fact]
            public void Should_Get_From_SinglePath_When_SingleError()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["path.name"] = new List<int>()
                        {
                            1
                        }
                    },
                    "translation2");

                result.Count.Should().Be(1);
                result.Keys.Should().Contain("path.name");

                result["path.name"].Count.Should().Be(2);
                result["path.name"].ElementAt(0).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result["path.name"].ElementAt(1).Should().Be("MESSAGE_22 textArgValue textArgValue");
            }

            [Fact]
            public void Should_Get_From_SinglePath_When_ManyErrors()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["new.path.name"] = new List<int>()
                        {
                            0,
                            1
                        }
                    },
                    "translation2");

                result.Count.Should().Be(1);
                result.Keys.Should().Contain("new.path.name");

                result["new.path.name"].Count.Should().Be(4);
                result["new.path.name"].ElementAt(0).Should().Be("MESSAGE_11");
                result["new.path.name"].ElementAt(1).Should().Be("MESSAGE_12");
                result["new.path.name"].ElementAt(2).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result["new.path.name"].ElementAt(3).Should().Be("MESSAGE_22 textArgValue textArgValue");
            }

            [Fact]
            public void Should_Get_From_SinglePathWithManyErrors_But_SingleError()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["new.path.name"] = new List<int>()
                        {
                            0
                        }
                    },
                    "translation2");

                result.Count.Should().Be(1);
                result.Keys.Should().Contain("new.path.name");

                result["new.path.name"].Count.Should().Be(2);
                result["new.path.name"].ElementAt(0).Should().Be("MESSAGE_11");
                result["new.path.name"].ElementAt(1).Should().Be("MESSAGE_12");
            }

            [Fact]
            public void Should_Get_From_ManyPaths()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["name"] = new List<int>()
                        {
                            0
                        },
                        ["path.name"] = new List<int>()
                        {
                            1
                        },
                        ["new.path.name"] = new List<int>()
                        {
                            0,
                            1
                        }
                    },
                    "translation2");

                result.Count.Should().Be(3);

                result.Keys.Should().Contain("name");
                result["name"].Count.Should().Be(2);
                result["name"].ElementAt(0).Should().Be("MESSAGE_11");
                result["name"].ElementAt(1).Should().Be("MESSAGE_12");

                result.Keys.Should().Contain("path.name");
                result["path.name"].Count.Should().Be(2);
                result["path.name"].ElementAt(0).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result["path.name"].ElementAt(1).Should().Be("MESSAGE_22 textArgValue textArgValue");

                result.Keys.Should().Contain("new.path.name");
                result["new.path.name"].Count.Should().Be(4);
                result["new.path.name"].ElementAt(0).Should().Be("MESSAGE_11");
                result["new.path.name"].ElementAt(1).Should().Be("MESSAGE_12");
                result["new.path.name"].ElementAt(2).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result["new.path.name"].ElementAt(3).Should().Be("MESSAGE_22 textArgValue textArgValue");
            }

            [Fact]
            public void Should_Get_From_ManyPaths_And_DifferentTranslations()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                var result1 = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["name"] = new List<int>()
                        {
                            0
                        },
                        ["new.path.name"] = new List<int>()
                        {
                            1
                        }
                    },
                    "translation1");

                result1.Count.Should().Be(2);

                result1.Keys.Should().Contain("name");
                result1["name"].Count.Should().Be(2);
                result1["name"].ElementAt(0).Should().Be("message11");
                result1["name"].ElementAt(1).Should().Be("message12");

                result1.Keys.Should().Contain("new.path.name");
                result1["new.path.name"].Count.Should().Be(2);
                result1["new.path.name"].ElementAt(0).Should().Be("message21 123 textArgValue");
                result1["new.path.name"].ElementAt(1).Should().Be("message22 textargvalue TEXTARGVALUE");

                var result2 = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["name"] = new List<int>()
                        {
                            0
                        },
                        ["new.path.name"] = new List<int>()
                        {
                            1
                        }
                    },
                    "translation2");

                result2.Count.Should().Be(2);

                result2.Keys.Should().Contain("name");
                result2["name"].Count.Should().Be(2);
                result2["name"].ElementAt(0).Should().Be("MESSAGE_11");
                result2["name"].ElementAt(1).Should().Be("MESSAGE_12");

                result2.Keys.Should().Contain("new.path.name");
                result2["new.path.name"].Count.Should().Be(2);
                result2["new.path.name"].ElementAt(0).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result2["new.path.name"].ElementAt(1).Should().Be("MESSAGE_22 textArgValue textArgValue");
            }

            [Fact]
            public void Should_Get_With_EnglishTranslation_When_NoTranslationSpecified()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["English"] = new Dictionary<string, string>()
                    {
                        ["key11"] = "en11",
                        ["key12"] = "en12",
                        ["key21"] = "en21 {numberArg} {textArg|case=upper}",
                        ["key22"] = "en22 {textArg}"
                    },
                    ["translation1"] = DefaultTranslations["translation1"],
                    ["translation2"] = DefaultTranslations["translation2"]
                };

                var messagesService = new MessagesService(translations, DefaultErrors, DefaultErrorsMap);

                var result = messagesService.GetErrorsMessages(new Dictionary<string, List<int>>()
                {
                    ["path.name"] = new List<int>()
                    {
                        1
                    }
                });

                result.Count.Should().Be(1);
                result.Keys.Should().Contain("path.name");

                result["path.name"].Count.Should().Be(2);
                result["path.name"].ElementAt(0).Should().Be("en21 123 TEXTARGVALUE");
                result["path.name"].ElementAt(1).Should().Be("en22 textArgValue");
            }

            [Fact]
            public void Should_Get_With_ErrorsWithCustomMessages()
            {
                var errors = new Dictionary<int, IError>()
                {
                    [0] = new Error()
                    {
                        Messages = new[]
                        {
                            "customMessage",
                            "key12"
                        },
                        Args = Array.Empty<IArg>()
                    },
                    [1] = DefaultErrors[1]
                };

                var messagesService = new MessagesService(DefaultTranslations, errors, DefaultErrorsMap);

                var result1 = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["new.path.name"] = new List<int>()
                        {
                            0
                        },
                    },
                    "translation1");

                result1.Count.Should().Be(1);
                result1.Keys.Should().Contain("new.path.name");

                result1["new.path.name"].Count.Should().Be(2);
                result1["new.path.name"].ElementAt(0).Should().Be("customMessage");
                result1["new.path.name"].ElementAt(1).Should().Be("message12");

                var result2 = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["new.path.name"] = new List<int>()
                        {
                            0
                        },
                    },
                    "translation2");

                result2.Count.Should().Be(1);
                result2.Keys.Should().Contain("new.path.name");

                result2["new.path.name"].Count.Should().Be(2);
                result2["new.path.name"].ElementAt(0).Should().Be("customMessage");
                result2["new.path.name"].ElementAt(1).Should().Be("MESSAGE_12");
            }

            [Fact]
            public void Should_Get_With_TranslateArg()
            {
                var errors = new Dictionary<int, IError>()
                {
                    [0] = new Error()
                    {
                        Messages = new[]
                        {
                            "translation {_translation|key=key12}",
                            "key12"
                        },
                        Args = Array.Empty<IArg>()
                    },
                    [1] = DefaultErrors[1]
                };

                var messagesService = new MessagesService(DefaultTranslations, errors, DefaultErrorsMap);

                var result1 = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["new.path.name"] = new List<int>()
                        {
                            0
                        },
                    },
                    "translation1");

                result1.Count.Should().Be(1);
                result1.Keys.Should().Contain("new.path.name");

                result1["new.path.name"].Count.Should().Be(2);
                result1["new.path.name"].ElementAt(0).Should().Be("translation message12");
                result1["new.path.name"].ElementAt(1).Should().Be("message12");

                var result2 = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["new.path.name"] = new List<int>()
                        {
                            0
                        },
                    },
                    "translation2");

                result2.Count.Should().Be(1);
                result2.Keys.Should().Contain("new.path.name");

                result2["new.path.name"].Count.Should().Be(2);
                result2["new.path.name"].ElementAt(0).Should().Be("translation MESSAGE_12");
                result2["new.path.name"].ElementAt(1).Should().Be("MESSAGE_12");
            }

            [Fact]
            public void Should_Get_With_TranslateArg_And_LeaveKey_When_KeyIsInvalid()
            {
                var errors = new Dictionary<int, IError>()
                {
                    [0] = new Error()
                    {
                        Messages = new[]
                        {
                            "translation {_translation|key=invalidKey}",
                            "key12"
                        },
                        Args = Array.Empty<IArg>()
                    },
                    [1] = DefaultErrors[1]
                };

                var messagesService = new MessagesService(DefaultTranslations, errors, DefaultErrorsMap);

                var result1 = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["new.path.name"] = new List<int>()
                        {
                            0
                        },
                    },
                    "translation1");

                result1.Count.Should().Be(1);
                result1.Keys.Should().Contain("new.path.name");

                result1["new.path.name"].Count.Should().Be(2);
                result1["new.path.name"].ElementAt(0).Should().Be("translation invalidKey");
                result1["new.path.name"].ElementAt(1).Should().Be("message12");
            }

            [Fact]
            public void Should_Get_With_PathArgs()
            {
                var errors = new Dictionary<int, IError>()
                {
                    [0] = new Error()
                    {
                        Messages = new[]
                        {
                            "name11: {_name}",
                            "path12: {_path}"
                        },
                        Args = Array.Empty<IArg>()
                    },
                    [1] = new Error()
                    {
                        Messages = new[]
                        {
                            "name21: {_name}",
                            "path22: {_path}"
                        },
                        Args = Array.Empty<IArg>()
                    }
                };

                var messagesService = new MessagesService(DefaultTranslations, errors, DefaultErrorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["name"] = new List<int>()
                        {
                            0
                        },
                        ["path.name"] = new List<int>()
                        {
                            1
                        },
                        ["new.path.name"] = new List<int>()
                        {
                            0,
                            1
                        },
                    },
                    "translation1");

                result.Count.Should().Be(3);

                result.Keys.Should().Contain("name");
                result["name"].Count.Should().Be(2);
                result["name"].ElementAt(0).Should().Be("name11: name");
                result["name"].ElementAt(1).Should().Be("path12: name");

                result.Keys.Should().Contain("path.name");
                result["path.name"].Count.Should().Be(2);
                result["path.name"].ElementAt(0).Should().Be("name21: name");
                result["path.name"].ElementAt(1).Should().Be("path22: path.name");

                result.Keys.Should().Contain("new.path.name");
                result["new.path.name"].Count.Should().Be(4);
                result["new.path.name"].ElementAt(0).Should().Be("name11: name");
                result["new.path.name"].ElementAt(1).Should().Be("path12: new.path.name");
                result["new.path.name"].ElementAt(2).Should().Be("name21: name");
                result["new.path.name"].ElementAt(3).Should().Be("path22: new.path.name");
            }

            [Fact]
            public void Should_Get_With_PathArgs_MixedWithOtherArgs()
            {
                var errors = new Dictionary<int, IError>()
                {
                    [0] = new Error()
                    {
                        Messages = new[]
                        {
                            "name11: {_name} {_translation|key=key11} {numberArg}",
                            "path12: {_path} {textArg|case=upper}"
                        },
                        Args = new[]
                        {
                            Arg.Text("textArg", "textArgValue"),
                            Arg.Number("numberArg", 123)
                        }
                    },
                    [1] = DefaultErrors[1]
                };

                var messagesService = new MessagesService(DefaultTranslations, errors, DefaultErrorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["name"] = new List<int>()
                        {
                            0
                        },
                        ["new.path.name"] = new List<int>()
                        {
                            0,
                        },
                    },
                    "translation2");

                result.Count.Should().Be(2);

                result.Keys.Should().Contain("name");
                result["name"].Count.Should().Be(2);
                result["name"].ElementAt(0).Should().Be("name11: name MESSAGE_11 123");
                result["name"].ElementAt(1).Should().Be("path12: name TEXTARGVALUE");

                result.Keys.Should().Contain("new.path.name");
                result["new.path.name"].Count.Should().Be(2);
                result["new.path.name"].ElementAt(0).Should().Be("name11: name MESSAGE_11 123");
                result["new.path.name"].ElementAt(1).Should().Be("path12: new.path.name TEXTARGVALUE");
            }

            [Fact]
            public void Should_Get_With_IndexesInPath()
            {
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>()
                {
                    ["#"] = new[]
                    {
                        0
                    },
                    ["path.#.name"] = new[]
                    {
                        1
                    },
                    ["new.#.path.#"] = new[]
                    {
                        0,
                        1
                    },
                };

                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, errorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["#1"] = new List<int>()
                        {
                            0
                        },
                        ["path.#1.name"] = new List<int>()
                        {
                            1,
                        },
                        ["new.#4.path.#9"] = new List<int>()
                        {
                            1,
                        },
                    },
                    "translation2");

                result.Count.Should().Be(3);

                result.Keys.Should().Contain("#1");
                result["#1"].Count.Should().Be(2);
                result["#1"].ElementAt(0).Should().Be("MESSAGE_11");
                result["#1"].ElementAt(1).Should().Be("MESSAGE_12");

                result.Keys.Should().Contain("path.#1.name");
                result["path.#1.name"].Count.Should().Be(2);
                result["path.#1.name"].ElementAt(0).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result["path.#1.name"].ElementAt(1).Should().Be("MESSAGE_22 textArgValue textArgValue");

                result.Keys.Should().Contain("new.#4.path.#9");
                result["new.#4.path.#9"].Count.Should().Be(2);
                result["new.#4.path.#9"].ElementAt(0).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result["new.#4.path.#9"].ElementAt(1).Should().Be("MESSAGE_22 textArgValue textArgValue");
            }

            [Fact]
            public void Should_Get_With_IndexesInPath_With_SamePathWithDifferentIndexes()
            {
                var errorsMap = new Dictionary<string, IReadOnlyList<int>>()
                {
                    ["#"] = new[]
                    {
                        0,
                        1
                    },
                    ["new.#.path.#"] = new[]
                    {
                        0,
                        1
                    },
                };

                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, errorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["#1"] = new List<int>()
                        {
                            0
                        },
                        ["#99"] = new List<int>()
                        {
                            1
                        },
                        ["new.#0.path.#99"] = new List<int>()
                        {
                            0,
                        },
                        ["new.#10.path.#8"] = new List<int>()
                        {
                            1,
                        },
                        ["new.#21.path.#8888"] = new List<int>()
                        {
                            0,
                            1,
                        },
                    },
                    "translation2");

                result.Keys.Count().Should().Be(5);

                result.Keys.Should().Contain("#1");
                result["#1"].Count.Should().Be(2);
                result["#1"].ElementAt(0).Should().Be("MESSAGE_11");
                result["#1"].ElementAt(1).Should().Be("MESSAGE_12");

                result.Keys.Should().Contain("#99");
                result["#99"].Count.Should().Be(2);
                result["#99"].ElementAt(0).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result["#99"].ElementAt(1).Should().Be("MESSAGE_22 textArgValue textArgValue");

                result.Keys.Should().Contain("new.#0.path.#99");
                result["new.#0.path.#99"].Count.Should().Be(2);
                result["new.#0.path.#99"].ElementAt(0).Should().Be("MESSAGE_11");
                result["new.#0.path.#99"].ElementAt(1).Should().Be("MESSAGE_12");

                result.Keys.Should().Contain("new.#10.path.#8");
                result["new.#10.path.#8"].Count.Should().Be(2);
                result["new.#10.path.#8"].ElementAt(0).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result["new.#10.path.#8"].ElementAt(1).Should().Be("MESSAGE_22 textArgValue textArgValue");

                result.Keys.Should().Contain("new.#21.path.#8888");
                result["new.#21.path.#8888"].Count.Should().Be(4);
                result["new.#21.path.#8888"].ElementAt(0).Should().Be("MESSAGE_11");
                result["new.#21.path.#8888"].ElementAt(1).Should().Be("MESSAGE_12");
                result["new.#21.path.#8888"].ElementAt(2).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result["new.#21.path.#8888"].ElementAt(3).Should().Be("MESSAGE_22 textArgValue textArgValue");
            }

            [Fact]
            public void Should_Get_With_IndexesInPath_With_PathArgs()
            {
                var errors = new Dictionary<int, IError>()
                {
                    [0] = new Error()
                    {
                        Messages = new[]
                        {
                            "name11: {_name}",
                            "path12: {_path}"
                        },
                        Args = Array.Empty<IArg>()
                    },
                    [1] = new Error()
                    {
                        Messages = new[]
                        {
                            "name21: {_name}",
                            "path22: {_path}"
                        },
                        Args = Array.Empty<IArg>()
                    }
                };

                var errorsMap = new Dictionary<string, IReadOnlyList<int>>()
                {
                    ["#"] = new[]
                    {
                        0,
                        1
                    },
                    ["new.#.path.#"] = new[]
                    {
                        0,
                        1
                    },
                };

                var messagesService = new MessagesService(DefaultTranslations, errors, errorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["#1"] = new List<int>()
                        {
                            0
                        },
                        ["#99"] = new List<int>()
                        {
                            1
                        },
                        ["new.#0.path.#99"] = new List<int>()
                        {
                            0,
                        },
                        ["new.#10.path.#8"] = new List<int>()
                        {
                            1,
                        },
                        ["new.#21.path.#8888"] = new List<int>()
                        {
                            0,
                            1,
                        },
                    },
                    "translation2");

                result.Keys.Count().Should().Be(5);

                result.Keys.Should().Contain("#1");
                result["#1"].Count.Should().Be(2);
                result["#1"].ElementAt(0).Should().Be("name11: #1");
                result["#1"].ElementAt(1).Should().Be("path12: #1");

                result.Keys.Should().Contain("#99");
                result["#99"].Count.Should().Be(2);
                result["#99"].ElementAt(0).Should().Be("name21: #99");
                result["#99"].ElementAt(1).Should().Be("path22: #99");

                result.Keys.Should().Contain("new.#0.path.#99");
                result["new.#0.path.#99"].Count.Should().Be(2);
                result["new.#0.path.#99"].ElementAt(0).Should().Be("name11: #99");
                result["new.#0.path.#99"].ElementAt(1).Should().Be("path12: new.#0.path.#99");

                result.Keys.Should().Contain("new.#10.path.#8");
                result["new.#10.path.#8"].Count.Should().Be(2);
                result["new.#10.path.#8"].ElementAt(0).Should().Be("name21: #8");
                result["new.#10.path.#8"].ElementAt(1).Should().Be("path22: new.#10.path.#8");

                result.Keys.Should().Contain("new.#21.path.#8888");
                result["new.#21.path.#8888"].Count.Should().Be(4);
                result["new.#21.path.#8888"].ElementAt(0).Should().Be("name11: #8888");
                result["new.#21.path.#8888"].ElementAt(1).Should().Be("path12: new.#21.path.#8888");
                result["new.#21.path.#8888"].ElementAt(2).Should().Be("name21: #8888");
                result["new.#21.path.#8888"].ElementAt(3).Should().Be("path22: new.#21.path.#8888");
            }

            [Fact]
            public void Should_Get_With_NotRegisteredPaths()
            {
                var messagesService = new MessagesService(DefaultTranslations, DefaultErrors, DefaultErrorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["some.not.registered.path"] = new List<int>()
                        {
                            0
                        },
                        ["another.path.#2.with.#12.indexes"] = new List<int>()
                        {
                            0,
                            1
                        },
                    },
                    "translation2");

                result.Count.Should().Be(2);

                result.Keys.Should().Contain("some.not.registered.path");
                result["some.not.registered.path"].Count.Should().Be(2);
                result["some.not.registered.path"].ElementAt(0).Should().Be("MESSAGE_11");
                result["some.not.registered.path"].ElementAt(1).Should().Be("MESSAGE_12");

                result.Keys.Should().Contain("another.path.#2.with.#12.indexes");
                result["another.path.#2.with.#12.indexes"].Count.Should().Be(4);
                result["another.path.#2.with.#12.indexes"].ElementAt(0).Should().Be("MESSAGE_11");
                result["another.path.#2.with.#12.indexes"].ElementAt(1).Should().Be("MESSAGE_12");
                result["another.path.#2.with.#12.indexes"].ElementAt(2).Should().Be("MESSAGE_21 0123 TEXTARGVALUE");
                result["another.path.#2.with.#12.indexes"].ElementAt(3).Should().Be("MESSAGE_22 textArgValue textArgValue");
            }

            [Fact]
            public void Should_Get_With_NotRegisteredPaths_With_PathArgs()
            {
                var errors = new Dictionary<int, IError>()
                {
                    [0] = new Error()
                    {
                        Messages = new[]
                        {
                            "name11: {_name}",
                            "path12: {_path}"
                        },
                        Args = Array.Empty<IArg>()
                    },
                    [1] = new Error()
                    {
                        Messages = new[]
                        {
                            "name21: {_name}",
                            "path22: {_path}"
                        },
                        Args = Array.Empty<IArg>()
                    }
                };

                var messagesService = new MessagesService(DefaultTranslations, errors, DefaultErrorsMap);

                var result = messagesService.GetErrorsMessages(
                    new Dictionary<string, List<int>>()
                    {
                        ["some.not.registered.path"] = new List<int>()
                        {
                            0
                        },
                        ["another.path.#2.with.indexes.#12"] = new List<int>()
                        {
                            0,
                            1
                        },
                    },
                    "translation2");

                result.Count.Should().Be(2);

                result.Keys.Should().Contain("some.not.registered.path");
                result["some.not.registered.path"].Count.Should().Be(2);
                result["some.not.registered.path"].ElementAt(0).Should().Be("name11: path");
                result["some.not.registered.path"].ElementAt(1).Should().Be("path12: some.not.registered.path");

                result.Keys.Should().Contain("another.path.#2.with.indexes.#12");
                result["another.path.#2.with.indexes.#12"].Count.Should().Be(4);
                result["another.path.#2.with.indexes.#12"].ElementAt(0).Should().Be("name11: #12");
                result["another.path.#2.with.indexes.#12"].ElementAt(1).Should().Be("path12: another.path.#2.with.indexes.#12");
                result["another.path.#2.with.indexes.#12"].ElementAt(2).Should().Be("name21: #12");
                result["another.path.#2.with.indexes.#12"].ElementAt(3).Should().Be("path22: another.path.#2.with.indexes.#12");
            }
        }
    }
}
