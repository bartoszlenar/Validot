namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithMessageExtension
    {
        /// <summary>
        /// Overwrites the entire error output (of the closest preceding scope command) with a single error message.
        /// This is a parameter command - it can be followed by a new scope command or other parameter commands: WithExtraMessage, WithExtraCode.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="message">Error message to be saved in the error output in case the closest preceding scope command indicates invalid value. This is also a message key that could be replaced by the translation dictionary.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IWithMessageOut<T> WithMessage<T>(this IWithMessageIn<T> @this, string message)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithMessageCommand(message));
        }

        /// <inheritdoc cref="WithMessage{T}(Validot.Specification.IWithMessageIn{T},string)"/>
        public static IWithMessageForbiddenOut<T> WithMessage<T>(this IWithMessageForbiddenIn<T> @this, string message)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithMessageCommand(message));
        }
    }

    namespace Specification
    {
        public interface IWithMessageOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithExtraMessageIn<T>,
            IWithExtraCodeIn<T>,
            IAndIn<T>
        {
        }

        public interface IWithMessageIn<T>
        {
        }

        public interface IWithMessageForbiddenOut<T> :
            ISpecificationOut<T>,
            IWithExtraMessageForbiddenIn<T>,
            IWithExtraCodeForbiddenIn<T>
        {
        }

        public interface IWithMessageForbiddenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithMessageIn<T>, IWithMessageOut<T>, IWithMessageForbiddenIn<T>, IWithMessageForbiddenOut<T>
        {
        }
    }
}
