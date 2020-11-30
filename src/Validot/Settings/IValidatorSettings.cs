namespace Validot.Settings
{
    using System.Collections.Generic;

    public interface IValidatorSettings
    {
        /// <summary>
        /// Gets translations dictionary - the key is the translation name, the value is the translation dictionary (where the key is the message key and the value is error message content for this key).
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; }

        /// <summary>
        /// Gets a value indicating whether reference loop protection is enabled or not. If null, then it will be enabled automatically if the reference loop occurence is theoretically possible (based on the specification).
        /// Reference loop protection is the mechanism that tracks self-references and prevents infinite loop traversing during the validation process.
        /// </summary>
        bool ReferenceLoopProtection { get; }
    }
}