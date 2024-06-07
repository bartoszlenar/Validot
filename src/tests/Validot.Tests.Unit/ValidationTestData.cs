namespace Validot.Tests.Unit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Validot.Translations;

    public static class ValidationTestData
    {
        public class TestCase
        {
            public string Name { get; set; }

            public Specification<TestClass> Specification { get; set; }

            public IReadOnlyDictionary<string, IReadOnlyList<ErrorTestCase>> ExpectedTemplate { get; set; }

            public IReadOnlyList<ValidationTestCase> ValidationCases { get; set; }
        }

        public class ValidationTestCase
        {
            public TestClass Model { get; set; }

            public IReadOnlyDictionary<string, IReadOnlyList<ErrorTestCase>> Errors { get; set; }

            public string FailFastErrorKey { get; set; }

            public ReferenceLoopExceptionCase ReferenceLoopExceptionCase { get; set; }
        }

        public class ErrorTestCase
        {
            public IReadOnlyList<string> Messages { get; set; }

            public IReadOnlyList<string> Codes { get; set; }

            public IReadOnlyList<ArgTestCase> Args { get; set; }
        }

        public class ReferenceLoopExceptionCase
        {
            public Type Type { get; set; }

            public string Path { get; set; }

            public string NestedPath { get; set; }
        }

        public class ArgTestCase
        {
            public string Name { get; set; }

            public dynamic Value { get; set; }
        }

        public class TestCollection<T> : IEnumerable<T>
        {
            private readonly IEnumerable<T> _innerCollection;

            public TestCollection(IEnumerable<T> innerCollection)
            {
                _innerCollection = innerCollection;
            }

            public IEnumerator<T> GetEnumerator() => _innerCollection.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class TestMember
        {
            public string MemberText { get; set; }

            public DateTimeOffset MemberDate { get; set; }

            public TestClass NestedSelf { get; set; }
        }

        public struct TestStruct
        {
            public int StructNumber { get; set; }
        }

        public class TestParent
        {
            public int ParentNumber { get; set; }
        }

        public class TestChild : TestParent
        {
            public int ChildNumber { get; set; }
        }

        public class TestClass
        {
            public string HybridField;

            public int ValueField;

            public string Hybrid { get; set; }

            public int Value { get; set; }

            public object Reference { get; set; }

            public bool? Nullable { get; set; }

            public TestClass Self { get; set; }

            public TestCollection<TestClass> SelfCollection { get; set; }

            public TestCollection<int> Collection { get; set; }

            public TestMember Member { get; set; }

            public TestStruct StructMember { get; set; }

            public TestCollection<TestMember> MembersCollection { get; set; }

            public TestChild Child { get; set; }

            public Dictionary<TestMember, TestClass> ComplexDictionary { get; set; }

            public Dictionary<string, string> SimpleDictionary { get; set; }
        }

        private static readonly Dictionary<string, IReadOnlyList<ErrorTestCase>> NoErrors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>();

        public static TestCase RenamedClone(TestCase input, string prefix) => new TestCase()
        {
            Name = $"{prefix}_{input.Name}",
            Specification = input.Specification,
            ValidationCases = input.ValidationCases,
            ExpectedTemplate = input.ExpectedTemplate
        };

        public static IReadOnlyList<TestCase> GetCases()
        {
            var cases = new List<TestCase>();

            cases.AddRange(GlobalPresenceCases.Select(c => RenamedClone(c, nameof(GlobalPresenceCases))));
            cases.AddRange(MemberPresenceCases.Select(c => RenamedClone(c, nameof(MemberPresenceCases))));
            cases.AddRange(SingleRuleCases.Select(c => RenamedClone(c, nameof(SingleRuleCases))));
            cases.AddRange(SingleRuleTemplateCases.Select(c => RenamedClone(c, nameof(SingleRuleTemplateCases))));
            cases.AddRange(RulesCases.Select(c => RenamedClone(c, nameof(RulesCases))));
            cases.AddRange(MemberCases.Select(c => RenamedClone(c, nameof(MemberCases))));
            cases.AddRange(CommandCases.Select(c => RenamedClone(c, nameof(CommandCases))));
            cases.AddRange(PathCases.Select(c => RenamedClone(c, nameof(PathCases))));
            cases.AddRange(OverwritingCases.Select(c => RenamedClone(c, nameof(OverwritingCases))));
            cases.AddRange(FailFastCases.Select(c => RenamedClone(c, nameof(FailFastCases))));
            cases.AddRange(MixedCases.Select(c => RenamedClone(c, nameof(MixedCases))));
            cases.AddRange(ReferencesLoopCases().Select(c => RenamedClone(c, nameof(ReferencesLoopCases))));

            return cases;
        }

        public static IReadOnlyList<TestCase> GlobalPresenceCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "optional",
                Specification = s => s.Optional(),
                ExpectedTemplate = NoErrors,
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = NoErrors,
                        FailFastErrorKey = null,
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "no_commands",
                Specification = s => s,
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required",
                Specification = s => s.Required(),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_custom_message",
                Specification = s => s.Required().WithMessage("Custom Message"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "Custom Message" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Custom Message" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_extra_custom_message",
                Specification = s => s.Required().WithExtraMessage("Extra Custom Message"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required, "Extra Custom Message" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required, "Extra Custom Message" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_custom_code",
                Specification = s => s.Required().WithCode("Code1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Codes = new[] { "Code1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Codes = new[] { "Code1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_extra_custom_code",
                Specification = s => s.Required().WithExtraCode("Extra_Custom_Code"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required, },
                            Codes = new[] { "Extra_Custom_Code" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required, },
                                    Codes = new[] { "Extra_Custom_Code" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_mix",
                Specification = s => s.Required().WithMessage("Message 1").WithExtraMessage("Message 2").WithExtraCode("Code1").WithExtraCode("Code2"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "Message 1", "Message 2" },
                            Codes = new[] { "Code1", "Code2" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Message 1", "Message 2" },
                                    Codes = new[] { "Code1", "Code2" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_mix_with_more_commands_after",
                Specification = s => s
                    .Required().WithMessage("Message 1").WithExtraMessage("Message 2").WithExtraCode("Code1").WithExtraCode("Code2")
                    .Rule(m => true).WithMessage("rule message 1")
                    .Rule(m => true).WithCode("rule_code_1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "Message 1", "Message 2" },
                            Codes = new[] { "Code1", "Code2" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "rule message 1" },
                        },
                        new ErrorTestCase()
                        {
                            Codes = new[] { "rule_code_1" },
                        },
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Message 1", "Message 2" },
                                    Codes = new[] { "Code1", "Code2" }
                                },
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden",
                Specification = s => s.Forbidden(),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Forbidden }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Forbidden }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden_custom_message",
                Specification = s => s.Forbidden().WithMessage("Custom Message"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "Custom Message" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Custom Message" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden_extra_custom_message",
                Specification = s => s.Forbidden().WithExtraMessage("Extra Custom Message"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Forbidden, "Extra Custom Message" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Forbidden, "Extra Custom Message" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden_custom_code",
                Specification = s => s.Forbidden().WithCode("Code1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Codes = new[] { "Code1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Codes = new[] { "Code1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden_extra_custom_code",
                Specification = s => s.Forbidden().WithExtraCode("Code1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Forbidden },
                            Codes = new[] { "Code1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Forbidden },
                                    Codes = new[] { "Code1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden_mix",
                Specification = s => s.Forbidden().WithMessage("Message 1").WithExtraMessage("Message 2").WithExtraCode("Code1").WithExtraCode("Code2"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "Message 1", "Message 2" },
                            Codes = new[] { "Code1", "Code2" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Message 1", "Message 2" },
                                    Codes = new[] { "Code1", "Code2" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
        };

        public static IReadOnlyList<TestCase> MemberPresenceCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "optional",
                Specification = s => s
                    .Member(m => m.Member, m => m.Optional()),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null,
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "no_commands",
                Specification = s => s
                    .Member(m => m.Member, m => m),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required",
                Specification = s => s
                    .Member(m => m.Member, m => m.Required()),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_custom_message",
                Specification = s => s
                    .Member(m => m.Member, m => m.Required().WithMessage("Custom Message")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "Custom Message" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Custom Message" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_extra_custom_message",
                Specification = s => s
                    .Member(m => m.Member, m => m.Required().WithExtraMessage("Extra Custom Message")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required, "Extra Custom Message" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required, "Extra Custom Message" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_custom_code",
                Specification = s => s
                    .Member(m => m.Member, m => m.Required().WithCode("Code1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Codes = new[] { "Code1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Codes = new[] { "Code1" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_extra_custom_code",
                Specification = s => s
                    .Member(m => m.Member, m => m.Required().WithExtraCode("Extra_Custom_Code")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required, },
                            Codes = new[] { "Extra_Custom_Code" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required, },
                                    Codes = new[] { "Extra_Custom_Code" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_mix",
                Specification = s => s
                    .Member(m => m.Member, m => m.Required().WithMessage("Message 1").WithExtraMessage("Message 2").WithExtraCode("Code1").WithExtraCode("Code2")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "Message 1", "Message 2" },
                            Codes = new[] { "Code1", "Code2" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Message 1", "Message 2" },
                                    Codes = new[] { "Code1", "Code2" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "required_mix_with_more_commands_after",
                Specification = s => s
                    .Member(m => m.Member, m => m
                        .Required().WithMessage("Message 1").WithExtraMessage("Message 2").WithExtraCode("Code1").WithExtraCode("Code2")
                        .Rule(m1 => true).WithMessage("rule message 1")
                        .Rule(m1 => true).WithCode("rule_code_1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "Message 1", "Message 2" },
                            Codes = new[] { "Code1", "Code2" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "rule message 1" },
                        },
                        new ErrorTestCase()
                        {
                            Codes = new[] { "rule_code_1" },
                        },
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Message 1", "Message 2" },
                                    Codes = new[] { "Code1", "Code2" }
                                },
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden",
                Specification = s => s
                    .Member(m => m.Member, m => m.Forbidden()),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Forbidden }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Forbidden }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden_custom_message",
                Specification = s => s
                    .Member(m => m.Member, m => m.Forbidden().WithMessage("Custom Message")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "Custom Message" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Custom Message" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden_extra_custom_message",
                Specification = s => s
                    .Member(m => m.Member, m => m.Forbidden().WithExtraMessage("Extra Custom Message")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Forbidden, "Extra Custom Message" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Forbidden, "Extra Custom Message" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden_custom_code",
                Specification = s => s
                    .Member(m => m.Member, m => m.Forbidden().WithCode("Code1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Codes = new[] { "Code1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Codes = new[] { "Code1" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden_extra_custom_code",
                Specification = s => s
                    .Member(m => m.Member, m => m.Forbidden().WithExtraCode("Code1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Forbidden },
                            Codes = new[] { "Code1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Forbidden },
                                    Codes = new[] { "Code1" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                },
            },
            new TestCase()
            {
                Name = "forbidden_mix",
                Specification = s => s
                    .Member(m => m.Member, m => m.Forbidden().WithMessage("Message 1").WithExtraMessage("Message 2").WithExtraCode("Code1").WithExtraCode("Code2")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "Message 1", "Message 2" },
                            Codes = new[] { "Code1", "Code2" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Message 1", "Message 2" },
                                    Codes = new[] { "Code1", "Code2" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                },
            },
        };

        public static IReadOnlyList<TestCase> MemberCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "reference",
                Specification = s => s
                    .Member(m => m.Member, m => m.Rule(m1 => m1.MemberText != null).WithMessage("message 1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                MessageKey.Global.Required,
                            }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                "message 1"
                            }
                        }
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        MessageKey.Global.Required,
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = "Member",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = null } },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        "message 1"
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xyz" } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "value",
                Specification = s => s
                    .Member(m => m.StructMember, m => m.Rule(m1 => m1.StructNumber != 0).WithMessage("message 1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["StructMember"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                "message 1"
                            }
                        }
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { StructMember = new TestStruct() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["StructMember"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        "message 1"
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = "StructMember",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { StructMember = new TestStruct() { StructNumber = 321 } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "collection",
                Specification = s => s
                    .Member(m => m.Collection, m => m.Rule(m1 => m1.ElementAt(2) != 0).WithMessage("message 1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Collection"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                MessageKey.Global.Required,
                            }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                "message 1"
                            }
                        }
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Collection = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Collection"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        MessageKey.Global.Required
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = "Collection",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Collection = new TestCollection<int>(new[] { 0, 0, 0, 0 }) },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Collection"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        "message 1"
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = "Collection",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Collection = new TestCollection<int>(new[] { 1, 2, 3, 4 }) },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "nullable",
                Specification = s => s
                    .Member(m => m.Nullable, m => m.Rule(m1 => m1.HasValue && m1.Value).WithMessage("message 1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Nullable"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                MessageKey.Global.Required,
                            }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                "message 1"
                            }
                        }
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Nullable = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Nullable"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        MessageKey.Global.Required
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = "Nullable",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Nullable = false },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Nullable"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        "message 1"
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = "Nullable",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Nullable = true },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "field",
                Specification = s => s
                    .Member(m => m.HybridField, m => m.Rule(m1 => !string.IsNullOrEmpty(m1)).WithMessage("message 1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["HybridField"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                MessageKey.Global.Required,
                            }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                "message 1"
                            }
                        }
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { HybridField = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["HybridField"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        MessageKey.Global.Required
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = "HybridField",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { HybridField = "" },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["HybridField"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        "message 1"
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = "HybridField",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { HybridField = "xyz" },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
        };

        public static IReadOnlyList<TestCase> SingleRuleCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "rule",
                Specification = s => s.Rule(m => m.Value == 0),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Error }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Error }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_message",
                Specification = s => s.Rule(m => m.Value == 0).WithMessage("message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                }
                            },
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_extra_message",
                Specification = s => s.Rule(m => m.Value == 0).WithExtraMessage("message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_code",
                Specification = s => s.Rule(m => m.Value == 0).WithCode("code_1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Codes = new[] { "code_1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Codes = new[] { "code_1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_extra_code",
                Specification = s => s.Rule(m => m.Value == 0).WithExtraCode("code_1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Codes = new[] { "code_1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Codes = new[] { "code_1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_messages_and_codes",
                Specification = s => s.Rule(m => m.Value == 0).WithMessage("message 1").WithExtraMessage("message 2").WithExtraCode("code_1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1", "message 2" },
                            Codes = new[] { "code_1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1", "message 2" },
                                    Codes = new[] { "code_1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_different_path",
                Specification = s => s.Rule(m => m.Value == 0).WithPath("extra").WithMessage("message 1").WithExtraMessage("message 2").WithExtraCode("code_1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                    },
                    ["extra"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1", "message 2" },
                            Codes = new[] { "code_1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["extra"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1", "message 2" },
                                    Codes = new[] { "code_1" }
                                }
                            }
                        },
                        FailFastErrorKey = "extra"
                    },
                },
            },
        };

        public static IReadOnlyList<TestCase> SingleRuleTemplateCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "rule",
                Specification = s => s.RuleTemplate(m => m.Value == 0, "message key"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message key" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message key" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_args",
                Specification = s => s.RuleTemplate(m => m.Value == 0, "message key", Arg.Number("arg1", 123), Arg.Text("arg2", "xyz")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message key" },
                            Args = new[]
                            {
                                new ArgTestCase() { Name = "arg1", Value = 123 },
                                new ArgTestCase() { Name = "arg2", Value = "xyz" },
                            }
                        },
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message key" },
                                    Args = new[]
                                    {
                                        new ArgTestCase() { Name = "arg1", Value = 123 },
                                        new ArgTestCase() { Name = "arg2", Value = "xyz" },
                                    }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_message",
                Specification = s => s.RuleTemplate(m => m.Value == 0, "message key").WithMessage("message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                }
                            },
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_args_and_message",
                Specification = s => s.RuleTemplate(m => m.Value == 0, "message key", Arg.Number("arg1", 123), Arg.Text("arg2", "xyz")).WithMessage("message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" },
                            Args = new[]
                            {
                                new ArgTestCase() { Name = "arg1", Value = 123 },
                                new ArgTestCase() { Name = "arg2", Value = "xyz" },
                            }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" },
                                    Args = new[]
                                    {
                                        new ArgTestCase() { Name = "arg1", Value = 123 },
                                        new ArgTestCase() { Name = "arg2", Value = "xyz" },
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_extra_message",
                Specification = s => s.RuleTemplate(m => m.Value == 0, "message key").WithExtraMessage("message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message key", "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message key", "message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_code",
                Specification = s => s.RuleTemplate(m => m.Value == 0, "message key").WithCode("code_1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Codes = new[] { "code_1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Codes = new[] { "code_1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_extra_code",
                Specification = s => s.RuleTemplate(m => m.Value == 0, "message key").WithExtraCode("code_1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message key" },
                            Codes = new[] { "code_1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message key" },
                                    Codes = new[] { "code_1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_messages_and_codes",
                Specification = s => s.RuleTemplate(m => m.Value == 0, "message key").WithMessage("message 1").WithExtraMessage("message 2").WithExtraCode("code_1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1", "message 2" },
                            Codes = new[] { "code_1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1", "message 2" },
                                    Codes = new[] { "code_1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "rule_with_different_path",
                Specification = s => s.RuleTemplate(m => m.Value == 0, "message key").WithPath("extra").WithMessage("message 1").WithExtraMessage("message 2").WithExtraCode("code_1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                    },
                    ["extra"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1", "message 2" },
                            Codes = new[] { "code_1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["extra"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1", "message 2" },
                                    Codes = new[] { "code_1" }
                                }
                            }
                        },
                        FailFastErrorKey = "extra"
                    },
                },
            },
        };

        public static IReadOnlyList<TestCase> RulesCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "no_rules",
                Specification = s => s,
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "single_rule",
                Specification = s => s.Rule(m => m.Value == 0).WithMessage("message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "single_rule_with_condition",
                Specification = s => s.Rule(m => m.Value == 0).WithCondition(m => m.Nullable == true).WithMessage("message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Nullable = false },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Nullable = true },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Nullable = false },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Nullable = true },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "many_rules",
                Specification = s => s
                    .Rule(m => m.Value == 0).WithMessage("message 1")
                    .Rule(m => m.Nullable == false).WithMessage("message 21").WithExtraMessage("message 22")
                    .Rule(m => m.Reference == null).WithMessage("message 3").WithExtraCode("code_3"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 21", "message 22" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" },
                            Codes = new[] { "code_3" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Nullable = false, Reference = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Nullable = true, Reference = new object() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 21", "message 22" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" },
                                    Codes = new[] { "code_3" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Nullable = true, Reference = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 21", "message 22" }
                                },
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Nullable = false, Reference = new object() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" },
                                    Codes = new[] { "code_3" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
            new TestCase()
            {
                Name = "many_rules_with_custom_unique_paths",
                Specification = s => s
                    .Rule(m => m.Value == 0).WithPath("Name1").WithMessage("message 1")
                    .Rule(m => m.Nullable == false).WithPath("Name2").WithMessage("message 21").WithExtraMessage("message 22")
                    .Rule(m => m.Reference == null).WithPath("Name31.Name32").WithMessage("message 3").WithExtraCode("code_3"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                    },
                    ["Name1"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        },
                    },
                    ["Name2"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 21", "message 22" }
                        },
                    },
                    ["Name31.Name32"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" },
                            Codes = new[] { "code_3" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Nullable = false, Reference = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Nullable = true, Reference = new object() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Name1"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["Name2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 21", "message 22" }
                                },
                            },
                            ["Name31.Name32"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" },
                                    Codes = new[] { "code_3" }
                                }
                            }
                        },
                        FailFastErrorKey = "Name1"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Nullable = true, Reference = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Name2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 21", "message 22" }
                                },
                            },
                        },
                        FailFastErrorKey = "Name2"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Nullable = false, Reference = new object() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Name1"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["Name31.Name32"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" },
                                    Codes = new[] { "code_3" }
                                }
                            }
                        },
                        FailFastErrorKey = "Name1"
                    },
                },
            },
            new TestCase()
            {
                Name = "many_rules_with_custom_paths",
                Specification = s => s
                    .Rule(m => m.Value == 0).WithPath("Name1").WithMessage("message 1")
                    .Rule(m => m.Nullable == false).WithPath("Name2").WithMessage("message 21").WithExtraMessage("message 22")
                    .Rule(m => m.Reference == null).WithPath("Name1").WithMessage("message 3").WithExtraCode("code_3"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                    },
                    ["Name1"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" },
                            Codes = new[] { "code_3" }
                        }
                    },
                    ["Name2"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 21", "message 22" }
                        },
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Nullable = false, Reference = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Nullable = true, Reference = new object() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Name1"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" },
                                    Codes = new[] { "code_3" }
                                }
                            },
                            ["Name2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 21", "message 22" }
                                },
                            },
                        },
                        FailFastErrorKey = "Name1"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Nullable = true, Reference = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Name2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 21", "message 22" }
                                },
                            },
                        },
                        FailFastErrorKey = "Name2"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Nullable = false, Reference = new object() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Name1"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" },
                                    Codes = new[] { "code_3" }
                                }
                            }
                        },
                        FailFastErrorKey = "Name1"
                    },
                },
            },
            new TestCase()
            {
                Name = "many_rules_with_conditions",
                Specification = s => s
                    .Rule(m => m.Value == 0).WithCondition(t => false).WithMessage("message 1")
                    .Rule(m => m.Nullable == false).WithCondition(t => true).WithMessage("message 21").WithExtraMessage("message 22")
                    .Rule(m => m.Reference == null).WithCondition(t => true).WithMessage("message 3").WithExtraCode("code_3"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 21", "message 22" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" },
                            Codes = new[] { "code_3" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Nullable = false, Reference = null },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Nullable = true, Reference = new object() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 21", "message 22" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" },
                                    Codes = new[] { "code_3" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Nullable = true, Reference = null },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 21", "message 22" }
                                },
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Nullable = false, Reference = new object() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" },
                                    Codes = new[] { "code_3" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                },
            },
        };

        public static IReadOnlyList<TestCase> CommandCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "Member",
                Specification = s => s.Member(m => m.Member, m => m.Rule(m1 => m1.MemberText != null).WithMessage("message 1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xyz" } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "AsCollection",
                Specification = s => s.Member(m => m.MembersCollection, m => m
                    .AsCollection<TestCollection<TestMember>, TestMember>(c => c.Rule(c1 => c1.MemberText != null).WithMessage("message 1"))
                ),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["MembersCollection"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["MembersCollection.#"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["MembersCollection"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = "MembersCollection"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { MembersCollection = new TestCollection<TestMember>(Array.Empty<TestMember>()) },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            MembersCollection = new TestCollection<TestMember>(
                                new TestMember[]
                                {
                                    new TestMember() { MemberText = "abc" },
                                    null,
                                    new TestMember(),
                                    new TestMember() { MemberText = "abc" },
                                    new TestMember(),
                                    null
                                })
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["MembersCollection.#1"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            },
                            ["MembersCollection.#2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["MembersCollection.#4"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["MembersCollection.#5"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            },
                        },
                        FailFastErrorKey = "MembersCollection.#1"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            MembersCollection = new TestCollection<TestMember>(
                                new TestMember[]
                                {
                                    new TestMember() { MemberText = "abc" },
                                    new TestMember() { MemberText = "abc" },
                                    new TestMember(),
                                    null,
                                })
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["MembersCollection.#2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["MembersCollection.#3"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            },
                        },
                        FailFastErrorKey = "MembersCollection.#2"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            MembersCollection = new TestCollection<TestMember>(
                                new TestMember[]
                                {
                                    new TestMember() { MemberText = "abc" },
                                    new TestMember() { MemberText = "abc" },
                                })
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "AsNullable",
                Specification = s => s.Member(m => m.Nullable, m => m
                    .AsNullable(m1 => m1.Rule(m2 => m2).WithMessage("message 1"))
                ),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Nullable"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Nullable"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = "Nullable"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Nullable = false },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Nullable"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Nullable"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Nullable = true },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "AsConverted",
                Specification = s => s.Member(m => m.Value, m => m
                    .AsConverted(value => value.ToString(CultureInfo.InvariantCulture), v => v.Rule(str => str.Length <= 4).WithMessage("Number must be written using no more than 4 digits."))
                ),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                MessageKey.Global.Required
                            }
                        }
                    },
                    ["Value"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                MessageKey.Global.Required
                            }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                "Number must be written using no more than 4 digits."
                            }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Value = 123456
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Value"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        "Number must be written using no more than 4 digits."
                                    }
                                },
                            }
                        },
                        FailFastErrorKey = "Value"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Value = 12
                        },
                        Errors = NoErrors, FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Nullable = true
                        },
                        Errors = NoErrors, FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "AsType",
                Specification = s => s.Member(m => m.Child, m => m
                    .AsType(new Specification<TestParent>(p => p
                        .Member(p1 => p1.ParentNumber, n => n.NonZero().WithMessage("Must not be zero")))
                    )
                ),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                MessageKey.Global.Required
                            }
                        }
                    },
                    ["Child"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                MessageKey.Global.Required
                            }
                        }
                    },
                    ["Child.ParentNumber"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[]
                            {
                                "Must not be zero"
                            }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Child = new TestChild() { ParentNumber = 0 }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Child.ParentNumber"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[]
                                    {
                                        "Must not be zero"
                                    }
                                }
                            },
                        },
                        FailFastErrorKey = "Child.ParentNumber"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Child = new TestChild() { ParentNumber = 10 }
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "AsModel",
                Specification = s => s.Member(m => m.Member, m => m
                    .AsModel(m1 => m1.Rule(m2 => m2.MemberText != null).WithMessage("message 1"))),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xyz" } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "AsDictionary_Simple",
                Specification = s => s.Member(m => m.SimpleDictionary, m => m
                    .AsDictionary(m1 => m1
                        .Rule(v => v.Length == 3).WithMessage("message 3")
                        .Rule(v => v.Contains("x")).WithMessage("message x")
                    )
                ),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["SimpleDictionary"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["SimpleDictionary.#"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message x" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SimpleDictionary = new Dictionary<string, string>()
                            {
                                { "key1", "xx1" },
                                { "key2", "xxx2" },
                                { "key3", "xx2" },
                                { "key4", "xx4" },
                                { "key5", "yy5" },
                                { "key6", "xx6" },
                                { "key7", "oops" },
                                { "key8", "8xx" },
                                { "key9", null },
                            }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["SimpleDictionary.key2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.key5"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.key7"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.key9"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Global.Required" }
                                },
                            },
                        },
                        FailFastErrorKey = "SimpleDictionary.key2"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SimpleDictionary = new Dictionary<string, string>()
                            {
                                { "key.", "x with dot at end" },
                                { "key 2", "x with space" },
                                { "key .", "x with space and dot" },
                                { "k.e.y", "with dots" },
                                { "...ke....y..", "with dots" },
                                { "k e y", "with spaces" },
                                { "   ", "x with three spaces" },
                                { string.Empty, "x with empty" },
                                { "<key3", "x with angle bracket" },
                                { "<<<key4", "with angle brackets" },
                                { "< < <key", "with angle brackets and spaces" },
                                { "<< <k.e.y.", "with angle brackets, spaces and dots" },
                            }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["SimpleDictionary.key"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.key 2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.key "] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.k.e.y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.ke.y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.k e y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.   "] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary. "] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.key3"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.key4"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary. < <key"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary. <k.e.y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                        },
                        FailFastErrorKey = "SimpleDictionary.key",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SimpleDictionary = new Dictionary<string, string>()
                            {
                                { "<<key", "aaa" },
                                { "<<<<key", "abcx" },
                                { "...key2...", "x with space and dot" },
                                { ".key2.", "abc" },
                                { "<<.key3..", "abc" },
                                { "<.key3.", "abc" },
                                { "<<<<.key3....", "" },
                            }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["SimpleDictionary.key"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.key2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.key3"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                        },
                        FailFastErrorKey = "SimpleDictionary.key",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SimpleDictionary = new Dictionary<string, string>()
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null,
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SimpleDictionary = new Dictionary<string, string>()
                            {
                                { "key1", "xx1" },
                                { "key2", "xx2" },
                                { "key3", "xx3" },
                                { "<<key4", "xx4" }
                            }
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null,
                    },
                }
            },
            new TestCase()
            {
                Name = "AsDictionary_Simple_WithKeyStringifier",
                Specification = s => s.Member(m => m.SimpleDictionary, m => m
                    .AsDictionary(
                        m1 => m1
                        .Rule(v => v.Length == 3).WithMessage("message 3")
                        .Rule(v => v.Contains("x")).WithMessage("message x"),
                        key => key.ToUpperInvariant()
                    )
                ),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["SimpleDictionary"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["SimpleDictionary.#"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message x" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SimpleDictionary = new Dictionary<string, string>()
                            {
                                { "key1", "xx1" },
                                { "key2", "xxx2" },
                                { "key3", "xx2" },
                                { "key4", "xx4" },
                                { "key5", "yy5" },
                                { "key6", "xx6" },
                                { "key7", "oops" },
                                { "key8", "8xx" },
                                { "key9", null },
                            }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["SimpleDictionary.KEY2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.KEY5"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.KEY7"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.KEY9"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Global.Required" }
                                },
                            },
                        },
                        FailFastErrorKey = "SimpleDictionary.KEY2"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SimpleDictionary = new Dictionary<string, string>()
                            {
                                { "key.", "x with dot at end" },
                                { "key 2", "x with space" },
                                { "key .", "x with space and dot" },
                                { "k.e.y", "with dots" },
                                { "...ke....y..", "with dots" },
                                { "k e y", "with spaces" },
                                { "   ", "x with three spaces" },
                                { string.Empty, "x with empty" },
                                { "<key3", "x with angle bracket" },
                                { "<<<key4", "with angle brackets" },
                                { "< < <key", "with angle brackets and spaces" },
                                { "<< <k.e.y.", "with angle brackets, spaces and dots" },
                            }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["SimpleDictionary.KEY"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.KEY 2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.KEY "] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.K.E.Y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.KE.Y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.K E Y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.   "] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary. "] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.KEY3"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.KEY4"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary. < <KEY"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary. <K.E.Y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                        },
                        FailFastErrorKey = "SimpleDictionary.KEY",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SimpleDictionary = new Dictionary<string, string>()
                            {
                                { "<<key", "aaa" },
                                { "<<<<key", "abcx" },
                                { "...key2...", "x with space and dot" },
                                { ".key2.", "abc" },
                                { "<<.key3..", "abc" },
                                { "<.key3.", "abc" },
                                { "<<<<.key3....", "" },
                            }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["SimpleDictionary.KEY"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["SimpleDictionary.KEY2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["SimpleDictionary.KEY3"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                        },
                        FailFastErrorKey = "SimpleDictionary.KEY",
                    },
                }
            },
            new TestCase()
            {
                Name = "AsDictionary_Complex",
                Specification = s => s.Member(m => m.ComplexDictionary, m => m.AsDictionary(
                    m1 => m1
                        .Rule(v => v.Hybrid?.Length == 3).WithMessage("message 3")
                        .Rule(v => v.Hybrid?.Contains("x") == true).WithMessage("message x"),
                    k => k.MemberText.ToUpperInvariant())
                ),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["ComplexDictionary"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["ComplexDictionary.#"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message x" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            ComplexDictionary = new Dictionary<TestMember, TestClass>()
                            {
                                { new TestMember() { MemberText = "key1" }, new TestClass() { Hybrid = "xx1" } },
                                { new TestMember() { MemberText = "key2" }, new TestClass() { Hybrid = "xxx2" } },
                                { new TestMember() { MemberText = "key3" }, new TestClass() { Hybrid = "xx2" } },
                                { new TestMember() { MemberText = "key4" }, new TestClass() { Hybrid = "xx4" } },
                                { new TestMember() { MemberText = "key5" }, new TestClass() { Hybrid = "yy5" } },
                                { new TestMember() { MemberText = "key6" }, new TestClass() { Hybrid = "xx6" } },
                                { new TestMember() { MemberText = "key7" }, new TestClass() { Hybrid = "oops" } },
                                { new TestMember() { MemberText = "key8" }, new TestClass() { Hybrid = "8xx" } },
                                { new TestMember() { MemberText = "key9" }, new TestClass() { Hybrid = null } },
                                { new TestMember() { MemberText = "key10" }, null },
                            }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["ComplexDictionary.KEY2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["ComplexDictionary.KEY5"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["ComplexDictionary.KEY7"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["ComplexDictionary.KEY9"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["ComplexDictionary.KEY10"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "Global.Required" }
                                },
                            },
                        },
                        FailFastErrorKey = "ComplexDictionary.KEY2"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            ComplexDictionary = new Dictionary<TestMember, TestClass>()
                            {
                                { new TestMember() { MemberText = "key." }, new TestClass() { Hybrid = "x with dot at end" } },
                                { new TestMember() { MemberText = "key 2" }, new TestClass() { Hybrid = "x with space" } },
                                { new TestMember() { MemberText = "key ." }, new TestClass() { Hybrid = "x with space and dot" } },
                                { new TestMember() { MemberText = "k.e.y" }, new TestClass() { Hybrid = "with dots" } },
                                { new TestMember() { MemberText = "...ke....y.." }, new TestClass() { Hybrid = "with dots" } },
                                { new TestMember() { MemberText = "k e y" }, new TestClass() { Hybrid = "with spaces" } },
                                { new TestMember() { MemberText = "   " }, new TestClass() { Hybrid = "x with three spaces" } },
                                { new TestMember() { MemberText = string.Empty }, new TestClass() { Hybrid = "x with empty" } },
                                { new TestMember() { MemberText = "<key3" }, new TestClass() { Hybrid = "x with angle bracket" } },
                                { new TestMember() { MemberText = "<<<key4" }, new TestClass() { Hybrid = "with angle brackets" } },
                                { new TestMember() { MemberText = "< < <key" }, new TestClass() { Hybrid = "with angle brackets and spaces" } },
                                { new TestMember() { MemberText = "<< <k.e.y." }, new TestClass() { Hybrid = "with angle brackets, spaces and dots" } },
                            }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["ComplexDictionary.KEY"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["ComplexDictionary.KEY 2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["ComplexDictionary.KEY "] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["ComplexDictionary.K.E.Y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["ComplexDictionary.KE.Y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["ComplexDictionary.K E Y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["ComplexDictionary.   "] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["ComplexDictionary. "] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["ComplexDictionary.KEY3"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            },
                            ["ComplexDictionary.KEY4"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["ComplexDictionary. < <KEY"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["ComplexDictionary. <K.E.Y"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                        },
                        FailFastErrorKey = "ComplexDictionary.KEY",
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            ComplexDictionary = new Dictionary<TestMember, TestClass>()
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null,
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            ComplexDictionary = new Dictionary<TestMember, TestClass>()
                            {
                                { new TestMember() { MemberText = "key1" }, new TestClass() { Hybrid = "xx1" } },
                                { new TestMember() { MemberText = "key2" }, new TestClass() { Hybrid = "xx2" } },
                                { new TestMember() { MemberText = "<<key3" }, new TestClass() { Hybrid = "xx3" } },
                                { new TestMember() { MemberText = "<<x.key4" }, new TestClass() { Hybrid = "xx4" } },
                            }
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null,
                    },
                }
            }
        };

        public static IReadOnlyList<TestCase> PathCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "Member_Raw",
                Specification = s => s
                    .Member(m => m.Member, m => m.Rule(m1 => m1.MemberText != null).WithMessage("message 1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xyz" } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "Member_Rule_LevelUp",
                Specification = s => s
                    .Member(m => m.Member, m => m.Rule(m1 => m1.MemberText != null).WithPath("<").WithMessage("message 1")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xyz" } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "Member_Renamed",
                Specification = s => s
                    .Member(m => m.Member, m => m.Rule(m1 => m1.MemberText != null).WithMessage("message 1")).WithPath("Renamed"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Renamed"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Renamed"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = "Renamed"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Renamed"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Renamed"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xyz" } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "Member_Rule_LevelDown",
                Specification = s => s
                    .Member(m => m.Member, m => m
                        .Rule(m1 => m1.MemberText != null).WithPath("SomeInnerName").WithMessage("message 1")
                    ),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member.SomeInnerName"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member.SomeInnerName"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Member.SomeInnerName"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xyz" } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "Member_LevelDown",
                Specification = s => s
                    .Member(m => m.Member, m => m
                        .Rule(m1 => m1.MemberText != null).WithMessage("message 1")
                    ).WithPath("Member.SomeInnerName"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member.SomeInnerName"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member.SomeInnerName"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = "Member.SomeInnerName"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member.SomeInnerName"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Member.SomeInnerName"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xyz" } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "MultipleCommandsToSinglePath",
                Specification = s => s
                    .Member(m => m.Member, m => m
                        .Rule(m1 => m1.MemberText != null).WithMessage("message 1")
                    ).WithPath("Single.Path")
                    .Member(m => m.Collection, m => m
                        .AsCollection<TestCollection<int>, int>(m1 => m1.Rule(m3 => m3 != 0).WithPath("<").WithMessage("message 2"))
                    ).WithPath("Single.Path")
                    .Member(m => m.Collection, m => m
                        .AsCollection<TestCollection<int>, int>(m1 => m1.Rule(m3 => m3 != 0).WithPath("<<Single.Path").WithMessage("message 3"))
                    )
                    .Member(m => m.Nullable, m => m
                        .AsNullable(m1 => m1.Rule(m3 => m3).WithMessage("message 4"))
                    ).WithPath("Single.Path")
                    .Member(m => m.Nullable, m => m
                        .AsNullable(m1 => m1.Rule(m3 => m3).WithPath("<Single.Path").WithMessage("message 5"))
                    )
                    .Member(m => m.Self, m => m
                        .AsModel(m1 => m1.Rule(m3 => m3.Value != 0).WithMessage("message 6"))
                    ).WithPath("<Single.Path")
                    .Member(m => m.Self, m => m
                        .AsModel(m1 => m1.Rule(m3 => m3.Value != 0).WithPath("<Single.Path").WithMessage("message 7"))
                    )
                    .AsModel(m1 => m1.Rule(m3 => m3.Value != 0).WithPath("Single.Path").WithMessage("message 8")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Collection"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Nullable"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Self"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Single.Path"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 2" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 4" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 5" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 6" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 7" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 8" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Collection"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            },
                            ["Nullable"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            },
                            ["Self"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            },
                            ["Single.Path"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 8" }
                                }
                            },
                        },
                        FailFastErrorKey = "Single.Path"
                    },
                },
            },
            new TestCase()
            {
                Name = "CollectionLevels",
                Specification = s => s
                    .Member(m => m.Collection, m => m
                        .AsCollection<TestCollection<int>, int>(m1 => m1
                            .Rule(m3 => m3 != 0).WithPath("Zero").WithMessage("message !0")
                            .Rule(m3 => m3 != 1).WithPath("<").WithMessage("message !1")
                            .Rule(m3 => m3 != 2).WithPath("<<Up").WithMessage("message !2")
                            .Rule(m3 => m3 != 3).WithPath("Down.Down").WithMessage("message !3")
                        )
                    ),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Collection.#.Zero"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message !0" }
                        }
                    },
                    ["Collection"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message !1" }
                        }
                    },
                    ["Up"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message !2" }
                        }
                    },
                    ["Collection.#.Down.Down"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message !3" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Collection = new TestCollection<int>(new[]
                            {
                                0,
                                1,
                                2,
                                3,
                                0,
                                3
                            })
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Collection.#0.Zero"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message !0" }
                                }
                            },
                            ["Collection"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message !1" }
                                }
                            },
                            ["Up"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message !2" }
                                }
                            },
                            ["Collection.#3.Down.Down"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message !3" }
                                }
                            },
                            ["Collection.#4.Zero"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message !0" }
                                }
                            },
                            ["Collection.#5.Down.Down"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message !3" }
                                }
                            },
                        },
                        FailFastErrorKey = "Collection.#0.Zero"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Collection = new TestCollection<int>(new[]
                            {
                                2,
                                2,
                                1,
                                1
                            })
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Up"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message !2" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message !2" }
                                }
                            },
                            ["Collection"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message !1" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message !1" }
                                }
                            },
                        },
                        FailFastErrorKey = "Up"
                    },
                },
            },
            new TestCase()
            {
                Name = "ChainedAsModel_RenamedInside",
                Specification = s => s
                    .AsModel(m => m.Rule(m1 => m1.Collection.Contains(0)).WithPath("M0").WithMessage("message 0"))
                    .AsModel(m => m.Rule(m1 => m1.Collection.Contains(1)).WithPath("M1").WithMessage("message 1"))
                    .AsModel(m => m.Rule(m1 => m1.Collection.Contains(2)).WithPath("M2").WithMessage("message 2"))
                    .AsModel(m => m.Rule(m1 => m1.Collection.Contains(3)).WithPath("M3").WithMessage("message 3")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["M0"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 0" }
                        }
                    },
                    ["M1"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    },
                    ["M2"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 2" }
                        }
                    },
                    ["M3"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" }
                        }
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            },
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Collection = new TestCollection<int>(new[]
                            {
                                0,
                                2,
                            })
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["M1"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                }
                            },
                            ["M3"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                }
                            },
                        },
                        FailFastErrorKey = "M1"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Collection = new TestCollection<int>(new[]
                            {
                                0,
                                2,
                            })
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["M1"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                }
                            },
                            ["M3"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                }
                            },
                        },
                        FailFastErrorKey = "M1"
                    },
                },
            },
            new TestCase()
            {
                Name = "ChainedAsModel_RenamedOutside",
                Specification = s => s
                    .AsModel(m => m.Rule(m1 => m1.Collection.Contains(0)).WithMessage("message 0")).WithPath("M0")
                    .AsModel(m => m.Rule(m1 => m1.Collection.Contains(1)).WithMessage("message 1")).WithPath("M1")
                    .AsModel(m => m.Rule(m1 => m1.Collection.Contains(2)).WithMessage("message 2")).WithPath("M2")
                    .AsModel(m => m.Rule(m1 => m1.Collection.Contains(3)).WithMessage("message 3")).WithPath("M3"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["M0"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 0" }
                        }
                    },
                    ["M1"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        }
                    },
                    ["M2"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 2" }
                        }
                    },
                    ["M3"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" }
                        }
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                }
                            },
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Collection = new TestCollection<int>(new[]
                            {
                                0,
                                2,
                            })
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["M1"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                }
                            },
                            ["M3"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                }
                            },
                        },
                        FailFastErrorKey = "M1"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Collection = new TestCollection<int>(new[]
                            {
                                0,
                                2,
                            })
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["M1"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                }
                            },
                            ["M3"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                }
                            },
                        },
                        FailFastErrorKey = "M1"
                    },
                },
            },
        };

        public static IReadOnlyList<TestCase> OverwritingCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "Member_SingleInnerRule",
                Specification = s => s
                    .Member(m => m.Member, m => m.Rule(m1 => m1.MemberText != null).WithMessage("inner message 1")).WithMessage("outer message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "outer message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xyz" } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "Member_MultipleInnerRules",
                Specification = s => s
                    .Member(m => m.Member, m => m
                        .Rule(m1 => m1.MemberText != null).WithMessage("inner message 1")
                        .Rule(m1 => m1.MemberText?.Contains("x") == true).WithMessage("inner message 2")
                        .Rule(m1 => m1.MemberText?.Contains("z") == true).WithMessage("inner message 3")
                    ).WithMessage("outer message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["Member"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "outer message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "abc" } },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xxx" } },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = "Member"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Member = new TestMember() { MemberText = "xyz" } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "AsModel_SingleInnerRule",
                Specification = s => s
                    .AsModel(m => m.Rule(m1 => m1.Hybrid != null).WithMessage("inner message 1")).WithMessage("outer message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "outer message 1" }
                        }
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Hybrid = "xyz" },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "AsModel_MultipleInnerRules",
                Specification = s => s
                    .AsModel(m => m
                        .Rule(m1 => m1.Hybrid != null).WithMessage("inner message 1")
                        .Rule(m1 => m1.Hybrid?.Contains("x") == true).WithMessage("inner message 2")
                        .Rule(m1 => m1.Hybrid?.Contains("z") == true).WithMessage("inner message 3")
                    ).WithMessage("outer message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "outer message 1" }
                        }
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Hybrid = "" },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Hybrid = "abc" },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Hybrid = "xxx" },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Hybrid = "xyz" },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "Collection",
                Specification = s => s
                    .Member(m => m.SelfCollection, m => m.AsCollection<TestCollection<TestClass>, TestClass>(mx => mx
                        .Rule(m1 => m1.Hybrid != null).WithMessage("inner message 1")
                        .Rule(m1 => m1.Hybrid?.Contains("x") == true).WithMessage("inner message 2")
                        .Rule(m1 => m1.Hybrid?.Contains("z") == true).WithMessage("inner message 3")
                    )).WithMessage("outer message 1"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        }
                    },
                    ["SelfCollection"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "outer message 1" }
                        }
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["SelfCollection"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = "SelfCollection"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { SelfCollection = new TestCollection<TestClass>(new TestClass[] { }) },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SelfCollection = new TestCollection<TestClass>(new TestClass[]
                            {
                                new TestClass() { Hybrid = "abc" },
                                new TestClass() { Hybrid = "xyz" },
                                new TestClass() { Hybrid = "xyz" },
                            })
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["SelfCollection"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = "SelfCollection"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SelfCollection = new TestCollection<TestClass>(new TestClass[]
                            {
                                new TestClass() { Hybrid = "xxx" },
                                new TestClass() { Hybrid = "xxx" },
                                new TestClass() { Hybrid = "xxx" },
                            })
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["SelfCollection"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "outer message 1" }
                                }
                            }
                        },
                        FailFastErrorKey = "SelfCollection"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            SelfCollection = new TestCollection<TestClass>(new TestClass[]
                            {
                                new TestClass() { Hybrid = "xyz" },
                                new TestClass() { Hybrid = "xyz" },
                                new TestClass() { Hybrid = "xyz" },
                            })
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
        };

        public static IReadOnlyList<TestCase> FailFastCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "TopLevelRules",
                Specification = s => s
                    .Rule(m => m.Hybrid?.Contains("x") == true).WithMessage("message x")
                    .Rule(m => m.Hybrid?.Contains("y") == true).WithMessage("message y")
                    .Rule(m => m.Hybrid?.Contains("z") == true).WithMessage("message z"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message x" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message y" }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message z" }
                        }
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message y" }
                                },
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message z" }
                                }
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Hybrid = "xyz" },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "Members",
                Specification = s => s
                    .Member(m => m.Nullable, m => m.Rule(m1 => m1 == true).WithMessage("message 1"))
                    .Member(m => m.Value, m => m.Rule(m1 => m1 == 1).WithMessage("message 2"))
                    .Member(m => m.StructMember, m => m.Rule(m1 => m1.StructNumber == 1).WithMessage("message 3")),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                    },
                    ["Nullable"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        },
                    },
                    ["Value"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 2" }
                        },
                    },
                    ["StructMember"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 3" }
                        },
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Nullable = null,
                            Value = 0,
                            StructMember = new TestStruct() { StructNumber = 0 }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Nullable"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            },
                            ["Value"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 2" }
                                },
                            },
                            ["StructMember"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            }
                        },
                        FailFastErrorKey = "Nullable"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Nullable = false,
                            Value = 1,
                            StructMember = new TestStruct() { StructNumber = 0 }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Nullable"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["StructMember"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            }
                        },
                        FailFastErrorKey = "Nullable"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Nullable = true,
                            Value = 0,
                            StructMember = new TestStruct() { StructNumber = 0 }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Value"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 2" }
                                },
                            },
                            ["StructMember"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            }
                        },
                        FailFastErrorKey = "Value"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Nullable = true,
                            Value = 1,
                            StructMember = new TestStruct() { StructNumber = 0 }
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["StructMember"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 3" }
                                },
                            }
                        },
                        FailFastErrorKey = "StructMember"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Nullable = true,
                            Value = 1,
                            StructMember = new TestStruct() { StructNumber = 1 }
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "Collection",
                Specification = s => s
                    .Member(m => m.Collection, m => m.AsCollection<TestCollection<int>, int>(m1 => m1
                        .Rule(m2 => m2 != 0).WithMessage("message 1")
                    )),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                    },
                    ["Collection"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                    },
                    ["Collection.#"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" }
                        },
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass(),
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Collection"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { MessageKey.Global.Required }
                                },
                            }
                        },
                        FailFastErrorKey = "Collection"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Collection = new TestCollection<int>(new[] { 0, 1, 0 }) },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Collection.#0"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["Collection.#2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Collection.#0"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Collection = new TestCollection<int>(new[] { 1, 0, 0 }) },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Collection.#1"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["Collection.#2"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Collection.#1"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Collection = new TestCollection<int>(new[] { 1, 1, 1 }) },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
        };

        public static IReadOnlyList<TestCase> MixedCases { get; } = new[]
        {
            new TestCase()
            {
                Name = "WithCondition_and_WithPath",
                Specification = s => s
                    .Rule(m => m.Hybrid?.Contains("x") == true).WithCondition(m => m.HybridField?.Contains("x") == true).WithPath("XXX").WithMessage("message x")
                    .Rule(m => m.Hybrid?.Contains("y") == true).WithCondition(m => m.HybridField?.Contains("y") == true).WithPath("YYY").WithMessage("message y")
                    .Rule(m => m.Hybrid?.Contains("z") == true).WithCondition(m => m.HybridField?.Contains("z") == true).WithPath("ZZZ").WithMessage("message z"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                    },
                    ["XXX"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message x" }
                        },
                    },
                    ["YYY"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message y" }
                        },
                    },
                    ["ZZZ"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message z" }
                        },
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            HybridField = "xyz"
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["XXX"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["YYY"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message y" }
                                },
                            },
                            ["ZZZ"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message z" }
                                },
                            }
                        },
                        FailFastErrorKey = "XXX"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Hybrid = "y",
                            HybridField = "xyz"
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["XXX"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["ZZZ"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message z" }
                                },
                            }
                        },
                        FailFastErrorKey = "XXX"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Hybrid = "xz",
                            HybridField = "y"
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["YYY"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message y" }
                                },
                            },
                        },
                        FailFastErrorKey = "YYY"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Hybrid = "xyz",
                            HybridField = "xyz"
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Hybrid = "xyz"
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
            new TestCase()
            {
                Name = "WithCondition_and_WithPath_AndVersion",
                Specification = s => s
                    .Rule(m => m.Hybrid?.Contains("x") == true).WithCondition(m => m.HybridField?.Contains("x") == true).WithPath("XXX").WithMessage("message x")
                    .And()
                    .Rule(m => m.Hybrid?.Contains("y") == true).WithCondition(m => m.HybridField?.Contains("y") == true).WithPath("YYY").WithMessage("message y")
                    .And()
                    .Rule(m => m.Hybrid?.Contains("z") == true).WithCondition(m => m.HybridField?.Contains("z") == true).WithPath("ZZZ").WithMessage("message z"),
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.Required }
                        },
                    },
                    ["XXX"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message x" }
                        },
                    },
                    ["YYY"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message y" }
                        },
                    },
                    ["ZZZ"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message z" }
                        },
                    }
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            HybridField = "xyz"
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["XXX"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["YYY"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message y" }
                                },
                            },
                            ["ZZZ"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message z" }
                                },
                            }
                        },
                        FailFastErrorKey = "XXX"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Hybrid = "y",
                            HybridField = "xyz"
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["XXX"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message x" }
                                },
                            },
                            ["ZZZ"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message z" }
                                },
                            }
                        },
                        FailFastErrorKey = "XXX"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Hybrid = "xz",
                            HybridField = "y"
                        },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["YYY"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message y" }
                                },
                            },
                        },
                        FailFastErrorKey = "YYY"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Hybrid = "xyz",
                            HybridField = "xyz"
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass()
                        {
                            Hybrid = "xyz"
                        },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                },
            },
        };

        public static IEnumerable<TestCase> ReferencesLoopCases()
        {
            Specification<TestClass> directSpecification = null;

            directSpecification = s => s
                .Optional()
                .Member(m => m.Self, directSpecification)
                .Rule(m => m.Value != 0).WithMessage("message 1");

            TestClass direct1 = new TestClass();

            direct1.Self = direct1;

            TestClass direct2 = new TestClass()
            {
                Self = new TestClass()
                {
                    Self = new TestClass()
                    {
                        Self = new TestClass()
                    }
                }
            };

            direct2.Self.Self.Self.Self = direct2;

            TestClass direct3 = new TestClass()
            {
                Self = new TestClass()
                {
                    Self = new TestClass()
                    {
                        Self = new TestClass()
                    }
                }
            };

            direct3.Self.Self.Self.Self = direct3.Self.Self;

            yield return new TestCase()
            {
                Name = "Direct",
                Specification = directSpecification,
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" },
                        },
                    },
                    ["Self"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.ReferenceLoop },
                            Args = new[] { new ArgTestCase() { Name = "type", Value = typeof(TestClass) } }
                        },
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Self = new TestClass() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["Self"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Self"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Self = new TestClass() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Self"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Self"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Self = new TestClass() { Value = 0, Self = new TestClass() { Value = 0 } } },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Self"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["Self.Self"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Self.Self"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Self = new TestClass() { Value = 2, Self = new TestClass() { Value = 3 } } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = direct1,
                        ReferenceLoopExceptionCase = new ReferenceLoopExceptionCase()
                        {
                            Type = typeof(TestClass),
                            NestedPath = "Self",
                            Path = ""
                        },
                    },
                    new ValidationTestCase()
                    {
                        Model = direct2,
                        ReferenceLoopExceptionCase = new ReferenceLoopExceptionCase()
                        {
                            Type = typeof(TestClass),
                            NestedPath = "Self.Self.Self.Self",
                            Path = ""
                        },
                    },
                    new ValidationTestCase()
                    {
                        Model = direct3,
                        ReferenceLoopExceptionCase = new ReferenceLoopExceptionCase()
                        {
                            Type = typeof(TestClass),
                            NestedPath = "Self.Self.Self.Self",
                            Path = "Self.Self"
                        },
                    },
                },
            };

            Specification<TestClass> indirectSpecification1 = null;
            Specification<TestMember> indirectSpecification2;

            indirectSpecification2 = s => s
                .Optional()
                .Member(m => m.NestedSelf, indirectSpecification1);

            indirectSpecification1 = s => s
                .Optional()
                .Member(m => m.Member, indirectSpecification2)
                .Rule(m => m.Value != 0).WithMessage("message 1");

            var indirect1 = new TestClass();

            indirect1.Member = new TestMember()
            {
                NestedSelf = indirect1
            };

            var indirect2 = new TestClass();

            indirect2.Member = new TestMember()
            {
                NestedSelf = new TestClass()
                {
                    Member = new TestMember()
                    {
                        NestedSelf = new TestClass()
                        {
                            Member = new TestMember()
                            {
                                NestedSelf = new TestClass()
                                {
                                    Member = new TestMember()
                                }
                            }
                        }
                    }
                }
            };

            indirect2.Member.NestedSelf.Member.NestedSelf.Member.NestedSelf.Member.NestedSelf = indirect2;

            var indirect3 = new TestClass();

            indirect3.Member = new TestMember()
            {
                NestedSelf = new TestClass()
                {
                    Member = new TestMember()
                    {
                        NestedSelf = new TestClass()
                        {
                            Member = new TestMember()
                            {
                                NestedSelf = new TestClass()
                                {
                                    Member = new TestMember()
                                }
                            }
                        }
                    }
                }
            };

            indirect3.Member.NestedSelf.Member.NestedSelf.Member.NestedSelf.Member.NestedSelf = indirect3.Member.NestedSelf.Member.NestedSelf.Member.NestedSelf;

            var indirect4 = new TestClass();

            indirect4.Member = new TestMember()
            {
                NestedSelf = new TestClass()
                {
                    Member = new TestMember()
                    {
                        NestedSelf = new TestClass()
                        {
                            Member = new TestMember()
                            {
                                NestedSelf = new TestClass()
                            }
                        }
                    }
                }
            };

            indirect4.Member.NestedSelf.Member.NestedSelf.Member.NestedSelf.Member = indirect4.Member;

            var indirect5 = new TestClass();

            indirect5.Member = new TestMember()
            {
                NestedSelf = new TestClass()
                {
                    Member = new TestMember()
                    {
                        NestedSelf = new TestClass()
                        {
                            Member = new TestMember()
                            {
                                NestedSelf = new TestClass()
                            }
                        }
                    }
                }
            };

            indirect5.Member.NestedSelf.Member.NestedSelf.Member.NestedSelf.Member = indirect5.Member.NestedSelf.Member.NestedSelf.Member;

            yield return new TestCase()
            {
                Name = "Indirect",
                Specification = indirectSpecification1,
                ExpectedTemplate = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                {
                    [""] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { "message 1" },
                        },
                    },
                    ["Member.NestedSelf"] = new[]
                    {
                        new ErrorTestCase()
                        {
                            Messages = new[] { MessageKey.Global.ReferenceLoop },
                            Args = new[] { new ArgTestCase() { Name = "type", Value = typeof(TestClass) } }
                        },
                    },
                },
                ValidationCases = new[]
                {
                    new ValidationTestCase()
                    {
                        Model = null,
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0 },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Member = new TestMember() },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                        },
                        FailFastErrorKey = ""
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 0, Member = new TestMember() { NestedSelf = new TestClass() } },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            [""] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["Member.NestedSelf"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Member.NestedSelf"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Member = new TestMember() { NestedSelf = new TestClass() { Member = new TestMember() { NestedSelf = new TestClass() } } } },
                        Errors = new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                        {
                            ["Member.NestedSelf"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            },
                            ["Member.NestedSelf.Member.NestedSelf"] = new[]
                            {
                                new ErrorTestCase()
                                {
                                    Messages = new[] { "message 1" }
                                },
                            }
                        },
                        FailFastErrorKey = "Member.NestedSelf.Member.NestedSelf"
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1 },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = new TestClass() { Value = 1, Member = new TestMember() { NestedSelf = new TestClass() { Value = 1, Member = new TestMember() { NestedSelf = new TestClass() { Value = 1 } } } } },
                        Errors = NoErrors,
                        FailFastErrorKey = null
                    },
                    new ValidationTestCase()
                    {
                        Model = indirect1,
                        ReferenceLoopExceptionCase = new ReferenceLoopExceptionCase()
                        {
                            Type = typeof(TestClass),
                            NestedPath = "Member.NestedSelf",
                            Path = ""
                        },
                    },
                    new ValidationTestCase()
                    {
                        Model = indirect2,
                        ReferenceLoopExceptionCase = new ReferenceLoopExceptionCase()
                        {
                            Type = typeof(TestClass),
                            NestedPath = "Member.NestedSelf.Member.NestedSelf.Member.NestedSelf.Member.NestedSelf",
                            Path = ""
                        },
                    },
                    new ValidationTestCase()
                    {
                        Model = indirect3,
                        ReferenceLoopExceptionCase = new ReferenceLoopExceptionCase()
                        {
                            Type = typeof(TestClass),
                            NestedPath = "Member.NestedSelf.Member.NestedSelf.Member.NestedSelf.Member.NestedSelf",
                            Path = "Member.NestedSelf.Member.NestedSelf.Member.NestedSelf"
                        },
                    },
                    new ValidationTestCase()
                    {
                        Model = indirect4,
                        ReferenceLoopExceptionCase = new ReferenceLoopExceptionCase()
                        {
                            Type = typeof(TestMember),
                            NestedPath = "Member.NestedSelf.Member.NestedSelf.Member.NestedSelf.Member",
                            Path = "Member"
                        },
                    },
                    new ValidationTestCase()
                    {
                        Model = indirect5,
                        ReferenceLoopExceptionCase = new ReferenceLoopExceptionCase()
                        {
                            Type = typeof(TestMember),
                            NestedPath = "Member.NestedSelf.Member.NestedSelf.Member.NestedSelf.Member",
                            Path = "Member.NestedSelf.Member.NestedSelf.Member"
                        },
                    },
                },
            };
        }

        public static IEnumerable<object[]> CasesForTemplate_Data()
        {
            return GetCases().Select(c => new object[]
            {
                $"M_{c.Name}",
                c.Specification,
                c.ExpectedTemplate
            });
        }

        public static IEnumerable<object[]> CasesForValidation_Data()
        {
            foreach (var c in GetCases())
            {
                var i = 0;

                foreach (var v in c.ValidationCases)
                {
                    yield return new object[]
                    {
                        $"V_{c.Name}_{++i}",
                        c.Specification,
                        v.Model,
                        v.Errors,
                        v.ReferenceLoopExceptionCase
                    };
                }
            }
        }

        public static IEnumerable<object[]> CasesForReferenceLoop_Data()
        {
            var cases = new List<TestCase>();

            cases.AddRange(RulesCases.Select(c => RenamedClone(c, nameof(RulesCases))));
            cases.AddRange(ReferencesLoopCases().Select(c => RenamedClone(c, nameof(ReferencesLoopCases))));

            foreach (var c in cases)
            {
                var i = 0;

                foreach (var v in c.ValidationCases)
                {
                    yield return new object[]
                    {
                        $"RL_T_{c.Name}_{++i}",
                        true,
                        c.Specification,
                        v.Model,
                        v.Errors,
                        v.ReferenceLoopExceptionCase
                    };

                    if (v.ReferenceLoopExceptionCase is null)
                    {
                        yield return new object[]
                        {
                            $"RL_F_{c.Name}_{++i}",
                            false,
                            c.Specification,
                            v.Model,
                            v.Errors,
                            v.ReferenceLoopExceptionCase
                        };
                    }
                }
            }
        }

        public static IEnumerable<object[]> CasesForValidationWithFailFast_Data()
        {
            foreach (var c in GetCases())
            {
                var i = 0;

                foreach (var v in c.ValidationCases)
                {
                    yield return new object[]
                    {
                        $"F_{c.Name}_{++i}",
                        c.Specification,
                        v.Model,
                        v.FailFastErrorKey is null
                            ? NoErrors
                            : new Dictionary<string, IReadOnlyList<ErrorTestCase>>()
                            {
                                [v.FailFastErrorKey] = new[]
                                {
                                    v.Errors[v.FailFastErrorKey][0]
                                }
                            },
                        v.ReferenceLoopExceptionCase
                    };
                }
            }
        }

        public static IEnumerable<object[]> CasesForIsValid_Data()
        {
            foreach (var c in GetCases())
            {
                var i = 0;

                foreach (var v in c.ValidationCases)
                {
                    yield return new object[]
                    {
                        $"I_{c.Name}_{++i}",
                        c.Specification,
                        v.Model,
                        v.Errors == null || !v.Errors.Any(),
                        v.ReferenceLoopExceptionCase
                    };
                }
            }
        }

        public static IEnumerable<object[]> CasesForFeed_Data()
        {
            foreach (var c in GetCases())
            {
                var i = 0;

                foreach (var v in c.ValidationCases.Where(v => v.ReferenceLoopExceptionCase is null))
                {
                    yield return new object[]
                    {
                        $"FEED_{c.Name}_{++i}",
                        c.Specification,
                        v.Model,
                        c.ExpectedTemplate,
                        v.Errors,
                    };
                }
            }
        }

        public static IEnumerable<object[]> CasesForFeedMultipleTimes_Data()
        {
            foreach (var c in GetCases())
            {
                var caseData = c.ValidationCases
                    .Where(v => v.ReferenceLoopExceptionCase is null)
                    .Select(s => new
                    {
                        s.Model,
                        s.Errors
                    }).ToArray();

                yield return new object[]
                {
                    $"FEED_MULTIPLE_{c.Name}",
                    c.Specification,
                    caseData.Select(c1 => c1.Model).ToArray(),
                    c.ExpectedTemplate,
                    caseData.Select(c1 => c1.Errors).ToArray(),
                };
            }
        }
    }
}
