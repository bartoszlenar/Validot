namespace Validot.Errors.Translator;

using Validot.Errors.Args;

internal class TranslationResult
{
    public required IReadOnlyList<string> Messages { get; init; }

    public required IReadOnlyDictionary<int, IReadOnlyList<ArgPlaceholder>> IndexedPathPlaceholders { get; init; }

    public bool AnyPathPlaceholders => IndexedPathPlaceholders.Count > 0;
}
