namespace Validot;

/// <summary>
/// Holds specification for the models of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the specification that this instance holds.</typeparam>
public interface ISpecificationHolder<T>
{
    /// <summary>
    /// Gets specification for the models of type <typeparamref name="T"/>.
    /// </summary>
    Specification<T> Specification { get; }
}
