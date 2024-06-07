namespace AssemblyWithHolders
{
    using System;

    using Validot;
    using Validot.Factory;
    using Validot.Settings;

    public class HolderOfIntSpecificationAndSettings : ISpecificationHolder<int>, ISettingsHolder
    {
        public Specification<int> Specification { get; } = s => s
            .GreaterThanOrEqualTo(1).WithMessage("Min value is 1")
            .LessThanOrEqualTo(10).WithMessage("Max value is 10");

        public Func<ValidatorSettings, ValidatorSettings> Settings { get; } = s => s
            .WithTranslation("English", "Min value is 1", "The minimum value is 1")
            .WithTranslation("English", "Max value is 10", "The maximum value is 10")
            .WithTranslation("BinaryEnglish", "Min value is 1", "The minimum value is 0b0001")
            .WithTranslation("BinaryEnglish", "Max value is 10", "The maximum value is 0b1010")
            .WithReferenceLoopProtection();

    }
}