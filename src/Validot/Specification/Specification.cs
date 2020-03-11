namespace Validot
{
    using Validot.Specification;

    public delegate ISpecificationOut<T> Specification<T>(ISpecificationIn<T> api);
}
