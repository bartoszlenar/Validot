namespace Validot.Tests.Unit.Errors.Translator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Validot.Errors;
    using Validot.Errors.Args;
    using Validot.Errors.Translator;

    using Xunit;

    public class MessageTranslatorTests
    {
        public class TranslateMessagesWithPathPlaceholders
        {
            private static readonly ArgPlaceholder ParameterlessNamePlaceholders = new ArgPlaceholder()
            {
                Name = "_name",
                Placeholder = "{_name}",
                Parameters = new Dictionary<string, string>()
            };

            private static readonly ArgPlaceholder TitleCaseNamePlaceholders = new ArgPlaceholder()
            {
                Name = "_name",
                Placeholder = "{_name|format=titleCase}",
                Parameters = new Dictionary<string, string>()
                {
                    ["format"] = "titleCase"
                }
            };

            private static readonly ArgPlaceholder ParameterlessPathPlaceholders = new ArgPlaceholder()
            {
                Name = "_path",
                Placeholder = "{_path}",
                Parameters = new Dictionary<string, string>()
            };

            [Fact]
            public void Should_ThrowException_When_Path_IsNull()
            {
                var errorMessages = new[]
                {
                    "message1",
                    "message2 {_path}"
                };

                var indexedPathsPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        ParameterlessNamePlaceholders
                    }
                };

                Action action = () => MessageTranslator.TranslateMessagesWithPathPlaceholders(null, errorMessages, indexedPathsPlaceholders);

                action.Should().ThrowExactly<NullReferenceException>();
            }

            [Fact]
            public void Should_ThrowException_When_ErrorMessages_IsNull()
            {
                var indexedPathsPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        ParameterlessNamePlaceholders
                    }
                };

                Action action = () => MessageTranslator.TranslateMessagesWithPathPlaceholders("path", null, indexedPathsPlaceholders);

                action.Should().ThrowExactly<NullReferenceException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullIndexedPathHolders()
            {
                var errorMessages = new[]
                {
                    "message1",
                    "message2 {_path}"
                };

                Action action = () => MessageTranslator.TranslateMessagesWithPathPlaceholders("path", errorMessages, null);

                action.Should().ThrowExactly<NullReferenceException>();
            }

            [Fact]
            public void Should_Translate_WithPathArgs()
            {
                var errorMessages = new[]
                {
                    "message1",
                    "message2 {_name} {_path}",
                    "message3",
                    "message4 {_name}",
                    "message5 {_path}",
                };

                var indexedPathsPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        ParameterlessNamePlaceholders,
                        ParameterlessPathPlaceholders
                    },
                    [3] = new[]
                    {
                        ParameterlessNamePlaceholders
                    },
                    [4] = new[]
                    {
                        ParameterlessPathPlaceholders
                    }
                };

                var results = MessageTranslator.TranslateMessagesWithPathPlaceholders("some.path", errorMessages, indexedPathsPlaceholders);

                results.Count.Should().Be(5);

                results[0].Should().Be("message1");
                results[1].Should().Be("message2 path some.path");
                results[2].Should().Be("message3");
                results[3].Should().Be("message4 path");
                results[4].Should().Be("message5 some.path");
            }

            [Fact]
            public void Should_Translate_WithPathArgs_WithParameters()
            {
                var errorMessages = new[]
                {
                    "message1",
                    "message2 {_name|format=titleCase} {_path}",
                    "message3 >{_name}<",
                    "message4 '{_name|format=titleCase}'",
                    "message5 {_path}",
                };

                var indexedPathsPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        TitleCaseNamePlaceholders,
                        ParameterlessPathPlaceholders
                    },
                    [2] = new[]
                    {
                        ParameterlessNamePlaceholders
                    },
                    [3] = new[]
                    {
                        TitleCaseNamePlaceholders
                    },
                    [4] = new[]
                    {
                        ParameterlessPathPlaceholders
                    }
                };

                var results = MessageTranslator.TranslateMessagesWithPathPlaceholders("some.path.veryImportantPath", errorMessages, indexedPathsPlaceholders);

                results.Count.Should().Be(5);

                results[0].Should().Be("message1");
                results[1].Should().Be("message2 Very Important Path some.path.veryImportantPath");
                results[2].Should().Be("message3 >veryImportantPath<");
                results[3].Should().Be("message4 'Very Important Path'");
                results[4].Should().Be("message5 some.path.veryImportantPath");
            }

            public static IEnumerable<object[]> Should_Translate_WithNameArg_Data()
            {
                yield return new object[] { "someWeirdName123", TitleCaseNamePlaceholders, "Message {_name|format=titleCase}", "Message Some Weird Name 123" };
                yield return new object[] { "nested.path.someWeirdName123", TitleCaseNamePlaceholders, "Message {_name|format=titleCase}", "Message Some Weird Name 123" };
                yield return new object[] { "very.nested.path.SetSlot123ToInput456", TitleCaseNamePlaceholders, "Message >{_name|format=titleCase}<", "Message >Set Slot 123 To Input 456<" };
                yield return new object[] { "path.This_is_a_Test_of_Network123_in_12_days", TitleCaseNamePlaceholders, "XXX ### {_name|format=titleCase} ### XXX", "XXX ### This Is A Test Of Network 123 In 12 Days ### XXX" };
            }

            [Theory]
            [MemberData(nameof(Should_Translate_WithNameArg_Data))]
            public void Should_Translate_WithNameArg(string path, ArgPlaceholder placeholder, string message, string expectedTranslatedMessage)
            {
                var indexedPathsPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [0] = new[]
                    {
                        placeholder
                    }
                };

                var results = MessageTranslator.TranslateMessagesWithPathPlaceholders(path, new[] { message }, indexedPathsPlaceholders);

                results.Count.Should().Be(1);

                results[0].Should().Be(expectedTranslatedMessage);
            }

            [Fact]
            public void Should_Translate_WithPathArgs_And_Leave_When_MissingPlaceholders()
            {
                var errorMessages = new[]
                {
                    "message1",
                    "message2 {_name} {_path}",
                    "message3",
                    "message4 {_name}",
                    "message5 {_path}",
                };

                var indexedPathsPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        ParameterlessNamePlaceholders,
                    },
                    [4] = new[]
                    {
                        ParameterlessPathPlaceholders
                    }
                };

                var results = MessageTranslator.TranslateMessagesWithPathPlaceholders("some.path", errorMessages, indexedPathsPlaceholders);

                results.Count.Should().Be(5);

                results[0].Should().Be("message1");
                results[1].Should().Be("message2 path {_path}");
                results[2].Should().Be("message3");
                results[3].Should().Be("message4 {_name}");
                results[4].Should().Be("message5 some.path");
            }

            [Fact]
            public void Should_Translate_WithPathArgs_And_Leave_When_InvalidPlaceholders()
            {
                var errorMessages = new[]
                {
                    "message1",
                    "message2 {_name} {_path}",
                    "message3",
                    "message4 {_name}",
                    "message5 {_path}",
                };

                var indexedPathsPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        ParameterlessNamePlaceholders,
                        new ArgPlaceholder()
                        {
                            Name = "_invalid",
                            Placeholder = "{_invalid}"
                        }
                    },
                    [2] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "_invalid",
                            Placeholder = "{_invalid}"
                        }
                    },
                    [4] = new[]
                    {
                        ParameterlessPathPlaceholders,
                        new ArgPlaceholder()
                        {
                            Name = "_invalid",
                            Placeholder = "{_invalid}"
                        }
                    }
                };

                var results = MessageTranslator.TranslateMessagesWithPathPlaceholders("some.path", errorMessages, indexedPathsPlaceholders);

                results.Count.Should().Be(5);

                results[0].Should().Be("message1");
                results[1].Should().Be("message2 path {_path}");
                results[2].Should().Be("message3");
                results[3].Should().Be("message4 {_name}");
                results[4].Should().Be("message5 some.path");
            }
        }

        public class TranslateMessages
        {
            [Fact]
            public void Should_ThrowException_When_NullTranslationName()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1"
                    },
                    ["translation2"] = new Dictionary<string, string>
                    {
                        ["key2"] = "message2"
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1"
                    }
                };

                Action action = () => translator.TranslateMessages(null, error);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullError()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1"
                    },
                    ["translation2"] = new Dictionary<string, string>
                    {
                        ["key2"] = "message2"
                    }
                };

                var translator = new MessageTranslator(translations);

                Action action = () => translator.TranslateMessages("translation1", null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_TranslationNameNotFound()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1"
                    },
                    ["translation2"] = new Dictionary<string, string>
                    {
                        ["key2"] = "message2"
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1"
                    },
                    Args = new IArg[] { }
                };

                Action action = () => translator.TranslateMessages("translation3", error);

                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_Error_ContainsNullMessage()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1"
                    },
                    ["translation2"] = new Dictionary<string, string>
                    {
                        ["key2"] = "message2"
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1",
                        null
                    },
                    Args = new IArg[]
                    {
                        Arg.Text("test", "test")
                    }
                };

                Action action = () => translator.TranslateMessages("translation1", error);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_Translate_UsingSelectedTranslation_When_KeyInTranslation()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1"
                    },
                    ["translation2"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message2"
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1"
                    },
                    Args = new IArg[]
                    {
                    }
                };

                var result = translator.TranslateMessages("translation2", error);

                result.Messages.Count.Should().Be(1);
                result.Messages.Should().Contain("message2");

                result.AnyPathPlaceholders.Should().Be(false);
                result.IndexedPathPlaceholders.Should().BeEmpty();
            }

            [Fact]
            public void Should_Translate_ReturnSameMessage_When_KeyInTranslation()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1"
                    },
                    ["translation2"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message2"
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key123"
                    },
                    Args = new IArg[]
                    {
                    }
                };

                var result = translator.TranslateMessages("translation2", error);

                result.Messages.Count.Should().Be(1);
                result.Messages.Should().Contain("key123");

                result.AnyPathPlaceholders.Should().Be(false);
                result.IndexedPathPlaceholders.Should().BeEmpty();
            }

            [Fact]
            public void Should_Translate_UsingTranslationForExistingKeys_And_ReturnSameMessageForKeyInTranslation()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1",
                        ["key2"] = "message2",
                        ["key3"] = "message3"
                    },
                    ["translation2"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message11",
                        ["key2"] = "message22",
                        ["key3"] = "message33"
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1",
                        "key2",
                        "key123"
                    },
                    Args = new IArg[]
                    {
                    }
                };

                var result = translator.TranslateMessages("translation2", error);

                result.Messages.Count.Should().Be(3);
                result.Messages.Should().Contain("message11");
                result.Messages.Should().Contain("message22");
                result.Messages.Should().Contain("key123");

                result.AnyPathPlaceholders.Should().Be(false);
                result.IndexedPathPlaceholders.Should().BeEmpty();
            }

            [Fact]
            public void Should_Translate_And_UseArgs()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1 {arg1}",
                        ["key2"] = "message2 {arg2}"
                    },
                    ["translation2"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message11 {arg1}",
                        ["key2"] = "message22 {arg1} {arg2}",
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1",
                        "key2",
                    },
                    Args = new IArg[]
                    {
                        Arg.Text("arg1", "arg1Value"),
                        Arg.Text("arg2", "arg2Value")
                    }
                };

                var result = translator.TranslateMessages("translation2", error);

                result.Messages.Count.Should().Be(2);
                result.Messages.Should().Contain("message11 arg1Value");
                result.Messages.Should().Contain("message22 arg1Value arg2Value");

                result.AnyPathPlaceholders.Should().Be(false);
                result.IndexedPathPlaceholders.Should().BeEmpty();
            }

            [Fact]
            public void Should_Translate_And_UseArgs_WithParameters()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1 {arg1|case=upper} {arg1|case=lower}"
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1",
                    },
                    Args = new IArg[]
                    {
                        Arg.Text("arg1", "arg1Value"),
                    }
                };

                var result = translator.TranslateMessages("translation1", error);

                result.Messages.Count.Should().Be(1);
                result.Messages.Should().Contain("message1 ARG1VALUE arg1value");

                result.AnyPathPlaceholders.Should().Be(false);
                result.IndexedPathPlaceholders.Should().BeEmpty();
            }

            [Fact]
            public void Should_Translate_And_UseArgs_Special_Translation()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1 {_translation|key=key2}",
                        ["key2"] = "message2"
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1",
                    },
                    Args = new IArg[]
                    {
                    }
                };

                var result = translator.TranslateMessages("translation1", error);

                result.Messages.Count.Should().Be(1);
                result.Messages.Should().Contain("message1 message2");

                result.AnyPathPlaceholders.Should().Be(false);
                result.IndexedPathPlaceholders.Should().BeEmpty();
            }

            [Fact]
            public void Should_Translate_And_UseArgs_Special_Translation_Recursion()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1 {_translation|key=key1}",
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1",
                    },
                    Args = new IArg[]
                    {
                    }
                };

                var result = translator.TranslateMessages("translation1", error);

                result.Messages.Count.Should().Be(1);
                result.Messages.Should().Contain("message1 message1 {_translation|key=key1}");

                result.AnyPathPlaceholders.Should().Be(false);
                result.IndexedPathPlaceholders.Should().BeEmpty();
            }

            [Fact]
            public void Should_Translate_And_ExtractPathPlaceholders()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1",
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1",
                        "message with path: {_path}",
                        "message with name: {_name}",
                        "message with path and name: {_path} {_name}",
                    },
                    Args = new IArg[]
                    {
                    }
                };

                var result = translator.TranslateMessages("translation1", error);

                result.Messages.Count.Should().Be(4);
                result.Messages.ElementAt(0).Should().Be("message1");
                result.Messages.ElementAt(1).Should().Be("message with path: {_path}");
                result.Messages.ElementAt(2).Should().Be("message with name: {_name}");
                result.Messages.ElementAt(3).Should().Be("message with path and name: {_path} {_name}");

                result.AnyPathPlaceholders.Should().Be(true);
                result.IndexedPathPlaceholders.Count.Should().Be(3);

                result.IndexedPathPlaceholders.Keys.Should().Contain(1);
                result.IndexedPathPlaceholders.Keys.Should().Contain(2);
                result.IndexedPathPlaceholders.Keys.Should().Contain(3);

                result.IndexedPathPlaceholders[1].Count.Should().Be(1);
                result.IndexedPathPlaceholders[1].ElementAt(0).Name.Should().Be("_path");
                result.IndexedPathPlaceholders[1].ElementAt(0).Placeholder.Should().Be("{_path}");
                result.IndexedPathPlaceholders[1].ElementAt(0).Parameters.Should().BeEmpty();

                result.IndexedPathPlaceholders[2].Count.Should().Be(1);
                result.IndexedPathPlaceholders[2].ElementAt(0).Name.Should().Be("_name");
                result.IndexedPathPlaceholders[2].ElementAt(0).Placeholder.Should().Be("{_name}");
                result.IndexedPathPlaceholders[2].ElementAt(0).Parameters.Should().BeEmpty();

                result.IndexedPathPlaceholders[3].Count.Should().Be(2);
                result.IndexedPathPlaceholders[3].ElementAt(0).Name.Should().Be("_path");
                result.IndexedPathPlaceholders[3].ElementAt(0).Placeholder.Should().Be("{_path}");
                result.IndexedPathPlaceholders[3].ElementAt(0).Parameters.Should().BeEmpty();
                result.IndexedPathPlaceholders[3].ElementAt(1).Name.Should().Be("_name");
                result.IndexedPathPlaceholders[3].ElementAt(1).Placeholder.Should().Be("{_name}");
                result.IndexedPathPlaceholders[3].ElementAt(1).Parameters.Should().BeEmpty();
            }

            [Fact]
            public void Should_ThrowException_When_Error_ContainsNullArg()
            {
                var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
                {
                    ["translation1"] = new Dictionary<string, string>
                    {
                        ["key1"] = "message1"
                    },
                    ["translation2"] = new Dictionary<string, string>
                    {
                        ["key2"] = "message2"
                    }
                };

                var translator = new MessageTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1",
                    },
                    Args = new IArg[]
                    {
                        Arg.Text("test", "test"),
                        null
                    }
                };

                Action action = () => translator.TranslateMessages("translation1", error);

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new MessageTranslator(new Dictionary<string, IReadOnlyDictionary<string, string>>());
        }

        [Fact]
        public void Should_Set_TranslationNames()
        {
            var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["translation1"] = new Dictionary<string, string>
                {
                    ["key1"] = "message1"
                },
                ["translation2"] = new Dictionary<string, string>
                {
                    ["key2"] = "message2"
                },
                ["translation3"] = new Dictionary<string, string>
                {
                    ["key3"] = "message3"
                },
                ["translation4"] = new Dictionary<string, string>
                {
                    ["key4"] = "message4"
                }
            };

            var translator = new MessageTranslator(translations);

            translator.TranslationNames.Should().NotBeNull();
            translator.TranslationNames.Count.Should().Be(4);

            translator.TranslationNames.Should().Contain("translation1");
            translator.TranslationNames.Should().Contain("translation2");
            translator.TranslationNames.Should().Contain("translation3");
            translator.TranslationNames.Should().Contain("translation4");
        }

        [Fact]
        public void Should_Set_Translations()
        {
            var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["translation1"] = new Dictionary<string, string>
                {
                    ["key1"] = "message1",
                    ["key2"] = "message2"
                },
                ["translation2"] = new Dictionary<string, string>
                {
                    ["key3"] = "message3",
                    ["key4"] = "message4"
                }
            };

            var translator = new MessageTranslator(translations);

            translator.Translations.Should().BeSameAs(translations);
        }

        [Fact]
        public void Should_Set_Translations_WithoutModification()
        {
            var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["translation1"] = new Dictionary<string, string>
                {
                    ["key1"] = "message1",
                    ["key2"] = "message2"
                },
                ["translation2"] = new Dictionary<string, string>
                {
                    ["key3"] = "message3",
                    ["key4"] = "message4"
                }
            };

            var translator = new MessageTranslator(translations);

            translator.Translations.Keys.Should().Contain("translation1");
            translator.Translations.Keys.Count().Should().Be(2);

            translator.Translations["translation1"].Keys.Count().Should().Be(2);
            translator.Translations["translation1"].Keys.Should().Contain("key1");
            translator.Translations["translation1"]["key1"].Should().Be("message1");

            translator.Translations["translation1"].Keys.Should().Contain("key2");
            translator.Translations["translation1"]["key2"].Should().Be("message2");

            translator.Translations["translation2"].Keys.Count().Should().Be(2);
            translator.Translations["translation2"].Keys.Should().Contain("key3");
            translator.Translations["translation2"]["key3"].Should().Be("message3");

            translator.Translations["translation2"].Keys.Should().Contain("key4");
            translator.Translations["translation2"]["key4"].Should().Be("message4");
        }

        [Fact]
        public void Should_Set_TranslationArgs()
        {
            var translations = new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["translation1"] = new Dictionary<string, string>
                {
                    ["key11"] = "message11",
                    ["key12"] = "message12"
                },
                ["translation2"] = new Dictionary<string, string>
                {
                    ["key21"] = "message21"
                },
            };

            var translator = new MessageTranslator(translations);

            translator.TranslationArgs.Should().NotBeEmpty();
            translator.TranslationArgs.Count.Should().Be(2);

            translator.TranslationArgs.Keys.Should().Contain("translation1");

            translator.TranslationArgs["translation1"].Length.Should().Be(1);
            translator.TranslationArgs["translation1"][0].Should().BeAssignableTo<TranslationArg>();
            translator.TranslationArgs["translation1"][0].Name.Should().Be("_translation");

            translator.TranslationArgs["translation1"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key11"
            }).Should().Be("message11");

            translator.TranslationArgs["translation1"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key12"
            }).Should().Be("message12");

            translator.TranslationArgs["translation1"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key21"
            }).Should().Be("key21");

            translator.TranslationArgs["translation2"].Length.Should().Be(1);
            translator.TranslationArgs["translation2"][0].Should().BeAssignableTo<TranslationArg>();
            translator.TranslationArgs["translation2"][0].Name.Should().Be("_translation");

            translator.TranslationArgs["translation2"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key11"
            }).Should().Be("key11");

            translator.TranslationArgs["translation2"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key12"
            }).Should().Be("key12");

            translator.TranslationArgs["translation2"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key21"
            }).Should().Be("message21");
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_NullEntryInTranslation()
        {
            Action action = () => new MessageTranslator(new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["translation1"] = new Dictionary<string, string>
                {
                    ["key1"] = "message1",
                    ["key2"] = "message2"
                },
                ["translation2"] = new Dictionary<string, string>
                {
                    ["key1"] = "message1",
                    ["key2"] = null
                }
            });

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_NullTranslations()
        {
            Action action = () => new MessageTranslator(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_SingleNullTranslations()
        {
            Action action = () => new MessageTranslator(new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                ["translation1"] = new Dictionary<string, string>
                {
                    ["key1"] = "message1",
                    ["key2"] = "message2"
                },
                ["translation2"] = null
            });

            action.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
