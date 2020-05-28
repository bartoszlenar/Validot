namespace Validot.Translations
{
    using System.Collections.Generic;
    using System.Linq;

    internal class TranslationCompiler : ITranslationCompiler
    {
        private readonly Dictionary<string, Dictionary<string, string>> _translations = new Dictionary<string, Dictionary<string, string>>();

        private IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _cachedResults;

        private bool _changed = true;

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations
        {
            get
            {
                if (_changed)
                {
                    _cachedResults = _translations.ToDictionary(p => p.Key, p => (IReadOnlyDictionary<string, string>)p.Value);
                    _changed = false;
                }

                return _cachedResults;
            }
        }

        public void Add(string name, string messageKey, string translation)
        {
            ThrowHelper.NullArgument(name, nameof(name));
            ThrowHelper.NullArgument(messageKey, nameof(messageKey));
            ThrowHelper.NullArgument(translation, nameof(translation));

            _changed = true;

            if (!_translations.ContainsKey(name))
            {
                _translations.Add(name, new Dictionary<string, string>
                {
                    [messageKey] = translation
                });

                return;
            }

            _translations[name][messageKey] = translation;
        }
    }
}
