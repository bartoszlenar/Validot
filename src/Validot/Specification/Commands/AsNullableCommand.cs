namespace Validot.Specification.Commands
{
    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    internal abstract class AsNullableCommand : IScopeCommand
    {
        public abstract ICommandScopeBuilder GetScopeBuilder();
    }

    internal class AsNullableCommand<T> : AsNullableCommand
        where T : struct
    {
        public AsNullableCommand(Specification<T> specification)
        {
            ThrowHelper.NullArgument(specification, nameof(specification));

            Specification = specification;
        }

        public Specification<T> Specification { get; }

        public override ICommandScopeBuilder GetScopeBuilder()
        {
            return new CommandScopeBuilder<T?>(this, (command, context) =>
            {
                var cmd = (AsNullableCommand<T>)command;

                var block = new NullableCommandScope<T>
                {
                    ScopeId = context.GetOrRegisterSpecificationScope(cmd.Specification)
                };

                return block;
            });
        }
    }
}
