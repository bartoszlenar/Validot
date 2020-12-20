namespace Validot.Factory
{
    using System;
    using System.Linq;

    using Validot.Settings;

    /// <summary>
    /// Information about the class that implements <see cref="ISpecificationHolder{T}"/> (and, optionally, <see cref="ISettingsHolder"/>).
    /// </summary>
    public class HolderInfo
    {
        internal HolderInfo(Type holderType, Type specifiedType)
        {
            if (holderType is null)
            {
                throw new ArgumentNullException(nameof(holderType));
            }

            if (specifiedType is null)
            {
                throw new ArgumentNullException(nameof(specifiedType));
            }

            var hasParameterlessConstructor = holderType.IsClass && holderType.GetConstructor(Type.EmptyTypes) != null;

            if (!hasParameterlessConstructor)
            {
                throw new ArgumentException($"{holderType.GetFriendlyName()} must have parameterless constructor.", nameof(holderType));
            }

            if (holderType.GetInterfaces().All(i => i != typeof(ISpecificationHolder<>).MakeGenericType(specifiedType)))
            {
                throw new ArgumentException($"{holderType.GetFriendlyName()} is not a holder for {specifiedType.GetFriendlyName()} specification (doesn't implement ISpecificationHolder<{specifiedType.GetFriendlyName()}>).", nameof(holderType));
            }

            HolderType = holderType;
            SpecifiedType = specifiedType;

            HoldsSettings = holderType.GetInterfaces().Any(@interface => @interface == typeof(ISettingsHolder));
            ValidatorType = typeof(IValidator<>).MakeGenericType(SpecifiedType);
        }

        /// <summary>
        /// Gets the type of the specification holder.
        /// </summary>
        public Type HolderType { get; }

        /// <summary>
        /// Gets the type that is covered by the specification. It's T from ISpecificationHolder{T} and its member Specification{T}.
        /// </summary>
        public Type SpecifiedType { get; }

        /// <summary>
        /// Gets the type of the validator. It's IValidator{T}, where T is <see cref="SpecifiedType"/>.
        /// <seealso cref="IValidator{T}"/>
        /// </summary>
        public Type ValidatorType { get; }

        /// <summary>
        /// Gets a value indicating whether the specification holder is also a settings holder (implements <see cref="ISettingsHolder"/>).
        /// </summary>
        public bool HoldsSettings { get; }

        /// <summary>
        /// Creates the validator (of type IValidator{T}, where T is <see cref="SpecifiedType"/>) using the information from specification holder.
        /// </summary>
        /// <returns>IValidator{T} where T is <see cref="SpecifiedType"/>.</returns>
        public object CreateValidator()
        {
            var holderInstance = Activator.CreateInstance(HolderType);
            var holderInterfaceType = typeof(ISpecificationHolder<>).MakeGenericType(SpecifiedType);

            var specificationType = typeof(Specification<>).MakeGenericType(SpecifiedType);

            var specificationPropertyInfo = holderInterfaceType.GetProperty(nameof(ISpecificationHolder<object>.Specification), specificationType);

            var specification = specificationPropertyInfo.GetValue(holderInstance);

            Func<ValidatorSettings, ValidatorSettings> settingsBuilder;

            if (HoldsSettings)
            {
                var settingsPropertyInfo = typeof(ISettingsHolder).GetProperty(nameof(ISettingsHolder.Settings));

                settingsBuilder = settingsPropertyInfo?.GetValue(holderInstance) as Func<ValidatorSettings, ValidatorSettings>;
            }
            else
            {
                settingsBuilder = null;
            }

            var createArgs = new[]
            {
                specification,
                settingsBuilder
            };

            var createMethodInfo = typeof(ValidatorFactory).GetMethods()
                .Single(m =>
                    m.Name == nameof(ValidatorFactory.Create) &&
                    m.IsGenericMethod &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Specification<>).GetGenericTypeDefinition() &&
                    m.GetParameters()[1].ParameterType == typeof(Func<ValidatorSettings, ValidatorSettings>))
                .MakeGenericMethod(SpecifiedType);

            var validator = createMethodInfo.Invoke(Validator.Factory, createArgs);

            return validator;
        }
    }
}