namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithExtraCodeExtension
    {
        /// <summary>
        /// Appends error code to the error output (of the closest preceding scope command).
        /// This is a parameter command - it can be followed by a new scope command or other parameter commands: WithExtraCode.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="code">Error code to be saved in the error output in case the closest preceding scope command indicates invalid value.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IWithExtraCodeOut<T> WithExtraCode<T>(this IWithExtraCodeIn<T> @this, string code)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithExtraCodeCommand(code));
        }

        /// <inheritdoc cref="WithExtraCode{T}(Validot.Specification.IWithExtraCodeIn{T},string)"/>
        public static IWithExtraCodeForbiddenOut<T> WithExtraCode<T>(this IWithExtraCodeForbiddenIn<T> @this, string code)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithExtraCodeCommand(code));
        }
    }

    namespace Specification
    {
        public interface IWithExtraCodeOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWithExtraCodeIn<T>
        {
        }

        public interface IWithExtraCodeForbiddenOut<T> :
            ISpecificationOut<T>,
            IWithExtraCodeForbiddenIn<T>
        {
        }

        public interface IWithExtraCodeForbiddenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithExtraCodeIn<T>, IWithExtraCodeOut<T>, IWithExtraCodeForbiddenIn<T>, IWithExtraCodeForbiddenOut<T>
        {
        }
    }
}
