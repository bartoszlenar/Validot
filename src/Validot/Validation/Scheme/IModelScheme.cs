namespace Validot.Validation.Scheme
{
    using System.Collections.Generic;

    using Validot.Settings.Capacities;
    using Validot.Validation.Scopes;

    internal interface IModelScheme
    {
        ICapacityInfo CapacityInfo { get; }

        bool InfiniteReferencesLoopsDetected { get; }

        int RootSpecificationScopeId { get; }

        ISpecificationScope<T> GetSpecificationScope<T>(int specificationScopeId);

        string GetPathForScope(string path, string nextLevelName);

        string GetPathWithIndexes(string path, IReadOnlyCollection<string> indexesStack);
    }
}
