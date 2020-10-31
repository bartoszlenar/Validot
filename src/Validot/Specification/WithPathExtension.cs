namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithPathExtension
    {
        /// <summary>
        /// Overwrites the path of the current scope's error output.
        /// This is a parameter command - it can be followed by a new scope command or other parameter commands: WithMessage, WithExtraMessage, WithCode, WithExtraCode.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="path">New path of the current scope's error output. To nest it more deeply, simply use dots (e.g., "More.Nested.Level"). To move it to the upper level, use `&lt;` character (e.g., "&lt;", "&lt;&lt;TwoLevelsUp.And.ThreeDown").</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
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
            IWithMessageIn<T>,
            IWithExtraMessageIn<T>,
            IWithCodeIn<T>,
            IWithExtraCodeIn<T>,
            IAndIn<T>
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
