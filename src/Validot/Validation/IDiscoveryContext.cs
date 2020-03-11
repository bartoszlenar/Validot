namespace Validot.Validation
{
    internal interface IDiscoveryContext
    {
        void AddError(int errorId);

        void EnterPath(string name);

        void LeavePath();

        void EnterCollectionItemPath();

        void EnterScope<T>(int id);
    }
}
