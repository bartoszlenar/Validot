namespace Validot.Tests.Unit.Translations
{
    using System.Reflection;

    using FluentAssertions;

    using Validot.Translations;

    using Xunit;

    public class MessageKeyTests
    {
        [Fact]
        public void Should_HaveAllValuesAsThePathToTheProperty()
        {
            var globalType = typeof(MessageKey);

            var innerTypes = globalType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var innerType in innerTypes)
            {
                var properties = innerType.GetProperties(BindingFlags.Public | BindingFlags.Static);

                foreach (var property in properties)
                {
                    var value = property.GetValue(null);

                    value.Should().BeOfType<string>();

                    value.Should().Be($"{innerType.Name}.{property.Name}");
                }
            }
        }

        [Fact]
        public void All_Should_ContainsAllPaths()
        {
            var counter = 0;

            var globalType = typeof(MessageKey);

            var innerTypes = globalType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var innerType in innerTypes)
            {
                var properties = innerType.GetProperties(BindingFlags.Public | BindingFlags.Static);

                foreach (var property in properties)
                {
                    var value = property.GetValue(null);
                    value.Should().BeOfType<string>();
                    counter++;

                    MessageKey.All.Should().Contain(value as string);
                }
            }

            MessageKey.All.Should().NotContainNulls();
            MessageKey.All.Should().HaveCount(counter);
        }
    }
}
