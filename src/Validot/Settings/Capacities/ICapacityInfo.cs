namespace Validot.Settings.Capacities
{
    public interface ICapacityInfo
    {
        bool ShouldRead { get; }

        int ErrorsPathsCapacity { get; }

        bool TryGetErrorsCapacityForPath(string path, out int capacity);
    }
}
