namespace Validot.Specification.Commands
{
    internal class WithMessageCommand : ICommand
    {
        public WithMessageCommand(string message)
        {
            ThrowHelper.NullArgument(message, nameof(message));

            Message = message;
        }

        public string Message { get; }
    }
}
