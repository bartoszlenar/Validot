namespace Validot.Errors
{
    using System.Collections.Generic;

    using Validot.Errors.Args;

    public interface IError
    {
        IReadOnlyList<string> Messages { get; }

        IReadOnlyList<string> Codes { get; }

        IReadOnlyList<IArg> Args { get; }
    }
}
