namespace Validot.Errors.Translator
{
    using System.Collections.Generic;

    using Validot.Errors.Args;

    internal class TranslationResult
    {
        public IReadOnlyList<string> Messages { get; set; }

        public IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>> IndexedPathPlaceholders { get; set; }

        public bool AnyPathPlaceholders => IndexedPathPlaceholders != null && IndexedPathPlaceholders.Count > 0;
    }
}
