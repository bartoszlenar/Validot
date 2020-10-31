namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithCodeExtension
    {
        /// <summary>
        /// Overwrites the entire error output (of the closest preceding scope command) with a single error code.
        /// This is a parameter command - it can be followed by a new scope command or other parameter commands: WithExtraCode.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="code">Error code to be saved in the error output in case the closest preceding scope command indicates invalid value.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IWithCodeOut<T> WithCode<T>(this IWithCodeIn<T> @this, string code)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithCodeCommand(code));
        }

        /// <inheritdoc cref="WithCode{T}(Validot.Specification.IWithCodeIn{T},string)"/>
        public static IWithCodeForbiddenOut<T> WithCode<T>(this IWithCodeForbiddenIn<T> @this, string code)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithCodeCommand(code));
        }
    }

    namespace Specification
    {
        public interface IWithCodeOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWithCodeIn<T>
        {
        }

        public interface IWithCodeForbiddenOut<T> :
            ISpecificationOut<T>,
            IWithExtraCodeForbiddenIn<T>
        {
        }

        public interface IWithCodeForbiddenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithCodeIn<T>, IWithCodeOut<T>, IWithCodeForbiddenIn<T>, IWithCodeForbiddenOut<T>
        {
        }
    }
}
