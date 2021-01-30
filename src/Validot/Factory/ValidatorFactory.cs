namespace Validot.Factory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Validot.Settings;
    using Validot.Validation.Scheme;

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
            var resolvedSettings = GetResolvedSettings(ValidatorSettings.GetDefault(), settings);

            var modelScheme = ModelSchemeFactory.Create(specification);

            SetReferenceLoopProtection(resolvedSettings, modelScheme.IsReferenceLoopPossible);

            return Create(specification, resolvedSettings);
        }

        /// <summary>
        /// Creates instance of <see cref="IValidator{T}"/> that can validate objects of type <typeparamref name="T"/> against the specification provided in the <paramref name="specificationHolder"/>.
        /// </summary>
        /// <param name="specificationHolder">Object that provides specification used to validate models (its member <see cref="ISpecificationHolder{T}.Specification"/>) and, optionally, settings (<see cref="ISettingsHolder.Settings"/>, if it implements also <see cref="ISettingsHolder"/>.</param>
        /// <param name="settings">Settings builder that helps adjust the created <see cref="IValidator{T}"/>'s settings. If not present, the default values are provided. Overrides translations delivered by <paramref name="specificationHolder"/>, if it implements also <see cref="ISettingsHolder"/>.</param>
        /// <typeparam name="T">Type of the models that this instance of <see cref="IValidator{T}"/> can validate.</typeparam>
        /// <returns>Instance of <see cref="IValidator{T}"/>, fully initialized and ready to work.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="specificationHolder"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="specificationHolder"/>'s <see cref="ISettingsHolder.Settings"/> is null.</exception>
        public IValidator<T> Create<T>(ISpecificationHolder<T> specificationHolder, Func<ValidatorSettings, ValidatorSettings> settings = null)
        {
            if (specificationHolder is null)
            {
                throw new ArgumentNullException(nameof(specificationHolder));
            }

            var validatorSettings = ValidatorSettings.GetDefault();

            if (specificationHolder is ISettingsHolder validatorSettingsHolder)
            {
                if (validatorSettingsHolder.Settings is null)
                {
                    throw new ArgumentException($"{nameof(ISettingsHolder)} can't have null {nameof(ISettingsHolder.Settings)}", nameof(specificationHolder));
                }

                validatorSettings = GetResolvedSettings(validatorSettings, validatorSettingsHolder.Settings);
            }

            validatorSettings = GetResolvedSettings(validatorSettings, settings);

            var modelScheme = ModelSchemeFactory.Create(specificationHolder.Specification);

            SetReferenceLoopProtection(validatorSettings, modelScheme.IsReferenceLoopPossible);

            return Create(specificationHolder.Specification, validatorSettings);
        }

        /// <summary>
        /// Creates instance of <see cref="IValidator{T}"/> that can validate objects of type <typeparamref name="T"/> against the provided specification.
        /// </summary>
        /// <param name="specification">Specification used to validate models.</param>
        /// <param name="settings">Settings used to validate models.</param>
        /// <typeparam name="T">Type of the models that this instance of <see cref="IValidator{T}"/> can validate.</typeparam>
        /// <returns>Instance of <see cref="IValidator{T}"/>, fully initialized and ready to work.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="settings"/> is not an instance of <see cref="ValidatorSettings"/>.</exception>
        public IValidator<T> Create<T>(Specification<T> specification, IValidatorSettings settings)
        {
            var modelScheme = ModelSchemeFactory.Create(specification);

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (!(settings is ValidatorSettings validatorSettings))
            {
                throw new ArgumentException($"Custom {nameof(IValidatorSettings)} implementations are not supported.", nameof(settings));
            }

            validatorSettings.IsLocked = true;

            return new Validator<T>(modelScheme, settings);
        }

        /// <summary>
        /// Fetches information about the specification holders contained in the given assemblies and provides the way to create the validators out of them. Helps with populating IoC/DI containers almost automatically, or with very limited effort.
        /// </summary>
        /// <param name="assemblies">Assemblies to scan for specification holders. Must not be empty. If you don't know what to do, try passing AppDomain.CurrentDomain.GetAssemblies().</param>
        /// <returns>Collection of items containing information about the specification holders found in provided assembly and a method to create the validators out of them.</returns>
        public IReadOnlyList<HolderInfo> FetchHolders(params Assembly[] assemblies)
        {
            ThrowHelper.NullArgument(assemblies, nameof(assemblies));
            ThrowHelper.NullInCollection(assemblies, nameof(assemblies));

            if (assemblies.Length == 0)
            {
                throw new ArgumentException("Assembly collection must not be empty", nameof(assemblies));
            }

            var holders = new List<HolderInfo>();

            var holderTypes = assemblies.SelectMany(GetAllSpecificationHoldersFromAssembly).ToList();

            foreach (var holderType in holderTypes)
            {
                var specificationHolderTypes = holderType.GetInterfaces().Where(IsSpecificationHolderInterface).ToList();

                foreach (var specificationHolderType in specificationHolderTypes)
                {
                    var specifiedType = specificationHolderType.GetGenericArguments().Single();

                    holders.Add(new HolderInfo(holderType, specifiedType));
                }
            }

            return holders;

            IReadOnlyList<Type> GetAllSpecificationHoldersFromAssembly(Assembly assembly)
            {
                return assembly
                    .GetTypes()
                    .Where(type => type.IsClass &&
                                   type.GetConstructor(Type.EmptyTypes) != null &&
                                   type.GetInterfaces().Any(IsSpecificationHolderInterface))
                    .ToArray();
            }

            bool IsSpecificationHolderInterface(Type @interface)
            {
                return @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ISpecificationHolder<>).GetGenericTypeDefinition();
            }
        }

        private static void SetReferenceLoopProtection(ValidatorSettings settings, bool isReferenceLoopPossible)
        {
            if (!settings.ReferenceLoopProtectionEnabled.HasValue)
            {
                _ = isReferenceLoopPossible
                    ? settings.WithReferenceLoopProtection()
                    : settings.WithReferenceLoopProtectionDisabled();
            }
        }

        private static ValidatorSettings GetResolvedSettings(ValidatorSettings initSettings, Func<ValidatorSettings, ValidatorSettings> settingsBuilder)
        {
            var resolvedSettings = settingsBuilder is null
                ? initSettings
                : settingsBuilder(initSettings);

            if (!ReferenceEquals(initSettings, resolvedSettings))
            {
                throw new InvalidOperationException("Validator settings fluent API should return the same reference as received.");
            }

            return resolvedSettings;
        }
    }
}
