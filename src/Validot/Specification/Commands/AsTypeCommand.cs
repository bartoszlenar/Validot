namespace Validot.Specification.Commands
{
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    internal abstract class AsTypeCommand : IScopeCommand
    {
        public abstract ICommandScopeBuilder GetScopeBuilder();
    }

    internal class AsTypeCommand<T, TType> : AsTypeCommand
    {
        public AsTypeCommand(Specification<TType> specification)
        {
            ThrowHelper.NullArgument(specification, nameof(specification));

            Specification = specification;
        }

        public Specification<TType> Specification { get; }

        public override ICommandScopeBuilder GetScopeBuilder()
        {
            return new CommandScopeBuilder<T>(this, (command, context) =>
            {
                var cmd = (AsTypeCommand<T, TType>)command;

                var scope = new TypeCommandScope<T, TType>()
                {
                    ScopeId = context.GetOrRegisterSpecificationScope(cmd.Specification),
                };

                return scope;
            });
        }
    }
}
