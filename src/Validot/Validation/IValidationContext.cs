namespace Validot.Validation
{
    using Validot.Validation.Scopes.Builders;

    internal interface IValidationContext
    {
        bool ShouldFallBack { get; }

        void AddError(int errorId);

        void EnterPath(string name);

        void LeavePath();

        void EnableErrorDetectionMode(ErrorMode errorMode, int errorId);

        void EnterCollectionItemPath(int i);

        void EnterScope<T>(int scopeId, T model);
    }
}
