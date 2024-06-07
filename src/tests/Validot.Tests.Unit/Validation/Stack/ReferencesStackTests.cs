namespace Validot.Tests.Unit.Validation.Stack
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Validation.Stacks;

    using Xunit;

    public class ReferencesStackTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new ReferencesStack();
        }

        public class TryPush
        {
            [Fact]
            public void Should_Push_And_ReturnTrue()
            {
                var referencesStack = new ReferencesStack();

                var result = referencesStack.TryPush(10, "some.path", new object(), out var higherLevelPath);

                result.Should().BeTrue();

                higherLevelPath.Should().BeNull();
            }

            [Fact]
            public void Should_Push_And_ReturnTrue_When_DifferentScopeId_And_DifferentObject()
            {
                var referencesStack = new ReferencesStack();

                var result1 = referencesStack.TryPush(10, "some.path", new object(), out var higherLevelPath1);

                var result2 = referencesStack.TryPush(20, "some.path.next", new object(), out var higherLevelPath2);

                result1.Should().BeTrue();
                higherLevelPath1.Should().BeNull();

                result2.Should().BeTrue();
                higherLevelPath2.Should().BeNull();
            }

            [Fact]
            public void Should_Push_And_ReturnTrue_When_DifferentScopeId_And_SameObject()
            {
                var referencesStack = new ReferencesStack();

                var model = new object();

                var result1 = referencesStack.TryPush(10, "some.path", model, out var higherLevelPath1);

                var result2 = referencesStack.TryPush(20, "some.path.next", model, out var higherLevelPath2);

                result1.Should().BeTrue();
                higherLevelPath1.Should().BeNull();

                result2.Should().BeTrue();
                higherLevelPath2.Should().BeNull();
            }

            [Fact]
            public void Should_Push_And_ReturnTrue_When_SameScopeId_And_DifferentObject()
            {
                var referencesStack = new ReferencesStack();

                var result1 = referencesStack.TryPush(10, "some.path", new object(), out var higherLevelPath1);

                var result2 = referencesStack.TryPush(10, "some.path.next", new object(), out var higherLevelPath2);

                result1.Should().BeTrue();
                higherLevelPath1.Should().BeNull();

                result2.Should().BeTrue();
                higherLevelPath2.Should().BeNull();
            }

            [Fact]
            public void Should_Push_And_ReturnFalse_And_PreviousPath_When_SameScopeId_And_SameObject()
            {
                var referencesStack = new ReferencesStack();

                var model = new object();

                var result1 = referencesStack.TryPush(10, "some.path", model, out var higherLevelPath1);

                var result2 = referencesStack.TryPush(10, "some.path.next", model, out var higherLevelPath2);

                result1.Should().BeTrue();
                higherLevelPath1.Should().BeNull();

                result2.Should().BeFalse();
                higherLevelPath2.Should().Be("some.path");
            }

            [Fact]
            public void Should_Push_And_ReturnFalse_And_PreviousPath_When_SameScopeId_And_SameObject_And_MorePushedInBetween()
            {
                var referencesStack = new ReferencesStack();

                var model = new object();

                var result1 = referencesStack.TryPush(10, "some.path", model, out var higherLevelPath1);

                var result2 = referencesStack.TryPush(20, "some.path.next", new object(), out var higherLevelPath2);

                var result3 = referencesStack.TryPush(30, "some.path.next.next", new object(), out var higherLevelPath3);

                var result4 = referencesStack.TryPush(10, "some.path.next.next.next", model, out var higherLevelPath4);

                result1.Should().BeTrue();
                higherLevelPath1.Should().BeNull();

                result2.Should().BeTrue();
                higherLevelPath2.Should().BeNull();

                result3.Should().BeTrue();
                higherLevelPath3.Should().BeNull();

                result4.Should().BeFalse();
                higherLevelPath4.Should().Be("some.path");
            }

            [Fact]
            public void Should_Push_And_ReturnFalse_And_PreviousPath_When_SameScopeId_And_SameObject_And_MorePushedInBetween_AlsoWithSameParameters()
            {
                var referencesStack = new ReferencesStack();

                var model = new object();

                var result1 = referencesStack.TryPush(10, "some.path", model, out var higherLevelPath1);

                var result2 = referencesStack.TryPush(10, "some.path.next", new object(), out var higherLevelPath2);

                var result3 = referencesStack.TryPush(20, "some.path.next.next", model, out var higherLevelPath3);

                var result4 = referencesStack.TryPush(10, "some.path.next.next.next", model, out var higherLevelPath4);

                result1.Should().BeTrue();
                higherLevelPath1.Should().BeNull();

                result2.Should().BeTrue();
                higherLevelPath2.Should().BeNull();

                result3.Should().BeTrue();
                higherLevelPath3.Should().BeNull();

                result4.Should().BeFalse();
                higherLevelPath4.Should().Be("some.path");
            }
        }

        public class Pop
        {
            [Fact]
            public void Should_ThrowException_When_Empty()
            {
                var referencesStack = new ReferencesStack();

                Action action = () => referencesStack.Pop(10, out _);

                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_InvalidKey()
            {
                var referencesStack = new ReferencesStack();

                referencesStack.TryPush(11, "some", new object(), out _);
                referencesStack.TryPush(66, "some.path", new object(), out _);

                Action action = () => referencesStack.Pop(10, out _);

                action.Should().ThrowExactly<KeyNotFoundException>();
            }

            [Fact]
            public void Should_ThrowException_When_Pop_FromEmptyStack()
            {
                var referencesStack = new ReferencesStack();

                referencesStack.TryPush(10, "some", new object(), out _);

                _ = referencesStack.Pop(10, out _);

                Action action = () => referencesStack.Pop(10, out _);

                action.Should().ThrowExactly<InvalidOperationException>();
            }

            [Fact]
            public void Should_ThrowException_When_Pop_FromEmptyStack_AfterMultipleOperations()
            {
                var referencesStack = new ReferencesStack();

                referencesStack.TryPush(10, "some", new object(), out _);
                referencesStack.TryPush(10, "some", new object(), out _);
                referencesStack.TryPush(10, "some", new object(), out _);

                _ = referencesStack.Pop(10, out _);
                _ = referencesStack.Pop(10, out _);
                _ = referencesStack.Pop(10, out _);

                Action action = () => referencesStack.Pop(10, out _);

                action.Should().ThrowExactly<InvalidOperationException>();
            }

            [Fact]
            public void Should_Pop()
            {
                var referencesStack = new ReferencesStack();

                var model = new object();

                referencesStack.TryPush(10, "some", model, out _);

                var poppedModel = referencesStack.Pop(10, out var path);

                poppedModel.Should().BeSameAs(model);
                path.Should().Be("some");
            }

            [Fact]
            public void Should_Pop_FromScopeId()
            {
                var referencesStack = new ReferencesStack();

                var model = new object();

                referencesStack.TryPush(10, "some", model, out _);
                referencesStack.TryPush(11, "some", new object(), out _);

                var poppedModel = referencesStack.Pop(10, out var path);

                poppedModel.Should().BeSameAs(model);
                path.Should().Be("some");
            }

            [Fact]
            public void Should_Pop_FromScopeId_AfterMultipleOperations()
            {
                var referencesStack = new ReferencesStack();

                var model1 = new object();
                var model2 = new object();
                var model3 = new object();

                referencesStack.TryPush(10, "some", model1, out _);
                referencesStack.TryPush(10, "some.path", model2, out _);
                referencesStack.TryPush(10, "some.path.next", model3, out _);

                var poppedModel3 = referencesStack.Pop(10, out var path3);
                var poppedModel2 = referencesStack.Pop(10, out var path2);
                var poppedModel1 = referencesStack.Pop(10, out var path1);

                poppedModel1.Should().BeSameAs(model1);
                path1.Should().Be("some");

                poppedModel2.Should().BeSameAs(model2);
                path2.Should().Be("some.path");

                poppedModel3.Should().BeSameAs(model3);
                path3.Should().Be("some.path.next");
            }

            [Fact]
            public void Should_Pop_LastSuccessfulPushed()
            {
                var referencesStack = new ReferencesStack();

                var model1 = new object();
                var model2 = new object();

                referencesStack.TryPush(10, "some", model1, out _);
                referencesStack.TryPush(10, "some.path", model2, out _);
                referencesStack.TryPush(10, "some.path.next", model1, out _);

                var poppedModel2 = referencesStack.Pop(10, out var path2);
                var poppedModel1 = referencesStack.Pop(10, out var path1);

                Action action = () => referencesStack.Pop(10, out _);

                action.Should().ThrowExactly<InvalidOperationException>();

                poppedModel1.Should().BeSameAs(model1);
                path1.Should().Be("some");

                poppedModel2.Should().BeSameAs(model2);
                path2.Should().Be("some.path");
            }
        }

        public class GetStoredReferencesCount
        {
            [Fact]
            public void Should_BeZero_When_Empty()
            {
                var referencesStack = new ReferencesStack();

                referencesStack.GetStoredReferencesCount().Should().Be(0);
            }

            [Fact]
            public void Should_BeOne_When_SingleReferenceAdded()
            {
                var referencesStack = new ReferencesStack();

                referencesStack.TryPush(10, "some.path", new object(), out _);

                referencesStack.GetStoredReferencesCount().Should().Be(1);
            }

            [Fact]
            public void Should_Increment_When_PushingItems()
            {
                var referencesStack = new ReferencesStack();

                referencesStack.TryPush(10, "some.0", new object(), out _);
                referencesStack.TryPush(11, "some.1", new object(), out _);
                referencesStack.TryPush(12, "some.2", new object(), out _);
                referencesStack.TryPush(13, "some.3", new object(), out _);

                referencesStack.GetStoredReferencesCount().Should().Be(4);
            }

            [Fact]
            public void Should_Increment_When_TryPushReturnsTrue()
            {
                var referencesStack = new ReferencesStack();

                var model = new object();

                referencesStack.GetStoredReferencesCount().Should().Be(0);

                referencesStack.TryPush(10, "some.0", model, out _);

                referencesStack.GetStoredReferencesCount().Should().Be(1);

                referencesStack.TryPush(11, "some.1", new object(), out _);

                referencesStack.GetStoredReferencesCount().Should().Be(2);

                referencesStack.TryPush(12, "some.2", new object(), out _);

                referencesStack.GetStoredReferencesCount().Should().Be(3);

                referencesStack.TryPush(13, "some.3", new object(), out _);

                referencesStack.GetStoredReferencesCount().Should().Be(4);

                referencesStack.TryPush(10, "some.4", model, out _);

                referencesStack.GetStoredReferencesCount().Should().Be(4);

                referencesStack.TryPush(10, "some.5", model, out _);

                referencesStack.GetStoredReferencesCount().Should().Be(4);

                referencesStack.TryPush(14, "some.6", model, out _);

                referencesStack.GetStoredReferencesCount().Should().Be(5);

                referencesStack.TryPush(15, "some.7", new object(), out _);

                referencesStack.GetStoredReferencesCount().Should().Be(6);
            }

            [Fact]
            public void Should_Decrement_When_PoppingItems()
            {
                var referencesStack = new ReferencesStack();

                referencesStack.TryPush(10, "some.0", new object(), out _);
                referencesStack.TryPush(11, "some.1", new object(), out _);
                referencesStack.TryPush(12, "some.2", new object(), out _);
                referencesStack.TryPush(13, "some.3", new object(), out _);

                referencesStack.GetStoredReferencesCount().Should().Be(4);

                referencesStack.Pop(10, out _);
                referencesStack.GetStoredReferencesCount().Should().Be(3);

                referencesStack.Pop(11, out _);
                referencesStack.GetStoredReferencesCount().Should().Be(2);

                referencesStack.Pop(12, out _);
                referencesStack.GetStoredReferencesCount().Should().Be(1);

                referencesStack.Pop(13, out _);
                referencesStack.GetStoredReferencesCount().Should().Be(0);
            }
        }
    }
}
