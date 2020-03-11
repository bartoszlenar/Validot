namespace Validot.Validation.Scopes.Builders
{
    using Validot.Specification.Commands;

    internal interface ICommandScopeBuilder
    {
        ICommandScope Build(IScopeBuilderContext context);

        bool TryAdd(ICommand command);
    }
}
