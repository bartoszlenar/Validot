namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Errors;
    using Validot.Specification;
    using Validot.Validation.Scopes.Builders;

    public static class ErrorContentApiHelper
    {
        public class ExpectedErrorContent
        {
            public IReadOnlyList<string> Messages { get; set; } = Array.Empty<string>();

            public IReadOnlyList<string> Codes { get; set; } = Array.Empty<string>();

            internal ErrorMode Mode { get; set; } = ErrorMode.Append;

            public bool ShouldBeEmpty(int initialMessagesAmount = 1, int initialCodesAmount = 0)
            {
                var emptyMessages = (Mode == ErrorMode.Override || initialMessagesAmount == 0) && Messages.Count == 0;

                var emptyCodes = (Mode == ErrorMode.Override || initialCodesAmount == 0) && Codes.Count == 0;

                return emptyMessages && emptyCodes;
            }

            public void Match(IError error, int initialMessagesAmount = 1, int initialCodesAmount = 0)
            {
                var messagesStartIndex = Mode == ErrorMode.Override ? 0 : initialMessagesAmount;
                var codesStartIndex = Mode == ErrorMode.Override ? 0 : initialCodesAmount;

                error.Messages.Count.Should().Be(messagesStartIndex + Messages.Count);
                error.Codes.Count.Should().Be(codesStartIndex + Codes.Count);

                for (var i = 0; i < Messages.Count; ++i)
                {
                    error.Messages[i + messagesStartIndex].Should().Be(Messages[i]);
                }

                for (var i = 0; i < Codes.Count; ++i)
                {
                    error.Codes[i + codesStartIndex].Should().Be(Codes[i]);
                }
            }
        }

        public static IEnumerable<object[]> AllCases<T>()
        {
            var list = new List<IEnumerable<object[]>>()
            {
                SettingsOnlyMessages<T>(),
                SettingsOnlyCodes<T>(),
                SettingMessagesAndCodes<T>()
            };

            foreach (var item in list)
            {
                foreach (var i in item)
                {
                    yield return i;
                }
            }
        }

        public static IEnumerable<object[]> NoCommand<T>()
        {
            yield return new object[]
            {
                "0",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    return target;
                }),

                new ExpectedErrorContent()
            };
        }

        public static IEnumerable<object[]> SettingsOnlyMessages<T>()
        {
            yield return new object[]
            {
                "M1",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithMessage<T>(target, "123");

                    return target;
                }),

                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "123"
                    }
                }
            };

            yield return new object[]
            {
                "M2",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithMessage<T>(target, "123");
                    target = WithExtraMessage<T>(target, "456");

                    return target;
                }),

                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "123",
                        "456"
                    }
                }
            };

            yield return new object[]
            {
                "M3",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithExtraMessage<T>(target, "123");

                    return target;
                }),

                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Append,
                    Messages = new[]
                    {
                        "123",
                    }
                }
            };

            yield return new object[]
            {
                "M4",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithMessage<T>(target, "123");
                    target = WithExtraMessage<T>(target, "456");
                    target = WithExtraMessage<T>(target, "789");
                    target = WithExtraMessage<T>(target, "101112");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "123",
                        "456",
                        "789",
                        "101112"
                    }
                }
            };

            yield return new object[]
            {
                "M4",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithExtraMessage<T>(target, "123");
                    target = WithExtraMessage<T>(target, "456");
                    target = WithExtraMessage<T>(target, "789");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Append,
                    Messages = new[]
                    {
                        "123",
                        "456",
                        "789",
                    }
                }
            };

            yield return new object[]
            {
                "M5",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithMessage<T>(target, "123");
                    target = WithExtraMessage<T>(target, "123");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "123",
                        "123"
                    }
                }
            };

            yield return new object[]
            {
                "M6",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithExtraMessage<T>(target, "123");
                    target = WithExtraMessage<T>(target, "123");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Append,
                    Messages = new[]
                    {
                        "123",
                        "123"
                    }
                }
            };

            yield return new object[]
            {
                "M7",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithMessage<T>(target, "123");
                    target = WithExtraMessage<T>(target, "456");
                    target = WithExtraMessage<T>(target, "456");
                    target = WithExtraMessage<T>(target, "789");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "123",
                        "456",
                        "456",
                        "789",
                    }
                }
            };

            yield return new object[]
            {
                "M8",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithErrorCleared<T>(target);

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = Array.Empty<string>()
                }
            };

            yield return new object[]
            {
                "M9",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithErrorCleared<T>(target);
                    target = WithMessage<T>(target, "123");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "123"
                    }
                }
            };

            yield return new object[]
            {
                "M9",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithErrorCleared<T>(target);
                    target = WithMessage<T>(target, "123");
                    target = WithExtraMessage<T>(target, "456");
                    target = WithExtraMessage<T>(target, "789");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "123",
                        "456",
                        "789",
                    }
                }
            };

            yield return new object[]
            {
                "M11",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithErrorCleared<T>(target);
                    target = WithMessage<T>(target, "123");
                    target = WithExtraMessage<T>(target, "123");
                    target = WithExtraMessage<T>(target, "456");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "123",
                        "123",
                        "456",
                    }
                }
            };
        }

        public static IEnumerable<object[]> SettingsOnlyCodes<T>()
        {
            yield return new object[]
            {
                "C1",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithCode<T>(target, "123");

                    return target;
                }),

                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Codes = new[]
                    {
                        "123"
                    }
                }
            };

            yield return new object[]
            {
                "C2",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithCode<T>(target, "123");
                    target = WithExtraCode<T>(target, "456");

                    return target;
                }),

                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Codes = new[]
                    {
                        "123",
                        "456"
                    }
                }
            };

            yield return new object[]
            {
                "C3",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithExtraCode<T>(target, "123");

                    return target;
                }),

                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Append,
                    Codes = new[]
                    {
                        "123",
                    }
                }
            };

            yield return new object[]
            {
                "C4",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithCode<T>(target, "123");
                    target = WithExtraCode<T>(target, "456");
                    target = WithExtraCode<T>(target, "789");
                    target = WithExtraCode<T>(target, "101112");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Codes = new[]
                    {
                        "123",
                        "456",
                        "789",
                        "101112"
                    }
                }
            };

            yield return new object[]
            {
                "C4",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithExtraCode<T>(target, "123");
                    target = WithExtraCode<T>(target, "456");
                    target = WithExtraCode<T>(target, "789");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Append,
                    Codes = new[]
                    {
                        "123",
                        "456",
                        "789",
                    }
                }
            };

            yield return new object[]
            {
                "C5",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithCode<T>(target, "123");
                    target = WithExtraCode<T>(target, "123");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Codes = new[]
                    {
                        "123",
                        "123"
                    }
                }
            };

            yield return new object[]
            {
                "C6",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithExtraCode<T>(target, "123");
                    target = WithExtraCode<T>(target, "123");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Append,
                    Codes = new[]
                    {
                        "123",
                        "123"
                    }
                }
            };

            yield return new object[]
            {
                "C7",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithCode<T>(target, "123");
                    target = WithExtraCode<T>(target, "456");
                    target = WithExtraCode<T>(target, "456");
                    target = WithExtraCode<T>(target, "789");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Codes = new[]
                    {
                        "123",
                        "456",
                        "456",
                        "789",
                    }
                }
            };

            yield return new object[]
            {
                "C8",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithErrorCleared<T>(target);

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Codes = Array.Empty<string>()
                }
            };

            yield return new object[]
            {
                "C9",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithErrorCleared<T>(target);
                    target = WithCode<T>(target, "123");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Codes = new[]
                    {
                        "123"
                    }
                }
            };

            yield return new object[]
            {
                "C9",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithErrorCleared<T>(target);
                    target = WithCode<T>(target, "123");
                    target = WithExtraCode<T>(target, "456");
                    target = WithExtraCode<T>(target, "789");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Codes = new[]
                    {
                        "123",
                        "456",
                        "789",
                    }
                }
            };

            yield return new object[]
            {
                "C11",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithErrorCleared<T>(target);
                    target = WithCode<T>(target, "123");
                    target = WithExtraCode<T>(target, "123");
                    target = WithExtraCode<T>(target, "456");

                    return target;
                }),
                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Codes = new[]
                    {
                        "123",
                        "123",
                        "456",
                    }
                }
            };
        }

        public static IEnumerable<object[]> SettingMessagesAndCodes<T>()
        {
            yield return new object[]
            {
                "MC1",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithMessage<T>(target, "m123");
                    target = WithExtraCode<T>(target, "c123");

                    return target;
                }),

                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "m123"
                    },
                    Codes = new[]
                    {
                        "c123"
                    },
                }
            };

            yield return new object[]
            {
                "MC2",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithMessage<T>(target, "m123");
                    target = WithExtraMessage<T>(target, "m456");

                    target = WithExtraCode<T>(target, "c123");

                    return target;
                }),

                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "m123",
                        "m456"
                    },
                    Codes = new[]
                    {
                        "c123",
                    },
                }
            };

            yield return new object[]
            {
                "MC3",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithExtraMessage<T>(target, "m123");

                    target = WithExtraCode<T>(target, "c123");

                    return target;
                }),

                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Append,
                    Messages = new[]
                    {
                        "m123",
                    },
                    Codes = new[]
                    {
                        "c123",
                    },
                }
            };

            yield return new object[]
            {
                "MC6",
                new Func<dynamic, ISpecificationOut<T>>(target =>
                {
                    target = WithErrorCleared<T>(target);

                    target = WithMessage<T>(target, "m123");

                    target = WithExtraCode<T>(target, "c123");

                    return target;
                }),

                new ExpectedErrorContent()
                {
                    Mode = ErrorMode.Override,
                    Messages = new[]
                    {
                        "m123",
                    },
                    Codes = new[]
                    {
                        "c123",
                    },
                }
            };
        }

        private static dynamic WithMessage<T>(dynamic api, string message)
        {
            if (api is IWithMessageIn<T> withMessageIn)
            {
                return WithMessageExtension.WithMessage<T>(withMessageIn, message);
            }

            if (api is IWithMessageForbiddenIn<T> withMessageForbiddenIn)
            {
                return WithMessageExtension.WithMessage<T>(withMessageForbiddenIn, message);
            }

            throw new InvalidOperationException("Dynamic api tests failed");
        }

        private static dynamic WithExtraMessage<T>(dynamic api, string message)
        {
            if (api is IWithExtraMessageIn<T> withExtraMessageIn)
            {
                return WithExtraMessageExtension.WithExtraMessage<T>(withExtraMessageIn, message);
            }

            if (api is IWithExtraMessageForbiddenIn<T> withExtraMessageForbiddenIn)
            {
                return WithExtraMessageExtension.WithExtraMessage<T>(withExtraMessageForbiddenIn, message);
            }

            throw new InvalidOperationException("Dynamic api tests failed");
        }

        private static dynamic WithCode<T>(dynamic api, string code)
        {
            if (api is IWithCodeIn<T> withCodeIn)
            {
                return WithCodeExtension.WithCode<T>(withCodeIn, code);
            }

            if (api is IWithCodeForbiddenIn<T> withCodeForbiddenIn)
            {
                return WithCodeExtension.WithCode<T>(withCodeForbiddenIn, code);
            }

            throw new InvalidOperationException("Dynamic api tests failed");
        }

        private static dynamic WithExtraCode<T>(dynamic api, string code)
        {
            if (api is IWithExtraCodeIn<T> withExtraCodeIn)
            {
                return WithExtraCodeExtension.WithExtraCode<T>(withExtraCodeIn, code);
            }

            if (api is IWithExtraCodeForbiddenIn<T> withExtraCodeForbiddenIn)
            {
                return WithExtraCodeExtension.WithExtraCode<T>(withExtraCodeForbiddenIn, code);
            }

            throw new InvalidOperationException("Dynamic api tests failed");
        }

        private static dynamic WithErrorCleared<T>(dynamic api)
        {
            if (api is IWithErrorClearedIn<T> withoutMessageIn)
            {
                return WithErrorClearedExtension.WithErrorCleared<T>(withoutMessageIn);
            }

            if (api is IWithErrorClearedForbiddenIn<T> withCodeForbidden)
            {
                return WithErrorClearedExtension.WithErrorCleared<T>(withCodeForbidden);
            }

            throw new InvalidOperationException("Dynamic api tests failed");
        }
    }
}
