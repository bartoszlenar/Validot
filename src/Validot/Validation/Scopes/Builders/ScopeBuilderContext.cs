namespace Validot.Validation.Scopes.Builders
{
    using System;
    using System.Collections.Generic;

    using Validot.Errors;
    using Validot.Errors.Args;
    using Validot.Translations;

    internal class ScopeBuilderContext : IScopeBuilderContext, IDiscoveryContextActions
    {
        private static readonly ScopeBuilder ScopeBuilder = new ScopeBuilder();

        private readonly Dictionary<int, IError> _errors = new Dictionary<int, IError>();

        private readonly Dictionary<int, object> _scopes = new Dictionary<int, object>();

        private readonly Dictionary<int, object> _specifications = new Dictionary<int, object>();

        private readonly Dictionary<int, Type> _types = new Dictionary<int, Type>();

        public ScopeBuilderContext()
        {
            DefaultErrorId = RegisterError(new Error
            {
                Messages = new[]
                {
                    MessageKey.Global.Error
                },
                Codes = Array.Empty<string>(),
                Args = Array.Empty<IArg>()
            });

            ForbiddenErrorId = RegisterError(new Error
            {
                Messages = new[]
                {
                    MessageKey.Global.Forbidden
                },
                Codes = Array.Empty<string>(),
                Args = Array.Empty<IArg>()
            });

            RequiredErrorId = RegisterError(new Error
            {
                Messages = new[]
                {
                    MessageKey.Global.Required
                },
                Codes = Array.Empty<string>(),
                Args = Array.Empty<IArg>()
            });
        }

        public IReadOnlyDictionary<int, IError> Errors => _errors;

        public IReadOnlyDictionary<int, object> Scopes => _scopes;

        public IReadOnlyDictionary<int, Type> Types => _types;

        public int DefaultErrorId { get; }

        public int ForbiddenErrorId { get; }

        public int RequiredErrorId { get; }

        public IDiscoverable GetDiscoverableSpecificationScope(int id) => (IDiscoverable)_scopes[id];

        public int RegisterError(IError error)
        {
            ThrowHelper.NullArgument(error, nameof(error));

            var id = _errors.Count;
            _errors.Add(id, error);

            return id;
        }

        public int GetOrRegisterSpecificationScope<T>(Specification<T> specification)
        {
            ThrowHelper.NullArgument(specification, nameof(specification));

            foreach (var pair in _specifications)
            {
                if (ReferenceEquals(pair.Value, specification))
                {
                    return pair.Key;
                }
            }

            var id = _specifications.Count;
            _specifications.Add(id, specification);
            _types.Add(id, typeof(T));

            var scope = ScopeBuilder.Build(specification, this);
            _scopes.Add(id, scope);

            return id;
        }
    }
}
