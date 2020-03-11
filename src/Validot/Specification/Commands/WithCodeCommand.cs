namespace Validot.Specification.Commands
{
    internal class WithCodeCommand : ICommand
    {
        public WithCodeCommand(string code)
        {
            ThrowHelper.NullArgument(code, nameof(code));

            Code = code;
        }

        public string Code { get; }
    }
}
