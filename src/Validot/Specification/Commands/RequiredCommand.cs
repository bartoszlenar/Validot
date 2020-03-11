namespace Validot.Specification.Commands
{
    using System;

    internal class RequiredCommand : ICommand
    {
        private static readonly Lazy<RequiredCommand> LazyInstance = new Lazy<RequiredCommand>(() => new RequiredCommand(), true);

        public static RequiredCommand Instance => LazyInstance.Value;
    }
}
