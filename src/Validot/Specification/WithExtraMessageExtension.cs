namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithExtraMessageExtension
    {
        /// <summary>
        /// Appends error message to the error output (of the closest preceding scope command).
        /// This is a parameter command - it can be followed by a new scope command or other parameter commands: WithExtraMessage, WithExtraCode.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="message">Error message to be saved in the error output in case the closest preceding scope command indicates invalid value. This is also a message key that could be replaced by the translation dictionary.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IWithExtraMessageOut<T> WithExtraMessage<T>(this IWithExtraMessageIn<T> @this, string message)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithExtraMessageCommand(message));
        }

        /// <inheritdoc cref="WithExtraMessage{T}(Validot.Specification.IWithExtraMessageIn{T},string)"/>
        public static IWithExtraMessageForbiddenOut<T> WithExtraMessage<T>(this IWithExtraMessageForbiddenIn<T> @this, string message)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithExtraMessageCommand(message));
        }
    }

    namespace Specification
    {
        public interface IWithExtraMessageOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithExtraMessageIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWithExtraMessageIn<T>
        {
        }

        public interface IWithExtraMessageForbiddenOut<T> :
            ISpecificationOut<T>,
            IWithExtraMessageForbiddenIn<T>,
            IWithExtraCodeForbiddenIn<T>
        {
        }

        public interface IWithExtraMessageForbiddenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithExtraMessageIn<T>, IWithExtraMessageOut<T>, IWithExtraMessageForbiddenIn<T>, IWithExtraMessageForbiddenOut<T>
        {
        }
    }
}
