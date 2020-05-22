namespace Validot.Validation.Scopes
{
    using System;

    using Validot.Validation.Scopes.Builders;

    internal class RuleCommandScope<T> : ICommandScope<T>
    {
        public Predicate<T> IsValid { get; set; }

        public int ErrorId { get; set; } = -1;

        public ErrorMode ErrorMode { get; set; } = ErrorMode.Override;

        public Predicate<T> ExecutionCondition { get; set; }

        public string Path { get; set; }

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
            context.EnterPath(Path);
            context.AddError(ErrorId);
            context.LeavePath();
        }

        public void Validate(T model, IValidationContext context)
        {
            var shouldExecute = ExecutionCondition?.Invoke(model) ?? true;

            if (!shouldExecute)
            {
                return;
            }

            context.EnterPath(Path);

            if (!IsValid(model))
            {
                context.AddError(ErrorId);
            }

            context.LeavePath();
        }
    }
}
