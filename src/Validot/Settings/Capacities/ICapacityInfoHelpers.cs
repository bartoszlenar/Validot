namespace Validot.Settings.Capacities
{
    public interface ICapacityInfoHelpers
    {
        bool ContainsIndexes(string path);

        string GetWithoutIndexes(string path);
    }
}
