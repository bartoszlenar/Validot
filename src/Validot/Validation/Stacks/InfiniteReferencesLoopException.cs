namespace Validot.Validation.Stacks
{
    using System;

    public class InfiniteReferencesLoopException : ValidotException
    {
        internal InfiniteReferencesLoopException(string path, string infiniteLoopNestedPath, int scopeId, Type type)
            : base(GetMessage(path, infiniteLoopNestedPath, type))
        {
            Path = path;
            Type = type;
            ScopeId = scopeId;
            InfiniteLoopNestedPath = infiniteLoopNestedPath;
        }

        public int ScopeId { get; }

        public Type Type { get; }

        public string Path { get; }

        public string InfiniteLoopNestedPath { get; }

        private static string GetMessage(string path, string infiniteLoopNestedPath, Type type)
        {
            var pathStringified = string.IsNullOrEmpty(path)
                ? "the root path"
                : $"the path {path}";

            return $"Infinite references loop detected: object of type {type.GetFriendlyName()} is both under {pathStringified} and in the nested path {infiniteLoopNestedPath}";
        }
    }
}
