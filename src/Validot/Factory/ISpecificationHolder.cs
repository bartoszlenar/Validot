namespace Validot
{
    public interface ISpecificationHolder<T>
    {
        Specification<T> Specification { get; }
    }
}
