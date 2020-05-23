namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithPathExtension
    {
        public static IWithPathOut<T> WithPath<T>(this IWithPathIn<T> @this, string path)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithPathCommand(path));
        }
    }

    namespace Specification
    {
        public interface IWithPathOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithErrorClearedIn<T>,
            IWithMessageIn<T>,
            IWithExtraMessageIn<T>,
            IWithCodeIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWithPathIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithPathIn<T>, IWithPathOut<T>
        {
        }
    }
}
