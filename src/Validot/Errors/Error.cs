namespace Validot.Errors;

using Validot.Errors.Args;

// todo: nice immutable
internal class Error : IError
{
    public IReadOnlyList<string> Messages { get; set; } = null!;

    public IReadOnlyList<string> Codes { get; set; } = null!;

    public IReadOnlyList<IArg> Args { get; set; } = null!;
}
