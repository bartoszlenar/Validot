namespace Validot.Errors.Args;

// todo: nice immutable
public sealed class ArgPlaceholder
{
    public string Placeholder { get; init; } = null!;

    public string Name { get; init; } = null!;

    public IReadOnlyDictionary<string, string> Parameters { get; init; } = null!;
}
