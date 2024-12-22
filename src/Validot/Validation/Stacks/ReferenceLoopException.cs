namespace Validot.Validation.Stacks;

using System;

public sealed class ReferenceLoopException : ValidotException
{
    internal ReferenceLoopException(string path, string nestedPath, int scopeId, Type type)
        : base(GetMessage(path, nestedPath, type))
    {
        Path = path;
        NestedPath = nestedPath;
        Type = type;
        ScopeId = scopeId;
    }

    internal ReferenceLoopException(int scopeId, Type type)
        : base($"Reference loop detected: object of type {type.GetFriendlyName()} has been detected twice in the reference graph, effectively creating the infinite references loop (where exactly, that information is not available - is that validation comes from IsValid method, please repeat it using the Validate method and examine the exception thrown)")
    {
        Path = null;
        NestedPath = null;
        Type = type;
        ScopeId = scopeId;
    }

    public int ScopeId { get; }

    public Type Type { get; }

    public string Path { get; }

    public string NestedPath { get; }

    private static string GetMessage(string path, string infiniteLoopNestedPath, Type type)
    {
        var pathStringified = string.IsNullOrEmpty(path)
            ? "the root path, so the validated object itself,"
            : $"the path '{path}'";

        return $"Reference loop detected: object of type {type.GetFriendlyName()} has been detected twice in the reference graph, effectively creating an infinite references loop (at first under {pathStringified} and then under the nested path '{infiniteLoopNestedPath}')";
    }
}
