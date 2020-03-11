namespace Validot.Specification.Commands
{
    using Validot.Validation.Scopes.Builders;

    internal interface IScopeCommand : ICommand
    {
        ICommandScopeBuilder GetScopeBuilder();
    }
}
