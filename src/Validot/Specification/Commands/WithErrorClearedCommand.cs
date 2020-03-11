namespace Validot.Specification.Commands
{
    using System;

    internal class WithErrorClearedCommand : ICommand
    {
        private static readonly Lazy<WithErrorClearedCommand> LazyInstance = new Lazy<WithErrorClearedCommand>(() => new WithErrorClearedCommand(), true);

        public static WithErrorClearedCommand Instance => LazyInstance.Value;
    }
}
