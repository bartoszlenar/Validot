namespace Validot.Validation
{
    using System;

    using Validot.Validation.Scheme;
    using Validot.Validation.Scopes.Builders;
    using Validot.Validation.Stacks;

    internal class IsValidValidationContext : IValidationContext
    {
        private readonly bool _referencesLoopProtectionEnabled;

        private readonly ReferencesStack _referencesStack;

        private readonly IModelScheme _modelScheme;

        public bool ErrorFound { get; private set; }

        public bool ShouldFallBack => ErrorFound;

        public IsValidValidationContext(IModelScheme modelScheme, ReferenceLoopProtectionSettings referenceLoopProtectionSettings)
        {
            _modelScheme = modelScheme;

            ReferenceLoopProtectionSettings = referenceLoopProtectionSettings;

            if (!(referenceLoopProtectionSettings is null))
            {
                _referencesLoopProtectionEnabled = true;

                _referencesStack = new ReferencesStack();

                if (_modelScheme.RootModelType.IsClass && !(referenceLoopProtectionSettings.RootModelReference is null))
                {
                    _referencesStack.TryPush(_modelScheme.RootSpecificationScopeId, string.Empty, referenceLoopProtectionSettings.RootModelReference, out _);
                }
            }
        }

        public ReferenceLoopProtectionSettings ReferenceLoopProtectionSettings { get; }

        public void AddError(int errorId, bool skipIfDuplicateInPath = false)
        {
            ErrorFound = true;
        }

        public void EnterPath(string path)
        {
        }

        public void LeavePath()
        {
        }

        public void EnableErrorDetectionMode(ErrorMode errorMode, int errorId)
        {
        }

        public void EnterCollectionItemPath(int i)
        {
        }

        public void EnterScope<T>(int scopeId, T model)
        {
            var useReferenceLoopProtection = typeof(T).IsClass && _referencesLoopProtectionEnabled;

            if (useReferenceLoopProtection && !_referencesStack.TryPush(scopeId, string.Empty, model, out _))
            {
                FailWithException(scopeId, typeof(T));

                return;
            }

            var specificationScope = _modelScheme.GetSpecificationScope<T>(scopeId);

            specificationScope.Validate(model, this);

            if (useReferenceLoopProtection)
            {
                _referencesStack.Pop(scopeId, out _);
            }
        }

        public int? GetLoopProtectionReferencesStackCount()
        {
            return _referencesStack?.GetStoredReferencesCount();
        }

        private void FailWithException(int scopeId, Type type)
        {
            throw new ReferenceLoopException(scopeId, type);
        }
    }
}
