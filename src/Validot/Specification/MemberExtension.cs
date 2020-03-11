namespace Validot
{
    using System;
    using System.Linq.Expressions;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class MemberExtension
    {
        public static IRuleOut<T> Member<T, TMember>(this IRuleIn<T> @this, Expression<Func<T, TMember>> memberSelector, Specification<TMember> specification)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new MemberCommand<T, TMember>(memberSelector, specification));
        }
    }
}
