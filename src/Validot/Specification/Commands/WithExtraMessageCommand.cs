namespace Validot.Specification.Commands
{
    internal class WithExtraMessageCommand : ICommand
    {
        public WithExtraMessageCommand(string message)
        {
            ThrowHelper.NullArgument(message, nameof(message));

            Message = message;
        }

        public string Message { get; }
    }
}
