namespace Validot.Factory
{
    using System;

    using Validot.Settings;

    public interface IValidatorFactory
    {
        IValidator<T> Create<T>(Specification<T> specification, Func<ValidatorSettings, ValidatorSettings> settings = null);

        IValidator<T> Create<T>(ISpecificationHolder<T> specificationHolder, Func<ValidatorSettings, ValidatorSettings> settings = null);
    }
}
