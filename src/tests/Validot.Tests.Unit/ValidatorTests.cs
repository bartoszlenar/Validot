namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Errors;
    using Validot.Errors.Args;
    using Validot.Settings;
    using Validot.Validation.Scheme;

    using Xunit;

    public class ValidatorTests
    {
        [Fact]
        public void Should_ThrowException_When_NullModelScheme()
        {
            Action action = () => _ = new Validator<object>(null, Substitute.For<IValidatorSettings>());

            action.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("modelScheme");
        }

        [Fact]
        public void Should_ThrowException_When_NullSettings()
        {
            Action action = () => _ = new Validator<object>(Substitute.For<IModelScheme<object>>(), null);

            action.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("settings");
        }

        [Fact]
        public void Should_SetTemplate()
        {
            var modelScheme = Substitute.For<IModelScheme<object>>();

            modelScheme.ErrorRegistry.Returns(new Dictionary<int, IError>()
            {
                [0] = new Error()
                {
                    Messages = new[]
                    {
                        "Zero"
                    },
                    Codes = new[]
                    {
                        "000"
                    },
                    Args = Array.Empty<IArg>()
                },
                [1] = new Error()
                {
                    Messages = new[]
                    {
                        "One"
                    },
                    Codes = new[]
                    {
                        "111"
                    },
                    Args = Array.Empty<IArg>()
                }
            });

            modelScheme.Template.Returns(new Dictionary<string, IReadOnlyList<int>>()
            {
                [""] = new[]
                {
                    0
                },
                ["path"] = new[]
                {
                    1
                },
                ["path.nested"] = new[]
                {
                    0, 1
                }
            });

            var settings = Substitute.For<IValidatorSettings>();

            settings.Translations.Returns(new Dictionary<string, IReadOnlyDictionary<string, string>>()
            {
                ["English"] = new Dictionary<string, string>()
                {
                    ["Zero"] = "English translated ZERO",
                    ["One"] = "English translated ONE!!!"
                }
            });

            var validator = new Validator<object>(modelScheme, settings);

            validator.Template.Should().NotBeNull();

            validator.Template.MessageMap[""].Should().HaveCount(1);
            validator.Template.MessageMap[""].Should().Contain("English translated ZERO");

            validator.Template.MessageMap["path"].Should().HaveCount(1);
            validator.Template.MessageMap["path"].Should().Contain("English translated ONE!!!");

            validator.Template.MessageMap["path.nested"].Should().HaveCount(2);
            validator.Template.MessageMap["path.nested"].Should().Contain("English translated ZERO", "English translated ONE!!!");

            validator.Template.CodeMap[""].Should().HaveCount(1);
            validator.Template.CodeMap[""].Should().Contain("000");

            validator.Template.CodeMap["path"].Should().HaveCount(1);
            validator.Template.CodeMap["path"].Should().Contain("111");

            validator.Template.CodeMap["path.nested"].Should().HaveCount(2);
            validator.Template.CodeMap["path.nested"].Should().Contain("000", "111");
        }

        [Fact]
        public void Should_SetSettings()
        {
            var settings = Substitute.For<IValidatorSettings>();

            settings.Translations.Returns(new Dictionary<string, IReadOnlyDictionary<string, string>>()
            {
                ["English"] = new Dictionary<string, string>()
                {
                    ["X"] = "XXX"
                }
            });

            var modelScheme = Substitute.For<IModelScheme<object>>();

            modelScheme.ErrorRegistry.Returns(new Dictionary<int, IError>()
            {
                [0] = new Error()
                {
                    Messages = new[]
                    {
                        "X"
                    },
                    Codes = Array.Empty<string>(),
                    Args = Array.Empty<IArg>()
                }
            });

            var validator = new Validator<object>(modelScheme, settings);

            validator.Settings.Should().NotBeNull();
            validator.Settings.Should().BeSameAs(settings);
        }

        [Fact]
        public void Factory_Should_NotBeNull()
        {
            Validator.Factory.Should().NotBeNull();
        }
    }
}
