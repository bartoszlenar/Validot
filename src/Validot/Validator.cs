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

    public abstract class Validator
    {
        public static IValidatorFactory Factory { get; } = new ValidatorFactory();
    }

    public class Validator<T> : Validator, IValidator<T>
    {
        private readonly IMessagesService _messagesService;

        private readonly ModelScheme<T> _modelScheme;

        public Validator(Specification<T> specification, IValidatorSettings settings = null)
        {
            Settings = (settings ?? ValidatorSettings.GetDefault()).GetVerified();

            _modelScheme = ModelSchemeFactory.Create(specification, Settings.CapacityInfo);
            _messagesService = new MessagesService(Settings.Translations, _modelScheme.ErrorsRegistry, _modelScheme.ErrorsMap);

            ErrorsMap = new ValidationResult(_modelScheme.ErrorsMap.ToDictionary(p => p.Key, p => p.Value.ToList()), _modelScheme.ErrorsRegistry, _messagesService);
        }

        public IValidatorSettings Settings { get; }

        public IValidationResult ErrorsMap { get; }

        public bool IsValid(T model)
        {
            var validationContext = new ValidationContext(_modelScheme, true);

            _modelScheme.RootSpecificationScope.Validate(model, validationContext);

            return validationContext.Errors.Count > 0;
        }

        public IValidationResult Validate(T model, bool failFast = false)
        {
            var validationContext = new ValidationContext(_modelScheme, failFast);

            _modelScheme.RootSpecificationScope.Validate(model, validationContext);

            if (!failFast &&
                Settings.CapacityInfo is IFeedableCapacityInfo feedableCapacityInfo &&
                feedableCapacityInfo.ShouldFeed)
            {
                feedableCapacityInfo.Feed(validationContext);
            }

            return validationContext.Errors is null || validationContext.Errors.Count == 0
                ? ValidationResult.NoErrorsResult
                : new ValidationResult(validationContext.Errors, _modelScheme.ErrorsRegistry, _messagesService);
        }
    }
}
