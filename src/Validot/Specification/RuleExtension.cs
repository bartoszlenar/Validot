namespace Validot
{
    using System;

    using Validot.Errors.Args;
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class RuleExtension
    {
        /// <summary>
        /// Validates the current scope value with a custom logic wrapped in the predicate. Error output is saved in the current scope.
        /// To specify the error output (messages, codes, etc.), follow this commands with parameters commands like WithMessage, WithExtraMessage, WithCode, WithExtraCode.
        /// This is a scope command - its error output can be altered with any of the parameter commands (WithCondition, WithPath, WithMessage, WithExtraMessage, WithCode, WithExtraCode).
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="predicate">Predicate that takes the current scope value and returns true if its valid, false - if invalid.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IRuleOut<T> Rule<T>(this IRuleIn<T> @this, Predicate<T> predicate)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new RuleCommand<T>(predicate));
        }

        /// <summary>
        /// This command shouldn't be used in the specification. It's primarily for creating predefined rules.
        /// Find "Custom Rules" section in the documentation.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="predicate">Predicate that takes the current scope value and returns true if its valid, false - if invalid.</param>
        /// <param name="key">Message key (same meaning as in <see cref="WithMessageExtension.WithMessage{T}(Validot.Specification.IWithMessageIn{T},string)"/>).</param>
        /// <param name="args">Message args (same meaning as in <see cref="WithMessageExtension.WithMessage{T}(Validot.Specification.IWithMessageIn{T},string)"/>).</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
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
            IWithMessageIn<T>,
            IWithExtraMessageIn<T>,
            IWithCodeIn<T>,
            IWithExtraCodeIn<T>,
            IAndIn<T>
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
