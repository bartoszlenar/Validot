namespace Validot.Validation.Scheme
{
    using System;
    using System.Collections.Generic;

    using Validot.Errors;
    using Validot.Settings.Capacities;
    using Validot.Validation.Scopes;

    internal class ModelScheme<T> : IModelScheme
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _pathsMap;

        private readonly IReadOnlyDictionary<int, object> _specificationScopes;

        public ModelScheme(IReadOnlyDictionary<int, object> specificationScopes, int rootSpecificationScopeId, IReadOnlyDictionary<int, IError> errorsRegistry, IReadOnlyDictionary<string, IReadOnlyList<int>> errorsMap, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> pathsMap, ICapacityInfo capacityInfo, bool isReferenceLoopPossible)
        {
            ThrowHelper.NullArgument(specificationScopes, nameof(specificationScopes));
            ThrowHelper.NullInCollection(specificationScopes.Values, $"{nameof(specificationScopes)}.{nameof(specificationScopes.Values)}");
            _specificationScopes = specificationScopes;

            if (!_specificationScopes.ContainsKey(rootSpecificationScopeId))
            {
                throw new ArgumentException($"{nameof(specificationScopes)} doesn't contain specification scope with id {rootSpecificationScopeId} ({nameof(rootSpecificationScopeId)})");
            }

            if (!(_specificationScopes[rootSpecificationScopeId] is SpecificationScope<T>))
            {
                throw new ArgumentException($"specification scope with id {rootSpecificationScopeId} ({nameof(rootSpecificationScopeId)}) is not of type {typeof(SpecificationScope<T>).FullName}");
            }

            RootSpecificationScope = (SpecificationScope<T>)specificationScopes[rootSpecificationScopeId];

            ThrowHelper.NullArgument(errorsRegistry, nameof(errorsRegistry));
            ThrowHelper.NullInCollection(errorsRegistry.Values, $"{nameof(errorsRegistry)}.{nameof(errorsRegistry.Values)}");
            ErrorsRegistry = errorsRegistry;

            ThrowHelper.NullArgument(errorsMap, nameof(errorsMap));
            ThrowHelper.NullInCollection(errorsMap.Values, $"{nameof(errorsMap)}.{nameof(errorsMap.Values)}");
            ErrorsMap = errorsMap;

            ThrowHelper.NullArgument(pathsMap, nameof(pathsMap));
            ThrowHelper.NullInCollection(pathsMap.Values, $"{nameof(pathsMap)}.{nameof(pathsMap.Values)}");

            foreach (var item in pathsMap.Values)
            {
                foreach (var innerItem in item)
                {
                    if (innerItem.Value == null)
                    {
                        throw new ArgumentNullException($"Collection `{nameof(pathsMap)}` contains null in inner dictionary under key `{innerItem.Key}`");
                    }
                }
            }

            _pathsMap = pathsMap;

            ThrowHelper.NullArgument(capacityInfo, nameof(capacityInfo));
            CapacityInfo = capacityInfo;
            IsReferenceLoopPossible = isReferenceLoopPossible;
            RootModelType = typeof(T);
            RootSpecificationScopeId = rootSpecificationScopeId;
        }

        public IReadOnlyDictionary<int, IError> ErrorsRegistry { get; }

        public IReadOnlyDictionary<string, IReadOnlyList<int>> ErrorsMap { get; }

        public SpecificationScope<T> RootSpecificationScope { get; }

        public bool IsReferenceLoopPossible { get; }

        public int RootSpecificationScopeId { get; }

        public Type RootModelType { get; }

        public ICapacityInfo CapacityInfo { get; }

        public string GetPathForScope(string path, string nextLevelName)
        {
            if (_pathsMap.ContainsKey(path) && _pathsMap[path].ContainsKey(nextLevelName))
            {
                return _pathsMap[path][nextLevelName];
            }

            return PathsHelper.ResolveNextLevelPath(path, nextLevelName);
        }

        public ISpecificationScope<TModel> GetSpecificationScope<TModel>(int specificationScopeId)
        {
            return (SpecificationScope<TModel>)_specificationScopes[specificationScopeId];
        }

        public string GetPathWithIndexes(string path, IReadOnlyCollection<string> indexesStack)
        {
            return PathsHelper.GetWithIndexes(path, indexesStack);
        }
    }
}
