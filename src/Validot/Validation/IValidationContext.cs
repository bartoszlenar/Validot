namespace Validot.Validation
{
    using Validot.Validation.Scopes.Builders;

    internal interface IValidationContext
    {
        bool ShouldFallBack { get; }

        void AddError(int errorId, bool skipIfDuplicateInPath = false);

        void EnterPath(string path);

        void LeavePath();

        void EnableErrorDetectionMode(ErrorMode errorMode, int errorId);

        void EnterCollectionItemPath(int i);

        void EnterScope<T>(int scopeId, T model);
    }
}
