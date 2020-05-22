namespace Validot.Settings.Capacities
{
    internal class CapacityInfoHelpers : ICapacityInfoHelpers
    {
        public bool ContainsIndexes(string path) => PathHelper.ContainsIndexes(path);

        public string GetWithoutIndexes(string path) => PathHelper.GetWithoutIndexes(path);
    }
}
