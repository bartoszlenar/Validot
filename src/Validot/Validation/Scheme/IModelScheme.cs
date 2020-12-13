namespace Validot.Validation.Scheme
{
    using System;
    using System.Collections.Generic;

    using Validot.Errors;
    using Validot.Validation.Scopes;

    internal interface IModelScheme
    {
        int RootSpecificationScopeId { get; }

        Type RootModelType { get; }

        ISpecificationScope<T> GetSpecificationScope<T>(int specificationScopeId);

        string ResolvePath(string basePath, string relativePath);

        string GetPathWithIndexes(string path, IReadOnlyCollection<string> indexesStack);
    }

    internal interface IModelScheme<T> : IModelScheme
    {
        bool IsReferenceLoopPossible { get; }

        ISpecificationScope<T> RootSpecificationScope { get; }

        IReadOnlyDictionary<string, IReadOnlyList<int>> Template { get; }

        IReadOnlyDictionary<int, IError> ErrorRegistry { get; }
    }
}
