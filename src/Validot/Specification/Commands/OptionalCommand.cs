namespace Validot.Specification.Commands
{
    using System;

    internal class OptionalCommand : ICommand
    {
        private static readonly Lazy<OptionalCommand> LazyInstance = new Lazy<OptionalCommand>(() => new OptionalCommand(), true);

        public static OptionalCommand Instance => LazyInstance.Value;
    }
}
