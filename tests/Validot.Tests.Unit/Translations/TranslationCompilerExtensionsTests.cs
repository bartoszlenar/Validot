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
            var translationCompiler = Substitute.For<ITranslationCompiler>();

            var dictionary = new Dictionary<string, string>()
            {
                ["k1"] = "v1",
                ["k2"] = "v2",
                ["k3"] = "v3",
                ["k4"] = "v4",
            };

            translationCompiler.Add("name", dictionary);

            translationCompiler.ReceivedWithAnyArgs(4).Add(default, default, default);

            translationCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k1"), Arg.Is("v1"));
            translationCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k2"), Arg.Is("v2"));
            translationCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k3"), Arg.Is("v3"));
            translationCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k4"), Arg.Is("v4"));
        }

        [Fact]
        public void Should_Add_PassAllEntriesFromDictionary_ToSelectedTranslation_MultipleTimes()
        {
            var translationCompiler = Substitute.For<ITranslationCompiler>();

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

            translationCompiler.Add("name1", dictionary1);
            translationCompiler.Add("name2", dictionary2);

            translationCompiler.ReceivedWithAnyArgs(8).Add(default, default, default);

            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k11"), Arg.Is("v11"));
            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k12"), Arg.Is("v12"));
            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k13"), Arg.Is("v13"));
            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k14"), Arg.Is("v14"));

            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k21"), Arg.Is("v21"));
            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k22"), Arg.Is("v22"));
            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k23"), Arg.Is("v23"));
            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k24"), Arg.Is("v24"));
        }

        [Fact]
        public void Should_Add_PassAllTranslationDictionaryEntries()
        {
            var translationCompiler = Substitute.For<ITranslationCompiler>();

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

            translationCompiler.Add(dictionary);

            translationCompiler.ReceivedWithAnyArgs(4).Add(default, default, default);

            translationCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k1"), Arg.Is("v1"));
            translationCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k2"), Arg.Is("v2"));
            translationCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k3"), Arg.Is("v3"));
            translationCompiler.Received(1).Add(Arg.Is("name"), Arg.Is("k4"), Arg.Is("v4"));
        }

        [Fact]
        public void Should_Add_PassAllTranslationDictionaryEntries_When_MoreThanOneTranslationDefined()
        {
            var translationCompiler = Substitute.For<ITranslationCompiler>();

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

            translationCompiler.Add(dictionary);

            translationCompiler.ReceivedWithAnyArgs(8).Add(default, default, default);

            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k11"), Arg.Is("v11"));
            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k12"), Arg.Is("v12"));
            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k13"), Arg.Is("v13"));
            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k14"), Arg.Is("v14"));

            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k21"), Arg.Is("v21"));
            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k22"), Arg.Is("v22"));
            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k23"), Arg.Is("v23"));
            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k24"), Arg.Is("v24"));
        }

        [Fact]
        public void Should_Add_PassAllTranslationDictionaryEntries_MultipleTimes()
        {
            var translationCompiler = Substitute.For<ITranslationCompiler>();

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

            translationCompiler.Add(dictionary1);
            translationCompiler.Add(dictionary2);

            translationCompiler.ReceivedWithAnyArgs(8).Add(default, default, default);

            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k11"), Arg.Is("v11"));
            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k12"), Arg.Is("v12"));
            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k13"), Arg.Is("v13"));
            translationCompiler.Received(1).Add(Arg.Is("name1"), Arg.Is("k14"), Arg.Is("v14"));

            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k21"), Arg.Is("v21"));
            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k22"), Arg.Is("v22"));
            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k23"), Arg.Is("v23"));
            translationCompiler.Received(1).Add(Arg.Is("name2"), Arg.Is("k24"), Arg.Is("v24"));
        }
    }
}
