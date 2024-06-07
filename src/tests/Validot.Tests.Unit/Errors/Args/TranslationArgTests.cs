namespace Validot.Tests.Unit.Errors.Args
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Errors.Args;

    using Xunit;

    public class TranslationArgTests
    {
        [Fact]
        public void Should_Initialize()
        {
            var translation = new Dictionary<string, string>();

            var arg = new TranslationArg(translation);

            TranslationArg.Name.Should().Be("_translation");

            (arg as IArg).Name.Should().Be("_translation");

            arg.AllowedParameters.Count.Should().Be(1);
            arg.AllowedParameters.Should().Contain("key");
        }

        [Theory]
        [InlineData("test", "{_translation|key=test}")]
        [InlineData("Some.Sample.Value", "{_translation|key=Some.Sample.Value}")]
        public void Should_CreatePlaceHolder(string key, string expectedPlaceholder)
        {
            var placeholder = TranslationArg.CreatePlaceholder(key);

            placeholder.Should().Be(expectedPlaceholder);
        }

        [Fact]
        public void Should_Stringify_UsingDictionary_And_Key()
        {
            var translation = new Dictionary<string, string>()
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
            };

            var arg = new TranslationArg(translation);

            var key1 = arg.ToString(new Dictionary<string, string>()
            {
                ["key"] = "key1",
            });

            var key2 = arg.ToString(new Dictionary<string, string>()
            {
                ["key"] = "key2",
            });

            key1.Should().Be("value1");
            key2.Should().Be("value2");
        }

        [Fact]
        public void Should_Stringify_ReturnKey_If_KeyNotInTranslation()
        {
            var translation = new Dictionary<string, string>()
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
            };

            var arg = new TranslationArg(translation);

            var key1 = arg.ToString(new Dictionary<string, string>()
            {
                ["key"] = "invalid",
            });

            var key2 = arg.ToString(new Dictionary<string, string>()
            {
                ["key"] = "KEY1",
            });

            key1.Should().Be("invalid");
            key2.Should().Be("KEY1");
        }

        [Fact]
        public void Should_Stringify_ReturnArgName_If_NullParameters()
        {
            var translation = new Dictionary<string, string>()
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
            };

            var arg = new TranslationArg(translation);

            var name = arg.ToString(null);

            name.Should().Be("_translation");
        }

        [Fact]
        public void Should_Stringify_ReturnArgName_If_InvalidParameterName()
        {
            var translation = new Dictionary<string, string>()
            {
                ["key1"] = "value1",
                ["key2"] = "value2",
            };

            var arg = new TranslationArg(translation);

            var key = new Dictionary<string, string>()
            {
                ["invalid_key"] = "value1"
            };

            var name = arg.ToString(key);

            name.Should().Be("_translation");
        }

        [Fact]
        public void Should_ThrowException_When_NullKey()
        {
            new Action(() =>
            {
                new TranslationArg(null);
            }).Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
