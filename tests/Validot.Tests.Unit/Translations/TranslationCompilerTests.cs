namespace Validot.Tests.Unit.Translations
{
    using System;

    using FluentAssertions;

    using Validot.Translations;

    using Xunit;

    public class TranslationCompilerTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new TranslationCompiler();
        }

        [Fact]
        public void Should_Initialize_Empty()
        {
            var translationCompiler = new TranslationCompiler();

            translationCompiler.Translations.Should().NotBeNull();
            translationCompiler.Translations.Should().BeEmpty();
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
            var translationCompiler = new TranslationCompiler();

            Action action = () => translationCompiler.Add(name, messageKey, tralsnation);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Add_Should_AddTranslation()
        {
            var translationCompiler = new TranslationCompiler();

            translationCompiler.Add("name", "key", "value");

            translationCompiler.Translations.Keys.Should().HaveCount(1);
            translationCompiler.Translations.Keys.Should().Contain("name");
            translationCompiler.Translations["name"].Keys.Should().HaveCount(1);
            translationCompiler.Translations["name"].Keys.Should().Contain("key");
            translationCompiler.Translations["name"]["key"].Should().Be("value");
        }

        [Fact]
        public void Add_Should_AddTranslation_ManyTimes()
        {
            var translationCompiler = new TranslationCompiler();

            translationCompiler.Add("name1", "key1", "value1");
            translationCompiler.Add("name2", "key2", "value2");

            translationCompiler.Translations.Keys.Should().HaveCount(2);
            translationCompiler.Translations.Keys.Should().Contain("name1");
            translationCompiler.Translations.Keys.Should().Contain("name2");

            translationCompiler.Translations["name1"].Keys.Should().HaveCount(1);
            translationCompiler.Translations["name1"].Keys.Should().Contain("key1");
            translationCompiler.Translations["name1"]["key1"].Should().Be("value1");

            translationCompiler.Translations["name2"].Keys.Should().HaveCount(1);
            translationCompiler.Translations["name2"].Keys.Should().Contain("key2");
            translationCompiler.Translations["name2"]["key2"].Should().Be("value2");
        }

        [Fact]
        public void Add_Should_AddTranslation_ManyTimes_SameTranslation()
        {
            var translationCompiler = new TranslationCompiler();

            translationCompiler.Add("name", "key1", "value1");
            translationCompiler.Add("name", "key2", "value2");

            translationCompiler.Translations.Keys.Should().HaveCount(1);
            translationCompiler.Translations.Keys.Should().Contain("name");

            translationCompiler.Translations["name"].Keys.Should().HaveCount(2);

            translationCompiler.Translations["name"].Keys.Should().Contain("key1");
            translationCompiler.Translations["name"]["key1"].Should().Be("value1");

            translationCompiler.Translations["name"].Keys.Should().Contain("key2");
            translationCompiler.Translations["name"]["key2"].Should().Be("value2");
        }

        [Fact]
        public void Add_Should_AddTranslation_Should_OverwriteValue()
        {
            var translationCompiler = new TranslationCompiler();

            translationCompiler.Add("name1", "key1", "value1");
            translationCompiler.Add("name2", "key2", "value2");

            translationCompiler.Add("name2", "key2", "VALUE_2");

            translationCompiler.Translations.Keys.Should().HaveCount(2);
            translationCompiler.Translations.Keys.Should().Contain("name1");
            translationCompiler.Translations.Keys.Should().Contain("name2");

            translationCompiler.Translations["name1"].Keys.Should().HaveCount(1);
            translationCompiler.Translations["name1"].Keys.Should().Contain("key1");
            translationCompiler.Translations["name1"]["key1"].Should().Be("value1");

            translationCompiler.Translations["name2"].Keys.Should().HaveCount(1);
            translationCompiler.Translations["name2"].Keys.Should().Contain("key2");
            translationCompiler.Translations["name2"]["key2"].Should().Be("VALUE_2");
        }

        [Fact]
        public void Add_Should_AddTranslation_Should_OverwriteValue_WhenSameKeysInDifferentDictionaries()
        {
            var translationCompiler = new TranslationCompiler();

            translationCompiler.Add("name1", "key", "value1");
            translationCompiler.Add("name2", "key", "value2");

            translationCompiler.Add("name2", "key", "VALUE_2");

            translationCompiler.Translations.Keys.Should().HaveCount(2);
            translationCompiler.Translations.Keys.Should().Contain("name1");
            translationCompiler.Translations.Keys.Should().Contain("name2");

            translationCompiler.Translations["name1"].Keys.Should().HaveCount(1);
            translationCompiler.Translations["name1"].Keys.Should().Contain("key");
            translationCompiler.Translations["name1"]["key"].Should().Be("value1");

            translationCompiler.Translations["name2"].Keys.Should().HaveCount(1);
            translationCompiler.Translations["name2"].Keys.Should().Contain("key");
            translationCompiler.Translations["name2"]["key"].Should().Be("VALUE_2");
        }
    }
}
