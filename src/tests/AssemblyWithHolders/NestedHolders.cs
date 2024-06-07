// unset

namespace AssemblyWithHolders
{
    using System;

    using Validot;
    using Validot.Factory;
    using Validot.Settings;

    public class NestedHolders
    {
        public class NestedHolderOfBoolSpecification : ISpecificationHolder<bool>
        {
            public Specification<bool> Specification { get; } = s => s.True().WithMessage("Must be true");
        }
        
        public class NestedHolderOfStringSpecification : ISpecificationHolder<string>
        {
            public Specification<string> Specification { get; } = s => s.NotEmpty();
        }
        
        public class NestedHolderOfStringSpecificationAndSettings : ISpecificationHolder<string>, ISettingsHolder
        {
            public Specification<string> Specification { get; } = s => s.NotEmpty();

            public Func<ValidatorSettings, ValidatorSettings> Settings { get; } = s => s.WithTranslation("English", "Texts.NotEmpty", "Cannot be empty!");
        }
    }
}