namespace Validot.Validation.Scheme
{
    using System;
    using System.Collections.Generic;

    using Validot.Errors;
    using Validot.Validation.Scopes;

    internal class ModelScheme<T> : IModelScheme
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _pathMap;

        private readonly IReadOnlyDictionary<int, object> _specificationScopes;

        public ModelScheme(IReadOnlyDictionary<int, object> specificationScopes, int rootSpecificationScopeId, IReadOnlyDictionary<int, IError> errorRegistry, IReadOnlyDictionary<string, IReadOnlyList<int>> template, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> pathMap, bool isReferenceLoopPossible)
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

            ThrowHelper.NullArgument(errorRegistry, nameof(errorRegistry));
            ThrowHelper.NullInCollection(errorRegistry.Values, $"{nameof(errorRegistry)}.{nameof(errorRegistry.Values)}");
            ErrorRegistry = errorRegistry;

            ThrowHelper.NullArgument(template, nameof(template));
            ThrowHelper.NullInCollection(template.Values, $"{nameof(template)}.{nameof(template.Values)}");
            Template = template;

            ThrowHelper.NullArgument(pathMap, nameof(pathMap));
            ThrowHelper.NullInCollection(pathMap.Values, $"{nameof(pathMap)}.{nameof(pathMap.Values)}");

            foreach (var item in pathMap.Values)
            {
                foreach (var innerItem in item)
                {
                    if (innerItem.Value == null)
                    {
                        throw new ArgumentNullException($"Collection `{nameof(pathMap)}` contains null in inner dictionary under key `{innerItem.Key}`");
                    }
                }
            }

            _pathMap = pathMap;

            IsReferenceLoopPossible = isReferenceLoopPossible;
            RootModelType = typeof(T);
            RootSpecificationScopeId = rootSpecificationScopeId;
        }

        public IReadOnlyDictionary<int, IError> ErrorRegistry { get; }

        public IReadOnlyDictionary<string, IReadOnlyList<int>> Template { get; }

        public SpecificationScope<T> RootSpecificationScope { get; }

        public bool IsReferenceLoopPossible { get; }

        public int RootSpecificationScopeId { get; }

        public Type RootModelType { get; }

        public string ResolvePath(string basePath, string relativePath)
        {
            if (_pathMap.ContainsKey(basePath) && _pathMap[basePath].ContainsKey(relativePath))
            {
                return _pathMap[basePath][relativePath];
            }

            return PathHelper.ResolvePath(basePath, relativePath);
        }

        public ISpecificationScope<TModel> GetSpecificationScope<TModel>(int specificationScopeId)
        {
            return (SpecificationScope<TModel>)_specificationScopes[specificationScopeId];
        }

        public string GetPathWithIndexes(string path, IReadOnlyCollection<string> indexesStack)
        {
            return PathHelper.GetWithIndexes(path, indexesStack);
        }
    }
}
