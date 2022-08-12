namespace Validot.Specification.Commands
{
    using System;

    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    internal abstract class AsConvertedCommand : IScopeCommand
    {
        public abstract ICommandScopeBuilder GetScopeBuilder();
    }

    internal class AsConvertedCommand<T, TTarget> : AsConvertedCommand
    {
        public AsConvertedCommand(Converter<T, TTarget> converter, Specification<TTarget> specification)
        {
            ThrowHelper.NullArgument(converter, nameof(converter));

            ThrowHelper.NullArgument(specification, nameof(specification));

            Converter = converter;

            Specification = specification;
        }

        public Converter<T, TTarget> Converter { get; }

        public Specification<TTarget> Specification { get; }

        public override ICommandScopeBuilder GetScopeBuilder()
        {
            return new CommandScopeBuilder<T>(this, (command, context) =>
            {
                var cmd = (AsConvertedCommand<T, TTarget>)command;

                var scope = new ConvertedCommandScope<T, TTarget>()
                {
                    ScopeId = context.GetOrRegisterSpecificationScope(cmd.Specification),
                    Converter = Converter,
                };

                return scope;
            });
        }
    }
}