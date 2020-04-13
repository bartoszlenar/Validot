namespace Validot.Validation.Stacks
{
    using System;

    public class ReferenceLoopException : ValidotException
    {
        internal ReferenceLoopException(string path, string nestedPath, int scopeId, Type type)
            : base(GetMessage(path, nestedPath, type))
        {
            Path = path;
            NestedPath = nestedPath;
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
                ? "the root path"
                : $"the path {path}";

            return $"Reference loop detected: object of type {type.GetFriendlyName()} is both under {pathStringified} and in the nested path {infiniteLoopNestedPath}";
        }
    }
}
