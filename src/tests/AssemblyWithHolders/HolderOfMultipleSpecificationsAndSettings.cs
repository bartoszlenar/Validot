namespace AssemblyWithHolders
{
    using System;

    using Validot;
    using Validot.Factory;
    using Validot.Settings;

    public class HolderOfMultipleSpecificationsAndSettings : ISpecificationHolder<float>, ISpecificationHolder<double>, ISettingsHolder
    {
        Specification<float> ISpecificationHolder<float>.Specification => s => s
            .GreaterThan(1).WithMessage("Min value is 1")
            .LessThan(10).WithMessage("Max value is 10");

        Specification<double> ISpecificationHolder<double>.Specification => s => s
            .GreaterThan(1).WithMessage("Min value is 1")
            .LessThan(10).WithMessage("Max value is 10");

        public Func<ValidatorSettings, ValidatorSettings> Settings { get; } = s => s
            .WithTranslation("English", "Min value is 1", "Minimum value is 1")
            .WithTranslation("English", "Max value is 10", "Maximum value is 10");
    }
}