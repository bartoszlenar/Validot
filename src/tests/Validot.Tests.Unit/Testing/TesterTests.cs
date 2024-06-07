namespace Validot.Tests.Unit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Validot.Errors;
    using Validot.Testing;

    using Xunit;

    public class TesterTests
    {
        public class TestExceptionOnInit
        {
            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                Action action = () => Tester.TestExceptionOnInit((Specification<object>)null, typeof(InvalidOperationException));

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullType()
            {
                var specification = new Specification<object>(m => m);

                Action action = () => Tester.TestExceptionOnInit(specification, null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_RethrowExceptionThrownInSpecification()
            {
                var exception = new InvalidOperationException();

                var specification = new Specification<object>(m => throw exception);

                Exception result = Tester.TestExceptionOnInit(specification, typeof(InvalidOperationException));

                result.Should().BeSameAs(exception);
            }

            [Fact]
            public void Should_RethrowExceptionThrownInSpecification_When_ExpectedExceptionTypeDerivesFromThrown()
            {
                var exception = new InvalidOperationException();

                var specification = new Specification<object>(m => throw exception);

                Exception result = Tester.TestExceptionOnInit(specification, typeof(Exception));

                result.Should().BeSameAs(exception);
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_DifferentTypeOfExceptionIsThrownFromSpecification()
            {
                var exception = new InvalidOperationException();

                var specification = new Specification<object>(m => throw exception);

                Action action = () => Tester.TestExceptionOnInit(specification, typeof(ArgumentNullException));

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Exception of type {typeof(ArgumentNullException).FullName} was expected, but found {typeof(InvalidOperationException).FullName}.");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_NoExceptionThrownFromSpecification()
            {
                var specification = new Specification<object>(m => m);

                Action action = () => Tester.TestExceptionOnInit(specification, typeof(InvalidOperationException));

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Exception of type {typeof(InvalidOperationException).FullName} was expected, but no exception has been thrown.");
            }
        }

        public class TestSpecification
        {
            [Fact]
            public void Should_ExecuteValidationOnObject_When_ObjectIsReferenceType()
            {
                var model = new object();
                var tested = false;
                object testedModel = null;

                Tester.TestSpecification(
                    model,
                    s => s.Rule(m =>
                    {
                        tested = true;
                        testedModel = m;

                        return true;
                    }));

                tested.Should().BeTrue();
                testedModel.Should().BeSameAs(model);
            }

            [Fact]
            public void Should_ExecuteValidationOnObject_When_ObjectIsValueType()
            {
                Guid model = Guid.NewGuid();
                var tested = false;
                Guid testedModel = Guid.Empty;

                Tester.TestSpecification(
                    model,
                    s => s.Rule(m =>
                    {
                        tested = true;
                        testedModel = m;

                        return true;
                    }));

                tested.Should().BeTrue();
                testedModel.Should().Be(model);
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Arg_InvalidName()
            {
                Specification<object> specification = s => s
                    .RuleTemplate(r => false, "key1", Arg.Text("arg1", "argValue1")).WithPath("member1")
                    .RuleTemplate(r => false, "key21", Arg.Text("arg21", "argValue21")).WithPath("member2")
                    .RuleTemplate(r => false, "key22", Arg.Text("arg221", "argValue221"), Arg.Text("arg22x", "argValue222")).WithPath("member2")
                    .RuleTemplate(r => false, "key23", Arg.Text("arg23", "argValue23")).WithPath("member2")
                    .RuleTemplate(r => false, "key3", Arg.Text("arg3", "argValue3")).WithPath("member3");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key1",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg1", "argValue1"),
                                },
                            },
                        },
                        ["member2"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key21",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg21", "argValue21"),
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key22",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg221", "argValue221"),
                                    Arg.Text("arg222", "argValue222"),
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key23",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg23", "argValue23"),
                                },
                            },
                        },
                        ["member3"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key3",
                                },
                                Codes = new[]
                                {
                                    "code3",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member2`, index 1) arg is missing: `arg222`");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Arg_InvalidType()
            {
                Specification<object> specification = s => s
                    .RuleTemplate(r => false, "key1", Arg.Text("arg1", "argValue1")).WithPath("member1")
                    .RuleTemplate(r => false, "key21", Arg.Text("arg21", "argValue21")).WithPath("member2")
                    .RuleTemplate(r => false, "key22", Arg.Text("arg221", "argValue221"), Arg.Text("arg222", "argValue222")).WithPath("member2")
                    .RuleTemplate(r => false, "key23", Arg.Text("arg23", "argValue23")).WithPath("member2")
                    .RuleTemplate(r => false, "key3", Arg.Text("arg3", "argValue3")).WithPath("member3");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key1",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg1", "argValue1"),
                                },
                            },
                        },
                        ["member2"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key21",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg21", "argValue21"),
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key22",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg221", "argValue221"),
                                    Arg.Number("arg222", 222),
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key23",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg23", "argValue23"),
                                },
                            },
                        },
                        ["member3"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key3",
                                },
                                Codes = new[]
                                {
                                    "code3",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member2`, index 1) arg (name `arg222`) type to be `Validot.Errors.Args.NumberArg<System.Int32>`, but found `Validot.Errors.Args.TextArg`");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Arg_InvalidValue()
            {
                Specification<object> specification = s => s
                    .RuleTemplate(r => false, "key1", Arg.Text("arg1", "argValue1")).WithPath("member1")
                    .RuleTemplate(r => false, "key21", Arg.Text("arg21", "argValue21")).WithPath("member2")
                    .RuleTemplate(r => false, "key22", Arg.Text("arg221", "argValue221"), Arg.Text("arg222", "argValue22x")).WithPath("member2")
                    .RuleTemplate(r => false, "key23", Arg.Text("arg23", "argValue23")).WithPath("member2")
                    .RuleTemplate(r => false, "key3", Arg.Text("arg3", "argValue3")).WithPath("member3");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key1",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg1", "argValue1"),
                                },
                            },
                        },
                        ["member2"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key21",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg21", "argValue21"),
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key22",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg221", "argValue221"),
                                    Arg.Text("arg222", "argValue222"),
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key23",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg23", "argValue23"),
                                },
                            },
                        },
                        ["member3"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key3",
                                },
                                Codes = new[]
                                {
                                    "code3",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member2`, index 1) arg (name `arg222`) value to be `argValue222`, but found `argValue22x`");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Arg_InvalidValue_Double()
            {
                Specification<object> specification = s => s
                    .RuleTemplate(r => false, "key", Arg.Number("arg", 123.123457D)).WithPath("member");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key",
                                },
                                Args = new[]
                                {
                                    Arg.Number("arg", 123.123456D),
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member`, index 0) arg (name `arg`) double value to be `123.123456`, but found `123.123457`");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Arg_InvalidValue_Float()
            {
                Specification<object> specification = s => s
                    .RuleTemplate(r => false, "key", Arg.Number("arg", 123.1235F)).WithPath("member");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key",
                                },
                                Args = new[]
                                {
                                    Arg.Number("arg", 123.1234F),
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member`, index 0) arg (name `arg`) float value to be `123.1234`, but found `123.1235`");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Args_InvalidAmount()
            {
                Specification<object> specification = s => s
                        .RuleTemplate(r => false, "key1", Arg.Text("arg1", "argValue1")).WithPath("member1")
                        .RuleTemplate(r => false, "key21", Arg.Text("arg21", "argValue21")).WithPath("member2")
                        .RuleTemplate(r => false, "key22", Arg.Text("arg22", "argValue22")).WithPath("member2")
                        .RuleTemplate(r => false, "key23", Arg.Text("arg23", "argValue23")).WithPath("member2")
                        .RuleTemplate(r => false, "key3", Arg.Text("arg3", "argValue3")).WithPath("member3")
                    ;

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key1",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg1", "argValue1"),
                                },
                            },
                        },
                        ["member2"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key21",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg21", "argValue21"),
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key22",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg221", "argValue221"),
                                    Arg.Text("arg222", "argValue222"),
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key23",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg23", "argValue23"),
                                },
                            },
                        },
                        ["member3"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "key3",
                                },
                                Codes = new[]
                                {
                                    "code3",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member2`, index 1) args amount to be 2, but found 1");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Args_Presence_ExistButNotExpected()
            {
                Specification<object> specification = s => s
                    .RuleTemplate(m => false, "message1", Arg.Text("arg", "argValue")).WithPath("member1");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "message1",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member1`, index 0) args amount to be 0, but found 1");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Args_Presence_NotExistButExpected()
            {
                Specification<object> specification = s => s
                    .RuleTemplate(m => false, "message1").WithPath("member1");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "message1",
                                },
                                Args = new[]
                                {
                                    Arg.GuidValue("arg", Guid.Empty),
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member1`, index 0) args amount to be 1, but found 0");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Code_InvalidContent()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithExtraCode("code1")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraCode("code211")
                    .WithExtraCode("code212")
                    .WithExtraCode("code213")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraCode("code221")
                    .WithExtraCode("code22x")
                    .WithExtraCode("code223")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraCode("code231")
                    .WithExtraCode("code232")
                    .WithExtraCode("code233")
                    .Rule(m => false).WithPath("member3").WithExtraCode("code3");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Codes = new[]
                                {
                                    "code1",
                                },
                            },
                        },
                        ["member2"] = new[]
                        {
                            new Error
                            {
                                Codes = new[]
                                {
                                    "code211",
                                    "code212",
                                    "code213",
                                },
                            },
                            new Error
                            {
                                Codes = new[]
                                {
                                    "code221",
                                    "code222",
                                    "code223",
                                },
                            },
                            new Error
                            {
                                Codes = new[]
                                {
                                    "code231",
                                    "code232",
                                    "code233",
                                },
                            },
                        },
                        ["member3"] = new[]
                        {
                            new Error
                            {
                                Codes = new[]
                                {
                                    "code3",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member2`, index 1) code (index 1) to be `code222`, but found `code22x`");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Codes_InvalidAmount()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithExtraCode("code1")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraCode("code211")
                    .WithExtraCode("code212")
                    .WithExtraCode("code213")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraCode("code221")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraCode("code231")
                    .WithExtraCode("code232")
                    .WithExtraCode("code233")
                    .Rule(m => false).WithPath("member3").WithExtraCode("code3");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Codes = new[]
                                {
                                    "code1",
                                },
                            },
                        },
                        ["member2"] = new[]
                        {
                            new Error
                            {
                                Codes = new[]
                                {
                                    "code211",
                                    "code212",
                                    "code213",
                                },
                            },
                            new Error
                            {
                                Codes = new[]
                                {
                                    "code221",
                                    "code222",
                                    "code223",
                                },
                            },
                            new Error
                            {
                                Codes = new[]
                                {
                                    "code231",
                                    "code232",
                                    "code233",
                                },
                            },
                        },
                        ["member3"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "code3",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member2`, index 1) codes amount to be 3, but found 1");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Codes_Presence_ExistButNotExpected()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithMessage("message").WithExtraCode("code1").WithExtraCode("code2");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "message",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member1`, index 0) codes amount to be 0, but found 2");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Codes_Presence_NotExistButExpected()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithMessage("message1");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "message1",
                                },
                                Codes = new[]
                                {
                                    "code1",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member1`, index 0) codes amount to be 1, but found 0");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Message_InvalidContent()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithMessage("error1")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraMessage("error211")
                    .WithExtraMessage("error212")
                    .WithExtraMessage("error213")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraMessage("error221")
                    .WithExtraMessage("error22x")
                    .WithExtraMessage("error223")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraMessage("error231")
                    .WithExtraMessage("error232")
                    .WithExtraMessage("error233")
                    .Rule(m => false).WithPath("member3").WithMessage("error3");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error1",
                                },
                            },
                        },
                        ["member2"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error211",
                                    "error212",
                                    "error213",
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error221",
                                    "error222",
                                    "error223",
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error231",
                                    "error232",
                                    "error233",
                                },
                            },
                        },
                        ["member3"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error3",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member2`, index 1) message (index 1) to be `error222`, but found `error22x`");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Messages_InvalidAmount()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithMessage("error1")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraMessage("error211")
                    .WithExtraMessage("error212")
                    .WithExtraMessage("error213")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraMessage("error221")
                    .Rule(m => false).WithPath("member2")
                    .WithExtraMessage("error231")
                    .WithExtraMessage("error232")
                    .WithExtraMessage("error233")
                    .Rule(m => false).WithPath("member3").WithMessage("error3");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error1",
                                },
                            },
                        },
                        ["member2"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error211",
                                    "error212",
                                    "error213",
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error221",
                                    "error222",
                                    "error223",
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error231",
                                    "error232",
                                    "error233",
                                },
                            },
                        },
                        ["member3"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error3",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member2`, index 1) messages amount to be 3, but found 1");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Messages_Presence_ExistButNotExpected()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithMessage("error1").WithExtraMessage("extra1");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Codes = new[]
                                {
                                    "code1",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member1`, index 0) messages amount to be 0, but found 2");
            }

            [Fact]
            public void Should_ReturnFailure_When_Error_Messages_Presence_NotExistButExpected()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithCode("code1");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "message1",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error (for path `member1`, index 0) messages amount to be 1, but found 0");
            }

            [Fact]
            public void Should_ReturnFailure_When_Model_IsInvalid_And_NoErrorsAreExpected()
            {
                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    s => s.Rule(m => false));

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected result IsValid: True, but AnyErrors: True");
            }

            [Fact]
            public void Should_ReturnFailure_When_Model_IsValid_And_ErrorsAreExpected()
            {
                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    s => s.Rule(m => true),
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member"] = new[]
                        {
                            new Error(),
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected result IsValid: False, but AnyErrors: False");
            }

            [Fact]
            public void Should_ReturnFailure_When_Path_ExpectedPathIsMissing()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithMessage("error1")
                    .Rule(m => false).WithPath("member_two").WithMessage("error2")
                    .Rule(m => false).WithPath("member3").WithMessage("error3");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error(),
                        },
                        ["member2"] = new[]
                        {
                            new Error(),
                        },
                        ["member3"] = new[]
                        {
                            new Error(),
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected error path is missing: `member2`");
            }

            [Fact]
            public void Should_ReturnFailure_When_PathErrors_Amount_DifferentThanExpected()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithMessage("error1")
                    .Rule(m => false).WithPath("member2").WithMessage("error2")
                    .Rule(m => false).WithPath("member3").WithMessage("error3");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error1",
                                },
                            },
                        },
                        ["member2"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error2",
                                },
                            },
                        },
                        ["member3"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error3",
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error31",
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected errors amount (for path `member3`): 2, but found 1");
            }

            [Fact]
            public void Should_ReturnFailure_When_Paths_Amount_DifferentThanExpected()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1").WithMessage("error1")
                    .Rule(m => false).WithPath("member2").WithMessage("error2");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error(),
                        },
                        ["member2"] = new[]
                        {
                            new Error(),
                        },
                        ["member3"] = new[]
                        {
                            new Error(),
                        },
                    });

                testResult.Success.Should().BeFalse();
                testResult.Message.Should().Be("Expected amount of paths with errors: 3, but found: 2");
            }

            [Fact]
            public void Should_ReturnSuccess_When_ModelIsValid_And_NoErrorsAreExpected()
            {
                TestResult testResult = Tester.TestSpecification(new object(), s => s.Rule(m => true));

                testResult.Success.Should().BeTrue();
            }

            [Fact]
            public void Should_ReturnSuccess_When_AllAsExpected()
            {
                Specification<object> specification = s => s
                    .Rule(m => false).WithPath("member1")
                    .WithMessage("error1")
                    .WithExtraMessage("error1_1")
                    .WithExtraCode("code1_1")
                    .RuleTemplate(m => false, "error21", Arg.Number("arg21", 12345.6789012345M)).WithPath("member2")
                    .RuleTemplate(m => false, "error22", Arg.Text("arg221", "awesome"), Arg.Enum("arg222", StringComparison.InvariantCultureIgnoreCase)).WithPath("member2")
                    .RuleTemplate(m => false, "error3", Arg.Type("arg3", typeof(Guid))).WithPath("member3").WithExtraMessage("extramessage3").WithExtraCode("extracode3");

                TestResult testResult = Tester.TestSpecification(
                    new object(),
                    specification,
                    new Dictionary<string, IReadOnlyCollection<IError>>
                    {
                        ["member1"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error1",
                                    "error1_1",
                                },
                                Codes = new[]
                                {
                                    "code1_1",
                                },
                            },
                        },
                        ["member2"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error21",
                                },
                                Args = new[]
                                {
                                    Arg.Number("arg21", 12345.6789012345M),
                                },
                            },
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error22",
                                },
                                Args = new[]
                                {
                                    Arg.Text("arg221", "awesome"),
                                    Arg.Enum("arg222", StringComparison.InvariantCultureIgnoreCase),
                                },
                            },
                        },
                        ["member3"] = new[]
                        {
                            new Error
                            {
                                Messages = new[]
                                {
                                    "error3",
                                    "extramessage3",
                                },
                                Codes = new[]
                                {
                                    "extracode3",
                                },
                                Args = new[]
                                {
                                    Arg.Type("arg3", typeof(Guid)),
                                },
                            },
                        },
                    });

                testResult.Success.Should().BeTrue();
            }
        }

        public class TestSingleRule
        {
            [Fact]
            public void Should_Pass_When_AsExpected_ModelIsValid()
            {
                var specification = new Specification<object>(m => m.Rule(r => true));

                Tester.TestSingleRule(new object(), specification, true);
            }

            [Fact]
            public void Should_Pass_When_AsExpected_ModelIsInvalid_WithMessage()
            {
                var specification = new Specification<object>(m => m.Rule(r => false).WithMessage("message"));

                Tester.TestSingleRule(new object(), specification, false, "message");
            }

            [Fact]
            public void Should_Pass_When_AsExpected_ModelIsInvalid_WithMessage_And_Args()
            {
                var specification = new Specification<object>(m => m.RuleTemplate(r => false, "message", Arg.Number("arg1", 1), Arg.Text("arg2", "argValue2")));

                Tester.TestSingleRule(new object(), specification, false, "message", Arg.Number("arg1", 1), Arg.Text("arg2", "argValue2"));
            }

            [Fact]
            public void Should_ThrowException_When_ArgsWithoutMessage()
            {
                var specification = new Specification<object>(m => m);

                Action action = () => Tester.TestSingleRule(new object(), specification, true, null, Arg.Number("arg", 1));

                action.Should().ThrowExactly<ArgumentException>();
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Errors_Expected_And_NotExist()
            {
                var specification = new Specification<object>(m =>
                    m.Rule(r => true).WithMessage("message"));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message");

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Expected result IsValid: False, but AnyErrors: False");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Errors_NotExpected_And_Exist()
            {
                var specification = new Specification<object>(m =>
                    m.Rule(r => false).WithMessage("message"));

                Action action = () => Tester.TestSingleRule(new object(), specification, true);

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Expected result IsValid: True, but AnyErrors: True");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Errors_MultipleErrors_UnderOnePath()
            {
                var specification = new Specification<object>(m => m
                    .Rule(r => false).WithMessage("message1")
                    .Rule(r => false).WithMessage("message2"));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message1");

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Expected errors amount (for path ``): 1, but found 2");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Errors_MultipleErrors_UnderDifferentPaths()
            {
                var specification = new Specification<object>(m => m
                    .Rule(r => false).WithMessage("message1")
                    .Rule(r => false).WithPath("member1").WithMessage("message2"));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message1");

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Expected amount of paths with errors: 1, but found: 2");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Error_NotInRoot()
            {
                var specification = new Specification<object>(m => m.Rule(r => false).WithPath("member1").WithMessage("message1"));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message1");

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Expected error path is missing: ``");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_MultipleMessages()
            {
                var specification = new Specification<object>(m =>
                    m.Rule(r => false).WithMessage("message1").WithExtraMessage("message2"));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message1");

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Expected error (for path ``, index 0) messages amount to be 1, but found 2");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_AnyCode()
            {
                var specification = new Specification<object>(m =>
                    m.Rule(r => false).WithMessage("message1").WithExtraCode("message2"));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message1");

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Expected error (for path ``, index 0) codes amount to be 0, but found 1");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Error_HasArgs_And_ArgsNotExpected()
            {
                var specification = new Specification<object>(m =>
                    m.RuleTemplate(r => false, "message", Arg.Text("arg", "argValue")));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message");

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Expected error (for path ``, index 0) args amount to be 0, but found 1");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Error_HasNoArgs_And_ArgsAreExpected()
            {
                var specification = new Specification<object>(m =>
                    m.RuleTemplate(r => false, "message"));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message", Arg.Text("arg", "argValue"));

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Expected error (for path ``, index 0) args amount to be 1, but found 0");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Error_Args_InvalidAmount()
            {
                var specification = new Specification<object>(m =>
                    m.RuleTemplate(r => false, "message", Arg.Text("arg1", "argValue1"), Arg.Text("arg2", "argValue2")));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message", Arg.Text("arg", "argValue"));

                action.Should().ThrowExactly<TestFailedException>().WithMessage($"Expected error (for path ``, index 0) args amount to be 1, but found 2");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Error_Args_InvalidType()
            {
                var specification = new Specification<object>(m =>
                    m.RuleTemplate(r => false, "message", Arg.Text("arg1", "argValue1")));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message", Arg.Number("arg1", 1));

                action.Should().ThrowExactly<TestFailedException>().WithMessage("Expected error (for path ``, index 0) arg (name `arg1`) type to be `Validot.Errors.Args.NumberArg<System.Int32>`, but found `Validot.Errors.Args.TextArg`");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Error_Args_InvalidName()
            {
                var specification = new Specification<object>(m =>
                    m.RuleTemplate(r => false, "message", Arg.Text("arg2", "argValue1")));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message", Arg.Number("arg1", 1));

                action.Should().ThrowExactly<TestFailedException>().WithMessage("Expected error (for path ``, index 0) arg is missing: `arg1`");
            }

            [Fact]
            public void Should_ThrowTestFailedException_When_Error_Args_InvalidValue()
            {
                var specification = new Specification<object>(m =>
                    m.RuleTemplate(r => false, "message", Arg.Text("arg1", "argValue1"), Arg.Text("arg2", "argValue2X")));

                Action action = () => Tester.TestSingleRule(new object(), specification, false, "message", Arg.Text("arg1", "argValue1"), Arg.Text("arg2", "argValue2"));

                action.Should().ThrowExactly<TestFailedException>().WithMessage("Expected error (for path ``, index 0) arg (name `arg2`) value to be `argValue2`, but found `argValue2X`");
            }
        }

        public class TestResultToString
        {
            [Fact]
            public void Should_ThrowException_When_NullString()
            {
                Action action = () =>
                {
                    Tester.TestResultToString(null, ToStringContentType.Messages, "asd");
                };

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_NullExpectedLines()
            {
                Action action = () =>
                {
                    Tester.TestResultToString("abc", ToStringContentType.Messages, null);
                };

                action.Should().ThrowExactly<ArgumentNullException>();
            }

            [Fact]
            public void Should_ThrowException_When_EmptyExpectedLines()
            {
                Action action = () =>
                {
                    Tester.TestResultToString("abc", ToStringContentType.Messages, new string[] { });
                };

                action.Should().ThrowExactly<ArgumentException>().And.Message.Should().StartWith("Empty list of expected lines");
            }

            [Theory]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(10)]
            public void Should_ThrowException_When_CodesExpected_And_MoreThanOneLine(int lines)
            {
                Action action = () =>
                {
                    Tester.TestResultToString(
                        "abc",
                        ToStringContentType.Codes,
                        Enumerable.Range(0, lines).Select(i => $"{i}").ToArray()
                    );
                };

                action.Should().ThrowExactly<ArgumentException>().And.Message.Should().StartWith($"Expected codes only (all in the single line), but found lines: {lines}");
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            public void Should_ThrowException_When_MessagesAndCodesExpected_And_LessThanThreeLines(int lines)
            {
                Action action = () =>
                {
                    Tester.TestResultToString(
                        "abc",
                        ToStringContentType.MessagesAndCodes,
                        Enumerable.Range(0, lines).Select(i => $"{i}").ToArray()
                    );
                };

                action.Should().ThrowExactly<ArgumentException>().And.Message.Should().StartWith($"Expected codes and messages (so at least 3 lines), but found lines: {lines}");
            }

            [Theory]
            [InlineData(3)]
            [InlineData(5)]
            [InlineData(10)]
            public void Should_ThrowException_When_MessagesAndCodesExpected_And_SecondLineNotEmpty(int lines)
            {
                var expectedLines = Enumerable.Range(0, lines).Select(i => $"{i}").ToArray();

                Action action = () =>
                {
                    Tester.TestResultToString(
                        "abc",
                        ToStringContentType.MessagesAndCodes,
                        expectedLines
                    );
                };

                action.Should().ThrowExactly<ArgumentException>().And.Message.Should().StartWith($"Expected codes and messages (divided by a single empty line), but found in second line: {expectedLines[1]}");
            }

            [Theory]
            [InlineData(3)]
            [InlineData(5)]
            [InlineData(10)]
            public void Should_ThrowException_When_MessagesAndCodesExpected_And_ExtraEmptyLine(int lines)
            {
                var expectedLines = Enumerable.Range(0, lines).Select(i => $"{i}").ToArray();
                expectedLines[1] = "";
                expectedLines[lines - 1] = "";

                Action action = () =>
                {
                    Tester.TestResultToString(
                        "abc",
                        ToStringContentType.MessagesAndCodes,
                        expectedLines);
                };

                action.Should().ThrowExactly<ArgumentException>().And.Message.Should().StartWith("Expected codes and messages (divided by a single empty line), also another empty line");
            }

            [Theory]
            [InlineData(1)]
            [InlineData(3)]
            [InlineData(5)]
            [InlineData(10)]
            public void Should_ThrowException_When_MessagesExpected_And_EmptyLine(int lines)
            {
                var expectedLines = Enumerable.Range(0, lines).Select(i => $"{i}").ToArray();
                expectedLines[lines - 1] = "";

                Action action = () =>
                {
                    Tester.TestResultToString(
                        "abc",
                        ToStringContentType.Messages,
                        expectedLines);
                };

                action.Should().ThrowExactly<ArgumentException>().And.Message.Should().StartWith($"Expected messages only, but found empty line");
            }

            [Theory]
            [InlineData(1, 3, ToStringContentType.Messages)]
            [InlineData(5, 1, ToStringContentType.Messages)]
            [InlineData(5, 3, ToStringContentType.MessagesAndCodes)]
            [InlineData(8, 9, ToStringContentType.MessagesAndCodes)]
            public void Should_Fail_When_DifferentLineAmount(int linesCount, int expectedLinesCount, ToStringContentType toStringContentType)
            {
                var lines = Enumerable.Range(0, linesCount).Select(i => $"{i}").ToArray();
                var expectedLines = Enumerable.Range(0, expectedLinesCount).Select(i => $"{i}").ToArray();

                if (toStringContentType == ToStringContentType.MessagesAndCodes)
                {
                    expectedLines[1] = "";
                    lines[1] = "";
                }

                var input = string.Join(Environment.NewLine, lines);

                var result = Tester.TestResultToString(input, toStringContentType, expectedLines);

                result.Success.Should().BeFalse();
                result.Message.Should().Be($"Expected amount of lines: {expectedLinesCount}, but found: {lines.Length}");
            }

            public static IEnumerable<object[]> Should_Fail_When_MissingCodes_Data()
            {
                foreach (var expectedStringContent in new[] { ToStringContentType.Codes, ToStringContentType.MessagesAndCodes })
                {
                    yield return new object[]
                    {
                        "a, b, c, d",
                        "a, b, c, d, e, f, g",
                        "e, f, g",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        "a, b, c, d, e",
                        "a, b, c, d, e, f, g",
                        "f, g",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        "a, b, c, d, e, f",
                        "a, b, c, d, e, f, g",
                        "g",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        "a, c, e, f",
                        "a, b, c, d, e, f, g",
                        "b, d, g",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        "f",
                        "a, b, c, d, e, f, g",
                        "a, b, c, d, e, g",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        "e, c, a",
                        "a, b, c, d, e, f, g",
                        "b, d, f, g",
                        expectedStringContent
                    };
                }
            }

            [Theory]
            [MemberData(nameof(Should_Fail_When_MissingCodes_Data))]
            public void Should_Fail_When_MissingCodes(string codesString, string expectedCodesString, string missingCodesString, ToStringContentType toStringContentType)
            {
                if (toStringContentType == ToStringContentType.MessagesAndCodes)
                {
                    codesString += string.Join(Environment.NewLine, new[] { Environment.NewLine, "m1", "m2", "m3" });
                    expectedCodesString += string.Join(Environment.NewLine, new[] { Environment.NewLine, "m1", "m2", "m3" });
                }

                var result = Tester.TestResultToString(codesString, toStringContentType, expectedCodesString.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

                result.Success.Should().BeFalse();
                result.Message.Should().Be($"Expected codes that are missing: {missingCodesString}");
            }

            public static IEnumerable<object[]> Should_Fail_When_InvalidAmountOfCodes_Data()
            {
                foreach (var expectedStringContent in new[] { ToStringContentType.Codes, ToStringContentType.MessagesAndCodes })
                {
                    yield return new object[]
                    {
                        "a, b, c, d, e, f, g, A, B",
                        "a, b, c, d, e, f, g",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        "a, b, c, d, e, f, g",
                        "a, b, c, d, e, f",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        "a, b, c, d, e, f, g",
                        "a, c, e, f",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        "a, b, c, d, e, f, g",
                        "a, b, c, d, e, g",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        "a, b, c, d, e, f, g, g, a",
                        "a, b, c, d, e, f, g",
                        expectedStringContent
                    };
                }
            }

            [Theory]
            [MemberData(nameof(Should_Fail_When_InvalidAmountOfCodes_Data))]
            public void Should_Fail_When_InvalidAmountOfCodes(string codesString, string expectedCodesString, ToStringContentType toStringContentType)
            {
                var codeAmount = codesString.Split(new[] { ", " }, StringSplitOptions.None).Length;
                var expectedCodeAmount = expectedCodesString.Split(new[] { ", " }, StringSplitOptions.None).Length;

                if (toStringContentType == ToStringContentType.MessagesAndCodes)
                {
                    codesString += string.Join(Environment.NewLine, new[] { Environment.NewLine, "m1", "m2", "m3" });
                    expectedCodesString += string.Join(Environment.NewLine, new[] { Environment.NewLine, "m1", "m2", "m3" });
                }

                var result = Tester.TestResultToString(codesString, toStringContentType, expectedCodesString.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

                result.Success.Should().BeFalse();
                result.Message.Should().Be($"Expected amount of codes: {expectedCodeAmount}, but found: {codeAmount}");
            }

            [Theory]
            [InlineData(1)]
            [InlineData(5)]
            [InlineData(10)]
            public void Should_Fail_When_ExpectingMessagesAndCodes_And_SecondLineIsNotEmpty(int linesCount)
            {
                var lines = Enumerable.Range(0, linesCount + 2).Select(i => $"{i}").ToArray();
                lines[0] = "a, b, c";
                lines[1] = "_not_empty_";

                var expectedLines = Enumerable.Range(0, linesCount + 2).Select(i => $"{i}").ToArray();
                expectedLines[0] = "a, b, c";
                expectedLines[1] = string.Empty;

                var input = string.Join(Environment.NewLine, lines);

                var result = Tester.TestResultToString(input, ToStringContentType.MessagesAndCodes, expectedLines);

                result.Success.Should().BeFalse();
                result.Message.Should().Be($"Expected codes and messages (divided by a single line), but found in second line: _not_empty_");
            }

            public static IEnumerable<object[]> Should_Fail_When_MissingMessages_Data()
            {
                foreach (var expectedStringContent in new[] { ToStringContentType.Messages, ToStringContentType.MessagesAndCodes })
                {
                    yield return new object[]
                    {
                        new[] { "a", "b", "c", "X" },
                        new[] { "a", "b", "c", "e" },
                        "`e`",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        new[] { "c", "a", "X", "b" },
                        new[] { "a", "b", "c", "e" },
                        "`e`",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        new[] { "c", "X", "a", "b", },
                        new[] { "a", "b", "c", "e" },
                        "`e`",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        new[] { "a", "b", "c", "X", "Y" },
                        new[] { "a", "b", "c", "e", "f" },
                        "`e`, `f`",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        new[] { "X", "c", "a", "Y", "b" },
                        new[] { "a", "b", "c", "e", "f" },
                        "`e`, `f`",
                        expectedStringContent
                    };

                    yield return new object[]
                    {
                        new[] { "a", "X", "Y", "X", "Y" },
                        new[] { "a", "b", "c", "e", "f" },
                        "`b`, `c`, `e`, `f`",
                        expectedStringContent
                    };
                }
            }

            [Theory]
            [MemberData(nameof(Should_Fail_When_MissingMessages_Data))]
            public void Should_Fail_When_MissingMessages(string[] messages, string[] expectedMessages, string missingMessages, ToStringContentType toStringContentType)
            {
                if (toStringContentType == ToStringContentType.MessagesAndCodes)
                {
                    var codesLines = new[]
                    {
                        "a, b, c",
                        ""
                    };

                    messages = codesLines.Concat(messages).ToArray();
                    expectedMessages = codesLines.Concat(expectedMessages).ToArray();
                }

                var input = string.Join(Environment.NewLine, messages);

                var result = Tester.TestResultToString(input, toStringContentType, expectedMessages);

                result.Success.Should().BeFalse();
                result.Message.Should().Be($"Expected messages that are missing: {missingMessages}");
            }

            public static IEnumerable<object[]> Should_Succeed_Data()
            {
                yield return new object[]
                {
                    new[] { "a, b, c, d, e", "", "M1", "M2", "M3" },
                    new[] { "a, b, c, d, e", "", "M1", "M2", "M3" },
                    ToStringContentType.MessagesAndCodes
                };

                yield return new object[]
                {
                    new[] { "e, d, c, b, a", "", "M3", "M2", "M1" },
                    new[] { "a, b, c, d, e", "", "M1", "M2", "M3" },
                    ToStringContentType.MessagesAndCodes
                };

                yield return new object[]
                {
                    new[] { "c, d, e, a, b", "", "M1" },
                    new[] { "a, b, c, d, e", "", "M1" },
                    ToStringContentType.MessagesAndCodes
                };

                yield return new object[]
                {
                    new[] { "M1", "M2", "M3" },
                    new[] { "M1", "M2", "M3" },
                    ToStringContentType.Messages
                };

                yield return new object[]
                {
                    new[] { "M3", "M2", "M1" },
                    new[] { "M1", "M2", "M3" },
                    ToStringContentType.Messages
                };

                yield return new object[]
                {
                    new[] { "c, d, e, a, b", },
                    new[] { "a, b, c, d, e" },
                    ToStringContentType.Codes
                };

                yield return new object[]
                {
                    new[] { "a, b, c, d, e" },
                    new[] { "a, b, c, d, e" },
                    ToStringContentType.Codes
                };
            }

            [Theory]
            [MemberData(nameof(Should_Succeed_Data))]
            public void Should_Succeed(string[] messages, string[] expectedMessages, ToStringContentType toStringContentType)
            {
                var input = string.Join(Environment.NewLine, messages);

                var result = Tester.TestResultToString(input, toStringContentType, expectedMessages);

                result.Success.Should().BeTrue();
                result.Message.Should().BeNullOrEmpty();
            }
        }

        public class ShouldResultToStringHaveLines
        {
            [Fact]
            public void Should_ThrowException_When_InvalidCodes()
            {
                Action action = () =>
                {
                    "a, b, c, d".ShouldResultToStringHaveLines(
                        ToStringContentType.Codes,
                        "b, c, a, d, e"
                        );
                };

                action.Should().ThrowExactly<TestFailedException>().And.Message.Should().Be("Expected codes that are missing: e");
            }

            [Fact]
            public void Should_ThrowException_When_InvalidCodes_WithMessages()
            {
                var messages = new[]
                {
                    "a, b, c, d",
                    "",
                    "A",
                    "B",
                    "C",
                    "D"
                };

                Action action = () =>
                {
                    string.Join(Environment.NewLine, messages).ShouldResultToStringHaveLines(
                        ToStringContentType.MessagesAndCodes,
                        "a, d, b, c, e",
                        "",
                        "C",
                        "B",
                        "A",
                        "D"
                    );
                };

                action.Should().ThrowExactly<TestFailedException>().And.Message.Should().Be("Expected codes that are missing: e");
            }

            [Fact]
            public void Should_ThrowException_When_InvalidMessages()
            {
                var messages = new[]
                {
                    "A",
                    "B",
                    "C",
                    "X"
                };

                Action action = () =>
                {
                    string.Join(Environment.NewLine, messages).ShouldResultToStringHaveLines(
                        ToStringContentType.Messages,
                        "C",
                        "B",
                        "A",
                        "D"
                    );
                };

                action.Should().ThrowExactly<TestFailedException>().And.Message.Should().Be("Expected messages that are missing: `D`");
            }

            [Fact]
            public void Should_ThrowException_When_InvalidMessages_WithCodes()
            {
                var messages = new[]
                {
                    "a, b, c, d, e",
                    "",
                    "A",
                    "B",
                    "C",
                    "X"
                };

                Action action = () =>
                {
                    string.Join(Environment.NewLine, messages).ShouldResultToStringHaveLines(
                        ToStringContentType.MessagesAndCodes,
                        "a, b, c, d, e",
                        "",
                        "C",
                        "B",
                        "A",
                        "D"
                    );
                };

                action.Should().ThrowExactly<TestFailedException>().And.Message.Should().Be("Expected messages that are missing: `D`");
            }

            [Fact]
            public void Should_NotThrowException_When_AllGood()
            {
                var messages = new[]
                {
                    "a, b, c, d, e",
                    "",
                    "A",
                    "B",
                    "C",
                    "D"
                };

                Action action = () =>
                {
                    string.Join(Environment.NewLine, messages).ShouldResultToStringHaveLines(
                        ToStringContentType.MessagesAndCodes,
                        "a, b, c, d, e",
                        "",
                        "A",
                        "B",
                        "C",
                        "D"
                    );
                };

                action.Should().NotThrow();
            }
        }
    }
}
