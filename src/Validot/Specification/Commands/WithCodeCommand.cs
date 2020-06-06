namespace Validot.Specification.Commands
{
    using System;

    internal class WithCodeCommand : ICommand
    {
        public WithCodeCommand(string code)
        {
            ThrowHelper.NullArgument(code, nameof(code));

            if (!CodeHelper.IsCodeValid(code))
            {
                throw new ArgumentException($"Invalid code: {code}", nameof(code));
            }

            Code = code;
        }

        public string Code { get; }
    }
}
