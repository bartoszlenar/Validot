namespace Validot.Settings.Capacities
{
    internal class CapacityInfoHelpers : ICapacityInfoHelpers
    {
        public bool ContainsIndexes(string path) => PathsHelper.ContainsIndexes(path);

        public string GetWithoutIndexes(string path) => PathsHelper.GetWithoutIndexes(path);
    }
}
