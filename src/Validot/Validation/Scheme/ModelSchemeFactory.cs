namespace Validot.Validation.Scheme;

using System.Collections.Generic;
using System.Linq;

using Validot.Validation.Scopes;
using Validot.Validation.Scopes.Builders;

internal static class ModelSchemeFactory
{
    public static IModelScheme<T> Create<T>(Specification<T> specification)
    {
        ThrowHelper.NullArgument(specification, nameof(specification));

        var scopeBuilderContext = new ScopeBuilderContext();

        var rootSpecificationScopeId = scopeBuilderContext.GetOrRegisterSpecificationScope(specification);

        var discoveryContext = new DiscoveryContext(scopeBuilderContext, rootSpecificationScopeId);

        var rootSpecificationScope = (SpecificationScope<T>)scopeBuilderContext.Scopes[rootSpecificationScopeId];

        rootSpecificationScope.Discover(discoveryContext);

        return new ModelScheme<T>(
            scopeBuilderContext.Scopes,
            rootSpecificationScopeId,
            scopeBuilderContext.Errors,
            discoveryContext.Errors.ToDictionary(p => p.Key, p => (IReadOnlyList<int>)p.Value),
            discoveryContext.Paths.ToDictionary(p => p.Key, p => (IReadOnlyDictionary<string, string>)p.Value),
            discoveryContext.ReferenceLoopRoots.Count > 0);
    }
}
