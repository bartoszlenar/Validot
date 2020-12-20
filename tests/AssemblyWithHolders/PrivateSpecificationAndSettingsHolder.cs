namespace AssemblyWithHolders
{
    using System;

    using Validot;
    using Validot.Factory;
    using Validot.Settings;

    internal class PrivateSpecificationAndSettingsHolder : ISpecificationHolder<string>, ISettingsHolder
    {
        public Specification<string> Specification { get; } = s => s.NotEmpty();

        public Func<ValidatorSettings, ValidatorSettings> Settings { get; } = s => s.WithReferenceLoopProtection();
    }
}