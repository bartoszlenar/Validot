namespace Validot.Specification.Commands
{
    internal class WithExtraCodeCommand : ICommand
    {
        public WithExtraCodeCommand(string code)
        {
            ThrowHelper.NullArgument(code, nameof(code));

            Code = code;
        }

        public string Code { get; }
    }
}
