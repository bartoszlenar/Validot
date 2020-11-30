namespace Validot.Factory
{
    using System;

    using Validot.Settings;

    public interface ISettingsHolder
    {
        Func<ValidatorSettings, ValidatorSettings> Settings { get; }
    }
}