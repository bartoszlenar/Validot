namespace Validot.Errors.Args;

public sealed class ArgPlaceholder
{
    public string? Placeholder { get; set; }

    public string? Name { get; set; }

    public IReadOnlyDictionary<string, string>? Parameters { get; set; }
}
