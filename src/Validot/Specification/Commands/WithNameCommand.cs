namespace Validot.Specification.Commands
{
    using System;

    internal class WithNameCommand : ICommand
    {
        public WithNameCommand(string name)
        {
            ThrowHelper.NullArgument(name, nameof(name));

            if (!PathsHelper.IsValidAsName(name))
            {
                throw new ArgumentException("Invalid name", nameof(name));
            }

            Name = name;
        }

        public string Name { get; }
    }
}
