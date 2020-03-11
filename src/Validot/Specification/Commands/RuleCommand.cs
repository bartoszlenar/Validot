namespace Validot.Specification.Commands
{
    using System;
    using System.Collections.Generic;

    using Validot.Errors.Args;
    using Validot.Validation.Scopes.Builders;

    internal abstract class RuleCommand : IScopeCommand
    {
        public abstract ICommandScopeBuilder GetScopeBuilder();
    }

    internal class RuleCommand<T> : RuleCommand
    {
        public RuleCommand(Predicate<T> validCondition)
        {
            ThrowHelper.NullArgument(validCondition, nameof(validCondition));

            ValidCondition = validCondition;
        }

        public RuleCommand(Predicate<T> predicate, string message, IReadOnlyList<IArg> args = null)
            : this(predicate)
        {
            ThrowHelper.NullArgument(message, nameof(message));

            if (args != null)
            {
                ThrowHelper.NullInCollection(args, nameof(args));
            }

            Message = message;

            Args = args;
        }

        public Predicate<T> ValidCondition { get; }

        public string Message { get; }

        public IReadOnlyList<IArg> Args { get; }

        public override ICommandScopeBuilder GetScopeBuilder()
        {
            return new RuleCommandScopeBuilder<T>(this);
        }
    }
}
