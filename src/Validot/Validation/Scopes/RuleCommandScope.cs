namespace Validot.Validation.Scopes
{
    using System;

    using Validot.Validation.Scopes.Builders;

    internal class RuleCommandScope<T> : ICommandScope<T>
    {
        public Predicate<T> IsValid { get; set; }

        public int ErrorId { get; set; } = -1;

        public ErrorMode ErrorMode { get; set; } = ErrorMode.Override;

        public Predicate<T> ShouldExecute { get; set; }

        public string Name { get; set; }

        int? ICommandScope<T>.ErrorId
        {
            get => ErrorId;
            set
            {
                if (!value.HasValue)
                {
                    throw new InvalidOperationException($"{nameof(RuleCommandScope<T>)} cannot have null {nameof(ErrorId)}");
                }

                ErrorId = value.Value;
            }
        }

        public void Discover(IDiscoveryContext context)
        {
            context.EnterPath(Name);
            context.AddError(ErrorId);
            context.LeavePath();
        }

        public void Validate(T model, IValidationContext context)
        {
            var shouldExecute = ShouldExecute?.Invoke(model) ?? true;

            if (!shouldExecute)
            {
                return;
            }

            context.EnterPath(Name);

            if (!IsValid(model))
            {
                context.AddError(ErrorId);
            }

            context.LeavePath();
        }
    }
}
