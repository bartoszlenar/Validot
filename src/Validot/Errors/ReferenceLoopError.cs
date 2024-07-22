namespace Validot.Errors;

using Validot.Errors.Args;
using Validot.Translations;

internal class ReferenceLoopError(Type type) : IError
{
    public IReadOnlyList<string> Messages { get; } = new[]
        {
            MessageKey.Global.ReferenceLoop,
        };

    public IReadOnlyList<string> Codes { get; } = Array.Empty<string>();

    public IReadOnlyList<IArg> Args { get; } = new[]
        {
            Arg.Type("type", type),
        };
}
