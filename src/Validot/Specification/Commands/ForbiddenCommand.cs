namespace Validot.Specification.Commands
{
    using System;

    internal class ForbiddenCommand : ICommand
    {
        private static readonly Lazy<ForbiddenCommand> LazyInstance = new Lazy<ForbiddenCommand>(() => new ForbiddenCommand(), true);

        public static ForbiddenCommand Instance => LazyInstance.Value;
    }
}
