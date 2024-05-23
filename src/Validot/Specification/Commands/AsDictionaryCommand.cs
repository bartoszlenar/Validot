namespace Validot.Specification.Commands
{
    using System;
    using System.Collections.Generic;

    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    internal abstract class AsDictionaryCommand : IScopeCommand
    {
        public abstract ICommandScopeBuilder GetScopeBuilder();
    }

    internal class AsDictionaryCommand<T, TKey, TValue> : AsDictionaryCommand
        where T : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public AsDictionaryCommand(Specification<TValue> specification, Func<TKey, string> keyStringifier)
        {
            ThrowHelper.NullArgument(specification, nameof(specification));

            Specification = specification;

            KeyStringifier = keyStringifier;
        }

        public Specification<TValue> Specification { get; }

        public Func<TKey, string> KeyStringifier { get; }

        public override ICommandScopeBuilder GetScopeBuilder()
        {
            return new CommandScopeBuilder<T>(this, (command, context) =>
            {
                var cmd = (AsDictionaryCommand<T, TKey, TValue>)command;

                var scope = new DictionaryCommandScope<T, TKey, TValue>
                {
                    ScopeId = context.GetOrRegisterSpecificationScope(cmd.Specification),
                    KeyStringifier = cmd.KeyStringifier,
                };

                return scope;
            });
        }
    }
}