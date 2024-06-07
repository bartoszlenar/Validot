namespace Validot.Tests.Unit.Specification
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class AsDictionaryExtensionTests
    {
        public class IEnumerable
        {
            [Fact]
            public void Should_Add_DictionaryCommand()
            {
                Func<int, string> keyStringifier = i => i.ToString(CultureInfo.InvariantCulture);

                Specification<object> valueSpecification = s => s;

                ApiTester.TestSingleCommand<IEnumerable<KeyValuePair<int, object>>, IRuleIn<IEnumerable<KeyValuePair<int, object>>>, IRuleOut<IEnumerable<KeyValuePair<int, object>>>, AsDictionaryCommand<IEnumerable<KeyValuePair<int, object>>, int, object>>(
                    s => s.AsDictionary(valueSpecification, keyStringifier),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(valueSpecification);

                        command.KeyStringifier.Should().NotBeNull();
                        command.KeyStringifier.Should().BeSameAs(keyStringifier);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IEnumerable<KeyValuePair<int, object>>, IRuleIn<IEnumerable<KeyValuePair<int, object>>>, IRuleOut<IEnumerable<KeyValuePair<int, object>>>>(
                    s => s.AsDictionary<IEnumerable<KeyValuePair<int, object>>, int, object>(null, i => i.ToString(CultureInfo.InvariantCulture)),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullStringifier()
            {
                ApiTester.TextException<IEnumerable<KeyValuePair<int, object>>, IRuleIn<IEnumerable<KeyValuePair<int, object>>>, IRuleOut<IEnumerable<KeyValuePair<int, object>>>>(
                    s => s.AsDictionary<IEnumerable<KeyValuePair<int, object>>, int, object>(s1 => s1, null),
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
                Func<int, string> keyStringifier = i => i.ToString(CultureInfo.InvariantCulture);

                Specification<object> valueSpecification = s => s;

                ApiTester.TestSingleCommand<IReadOnlyCollection<KeyValuePair<int, object>>, IRuleIn<IReadOnlyCollection<KeyValuePair<int, object>>>, IRuleOut<IReadOnlyCollection<KeyValuePair<int, object>>>, AsDictionaryCommand<IReadOnlyCollection<KeyValuePair<int, object>>, int, object>>(
                    s => s.AsDictionary(valueSpecification, keyStringifier),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(valueSpecification);

                        command.KeyStringifier.Should().NotBeNull();
                        command.KeyStringifier.Should().BeSameAs(keyStringifier);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IReadOnlyCollection<KeyValuePair<int, object>>, IRuleIn<IReadOnlyCollection<KeyValuePair<int, object>>>, IRuleOut<IReadOnlyCollection<KeyValuePair<int, object>>>>(
                    s => s.AsDictionary<IReadOnlyCollection<KeyValuePair<int, object>>, int, object>(null, i => i.ToString(CultureInfo.InvariantCulture)),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullStringifier()
            {
                ApiTester.TextException<IReadOnlyCollection<KeyValuePair<int, object>>, IRuleIn<IReadOnlyCollection<KeyValuePair<int, object>>>, IRuleOut<IReadOnlyCollection<KeyValuePair<int, object>>>>(
                    s => s.AsDictionary<IReadOnlyCollection<KeyValuePair<int, object>>, int, object>(s1 => s1, null),
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
                Func<int, string> keyStringifier = i => i.ToString(CultureInfo.InvariantCulture);

                Specification<object> valueSpecification = s => s;

                ApiTester.TestSingleCommand<Dictionary<int, object>, IRuleIn<Dictionary<int, object>>, IRuleOut<Dictionary<int, object>>, AsDictionaryCommand<Dictionary<int, object>, int, object>>(
                    s => s.AsDictionary(valueSpecification, keyStringifier),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(valueSpecification);

                        command.KeyStringifier.Should().NotBeNull();
                        command.KeyStringifier.Should().BeSameAs(keyStringifier);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<Dictionary<int, object>, IRuleIn<Dictionary<int, object>>, IRuleOut<Dictionary<int, object>>>(
                    s => s.AsDictionary<Dictionary<int, object>, int, object>(null, i => i.ToString(CultureInfo.InvariantCulture)),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullStringifier()
            {
                ApiTester.TextException<Dictionary<int, object>, IRuleIn<Dictionary<int, object>>, IRuleOut<Dictionary<int, object>>>(
                    s => s.AsDictionary<Dictionary<int, object>, int, object>(s1 => s1, null),
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
                Func<int, string> keyStringifier = i => i.ToString(CultureInfo.InvariantCulture);

                Specification<object> valueSpecification = s => s;

                ApiTester.TestSingleCommand<IDictionary<int, object>, IRuleIn<IDictionary<int, object>>, IRuleOut<IDictionary<int, object>>, AsDictionaryCommand<IDictionary<int, object>, int, object>>(
                    s => s.AsDictionary(valueSpecification, keyStringifier),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(valueSpecification);

                        command.KeyStringifier.Should().NotBeNull();
                        command.KeyStringifier.Should().BeSameAs(keyStringifier);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IDictionary<int, object>, IRuleIn<IDictionary<int, object>>, IRuleOut<IDictionary<int, object>>>(
                    s => s.AsDictionary<IDictionary<int, object>, int, object>(null, i => i.ToString(CultureInfo.InvariantCulture)),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullStringifier()
            {
                ApiTester.TextException<IDictionary<int, object>, IRuleIn<IDictionary<int, object>>, IRuleOut<IDictionary<int, object>>>(
                    s => s.AsDictionary<IDictionary<int, object>, int, object>(s1 => s1, null),
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
                Func<int, string> keyStringifier = i => i.ToString(CultureInfo.InvariantCulture);

                Specification<object> valueSpecification = s => s;

                ApiTester.TestSingleCommand<IReadOnlyDictionary<int, object>, IRuleIn<IReadOnlyDictionary<int, object>>, IRuleOut<IReadOnlyDictionary<int, object>>, AsDictionaryCommand<IReadOnlyDictionary<int, object>, int, object>>(
                    s => s.AsDictionary(valueSpecification, keyStringifier),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(valueSpecification);

                        command.KeyStringifier.Should().NotBeNull();
                        command.KeyStringifier.Should().BeSameAs(keyStringifier);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IReadOnlyDictionary<int, object>, IRuleIn<IReadOnlyDictionary<int, object>>, IRuleOut<IReadOnlyDictionary<int, object>>>(
                    s => s.AsDictionary<IReadOnlyDictionary<int, object>, int, object>(null, i => i.ToString(CultureInfo.InvariantCulture)),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullStringifier()
            {
                ApiTester.TextException<IReadOnlyDictionary<int, object>, IRuleIn<IReadOnlyDictionary<int, object>>, IRuleOut<IReadOnlyDictionary<int, object>>>(
                    s => s.AsDictionary<IReadOnlyDictionary<int, object>, int, object>(s1 => s1, null),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }
    }
}