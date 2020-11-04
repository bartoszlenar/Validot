namespace Validot
{
    using Validot.Specification;

    public static class AndExtension
    {
        /// <summary>
        /// Contains no validation logic and no meaning. It's purpose is to visually separate rules in the fluent API method chain.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <typeparam name="T">Type of the specified model.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IAndOut<T> And<T>(this IAndIn<T> @this)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return (SpecificationApi<T>)@this;
        }
    }

    namespace Specification
    {
        public interface IAndOut<T> :
            IRuleIn<T>
        {
        }

        public interface IAndIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IAndIn<T>, IAndOut<T>
        {
        }
    }
}
