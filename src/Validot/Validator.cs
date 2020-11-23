namespace Validot
{
    using System.Linq;

    using Validot.Errors;
    using Validot.Factory;
    using Validot.Results;
    using Validot.Settings;
    using Validot.Validation;
    using Validot.Validation.Scheme;
    using Validot.Validation.Stacks;

    public abstract class Validator
    {
        /// <summary>
        /// Gets validator factory - the recommended way of creating instances of <see cref="Validator{T}"/>.
        /// </summary>
        public static ValidatorFactory Factory { get; } = new ValidatorFactory();
    }

    /// <inheritdoc cref="IValidator{T}"/>
    public sealed class Validator<T> : Validator, IValidator<T>
    {
        private readonly IMessageService _messageService;

        private readonly ModelScheme<T> _modelScheme;

        private readonly bool _referenceLoopProtectionEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator{T}"/> class.
        /// However, the recommended way is using Validator.Factory.Create instead of this constructor.
        /// </summary>
        /// <param name="specification">Specification used to validate models.</param>
        /// <param name="settings">Settings used to validate models.</param>
        public Validator(Specification<T> specification, ValidatorSettings settings = null)
        {
            Settings = settings ?? ValidatorSettings.GetDefault();

            _modelScheme = ModelSchemeFactory.Create(specification);
            _messageService = new MessageService(Settings.Translations, _modelScheme.ErrorRegistry, _modelScheme.Template);

            Template = new ValidationResult(_modelScheme.Template.ToDictionary(p => p.Key, p => p.Value.ToList()), _modelScheme.ErrorRegistry, _messageService);

            if (_modelScheme.IsReferenceLoopPossible)
            {
                if (Settings.ReferenceLoopProtection.HasValue)
                {
                    _referenceLoopProtectionEnabled = Settings.ReferenceLoopProtection == true;
                }
                else
                {
                    _referenceLoopProtectionEnabled = _modelScheme.IsReferenceLoopPossible;
                }
            }
            else
            {
                _referenceLoopProtectionEnabled = false;
            }

            Settings.IsLocked = true;
        }

        /// <inheritdoc cref="IValidator{T}.Settings"/>
        public ValidatorSettings Settings { get; }

        /// <inheritdoc cref="IValidator{T}.Template"/>
        public IValidationResult Template { get; }

        /// <inheritdoc cref="IValidator{T}.IsValid"/>
        public bool IsValid(T model)
        {
            var validationContext = new IsValidValidationContext(_modelScheme, _referenceLoopProtectionEnabled ? new ReferenceLoopProtectionSettings(model) : null);

            _modelScheme.RootSpecificationScope.Validate(model, validationContext);

            return !validationContext.ErrorFound;
        }

        /// <inheritdoc cref="IValidator{T}.Validate"/>
        public IValidationResult Validate(T model, bool failFast = false)
        {
            var validationContext = new ValidationContext(_modelScheme, failFast, _referenceLoopProtectionEnabled ? new ReferenceLoopProtectionSettings(model) : null);

            _modelScheme.RootSpecificationScope.Validate(model, validationContext);

            var isValid = validationContext.Errors is null;

            return isValid
                ? ValidationResult.NoErrorsResult
                : new ValidationResult(validationContext.Errors, _modelScheme.ErrorRegistry, _messageService);
        }
    }
}
