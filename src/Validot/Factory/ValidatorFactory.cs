namespace Validot.Factory
{
    using System;

    using Validot.Settings;

    /// <summary>
    /// Creates instances of <see cref="IValidator{T}"/>.
    /// </summary>
    public sealed class ValidatorFactory
    {
        /// <summary>
        /// Creates instance of <see cref="IValidator{T}"/> that can validate objects of type <typeparamref name="T"/> against the provided specification.
        /// </summary>
        /// <param name="specification">Specification used to validate models.</param>
        /// <param name="settings">Settings builder that helps adjust the created <see cref="IValidator{T}"/>'s settings. If not present, the default values are provided.</param>
        /// <typeparam name="T">Type of the models that this instance of <see cref="IValidator{T}"/> can validate.</typeparam>
        /// <returns>Instance of <see cref="IValidator{T}"/>, fully initialized and ready to work.</returns>
        public IValidator<T> Create<T>(Specification<T> specification, Func<ValidatorSettings, ValidatorSettings> settings = null)
        {
            var initSettings = ValidatorSettings.GetDefault();

            if (settings != null)
            {
                initSettings = settings(initSettings);
            }

            return new Validator<T>(specification, initSettings);
        }

        /// <summary>
        /// Creates instance of <see cref="IValidator{T}"/> that can validate objects of type <typeparamref name="T"/> against the specification provided in the <paramref name="specificationHolder"/>.
        /// </summary>
        /// <param name="specificationHolder">Object that provides specification used to validate models (its member <see cref="ISpecificationHolder{T}.Specification"/>) and, optionally, translations (<see cref="ITranslationHolder.Translations"/>, if it implements <see cref="ITranslationHolder"/>.</param>
        /// <param name="settings">Settings builder that helps adjust the created <see cref="IValidator{T}"/>'s settings. If not present, the default values are provided. Overrides translations delivered by <paramref name="specificationHolder"/> if the instance implements also <see cref="ITranslationHolder"/>.</param>
        /// <typeparam name="T">Type of the models that this instance of <see cref="IValidator{T}"/> can validate.</typeparam>
        /// <returns>Instance of <see cref="IValidator{T}"/>, fully initialized and ready to work.</returns>
        public IValidator<T> Create<T>(ISpecificationHolder<T> specificationHolder, Func<ValidatorSettings, ValidatorSettings> settings = null)
        {
            if (specificationHolder is null)
            {
                throw new ArgumentNullException(nameof(specificationHolder));
            }

            var initSettings = ValidatorSettings.GetDefault();

            if (specificationHolder is ITranslationHolder translationHolder)
            {
                _ = initSettings.WithTranslation(translationHolder);
            }

            if (settings != null)
            {
                initSettings = settings(initSettings);
            }

            return new Validator<T>(specificationHolder.Specification, initSettings);
        }
    }
}
