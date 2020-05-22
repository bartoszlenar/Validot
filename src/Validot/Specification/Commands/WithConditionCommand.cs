namespace Validot.Specification.Commands
{
    using System;

    internal abstract class WithConditionCommand : ICommand
    {
    }

    internal class WithConditionCommand<T> : WithConditionCommand
    {
        public WithConditionCommand(Predicate<T> executionCondition)
        {
            ThrowHelper.NullArgument(executionCondition, nameof(executionCondition));

            ExecutionCondition = executionCondition;
        }

        public Predicate<T> ExecutionCondition { get; }
    }
}
