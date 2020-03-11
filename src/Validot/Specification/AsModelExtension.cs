namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsModelExtension
    {
        public static IRuleOut<T> AsModel<T>(this IRuleIn<T> @this, Specification<T> specification)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new AsModelCommand<T>(specification));
        }
    }
}
