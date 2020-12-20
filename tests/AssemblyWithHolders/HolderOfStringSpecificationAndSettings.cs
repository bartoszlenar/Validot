namespace AssemblyWithHolders
{
    using System;

    using Validot;
    using Validot.Factory;
    using Validot.Settings;

    public class HolderOfStringSpecificationAndSettings : ISpecificationHolder<string>, ISettingsHolder
    {
        public Specification<string> Specification { get; } = s => s
            .NotEmpty().WithMessage("Empty text not allowed")
            .MinLength(3).WithMessage("Text shorter than 3 characters not allowed")
            .MaxLength(10).WithMessage("Text longer than 10 characters not allowed")
            .NotContains("!").WithMessage("Text containing exclamation mark not allowed");

        public Func<ValidatorSettings, ValidatorSettings> Settings { get; } = s => s
            .WithReferenceLoopProtection()
            .WithTranslation("English", "Empty text not allowed", "Empty string is invalid!")
            .WithTranslation("English", "Text shorter than 3 characters not allowed", "Only strings of length from 3 to 10 are allowed")
            .WithTranslation("English", "Text longer than 10 characters not allowed", "Only strings of length from 3 to 10 are allowed");
    }
}