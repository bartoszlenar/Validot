namespace Validot.Specification.Commands
{
    using System.Collections.Generic;

    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    internal abstract class AsCollectionCommand : IScopeCommand
    {
        public abstract ICommandScopeBuilder GetScopeBuilder();
    }

    internal class AsCollectionCommand<T, TItem> : AsCollectionCommand
        where T : IEnumerable<TItem>
    {
        public AsCollectionCommand(Specification<TItem> specification)
        {
            ThrowHelper.NullArgument(specification, nameof(specification));

            Specification = specification;
        }

        public Specification<TItem> Specification { get; }

        public override ICommandScopeBuilder GetScopeBuilder()
        {
            return new CommandScopeBuilder<T>(this, (command, context) =>
            {
                var cmd = (AsCollectionCommand<T, TItem>)command;

                var block = new CollectionCommandScope<T, TItem>
                {
                    ScopeId = context.GetOrRegisterSpecificationScope(cmd.Specification)
                };

                return block;
            });
        }
    }
}
