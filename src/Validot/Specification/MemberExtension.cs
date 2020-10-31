namespace Validot
{
    using System;
    using System.Linq.Expressions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class MemberExtension
    {
        /// <summary>
        /// Validates the current scope value's member against the specification. The error output is saved under the path derived from the member's name.
        /// This is a scope command - its error output can be altered with any of the parameter commands (WithCondition, WithPath, WithMessage, WithExtraMessage, WithCode, WithExtraCode).
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="memberSelector">Expression that points at the current scope's direct member. Only one step down is allowed.</param>
        /// <param name="specification">Specification for the current scope value's member.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <typeparam name="TMember">Type of the current scope value's member.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IRuleOut<T> Member<T, TMember>(this IRuleIn<T> @this, Expression<Func<T, TMember>> memberSelector, Specification<TMember> specification)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new MemberCommand<T, TMember>(memberSelector, specification));
        }
    }
}
