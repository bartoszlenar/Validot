namespace Validot.Tests.Unit.Translations
{
    using System.Collections.Generic;

    using NSubstitute;

    using Validot.Translations;

    using Xunit;

    public class TranslationCompilerExtensionsTests
    {
        [Fact]
        public void Should_Add_PassAllEntriesFromDictionary_ToSelectedTranslation()
        {
            var translationsCompiler = Substitute.For<ITranslationsCompiler>();

            var dictionary = new Dictionary<string, string>()
            {
                ["k1"] = "v1",
                ["k2"] = "v2",
                ["k3"] = "v3",
                ["k4"] = "v4",
            };

            translationsCompiler.Add("name", dictionary);

            translationsCompiler.ReceivedWithAnyArgs(4).Add(default, default, default);

            translationsCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k1"), Arg.Is("v1"));
            translationsCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k2"), Arg.Is("v2"));
            translationsCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k3"), Arg.Is("v3"));
            translationsCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k4"), Arg.Is("v4"));
        }

        [Fact]
        public void Should_Add_PassAllEntriesFromDictionary_ToSelectedTranslation_MultipleTimes()
        {
            var translationsCompiler = Substitute.For<ITranslationsCompiler>();

            var dictionary1 = new Dictionary<string, string>()
            {
                ["k11"] = "v11",
                ["k12"] = "v12",
                ["k13"] = "v13",
                ["k14"] = "v14",
            };

            var dictionary2 = new Dictionary<string, string>()
            {
                ["k21"] = "v21",
                ["k22"] = "v22",
                ["k23"] = "v23",
                ["k24"] = "v24",
            };

            translationsCompiler.Add("name1", dictionary1);
            translationsCompiler.Add("name2", dictionary2);

            translationsCompiler.ReceivedWithAnyArgs(8).Add(default, default, default);

            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k11"), Arg.Is("v11"));
            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k12"), Arg.Is("v12"));
            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k13"), Arg.Is("v13"));
            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k14"), Arg.Is("v14"));

            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k21"), Arg.Is("v21"));
            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k22"), Arg.Is("v22"));
            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k23"), Arg.Is("v23"));
            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k24"), Arg.Is("v24"));
        }

        [Fact]
        public void Should_Add_PassAllTranslationDictionaryEntries()
        {
            var translationsCompiler = Substitute.For<ITranslationsCompiler>();

            var dictionary = new Dictionary<string, IReadOnlyDictionary<string, string>>()
            {
                ["name"] = new Dictionary<string, string>()
                {
                    ["k1"] = "v1",
                    ["k2"] = "v2",
                    ["k3"] = "v3",
                    ["k4"] = "v4",
                }
            };

            translationsCompiler.Add(dictionary);

            translationsCompiler.ReceivedWithAnyArgs(4).Add(default, default, default);

            translationsCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k1"), Arg.Is("v1"));
            translationsCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k2"), Arg.Is("v2"));
            translationsCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k3"), Arg.Is("v3"));
            translationsCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k4"), Arg.Is("v4"));
        }

        [Fact]
        public void Should_Add_PassAllTranslationDictionaryEntries_When_MoreThanOneTranslationDefined()
        {
            var translationsCompiler = Substitute.For<ITranslationsCompiler>();

            var dictionary = new Dictionary<string, IReadOnlyDictionary<string, string>>()
            {
                ["name1"] = new Dictionary<string, string>()
                {
                    ["k11"] = "v11",
                    ["k12"] = "v12",
                    ["k13"] = "v13",
                    ["k14"] = "v14",
                },
                ["name2"] = new Dictionary<string, string>()
                {
                    ["k21"] = "v21",
                    ["k22"] = "v22",
                    ["k23"] = "v23",
                    ["k24"] = "v24",
                }
            };

            translationsCompiler.Add(dictionary);

            translationsCompiler.ReceivedWithAnyArgs(8).Add(default, default, default);

            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k11"), Arg.Is("v11"));
            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k12"), Arg.Is("v12"));
            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k13"), Arg.Is("v13"));
            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k14"), Arg.Is("v14"));

            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k21"), Arg.Is("v21"));
            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k22"), Arg.Is("v22"));
            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k23"), Arg.Is("v23"));
            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k24"), Arg.Is("v24"));
        }

        [Fact]
        public void Should_Add_PassAllTranslationDictionaryEntries_MultipleTimes()
        {
            var translationsCompiler = Substitute.For<ITranslationsCompiler>();

            var dictionary1 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
            {
                ["name1"] = new Dictionary<string, string>()
                {
                    ["k11"] = "v11",
                    ["k12"] = "v12",
                    ["k13"] = "v13",
                    ["k14"] = "v14",
                }
            };

            var dictionary2 = new Dictionary<string, IReadOnlyDictionary<string, string>>()
            {
                ["name2"] = new Dictionary<string, string>()
                {
                    ["k21"] = "v21",
                    ["k22"] = "v22",
                    ["k23"] = "v23",
                    ["k24"] = "v24",
                }
            };

            translationsCompiler.Add(dictionary1);
            translationsCompiler.Add(dictionary2);

            translationsCompiler.ReceivedWithAnyArgs(8).Add(default, default, default);

            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k11"), Arg.Is("v11"));
            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k12"), Arg.Is("v12"));
            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k13"), Arg.Is("v13"));
            translationsCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k14"), Arg.Is("v14"));

            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k21"), Arg.Is("v21"));
            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k22"), Arg.Is("v22"));
            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k23"), Arg.Is("v23"));
            translationsCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k24"), Arg.Is("v24"));
        }
    }
}
