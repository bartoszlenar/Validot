namespace Validot.Tests.Unit.Translations
{
    using System;

    using FluentAssertions;

    using Validot.Translations;

    using Xunit;

    public class TranslationsCompilerTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new TranslationsCompiler();
        }

        [Fact]
        public void Should_Initialize_Empty()
        {
            var translationsCompiler = new TranslationsCompiler();

            translationsCompiler.Translations.Should().NotBeNull();
            translationsCompiler.Translations.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null, "2", "3")]
        [InlineData("1", null, "3")]
        [InlineData("1", "2", null)]
        [InlineData("1", null, null)]
        [InlineData(null, "2", null)]
        [InlineData(null, null, "3")]
        [InlineData(null, null, null)]
        public void Add_Should_ThrowException_When_NullArgs(string name, string messageKey, string tralsnation)
        {
            var translationsCompiler = new TranslationsCompiler();

            Action action = () => translationsCompiler.Add(name, messageKey, tralsnation);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Add_Should_AddTranslation()
        {
            var translationsCompiler = new TranslationsCompiler();

            translationsCompiler.Add("name", "key", "value");

            translationsCompiler.Translations.Keys.Should().HaveCount(1);
            translationsCompiler.Translations.Keys.Should().Contain("name");
            translationsCompiler.Translations["name"].Keys.Should().HaveCount(1);
            translationsCompiler.Translations["name"].Keys.Should().Contain("key");
            translationsCompiler.Translations["name"]["key"].Should().Be("value");
        }

        [Fact]
        public void Add_Should_AddTranslation_ManyTimes()
        {
            var translationsCompiler = new TranslationsCompiler();

            translationsCompiler.Add("name1", "key1", "value1");
            translationsCompiler.Add("name2", "key2", "value2");

            translationsCompiler.Translations.Keys.Should().HaveCount(2);
            translationsCompiler.Translations.Keys.Should().Contain("name1");
            translationsCompiler.Translations.Keys.Should().Contain("name2");

            translationsCompiler.Translations["name1"].Keys.Should().HaveCount(1);
            translationsCompiler.Translations["name1"].Keys.Should().Contain("key1");
            translationsCompiler.Translations["name1"]["key1"].Should().Be("value1");

            translationsCompiler.Translations["name2"].Keys.Should().HaveCount(1);
            translationsCompiler.Translations["name2"].Keys.Should().Contain("key2");
            translationsCompiler.Translations["name2"]["key2"].Should().Be("value2");
        }

        [Fact]
        public void Add_Should_AddTranslation_ManyTimes_SameTranslation()
        {
            var translationsCompiler = new TranslationsCompiler();

            translationsCompiler.Add("name", "key1", "value1");
            translationsCompiler.Add("name", "key2", "value2");

            translationsCompiler.Translations.Keys.Should().HaveCount(1);
            translationsCompiler.Translations.Keys.Should().Contain("name");

            translationsCompiler.Translations["name"].Keys.Should().HaveCount(2);

            translationsCompiler.Translations["name"].Keys.Should().Contain("key1");
            translationsCompiler.Translations["name"]["key1"].Should().Be("value1");

            translationsCompiler.Translations["name"].Keys.Should().Contain("key2");
            translationsCompiler.Translations["name"]["key2"].Should().Be("value2");
        }

        [Fact]
        public void Add_Should_AddTranslation_Should_OverwriteValue()
        {
            var translationsCompiler = new TranslationsCompiler();

            translationsCompiler.Add("name1", "key1", "value1");
            translationsCompiler.Add("name2", "key2", "value2");

            translationsCompiler.Add("name2", "key2", "VALUE_2");

            translationsCompiler.Translations.Keys.Should().HaveCount(2);
            translationsCompiler.Translations.Keys.Should().Contain("name1");
            translationsCompiler.Translations.Keys.Should().Contain("name2");

            translationsCompiler.Translations["name1"].Keys.Should().HaveCount(1);
            translationsCompiler.Translations["name1"].Keys.Should().Contain("key1");
            translationsCompiler.Translations["name1"]["key1"].Should().Be("value1");

            translationsCompiler.Translations["name2"].Keys.Should().HaveCount(1);
            translationsCompiler.Translations["name2"].Keys.Should().Contain("key2");
            translationsCompiler.Translations["name2"]["key2"].Should().Be("VALUE_2");
        }

        [Fact]
        public void Add_Should_AddTranslation_Should_OverwriteValue_WhenSameKeysInDifferentDictionaries()
        {
            var translationsCompiler = new TranslationsCompiler();

            translationsCompiler.Add("name1", "key", "value1");
            translationsCompiler.Add("name2", "key", "value2");

            translationsCompiler.Add("name2", "key", "VALUE_2");

            translationsCompiler.Translations.Keys.Should().HaveCount(2);
            translationsCompiler.Translations.Keys.Should().Contain("name1");
            translationsCompiler.Translations.Keys.Should().Contain("name2");

            translationsCompiler.Translations["name1"].Keys.Should().HaveCount(1);
            translationsCompiler.Translations["name1"].Keys.Should().Contain("key");
            translationsCompiler.Translations["name1"]["key"].Should().Be("value1");

            translationsCompiler.Translations["name2"].Keys.Should().HaveCount(1);
            translationsCompiler.Translations["name2"].Keys.Should().Contain("key");
            translationsCompiler.Translations["name2"]["key"].Should().Be("VALUE_2");
        }
    }
}
