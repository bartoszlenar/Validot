namespace Validot.Settings
{
    using System.Collections.Generic;

    using Validot.Settings.Capacities;

    public interface IValidatorSettings
    {
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Translations { get; }

        ICapacityInfo CapacityInfo { get; }

        bool InfiniteReferencesLoopProtectionEnabled { get; }
    }
}
