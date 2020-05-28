namespace Validot.Settings.Capacities
{
    using System.Collections.Generic;

    using Validot.Validation;

    public sealed class MaxCapacityInfo : IFeedableCapacityInfo, ICapacityInfoHelpersConsumer
    {
        private IReadOnlyDictionary<string, int> _maxCapacities;

        private ICapacityInfoHelpers _helpers;

        public bool ShouldRead => true;

        public int ErrorsPathsCapacity { get; private set; }

        public bool ShouldFeed { get; private set; } = true;

        public bool TryGetErrorsCapacityForPath(string path, out int capacity)
        {
            var indexlessPath = _helpers.ContainsIndexes(path)
                ? _helpers.GetWithoutIndexes(path)
                : path;

            if (_maxCapacities is null ||
                !_maxCapacities.ContainsKey(indexlessPath))
            {
                capacity = -1;

                return false;
            }

            capacity = _maxCapacities[path];
            return true;
        }

        public void Feed(IErrorsHolder errorsHolder)
        {
            if (!ShouldFeed)
            {
                return;
            }

            ThrowHelper.NullArgument(errorsHolder, nameof(errorsHolder));

            var maxCapacities = new Dictionary<string, int>(errorsHolder.Errors.Count);

            foreach (var pair in errorsHolder.Errors)
            {
                maxCapacities.Add(pair.Key, errorsHolder.Errors[pair.Key].Count);
            }

            _maxCapacities = maxCapacities;

            ErrorsPathsCapacity = errorsHolder.Errors.Count;

            ShouldFeed = false;
        }

        public void InjectHelpers(ICapacityInfoHelpers helpers)
        {
            _helpers = helpers;
        }
    }
}
