namespace Validot.Tests.Unit.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Validot.Errors;
    using Validot.Errors.Args;

    using Xunit;

    public class MessagesCacheTests
    {
        public class AddMessage
        {
            [Fact]
            public void Should_AddMessage()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage(
                    "trans1",
                    1,
                    new[]
                    {
                        "message1",
                        "message2"
                    });

                var cachedMessages = messagesCache.GetMessages("trans1", 1);

                cachedMessages.Count.Should().Be(2);
                cachedMessages.Should().Contain("message1");
                cachedMessages.Should().Contain("message2");
            }

            [Fact]
            public void Should_AddMessage_MultipleTimes_ToDifferentDictionaries()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage(
                    "trans1",
                    1,
                    new[]
                    {
                        "message1"
                    });

                messagesCache.AddMessage(
                    "trans2",
                    2,
                    new[]
                    {
                        "message2"
                    });

                var cachedMessages1 = messagesCache.GetMessages("trans1", 1);
                cachedMessages1.Count.Should().Be(1);
                cachedMessages1.Should().Contain("message1");

                var cachedMessages2 = messagesCache.GetMessages("trans2", 2);
                cachedMessages2.Count.Should().Be(1);
                cachedMessages2.Should().Contain("message2");
            }

            [Fact]
            public void Should_AddMessage_MultipleTimes_ToDifferentDictionaries_SameErrorId()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage(
                    "trans1",
                    1,
                    new[]
                    {
                        "message1"
                    });

                messagesCache.AddMessage(
                    "trans2",
                    1,
                    new[]
                    {
                        "message2"
                    });

                var cachedMessages1 = messagesCache.GetMessages("trans1", 1);
                cachedMessages1.Count.Should().Be(1);
                cachedMessages1.Should().Contain("message1");

                var cachedMessages2 = messagesCache.GetMessages("trans2", 1);
                cachedMessages2.Count.Should().Be(1);
                cachedMessages2.Should().Contain("message2");
            }

            [Fact]
            public void Should_ThrowException_When_AddingMultipleTimes_ToSameDictionary_And_ToSameError()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage(
                    "trans1",
                    1,
                    new[]
                    {
                        "message1"
                    });

                Action action = () => messagesCache.AddMessage(
                    "trans1",
                    1,
                    new[]
                    {
                        "message2"
                    });

                action.Should().ThrowExactly<ArgumentException>();
            }

            [Fact]
            public void Should_ThrowException_When_MessagesCollectionContainsNull()
            {
                var messagesCache = new MessagesCache();

                Action action = () => messagesCache.AddMessage(
                    "trans1",
                    1,
                    new[]
                    {
                        "message1",
                        null
                    });

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullMessagesCollection()
            {
                var messagesCache = new MessagesCache();

                Action action = () => messagesCache.AddMessage(
                    "trans1",
                    1,
                    null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullTranslationName()
            {
                var messagesCache = new MessagesCache();

                Action action = () => messagesCache.AddMessage(
                    null,
                    1,
                    new[]
                    {
                        "message2"
                    });

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class GetMessages
        {
            [Fact]
            public void Should_GetMessages()
            {
                var messagesCache = new MessagesCache();

                var messages = new[]
                {
                    "message1",
                    "message2",
                    "message3"
                };

                messagesCache.AddMessage(
                    "trans1",
                    1,
                    messages);

                var cachedMessages1 = messagesCache.GetMessages("trans1", 1);

                cachedMessages1.Should().BeSameAs(messages);
                cachedMessages1.Count.Should().Be(3);
                cachedMessages1.Should().Contain("message1");
                cachedMessages1.Should().Contain("message2");
                cachedMessages1.Should().Contain("message3");
            }

            [Fact]
            public void Should_ThrowException_When_InvalidErrorId()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessage("translation", 1, new[]
                {
                    "message"
                });

                Action action = () =>
                {
                    messageCache.GetMessages("translation", 123);
                };

                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_InvalidTranslationName()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessage("translation", 1, new[]
                {
                    "message"
                });

                Action action = () =>
                {
                    messageCache.GetMessages("invalid_translation", 1);
                };

                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullTranslationName()
            {
                var messageCache = new MessagesCache();

                Action action = () =>
                {
                    messageCache.GetMessages(null, 1);
                };

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class GetErrorsMessagesAmount
        {
            public static IEnumerable<object[]> Should_GetErrorsMessagesAmount_Data()
            {
                var errorsMessages = new Dictionary<int, string[]>
                {
                    [1] = new[]
                    {
                        "1",
                        "2",
                        "3"
                    },
                    [2] = new[]
                    {
                        "1",
                        "2",
                        "3",
                        "4",
                        "5"
                    },
                    [3] = new[]
                    {
                        "1",
                        "2",
                        "3",
                        "4",
                        "5",
                        "6",
                        "7"
                    },
                    [4] = new[]
                    {
                        "1",
                        "2",
                        "3",
                        "4",
                        "5",
                        "6",
                        "7",
                        "8",
                        "9"
                    }
                };

                yield return new object[]
                {
                    errorsMessages,
                    new List<int>
                    {
                        1,
                        3,
                        4
                    },
                    19
                };

                yield return new object[]
                {
                    errorsMessages,
                    new List<int>
                    {
                        1
                    },
                    3
                };

                yield return new object[]
                {
                    errorsMessages,
                    new List<int>
                    {
                        2,
                        4
                    },
                    14
                };

                yield return new object[]
                {
                    errorsMessages,
                    new List<int>
                    {
                        1,
                        2
                    },
                    8
                };
            }

            [Theory]
            [MemberData(nameof(Should_GetErrorsMessagesAmount_Data))]
            public void Should_GetErrorsMessagesAmount(Dictionary<int, string[]> errorMessages, List<int> errorsIdsToCheck, int expectedErrorsAmount)
            {
                var messagesCache = new MessagesCache();

                foreach (KeyValuePair<int, string[]> pair in errorMessages)
                {
                    messagesCache.AddMessage("translation", pair.Key, pair.Value);
                }

                var amount = messagesCache.GetErrorsMessagesAmount(errorsIdsToCheck);

                amount.Should().Be(expectedErrorsAmount);
            }

            [Fact]
            public void Should_NotModifyList()
            {
                var messagesCache = new MessagesCache();

                var list = new List<int>
                {
                    1,
                    2,
                    3,
                    4,
                    5
                };

                foreach (var id in list)
                {
                    messagesCache.AddMessage("translation", id, new[]
                    {
                        "message "
                    });
                }

                messagesCache.GetErrorsMessagesAmount(list);

                list.Count.Should().Be(5);

                list.ElementAt(0).Should().Be(1);
                list.ElementAt(1).Should().Be(2);
                list.ElementAt(2).Should().Be(3);
                list.ElementAt(3).Should().Be(4);
                list.ElementAt(4).Should().Be(5);
            }

            [Fact]
            public void Should_ThrowException_When_NullIdsList()
            {
                var messageCache = new MessagesCache();

                Action action = () =>
                {
                    messageCache.GetErrorsMessagesAmount(null);
                };

                action.Should().ThrowExactly<NullReferenceException>();
            }
        }

        public class AddMessageWithPathArgs
        {
            [Fact]
            public void Should_AddMessageWithPathArgs_MultipleErrors()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                messageCache.AddMessageWithPathArgs("translation", "path", 2, new[]
                {
                    "message2"
                });

                var messages1 = messageCache.GetMessagesWithPathArgs("translation", "path", 1);

                messages1.Count.Should().Be(1);
                messages1.ElementAt(0).Should().Be("message1");

                var messages2 = messageCache.GetMessagesWithPathArgs("translation", "path", 2);

                messages2.Count.Should().Be(1);
                messages2.ElementAt(0).Should().Be("message2");
            }

            [Fact]
            public void Should_AddMessageWithPathArgs_MultiplePaths()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path1", 1, new[]
                {
                    "message1"
                });

                messageCache.AddMessageWithPathArgs("translation", "path1", 2, new[]
                {
                    "message2"
                });

                messageCache.AddMessageWithPathArgs("translation", "path2", 1, new[]
                {
                    "message3"
                });

                var messages1 = messageCache.GetMessagesWithPathArgs("translation", "path1", 1);
                messages1.Count.Should().Be(1);
                messages1.ElementAt(0).Should().Be("message1");

                var messages2 = messageCache.GetMessagesWithPathArgs("translation", "path1", 2);
                messages2.Count.Should().Be(1);
                messages2.ElementAt(0).Should().Be("message2");

                var messages3 = messageCache.GetMessagesWithPathArgs("translation", "path2", 1);
                messages3.Count.Should().Be(1);
                messages3.ElementAt(0).Should().Be("message3");
            }

            [Fact]
            public void Should_AddMessageWithPathArgs_MultipleTranslations()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation1", "path1", 1, new[]
                {
                    "message1"
                });

                messageCache.AddMessageWithPathArgs("translation1", "path1", 2, new[]
                {
                    "message2"
                });

                messageCache.AddMessageWithPathArgs("translation1", "path2", 1, new[]
                {
                    "message3"
                });

                messageCache.AddMessageWithPathArgs("translation2", "path1", 1, new[]
                {
                    "message4"
                });

                var messages1 = messageCache.GetMessagesWithPathArgs("translation1", "path1", 1);
                messages1.Count.Should().Be(1);
                messages1.ElementAt(0).Should().Be("message1");

                var messages2 = messageCache.GetMessagesWithPathArgs("translation1", "path1", 2);
                messages2.Count.Should().Be(1);
                messages2.ElementAt(0).Should().Be("message2");

                var messages3 = messageCache.GetMessagesWithPathArgs("translation1", "path2", 1);
                messages3.Count.Should().Be(1);
                messages3.ElementAt(0).Should().Be("message3");

                var messages4 = messageCache.GetMessagesWithPathArgs("translation2", "path1", 1);
                messages4.Count.Should().Be(1);
                messages4.ElementAt(0).Should().Be("message4");
            }

            [Fact]
            public void Should_AddMessageWithPathArgs_SingleError()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message"
                });

                var messages = messageCache.GetMessagesWithPathArgs("translation", "path", 1);

                messages.Count.Should().Be(1);
                messages.ElementAt(0).Should().Be("message");
            }

            [Fact]
            public void Should_ThrowException_When_AddMultipleTimesToSameError()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                Action action = () =>
                {
                    messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                    {
                        "message2"
                    });
                };

                action.Should().ThrowExactly<ArgumentException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullMessages()
            {
                var messageCache = new MessagesCache();

                Action action = () =>
                {
                    messageCache.AddMessageWithPathArgs("translation", null, 1, null);
                };

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_MessagesWithNull()
            {
                var messageCache = new MessagesCache();

                Action action = () =>
                {
                    messageCache.AddMessageWithPathArgs("translation", null, 1, new[]
                    {
                        "message",
                        null
                    });
                };

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                var messageCache = new MessagesCache();

                Action action = () =>
                {
                    messageCache.AddMessageWithPathArgs("translation", null, 1, new[]
                    {
                        "message"
                    });
                };

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullTranslationName()
            {
                var messageCache = new MessagesCache();

                Action action = () =>
                {
                    messageCache.AddMessageWithPathArgs(null, "path", 1, new[]
                    {
                        "message"
                    });
                };

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class GetMessagesWithPathArgs
        {
            [Fact]
            public void Should_GetMessagesWithPathArgs()
            {
                var messageCache = new MessagesCache();

                var messages = new[]
                {
                    "message1",
                    "message2"
                };

                messageCache.AddMessageWithPathArgs("translation", "path", 1, messages);

                var cachedMessages = messageCache.GetMessagesWithPathArgs("translation", "path", 1);

                cachedMessages.Should().BeSameAs(messages);

                cachedMessages.Count.Should().Be(2);
                cachedMessages.ElementAt(0).Should().Be("message1");
                cachedMessages.ElementAt(1).Should().Be("message2");
            }

            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                Action action = () => messageCache.GetMessagesWithPathArgs("translation", null, 1);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullTranslation()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                Action action = () => messageCache.GetMessagesWithPathArgs(null, "path", 1);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_InvalidErrorId()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                Action action = () => messageCache.GetMessagesWithPathArgs("translation", "path", 100);

                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_InvalidPath()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                Action action = () => messageCache.GetMessagesWithPathArgs("translation", "invalidPath", 1);

                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_InvalidTranslation()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                Action action = () => messageCache.GetMessagesWithPathArgs("invalidTranslation", "path", 1);

                action.Should().ThrowExactly<KeyNotFoundException>();
            }
        }

        public class IsMessageWithPathArgsCached
        {
            [Fact]
            public void Should_ThrowException_When_NullPath()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                Action action = () => messageCache.IsMessageWithPathArgsCached("translation", null, 1);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullTranslation()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                Action action = () => messageCache.IsMessageWithPathArgsCached(null, "path", 1);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ReturnTrue_When_MessageIsCached()
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                messageCache.AddMessageWithPathArgs("translation1", "path", 1, new[]
                {
                    "message1"
                });

                var result1 = messageCache.IsMessageWithPathArgsCached("translation", "path", 1);

                result1.Should().Be(true);

                var result2 = messageCache.IsMessageWithPathArgsCached("translation1", "path", 1);

                result2.Should().Be(true);
            }

            [Theory]
            [InlineData("translationName", "path", 2)]
            [InlineData("translationName", "invalidPath", 1)]
            [InlineData("invalidTranslationName", "path", 1)]
            public void Should_ReturnFalse_When_MessageIsNotCached(string translationName, string path, int errorId)
            {
                var messageCache = new MessagesCache();

                messageCache.AddMessageWithPathArgs("translation", "path", 1, new[]
                {
                    "message1"
                });

                var result = messageCache.IsMessageWithPathArgsCached(translationName, path, errorId);

                result.Should().Be(false);
            }
        }

        public class AddIndexedPathPlaceholders
        {
            [Fact]
            public void Should_AddIndexedPathPlaceholders()
            {
                var messageCache = new MessagesCache();

                messageCache.AddIndexedPathPlaceholders("translationName", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        }
                    }
                });

                var result = messageCache.GetIndexedPathPlaceholders("translationName", 1);

                result.Keys.Count().Should().Be(1);

                result.Keys.Single().Should().Be(1);

                result[1].Count.Should().Be(1);
                result[1].Single().Name.Should().Be("arg");
                result[1].Single().Placeholder.Should().Be("{arg|test=true}");
                result[1].Single().Parameters.Keys.Count().Should().Be(1);
                result[1].Single().Parameters.Keys.Single().Should().Be("test");
                result[1].Single().Parameters["test"].Should().Be("true");
            }

            [Fact]
            public void Should_AddIndexedPathPlaceholders_Multiple()
            {
                var messageCache = new MessagesCache();

                messageCache.AddIndexedPathPlaceholders("translationName1", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg1",
                            Placeholder = "{arg1|test1=true1}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test1"] = "true1"
                            }
                        }
                    }
                });

                messageCache.AddIndexedPathPlaceholders("translationName1", 2, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [2] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg2",
                            Placeholder = "{arg2|test2=true2}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test2"] = "true2"
                            }
                        }
                    }
                });

                messageCache.AddIndexedPathPlaceholders("translationName2", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [3] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg3",
                            Placeholder = "{arg3|test3=true3}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test3"] = "true3"
                            }
                        }
                    }
                });

                var result1 = messageCache.GetIndexedPathPlaceholders("translationName1", 1);

                result1.Keys.Count().Should().Be(1);

                result1.Keys.Single().Should().Be(1);

                result1[1].Count.Should().Be(1);
                result1[1].Single().Name.Should().Be("arg1");
                result1[1].Single().Placeholder.Should().Be("{arg1|test1=true1}");
                result1[1].Single().Parameters.Keys.Count().Should().Be(1);
                result1[1].Single().Parameters.Keys.Single().Should().Be("test1");
                result1[1].Single().Parameters["test1"].Should().Be("true1");

                var result2 = messageCache.GetIndexedPathPlaceholders("translationName1", 2);

                result2.Keys.Count().Should().Be(1);

                result2.Keys.Single().Should().Be(2);

                result2[2].Count.Should().Be(1);
                result2[2].Single().Name.Should().Be("arg2");
                result2[2].Single().Placeholder.Should().Be("{arg2|test2=true2}");
                result2[2].Single().Parameters.Keys.Count().Should().Be(1);
                result2[2].Single().Parameters.Keys.Single().Should().Be("test2");
                result2[2].Single().Parameters["test2"].Should().Be("true2");

                var result3 = messageCache.GetIndexedPathPlaceholders("translationName2", 1);

                result3.Keys.Count().Should().Be(1);

                result3.Keys.Single().Should().Be(3);
                result3[3].Count.Should().Be(1);
                result3[3].Single().Name.Should().Be("arg3");
                result3[3].Single().Placeholder.Should().Be("{arg3|test3=true3}");
                result3[3].Single().Parameters.Keys.Count().Should().Be(1);
                result3[3].Single().Parameters.Keys.Single().Should().Be("test3");
                result3[3].Single().Parameters["test3"].Should().Be("true3");
            }

            [Fact]
            public void Should_ThrowException_AddingMultiple_ToSameTranslationAndError()
            {
                var messageCache = new MessagesCache();

                messageCache.AddIndexedPathPlaceholders("translationName1", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg1",
                            Placeholder = "{arg1|test1=true1}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test1"] = "true1"
                            }
                        }
                    }
                });

                Action action = () => messageCache.AddIndexedPathPlaceholders("translationName1", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [2] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg2",
                            Placeholder = "{arg2|test2=true2}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test2"] = "true2"
                            }
                        }
                    }
                });

                action.Should().ThrowExactly<ArgumentException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullTranslation()
            {
                var messageCache = new MessagesCache();

                Action action = () => messageCache.AddIndexedPathPlaceholders(null, 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        }
                    }
                });

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullIndexedPlaceholders()
            {
                var messageCache = new MessagesCache();

                Action action = () => messageCache.AddIndexedPathPlaceholders("translation", 1, null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_IndexedPlaceholders_WithNullPlaceholdersArray()
            {
                var messageCache = new MessagesCache();

                Action action = () => messageCache.AddIndexedPathPlaceholders("translation", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        }
                    },
                    [2] = null
                });

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_IndexedPlaceholders_WithNullPlaceholderInArray()
            {
                var messageCache = new MessagesCache();

                Action action = () => messageCache.AddIndexedPathPlaceholders("translation", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        },
                        null
                    },
                });

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            public static IEnumerable<object[]> Should_ThrowException_When_IndexedPlaceholders_WithPlaceholder_WithNullValue_Data()
            {
                yield return new object[]
                {
                    new ArgPlaceholder(),
                };

                yield return new object[]
                {
                    new ArgPlaceholder()
                    {
                        Name = null,
                        Placeholder = "{arg|test=true}",
                        Parameters = new Dictionary<string, string>
                        {
                            ["test"] = "true"
                        }
                    },
                };

                yield return new object[]
                {
                    new ArgPlaceholder()
                    {
                        Name = "arg",
                        Placeholder = null,
                        Parameters = new Dictionary<string, string>
                        {
                            ["test"] = "true"
                        }
                    },
                };

                yield return new object[]
                {
                    new ArgPlaceholder()
                    {
                        Name = "arg",
                        Placeholder = "{arg|test=true}",
                        Parameters = null
                    },
                };

                yield return new object[]
                {
                    new ArgPlaceholder()
                    {
                        Name = "arg",
                        Placeholder = "{arg|test=true}",
                        Parameters = new Dictionary<string, string>
                        {
                            ["test"] = null
                        }
                    },
                };
            }

            [Theory]
            [MemberData(nameof(Should_ThrowException_When_IndexedPlaceholders_WithPlaceholder_WithNullValue_Data))]
            public void Should_ThrowException_When_IndexedPlaceholders_WithPlaceholder_WithNullValue(ArgPlaceholder argPlaceholder)
            {
                var messageCache = new MessagesCache();

                Action action = () => messageCache.AddIndexedPathPlaceholders("translation", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        argPlaceholder
                    },
                });

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class GetIndexedPathPlaceholders
        {
            [Fact]
            public void Should_GetIndexedPathPlaceholders()
            {
                var messageCache = new MessagesCache();

                var indexedPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        }
                    }
                };

                messageCache.AddIndexedPathPlaceholders("translationName", 1, indexedPlaceholders);

                var result = messageCache.GetIndexedPathPlaceholders("translationName", 1);

                result.Should().BeSameAs(indexedPlaceholders);
            }

            [Fact]
            public void Should_GetIndexedPathPlaceholders_DifferentTranslationNameAndError()
            {
                var messageCache = new MessagesCache();

                var indexedPlaceholders1 = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        }
                    }
                };

                var indexedPlaceholders2 = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        }
                    }
                };

                var indexedPlaceholders3 = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        }
                    }
                };

                messageCache.AddIndexedPathPlaceholders("translationName1", 1, indexedPlaceholders1);
                messageCache.AddIndexedPathPlaceholders("translationName1", 2, indexedPlaceholders2);
                messageCache.AddIndexedPathPlaceholders("translationName2", 1, indexedPlaceholders3);

                var result1 = messageCache.GetIndexedPathPlaceholders("translationName1", 1);
                result1.Should().BeSameAs(indexedPlaceholders1);

                var result2 = messageCache.GetIndexedPathPlaceholders("translationName1", 2);
                result2.Should().BeSameAs(indexedPlaceholders2);

                var result3 = messageCache.GetIndexedPathPlaceholders("translationName2", 1);
                result3.Should().BeSameAs(indexedPlaceholders3);
            }

            [Fact]
            public void Should_ThrowException_When_InvalidErrorId()
            {
                var messageCache = new MessagesCache();

                var indexedPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        }
                    }
                };

                messageCache.AddIndexedPathPlaceholders("translationName", 1, indexedPlaceholders);

                Action action = () => messageCache.GetIndexedPathPlaceholders("translationName", 2);

                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_InvalidTranslationName()
            {
                var messageCache = new MessagesCache();

                var indexedPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        }
                    }
                };

                messageCache.AddIndexedPathPlaceholders("translationName", 1, indexedPlaceholders);

                Action action = () => messageCache.GetIndexedPathPlaceholders("invalidTranslationName", 1);

                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullTranslationName()
            {
                var messageCache = new MessagesCache();

                var indexedPlaceholders = new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "arg",
                            Placeholder = "{arg|test=true}",
                            Parameters = new Dictionary<string, string>
                            {
                                ["test"] = "true"
                            }
                        }
                    }
                };

                messageCache.AddIndexedPathPlaceholders("translationName", 1, indexedPlaceholders);

                Action action = () => messageCache.GetIndexedPathPlaceholders(null, 1);

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        public class VerifyIntegrity
        {
            [Fact]
            public void Should_BeValid_When_EmptyCache()
            {
                var messagesCache = new MessagesCache();

                messagesCache.VerifyIntegrity();
            }

            [Fact]
            public void Should_BeValid_When_SingleError()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message1",
                    "message2"
                });

                messagesCache.VerifyIntegrity();
            }

            [Fact]
            public void Should_BeValid_When_MultipleErrors()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation1", 2, new[]
                {
                    "message21",
                    "message22"
                });

                messagesCache.AddMessage("translation1", 3, new[]
                {
                    "message31",
                    "message32"
                });

                messagesCache.VerifyIntegrity();
            }

            [Fact]
            public void Should_ThrowException_When_MultipleErrors_DifferentAmountOfMessages()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation1", 2, new[]
                {
                    "message21",
                    "message22"
                });

                messagesCache.AddMessage("translation2", 1, new[]
                {
                    "message31",
                    "message32"
                });

                messagesCache.AddMessage("translation2", 2, new[]
                {
                    "message41",
                    "message42",
                    "message43"
                });

                Action action = () => messagesCache.VerifyIntegrity();

                action.Should().ThrowExactly<CacheIntegrityException>().WithMessage($"ErrorId 2, messages amount is expected to be 2 but found 3 in translation `translation2`");
            }

            [Fact]
            public void Should_ThrowException_When_ErrorIdNotInAllTranslations()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation2", 1, new[]
                {
                    "message21",
                    "message22"
                });

                messagesCache.AddMessage("translation2", 2, new[]
                {
                    "message31",
                    "message32"
                });

                Action action = () => messagesCache.VerifyIntegrity();

                action.Should().ThrowExactly<CacheIntegrityException>().WithMessage($"ErrorId 2 is not present in all translations");
            }

            [Fact]
            public void Should_BeValid_When_PathPlaceholders()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation2", 1, new[]
                {
                    "message21",
                    "message22"
                });

                messagesCache.AddIndexedPathPlaceholders("translation1", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [0] = Array.Empty<ArgPlaceholder>(),
                });

                messagesCache.AddIndexedPathPlaceholders("translation2", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = Array.Empty<ArgPlaceholder>(),
                });

                messagesCache.VerifyIntegrity();
            }

            [Fact]
            public void Should_ThrowException_When_PathPlaceholders_WithInvalidTranslation()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation2", 1, new[]
                {
                    "message21",
                    "message22"
                });

                messagesCache.AddIndexedPathPlaceholders("translation1", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [0] = Array.Empty<ArgPlaceholder>(),
                });

                messagesCache.AddIndexedPathPlaceholders("translation3", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = Array.Empty<ArgPlaceholder>(),
                });

                Action action = () => messagesCache.VerifyIntegrity();

                action.Should().ThrowExactly<CacheIntegrityException>().WithMessage($"Translation `translation3` is not expected in path placeholders");
            }

            [Fact]
            public void Should_ThrowException_When_PathPlaceholders_WithInvalidErrorId()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation2", 1, new[]
                {
                    "message21",
                    "message22"
                });

                messagesCache.AddIndexedPathPlaceholders("translation1", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [0] = Array.Empty<ArgPlaceholder>(),
                });

                messagesCache.AddIndexedPathPlaceholders("translation2", 2, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = Array.Empty<ArgPlaceholder>(),
                });

                Action action = () => messagesCache.VerifyIntegrity();

                action.Should().ThrowExactly<CacheIntegrityException>().WithMessage($"ErrorId 2 is not expected in path placeholders (translation `translation2`)");
            }

            [Fact]
            public void Should_ThrowException_When_PathPlaceholders_IndexExceedsMessagesAmount()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation1", 2, new[]
                {
                    "message21",
                    "message22"
                });

                messagesCache.AddIndexedPathPlaceholders("translation1", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [0] = Array.Empty<ArgPlaceholder>(),
                    [1] = Array.Empty<ArgPlaceholder>(),
                });

                messagesCache.AddIndexedPathPlaceholders("translation1", 2, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [0] = Array.Empty<ArgPlaceholder>(),
                    [3] = Array.Empty<ArgPlaceholder>(),
                });

                Action action = () => messagesCache.VerifyIntegrity();

                action.Should().ThrowExactly<CacheIntegrityException>().WithMessage($"ErrorId 2 max index for path placeholder is 1, but found 3 (translation `translation1`)");
            }

            [Fact]
            public void Should_ThrowException_When_MessageWithPathArgs_WithInvalidTranslation()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation1", 2, new[]
                {
                    "message21",
                    "message22"
                });

                messagesCache.AddMessageWithPathArgs("translation1", "path", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessageWithPathArgs("translation2", "path", 1, new[]
                {
                    "message11",
                    "message22"
                });

                Action action = () => messagesCache.VerifyIntegrity();

                action.Should().ThrowExactly<CacheIntegrityException>().WithMessage($"Translation `translation2` is not expected in messages with path args");
            }

            [Fact]
            public void Should_ThrowException_When_MessageWithPathArgs_WithInvalidErrorId()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation1", 2, new[]
                {
                    "message21",
                    "message22"
                });

                messagesCache.AddMessageWithPathArgs("translation1", "path", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessageWithPathArgs("translation1", "path", 3, new[]
                {
                    "message11",
                    "message22"
                });

                Action action = () => messagesCache.VerifyIntegrity();

                action.Should().ThrowExactly<CacheIntegrityException>().WithMessage($"Error ID 3 in translation `translation1` is not expected in messages with path args");
            }

            [Fact]
            public void Should_ThrowException_When_MessageWithPathArgs_MaxMessagesAmountExceeded()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation1", 2, new[]
                {
                    "message21",
                    "message22"
                });

                messagesCache.AddMessageWithPathArgs("translation1", "path1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessageWithPathArgs("translation1", "path2", 2, new[]
                {
                    "message21",
                    "message22",
                    "message23"
                });

                Action action = () => messagesCache.VerifyIntegrity();

                action.Should().ThrowExactly<CacheIntegrityException>().WithMessage($"Error ID 2 is expected to have max 2 messages, but found 3 in messages with path args (for translation `translation1` and path `path2`)");
            }

            [Fact]
            public void Should_BeValid_When_MessageWithPathArgs()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation1", 2, new[]
                {
                    "message21",
                    "message22",
                    "message22"
                });

                messagesCache.AddMessageWithPathArgs("translation1", "path1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessageWithPathArgs("translation1", "path2", 2, new[]
                {
                    "message21",
                    "message22",
                    "message23"
                });

                messagesCache.VerifyIntegrity();
            }

            [Fact]
            public void Should_BeValid_When_Placeholders_And_MessageWithPathArgs()
            {
                var messagesCache = new MessagesCache();

                messagesCache.AddMessage("translation1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation1", 2, new[]
                {
                    "message21",
                    "message22",
                    "message22"
                });

                messagesCache.AddMessage("translation2", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessage("translation2", 2, new[]
                {
                    "message21",
                    "message22",
                    "message22"
                });

                messagesCache.AddIndexedPathPlaceholders("translation1", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [0] = Array.Empty<ArgPlaceholder>(),
                    [1] = Array.Empty<ArgPlaceholder>(),
                });

                messagesCache.AddIndexedPathPlaceholders("translation1", 2, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [0] = Array.Empty<ArgPlaceholder>(),
                    [1] = Array.Empty<ArgPlaceholder>(),
                    [2] = Array.Empty<ArgPlaceholder>(),
                });

                messagesCache.AddIndexedPathPlaceholders("translation2", 1, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [1] = Array.Empty<ArgPlaceholder>(),
                });

                messagesCache.AddIndexedPathPlaceholders("translation2", 2, new Dictionary<int, IReadOnlyList<ArgPlaceholder>>()
                {
                    [0] = Array.Empty<ArgPlaceholder>(),
                    [2] = Array.Empty<ArgPlaceholder>(),
                });

                messagesCache.AddMessageWithPathArgs("translation1", "path1", 1, new[]
                {
                    "message11",
                    "message12"
                });

                messagesCache.AddMessageWithPathArgs("translation1", "path2", 2, new[]
                {
                    "message21",
                    "message22",
                    "message23"
                });

                messagesCache.AddMessageWithPathArgs("translation2", "path1", 1, new[]
                {
                    "message12"
                });

                messagesCache.AddMessageWithPathArgs("translation2", "path2", 2, new[]
                {
                    "message21",
                    "message23"
                });

                messagesCache.VerifyIntegrity();
            }
        }

        [Fact]
        public void Should_Initialize()
        {
            _ = new MessagesCache();
        }
    }
}
