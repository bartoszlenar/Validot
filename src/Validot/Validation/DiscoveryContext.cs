namespace Validot.Validation
{
    using System.Collections.Generic;

    using Validot.Errors;
    using Validot.Validation.Stacks;

    internal class DiscoveryContext : IDiscoveryContext, IErrorsHolder
    {
        private readonly IDiscoveryContextActions _actions;

        private readonly List<string> _infiniteReferencesLoopRoots = new List<string>();

        private readonly PathStack _pathsStack = new PathStack();

        private readonly Stack<int> _scopesStack = new Stack<int>();

        public DiscoveryContext(IDiscoveryContextActions actions)
        {
            _actions = actions;
        }

        public Dictionary<string, Dictionary<string, string>> Paths { get; } = new Dictionary<string, Dictionary<string, string>>();

        public Dictionary<string, List<int>> Errors { get; } = new Dictionary<string, List<int>>();

        public IReadOnlyCollection<string> InfiniteReferencesLoopRoots => _infiniteReferencesLoopRoots;

        public void AddError(int errorId, bool skipIfDuplicateInPath = false)
        {
            if (!Errors.ContainsKey(_pathsStack.Path))
            {
                Errors.Add(_pathsStack.Path, new List<int>());
            }

            if (skipIfDuplicateInPath && Errors[_pathsStack.Path].Contains(errorId))
            {
                return;
            }

            Errors[_pathsStack.Path].Add(errorId);
        }

        public void EnterScope<T>(int id)
        {
            if (_scopesStack.Contains(id))
            {
                var circularErrorId = _actions.RegisterError(new CircularDependencyError(typeof(T)));
                AddError(circularErrorId);

                if (!_infiniteReferencesLoopRoots.Contains(_pathsStack.Path))
                {
                    _infiniteReferencesLoopRoots.Add(_pathsStack.Path);
                }

                return;
            }

            _scopesStack.Push(id);

            var scope = _actions.GetDiscoverableSpecificationScope(id);

            scope.Discover(this);
        }

        public void LeavePath()
        {
            _pathsStack.Pop();
        }

        public void EnterCollectionItemPath()
        {
            var path = PathsHelper.ResolveNextLevelPath(_pathsStack.Path, PathsHelper.CollectionIndexPrefixString);

            _pathsStack.PushWithDiscoveryIndex(path);
        }

        public void EnterPath(string name)
        {
            name = name ?? string.Empty;

            var path = PathsHelper.ResolveNextLevelPath(_pathsStack.Path, name);

            if (!Paths.ContainsKey(_pathsStack.Path))
            {
                Paths.Add(_pathsStack.Path, new Dictionary<string, string>());
            }

            Paths[_pathsStack.Path][name] = path;

            _pathsStack.Push(path);
        }
    }
}
