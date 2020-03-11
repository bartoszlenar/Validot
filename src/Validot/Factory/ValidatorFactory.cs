namespace Validot.Factory
{
    using System;

    using Validot.Settings;

    internal class ValidatorFactory : IValidatorFactory
    {
        public IValidator<T> Create<T>(Specification<T> specification, Func<ValidatorSettings, ValidatorSettings> settings = null)
        {
            var initSettings = ValidatorSettings.GetDefault();

            if (settings != null)
            {
                initSettings = settings(initSettings);
            }

            return new Validator<T>(specification, initSettings);
        }

        public IValidator<T> Create<T>(ISpecificationHolder<T> specificationHolder, Func<ValidatorSettings, ValidatorSettings> settings = null)
        {
            var initSettings = ValidatorSettings.GetDefault();

            if (specificationHolder is ITranslationsHolder translationsHolder)
            {
                initSettings.WithTranslation(translationsHolder);
            }

            if (settings != null)
            {
                initSettings = settings(initSettings);
            }

            return new Validator<T>(specificationHolder.Specification, initSettings);
        }
    }
}
