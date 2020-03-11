namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsNullableExtension
    {
        public static IRuleOut<T?> AsNullable<T>(this IRuleIn<T?> @this, Specification<T> specification)
            where T : struct
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T?>)@this).AddCommand(new AsNullableCommand<T>(specification));
        }
    }
}
