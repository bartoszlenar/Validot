namespace Validot.Tests.Unit.Errors.Args
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Validot.Errors.Args;

    using Xunit;

    public class ArgsServiceTests
    {
        public class FormatMessage
        {
            [Fact]
            public void Should_ReturnEmptyString_When_MessageIsNull()
            {
                var formattedMessage = ArgsHelper.FormatMessage(null, Array.Empty<ArgPlaceholder>(), Array.Empty<IArg>());
                formattedMessage.Should().Be(string.Empty);
            }

            [Fact]
            public void Should_ReturnMessage_When_NullPlaceholders()
            {
                var formattedMessage = ArgsHelper.FormatMessage("test {test}", null, new[] { Arg.Text("test", "XXX") });

                formattedMessage.Should().Be("test {test}");
            }

            [Fact]
            public void Should_ReturnMessage_When_NoPlaceholders()
            {
                var formattedMessage = ArgsHelper.FormatMessage("test {test}", Array.Empty<ArgPlaceholder>(), new[] { Arg.Text("test", "XXX") });

                formattedMessage.Should().Be("test {test}");
            }

            [Fact]
            public void Should_ReturnMessage_When_NoArgs()
            {
                var formattedMessage = ArgsHelper.FormatMessage("test {test}", new[] { new ArgPlaceholder() { Name = "test", Placeholder = "{test}" } }, Array.Empty<IArg>());

                formattedMessage.Should().Be("test {test}");
            }

            [Fact]
            public void Should_ReturnMessage_When_NullArgs()
            {
                var formattedMessage = ArgsHelper.FormatMessage("test {test}", new[] { new ArgPlaceholder() { Name = "test", Placeholder = "{test}" } }, null);

                formattedMessage.Should().Be("test {test}");
            }

            [Fact]
            public void Should_FormatMessage_SinglePlaceholder()
            {
                var parametersChecked = 0;

                var testArg = new TestArg(parameters =>
                {
                    parameters.Should().BeNull();
                    parametersChecked++;
                });

                testArg.AllowedParameters = Array.Empty<string>();

                testArg.Name = "test";
                testArg.Value = "testValue";

                var formattedMessage = ArgsHelper.FormatMessage(
                    "test {test}",
                    new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "test",
                            Placeholder = "{test}",
                            Parameters = null
                        },
                    },
                    new IArg[]
                    {
                        testArg
                    });

                formattedMessage.Should().Be("test testValue");

                parametersChecked.Should().Be(1);
            }

            [Fact]
            public void Should_FormatMessage_SinglePlaceholder_ManyOccurrencesInMessage()
            {
                var parametersChecked = 0;

                var testArg = new TestArg(parameters =>
                {
                    parameters.Should().BeNull();
                    parametersChecked++;
                });

                testArg.AllowedParameters = Array.Empty<string>();

                testArg.Name = "test";
                testArg.Value = "testValue";

                var formattedMessage = ArgsHelper.FormatMessage(
                    "test {test} {test} {test}",
                    new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "test",
                            Placeholder = "{test}",
                            Parameters = null
                        },
                    },
                    new IArg[]
                    {
                        testArg
                    });

                formattedMessage.Should().Be("test testValue testValue testValue");

                parametersChecked.Should().Be(1);
            }

            [Fact]
            public void Should_FormatMessage_ManyPlaceholders_SingleArg()
            {
                var parametersSet = new[]
                {
                    new Dictionary<string, string>()
                    {
                        ["p1"] = "v1",
                    },
                    new Dictionary<string, string>()
                    {
                        ["p2"] = "v2",
                    },
                    new Dictionary<string, string>()
                    {
                        ["p3"] = "v3",
                    }
                };

                var parametersChecked1 = 0;

                var testArg1 = new TestArg(parameters =>
                {
                    parameters.Should().BeSameAs(parametersSet[parametersChecked1]);
                    parametersChecked1++;
                })
                {
                    AllowedParameters = new[] { "p1", "p2", "p3" },
                    Name = "test1",
                    Value = "testValue1",
                };

                var formattedMessage = ArgsHelper.FormatMessage(
                    "test {test1|p1=v1} {test1|p2=v2} {test1|p3=v3}",
                    new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "test1",
                            Placeholder = "{test1|p1=v1}",
                            Parameters = parametersSet[0],
                        },
                        new ArgPlaceholder()
                        {
                            Name = "test1",
                            Placeholder = "{test1|p2=v2}",
                            Parameters = parametersSet[1],
                        },
                        new ArgPlaceholder()
                        {
                            Name = "test1",
                            Placeholder = "{test1|p3=v3}",
                            Parameters = parametersSet[2],
                        },
                    },
                    new IArg[]
                    {
                        testArg1
                    });

                formattedMessage.Should().Be("test testValue1 testValue1 testValue1");

                parametersChecked1.Should().Be(3);
            }

            [Fact]
            public void Should_FormatMessage_ManyArgs_ManyPlaceholders()
            {
                var parametersChecked1 = 0;

                var testArg1 = new TestArg(parameters =>
                {
                    parameters.Should().BeNull();
                    parametersChecked1++;
                })
                {
                    AllowedParameters = Array.Empty<string>(),
                    Name = "test1",
                    Value = "testValue1",
                };

                var parametersChecked2 = 0;

                var testArg2 = new TestArg(parameters =>
                {
                    parameters.Should().BeNull();
                    parametersChecked2++;
                })
                {
                    AllowedParameters = Array.Empty<string>(),
                    Name = "test2",
                    Value = "testValue2",
                };

                var formattedMessage = ArgsHelper.FormatMessage(
                    "test {test1} {test2} {test1}",
                    new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "test1",
                            Placeholder = "{test1}",
                            Parameters = null
                        },
                        new ArgPlaceholder()
                        {
                            Name = "test2",
                            Placeholder = "{test2}",
                            Parameters = null
                        },
                    },
                    new IArg[]
                    {
                        testArg1,
                        testArg2
                    });

                formattedMessage.Should().Be("test testValue1 testValue2 testValue1");

                parametersChecked1.Should().Be(1);
                parametersChecked2.Should().Be(1);
            }

            [Fact]
            public void Should_FormatMessage_ManyArgs_ManyPlaceholders_ManyParams()
            {
                var paramSet1 = new Dictionary<string, string>()
                {
                    ["p1"] = "v1",
                };

                var parametersChecked1 = 0;

                var testArg1 = new TestArg(parameters =>
                {
                    parameters.Should().BeSameAs(paramSet1);
                    parametersChecked1++;
                })
                {
                    AllowedParameters = new[] { "p1" },
                    Name = "test1",
                    Value = "testValue1",
                };

                var paramSet2 = new Dictionary<string, string>()
                {
                    ["p2"] = "v2",
                };

                var parametersChecked2 = 0;

                var testArg2 = new TestArg(parameters =>
                {
                    parameters.Should().BeSameAs(paramSet2);
                    parametersChecked2++;
                })
                {
                    AllowedParameters = new[] { "p2" },
                    Name = "test2",
                    Value = "testValue2",
                };

                var formattedMessage = ArgsHelper.FormatMessage(
                    "test {test1|p1=v1} {test2|p2=v2} {test1|p1=v1}",
                    new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "test1",
                            Placeholder = "{test1|p1=v1}",
                            Parameters = paramSet1,
                        },
                        new ArgPlaceholder()
                        {
                            Name = "test2",
                            Placeholder = "{test2|p2=v2}",
                            Parameters = paramSet2
                        },
                    },
                    new IArg[]
                    {
                        testArg1,
                        testArg2
                    });

                formattedMessage.Should().Be("test testValue1 testValue2 testValue1");

                parametersChecked1.Should().Be(1);
                parametersChecked2.Should().Be(1);
            }

            [Fact]
            public void Should_FormatMessage_PassParametersToArgs()
            {
                var parameters = new Dictionary<string, string>()
                {
                    ["param1"] = "paramValue1",
                };

                var parametersChecked1 = 0;

                var testArg1 = new TestArg(p =>
                {
                    p.Should().BeSameAs(parameters);
                    parametersChecked1++;
                })
                {
                    AllowedParameters = new[] { "param1" },
                    Name = "test1",
                    Value = "testValue1",
                };

                var formattedMessage = ArgsHelper.FormatMessage(
                    "test1 {test1|param1=paramValue1}",
                    new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "test1",
                            Placeholder = "{test1|param1=paramValue1}",
                            Parameters = parameters
                        },
                    },
                    new IArg[]
                    {
                        testArg1
                    });

                formattedMessage.Should().Be("test1 testValue1");

                parametersChecked1.Should().Be(1);
            }

            [Fact]
            public void Should_FormatMessage_NotPassParametersToArgs_When_AtLeastOneParameterNotAllowed()
            {
                var parameters = new Dictionary<string, string>()
                {
                    ["param1"] = "paramValue1",
                    ["param2"] = "paramValue2",
                };

                var parametersChecked1 = 0;

                var testArg1 = new TestArg(p =>
                {
                    p.Should().BeSameAs(parameters);
                    parametersChecked1++;
                })
                {
                    AllowedParameters = new[] { "param1", "param2" },
                    Name = "test1",
                    Value = "testValue1",
                };

                var formattedMessage = ArgsHelper.FormatMessage(
                    "test1 {test1|param1=paramValue1|param2=paramValue2} {test1|param1=paramValue1|param2=paramValue2|invalidParameter=someValue}",
                    new[]
                    {
                        new ArgPlaceholder()
                        {
                            Name = "test1",
                            Placeholder = "{test1|param1=paramValue1|param2=paramValue2}",
                            Parameters = parameters,
                        },
                        new ArgPlaceholder()
                        {
                            Name = "test1",
                            Placeholder = "{test1|param1=paramValue1|param2=paramValue2|invalidParameter=someValue}",
                            Parameters = new Dictionary<string, string>()
                            {
                                ["param1"] = "paramValue1",
                                ["param2"] = "paramValue2",
                                ["invalidParameter"] = "someValue"
                            },
                        },
                    },
                    new IArg[]
                    {
                        testArg1
                    });

                formattedMessage.Should().Be("test1 testValue1 {test1|param1=paramValue1|param2=paramValue2|invalidParameter=someValue}");

                parametersChecked1.Should().Be(1);
            }

            private class TestArg : IArg
            {
                private readonly Action<IReadOnlyDictionary<string, string>> _parametersCheck;

                public TestArg(Action<IReadOnlyDictionary<string, string>> parametersCheck)
                {
                    _parametersCheck = parametersCheck;
                }

                public string Value { get; set; }

                public string Name { get; set; }

                public IReadOnlyCollection<string> AllowedParameters { get; set; }

                public string ToString(IReadOnlyDictionary<string, string> parameters)
                {
                    _parametersCheck?.Invoke(parameters);

                    return Value;
                }
            }
        }

        public class ExtractPlaceholders
        {
            [Theory]
            [InlineData("{single stuff")]
            [InlineData("single stuff}")]
            [InlineData("single }{ stuff")]
            [InlineData("single stuff")]
            [InlineData("")]
            public void Should_ReturnEmptyCollection_When_NoVariable(string message)
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders(message);

                placeholders.Should().BeEmpty();
            }

            [Theory]
            [InlineData("abc  {invalid|param1=value1param1=value2} def")]
            [InlineData("abc  {invalid|param1value1param1value2} def")]
            [InlineData("abc  {invalid|=} def")]
            [InlineData("abc  {invalid||} def")]
            public void Should_ReturnEmptyCollection_When_InvalidParameters(string message)
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders(message);

                placeholders.Should().BeEmpty();
            }

            [Theory]
            [InlineData("abc  {|param1=value1} def")]
            [InlineData("abc  {  |param1=value1} def")]
            public void Should_ReturnEmptyCollection_When_EmptyName(string message)
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders(message);

                placeholders.Should().BeEmpty();
            }

            [Fact]
            public void Should_Extract_And_SquashDuplicates()
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders("abc {single|param1=value1}  def {single|param1=value1}");

                placeholders.Should().ContainSingle();

                placeholders.Single().Placeholder.Should().Be("{single|param1=value1}");
                placeholders.Single().Name.Should().Be("single");
                placeholders.Single().Parameters.Should().ContainSingle();
                placeholders.Single().Parameters.Single().Key.Should().Be("param1");
                placeholders.Single().Parameters.Single().Value.Should().Be("value1");
            }

            [Fact]
            public void Should_Parse_When_ManySameVariables_With_DifferentParameters()
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders("abc {first|p1=v1} {first|p21=v21|p22=v22} def {first}");

                placeholders.Count.Should().Be(3);

                placeholders.ElementAt(0).Placeholder.Should().Be("{first|p1=v1}");
                placeholders.ElementAt(0).Name.Should().Be("first");
                placeholders.ElementAt(0).Parameters.Should().ContainSingle();
                placeholders.ElementAt(0).Parameters.Single().Key.Should().Be("p1");
                placeholders.ElementAt(0).Parameters.Single().Value.Should().Be("v1");

                placeholders.ElementAt(1).Placeholder.Should().Be("{first|p21=v21|p22=v22}");
                placeholders.ElementAt(1).Name.Should().Be("first");
                placeholders.ElementAt(1).Parameters.Count.Should().Be(2);
                placeholders.ElementAt(1).Parameters.ElementAt(0).Key.Should().Be("p21");
                placeholders.ElementAt(1).Parameters.ElementAt(0).Value.Should().Be("v21");
                placeholders.ElementAt(1).Parameters.ElementAt(1).Key.Should().Be("p22");
                placeholders.ElementAt(1).Parameters.ElementAt(1).Value.Should().Be("v22");

                placeholders.ElementAt(2).Placeholder.Should().Be("{first}");
                placeholders.ElementAt(2).Name.Should().Be("first");
                placeholders.ElementAt(2).Parameters.Should().BeEmpty();
            }

            [Fact]
            public void Should_Parse_When_ManyVariables()
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders("abc {first} {second} def {third}");

                placeholders.Count.Should().Be(3);

                placeholders.ElementAt(0).Placeholder.Should().Be("{first}");
                placeholders.ElementAt(0).Name.Should().Be("first");
                placeholders.ElementAt(0).Parameters.Should().BeEmpty();

                placeholders.ElementAt(1).Placeholder.Should().Be("{second}");
                placeholders.ElementAt(1).Name.Should().Be("second");
                placeholders.ElementAt(1).Parameters.Should().BeEmpty();

                placeholders.ElementAt(2).Placeholder.Should().Be("{third}");
                placeholders.ElementAt(2).Name.Should().Be("third");
                placeholders.ElementAt(2).Parameters.Should().BeEmpty();
            }

            [Fact]
            public void Should_Parse_When_ManyVariables_With_Parameters()
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders("abc {first|p1=v1} {second|p21=v21|p22=v22} def {third|p31=v31|p32=v32|p33=v33}");

                placeholders.Count.Should().Be(3);

                placeholders.ElementAt(0).Placeholder.Should().Be("{first|p1=v1}");
                placeholders.ElementAt(0).Name.Should().Be("first");
                placeholders.ElementAt(0).Parameters.Count.Should().Be(1);
                placeholders.ElementAt(0).Parameters.ElementAt(0).Key.Should().Be("p1");
                placeholders.ElementAt(0).Parameters.ElementAt(0).Value.Should().Be("v1");

                placeholders.ElementAt(1).Placeholder.Should().Be("{second|p21=v21|p22=v22}");
                placeholders.ElementAt(1).Name.Should().Be("second");
                placeholders.ElementAt(1).Parameters.Count.Should().Be(2);
                placeholders.ElementAt(1).Parameters.ElementAt(0).Key.Should().Be("p21");
                placeholders.ElementAt(1).Parameters.ElementAt(0).Value.Should().Be("v21");
                placeholders.ElementAt(1).Parameters.ElementAt(1).Key.Should().Be("p22");
                placeholders.ElementAt(1).Parameters.ElementAt(1).Value.Should().Be("v22");

                placeholders.ElementAt(2).Placeholder.Should().Be("{third|p31=v31|p32=v32|p33=v33}");
                placeholders.ElementAt(2).Name.Should().Be("third");
                placeholders.ElementAt(2).Parameters.Count.Should().Be(3);
                placeholders.ElementAt(2).Parameters.ElementAt(0).Key.Should().Be("p31");
                placeholders.ElementAt(2).Parameters.ElementAt(0).Value.Should().Be("v31");
                placeholders.ElementAt(2).Parameters.ElementAt(1).Key.Should().Be("p32");
                placeholders.ElementAt(2).Parameters.ElementAt(1).Value.Should().Be("v32");
                placeholders.ElementAt(2).Parameters.ElementAt(2).Key.Should().Be("p33");
                placeholders.ElementAt(2).Parameters.ElementAt(2).Value.Should().Be("v33");
            }

            [Fact]
            public void Should_Parse_When_SingleVariable()
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders("abc {single} def");

                placeholders.Count.Should().Be(1);

                placeholders.ElementAt(0).Placeholder.Should().Be("{single}");
                placeholders.ElementAt(0).Name.Should().Be("single");
                placeholders.ElementAt(0).Parameters.Count.Should().Be(0);
            }

            [Fact]
            public void Should_Parse_When_SingleVariable_With_ManyParameters()
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders("abc {single|param1=value1|param2=value2|param3=value3} def");

                placeholders.Count.Should().Be(1);

                placeholders.ElementAt(0).Placeholder.Should().Be("{single|param1=value1|param2=value2|param3=value3}");
                placeholders.ElementAt(0).Name.Should().Be("single");
                placeholders.ElementAt(0).Parameters.Count.Should().Be(3);
                placeholders.ElementAt(0).Parameters.ElementAt(0).Key.Should().Be("param1");
                placeholders.ElementAt(0).Parameters.ElementAt(0).Value.Should().Be("value1");
                placeholders.ElementAt(0).Parameters.ElementAt(1).Key.Should().Be("param2");
                placeholders.ElementAt(0).Parameters.ElementAt(1).Value.Should().Be("value2");
                placeholders.ElementAt(0).Parameters.ElementAt(2).Key.Should().Be("param3");
                placeholders.ElementAt(0).Parameters.ElementAt(2).Value.Should().Be("value3");
            }

            [Fact]
            public void Should_Parse_When_SingleVariable_With_SingleParameter()
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders("abc {single|param1=value1} def");

                placeholders.Count.Should().Be(1);

                placeholders.ElementAt(0).Placeholder.Should().Be("{single|param1=value1}");
                placeholders.ElementAt(0).Name.Should().Be("single");
                placeholders.ElementAt(0).Parameters.Count.Should().Be(1);
                placeholders.ElementAt(0).Parameters.ElementAt(0).Key.Should().Be("param1");
                placeholders.ElementAt(0).Parameters.ElementAt(0).Value.Should().Be("value1");
            }

            [Fact]
            public void Should_ParseOnlyValidOnes()
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders("{valid} abc {invalid|param1=value1|param1=value2} {valid2|param=value} def {invalid|param1=value1param1=value2} xyz {invalid2|param1value1param1value2}");

                placeholders.Count.Should().Be(2);

                placeholders.ElementAt(0).Placeholder.Should().Be("{valid}");
                placeholders.ElementAt(0).Name.Should().Be("valid");
                placeholders.ElementAt(0).Parameters.Count.Should().Be(0);

                placeholders.ElementAt(1).Placeholder.Should().Be("{valid2|param=value}");
                placeholders.ElementAt(1).Name.Should().Be("valid2");
                placeholders.ElementAt(1).Parameters.Count.Should().Be(1);
                placeholders.ElementAt(1).Parameters.ElementAt(0).Key.Should().Be("param");
                placeholders.ElementAt(1).Parameters.ElementAt(0).Value.Should().Be("value");
            }

            [Fact]
            public void Should_ReturnEmptyCollection_When_DuplicateParameter()
            {
                IReadOnlyCollection<ArgPlaceholder> placeholders = ArgsHelper.ExtractPlaceholders("abc  {invalid|param1=value1|param1=value2} def");

                placeholders.Should().BeEmpty();
            }

            [Fact]
            public void Should_ThrowException_When_NullMessage()
            {
                Action action = () => ArgsHelper.ExtractPlaceholders(null);

                action.Should().ThrowExactly<ArgumentNullException>();
            }
        }

        [Fact]
        public void Assignment_Should_BeEqualitySign()
        {
            ArgsHelper.Assignment.Should().Be('=');
        }

        [Fact]
        public void Divider_Should_BeVerticalBar()
        {
            ArgsHelper.Divider.Should().Be('|');
        }
    }
}
