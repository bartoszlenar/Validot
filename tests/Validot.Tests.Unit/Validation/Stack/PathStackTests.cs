namespace Validot.Tests.Unit.Validation.Stack
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Validot.Validation.Stacks;

    using Xunit;

    public class PathStackTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new PathStack();
        }

        [Fact]
        public void Should_Initialize_WithDefaultValues()
        {
            var pathStack = new PathStack();

            pathStack.Path.Should().BeEmpty();
            pathStack.Level.Should().Be(0);
            pathStack.HasIndexes.Should().BeFalse();
            pathStack.IndexesStack.Should().BeEmpty();
        }

        public class Push
        {
            [Fact]
            public void Should_SetPath()
            {
                var pathStack = new PathStack();

                pathStack.Push("name");

                pathStack.Path.Should().Be("name");
                pathStack.HasIndexes.Should().BeFalse();
                pathStack.IndexesStack.Should().BeEmpty();
                pathStack.Level.Should().Be(1);
            }

            [Theory]
            [InlineData(50)]
            [InlineData(200)]
            [InlineData(500)]
            public void Should_SetPath_MultipleTimes(int max)
            {
                var pathStack = new PathStack();

                for (var i = 0; i < max; ++i)
                {
                    pathStack.Push($"path_{i}");
                    pathStack.Path.Should().Be($"path_{i}");
                    pathStack.Level.Should().Be(i + 1);
                    pathStack.HasIndexes.Should().BeFalse();
                    pathStack.IndexesStack.Should().BeEmpty();
                }
            }
        }

        public class PushWithDiscoveryIndex
        {
            [Theory]
            [InlineData(1)]
            [InlineData(5)]
            [InlineData(10)]
            public void Push_Should_IncrementLevel(int count)
            {
                var pathStack = new PathStack();

                for (var i = 0; i < count; ++i)
                {
                    pathStack.Push($"name_{i}");
                }

                pathStack.Level.Should().Be(count);
            }

            [Fact]
            public void Should_SetPath_WithCollectionIndexPrefixAsIndex()
            {
                var pathStack = new PathStack();

                pathStack.PushWithDiscoveryIndex("name");
                pathStack.Path.Should().Be("name");
                pathStack.HasIndexes.Should().BeTrue();
                pathStack.IndexesStack.Should().ContainSingle("#");
                pathStack.Level.Should().Be(1);
            }

            [Theory]
            [InlineData(50)]
            [InlineData(200)]
            [InlineData(500)]
            public void Should_SetPath_WithCollectionIndexPrefixAsIndex_MultipleTimes(int max)
            {
                var pathStack = new PathStack();

                for (var i = 0; i < max; ++i)
                {
                    pathStack.PushWithDiscoveryIndex($"path_{i}");
                    pathStack.Path.Should().Be($"path_{i}");
                    pathStack.Level.Should().Be(i + 1);
                    pathStack.HasIndexes.Should().BeTrue();

                    var indexes = Enumerable.Range(0, i).Select(s => "#").ToArray();

                    pathStack.IndexesStack.Should().ContainInOrder(indexes);
                }
            }
        }

        public class PushWithIndex
        {
            [Theory]
            [InlineData(0)]
            [InlineData(2)]
            [InlineData(100)]
            [InlineData(500)]
            public void Should_SetPath_WithIndex(int index)
            {
                var pathStack = new PathStack();

                pathStack.PushWithIndex("name", index);
                pathStack.Path.Should().Be("name");
                pathStack.HasIndexes.Should().BeTrue();
                pathStack.IndexesStack.Should().ContainSingle($"{index}");
                pathStack.Level.Should().Be(1);
            }

            [Theory]
            [InlineData(50)]
            [InlineData(200)]
            [InlineData(500)]
            public void Should_SetPath_WithCollectionIndexPrefixAsIndex_MultipleTimes(int max)
            {
                var pathStack = new PathStack();

                for (var i = 0; i < max; ++i)
                {
                    pathStack.PushWithIndex($"path_{i}", i);
                    pathStack.Path.Should().Be($"path_{i}");
                    pathStack.Level.Should().Be(i + 1);
                    pathStack.HasIndexes.Should().BeTrue();

                    var tempStack = new Stack<string>(Enumerable.Range(0, i + 1).Select(s => $"{s}").ToArray());

                    pathStack.IndexesStack.Should().ContainInOrder(tempStack);
                }
            }
        }

        [Fact]
        public void Push_Mixed()
        {
            var pathStack = new PathStack();

            pathStack.Push("first");
            pathStack.Path.Should().Be("first");
            pathStack.HasIndexes.Should().BeFalse();
            pathStack.IndexesStack.Should().BeEmpty();
            pathStack.Level.Should().Be(1);

            pathStack.Push("second");
            pathStack.Path.Should().Be("second");
            pathStack.HasIndexes.Should().BeFalse();
            pathStack.IndexesStack.Should().BeEmpty();
            pathStack.Level.Should().Be(2);

            pathStack.PushWithIndex("third", 3);
            pathStack.Path.Should().Be("third");
            pathStack.HasIndexes.Should().BeTrue();
            pathStack.IndexesStack.Should().ContainInOrder("3");
            pathStack.Level.Should().Be(3);

            pathStack.PushWithDiscoveryIndex("fourth");
            pathStack.Path.Should().Be("fourth");
            pathStack.HasIndexes.Should().BeTrue();
            pathStack.IndexesStack.Should().ContainInOrder("#", "3");
            pathStack.Level.Should().Be(4);

            pathStack.PushWithIndex("fifth", 5);
            pathStack.Path.Should().Be("fifth");
            pathStack.HasIndexes.Should().BeTrue();
            pathStack.IndexesStack.Should().ContainInOrder("5", "#", "3");
            pathStack.Level.Should().Be(5);

            pathStack.Push("sixth");
            pathStack.Path.Should().Be("sixth");
            pathStack.HasIndexes.Should().BeTrue();
            pathStack.IndexesStack.Should().ContainInOrder("5", "#", "3");
            pathStack.Level.Should().Be(6);

            pathStack.PushWithDiscoveryIndex("seventh");
            pathStack.Path.Should().Be("seventh");
            pathStack.HasIndexes.Should().BeTrue();
            pathStack.IndexesStack.Should().ContainInOrder("#", "5", "#", "3");
            pathStack.Level.Should().Be(7);
        }

        public class Pop
        {
            [Fact]
            public void Should_GoToPreviousPath()
            {
                var pathStack = new PathStack();

                pathStack.Push("first");
                pathStack.Push("second");
                pathStack.Path.Should().Be("second");

                pathStack.Pop();
                pathStack.Path.Should().Be("first");
            }

            [Theory]
            [InlineData(5, 3, 2)]
            [InlineData(10, 8, 2)]
            public void Should_GoBackToPreviousPath_MultipleTimes(int max, int stepsUp, int finalLevel)
            {
                var pathStack = new PathStack();

                for (var i = 0; i < max; ++i)
                {
                    pathStack.Push($"level_{i + 1}");
                }

                for (var i = 0; i < stepsUp; ++i)
                {
                    pathStack.Pop();
                }

                pathStack.Path.Should().Be($"level_{finalLevel}");
            }

            [Theory]
            [InlineData(1)]
            [InlineData(5)]
            [InlineData(10)]
            public void Should_GoBackToEmptyString_If_ReachedRootLevel(int levels)
            {
                var pathStack = new PathStack();

                for (var i = 0; i < levels; ++i)
                {
                    pathStack.Push($"level_{i}");
                }

                for (var i = 0; i < levels; ++i)
                {
                    pathStack.Pop();
                }

                pathStack.Path.Should().Be("");
            }

            [Theory]
            [InlineData(1)]
            [InlineData(5)]
            [InlineData(10)]
            public void Should_ThrowException_When_ExceededRootLevel(int levels)
            {
                var pathStack = new PathStack();

                for (var i = 0; i < levels; ++i)
                {
                    pathStack.Push($"level_{i}");
                }

                for (var i = 0; i < levels; ++i)
                {
                    pathStack.Pop();
                }

                pathStack.Path.Should().Be("");

                Action action = () =>
                {
                    pathStack.Pop();
                };

                action.Should().ThrowExactly<InvalidOperationException>();
            }

            [Theory]
            [InlineData(1)]
            [InlineData(10)]
            [InlineData(200)]
            [InlineData(500)]
            public void Should_PopIndexes(int levels)
            {
                var pathStack = new PathStack();

                for (var i = 0; i < levels; ++i)
                {
                    if (i % 2 == 0)
                    {
                        pathStack.PushWithIndex($"level_{i}", i);
                    }
                    else
                    {
                        pathStack.PushWithDiscoveryIndex($"level_{i}");
                    }
                }

                var items = Enumerable.Range(0, levels).Select(i => i % 2 == 0 ? $"{i}" : "#").ToArray();

                for (var i = levels - 1; i >= 0; --i)
                {
                    pathStack.Path.Should().Be($"level_{i}");
                    pathStack.Level.Should().Be(i + 1);
                    pathStack.HasIndexes.Should().BeTrue();
                    pathStack.IndexesStack.Should().ContainInOrder(items.Take(i + 1).Reverse());

                    pathStack.Pop();
                }

                pathStack.Path.Should().Be("");
                pathStack.Level.Should().Be(0);
                pathStack.HasIndexes.Should().BeFalse();
            }
        }
    }
}
