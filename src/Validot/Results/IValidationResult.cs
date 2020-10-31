namespace Validot.Results
{
    using System.Collections.Generic;

    /// <summary>
    /// Object that contains full information about the errors found during the validation process.
    /// WARNING! This object is internally coupled with the instance of <see cref="IValidator{T}"/> that created it.
    /// It's safe to use its members to get the information you want and process them further, but don't cache the instance of <see cref="IValidationResult"/> itself or pass it around your system too much.
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether errors were detected during the validation process.
        /// </summary>
        bool AnyErrors { get; }

        /// <summary>
        /// Gets all the paths with errors detected during the validation process.
        /// If the collection doesn't contain certain path, it means that no errors were detected for it.
        /// </summary>
        IReadOnlyCollection<string> Paths { get; }

        /// <summary>
        /// Gets collection (without duplicates) of all the error codes saved during the validation process.
        /// </summary>
        IReadOnlyCollection<string> Codes { get; }

        /// <summary>
        /// Gets code map - a dictionary that links error codes with their paths.
        /// The key is the path, and the value is the list of error codes saved for this path during the validation process.
        /// If the dictionary doesn't contain certain path, it means that no error codes were saved for this path.
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyList<string>> CodeMap { get; }

        /// <summary>
        /// Gets message map - a dictionary that links error messages with their paths.
        /// The key is the path, and the value is the list of error messages saved for this path during the validation process.
        /// If the dictionary doesn't contain certain path, it means that no error messages were saved for this path.
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyList<string>> MessageMap { get; }

        /// <summary>
        /// Gets list of translation names that can be used to translate error messages using <see cref="GetTranslatedMessageMap"/> method.
        /// Translation names are the same as in the <see cref="Validator{T}"/> that produces this instance of <see cref="IValidationResult"/>.
        /// </summary>
        IReadOnlyList<string> TranslationNames { get; }

        /// <summary>
        /// Gets message map - a dictionary that links error messages with their paths.
        /// This is the same content as the one received in <see cref="MessageMap"/>, but the error messages are translated using the given translation name.
        /// </summary>
        /// <param name="translationName">Name of the translation that will be used to translate all error messages in the map. For the available input values, check the <see cref="TranslationNames"/> property.</param>
        /// <returns>Dictionary where the key is the path, and the value is the list of error messages saved for this path during the validation process.</returns>
        IReadOnlyDictionary<string, IReadOnlyList<string>> GetTranslatedMessageMap(string translationName);

        /// <summary>
        /// Returns friendly printing of all error codes (in the first line) and messages (each one in a separate line, preceded with its paths).
        /// First line contains error codes - comma separated values from <see cref="Codes"/> collection.
        /// Codes are followed with error messages - each message in a separate line, preceded with its path.
        /// </summary>
        /// <param name="translationName">Name of the translation that will be used to translate all error messages in the printing. For the available input values, check the <see cref="TranslationNames"/> property.</param>
        /// <returns>String containing two sections separated with an empty line: error codes (values from <see cref="Codes"/>, in the first line, coma separated) and messages (each one in a separate line, preceded with its path).</returns>
        string ToString(string translationName);
    }
}
