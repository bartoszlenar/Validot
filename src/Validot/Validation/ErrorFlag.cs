namespace Validot.Validation
{
    using System.Collections.Generic;

    internal class ErrorFlag
    {
        private readonly Dictionary<int, bool> _detectionForLevels;

        private readonly Dictionary<int, int> _errorsForLevels;

        public ErrorFlag(int capacity = 0)
        {
            _detectionForLevels = new Dictionary<int, bool>(capacity);
            _errorsForLevels = new Dictionary<int, int>(capacity);
        }

        public bool IsEnabledAtAnyLevel => _errorsForLevels.Count > 0;

        public bool IsDetectedAtAnylevel { get; private set; }

        public void SetEnabled(int level, int errorId)
        {
            if (_errorsForLevels.ContainsKey(level))
            {
                return;
            }

            _errorsForLevels.Add(level, errorId);
            _detectionForLevels.Add(level, false);
        }

        public void SetDetected(int level)
        {
            if (!IsEnabledAtAnyLevel)
            {
                return;
            }

            foreach (var enabledLevel in _errorsForLevels.Keys)
            {
                if (enabledLevel <= level)
                {
                    _detectionForLevels[enabledLevel] = true;
                    IsDetectedAtAnylevel = true;
                }
            }
        }

        public bool LeaveLevelAndTryGetError(int level, out int errorId)
        {
            if (_errorsForLevels.TryGetValue(level, out errorId))
            {
                var detected = _detectionForLevels[level];

                _errorsForLevels.Remove(level);
                _detectionForLevels.Remove(level);

                IsDetectedAtAnylevel = _detectionForLevels.Count > 0 && _detectionForLevels.ContainsValue(true);

                if (detected)
                {
                    return true;
                }
            }

            errorId = -1;

            return false;
        }
    }
}
