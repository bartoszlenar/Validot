namespace Validot
{
    using System.Linq;

    using Validot.Errors;
    using Validot.Factory;
    using Validot.Results;
    using Validot.Settings;
    using Validot.Settings.Capacities;
    using Validot.Validation;
    using Validot.Validation.Scheme;
    using Validot.Validation.Stacks;

    public abstract class Validator
    {
        public static ValidatorFactory Factory { get; } = new ValidatorFactory();
    }

    public sealed class Validator<T> : Validator, IValidator<T>
    {
        private readonly IMessageService _messageService;

        private readonly ModelScheme<T> _modelScheme;

        private readonly bool _referenceLoopProtectionEnabled;

        public Validator(Specification<T> specification, ValidatorSettings settings = null)
        {
            Settings = settings ?? ValidatorSettings.GetDefault();

            _modelScheme = ModelSchemeFactory.Create(specification, Settings.CapacityInfo);
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

        public ValidatorSettings Settings { get; }

        public IValidationResult Template { get; }

        public bool IsValid(T model)
        {
            var validationContext = new ValidationContext(_modelScheme, true, _referenceLoopProtectionEnabled ? new ReferenceLoopProtectionSettings(model) : null);

            _modelScheme.RootSpecificationScope.Validate(model, validationContext);

            return validationContext.Errors is null;
        }

        public IValidationResult Validate(T model, bool failFast = false)
        {
            var validationContext = new ValidationContext(_modelScheme, failFast, _referenceLoopProtectionEnabled ? new ReferenceLoopProtectionSettings(model) : null);

            _modelScheme.RootSpecificationScope.Validate(model, validationContext);

            var isValid = validationContext.Errors is null;

            if (!isValid &&
                !failFast &&
                Settings.CapacityInfo is IFeedableCapacityInfo feedableCapacityInfo &&
                feedableCapacityInfo.ShouldFeed)
            {
                feedableCapacityInfo.Feed(validationContext);
            }

            return isValid
                ? ValidationResult.NoErrorsResult
                : new ValidationResult(validationContext.Errors, _modelScheme.ErrorRegistry, _messageService);
        }
    }
}
