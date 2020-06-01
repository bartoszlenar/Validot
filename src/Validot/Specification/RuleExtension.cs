namespace Validot
{
    using System;

    using Validot.Errors.Args;
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class RuleExtension
    {
        public static IRuleOut<T> Rule<T>(this IRuleIn<T> @this, Predicate<T> predicate)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new RuleCommand<T>(predicate));
        }

        public static IRuleOut<T> RuleTemplate<T>(this IRuleIn<T> @this, Predicate<T> predicate, string key, params IArg[] args)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new RuleCommand<T>(predicate, key, args));
        }
    }

    namespace Specification
    {
        public interface IRuleOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithConditionIn<T>,
            IWithPathIn<T>,
            IWithErrorClearedIn<T>,
            IWithMessageIn<T>,
            IWithExtraMessageIn<T>,
            IWithCodeIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IRuleIn<T>
        {
        }

        public partial interface ISpecificationIn<T> : IRuleIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IRuleIn<T>, IRuleOut<T>
        {
        }
    }
}
