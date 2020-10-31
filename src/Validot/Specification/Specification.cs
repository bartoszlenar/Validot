namespace Validot
{
    using Validot.Specification;

    /// <summary>
    /// Specification describes the valid state of an object using fluent interface.
    /// </summary>
    /// <param name="api">Fluent API builder - input.</param>
    /// <typeparam name="T">Type of the specified model.</typeparam>
    /// <returns>Fluent API builder - output.</returns>
    public delegate ISpecificationOut<T> Specification<T>(ISpecificationIn<T> api);
}
