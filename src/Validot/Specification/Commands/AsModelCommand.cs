namespace Validot.Specification.Commands
{
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    internal abstract class AsModelCommand : IScopeCommand
    {
        public abstract ICommandScopeBuilder GetScopeBuilder();
    }

    internal class AsModelCommand<T> : AsModelCommand
    {
        public AsModelCommand(Specification<T> specification)
        {
            ThrowHelper.NullArgument(specification, nameof(specification));

            Specification = specification;
        }

        public Specification<T> Specification { get; }

        public override ICommandScopeBuilder GetScopeBuilder()
        {
            return new CommandScopeBuilder<T>(this, (command, context) =>
            {
                var cmd = (AsModelCommand<T>)command;

                var block = new ModelCommandScope<T>
                {
                    ScopeId = context.GetOrRegisterSpecificationScope(cmd.Specification)
                };

                return block;
            });
        }
    }
}
