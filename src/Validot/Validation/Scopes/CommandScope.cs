namespace Validot.Validation.Scopes
{
    using System;

    using Validot.Validation.Scopes.Builders;

    internal abstract class CommandScope<T> : ICommandScope<T>
    {
        public Predicate<T> ExecutionCondition { get; set; }

        public int? ErrorId { get; set; }

        public ErrorMode ErrorMode { get; set; } = ErrorMode.Append;

        public string Name { get; set; }

        public void Discover(IDiscoveryContext context)
        {
            context.EnterPath(Name);

            if (!ErrorId.HasValue || ErrorMode == ErrorMode.Append)
            {
                RunDiscovery(context);
            }

            if (ErrorId.HasValue)
            {
                context.AddError(ErrorId.Value);
            }

            context.LeavePath();
        }

        public void Validate(T model, IValidationContext context)
        {
            var shouldExecute = ExecutionCondition?.Invoke(model) ?? true;

            if (!shouldExecute)
            {
                return;
            }

            context.EnterPath(Name);

            if (ErrorId.HasValue)
            {
                context.EnableErrorDetectionMode(ErrorMode, ErrorId.Value);
            }

            RunValidation(model, context);

            context.LeavePath();
        }

        protected abstract void RunValidation(T model, IValidationContext context);

        protected abstract void RunDiscovery(IDiscoveryContext context);
    }
}
