namespace Validot.Validation.Stacks
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    internal class PathStack
    {
        private const int DefaultCollectionsCapacity = 30;

        private const int DefaultPreallocatedIndexes = 200;

        private static readonly IReadOnlyDictionary<int, string> PreallocatedIndexes = Enumerable.Range(0, DefaultPreallocatedIndexes).ToDictionary(i => i, i => i.ToString(CultureInfo.InvariantCulture));

        private readonly Stack<int> _indexesLevelsStack = new Stack<int>(DefaultCollectionsCapacity);

        private readonly Stack<string> _indexesStack = new Stack<string>(DefaultCollectionsCapacity);

        private readonly Stack<string> _stack = new Stack<string>(DefaultCollectionsCapacity);

        public string Path { get; private set; } = string.Empty;

        public int Level => _stack.Count;

        public bool HasIndexes => _indexesStack?.Count > 0;

        public IReadOnlyCollection<string> IndexesStack => _indexesStack;

        public void Push(string path)
        {
            Path = path;

            _stack.Push(path);
        }

        public void PushWithIndex(string path, int index)
        {
            if (index < DefaultPreallocatedIndexes)
            {
                PushWithIndex(path, PreallocatedIndexes[index]);
            }
            else
            {
                PushWithIndex(path, index.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void PushWithDiscoveryIndex(string path)
        {
            PushWithIndex(path, PathsHelper.CollectionIndexPrefixString);
        }

        public void Pop()
        {
            if (HasIndexes && _indexesLevelsStack.Peek() == Level)
            {
                _indexesStack.Pop();
                _indexesLevelsStack.Pop();
            }

            _stack.Pop();

            Path = _stack.Count > 0 ? _stack.Peek() : string.Empty;
        }

        private void PushWithIndex(string path, string index)
        {
            Push(path);

            _indexesStack.Push(index);
            _indexesLevelsStack.Push(Level);
        }
    }
}
