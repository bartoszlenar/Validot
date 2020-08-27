namespace Validot.Factory
{
    using System;

    using Validot.Settings;

    public sealed class ValidatorFactory
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
            if (specificationHolder is null)
            {
                throw new ArgumentNullException(nameof(specificationHolder));
            }

            var initSettings = ValidatorSettings.GetDefault();

            if (specificationHolder is ITranslationHolder translationHolder)
            {
                initSettings.WithTranslation(translationHolder);
            }

            if (settings != null)
            {
                initSettings = settings(initSettings);
            }

            return new Validator<T>(specificationHolder.Specification, initSettings);
        }
    }
}
