namespace Validot.Specification.Commands
{
    using System;

    internal abstract class WhenCommand : ICommand
    {
    }

    internal class WhenCommand<T> : WhenCommand
    {
        public WhenCommand(Predicate<T> executionCondition)
        {
            ThrowHelper.NullArgument(executionCondition, nameof(executionCondition));

            ExecutionCondition = executionCondition;
        }

        public Predicate<T> ExecutionCondition { get; }
    }
}
