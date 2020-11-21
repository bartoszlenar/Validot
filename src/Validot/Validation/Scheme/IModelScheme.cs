namespace Validot.Validation.Scheme
{
    using System;
    using System.Collections.Generic;

    using Validot.Validation.Scopes;

    internal interface IModelScheme
    {
        bool IsReferenceLoopPossible { get; }

        int RootSpecificationScopeId { get; }

        Type RootModelType { get; }

        ISpecificationScope<T> GetSpecificationScope<T>(int specificationScopeId);

        string ResolvePath(string basePath, string relativePath);

        string GetPathWithIndexes(string path, IReadOnlyCollection<string> indexesStack);
    }
}
