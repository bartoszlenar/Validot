namespace Validot.Validation.Stacks
{
    using System.Collections.Generic;

    internal class ReferencesStack
    {
        private const int InitPathsCapacity = 4;

        private const int InitScopesCapacity = 2;

        private readonly Dictionary<int, Stack<StackItem>> _dictionary = new Dictionary<int, Stack<StackItem>>(InitPathsCapacity);

        public bool TryPush(int scopeId, string path, object modelReference, out string higherLevelPath)
        {
            if (!_dictionary.ContainsKey(scopeId))
            {
                _dictionary.Add(scopeId, new Stack<StackItem>(InitScopesCapacity));
            }

            if (_dictionary[scopeId].Count > 0)
            {
                foreach (var stackItem in _dictionary[scopeId])
                {
                    if (ReferenceEquals(stackItem.Reference, modelReference))
                    {
                        higherLevelPath = stackItem.Path;

                        return false;
                    }
                }
            }

            var item = new StackItem
            {
                Path = path,
                Reference = modelReference
            };

            _dictionary[scopeId].Push(item);

            higherLevelPath = null;

            return true;
        }

        public object Pop(int scopeId, out string path)
        {
            var item = _dictionary[scopeId].Pop();

            path = item.Path;

            return item.Reference;
        }

        public int GetStoredReferencesCount()
        {
            var count = 0;

            foreach (var pair in _dictionary)
            {
                count += pair.Value.Count;
            }

            return count;
        }

        private class StackItem
        {
            public object Reference { get; set; }

            public string Path { get; set; }
        }
    }
}
