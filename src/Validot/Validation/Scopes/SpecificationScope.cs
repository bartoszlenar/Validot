namespace Validot.Validation.Scopes
{
    using System.Collections.Generic;

    internal class SpecificationScope<T> : ISpecificationScope<T>
    {
        private static readonly bool IsNullable = default(T) == null;

        public IReadOnlyList<ICommandScope<T>> CommandScopes { get; set; }

        public Presence Presence { get; set; }

        public int ForbiddenErrorId { get; set; }

        public int RequiredErrorId { get; set; }

        public void Discover(IDiscoveryContext context)
        {
            if (IsNullable)
            {
                if (Presence == Presence.Forbidden)
                {
                    context.AddError(ForbiddenErrorId, true);

                    return;
                }

                if (Presence == Presence.Required)
                {
                    context.AddError(RequiredErrorId, true);
                }
            }

            for (var i = 0; i < CommandScopes.Count; ++i)
            {
                CommandScopes[i].Discover(context);
            }
        }

        public void Validate(T model, IValidationContext context)
        {
            if (IsNullable)
            {
                if (model == null)
                {
                    if (Presence == Presence.Required)
                    {
                        context.AddError(RequiredErrorId, true);
                    }

                    return;
                }

                if (Presence == Presence.Forbidden)
                {
                    context.AddError(ForbiddenErrorId, true);

                    return;
                }
            }

            for (var i = 0; i < CommandScopes.Count; ++i)
            {
                CommandScopes[i].Validate(model, context);

                if (context.ShouldFallBack)
                {
                    return;
                }
            }
        }
    }
}
