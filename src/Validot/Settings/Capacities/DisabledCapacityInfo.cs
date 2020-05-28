namespace Validot.Settings.Capacities
{
    using System;

    public sealed class DisabledCapacityInfo : ICapacityInfo
    {
        public bool ShouldRead => false;

        public int ErrorsPathsCapacity => throw new InvalidOperationException($"{nameof(ErrorsPathsCapacity)} is unavailable in {nameof(DisabledCapacityInfo)}");

        public bool TryGetErrorsCapacityForPath(string path, out int capacity) => throw new InvalidOperationException($"{nameof(TryGetErrorsCapacityForPath)} is unavailable in {nameof(DisabledCapacityInfo)}");
    }
}
