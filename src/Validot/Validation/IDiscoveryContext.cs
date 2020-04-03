namespace Validot.Validation
{
    internal interface IDiscoveryContext
    {
        void AddError(int errorId, bool skipIfDuplicateInPath = false);

        void EnterPath(string name);

        void LeavePath();

        void EnterCollectionItemPath();

        void EnterScope<T>(int id);
    }
}
