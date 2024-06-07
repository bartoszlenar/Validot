namespace Validot.Tests.Unit.Specification
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class AsDictionaryWithKeyStringExtensionTests
    {
        public class IEnumerable
        {
            [Fact]
            public void Should_Add_DictionaryCommand()
            {
                Specification<object> valueSpecification = s => s;

                ApiTester.TestSingleCommand<IEnumerable<KeyValuePair<string, object>>, IRuleIn<IEnumerable<KeyValuePair<string, object>>>, IRuleOut<IEnumerable<KeyValuePair<string, object>>>, AsDictionaryCommand<IEnumerable<KeyValuePair<string, object>>, string, object>>(
                    s => s.AsDictionary(valueSpecification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(valueSpecification);

                        command.KeyStringifier.Should().BeNull();
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IEnumerable<KeyValuePair<string, object>>, IRuleIn<IEnumerable<KeyValuePair<string, object>>>, IRuleOut<IEnumerable<KeyValuePair<string, object>>>>(
                    s => s.AsDictionary<IEnumerable<KeyValuePair<string, object>>, string, object>(null, i => i.ToString(CultureInfo.InvariantCulture)),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        public class IReadOnlyCollection
        {
            [Fact]
            public void Should_Add_DictionaryCommand()
            {
                Specification<object> valueSpecification = s => s;

                ApiTester.TestSingleCommand<IReadOnlyCollection<KeyValuePair<string, object>>, IRuleIn<IReadOnlyCollection<KeyValuePair<string, object>>>, IRuleOut<IReadOnlyCollection<KeyValuePair<string, object>>>, AsDictionaryCommand<IReadOnlyCollection<KeyValuePair<string, object>>, string, object>>(
                    s => s.AsDictionary(valueSpecification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(valueSpecification);

                        command.KeyStringifier.Should().BeNull();
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IReadOnlyCollection<KeyValuePair<string, object>>, IRuleIn<IReadOnlyCollection<KeyValuePair<string, object>>>, IRuleOut<IReadOnlyCollection<KeyValuePair<string, object>>>>(
                    s => s.AsDictionary<IReadOnlyCollection<KeyValuePair<string, object>>, string, object>(null, i => i.ToString(CultureInfo.InvariantCulture)),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        public class Dictionary
        {
            [Fact]
            public void Should_Add_DictionaryCommand()
            {
                Specification<object> valueSpecification = s => s;

                ApiTester.TestSingleCommand<Dictionary<string, object>, IRuleIn<Dictionary<string, object>>, IRuleOut<Dictionary<string, object>>, AsDictionaryCommand<Dictionary<string, object>, string, object>>(
                    s => s.AsDictionary(valueSpecification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(valueSpecification);

                        command.KeyStringifier.Should().BeNull();
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<Dictionary<string, object>, IRuleIn<Dictionary<string, object>>, IRuleOut<Dictionary<string, object>>>(
                    s => s.AsDictionary<Dictionary<string, object>, string, object>(null, i => i.ToString(CultureInfo.InvariantCulture)),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        public class IDictionary
        {
            [Fact]
            public void Should_Add_DictionaryCommand()
            {
                Specification<object> valueSpecification = s => s;

                ApiTester.TestSingleCommand<IDictionary<string, object>, IRuleIn<IDictionary<string, object>>, IRuleOut<IDictionary<string, object>>, AsDictionaryCommand<IDictionary<string, object>, string, object>>(
                    s => s.AsDictionary(valueSpecification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(valueSpecification);

                        command.KeyStringifier.Should().BeNull();
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IDictionary<string, object>, IRuleIn<IDictionary<string, object>>, IRuleOut<IDictionary<string, object>>>(
                    s => s.AsDictionary<IDictionary<string, object>, string, object>(null, i => i.ToString(CultureInfo.InvariantCulture)),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        public class IReadOnlyDictionary
        {
            [Fact]
            public void Should_Add_DictionaryCommand()
            {
                Specification<object> valueSpecification = s => s;

                ApiTester.TestSingleCommand<IReadOnlyDictionary<string, object>, IRuleIn<IReadOnlyDictionary<string, object>>, IRuleOut<IReadOnlyDictionary<string, object>>, AsDictionaryCommand<IReadOnlyDictionary<string, object>, string, object>>(
                    s => s.AsDictionary(valueSpecification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(valueSpecification);

                        command.KeyStringifier.Should().BeNull();
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IReadOnlyDictionary<string, object>, IRuleIn<IReadOnlyDictionary<string, object>>, IRuleOut<IReadOnlyDictionary<string, object>>>(
                    s => s.AsDictionary<IReadOnlyDictionary<string, object>, string, object>(null, i => i.ToString(CultureInfo.InvariantCulture)),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }
    }
}