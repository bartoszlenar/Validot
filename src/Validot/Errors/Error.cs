namespace Validot.Errors
{
    using System.Collections.Generic;

    using Validot.Errors.Args;

    internal class Error : IError
    {
        public IReadOnlyList<string> Messages { get; set; }

        public IReadOnlyList<string> Codes { get; set; }

        public IReadOnlyList<IArg> Args { get; set; }
    }
}
