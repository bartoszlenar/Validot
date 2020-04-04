namespace Validot.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Validation.Scheme;
    using Validot.Validation.Scopes.Builders;
    using Validot.Validation.Stacks;

    internal class ValidationContext : IValidationContext, IErrorsHolder
    {
        private readonly ErrorFlag _appendingErrorFlag = new ErrorFlag(10);

        private readonly IModelScheme _modelScheme;

        private readonly ErrorFlag _overridingErrorFlag = new ErrorFlag(10);

        private readonly PathStack _pathStack = new PathStack();

        private readonly ReferencesStack _referencesStack;

        public ValidationContext(IModelScheme modelScheme, bool failFast = false, bool infiniteReferencesLoopProtectionEnabled = false)
        {
            _modelScheme = modelScheme;

            FailFast = failFast;

            InfiniteReferencesLoopProtectionEnabled = modelScheme.InfiniteReferencesLoopsDetected && infiniteReferencesLoopProtectionEnabled;

            if (InfiniteReferencesLoopProtectionEnabled)
            {
                _referencesStack = new ReferencesStack();
            }
        }

        public bool InfiniteReferencesLoopProtectionEnabled { get; }

        public bool FailFast { get; }

        public Dictionary<string, List<int>> Errors { get; private set; }

        public bool ShouldFallBack => (FailFast && !(Errors is null) && Errors.Count > 0) || _overridingErrorFlag.IsDetectedAtAnylevel;

        public void AddError(int errorId, bool skipIfDuplicateInPath = false)
        {
            if (_overridingErrorFlag.IsEnabledAtAnyLevel)
            {
                _overridingErrorFlag.SetDetected(_pathStack.Level);

                return;
            }

            _appendingErrorFlag.SetDetected(_pathStack.Level);

            SaveError(errorId, skipIfDuplicateInPath);
        }

        public void EnableErrorDetectionMode(ErrorMode errorMode, int errorId)
        {
            if (_overridingErrorFlag.IsEnabledAtAnyLevel)
            {
                return;
            }

            var flag = errorMode == ErrorMode.Append
                ? _appendingErrorFlag
                : _overridingErrorFlag;

            flag.SetEnabled(_pathStack.Level, errorId);
        }

        public void LeavePath()
        {
            if (_overridingErrorFlag.LeaveLevelAndTryGetError(_pathStack.Level, out var overridingErrorId))
            {
                SaveError(overridingErrorId, false);
            }

            if (_appendingErrorFlag.LeaveLevelAndTryGetError(_pathStack.Level, out var appendingErrorId))
            {
                SaveError(appendingErrorId, false);
            }

            _pathStack.Pop();
        }

        public void EnterCollectionItemPath(int i)
        {
            var newPath = _modelScheme.GetPathForScope(_pathStack.Path, PathsHelper.CollectionIndexPrefixString);

            _pathStack.PushWithIndex(newPath, i);
        }

        public void EnterPath(string name)
        {
            var newPath = name != null
                ? _modelScheme.GetPathForScope(_pathStack.Path, name)
                : _pathStack.Path;

            _pathStack.Push(newPath);
        }

        public void EnterScope<T>(int scopeId, T model)
        {
            var infiniteReferencesLoopProtectionEnabled = typeof(T).IsClass && InfiniteReferencesLoopProtectionEnabled;

            if (infiniteReferencesLoopProtectionEnabled && !_referencesStack.TryPush(scopeId, _pathStack.Path, model, out var higherLevelPath))
            {
                FailWithException(higherLevelPath, scopeId, typeof(T));

                return;
            }

            var specificationScope = _modelScheme.GetSpecificationScope<T>(scopeId);

            specificationScope.Validate(model, this);

            if (infiniteReferencesLoopProtectionEnabled)
            {
                _referencesStack.Pop(scopeId, out _);
            }
        }

        public int? GetInfiniteReferencesLoopProtectionStackCount()
        {
            return _referencesStack?.GetStoredReferencesCount();
        }

        private string GetCurrentPath()
        {
            return _pathStack.HasIndexes
                ? _modelScheme.GetPathWithIndexes(_pathStack.Path, _pathStack.IndexesStack)
                : _pathStack.Path;
        }

        private void FailWithException(string higherLevelPath, int scopeId, Type type)
        {
            if (_pathStack.HasIndexes)
            {
                var higherLevelPathIndexesAmount = PathsHelper.GetIndexesAmount(higherLevelPath);

                if (higherLevelPathIndexesAmount > 0)
                {
                    var stack = higherLevelPathIndexesAmount == _pathStack.IndexesStack.Count
                        ? _pathStack.IndexesStack
                        : _pathStack.IndexesStack.Skip(_pathStack.IndexesStack.Count - higherLevelPathIndexesAmount).ToList();

                    higherLevelPath = _modelScheme.GetPathWithIndexes(higherLevelPath, stack);
                }
            }

            throw new InfiniteReferencesLoopException(higherLevelPath, GetCurrentPath(), scopeId, type);
        }

        private void SaveError(int errorId, bool skipIfDuplicateInPath)
        {
            var shouldUseCapacityInfo = !FailFast && _modelScheme.CapacityInfo.ShouldRead;

            if (Errors is null)
            {
                Errors = shouldUseCapacityInfo
                    ? new Dictionary<string, List<int>>(_modelScheme.CapacityInfo.ErrorsPathsCapacity)
                    : new Dictionary<string, List<int>>(1);
            }

            var currentPath = GetCurrentPath();

            if (!Errors.ContainsKey(currentPath))
            {
                var errors = shouldUseCapacityInfo && _modelScheme.CapacityInfo.TryGetErrorsCapacityForPath(currentPath, out var capacity)
                    ? new List<int>(capacity)
                    : new List<int>(1);

                Errors.Add(currentPath, errors);
            }

            if (skipIfDuplicateInPath && Errors[currentPath].Contains(errorId))
            {
                return;
            }

            Errors[currentPath].Add(errorId);
        }
    }
}
