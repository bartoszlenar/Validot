namespace Validot.Tests.Unit.Specification
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    using Xunit;

    public class AsCollectionExtensionTests
    {
        public class IEnumerableCollection
        {
            [Fact]
            public void Should_Add_AsCollectionCommand()
            {
                Specification<object> specification = s => s;

                ApiTester.TestSingleCommand<IEnumerable<object>, IRuleIn<IEnumerable<object>>, IRuleOut<IEnumerable<object>>, AsCollectionCommand<IEnumerable<object>, object>>(
                    s => s.AsCollection(specification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(specification);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IEnumerable<object>, IRuleIn<IEnumerable<object>>, IRuleOut<IEnumerable<object>>>(
                    s => s.AsCollection(null),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        public class ArrayCollection
        {
            [Fact]
            public void Should_Add_AsCollectionCommand()
            {
                Specification<object> specification = s => s;

                ApiTester.TestSingleCommand<object[], IRuleIn<object[]>, IRuleOut<object[]>, AsCollectionCommand<object[], object>>(
                    s => s.AsCollection(specification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(specification);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<object[], IRuleIn<object[]>, IRuleOut<object[]>>(
                    s => s.AsCollection(null),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        public class ICollectionCollection
        {
            [Fact]
            public void Should_Add_AsCollectionCommand()
            {
                Specification<object> specification = s => s;

                ApiTester.TestSingleCommand<ICollection<object>, IRuleIn<ICollection<object>>, IRuleOut<ICollection<object>>, AsCollectionCommand<ICollection<object>, object>>(
                    s => s.AsCollection(specification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(specification);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<ICollection<object>, IRuleIn<ICollection<object>>, IRuleOut<ICollection<object>>>(
                    s => s.AsCollection(null),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        public class IReadOnlyCollectionCollection
        {
            [Fact]
            public void Should_Add_AsCollectionCommand()
            {
                Specification<object> specification = s => s;

                ApiTester.TestSingleCommand<IReadOnlyCollection<object>, IRuleIn<IReadOnlyCollection<object>>, IRuleOut<IReadOnlyCollection<object>>, AsCollectionCommand<IReadOnlyCollection<object>, object>>(
                    s => s.AsCollection(specification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(specification);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IReadOnlyCollection<object>, IRuleIn<IReadOnlyCollection<object>>, IRuleOut<IReadOnlyCollection<object>>>(
                    s => s.AsCollection(null),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        public class IListCollection
        {
            [Fact]
            public void Should_Add_AsCollectionCommand()
            {
                Specification<object> specification = s => s;

                ApiTester.TestSingleCommand<IList<object>, IRuleIn<IList<object>>, IRuleOut<IList<object>>, AsCollectionCommand<IList<object>, object>>(
                    s => s.AsCollection(specification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(specification);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IList<object>, IRuleIn<IList<object>>, IRuleOut<IList<object>>>(
                    s => s.AsCollection(null),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        public class IReadOnlyListCollection
        {
            [Fact]
            public void Should_Add_AsCollectionCommand()
            {
                Specification<object> specification = s => s;

                ApiTester.TestSingleCommand<IReadOnlyList<object>, IRuleIn<IReadOnlyList<object>>, IRuleOut<IReadOnlyList<object>>, AsCollectionCommand<IReadOnlyList<object>, object>>(
                    s => s.AsCollection(specification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(specification);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<IReadOnlyList<object>, IRuleIn<IReadOnlyList<object>>, IRuleOut<IReadOnlyList<object>>>(
                    s => s.AsCollection(null),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        public class ListCollection
        {
            [Fact]
            public void Should_Add_AsCollectionCommand()
            {
                Specification<object> specification = s => s;

                ApiTester.TestSingleCommand<List<object>, IRuleIn<List<object>>, IRuleOut<List<object>>, AsCollectionCommand<List<object>, object>>(
                    s => s.AsCollection(specification),
                    command =>
                    {
                        command.Specification.Should().NotBeNull();
                        command.Specification.Should().BeSameAs(specification);
                    });
            }

            [Fact]
            public void Should_ThrowException_When_NullSpecification()
            {
                ApiTester.TextException<List<object>, IRuleIn<List<object>>, IRuleOut<List<object>>>(
                    s => s.AsCollection(null),
                    addingAction =>
                    {
                        addingAction.Should().ThrowExactly<ArgumentNullException>();
                    });
            }
        }

        [Fact]
        public void Should_Add_AsCollectionCommand()
        {
            Specification<object> specification = s => s;

            ApiTester.TestSingleCommand<IEnumerable<object>, IRuleIn<IEnumerable<object>>, IRuleOut<IEnumerable<object>>, AsCollectionCommand<IEnumerable<object>, object>>(
                s => s.AsCollection<IEnumerable<object>, object>(specification),
                command =>
                {
                    command.Specification.Should().NotBeNull();
                    command.Specification.Should().BeSameAs(specification);
                });
        }

        [Fact]
        public void Should_ThrowException_When_NullSpecification()
        {
            ApiTester.TextException<IEnumerable<object>, IRuleIn<IEnumerable<object>>, IRuleOut<IEnumerable<object>>>(
                s => s.AsCollection<IEnumerable<object>, object>(null),
                addingAction =>
                {
                    addingAction.Should().ThrowExactly<ArgumentNullException>();
                });
        }
    }
}
