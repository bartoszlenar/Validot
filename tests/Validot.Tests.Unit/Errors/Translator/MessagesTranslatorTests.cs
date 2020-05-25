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

    public class MessagesTranslatorTests
    {
        public class TranslateErrorMessagesWithPathPlaceholders
        {
            private static readonly ArgPlaceholder ParameterlessNamePlaceholders = new ArgPlaceholder()
            {
                Name = "_name",
                Placeholder = "{_name}",
                Parameters = new Dictionary<string, string>()
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

                Action action = () => MessagesTranslator.TranslateErrorMessagesWithPathPlaceholders(null, errorMessages, indexedPathsPlaceholders);

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

                Action action = () => MessagesTranslator.TranslateErrorMessagesWithPathPlaceholders("path", null, indexedPathsPlaceholders);

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

                Action action = () => MessagesTranslator.TranslateErrorMessagesWithPathPlaceholders("path", errorMessages, null);

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

                var results = MessagesTranslator.TranslateErrorMessagesWithPathPlaceholders("some.path", errorMessages, indexedPathsPlaceholders);

                results.Count.Should().Be(5);

                results[0].Should().Be("message1");
                results[1].Should().Be("message2 path some.path");
                results[2].Should().Be("message3");
                results[3].Should().Be("message4 path");
                results[4].Should().Be("message5 some.path");
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

                var results = MessagesTranslator.TranslateErrorMessagesWithPathPlaceholders("some.path", errorMessages, indexedPathsPlaceholders);

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

                var results = MessagesTranslator.TranslateErrorMessagesWithPathPlaceholders("some.path", errorMessages, indexedPathsPlaceholders);

                results.Count.Should().Be(5);

                results[0].Should().Be("message1");
                results[1].Should().Be("message2 path {_path}");
                results[2].Should().Be("message3");
                results[3].Should().Be("message4 {_name}");
                results[4].Should().Be("message5 some.path");
            }
        }

        public class TranslateErrorMessages
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

                var translator = new MessagesTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1"
                    }
                };

                Action action = () => translator.TranslateErrorMessages(null, error);

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

                var translator = new MessagesTranslator(translations);

                Action action = () => translator.TranslateErrorMessages("translation1", null);

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

                var translator = new MessagesTranslator(translations);

                var error = new Error()
                {
                    Messages = new[]
                    {
                        "key1"
                    },
                    Args = new IArg[] { }
                };

                Action action = () => translator.TranslateErrorMessages("translation3", error);

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

                var translator = new MessagesTranslator(translations);

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

                Action action = () => translator.TranslateErrorMessages("translation1", error);

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

                var translator = new MessagesTranslator(translations);

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

                var result = translator.TranslateErrorMessages("translation2", error);

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

                var translator = new MessagesTranslator(translations);

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

                var result = translator.TranslateErrorMessages("translation2", error);

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

                var translator = new MessagesTranslator(translations);

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

                var result = translator.TranslateErrorMessages("translation2", error);

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

                var translator = new MessagesTranslator(translations);

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

                var result = translator.TranslateErrorMessages("translation2", error);

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

                var translator = new MessagesTranslator(translations);

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

                var result = translator.TranslateErrorMessages("translation1", error);

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

                var translator = new MessagesTranslator(translations);

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

                var result = translator.TranslateErrorMessages("translation1", error);

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

                var translator = new MessagesTranslator(translations);

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

                var result = translator.TranslateErrorMessages("translation1", error);

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

                var translator = new MessagesTranslator(translations);

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

                var result = translator.TranslateErrorMessages("translation1", error);

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

                var translator = new MessagesTranslator(translations);

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

                Action action = () => translator.TranslateErrorMessages("translation1", error);

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new MessagesTranslator(new Dictionary<string, IReadOnlyDictionary<string, string>>());
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

            var translator = new MessagesTranslator(translations);

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

            var translator = new MessagesTranslator(translations);

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

            var translator = new MessagesTranslator(translations);

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

            var translator = new MessagesTranslator(translations);

            translator.TranslationsArgs.Should().NotBeEmpty();
            translator.TranslationsArgs.Count.Should().Be(2);

            translator.TranslationsArgs.Keys.Should().Contain("translation1");

            translator.TranslationsArgs["translation1"].Length.Should().Be(1);
            translator.TranslationsArgs["translation1"][0].Should().BeAssignableTo<TranslationArg>();
            translator.TranslationsArgs["translation1"][0].Name.Should().Be("_translation");

            translator.TranslationsArgs["translation1"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key11"
            }).Should().Be("message11");

            translator.TranslationsArgs["translation1"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key12"
            }).Should().Be("message12");

            translator.TranslationsArgs["translation1"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key21"
            }).Should().Be("key21");

            translator.TranslationsArgs["translation2"].Length.Should().Be(1);
            translator.TranslationsArgs["translation2"][0].Should().BeAssignableTo<TranslationArg>();
            translator.TranslationsArgs["translation2"][0].Name.Should().Be("_translation");

            translator.TranslationsArgs["translation2"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key11"
            }).Should().Be("key11");

            translator.TranslationsArgs["translation2"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key12"
            }).Should().Be("key12");

            translator.TranslationsArgs["translation2"][0].ToString(new Dictionary<string, string>()
            {
                ["key"] = "key21"
            }).Should().Be("message21");
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_NullEntryInTranslation()
        {
            Action action = () => new MessagesTranslator(new Dictionary<string, IReadOnlyDictionary<string, string>>
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
            Action action = () => new MessagesTranslator(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Should_ThrowException_When_Initialize_With_SingleNullTranslations()
        {
            Action action = () => new MessagesTranslator(new Dictionary<string, IReadOnlyDictionary<string, string>>
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
