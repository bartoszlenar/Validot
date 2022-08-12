namespace Validot
{
    using System;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsConvertedExtension
    {
        public static IRuleOut<T> AsConverted<T, TTarget>(this IRuleIn<T> @this, Converter<T, TTarget> convert, Specification<TTarget> specification)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            ThrowHelper.NullArgument(convert, nameof(convert));

            return ((SpecificationApi<T>)@this).AddCommand(new AsConvertedCommand<T, TTarget>(convert, specification));
        }
    }
}