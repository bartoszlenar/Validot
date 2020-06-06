namespace Validot.Tests.Unit.Validation.Scopes.Builders
{
    using System;
    using System.Collections.Generic;

    using Validot.Errors;
    using Validot.Errors.Args;
    using Validot.Specification.Commands;

    public static class ErrorBuilderTestData
    {
        private class TestModel
        {
            public string TestMember { get; set; }
        }

        public static IEnumerable<object[]> CommandsEnabled()
        {
            var enabled = new ICommand[]
            {
                new WithMessageCommand("a"),
                new WithExtraMessageCommand("a"),
                new WithCodeCommand("a"),
                new WithExtraCodeCommand("a")
            };

            var disabled = new ICommand[]
            {
                new AsCollectionCommand<object[], object>(a => a),
                new AsModelCommand<object>(a => a),
                new AsNullableCommand<int>(a => a),
                new ForbiddenCommand(),
                new MemberCommand<TestModel, string>(a => a.TestMember, a => a),
                new OptionalCommand(),
                new RequiredCommand(),
                new RuleCommand<object>(a => false),
            };

            foreach (var cmd in enabled)
            {
                yield return new object[]
                {
                    cmd,
                    true
                };
            }

            foreach (var cmd in disabled)
            {
                yield return new object[]
                {
                    cmd,
                    false
                };
            }
        }

        public static IEnumerable<object[]> Keep_InvalidCommands_NotAffecting_ValidCommands()
        {
            yield return new object[]
            {
                new ICommand[]
                {
                    new ForbiddenCommand(),
                    new WithCodeCommand("abc"),
                    new OptionalCommand(),
                },
                new Error
                {
                    Args = Array.Empty<IArg>(),
                    Messages = Array.Empty<string>(),
                    Codes = new[]
                    {
                        "abc",
                    }
                }
            };

            yield return new object[]
            {
                new ICommand[]
                {
                    new RuleCommand<object>(a => true),
                    new WithMessageCommand("abc"),
                    new RuleCommand<object>(a => false),
                },
                new Error
                {
                    Args = Array.Empty<IArg>(),
                    Codes = Array.Empty<string>(),
                    Messages = new[]
                    {
                        "abc",
                    }
                }
            };

            yield return new object[]
            {
                new ICommand[]
                {
                    new OptionalCommand(),
                    new WithExtraMessageCommand("hij"),
                    new OptionalCommand(),
                    new WithExtraMessageCommand("klm"),
                    new RuleCommand<object>(a => true),
                    new WithExtraCodeCommand("123"),
                    new WithExtraCodeCommand("456"),
                    new RuleCommand<object>(a => true),
                    new OptionalCommand(),
                    new WithExtraCodeCommand("789"),
                    new RuleCommand<object>(a => false),
                    new WithExtraCodeCommand("101112"),
                },
                new Error
                {
                    Args = Array.Empty<IArg>(),
                    Codes = new[]
                    {
                        "123",
                        "456",
                        "789",
                        "101112"
                    },
                    Messages = new[]
                    {
                        "hij",
                        "klm"
                    }
                }
            };

            yield return new object[]
            {
                new ICommand[]
                {
                    new WithCodeCommand("123"),
                    new RuleCommand<object>(a => false),
                    new WithExtraMessageCommand("hij"),
                    new MemberCommand<TestModel, string>(m => m.TestMember, m => m),
                    new WithExtraCodeCommand("789"),
                    new WithExtraMessageCommand("klm"),
                    new MemberCommand<TestModel, string>(m => m.TestMember, m => m),
                    new WithExtraCodeCommand("101112"),
                    new MemberCommand<TestModel, string>(m => m.TestMember, m => m),
                    new ForbiddenCommand(),
                    new ForbiddenCommand(),
                },
                new Error
                {
                    Args = Array.Empty<IArg>(),
                    Codes = new[]
                    {
                        "123",
                        "789",
                        "101112"
                    },
                    Messages = new[]
                    {
                        "hij",
                        "klm"
                    }
                }
            };
        }

        public static class Messages
        {
            public static IEnumerable<object[]> SingleMessage_When_SingleWithMessageCommand()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("abc"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "abc"
                        }
                    }
                };
            }

            public static IEnumerable<object[]> SingleMessage_When_SingleExtraMessageCommand()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithExtraMessageCommand("abc"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "abc"
                        }
                    }
                };
            }

            public static IEnumerable<object[]> SingleMessage_FromLatestCommand_When_WithMessageCommandIsTheLastOne()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("abc"),
                        new WithMessageCommand("cde"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "cde"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("abc"),
                        new WithMessageCommand("cde"),
                        new WithMessageCommand("efg"),
                        new WithMessageCommand("hij"),
                        new WithMessageCommand("klm"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "klm"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("abc"),
                        new WithExtraMessageCommand("cde"),
                        new WithMessageCommand("efg"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "efg"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("abc"),
                        new WithExtraMessageCommand("cde"),
                        new WithMessageCommand("efg"),
                        new WithExtraMessageCommand("hij"),
                        new WithMessageCommand("klm"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "klm"
                        }
                    }
                };
            }

            public static IEnumerable<object[]> ManyMessages_When_WithMessageCommand_IsFollowedBy_WithExtraMessageCommands()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("abc"),
                        new WithExtraMessageCommand("cde"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "abc",
                            "cde"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("abc"),
                        new WithExtraMessageCommand("cde"),
                        new WithExtraMessageCommand("efg"),
                        new WithExtraMessageCommand("hij"),
                        new WithExtraMessageCommand("klm"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "abc",
                            "cde",
                            "efg",
                            "hij",
                            "klm"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("abc"),
                        new WithExtraMessageCommand("cde"),
                        new WithMessageCommand("efg"),
                        new WithExtraMessageCommand("hij"),
                        new WithExtraMessageCommand("klm"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "efg",
                            "hij",
                            "klm"
                        }
                    }
                };
            }
        }

        public static class Codes
        {
            public static IEnumerable<object[]> SingleCode_When_SingleWithCodeCommand()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithCodeCommand("abc"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Messages = Array.Empty<string>(),
                        Codes = new[]
                        {
                            "abc"
                        }
                    }
                };
            }

            public static IEnumerable<object[]> SingleCode_When_SingleExtraCodeCommand()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithExtraCodeCommand("abc"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Messages = Array.Empty<string>(),
                        Codes = new[]
                        {
                            "abc"
                        }
                    }
                };
            }

            public static IEnumerable<object[]> SingleCode_FromLatestCommand_When_WithCodeCommandIsTheLastOne()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithCodeCommand("abc"),
                        new WithCodeCommand("cde"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Messages = Array.Empty<string>(),
                        Codes = new[]
                        {
                            "cde"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithCodeCommand("abc"),
                        new WithCodeCommand("cde"),
                        new WithCodeCommand("efg"),
                        new WithCodeCommand("hij"),
                        new WithCodeCommand("klm"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Messages = Array.Empty<string>(),
                        Codes = new[]
                        {
                            "klm"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithCodeCommand("abc"),
                        new WithExtraCodeCommand("cde"),
                        new WithCodeCommand("efg"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Messages = Array.Empty<string>(),
                        Codes = new[]
                        {
                            "efg"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithCodeCommand("abc"),
                        new WithExtraCodeCommand("cde"),
                        new WithCodeCommand("efg"),
                        new WithExtraCodeCommand("hij"),
                        new WithCodeCommand("klm"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Messages = Array.Empty<string>(),
                        Codes = new[]
                        {
                            "klm"
                        }
                    }
                };
            }

            public static IEnumerable<object[]> ManyCodes_When_WithCodeCommand_IsFollowedBy_WithExtraCodeCommands()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithCodeCommand("abc"),
                        new WithExtraCodeCommand("cde"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Messages = Array.Empty<string>(),
                        Codes = new[]
                        {
                            "abc",
                            "cde"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithCodeCommand("abc"),
                        new WithExtraCodeCommand("cde"),
                        new WithExtraCodeCommand("efg"),
                        new WithExtraCodeCommand("hij"),
                        new WithExtraCodeCommand("klm"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Messages = Array.Empty<string>(),
                        Codes = new[]
                        {
                            "abc",
                            "cde",
                            "efg",
                            "hij",
                            "klm"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithCodeCommand("abc"),
                        new WithExtraCodeCommand("cde"),
                        new WithCodeCommand("efg"),
                        new WithExtraCodeCommand("hij"),
                        new WithExtraCodeCommand("klm"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Messages = Array.Empty<string>(),
                        Codes = new[]
                        {
                            "efg",
                            "hij",
                            "klm"
                        }
                    }
                };
            }
        }

        public static class MessagesAndCodes
        {
            public static IEnumerable<object[]> SingleMessage_With_SingleCode()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("abc"),
                        new WithExtraCodeCommand("123"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = new[]
                        {
                            "123"
                        },
                        Messages = new[]
                        {
                            "abc"
                        }
                    }
                };
            }

            public static IEnumerable<object[]> ManyMessages_With_ManyCodes()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("m1"),
                        new WithExtraMessageCommand("m2"),
                        new WithExtraMessageCommand("m3"),
                        new WithExtraCodeCommand("c1"),
                        new WithExtraCodeCommand("c2"),
                        new WithExtraCodeCommand("c3"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = new[]
                        {
                            "c1",
                            "c2",
                            "c3",
                        },
                        Messages = new[]
                        {
                            "m1",
                            "m2",
                            "m3",
                        }
                    }
                };
            }
        }

        public static class Modes
        {
            public static IEnumerable<object[]> Should_BeIn_AppendMode_If_OnlyExtraCommands()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithExtraMessageCommand("m4"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "m4"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithExtraCodeCommand("c4"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = new[]
                        {
                            "c4"
                        },
                        Messages = Array.Empty<string>()
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithExtraMessageCommand("m4"),
                        new WithExtraCodeCommand("c4"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = new[]
                        {
                            "c4"
                        },
                        Messages = new[]
                        {
                            "m4"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithExtraMessageCommand("m1"),
                        new WithExtraMessageCommand("m2"),
                        new WithExtraMessageCommand("m3"),
                        new WithExtraMessageCommand("m4"),
                        new WithExtraCodeCommand("c1"),
                        new WithExtraCodeCommand("c2"),
                        new WithExtraCodeCommand("c3"),
                        new WithExtraCodeCommand("c4"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = new[]
                        {
                            "c1",
                            "c2",
                            "c3",
                            "c4",
                        },
                        Messages = new[]
                        {
                            "m1",
                            "m2",
                            "m3",
                            "m4",
                        }
                    }
                };
            }

            public static IEnumerable<object[]> Should_BeIn_OverrideMode_If_AnyNonExtraCommand_Or_WithErrorClearedCommand()
            {
                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("m4"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "m4"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("m3"),
                        new WithExtraMessageCommand("m4"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = Array.Empty<string>(),
                        Messages = new[]
                        {
                            "m3",
                            "m4"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithCodeCommand("c4"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = new[]
                        {
                            "c4"
                        },
                        Messages = Array.Empty<string>()
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithCodeCommand("c3"),
                        new WithExtraCodeCommand("c4"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = new[]
                        {
                            "c3",
                            "c4"
                        },
                        Messages = Array.Empty<string>()
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("m3"),
                        new WithExtraMessageCommand("m4"),
                        new WithExtraCodeCommand("c4"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = new[]
                        {
                            "c4"
                        },
                        Messages = new[]
                        {
                            "m3",
                            "m4"
                        }
                    }
                };

                yield return new object[]
                {
                    new ICommand[]
                    {
                        new WithMessageCommand("m1"),
                        new WithExtraMessageCommand("m2"),
                        new WithExtraMessageCommand("m3"),
                        new WithExtraMessageCommand("m4"),
                        new WithExtraCodeCommand("c1"),
                        new WithExtraCodeCommand("c2"),
                        new WithExtraCodeCommand("c3"),
                        new WithExtraCodeCommand("c4"),
                    },
                    new Error
                    {
                        Args = Array.Empty<IArg>(),
                        Codes = new[]
                        {
                            "c1",
                            "c2",
                            "c3",
                            "c4",
                        },
                        Messages = new[]
                        {
                            "m1",
                            "m2",
                            "m3",
                            "m4",
                        }
                    }
                };
            }
        }
    }
}
