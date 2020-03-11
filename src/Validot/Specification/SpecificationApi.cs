namespace Validot.Specification
{
    using System.Collections.Generic;

    using Validot.Specification.Commands;

    public partial interface ISpecificationIn<T> : ISpecificationOut<T>
    {
    }

    public interface ISpecificationOut<T>
    {
    }

    internal partial class SpecificationApi<T> : ISpecificationIn<T>
    {
        private readonly List<ICommand> _commands = new List<ICommand>();

        public IReadOnlyList<ICommand> Commands => _commands;

        public SpecificationApi<T> AddCommand(ICommand command)
        {
            ThrowHelper.NullArgument(command, nameof(command));

            _commands.Add(command);

            return this;
        }
    }
}
